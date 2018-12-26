﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShellWindow.xaml.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System.Diagnostics;
using Orchestra;
using Orchestra.Services;
using Orchestra.Views;
using PresetMagician.Helpers;
using PresetMagician.Services.Interfaces;

namespace PresetMagician.Views
{
    using Catel.IoC;
    using Catel.Windows;
    using Services;
    /// <summary>
    /// Interaction logic for ShellWindow.xaml.
    /// </summary>
    public partial class ShellWindow : IShell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellWindow"/> class.
        /// </summary>
        public ShellWindow()
        {
            var serviceLocator = ServiceLocator.Default;

            var themeService = serviceLocator.ResolveType<IThemeService>();
            ThemeHelper.EnsureApplicationThemes(GetType().Assembly, themeService.ShouldCreateStyleForwarders());

            InitializeComponent();

            serviceLocator.RegisterInstance(FooProgressMar, "pleaseWaitService");

            var statusService = serviceLocator.ResolveType<ICustomStatusService>();
            statusService.Initialize(statusTextBlock);

            var dependencyResolver = this.GetDependencyResolver();
            var ribbonService = dependencyResolver.Resolve<IRibbonService>();

            var ribbonContent = ribbonService.GetRibbon();
            if (ribbonContent != null)
            {
                ribbonContentControl.SetCurrentValue(ContentProperty, ribbonContent);

                var ribbon = ribbonContent.FindVisualDescendantByType<Fluent.Ribbon>();
                if (ribbon != null)
                {
                    serviceLocator.RegisterInstance<Fluent.Ribbon>(ribbon);
                }
            }

            var statusBarContent = ribbonService.GetStatusBar();
            if (statusBarContent != null)
            {
                customStatusBarItem.SetCurrentValue(ContentProperty, statusBarContent);
            }

            var mainView = ribbonService.GetMainView();
            contentControl.Content = mainView;

            ShellDimensionsHelper.ApplyDimensions(this, mainView);
        }
    }
}