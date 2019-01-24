﻿using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using SharedModels;

namespace Drachenkatze.PresetMagician.VendorPresetParser.AudioThing
{
    // ReSharper disable once InconsistentNaming
    [UsedImplicitly]
    public class AudioThing_SR88 : AudioThing, IVendorPresetParser
    {
        public override List<int> SupportedPlugins => new List<int> {1397897272};


        protected override string GetDataDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                @"AudioThing\Presets\SR-88");
        }
    }
}