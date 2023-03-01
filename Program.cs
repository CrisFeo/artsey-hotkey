using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

static class Program {

  // Constants
  ///////////////////////

  const int COMBO_DELAY = 50;
  const int HOLD_DELAY = 100;

  static readonly Dictionary<Key, Btn> KEY_MAPPING = new Dictionary<Key, Btn>() {
    { Key.Q, Btn.PinkyTop },
    { Key.W, Btn.RingTop },
    { Key.E, Btn.MiddleTop },
    { Key.R, Btn.IndexTop },
    { Key.A, Btn.PinkyBottom },
    { Key.S, Btn.RingBottom },
    { Key.D, Btn.MiddleBottom },
    { Key.F, Btn.IndexBottom },
  };

  static readonly Layer LAYER_BRACE = new Layer(
    "brace",
    new[] {
      new Combo("Layer: base", () => layer = LAYER_BASE!, Btn.IndexTop),
      new Combo(")",      () => { shift = true; return Key.N0; },   Btn.MiddleTop),
      new Combo("(",      () => { shift = true; return Key.N9; },   Btn.RingTop),
      new Combo("}",      () => { shift = true; return Key.OEM6; }, Btn.PinkyTop),
      new Combo("]",      () => Key.OEM6,                           Btn.MiddleBottom),
      new Combo("[",      () => Key.OEM4,                           Btn.RingBottom),
      new Combo("{",      () => { shift = true; return Key.OEM4; }, Btn.PinkyBottom),
    }
  );

  static readonly Layer LAYER_NUMBER = new Layer(
    "number",
    new[] {
      new Combo("Layer: base", () => layer = LAYER_BASE!, Btn.PinkyTop),
      new Combo("1",      () => Key.N1, Btn.IndexTop),
      new Combo("2",      () => Key.N2, Btn.MiddleTop),
      new Combo("3",      () => Key.N3, Btn.RingTop),
      new Combo("4",      () => Key.N4, Btn.IndexBottom),
      new Combo("5",      () => Key.N5, Btn.MiddleBottom),
      new Combo("6",      () => Key.N6, Btn.RingBottom),
      new Combo("7",      () => Key.N7, Btn.IndexTop, Btn.MiddleTop),
      new Combo("8",      () => Key.N8, Btn.MiddleTop, Btn.RingTop),
      new Combo("9",      () => Key.N9, Btn.IndexBottom, Btn.MiddleBottom),
      new Combo("0",      () => Key.N0, Btn.MiddleBottom, Btn.RingBottom),
    }
  );

  static readonly Layer LAYER_PUNCTUATION = new Layer(
    "punctuation",
    new[] {
      new Combo("Layer: base", () => layer = LAYER_BASE!, Btn.IndexBottom),
      new Combo("!",      () => { shift = true; return Key.N1; },   Btn.IndexTop),
      new Combo("\\",     () => Key.OEM5,                           Btn.MiddleTop),
      new Combo(";",      () => Key.OEM1,                           Btn.RingTop),
      new Combo("`",      () => Key.OEM3,                           Btn.PinkyTop),
      new Combo("?",      () => { shift = true; return Key.OEM2; }, Btn.MiddleBottom),
      new Combo("-",      () => Key.OEMMinus,                       Btn.RingBottom),
      new Combo("=",      () => Key.OEMPlus,                        Btn.PinkyBottom),
    }
  );

