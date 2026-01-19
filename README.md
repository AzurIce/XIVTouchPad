# XIVTouchPad

Native-like touchpad gesture support for Final Fantasy XIV.

**XIVTouchPad** improves the game experience on laptops by hooking into raw input data to provide smooth, high-precision camera control using touchpad gestures.

## Features

*   **Smooth Camera Panning:** Use two-finger swipe gestures to rotate the camera (Yaw and Pitch).
*   **Direct Input Hooking:** Bypasses standard mouse emulation for smoother, more responsive control.
*   **Configurable Settings:**
    *   Adjust sensitivity for both Horizontal (Yaw) and Vertical (Pitch) axes.
    *   Invert axes independently.
    *   Separate logic ensures normal UI scrolling still works when hovering over windows.

## Getting Started

### Installation

1.  This plugin is currently in development.
2.  Clone this repository and build the project.
3.  Add the built `XIVTouchPad.dll` to your Dalamud Dev Plugin Locations.

### Usage

*   **Main Command:** `/touchpad`
    *   Toggles the debug/status window.
*   **Settings:**
    *   Open the settings window via the Plugin Installer or the "Open Settings" button in the main window.
    *   Adjust **Yaw/Pitch Sensitivity** to your liking.
    *   Toggle **Invert Yaw/Pitch** if the controls feel backwards.

## Technical Details

The plugin installs a local Windows hook (`WM_MOUSEWHEEL` / `WM_MOUSEHWHEEL`) to intercept high-precision touchpad scroll deltas before the game processes them as standard zoom/scroll events. When the cursor is *not* over a UI element, these deltas are translated directly into camera rotation values in the game's memory.

### Building

1. Open up `XIVTouchPad.sln` in your C# editor of choice (likely [Visual Studio 2022](https://visualstudio.microsoft.com) or [JetBrains Rider](https://www.jetbrains.com/rider/)).
2. Build the solution. By default, this will build a `Debug` build, but you can switch to `Release` in your IDE.
3. The resulting plugin can be found at `XIVTouchPad/bin/x64/Debug/XIVTouchPad.dll` (or `Release` if appropriate.)