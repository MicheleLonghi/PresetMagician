﻿using System;
using System.Windows;
using System.Windows.Media.Media3D;
using Microsoft.DwayneNeed.Numerics;

namespace Microsoft.DwayneNeed.Shapes
{
    public sealed class MobiusStrip : ParametricShape3D
    {
        static MobiusStrip()
        {
            // MinV should be -1.
            MinVProperty.OverrideMetadata(typeof(MobiusStrip), new PropertyMetadata(-1.0));

            // MaxV should be 1.
            MaxVProperty.OverrideMetadata(typeof(MobiusStrip), new PropertyMetadata(1.0));
        }

        public static DependencyProperty AProperty =
            DependencyProperty.Register(
                "A",
                typeof(double),
                typeof(MobiusStrip), new PropertyMetadata(
                    1.0, new PropertyChangedCallback(OnPropertyChangedAffectsModel)));

        public double A
        {
            get { return (double) GetValue(AProperty); }
            set { SetValue(AProperty, value); }
        }

        protected override Point3D Project(MemoizeMath u, MemoizeMath v)
        {
            double a = A;

            double x = (a - v.Value * Math.Sin(u.Value / 2.0)) * u.Sin;
            double y = (a - v.Value * Math.Sin(u.Value / 2.0)) * u.Cos;
            double z = v.Value * Math.Cos(u.Value / 2.0);
            return new Point3D(x, y, z);
        }
    }
}