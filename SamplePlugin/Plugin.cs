using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using SamplePlugin.Windows;
using SamplePlugin.Input;
using Dalamud.Bindings.ImGui;

namespace SamplePlugin;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IPlayerState PlayerState { get; private set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;

    private const string CommandName = "/pmycommand";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("SamplePlugin");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }
    private MouseHook? MouseHook { get; init; }

    // Public properties to store global mouse state
    public System.Numerics.Vector2 CurrentMousePos { get; private set; }
    public System.Numerics.Vector2 CurrentMouseDelta { get; private set; }
    public float CurrentMouseWheel { get; private set; } // Vertical scroll (often 2-finger swipe Y)
    public float CurrentMouseWheelH { get; private set; } // Horizontal scroll (often 2-finger swipe X)
    
    // Raw hook values
    public float RawMouseWheel { get; private set; }
    public float RawMouseWheelH { get; private set; }
    
    public bool IsLeftMouseDown { get; private set; }
    public bool IsRightMouseDown { get; private set; }

    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        // You might normally want to embed resources and load them from the manifest stream
        var goatImagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this, goatImagePath);
        
        // Initialize the native mouse hook
        try 
        {
            MouseHook = new MouseHook();
            MouseHook.OnMouseWheel += OnRawMouseWheel;
        }
        catch (System.Exception e)
        {
            Log.Error(e, "Failed to install mouse hook");
        }

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        // In response to the slash command, toggle the display status of our main ui
        CommandManager.AddHandler(CommandName, new CommandInfo((command, args) => MainWindow.Toggle())
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        // Tell the UI system that we want our windows to be drawn through the window system
        PluginInterface.UiBuilder.Draw += WindowSystem.Draw;

        // Register our global draw handler for background processing
        PluginInterface.UiBuilder.Draw += OnGlobalDraw;

        // This adds a button to the plugin installer entry of this plugin which allows
        // toggling the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUi;

        // Adds another button doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUi;

        // Add a simple message to the log with level set to information
        // Use /xllog to open the log window in-game
        // Example Output: 00:57:54.959 | INF | [SamplePlugin] ===A cool log message from Sample Plugin===
        Log.Information($"===A cool log message from {PluginInterface.Manifest.Name}===");
    }
    
    private float _accumulatedWheelV = 0;
    private float _accumulatedWheelH = 0;

    private void OnRawMouseWheel(int delta, bool isHorizontal)
    {
        // Standardize the delta (usually 120 per notch). 
        float val = delta / 120.0f;
        if (isHorizontal)
            _accumulatedWheelH += val;
        else
            _accumulatedWheelV += val;
    }

    private void OnGlobalDraw()
    {
        // Transfer accumulated values to public properties for this frame
        RawMouseWheel = _accumulatedWheelV;
        RawMouseWheelH = _accumulatedWheelH;
        
        // Reset accumulators
        _accumulatedWheelV = 0;
        _accumulatedWheelH = 0;

        // This runs every frame when the game UI is drawn, even if our windows are closed.
        // It is safe to call ImGui functions here.
        var io = ImGui.GetIO();
        CurrentMousePos = ImGui.GetMousePos();
        CurrentMouseDelta = io.MouseDelta;
        CurrentMouseWheel = io.MouseWheel;
        CurrentMouseWheelH = io.MouseWheelH;
        IsLeftMouseDown = ImGui.IsMouseDown(ImGuiMouseButton.Left);
        IsRightMouseDown = ImGui.IsMouseDown(ImGuiMouseButton.Right);
    }

    public void Dispose()
    {
        // Unregister all actions to not leak anything during disposal of plugin
        PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
        PluginInterface.UiBuilder.Draw -= OnGlobalDraw;
        PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUi;
        PluginInterface.UiBuilder.OpenMainUi -= ToggleMainUi;
        
        MouseHook?.Dispose();

        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
    }

    public void ToggleConfigUi() => ConfigWindow.Toggle();
    public void ToggleMainUi() => MainWindow.Toggle();
}
