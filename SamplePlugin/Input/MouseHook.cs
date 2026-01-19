using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SamplePlugin.Input;

public class MouseHook : IDisposable
{
    private const int GWLP_WNDPROC = -4;
    private const int WM_MOUSEWHEEL = 0x020A;
    private const int WM_MOUSEHWHEEL = 0x020E;

    // Delegate to keep alive
    private readonly WndProcDelegate _wndProcDelegate;
    private IntPtr _originalWndProc = IntPtr.Zero;
    private IntPtr _hWnd = IntPtr.Zero;

    // Event invoked when wheel is scrolled
    public event Action<int, bool>? OnMouseWheel; // delta, isHorizontal

    public MouseHook()
    {
        _hWnd = Process.GetCurrentProcess().MainWindowHandle;
        if (_hWnd == IntPtr.Zero)
        {
            throw new Exception("Could not find Main Window Handle");
        }

        _wndProcDelegate = new WndProcDelegate(WndProc);
        _originalWndProc = SetWindowLongPtr(_hWnd, GWLP_WNDPROC, Marshal.GetFunctionPointerForDelegate(_wndProcDelegate));
    }

    private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        if (msg == WM_MOUSEWHEEL || msg == WM_MOUSEHWHEEL)
        {
            try
            {
                // The high-order word of wParam is the wheel delta.
                long wVal = (long)wParam;
                short highWord = (short)((wVal >> 16) & 0xFFFF);

                if (highWord != 0)
                {
                    OnMouseWheel?.Invoke(highWord, msg == WM_MOUSEHWHEEL);
                }
            }
            catch (Exception)
            {
                // Swallow exceptions in the callback to prevent crashing the game loop
            }
        }

        return CallWindowProc(_originalWndProc, hWnd, msg, wParam, lParam);
    }

    public void Dispose()
    {
        if (_originalWndProc != IntPtr.Zero && _hWnd != IntPtr.Zero)
        {
            // Restore the original WndProc
            SetWindowLongPtr(_hWnd, GWLP_WNDPROC, _originalWndProc);
            _originalWndProc = IntPtr.Zero;
        }
    }

    // P/Invoke Definitions

    private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll")]
    private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
}
