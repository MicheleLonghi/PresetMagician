using System.Collections.Generic;
using JetBrains.Annotations;
using PresetMagician.Core.Interfaces;

namespace PresetMagician.VendorPresetParser.MeldaProduction
{
    // ReSharper disable once InconsistentNaming
    [UsedImplicitly]
    public class MeldaProduction_MLimiterMB : MeldaProduction, IVendorPresetParser
    {
        public override List<int> SupportedPlugins => new List<int> {1296131369};

        protected override string PresetFile { get; } =
            "MMultiBandLimiterpresets.xml";

        protected override string RootTag { get; } = "MMultiBandLimiterpresetspresets";
    }
}