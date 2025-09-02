using ForgeEvo.Core.Math;
using Veldrid;

namespace ForgeEvo.Core.Engine;

/// <summary>
///     Input handler for a game with keyboard and mouse controls.
/// </summary>
public static class InputHandler
{
    /// <summary>
    ///     Set of keys that are currently down.
    /// </summary>
    private static readonly HashSet<Key> KeysDown = [];

    /// <summary>
    ///     Set of keys that have been recently pressed.
    /// </summary>
    private static readonly HashSet<Key> KeysPressed = [];

    /// <summary>
    ///     Set of keys that have been recently released.
    /// </summary>
    private static readonly HashSet<Key> KeysReleased = [];

    /// <summary>
    ///     Set of mouse buttons that are currently down.
    /// </summary>
    private static readonly HashSet<MouseButton> MouseDown = [];

    /// <summary>
    ///     Set of mouse buttons that have been recently pressed.
    /// </summary>
    private static readonly HashSet<MouseButton> MousePressed = [];

    /// <summary>
    ///     Set of mouse buttons that have been recently released.
    /// </summary>
    private static readonly HashSet<MouseButton> MouseReleased = [];

    /// <summary>
    ///     Change in position of the mouse pointer from the last frame to the current frame. It is equivalent to
    ///     <c>_mousePosition - _mouseLastPosition</c>.
    /// </summary>
    private static MutableVector2D _mouseDelta;

    /// <summary>
    ///     Position of the mouse pointer in the last frame.
    /// </summary>
    private static MutableVector2D _mouseLastPosition;

    /// <summary>
    ///     Position of the mouse pointer in the current frame.
    /// </summary>
    private static MutableVector2D _mousePosition;

    /// <summary>
    ///     Event fired when any key is pressed.
    /// </summary>
    private static readonly Event KeyPressedEvent;

    /// <summary>
    ///     Event fired when any key is released.
    /// </summary>
    private static readonly Event KeyReleasedEvent;

    /// <summary>
    ///     Event fired when any mouse button is pressed.
    /// </summary>
    private static readonly Event MousePressedEvent;

    /// <summary>
    ///     Event fired when any mouse button is released.
    /// </summary>
    private static readonly Event MouseReleasedEvent;

    /// <summary>
    ///     Static initializer that registers key and mouse internal events with the event bus.
    /// </summary>
    static InputHandler()
    {
        EventBus.RegisterInternalEvents(
            InternalEvent.KeyPressed,
            InternalEvent.KeyReleased,
            InternalEvent.MousePressed,
            InternalEvent.MouseReleased
        );

        KeyPressedEvent = EventBus.GetByName(InternalEvent.KeyPressed.ToName());
        KeyReleasedEvent = EventBus.GetByName(InternalEvent.KeyReleased.ToName());

        MousePressedEvent = EventBus.GetByName(InternalEvent.MousePressed.ToName());
        MouseReleasedEvent = EventBus.GetByName(InternalEvent.MouseReleased.ToName());
    }

    /// <summary>
    ///     Change in scroll amount in the vertical direction from the last frame to the current frame.
    /// </summary>
    public static float ScrollDelta { get; private set; }

    /// <summary>
    ///     Position of the mouse pointer in the current frame.
    /// </summary>
    public static Vector2D MousePosition => _mousePosition;

    /// <summary>
    ///     Change in position of the mouse pointer from the last frame to the current frame.
    /// </summary>
    public static Vector2D MouseDelta => _mouseDelta;

    /// <summary>
    ///     Whether the mouse has moved since the last frame.
    /// </summary>
    public static bool DidMouseMove => _mouseDelta.LengthSquared() > 0;