  static readonly Layer LAYER_BASE = new Layer(
    "base",
    new[] {
      new Combo("A", () => Key.A, Btn.IndexTop),
      new Combo("R", () => Key.R, Btn.MiddleTop),
      new Combo("T", () => Key.T, Btn.RingTop),
      new Combo("S", () => Key.S, Btn.PinkyTop),

      new Combo("E", () => Key.E, Btn.IndexBottom),
      new Combo("Y", () => Key.Y, Btn.MiddleBottom),
      new Combo("I", () => Key.I, Btn.RingBottom),
      new Combo("O", () => Key.O, Btn.PinkyBottom),

      new Combo("M", () => Key.M, Btn.PinkyBottom, Btn.RingBottom, Btn.MiddleBottom),
      new Combo("N", () => Key.N, Btn.PinkyBottom, Btn.RingBottom),
      new Combo("P", () => Key.P, Btn.PinkyBottom, Btn.RingBottom, Btn.IndexBottom),
      new Combo("Q", () => Key.Q, Btn.PinkyTop, Btn.RingTop, Btn.IndexTop),
      new Combo("U", () => Key.U, Btn.RingBottom, Btn.MiddleBottom),
      new Combo("V", () => Key.V, Btn.PinkyTop, Btn.MiddleTop),
      new Combo("W", () => Key.W, Btn.PinkyTop, Btn.IndexTop),
      new Combo("X", () => Key.X, Btn.PinkyTop, Btn.RingTop, Btn.MiddleTop),
      new Combo("Z", () => Key.Z, Btn.PinkyTop, Btn.RingTop, Btn.MiddleTop, Btn.IndexTop),

      new Combo("B", () => Key.B, Btn.PinkyBottom, Btn.IndexBottom),
      new Combo("C", () => Key.C, Btn.MiddleBottom, Btn.IndexBottom),
      new Combo("D", () => Key.D, Btn.RingTop, Btn.MiddleTop, Btn.IndexTop),
      new Combo("F", () => Key.F, Btn.MiddleTop, Btn.IndexTop),
      new Combo("G", () => Key.G, Btn.RingTop, Btn.MiddleTop),
      new Combo("H", () => Key.H, Btn.IndexBottom, Btn.RingBottom),
      new Combo("J", () => Key.J, Btn.PinkyTop, Btn.RingTop),
      new Combo("K", () => Key.K, Btn.PinkyBottom, Btn.MiddleBottom),
      new Combo("L", () => Key.L, Btn.RingBottom, Btn.MiddleBottom, Btn.IndexBottom),

      new Combo("Escape", () => Key.Escape,             Btn.PinkyBottom, Btn.MiddleTop, Btn.IndexTop),
      new Combo("Tab",    () => Key.Tab,                Btn.PinkyBottom, Btn.RingTop, Btn.MiddleTop, Btn.IndexTop),
      new Combo("Ctrl*",  () => control = true,         Btn.PinkyTop, Btn.IndexBottom),
      new Combo("Win*",   () => windows = true,         Btn.PinkyTop, Btn.MiddleBottom),
      new Combo("Alt*",   () => menu = true,            Btn.PinkyTop, Btn.RingBottom),
      new Combo("Shift*", () => shift = true,           Btn.PinkyTop, Btn.RingTop, Btn.MiddleTop, Btn.IndexBottom),
      new Combo("ShiftL", () => shiftLock = !shiftLock, Btn.MiddleTop, Btn.MiddleBottom),
      // Unimplemented: Caps lock
      // Unimplemented: Clear bluetooth

      new Combo("Return",    () => Key.Return,                       Btn.IndexTop, Btn.IndexBottom),
      new Combo("'",         () => Key.OEM7,                         Btn.RingBottom, Btn.MiddleBottom, Btn.IndexTop),
      new Combo("Period",    () => Key.OEMPeriod,                    Btn.MiddleBottom, Btn.IndexTop),
      new Combo("Comma",     () => Key.OEMComma,                     Btn.RingBottom, Btn.IndexTop),
      new Combo("/",         () => Key.OEM2,                         Btn.PinkyBottom, Btn.IndexTop),
      new Combo("!",         () => { shift = true; return Key.N1; }, Btn.RingTop, Btn.RingBottom),
      new Combo("Space",     () => Key.Space,                        Btn.PinkyBottom, Btn.RingBottom, Btn.MiddleBottom, Btn.IndexBottom),
      new Combo("Backspace", () => Key.Backspace,                    Btn.MiddleTop, Btn.IndexBottom),
      new Combo("Delete",    () => Key.Delete,                       Btn.RingBottom, Btn.MiddleTop),
    },
    new[] {
      new Hold($"Layer: {LAYER_BRACE.name}",       () => layer = LAYER_BRACE,       Btn.IndexTop),
      new Hold($"Layer: {LAYER_NUMBER.name}",      () => layer = LAYER_NUMBER,      Btn.PinkyTop),
      new Hold($"Layer: {LAYER_PUNCTUATION.name}", () => layer = LAYER_PUNCTUATION, Btn.IndexBottom),
    }
  );

  // Enums
  ///////////////////////

  enum Btn {
    PinkyTop,
    RingTop,
    MiddleTop,
    IndexTop,
    PinkyBottom,
    RingBottom,
    MiddleBottom,
    IndexBottom,
  }

  // Classes
  ///////////////////////

  class Combo {

    public string name          { get; init; }
    public Func<Key?> handler   { get; init; }
    public HashSet<Btn> buttons { get; init; }

