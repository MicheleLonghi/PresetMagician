// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System.Threading.Tasks;
using System.Windows.Input;
using Catel;
using Catel.Logging;
using Catel.MVVM;
using Catel.Services;
using Orchestra.Services;
using Orchestra.ViewModels;
using PresetMagician.Services.Interfaces;
using PresetMagician.ViewModels;

namespace PresetMagician.Services
{
    public class AdvancedMessageService : Catel.Services.MessageService, IAdvancedMessageService
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IDispatcherService _dispatcherService;
        private readonly IUIVisualizerService _uiVisualizerService;
        private readonly IViewModelFactory _viewModelFactory;
        #endregion

        #region Constructors
        public AdvancedMessageService(IDispatcherService dispatcherService, IUIVisualizerService uiVisualizerService, 
            IViewModelFactory viewModelFactory, ILanguageService languageService)
            : base(dispatcherService, languageService)
        {
            Argument.IsNotNull(() => dispatcherService);
            Argument.IsNotNull(() => uiVisualizerService);
            Argument.IsNotNull(() => viewModelFactory);

            _dispatcherService = dispatcherService;
            _uiVisualizerService = uiVisualizerService;
            _viewModelFactory = viewModelFactory;
        }
        #endregion

        public Task<MessageResult> ShowErrorAsync(string message, string caption = "", string helpLink = "")
        {
            return ShowAsync(message, caption, helpLink, MessageButton.OK, MessageImage.Error);
        }

        public Task<MessageResult> ShowWarningAsync(string message, string caption = "", string helpLink = "")
        {
            return ShowAsync(message, caption, helpLink, MessageButton.OK, MessageImage.Warning);
        }

        public Task<MessageResult> ShowInformationAsync(string message, string caption = "", string helpLink = "")
        {
            return ShowAsync(message, caption, helpLink, MessageButton.OK, MessageImage.Information);
        }

        public Task<MessageResult> ShowAsync(string message, string caption = "", string helpLink = null, MessageButton button = MessageButton.OK, MessageImage icon = MessageImage.None)
        {
            Argument.IsNotNullOrWhitespace("message", message);

            Log.Info("Showing message to the user:\n\n{0}", this.GetAsText(message, button));

            var tcs = new TaskCompletionSource<MessageResult>();

#pragma warning disable AvoidAsyncVoid
            _dispatcherService.BeginInvoke(async () =>
#pragma warning restore AvoidAsyncVoid
            {
                var previousCursor = Mouse.OverrideCursor;
                Mouse.OverrideCursor = null;

                var vm = _viewModelFactory.CreateViewModel<HelpLinkMessageBoxViewModel>(null, null);

                vm.Message = message;
                vm.Button = button;
                vm.Icon = icon;
                vm.HelpLink = helpLink;

                vm.SetTitle(caption);

                await _uiVisualizerService.ShowDialogAsync(vm);

                Mouse.OverrideCursor = previousCursor;

                Log.Info("Result of message: {0}", vm.Result);

                tcs.TrySetResult(vm.Result);
            });

            return tcs.Task;
        }
    }
}