using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using PresetMagician.Core.Interfaces;

namespace PresetMagician.VendorPresetParser.SlateDigital
{
    // ReSharper disable once InconsistentNaming
    [UsedImplicitly]
    public class SlateDigital_VerbSuiteClassics : SlateDigitalPresetParser, IVendorPresetParser
    {
        public override List<int> SupportedPlugins => new List<int> {1450402640};

        protected override string PresetSectionName { get; } = "VscP";

        protected override string GetParseDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                @"Slate Digital\VerbSuite Classics\Presets");
        }
    }
}