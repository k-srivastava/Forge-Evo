using ForgeEvo.Core.Engine;
using ForgeEvo.Core.Math;

namespace ForgeEvo.Core.Graphics;

/// <summary>
///     Represents a <see cref="Sprite" /> that can be rendered to a <see cref="Display" /> via its
///     <see cref="SpriteRenderer" /> along with position and scale.
/// </summary>
public struct Image : IDisposable
{
    /// <summary>
    ///     Internal sprite of the image.
    /// </summary>
    internal readonly Sprite Sprite;

    /// <summary>
    ///     Unique indeitifier of the sprite for quick reference.
    /// </summary>
    public readonly int SpriteId;

    /// <summary>
    ///     Position of the image on the display from the top-left.
    /// </summary>
    public MutableVector2D Position;

    /// <summary>
    ///     Scale of the image applied to the base sprite.
    /// </summary>
    public MutableVector2D Scale;

    /// <summary>
    ///     Create a new image using an exisitng sprite referenced by its ID in the <see cref="SpriteRegistry" />.
    /// </summary>
    /// <param name="spriteId">ID of the sprite used by the image.</param>
    /// <param name="position">Position of the image.</param>
    /// <param name="scale">Scale of the image.</param>
    /// <exception cref="ArgumentException">The sprite ID must reference an existing sprite.</exception>
    public Image(int spriteId, Vector2D position, Vector2D scale)
    {
        SpriteId = spriteId;
        Position = position;
        Scale = scale;

        Sprite? rawSprite = SpriteRegistry.GetById(spriteId);
        Sprite = rawSprite ?? throw new ArgumentException($"Invalid sprite ID {spriteId}.");
    }

    /// <summary>
    ///     Create an new image directly from a sprite.
    /// </summary>
    /// <param name="sprite">Sprite to be used by the iamge.</param>
    /// <param name="position">Position of the image.</param>
    /// <param name="scale">Scale of the image.</param>
    public Image(Sprite sprite, Vector2D position, Vector2D scale)
    {
        SpriteId = sprite.Id;
        Position = position;
        Scale = scale;
        Sprite = sprite;
    }

    /// <summary>
    ///     Size of the image taking into account the sprite size and image scaling.
    /// </summary>
    public Size2D Size => new(
        (uint)System.Math.Abs(Sprite.Size.Width * Scale.X), (uint)System.Math.Abs(Sprite.Size.Height * Scale.Y)
    );

    #region IDisposable Members

    public void Dispose()
    {
        Sprite.Dispose();
    }

    #endregion

    /// <summary>
    ///     Render the image's sprite at its current position and scale.
    /// </summary>
    public void Render()
    {
        Display.Instance.SpriteRenderer.AddToDrawList(this);
    }

    public override string ToString() => $"Image(Sprite: {Sprite}, Position: {Position}, Scale: {Scale})";
}