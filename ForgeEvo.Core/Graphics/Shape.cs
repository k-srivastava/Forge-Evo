using ForgeEvo.Core.Engine;
using ForgeEvo.Core.Math;

namespace ForgeEvo.Core.Graphics;

/// <summary>
///     Default interface for a shape.
/// </summary>
public interface IShape
{
    /// <summary>
    ///     Center of the shape's bounding box.
    /// </summary>
    Vector2D Center { get; set; }

    /// <summary>
    ///     Top-left of the shape's bounding box.
    /// </summary>
    Vector2D TopLeft { get; set; }

    /// <summary>
    ///     Size of the shape's bounding box.
    /// </summary>
    Size2D Size { get; set; }
}

/// <summary>
///     Circle shape.
/// </summary>
public struct Circle : IShape, IRenderable
{
    /// <summary>
    ///     Minimum number of segments used to render the circle.
    /// </summary>
    public const uint MinSegments = 3;

    /// <summary>
    ///     Maximum number of segments used to render the circle.
    /// </summary>
    public const uint MaxSegments = 64;

    /// <summary>
    ///     Center of the circle.
    /// </summary>
    private Vector2D _center;

    /// <summary>
    ///     Size of the circle.
    /// </summary>
    /// <remarks>
    ///     Equivalent to a size with diameter as width and height.
    /// </remarks>
    private Size2D _size;

    /// <summary>
    ///     Radius of the circle.
    /// </summary>
    public float Radius;

    /// <summary>
    ///     Color of the circle.
    /// </summary>
    public Color Color;

    /// <summary>
    ///     Number of segments in the circle.
    /// </summary>
    public readonly uint Segments;

    /// <summary>
    ///     Create a new circle.
    /// </summary>
    /// <param name="center">Center of the circle.</param>
    /// <param name="radius">Radius of the circle.</param>
    /// <param name="color">Color of the circle.</param>
    /// <param name="segments">Number of segments in the circle.</param>
    public Circle(Vector2D center, float radius, Color color, uint segments = 32)
    {
        _center = center;
        Radius = radius;
        Color = color;
        Segments = System.Math.Clamp(segments, MinSegments, MaxSegments);

        var diameter = (uint)(radius * 2);
        _size = new(diameter, diameter);
    }

    public Vector2D TopLeft
    {
        get => new(_center.X - Radius, _center.Y - Radius);
        set => _center = new(value.X + Radius, value.Y + Radius);
    }

    public Vector2D Center
    {
        get => _center;
        set => _center = value;
    }

    public Size2D Size
    {
        get => _size;
        set
        {
            _size = value;
            Radius = value.Width / 2F;
        }
    }

    public void Enqueue()
    {
        if (!Display.Instance.Renderer.TryEnqueue(this))
            throw new InvalidOperationException("Master renderer does not have a circle renderer.");
    }
}