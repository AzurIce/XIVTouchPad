using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace XIVTouchPad.Input;

public class MouseHook : IDisposable
{
    private const int GWLP_WNDPROC = -4;
    private const int WM_MOUSEWHEEL = 0x020A;
    private const int WM_MOUSEHWHEEL = 0x020E;

    // Delegate to keep alive
    private readonly WndProcDelegate wndProcDelegate;
    private IntPtr originalWndProc = IntPtr.Zero;
    private IntPtr hWnd = IntPtr.Zero;

    // Event invoked when wheel is scrolled
    public event Action<int, bool>? OnMouseWheel; // delta, isHorizontal

    public MouseHook()
    {
        hWnd = Process.GetCurrentProcess().MainWindowHandle;
        if (hWnd == IntPtr.Zero)
        {
            throw new Exception("Could not find Main Window Handle");
        }

        wndProcDelegate = new WndProcDelegate(WndProc);
        originalWndProc = SetWindowLongPtr(hWnd, GWLP_WNDPROC, Marshal.GetFunctionPointerForDelegate(wndProcDelegate));
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

        return CallWindowProc(originalWndProc, hWnd, msg, wParam, lParam);
    }

    public void Dispose()
    {
        if (originalWndProc != IntPtr.Zero && hWnd != IntPtr.Zero)
        {
            // Restore the original WndProc
            SetWindowLongPtr(hWnd, GWLP_WNDPROC, originalWndProc);
            originalWndProc = IntPtr.Zero;
        }
    }

    // P/Invoke Definitions

    private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll")]
    private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
}
