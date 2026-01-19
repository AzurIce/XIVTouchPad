using System;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.Game.Control;

namespace XIVTouchPad.Interop;

/// <summary>
/// Wrapper for FFXIVClientStructs CameraManager to access WorldCamera easily.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public unsafe struct CameraManager
{
    // We map the first field to the actual ClientStructs manager to ensure size/alignment if needed,
    // or just use the pointer directly.
    // However, the provided EasyZoomReborn code suggests accessing WorldCamera at offset 0x0
    // But in ClientStructs, CameraManager instance is usually a pointer.
    // Let's trust the layout provided:
    
    [FieldOffset(0x0)] public FFXIVClientStructs.FFXIV.Client.Game.Control.CameraManager CS;
    [FieldOffset(0x0)] public GameCamera* WorldCamera;
    [FieldOffset(0x8)] public GameCamera* IdleCamera;
    [FieldOffset(0x10)] public GameCamera* MenuCamera;
    [FieldOffset(0x18)] public GameCamera* SpectatorCamera;
}

/// <summary>
/// A detailed GameCamera struct based on EasyZoomReborn / Hypostasis reverse engineering.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public unsafe struct GameCamera
{
    [FieldOffset(0x0)] public nint* VTable;
    [FieldOffset(0x60)] public float X;
    [FieldOffset(0x64)] public float Y;
    [FieldOffset(0x68)] public float Z;
    [FieldOffset(0x90)] public float LookAtX;
    [FieldOffset(0x94)] public float LookAtY;
    [FieldOffset(0x98)] public float LookAtZ;
    
    [FieldOffset(0x124)] public float CurrentZoom; 
    [FieldOffset(0x128)] public float MinZoom; 
    [FieldOffset(0x12C)] public float MaxZoom; 
    
    [FieldOffset(0x130)] public float CurrentFoV; 
    [FieldOffset(0x134)] public float MinFoV; 
    [FieldOffset(0x138)] public float MaxFoV; 
    
    // The Holy Grail: Rotation
    [FieldOffset(0x140)] public float CurrentHRotation; // Yaw: -pi -> pi
    [FieldOffset(0x144)] public float CurrentVRotation; // Pitch: -0.35 -> +1.57 (approx)
    [FieldOffset(0x148)] public float HRotationDelta;
    
    [FieldOffset(0x158)] public float MinVRotation; 
    [FieldOffset(0x15C)] public float MaxVRotation; 
    [FieldOffset(0x170)] public float Tilt;
    
    [FieldOffset(0x180)] public int Mode; // 0=1st, 1=3rd
    [FieldOffset(0x184)] public int ControlType; 
    
    [FieldOffset(0x1C0)] public float ViewX;
    [FieldOffset(0x1C4)] public float ViewY;
    [FieldOffset(0x1C8)] public float ViewZ;
    
    [FieldOffset(0x1F4)] public byte IsFlipped; 
}
