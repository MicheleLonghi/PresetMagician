using System.Collections.Generic;
using JetBrains.Annotations;
using PresetMagician.Core.Interfaces;

namespace PresetMagician.VendorPresetParser.MeldaProduction
{
    // ReSharper disable once InconsistentNaming
    [UsedImplicitly]
    public class MeldaProduction_MDrumEnhancer : MeldaProduction, IVendorPresetParser
    {
        public override List<int> SupportedPlugins => new List<int> {1296319854};

        protected override string PresetFile { get; } = "MDrumEnhancerpresets.xml";

        protected override string RootTag { get; } = "MDrumEnhancerpresets";
    }
}