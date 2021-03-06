using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using PresetMagician.Core.Interfaces;
using PresetMagician.VendorPresetParser.Common;

namespace PresetMagician.VendorPresetParser.ImageLine
{
    // ReSharper disable once InconsistentNaming
    [UsedImplicitly]
    public class ImageLine_ToxicBiohazard : RecursiveBankDirectoryParser, IVendorPresetParser
    {
        protected override string Extension { get; } = "tbio";
        public override List<int> SupportedPlugins => new List<int> {1416591412};

        protected override string GetParseDirectory()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                @"Image-Line\Toxic Biohazard\");
        }
    }
}