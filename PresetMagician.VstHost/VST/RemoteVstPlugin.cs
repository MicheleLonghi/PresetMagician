using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using Catel.IO;
using Drachenkatze.PresetMagician.Utils;
using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;
using Microsoft.DwayneNeed.Win32;
using Microsoft.DwayneNeed.Win32.User32;
using PresetMagician.Models;
using PresetMagician.VstHost.Util;
using Brushes = System.Windows.Media.Brushes;

namespace PresetMagician.VstHost.VST
{
    public class RemoteVstPlugin
    {
        private string _dllPath;
        private Timer _shutdownTimer;
        private VstHost _host;
        public bool BackgroundProcessing { get; set; }

        public RemoteVstPlugin(VstHost host)
        {
            _host = host;
        }

        public string DllPath
        {
            get { return _dllPath; }
            set
            {
                DllFilename = Path.GetFileName(value);
                _dllPath = value;
            }
        }

        public string DllFilename { get; set; } = "";
        public VstPluginContext PluginContext { get; set; }
        public bool IsEditorOpen { get; set; }

        private Window _editorWindow;
        private WindowInteropHelper _editorWindowHelper;

        private const int DWMWA_TRANSITIONS_FORCEDISABLED = 3;

        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hWnd, int attr, ref int value, int attrLen);

        public bool OpenEditorHidden()
        {
            if (!PluginContext.PluginCommandStub.EditorGetRect(out Rectangle wndRect))
            {
                return false;
            }

            if (wndRect.Width == 0 || wndRect.Height == 0)
            {
                return false;
            }

            _editorWindow = new PluginWindow //make sure the window is invisible
            {
                Width = wndRect.Width,
                Height = wndRect.Height,
                WindowStyle = WindowStyle.None,
                ShowInTaskbar = false,
                ShowActivated = false,
                Left = 0,
                Top = 0,
                ResizeMode = ResizeMode.NoResize,
                Margin = new Thickness(0),
                SnapsToDevicePixels = true,
                UseLayoutRounding = false,
                SizeToContent = SizeToContent.Manual,
                AllowsTransparency = true,
                Opacity = 0,
                Background = Brushes.Transparent
            };

            _editorWindowHelper = new WindowInteropHelper(_editorWindow);
            _editorWindowHelper.EnsureHandle();

            _editorWindow.Show();


            int value = 1; // TRUE to disable
            DwmSetWindowAttribute(_editorWindowHelper.Handle,
                DWMWA_TRANSITIONS_FORCEDISABLED,
                ref value,
                Marshal.SizeOf(value));

            var success = PluginContext.PluginCommandStub.EditorOpen(_editorWindowHelper.Handle);
            var handle = new HWND(_editorWindowHelper.Handle);
            NativeMethods.SetWindowPos(
                handle,
                HWND.BOTTOM,
                0,
                0,
                0,
                0,
                SWP.NOMOVE | SWP.NOSIZE);

            if (!success)
            {
                _editorWindow.Close();
                return false;
            }

            if (PluginContext.PluginCommandStub.EditorGetRect(out Rectangle wndRect2))
            {
                if (wndRect2.Width != 0 && wndRect2.Height != 0)
                {
                    _editorWindow.Width = wndRect2.Width;
                    _editorWindow.Height = wndRect2.Height;
                }
            }

            IsEditorOpen = true;
            return true;
        }

        public bool OpenEditor()
        {
            if (!PluginContext.PluginCommandStub.EditorGetRect(out Rectangle wndRect))
            {
                return false;
            }

            if (wndRect.Width == 0 || wndRect.Height == 0)
            {
                return false;
            }

            _editorWindow = new PluginWindow(wndRect.Width, wndRect.Height) //make sure the window is invisible
            {
                ShowInTaskbar = true,
                ShowActivated = true,
                Title = "Plugin Editor: " + PluginContext.PluginCommandStub.GetEffectName(),
                ResizeMode = ResizeMode.NoResize,
                Margin = new Thickness(0),
                SnapsToDevicePixels = true,
                UseLayoutRounding = false,
                Topmost = true,
                SizeToContent = SizeToContent.WidthAndHeight,
            };

            _editorWindowHelper = new WindowInteropHelper(_editorWindow);
            _editorWindowHelper.EnsureHandle();

            _editorWindow.Show();

            var success = PluginContext.PluginCommandStub.EditorOpen(_editorWindowHelper.Handle);

            if (!success)
            {
                _editorWindow.Close();
                return false;
            }

            if (PluginContext.PluginCommandStub.EditorGetRect(out Rectangle wndRect2))
            {
                if (wndRect2.Width != 0 && wndRect2.Height != 0)
                {
                    _editorWindow.Width = wndRect2.Width;
                    _editorWindow.Height = wndRect2.Height;
                }
            }

            IsEditorOpen = true;
            return true;
        }

        public void ResizeEditor(int width, int height)
        {
            if (IsEditorOpen && width != 0 && height != 0)
            {
                _editorWindow.Dispatcher.Invoke(DispatcherPriority.Render, new Action(() =>
                {
                    _editorWindow.Width = width;
                    _editorWindow.Height = height;
                }));
            }
        }

        public void RedrawEditor()
        {
            if (IsEditorOpen)
            {
                _editorWindow.Dispatcher.BeginInvoke(DispatcherPriority.Render,
                    new Action(() => { _editorWindow.InvalidateVisual(); }));
            }
        }

        public Bitmap CreateScreenshot()
        {
            return !IsEditorOpen ? null : ScreenCapture.PrintWindow(_editorWindowHelper.Handle);
        }

        public void CloseEditor()
        {
            if (!IsEditorOpen)
            {
                return;
            }

            PluginContext.PluginCommandStub.EditorClose();
            _editorWindow.Close();
            IsEditorOpen = false;
        }