    /// <summary>
    ///     Update the input handler with the latest keyboard and mouse data. Ideally called every game-loop.
    /// </summary>
    /// <param name="snapshot">Current Veldrid input snapshot which contains the latest keyboard and mouse data.</param>
    public static void Update(InputSnapshot snapshot)
    {
        KeysPressed.Clear();
        KeysReleased.Clear();

        MousePressed.Clear();
        MouseReleased.Clear();

        _mouseDelta = Vector2D.Zero;
        ScrollDelta = 0F;

        _mousePosition = new(snapshot.MousePosition);
        _mouseDelta = _mousePosition - _mouseLastPosition;
        _mouseLastPosition = _mousePosition;

        foreach (KeyEvent keyEvent in snapshot.KeyEvents)
        {
            if (keyEvent.Down)
            {
                if (KeysDown.Add((Key)keyEvent.Key))
                    KeysPressed.Add((Key)keyEvent.Key);
            }

            else
            {
                if (KeysDown.Remove((Key)keyEvent.Key))
                    KeysReleased.Add((Key)keyEvent.Key);
            }
        }

        foreach (MouseEvent mouseEvent in snapshot.MouseEvents)
        {
            if (mouseEvent.Down)
            {
                if (MouseDown.Add((MouseButton)mouseEvent.MouseButton))
                    MousePressed.Add((MouseButton)mouseEvent.MouseButton);
            }

            else
            {
                if (MouseDown.Remove((MouseButton)mouseEvent.MouseButton))
                    MouseReleased.Add((MouseButton)mouseEvent.MouseButton);
            }
        }

        if (snapshot.WheelDelta != 0F)
            ScrollDelta = snapshot.WheelDelta;

        if (KeysPressed.Count > 0)
            KeyPressedEvent.Post();

        if (KeysReleased.Count > 0)
            KeyReleasedEvent.Post();

        if (MousePressed.Count > 0)
            MousePressedEvent.Post();

        if (MouseReleased.Count > 0)
            MouseReleasedEvent.Post();
    }

    /// <summary>
    ///     Get whether a specific key is currently down.
    /// </summary>
    /// <param name="key">Key to check.</param>
    /// <returns>Whether that key is currently down.</returns>
    public static bool IsKeyDown(Key key) => KeysDown.Contains(key);

    /// <summary>
    ///     Get whether a specific key has been recently pressed.
    /// </summary>
    /// <param name="key">Key to check.</param>
    /// <returns>Whether that key has been recently pressed.</returns>
    public static bool IsKeyPressed(Key key) => KeysPressed.Contains(key);

    /// <summary>
    ///     Get whether a specific key has been recently released.
    /// </summary>
    /// <param name="key">Key to check.</param>
    /// <returns>Whether that key has been recently released.</returns>
    public static bool IsKeyReleased(Key key) => KeysReleased.Contains(key);

    /// <summary>
    ///     Get whether a specific mouse button is currently down.
    /// </summary>
    /// <param name="button">Mouse button to check.</param>
    /// <returns>Whether that mouse button is currently down.</returns>
    public static bool IsMouseDown(MouseButton button) => MouseDown.Contains(button);

    /// <summary>
    ///     Get whether a specific mouse button has been recently pressed.
    /// </summary>
    /// <param name="button">Mouse button to check.</param>
    /// <returns>Whether that mouse button has been recently pressed.</returns>
    public static bool IsMousePressed(MouseButton button) => MousePressed.Contains(button);

    /// <summary>
    ///     Get whether a specific mouse button has been recently released.
    /// </summary>
    /// <param name="button">Mouse button to check.</param>
    /// <returns>Whether that mouse button has been recently released.</returns>
    public static bool IsMouseReleased(MouseButton button) => MouseReleased.Contains(button);
}

/// <summary>
///     Keyboard keys supported in Forge.
/// </summary>
public enum Key
{
    /// <summary>
    ///     A key outside the known keys.
    /// </summary>
    Unknown = 0,

    /// <summary>
    ///     The left-shift key.
    /// </summary>
    LeftShift = 1,

    /// <summary>
    ///     The right-shift key.
    /// </summary>
    RightShift = 2,

    /// <summary>
    ///     The left-control key.
    /// </summary>
    LeftControl = 3,

    /// <summary>
    ///     The right-control key.
    /// </summary>
    RightControl = 4,

    /// <summary>
    ///     The left-alt key.
    /// </summary>
    LeftAlt = 5,

    /// <summary>
    ///     The right-alt key.
    /// </summary>
    RightAlt = 6,

    /// <summary>
    ///     The left-win key.
    /// </summary>
    LeftWindows = 7,

    /// <summary>
    ///     The right-win key.
    /// </summary>
    RightWindows = 8,

    /// <summary>
    ///     The 'Menu' key.
    /// </summary>
    Menu = 9,

    /// <summary>
    ///     The 'F1' key.
    /// </summary>
    Function1 = 10,

    /// <summary>
    ///     The 'F2' key.
    /// </summary>
    Function2 = 11,

