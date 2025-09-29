using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using ForgeEvo.Core.Engine;
using ForgeEvo.Core.Math;
using StbImageSharp;
using Veldrid;

namespace ForgeEvo.Core.Graphics;

/// <summary>
///     Represents a 2D sprite that can be rendered to a <see cref="Engine.Display" /> via its
///     <see cref="SpriteRenderer" />.
/// </summary>
public class Sprite : IDisposable
{
    /// <summary>
    ///     Veldrid texture used to render the sprite.
    /// </summary>
    private readonly Texture _texture;

    /// <summary>
    ///     Display to which the sprite is bound.
    /// </summary>
    internal readonly Display Display;

    /// <summary>
    ///     Unique ID of the sprite.
    /// </summary>
    public readonly int Id;

    /// <summary>
    ///     Size of the raw sprite in pixels.
    /// </summary>
    public readonly Size2D Size;

    /// <summary>
    ///     Veldrid texture view of the sprite.
    /// </summary>
    internal readonly TextureView TextureView;

    /// <summary>
    ///     Whether the sprite has been disposed.
    /// </summary>
    private bool _disposed;

    /// <summary>
    ///     Create a new sprite and register it with the <see cref="SpriteRegistry" />.
    /// </summary>
    /// <param name="display">Display to which the sprite is bound.</param>
    /// <param name="sourcePath">Source path of the image to load into the sprite.</param>
    /// <exception cref="FileNotFoundException">The <c>sourcePath</c> must lead to a valid exisiting file.</exception>
    public Sprite(Display display, string sourcePath)
    {
        if (!File.Exists(sourcePath))
            throw new FileNotFoundException("Sprite file not found.", sourcePath);

        ImageResult result = ImageResult.FromStream(File.OpenRead(sourcePath), ColorComponents.RedGreenBlueAlpha);

        Size = new((uint)result.Width, (uint)result.Height);
        Display = display;

        _texture = display.Device.ResourceFactory.CreateTexture(TextureDescription.Texture2D(
            Size.Width, Size.Height, 1, 1, PixelFormat.R8_G8_B8_A8_UNorm, TextureUsage.Sampled
        ));
        TextureView = display.Device.ResourceFactory.CreateTextureView(_texture);

        display.Device.UpdateTexture(_texture, result.Data, 0, 0, 0, Size.Width, Size.Height, 1, 0, 0);

        Id = SpriteRegistry.NextId();
        SpriteRegistry.Register(this);
    }

    #region IDisposable Members

    public void Dispose()
    {
        if (_disposed)
            return;

        _texture.Dispose();
        TextureView.Dispose();

        SpriteRegistry.Unregister(this);

        _disposed = true;

        GC.SuppressFinalize(this);
    }

    #endregion
}

/// <summary>
///     Provides a centralized, thread-safe system to manage the retrieval and deletion of sprites.
/// </summary>
public static class SpriteRegistry
{
    /// <summary>
    ///     A thread-safe dictionary that maps unique sprite IDs to their corresponding <see cref="Sprite" /> instances.
    /// </summary>
    private static readonly ConcurrentDictionary<int, WeakReference<Sprite>> SpritesById = [];

    /// <summary>
    ///     Tracks the next unique identifier for events, ensuring sequential and thread-safe ID generation.
    /// </summary>
    private static int _nextId;

    /// <summary>
    ///     Generate a list of all active sprites in the registry.
    /// </summary>
    public static IEnumerable<Sprite> ActiveSprites
    {
        get
        {
            foreach (WeakReference<Sprite> weafRef in SpritesById.Values)
            {
                if (weafRef.TryGetTarget(out Sprite? sprite))
                    yield return sprite;
            }
        }
    }

    /// <summary>
    ///     Generates the next unique identifier for sprites in a thread-safe manner.
    /// </summary>
    /// <returns>The next unqiue integer identifier.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int NextId() => Interlocked.Increment(ref _nextId);

    /// <summary>
    ///     Register an existing sprite with the registry.
    /// </summary>
    /// <param name="sprite">Sprite to register.</param>
    internal static void Register(Sprite sprite)
    {
        SpritesById[sprite.Id] = new(sprite);
    }

    /// <summary>
    ///     Unregister an exisiting sprite from the registry. Silently fails if the sprite is not registered.
    /// </summary>
    /// <param name="sprite">Sprite to unregister.</param>
    internal static void Unregister(Sprite sprite)
    {
        SpritesById.TryRemove(sprite.Id, out _);
    }

    /// <summary>
    ///     Attemps to retrieve a sprite by its unique ID.
    /// </summary>
    /// <param name="id">The unique identifier of the sprite.</param>
    /// <param name="sprite">The sprite associated with the given ID, or <c>null</c> if no such sprite exists.</param>
    /// <returns><c>true</c> if a sprite with the specified ID is found; otherwise, <c>false</c>.</returns>
    public static bool TryGetById(int id, out Sprite? sprite)
    {
        if (SpritesById.TryGetValue(id, out WeakReference<Sprite>? weakRef) && weakRef.TryGetTarget(out sprite))
            return true;

        sprite = null;
        return false;
    }

    /// <summary>
    ///     Retrieves a sprite by its unique ID.
    /// </summary>
    /// <param name="id">The unique identifier of the sprite.</param>
    /// <returns>The sprite associated with the given ID.</returns>
    /// <exception cref="InvalidOperationException">Thrown if a sprite with the specified ID does not exist.</exception>
    public static Sprite GetById(int id) =>
        SpritesById.TryGetValue(id, out WeakReference<Sprite>? weakRef) && weakRef.TryGetTarget(out Sprite? sprite)
            ? sprite
            : throw new InvalidOperationException($"Sprite with id {id} does not exist.");

    /// <summary>
    ///     Remove all sprites whose references have been garbage collected.
    /// </summary>
    public static void CleanUp()
    {
        foreach (int id in SpritesById.Keys)
        {
            if (SpritesById.TryGetValue(id, out WeakReference<Sprite>? weakRef) && !weakRef.TryGetTarget(out _))
                SpritesById.TryRemove(id, out _);
        }
    }
}