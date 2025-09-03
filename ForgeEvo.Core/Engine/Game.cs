using Veldrid;

namespace ForgeEvo.Core.Engine;

/// <summary>
///     Base game class for Forge.
/// </summary>
public abstract class Game
{
    /// <summary>
    ///     Event fired once the game has completed initialization.
    /// </summary>
    private readonly Event _gameInitializedEvent;

    /// <summary>
    ///     Event fired every time the game is rendered in the main game loop.
    /// </summary>
    private readonly Event _gameRenderedEvent;

    /// <summary>
    ///     Event fired every time the game is updated in the main game loop.
    /// </summary>
    private readonly Event _gameUpdatedEvent;

    /// <summary>
    ///     Display used by the game for rendering and managing the window. Provides functionality to create and configure a
    ///     game window, handle window events, and render graphics to the screen.
    /// </summary>
    protected readonly Display Display;

    /// <summary>
    ///     Whether the game is running.
    /// </summary>
    private bool _running = true;

    /// <summary>
    ///     Create a new game and register its corresponding events. Provides core functionality to manage the game loop,
    ///     rendering, and updating logic.
    /// </summary>
    protected Game(uint width = 800, uint height = 600, string title = "Forge Evo")
    {
        Display = new(width, height, title);
        Display.Window.Closed += () => _running = false;

        EventBus.RegisterInternalEvents(
            InternalEvent.GameInitialized,
            InternalEvent.GameUpdated,
            InternalEvent.GameRendered
        );

        _gameInitializedEvent = EventBus.GetByName(InternalEvent.GameInitialized.ToName());
        _gameUpdatedEvent = EventBus.GetByName(InternalEvent.GameUpdated.ToName());
        _gameRenderedEvent = EventBus.GetByName(InternalEvent.GameRendered.ToName());
    }

    /// <summary>
    ///     Runs the game initializer, starts the main game loop, and keeps the game running until explicitly stopped.
    /// </summary>
    public void Run()
    {
        Initialize();
        _gameInitializedEvent.Post();

        try
        {
            while (_running)
            {
                InputSnapshot input = Display.Window.PumpEvents();
                InputHandler.Update(input);

                Update();
                _gameUpdatedEvent.Post();

                Render();
                _gameRenderedEvent.Post();
            }
        }

        finally
        {
            Display.Dispose();
        }
    }

    /// <summary>
    ///     Initialization method for the game, called once before the main loop starts.
    /// </summary>
    protected abstract void Initialize();

    /// <summary>
    ///     Update method for the game.
    /// </summary>
    protected abstract void Update();

    /// <summary>
    ///     Render method for the game.
    /// </summary>
    protected abstract void Render();
}