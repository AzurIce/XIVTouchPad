using Dalamud.Configuration;
using System;

namespace SamplePlugin;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool IsConfigWindowMovable { get; set; } = true;
    public bool SomePropertyToBeSavedAndWithADefault { get; set; } = true;

    // Camera Control Settings
    public float SpeedYaw { get; set; } = 0.05f;
    public float SpeedPitch { get; set; } = 0.05f;
    public bool InvertYaw { get; set; } = false;
    public bool InvertPitch { get; set; } = false;

    // The below exists just to make saving less cumbersome
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
