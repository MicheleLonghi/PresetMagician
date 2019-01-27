﻿using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Drachenkatze.PresetMagician.VSTHost.VST
{
    public interface IPresetBank
    {
        string BankName { get; set; }
        IPresetBank ParentBank { get; set; }
        string BankPath { get; }

        /// <summary>
        /// Gets or sets the Presets value.
        /// </summary>

        ObservableCollection<IPresetBank> PresetBanks { get; set; }

        List<string> GetBankPath();
        IPresetBank FindBankPath(string bankPath);
        IPresetBank CreateRecursive(string bankPath);
    }
}