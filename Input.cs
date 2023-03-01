using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

static class Input {

  // Enums
  ///////////////////////

  enum Type : int {
    Mouse = 0,
    Key = 1,
    Hardware = 2,
  }

  enum KeyFlags : int {
    ExtendedKey = 0x0001,
    KeyUp = 0x0002,
    ScanCode = 0x0008,
    UniCode = 0x0004,
  }

  // Structs
  ///////////////////////

  [StructLayout(LayoutKind.Sequential)]
  struct Msg {
    public Type type;
    public Data data;
    public static int Size {
      get => Marshal.SizeOf(typeof(Msg));
    }
  }

  [StructLayout(LayoutKind.Explicit)]
  struct Data {
    [FieldOffset(0)] public Mouse mouse;
    [FieldOffset(0)] public Keyboard keyboard;
    [FieldOffset(0)] public Hardware hardware;
  }

  [StructLayout(LayoutKind.Sequential)]
  struct Mouse {
    public int dx;
    public int dy;
    public int mouseData;
    public int flags;
    public int time;
    public IntPtr extraInfo;
  }

  [StructLayout(LayoutKind.Sequential)]
  struct Keyboard {
    public short key;
    public short scan;
    public KeyFlags flags;
    public int time;
    public IntPtr extraInfo;
  }

  [StructLayout(LayoutKind.Sequential)]
  struct Hardware {
    public int msg;
    public short low;
    public short high;
  }

  // DLL imports
  ///////////////////////

  [DllImport("user32.dll")]
  static extern int SendInput(int count, Msg[] inputs, int size);

  [DllImport("user32.dll")]
  static extern IntPtr GetMessageExtraInfo();

  [DllImport("user32.dll")]
  static extern short MapVirtualKey(short code, int mapType);

  [DllImport("User32.dll")]
  static extern ushort GetAsyncKeyState(Key key);

  // Public methods
  ///////////////////////

  public static void Send(params (Key, bool)[] keystrokes) {
    var messages = new Msg[keystrokes.Length];
    for (var i = 0; i < keystrokes.Length; i++) {
      var (key, isDown) = keystrokes[i];
      messages[i] = new Msg {
        type = Type.Key,
        data = new Data {
          keyboard = new Keyboard {
            key = (short)key,
            scan = (short)(MapVirtualKey((short)key, 0) & 0xFFU),
            flags = 0
              | (isDown ? 0 : KeyFlags.KeyUp)
              | (IsExtended(key) ? KeyFlags.ExtendedKey : 0),
            time = 0,
            extraInfo = GetMessageExtraInfo(),
          },
        },
      };
    }
    SendInput(messages.Length, messages, Msg.Size);
  }

  // Internal methods
  ///////////////////////

  static bool IsExtended(Key key)  => key switch {
    Key.Menu => true,
    Key.LeftMenu => true,
    Key.RightMenu => true,
    Key.Control => true,
    Key.RightControl => true,
    Key.Insert => true,
    Key.Delete => true,
    Key.Home => true,
    Key.End => true,
    Key.Prior => true,
    Key.Next => true,
    Key.Right => true,
    Key.Up => true,
    Key.Left => true,
    Key.Down => true,
    Key.NumLock => true,
    Key.Cancel => true,
    Key.Snapshot => true,
    Key.Divide => true,
    _ => false,
  };

}
