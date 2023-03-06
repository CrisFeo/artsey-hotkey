using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

static class Program {

  // Constants
  ///////////////////////

  const int COMBO_DELAY = 30;
  const int HOLD_DELAY = 150;

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
      Combo.Exact("Layer: base", () => layer = LAYER_BASE!, Btn.IndexTop),
      Combo.Exact(")",      () => { shift = true; return Key.N0; },   Btn.MiddleTop),
      Combo.Exact("(",      () => { shift = true; return Key.N9; },   Btn.RingTop),
      Combo.Exact("}",      () => { shift = true; return Key.OEM6; }, Btn.PinkyTop),
      Combo.Exact("]",      () => Key.OEM6,                           Btn.MiddleBottom),
      Combo.Exact("[",      () => Key.OEM4,                           Btn.RingBottom),
      Combo.Exact("{",      () => { shift = true; return Key.OEM4; }, Btn.PinkyBottom),
    }
  );

  static readonly Layer LAYER_NUMBER = new Layer(
    "number",
    combos: new[] {
      Combo.Inexact("Layer: base", () => layer = LAYER_BASE!, Btn.PinkyTop),
      Combo.Exact("1",      () => Key.N1, Btn.IndexTop),
      Combo.Exact("2",      () => Key.N2, Btn.MiddleTop),
      Combo.Exact("3",      () => Key.N3, Btn.RingTop),
      Combo.Exact("4",      () => Key.N4, Btn.IndexBottom),
      Combo.Exact("5",      () => Key.N5, Btn.MiddleBottom),
      Combo.Exact("6",      () => Key.N6, Btn.RingBottom),
      Combo.Exact("7",      () => Key.N7, Btn.IndexTop, Btn.MiddleTop),
      Combo.Exact("8",      () => Key.N8, Btn.MiddleTop, Btn.RingTop),
      Combo.Exact("9",      () => Key.N9, Btn.IndexBottom, Btn.MiddleBottom),
      Combo.Exact("0",      () => Key.N0, Btn.MiddleBottom, Btn.RingBottom),
    }
  );

  static readonly Layer LAYER_PUNCTUATION = new Layer(
    "punctuation",
    combos: new[] {
      Combo.Inexact("Layer: base", () => layer = LAYER_BASE!, Btn.IndexBottom),
      Combo.Exact("!",      () => { shift = true; return Key.N1; },   Btn.IndexTop),
      Combo.Exact("\\",     () => Key.OEM5,                           Btn.MiddleTop),
      Combo.Exact(";",      () => Key.OEM1,                           Btn.RingTop),
      Combo.Exact("`",      () => Key.OEM3,                           Btn.PinkyTop),
      Combo.Exact("?",      () => { shift = true; return Key.OEM2; }, Btn.MiddleBottom),
      Combo.Exact("-",      () => Key.OEMMinus,                       Btn.RingBottom),
      Combo.Exact("=",      () => Key.OEMPlus,                        Btn.PinkyBottom),
    }
  );

  static readonly Layer LAYER_BASE = new Layer(
    "base",
    combos: new[] {
      Combo.Exact("A", () => Key.A, Btn.IndexTop),
      Combo.Exact("R", () => Key.R, Btn.MiddleTop),
      Combo.Exact("T", () => Key.T, Btn.RingTop),
      Combo.Exact("S", () => Key.S, Btn.PinkyTop),

      Combo.Exact("E", () => Key.E, Btn.IndexBottom),
      Combo.Exact("Y", () => Key.Y, Btn.MiddleBottom),
      Combo.Exact("I", () => Key.I, Btn.RingBottom),
      Combo.Exact("O", () => Key.O, Btn.PinkyBottom),

      Combo.Exact("M", () => Key.M, Btn.PinkyBottom, Btn.RingBottom, Btn.MiddleBottom),
      Combo.Exact("N", () => Key.N, Btn.PinkyBottom, Btn.RingBottom),
      Combo.Exact("P", () => Key.P, Btn.PinkyBottom, Btn.RingBottom, Btn.IndexBottom),
      Combo.Exact("Q", () => Key.Q, Btn.PinkyTop, Btn.RingTop, Btn.IndexTop),
      Combo.Exact("U", () => Key.U, Btn.RingBottom, Btn.MiddleBottom),
      Combo.Exact("V", () => Key.V, Btn.PinkyTop, Btn.MiddleTop),
      Combo.Exact("W", () => Key.W, Btn.PinkyTop, Btn.IndexTop),
      Combo.Exact("X", () => Key.X, Btn.PinkyTop, Btn.RingTop, Btn.MiddleTop),
      Combo.Exact("Z", () => Key.Z, Btn.PinkyTop, Btn.RingTop, Btn.MiddleTop, Btn.IndexTop),

      Combo.Exact("B", () => Key.B, Btn.PinkyBottom, Btn.IndexBottom),
      Combo.Exact("C", () => Key.C, Btn.MiddleBottom, Btn.IndexBottom),
      Combo.Exact("D", () => Key.D, Btn.RingTop, Btn.MiddleTop, Btn.IndexTop),
      Combo.Exact("F", () => Key.F, Btn.MiddleTop, Btn.IndexTop),
      Combo.Exact("G", () => Key.G, Btn.RingTop, Btn.MiddleTop),
      Combo.Exact("H", () => Key.H, Btn.IndexBottom, Btn.RingBottom),
      Combo.Exact("J", () => Key.J, Btn.PinkyTop, Btn.RingTop),
      Combo.Exact("K", () => Key.K, Btn.PinkyBottom, Btn.MiddleBottom),
      Combo.Exact("L", () => Key.L, Btn.RingBottom, Btn.MiddleBottom, Btn.IndexBottom),

      Combo.Exact("Escape", () => Key.Escape,             Btn.PinkyBottom, Btn.MiddleTop, Btn.IndexTop),
      Combo.Exact("Tab",    () => Key.Tab,                Btn.PinkyBottom, Btn.RingTop, Btn.MiddleTop, Btn.IndexTop),
      Combo.Exact("Ctrl*",  () => control = true,         Btn.PinkyTop, Btn.IndexBottom),
      Combo.Exact("Win*",   () => windows = true,         Btn.PinkyTop, Btn.MiddleBottom),
      Combo.Exact("Alt*",   () => menu = true,            Btn.PinkyTop, Btn.RingBottom),
      Combo.Exact("Shift*", () => shift = true,           Btn.PinkyTop, Btn.RingTop, Btn.MiddleTop, Btn.IndexBottom),
      Combo.Exact("ShiftL", () => shiftLock = !shiftLock, Btn.MiddleTop, Btn.MiddleBottom),
      // Unimplemented: Caps lock
      // Unimplemented: Clear bluetooth

      Combo.Exact("Return",    () => Key.Return,                       Btn.IndexTop, Btn.IndexBottom),
      Combo.Exact("'",         () => Key.OEM7,                         Btn.RingBottom, Btn.MiddleBottom, Btn.IndexTop),
      Combo.Exact("Period",    () => Key.OEMPeriod,                    Btn.MiddleBottom, Btn.IndexTop),
      Combo.Exact("Comma",     () => Key.OEMComma,                     Btn.RingBottom, Btn.IndexTop),
      Combo.Exact("/",         () => Key.OEM2,                         Btn.PinkyBottom, Btn.IndexTop),
      Combo.Exact("!",         () => { shift = true; return Key.N1; }, Btn.RingTop, Btn.RingBottom),
      Combo.Exact("Space",     () => Key.Space,                        Btn.PinkyBottom, Btn.RingBottom, Btn.MiddleBottom, Btn.IndexBottom),
      Combo.Exact("Backspace", () => Key.Backspace,                    Btn.MiddleTop, Btn.IndexBottom),
      Combo.Exact("Delete",    () => Key.Delete,                       Btn.RingBottom, Btn.MiddleTop),
    },
    holds: new[] {
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

    // Instance vars
    ///////////////////////

    public string name          { get; init; }
    public Func<Key?> handler   { get; init; }
    public HashSet<Btn> buttons { get; init; }
    public bool inexact         { get; init; }

    // Constructor
    ///////////////////////

    Combo(string name, Func<Key?> handler, HashSet<Btn> buttons, bool inexact) {
      this.name = name;
      this.handler = handler;
      this.buttons = buttons;
      this.inexact = inexact;
    }

    // Static methods
    ///////////////////////

    public static Combo Exact(string name, Func<Key?> handler, params Btn[] buttons) => new Combo(
      name,
      handler,
      new HashSet<Btn>(buttons),
      false
    );

    public static Combo Exact(string name, Action handler, params Btn[] buttons) => new Combo(
      name,
      () => { handler(); return null; },
      new HashSet<Btn>(buttons),
      false
    );

    public static Combo Inexact(string name, Action handler, params Btn[] buttons) => new Combo(
      name,
      () => { handler(); return null; },
      new HashSet<Btn>(buttons),
      true
    );

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

    public Layer(string name, Combo[]? combos = null, Hold[]? holds = null) {
      this.name = name;
      this.combos = combos ?? new Combo[0];
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
          if (combo.inexact) {
            if (pendingComboButtons.IsSupersetOf(combo.buttons)) {
              matchingCombo = combo;
              break;
            }
          } else {
            if (combo.buttons.SetEquals(pendingComboButtons)) {
              matchingCombo = combo;
              break;
            }
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
