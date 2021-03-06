using System.Collections.Generic;
using JetBrains.Annotations;
using PresetMagician.Core.Interfaces;

namespace PresetMagician.VendorPresetParser.MeldaProduction
{
    // ReSharper disable once InconsistentNaming
    [UsedImplicitly]
    public class MeldaProduction_MStereoProcessor : MeldaProduction, IVendorPresetParser
    {
        public override List<int> SupportedPlugins => new List<int> {1296114724};

        protected override string PresetFile { get; } =
            "MStereoProcessorpresets.xml";

        protected override string RootTag { get; } = "MStereoProcessorpresetspresets";
    }
}