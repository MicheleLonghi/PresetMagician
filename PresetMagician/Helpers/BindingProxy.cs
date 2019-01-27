﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;

namespace PresetMagician.Helpers
{
    public class BindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        public object Data
        {
            get { return (object) GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations",
            Justification = "It'll be fiiiiine")]
        public override string ToString()
        {
            return "BindingProxy: " + Convert.ToString(Data, CultureInfo.CurrentCulture);
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));
    }
}