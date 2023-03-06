using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

static class Hook {

  // Constants
  ///////////////////////

  const int HOOK_TYPE_KEYBOARD_LOW_LEVEL = 13;

  // Enums
  ///////////////////////

  enum MsgType : int {
    KEY_DOWN = 0x100,
    KEY_UP = 0x101,
    SYS_KEY_DOWN = 0x104,
    SYS_KEY_UP = 0x105,
  }

  enum Flags : int {
    INJECTED = 0x10,
  }

  // Structs
  ///////////////////////

  [StructLayout(LayoutKind.Sequential)]
  struct Msg {
    public int keyCode;
    public int scanCode;
    public Flags flags;
    public int time;
    public int extra;
  }

  // Delegates
  ///////////////////////

  delegate IntPtr HookFunc(int code, IntPtr typePtr, IntPtr msgPtr);

  // DLL imports
  ///////////////////////

  [DllImport("user32.dll")]
  static extern IntPtr SetWindowsHookEx(int type, HookFunc cbk, IntPtr mod, int thread);

  [DllImport("user32.dll")]
  static extern IntPtr CallNextHookEx(IntPtr hdl, int code, IntPtr type, IntPtr msg);

  [DllImport("user32.dll")]
  static extern bool UnhookWindowsHookEx(IntPtr hdl);

  // Internal vars
  ///////////////////////

  static HookFunc callback = OnHook;
  static Func<bool, Key, bool>? handler;
  static IntPtr hookPtr = IntPtr.Zero;

  // Public methods
  ///////////////////////

  public static void Install(Func<bool, Key, bool> handler) {
    Hook.handler = handler;
    hookPtr = SetWindowsHookEx(
      HOOK_TYPE_KEYBOARD_LOW_LEVEL,
      Hook.callback,
      Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]),
      0
    );
  }

  public static void Uninstall() {
    UnhookWindowsHookEx(hookPtr);
    hookPtr = IntPtr.Zero;
    handler = null;
  }

  // Internal methods
  ///////////////////////

  static IntPtr OnHook(int code, IntPtr typePtr, IntPtr msgPtr) {
    if (handler == null || code < 0) {
      return CallNextHookEx(hookPtr, code, typePtr, msgPtr);
    }
    var msgType = (MsgType)typePtr.ToInt32();
    var msg = (Msg)Marshal.PtrToStructure(
      msgPtr,
      typeof(Msg)
    )!;
    if ((msg.flags & Flags.INJECTED) != 0) {
      return CallNextHookEx(hookPtr, code, typePtr, msgPtr);
    }
    var key = (Key)msg.keyCode;
    var isDown = msgType switch {
      MsgType.KEY_DOWN => true,
      MsgType.KEY_UP => false,
      MsgType.SYS_KEY_DOWN => true,
      MsgType.SYS_KEY_UP => false,
      _ => throw new Exception($"invalid message type: {msgType}"),
    };
    var handled = handler(isDown, key);
    if (handled) {
      return new IntPtr(-1);
    }
    return CallNextHookEx(hookPtr, code, typePtr, msgPtr);
  }

}
