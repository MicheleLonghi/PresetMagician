﻿using System.Windows;
using Catel.IoC;
using Orchestra.Services;
using PresetMagician.Views;

namespace PresetMagician.Services
{
    public class RibbonService : IRibbonService
    {
        #region IRibbonService Members

        public FrameworkElement GetRibbon()
        {
            return ServiceLocator.Default.ResolveType<RibbonView>();
        }

        public FrameworkElement GetMainView()
        {
            return new MainView();
        }

        public FrameworkElement GetStatusBar()
        {
            return new StatusBarView();
        }

        #endregion IRibbonService Members
    }
}