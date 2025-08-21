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
    ///     Update the input handler with the latest keyboard and mouse data. Ideally called every game-loop.
    /// </summary>
    /// <param name="snapshot">Current Verldrid input snapshot which contains the latest keyboard and mouse data.</param>
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
            if (keyEvent.Down)
            {
                if (KeysDown.Add(keyEvent.Key))
                    KeysPressed.Add(keyEvent.Key);
            }

            else
            {
                if (KeysDown.Remove(keyEvent.Key))
                    KeysReleased.Add(keyEvent.Key);
            }

        foreach (MouseEvent mouseEvent in snapshot.MouseEvents)
            if (mouseEvent.Down)
            {
                if (MouseDown.Add(mouseEvent.MouseButton))
                    MousePressed.Add(mouseEvent.MouseButton);
            }

            else
            {
                if (MouseDown.Remove(mouseEvent.MouseButton))
                    MouseReleased.Add(mouseEvent.MouseButton);
            }

        if (snapshot.WheelDelta != 0F)
            ScrollDelta = snapshot.WheelDelta;
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