        public List<PluginInfoItem> GetPluginInfoItems(IVstPluginContext pluginContext)
        {
            var _pluginInfoItems = new List<PluginInfoItem>();

            if (pluginContext == null)
            {
                return _pluginInfoItems;
            }

            // plugin product
            _pluginInfoItems.Add(new PluginInfoItem("Base", "Plugin Name",
                pluginContext.PluginCommandStub.GetEffectName()));
            _pluginInfoItems.Add(new PluginInfoItem("Base", "Product ",
                pluginContext.PluginCommandStub.GetProductString()));
            _pluginInfoItems.Add(new PluginInfoItem("Base", "Vendor ",
                pluginContext.PluginCommandStub.GetVendorString()));
            _pluginInfoItems.Add(new PluginInfoItem("Base", "Vendor Version ",
                pluginContext.PluginCommandStub.GetVendorVersion().ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("Base", "Vst Support ",
                pluginContext.PluginCommandStub.GetVstVersion().ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("Base", "Plugin Category ",
                pluginContext.PluginCommandStub.GetCategory().ToString()));

            // plugin info
            _pluginInfoItems.Add(new PluginInfoItem("Base", "Flags ", pluginContext.PluginInfo.Flags.ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("Base", "Plugin ID ",
                pluginContext.PluginInfo.PluginID.ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("Base", "Plugin ID String",
                VstUtils.PluginIdNumberToIdString(pluginContext.PluginInfo.PluginID)));

            _pluginInfoItems.Add(new PluginInfoItem("Base", "Plugin Version ",
                pluginContext.PluginInfo.PluginVersion.ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("Base", "Audio Input Count ",
                pluginContext.PluginInfo.AudioInputCount.ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("Base", "Audio Output Count ",
                pluginContext.PluginInfo.AudioOutputCount.ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("Base", "Initial Delay ",
                pluginContext.PluginInfo.InitialDelay.ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("Base", "Program Count ",
                pluginContext.PluginInfo.ProgramCount.ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("Base", "Parameter Count ",
                pluginContext.PluginInfo.ParameterCount.ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("Base", "Tail Size ",
                pluginContext.PluginCommandStub.GetTailSize().ToString()));

            // can do
            _pluginInfoItems.Add(new PluginInfoItem("CanDo", nameof(VstPluginCanDo.Bypass),
                pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.Bypass)).ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("CanDo", nameof(VstPluginCanDo.MidiProgramNames),
                pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.MidiProgramNames))
                    .ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("CanDo", nameof(VstPluginCanDo.Offline),
                pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.Offline)).ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("CanDo", nameof(VstPluginCanDo.ReceiveVstEvents),
                pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.ReceiveVstEvents))
                    .ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("CanDo", nameof(VstPluginCanDo.ReceiveVstMidiEvent),
                pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.ReceiveVstMidiEvent))
                    .ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("CanDo", nameof(VstPluginCanDo.ReceiveVstTimeInfo),
                pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.ReceiveVstTimeInfo))
                    .ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("CanDo", nameof(VstPluginCanDo.SendVstEvents),
                pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.SendVstEvents))
                    .ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("CanDo", nameof(VstPluginCanDo.SendVstMidiEvent),
                pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.SendVstMidiEvent))
                    .ToString()));

            _pluginInfoItems.Add(new PluginInfoItem("CanDo", nameof(VstPluginCanDo.ConformsToWindowRules),
                pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.ConformsToWindowRules))
                    .ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("CanDo", nameof(VstPluginCanDo.Metapass),
                pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.Metapass))
                    .ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("CanDo", nameof(VstPluginCanDo.MixDryWet),
                pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.MixDryWet))
                    .ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("CanDo", nameof(VstPluginCanDo.Multipass),
                pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.Multipass))
                    .ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("CanDo", nameof(VstPluginCanDo.NoRealTime),
                pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.NoRealTime))
                    .ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("CanDo", nameof(VstPluginCanDo.PlugAsChannelInsert),
                pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.PlugAsChannelInsert))
                    .ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("CanDo", nameof(VstPluginCanDo.PlugAsSend),
                pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.PlugAsSend))
                    .ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("CanDo", nameof(VstPluginCanDo.SendVstTimeInfo),
                pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.SendVstTimeInfo))
                    .ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("CanDo", nameof(VstPluginCanDo.x1in1out),
                pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x1in1out))
                    .ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("CanDo", nameof(VstPluginCanDo.x1in2out),
                pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x1in2out))
                    .ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("CanDo", nameof(VstPluginCanDo.x2in1out),
                pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x2in1out))
                    .ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("CanDo", nameof(VstPluginCanDo.x2in2out),
                pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x2in2out))
                    .ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("CanDo", nameof(VstPluginCanDo.x2in4out),
                pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x2in4out))
                    .ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("CanDo", nameof(VstPluginCanDo.x4in2out),
                pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x4in2out))
                    .ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("CanDo", nameof(VstPluginCanDo.x4in4out),
                pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x4in4out))
                    .ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("CanDo", nameof(VstPluginCanDo.x4in8out),
                pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x4in8out))
                    .ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("CanDo", nameof(VstPluginCanDo.x8in4out),
                pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x8in4out))
                    .ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("CanDo", nameof(VstPluginCanDo.x8in8out),
                pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x8in8out))
                    .ToString()));

            _pluginInfoItems.Add(new PluginInfoItem("Program", "Current Program Index",
                pluginContext.PluginCommandStub.GetProgram().ToString()));
            _pluginInfoItems.Add(new PluginInfoItem("Program", "Current Program Name",
                pluginContext.PluginCommandStub.GetProgramName()));

            return _pluginInfoItems;
        }
    }
}