    public Combo(string name, Func<Key?> handler, params Btn[] buttons) {
      this.name = name;
      this.handler = handler;
      this.buttons = new HashSet<Btn>(buttons);
    }

    public Combo(string name, Action handler, params Btn[] buttons) {
      this.name = name;
      this.handler = () => {
        handler();
        return null;
      };
      this.buttons = new HashSet<Btn>(buttons);
    }

  }

  class Hold {

    public string name          { get; init; }
    public Action handler       { get; init; }
    public HashSet<Btn> buttons { get; init; }

    public Hold(string name, Action handler, params Btn[] buttons) {
      this.name = name;
      this.handler = handler;
      this.buttons = new HashSet<Btn>(buttons);
    }

  }

  class Layer {

    public string name    { get; init; }
    public Combo[] combos { get; init; }
    public Hold[] holds   { get; init; }

    public Layer(string name, Combo[] combos, Hold[]? holds = null) {
      this.name = name;
      this.combos = combos;
      this.holds = holds ?? new Hold[0];
    }

  }

  // Internal vars
  ///////////////////////

  static bool control;
  static bool windows;
  static bool menu;
  static bool shift;
  static bool shiftLock;
  static Layer layer = LAYER_BASE;
  static HashSet<Btn> pendingComboButtons = new HashSet<Btn>();
  static CancellationTokenSource? comboCts;
  static HashSet<Btn> pendingHoldButtons = new HashSet<Btn>();
  static CancellationTokenSource? holdCts;

  // Public methods
  ///////////////////////

  public static void Main() {
    Hook.Install((isDown, key) => {
      if (!KEY_MAPPING.TryGetValue(key, out var btn)) {
        return false;
      }
      if (isDown) {
        pendingHoldButtons.Add(btn);
        if (holdCts != null) holdCts.Cancel();
        holdCts = new CancellationTokenSource();
        Task.Run(async () => {
          await Task.Delay(HOLD_DELAY, holdCts.Token);
          if (pendingHoldButtons.Count == 0) return;
          var matchingHold = default(Hold?);
          foreach (var hold in layer.holds) {
            if (hold.buttons.SetEquals(pendingHoldButtons)) {
              matchingHold = hold;
              break;
            }
          }
          LogAction("hold", pendingHoldButtons, matchingHold?.name);
          if (matchingHold != null) {
            matchingHold.handler();
          }
          pendingHoldButtons.Clear();
        });
        return true;
      }
      pendingHoldButtons.Remove(btn);
      pendingComboButtons.Add(btn);
      if (comboCts != null) comboCts.Cancel();
      comboCts = new CancellationTokenSource();
      Task.Run(async () => {
        await Task.Delay(COMBO_DELAY, comboCts.Token);
        var matchingCombo = default(Combo?);
        foreach (var combo in layer.combos) {
          if (combo.buttons.SetEquals(pendingComboButtons)) {
            matchingCombo = combo;
            break;
          }
        }
        LogAction("combo", pendingComboButtons, matchingCombo?.name);
        if (matchingCombo != null) {
          var key = matchingCombo.handler();
          if (key != null) {
            var keystrokes = new List<(Key, bool)>();
            if (windows) keystrokes.Add((Key.RightWindows, true));
            if (control) keystrokes.Add((Key.Control, true));
            if (shift || shiftLock) keystrokes.Add((Key.Shift, true));
            if (menu) keystrokes.Add((Key.Menu, true));
            keystrokes.Add((key.Value, true));
            keystrokes.Add((key.Value, false));
            if (menu) keystrokes.Add((Key.Menu, false));
            if (shift || shiftLock) keystrokes.Add((Key.Shift, false));
            if (control) keystrokes.Add((Key.Control, false));
            if (windows) keystrokes.Add((Key.RightWindows, false));
            if (windows) windows = false;
            if (control) control = false;
            if (shift) shift = false;
            if (menu) menu = false;
            Input.Send(keystrokes.ToArray());
          }
        }
        pendingComboButtons.Clear();
      });
      return true;
    });
    Loop.Run();
    Hook.Uninstall();
  }

  // Internal methods
  ///////////////////////

  static void LogAction(string action, IEnumerable<Btn> buttons, string? result) {
    var msg = new List<string>();
    msg.Add(layer.name);
    msg.Add(action);
    msg.Add("[");
    foreach (var button in buttons) {
      msg.Add(button.ToString());
    }
    msg.Add("] >");
    msg.Add(result ?? "NONE");
    Console.WriteLine(String.Join(" ", msg));
  }

}
