using System;
using System.Reflection;
using System.Runtime.InteropServices;

static class Loop {

  // Structs
  ///////////////////////

  [StructLayout(LayoutKind.Sequential)]
  public struct Point {
    public long x;
    public long y;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct Msg {
    public IntPtr hwnd;
    public uint message;
    public UIntPtr wParam;
    public IntPtr lParam;
    public uint time;
    public Point pt;
  }

  // DLL imports
  ///////////////////////

  [DllImport("user32.dll")]
  static extern int GetMessage(ref Msg message, IntPtr wnd, uint filterMin, uint filterMax);

  [DllImport("user32.dll")]
  static extern bool TranslateMessage(ref Msg message);

  [DllImport("user32.dll")]
  static extern long DispatchMessage(ref Msg message);

  // Public methods
  ///////////////////////

  public static void Run() {
    var msg = new Msg();
    while (true) {
      var ret = GetMessage(ref msg, IntPtr.Zero, 0, 0);
      if (ret <= 0) break;
      TranslateMessage(ref msg);
      DispatchMessage(ref msg);
    }
  }

}