    /// <summary>
    ///     The 'F3' key.
    /// </summary>
    Function3 = 12,

    /// <summary>
    ///     The 'F4' key.
    /// </summary>
    Function4 = 13,

    /// <summary>
    ///     The 'F5' key.
    /// </summary>
    Function5 = 14,

    /// <summary>
    ///     The 'F6' key.
    /// </summary>
    Function6 = 15,

    /// <summary>
    ///     The 'F7' key.
    /// </summary>
    Function7 = 16,

    /// <summary>
    ///     The 'F8' key.
    /// </summary>
    Function8 = 17,

    /// <summary>
    ///     The 'F9' key.
    /// </summary>
    Function9 = 18,

    /// <summary>
    ///     The 'F10' key.
    /// </summary>
    Function10 = 19,

    /// <summary>
    ///     The 'F11' key.
    /// </summary>
    Function11 = 20,

    /// <summary>
    ///     The 'F12' key.
    /// </summary>
    Function12 = 21,

    /// <summary>
    ///     The 'F13' key.
    /// </summary>
    Function13 = 22,

    /// <summary>
    ///     The 'F14' key.
    /// </summary>
    Function14 = 23,

    /// <summary>
    ///     The 'F15' key.
    /// </summary>
    Function15 = 24,

    /// <summary>
    ///     The 'F16' key.
    /// </summary>
    Function16 = 25,

    /// <summary>
    ///     The 'F17' key.
    /// </summary>
    Function17 = 26,

    /// <summary>
    ///     The 'F18' key.
    /// </summary>
    Function18 = 27,

    /// <summary>
    ///     The 'F19' key.
    /// </summary>
    Function19 = 28,

    /// <summary>
    ///     The 'F20' key.
    /// </summary>
    Function20 = 29,

    /// <summary>
    ///     The 'F21' key.
    /// </summary>
    Function21 = 30,

    /// <summary>
    ///     The 'F22' key.
    /// </summary>
    Function22 = 31,

    /// <summary>
    ///     The 'F23' key.
    /// </summary>
    Function23 = 32,

    /// <summary>
    ///     The 'F24' key.
    /// </summary>
    Function24 = 33,

    /// <summary>
    ///     The 'F25' key.
    /// </summary>
    Function25 = 34,

    /// <summary>
    ///     The 'F26' key.
    /// </summary>
    Function26 = 35,

    /// <summary>
    ///     The 'F27' key.
    /// </summary>
    Function27 = 36,

    /// <summary>
    ///     The 'F28' key.
    /// </summary>
    Function28 = 37,

    /// <summary>
    ///     The 'F29' key.
    /// </summary>
    Function29 = 38,

    /// <summary>
    ///     The 'F30' key.
    /// </summary>
    Function30 = 39,

    /// <summary>
    ///     The 'F31' key.
    /// </summary>
    Function31 = 40,

    /// <summary>
    ///     The 'F32' key.
    /// </summary>
    Function32 = 41,

    /// <summary>
    ///     The 'F33' key.
    /// </summary>
    Function33 = 42,

    /// <summary>
    ///     The 'F34' key.
    /// </summary>
    Function34 = 43,

    /// <summary>
    ///     The 'F35' key.
    /// </summary>
    Function35 = 44,

    /// <summary>
    ///     The up-arrow key.
    /// </summary>
    Up = 45,

    /// <summary>
    ///     The down-arrow key.
    /// </summary>
    Down = 46,

    /// <summary>
    ///     The left-arrow key.
    /// </summary>
    Left = 47,

    /// <summary>
    ///     The right-arrow key.
    /// </summary>
    Right = 48,

    /// <summary>
    ///     The 'Enter' key.
    /// </summary>
    Enter = 49,

    /// <summary>
    ///     The 'Escape' key.
    /// </summary>
    Escape = 50,

    /// <summary>
    ///     The 'Space' key.
    /// </summary>
    Space = 51,

    /// <summary>
    ///     The 'Tab' key.
    /// </summary>
    Tab = 52,

    /// <summary>
    ///     The 'Backspace' key.
    /// </summary>
    Backspace = 53,

    /// <summary>
    ///     The 'Insert' key.
    /// </summary>
    Insert = 54,

    /// <summary>
    ///     The 'Delete' key.
    /// </summary>
    Delete = 55,

    /// <summary>
    ///     The 'PageUp' key.
    /// </summary>
    PageUp = 56,

