﻿using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using SharedModels;

namespace Drachenkatze.PresetMagician.VendorPresetParser.AudioThing
{
    // ReSharper disable once InconsistentNaming
    [UsedImplicitly]
    public class AudioThing_PhaseMotionV2 : AudioThing, IVendorPresetParser
    {
        public override List<int> SupportedPlugins => new List<int> {1884122930};


        protected override string GetDataDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                @"AudioThing\Presets\PhaseMotion2");
        }
    }
}