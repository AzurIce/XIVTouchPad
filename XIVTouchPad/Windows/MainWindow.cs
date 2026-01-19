using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using XIVTouchPad.Interop;

namespace XIVTouchPad.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly Plugin plugin;

    public MainWindow(Plugin plugin)
        : base("Touchpad Debug Info##TouchpadDebug", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 300),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        var mousePos = plugin.CurrentMousePos;
        var mouseDelta = plugin.CurrentMouseDelta;
        var wheelV = plugin.CurrentMouseWheel;
        var wheelH = plugin.CurrentMouseWheelH;
        
        var rawWheelV = plugin.RawMouseWheel;
        var rawWheelH = plugin.RawMouseWheelH;
        
        ImGui.TextDisabled("Touchpad/Mouse Debug Info:");
        ImGui.Separator();
        
        ImGui.Text($"Cursor:  ({mousePos.X:F1}, {mousePos.Y:F1})");
        ImGui.Text($"Motion:  ({mouseDelta.X:F1}, {mouseDelta.Y:F1})  <-- 1-finger slide");
        
        ImGui.Separator();
        ImGui.Text("Scrolling (2-finger):");
        
        // ImGui Values
        if (wheelV != 0) ImGui.TextColored(new Vector4(0, 1, 0, 1), $"[ImGui] Vertical: {wheelV:F1}");
        else ImGui.TextDisabled("[ImGui] Vertical: -");

        if (wheelH != 0) ImGui.TextColored(new Vector4(0, 1, 0, 1), $"[ImGui] Horizontal: {wheelH:F1}");
        else ImGui.TextDisabled("[ImGui] Horizontal: -");
        
        // Raw Hook Values
        if (rawWheelV != 0) ImGui.TextColored(new Vector4(0, 1, 1, 1), $"[Raw]   Vertical: {rawWheelV:F1} (System Hook)");
        else ImGui.TextDisabled("[Raw]   Vertical: -");
        
        if (rawWheelH != 0) ImGui.TextColored(new Vector4(0, 1, 1, 1), $"[Raw]   Horizontal: {rawWheelH:F1} (System Hook)");
        else ImGui.TextDisabled("[Raw]   Horizontal: -");

        // Clicks
        ImGui.Separator();
        ImGui.Text($"Left Click: {(plugin.IsLeftMouseDown ? "DOWN" : "UP")}");
        ImGui.SameLine();
        ImGui.Text($"Right Click: {(plugin.IsRightMouseDown ? "DOWN" : "UP")}");

        ImGui.Separator();
        ImGui.Text("Camera Control (SDK + Custom Struct):");
        
        unsafe 
        {
            // Use the SDK's ClientStructs to find the instance pointer
            var csInstance = FFXIVClientStructs.FFXIV.Client.Game.Control.CameraManager.Instance();
            
            if (csInstance != null)
            {
                // Cast to OUR struct which has 'WorldCamera' and correct offsets
                var manager = (XIVTouchPad.Interop.CameraManager*)csInstance;
                var camera = manager->WorldCamera;
                
                if (camera != null)
                {
                    ImGui.Text($"Cam Ptr: {(IntPtr)camera:X}");
                    
                    ImGui.Text($"Yaw (H):   {camera->CurrentHRotation:F3}");
                    ImGui.Text($"Pitch (V): {camera->CurrentVRotation:F3}");
                    
                    ImGui.Separator();
                    
                    if (ImGui.Button("Left (-0.1)"))
                    {
                        camera->CurrentHRotation -= 0.1f;
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("Right (+0.1)"))
                    {
                        camera->CurrentHRotation += 0.1f;
                    }
                    
                    if (ImGui.Button("Up (+0.1)"))
                    {
                        camera->CurrentVRotation += 0.1f;
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("Down (-0.1)"))
                    {
                        camera->CurrentVRotation -= 0.1f;
                    }
                }
                else
                {
                    ImGui.TextColored(new Vector4(1,0,0,1), "WorldCamera is NULL");
                }
            }
            else
            {
                ImGui.TextColored(new Vector4(1,0,0,1), "CameraManager Instance is NULL");
            }
        }

        ImGui.Separator();
        ImGui.TextWrapped("Note: Raw multi-touch finger count is not exposed by ImGui/Game natively. It requires OS-level window hooks (WM_TOUCH).");
        
        ImGui.Spacing();
        ImGui.Separator();
        
        if (ImGui.Button("Open Settings"))
        {
            plugin.ToggleConfigUi();
        }
    }
}