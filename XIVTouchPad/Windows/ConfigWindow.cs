using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace XIVTouchPad.Windows;

public class ConfigWindow : Window, IDisposable
{
    private readonly Configuration configuration;

    public ConfigWindow(Plugin plugin) : base("Touchpad Camera Config###TouchpadCameraConfig")
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(350, 200);
        SizeCondition = ImGuiCond.FirstUseEver;

        configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void PreDraw()
    {
        if (configuration.IsConfigWindowMovable)
        {
            Flags &= ~ImGuiWindowFlags.NoMove;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoMove;
        }
    }

    public override void Draw()
    {
        ImGui.Text("Touchpad Camera Settings");
        ImGui.Separator();
        ImGui.Spacing();

        var speedYaw = configuration.SpeedYaw;
        if (ImGui.SliderFloat("Yaw Sensitivity (X)", ref speedYaw, 0.01f, 1.0f))
        {
            configuration.SpeedYaw = speedYaw;
            configuration.Save();
        }

        var speedPitch = configuration.SpeedPitch;
        if (ImGui.SliderFloat("Pitch Sensitivity (Y)", ref speedPitch, 0.01f, 1.0f))
        {
            configuration.SpeedPitch = speedPitch;
            configuration.Save();
        }

        ImGui.Spacing();

        var invertYaw = configuration.InvertYaw;
        if (ImGui.Checkbox("Invert Yaw (X)", ref invertYaw))
        {
            configuration.InvertYaw = invertYaw;
            configuration.Save();
        }
        
        ImGui.SameLine();
        var invertPitch = configuration.InvertPitch;
        if (ImGui.Checkbox("Invert Pitch (Y)", ref invertPitch))
        {
            configuration.InvertPitch = invertPitch;
            configuration.Save();
        }

        ImGui.Separator();
        ImGui.Spacing();

        var movable = configuration.IsConfigWindowMovable;
        if (ImGui.Checkbox("Movable Config Window", ref movable))
        {
            configuration.IsConfigWindowMovable = movable;
            configuration.Save();
        }
    }
}