    /// <summary>
    ///     The 'PageDown' key.
    /// </summary>
    PageDown = 57,

    /// <summary>
    ///     The 'Home' key.
    /// </summary>
    Home = 58,

    /// <summary>
    ///     The 'End' key.
    /// </summary>
    End = 59,

    /// <summary>
    ///     The 'CapsLock' key.
    /// </summary>
    CapsLock = 60,

    /// <summary>
    ///     The 'ScrollLock' key.
    /// </summary>
    ScrollLock = 61,

    /// <summary>
    ///     The 'PrintScreen' key.
    /// </summary>
    PrintScreen = 62,

    /// <summary>
    ///     The 'Pause' key.
    /// </summary>
    Pause = 63,

    /// <summary>
    ///     The 'NumLock' key.
    /// </summary>
    NumLock = 64,

    /// <summary>
    ///     The 'Clear' key (Keypad5 with NumLock disabled, on typical keyboards).
    /// </summary>
    Clear = 65,

    /// <summary>
    ///     The 'Sleep' key.
    /// </summary>
    Sleep = 66,

    /// <summary>
    ///     The keypad 0 key.
    /// </summary>
    Keypad0 = 67,

    /// <summary>
    ///     The keypad 1 key.
    /// </summary>
    Keypad1 = 68,

    /// <summary>
    ///     The keypad 2 key.
    /// </summary>
    Keypad2 = 69,

    /// <summary>
    ///     The keypad 3 key.
    /// </summary>
    Keypad3 = 70,

    /// <summary>
    ///     The keypad 4 key.
    /// </summary>
    Keypad4 = 71,

    /// <summary>
    ///     The keypad 5 key.
    /// </summary>
    Keypad5 = 72,

    /// <summary>
    ///     The keypad 6 key.
    /// </summary>
    Keypad6 = 73,

    /// <summary>
    ///     The keypad 7 key.
    /// </summary>
    Keypad7 = 74,

    /// <summary>
    ///     The keypad 8 key.
    /// </summary>
    Keypad8 = 75,

    /// <summary>
    ///     The keypad 9 key.
    /// </summary>
    Keypad9 = 76,

    /// <summary>
    ///     The keypad-slash key.
    /// </summary>
    KeypadSlash = 77,

    /// <summary>
    ///     The keypad-asterisk key.
    /// </summary>
    KeypadAsterisk = 78,

    /// <summary>
    ///     The Keypad-minus key.
    /// </summary>
    KeypadMinus = 79,

    /// <summary>
    ///     The keypad-plus key.
    /// </summary>
    KeypadPlus = 80,

    /// <summary>
    ///     The keypad-period key.
    /// </summary>
    KeypadPeriod = 81,

    /// <summary>
    ///     The keypad-enter key.
    /// </summary>
    KeypadEnter = 82,

    /// <summary>
    ///     The 'A' key.
    /// </summary>
    A = 83,

    /// <summary>
    ///     The 'B' key.
    /// </summary>
    B = 84,

    /// <summary>
    ///     The 'C' key.
    /// </summary>
    C = 85,

    /// <summary>
    ///     The 'D' key.
    /// </summary>
    D = 86,

    /// <summary>
    ///     The 'E' key.
    /// </summary>
    E = 87,

    /// <summary>
    ///     The 'F' key.
    /// </summary>
    F = 88,

    /// <summary>
    ///     The 'G' key.
    /// </summary>
    G = 89,

    /// <summary>
    ///     The 'H' key.
    /// </summary>
    H = 90,

    /// <summary>
    ///     The 'I' key.
    /// </summary>
    I = 91,

    /// <summary>
    ///     The 'J' key.
    /// </summary>
    J = 92,

    /// <summary>
    ///     The 'K' key.
    /// </summary>
    K = 93,

    /// <summary>
    ///     The 'L' key.
    /// </summary>
    L = 94,

    /// <summary>
    ///     The 'M' key.
    /// </summary>
    M = 95,

    /// <summary>
    ///     The 'N' key.
    /// </summary>
    N = 96,

    /// <summary>
    ///     The 'O' key.
    /// </summary>
    O = 97,

    /// <summary>
    ///     The 'P' key.
    /// </summary>
    P = 98,

    /// <summary>
    ///     The 'Q' key.
    /// </summary>
    Q = 99,

    /// <summary>
    ///     The 'R' key.
    /// </summary>
    R = 100,

    /// <summary>
    ///     The 'S' key.
    /// </summary>
    S = 101,

    /// <summary>
    ///     The 'T' key.
    /// </summary>
    T = 102,

    /// <summary>
    ///     The 'U' key.
    /// </summary>
    U = 103,

    /// <summary>
    ///     The 'V' key.
    /// </summary>
    V = 104,

    /// <summary>
    ///     The 'W' key.
    /// </summary>
    W = 105,

    /// <summary>
    ///     The 'X' key.
    /// </summary>
    X = 106,

    /// <summary>
    ///     The 'Y' key.
    /// </summary>
    Y = 107,

    /// <summary>
    ///     The 'Z' key.
    /// </summary>
    Z = 108,

    /// <summary>
    ///     The number 0 key.
    /// </summary>
    Number0 = 109,

    /// <summary>
    ///     The number 1 key.
    /// </summary>
    Number1 = 110,

    /// <summary>
    ///     The number 2 key.
    /// </summary>
    Number2 = 111,

    /// <summary>
    ///     The number 3 key.
    /// </summary>
    Number3 = 112,

    /// <summary>
    ///     The number 4 key.
    /// </summary>
    Number4 = 113,

    /// <summary>
    ///     The number 5 key.
    /// </summary>
    Number5 = 114,

    /// <summary>
    ///     The number 6 key.
    /// </summary>
    Number6 = 115,

    /// <summary>
    ///     The number 7 key.
    /// </summary>
    Number7 = 116,

    /// <summary>
    ///     The number 8 key.
    /// </summary>
    Number8 = 117,

    /// <summary>
    ///     The number 9 key.
    /// </summary>
    Number9 = 118,

    /// <summary>
    ///     The '`' key.
    /// </summary>
    Tilde = 119,

    /// <summary>
    ///     The '-' key.
    /// </summary>
    Minus = 120,

    /// <summary>
    ///     The '+' key.
    /// </summary>
    Plus = 121,

    /// <summary>
    ///     The '[' key.
    /// </summary>
    LeftBracket = 122,

    /// <summary>
    ///     The ']' key.
    /// </summary>
    RightBracket = 123,

    /// <summary>
    ///     The ';' key.
    /// </summary>
    Semicolon = 124,

    /// <summary>
    ///     The "'" key.
    /// </summary>
    Quote = 125,

    /// <summary>
    ///     The ',' key.
    /// </summary>
    Comma = 126,

    /// <summary>
    ///     The '.' key.
    /// </summary>
    Period = 127,

    /// <summary>
    ///     The '/' key.
    /// </summary>
    Slash = 128,

    /// <summary>
    ///     The '\' key.
    /// </summary>
    Backslash = 129,

    /// <summary>
    ///     The secondary backslash key.
    /// </summary>
    NonUsBackSlash = 130,

    /// <summary>
    ///     Indicates the last available keyboard key.
    /// </summary>
    LastKey = 131
}

/// <summary>
///     Mouse buttons supported in Forge.
/// </summary>
public enum MouseButton
{
    /// <summary>
    ///     The left mouse button.
    /// </summary>
    Left = 0,

    /// <summary>
    ///     The middle mouse button.
    /// </summary>
    Middle = 1,

    /// <summary>
    ///     The right mouse button.
    /// </summary>
    Right = 2,

    /// <summary>
    ///     First extra mouse button.
    /// </summary>
    Button1 = 3,

    /// <summary>
    ///     Second extra mouse button.
    /// </summary>
    Button2 = 4,

    /// <summary>
    ///     Third extra mouse button.
    /// </summary>
    Button3 = 5,

    /// <summary>
    ///     Fourth extra mouse button.
    /// </summary>
    Button4 = 6,

    /// <summary>
    ///     Fifth extra mouse button.
    /// </summary>
    Button5 = 7,

    /// <summary>
    ///     Sixth extra mouse button.
    /// </summary>
    Button6 = 8,

    /// <summary>
    ///     Seventh extra mouse button.
    /// </summary>
    Button7 = 9,

    /// <summary>
    ///     Eighth extra mouse button.
    /// </summary>
    Button8 = 10,

    /// <summary>
    ///     Ninth extra mouse button.
    /// </summary>
    Button9 = 11,

    /// <summary>
    ///     Last available mouse button.
    /// </summary>
    LastButton = 12
}