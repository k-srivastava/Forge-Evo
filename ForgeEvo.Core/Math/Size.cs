using System.Runtime.CompilerServices;

namespace ForgeEvo.Core.Math;

/// <summary>
///     Forge's representation of an immutable 2D size.
/// </summary>
/// <param name="width">Width of the size.</param>
/// <param name="height">Height of the size.</param>
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct Size2D(uint width, uint height) : IEquatable<Size2D>
{
    /// <summary>
    ///     Width component of the 2D size in pixels.
    /// </summary>
    public readonly uint Width = width;

    /// <summary>
    ///     Height component of the 2D size in pixels.
    /// </summary>
    public readonly uint Height = height;

    #region Constants

    /// <summary>
    ///     Default zero size with zero width and height.
    /// </summary>
    public static readonly Size2D Zero = new(0, 0);

    /// <summary>
    ///     Default unit size with width and height set to 1.
    /// </summary>
    public static readonly Size2D One = new(1, 1);

    /// <summary>
    ///     Default widthways size with width set to 1.
    /// </summary>
    public static readonly Size2D Widthways = new(1, 0);

    /// <summary>
    ///     Default heightways size with height set to 1.
    /// </summary>
    public static readonly Size2D Heightways = new(0, 1);

    #endregion

    #region Conversions

    /// <summary>
    ///     Convert a Forge <c>Size2D</c> into a standard <see cref="Vector2D" />.
    /// </summary>
    /// <param name="size">Size to convert.</param>
    /// <returns>Standard <see cref="Vector2D" /> with the same components as floats.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Vector2D(Size2D size) => new(size.Width, size.Height);

    /// <summary>
    ///     Convert a standard <see cref="Vector2D" /> into a Forge <c>Size2D</c>.
    /// </summary>
    /// <param name="vector">Standard vector to convert.</param>
    /// <returns>Forge <c>Size2D</c> with the same components as uints.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Size2D(Vector2D vector) => new((uint)vector.X, (uint)vector.Y);

    #endregion

    #region Operators

    /// <summary>
    ///     Compute the sum of two sizes.
    /// </summary>
    /// <param name="a">First size.</param>
    /// <param name="b">Second size.</param>
    /// <returns>Summed size.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Size2D operator +(Size2D a, Size2D b) => new(a.Width + b.Width, a.Height + b.Height);

    /// <summary>
    ///     Compute the difference between two sizes.
    /// </summary>
    /// <param name="a">First size.</param>
    /// <param name="b">Second size.</param>
    /// <returns>Difference size.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Size2D operator -(Size2D a, Size2D b) => new(a.Width - b.Width, a.Height - b.Height);

    /// <summary>
    ///     Multiply a size with a scalar.
    /// </summary>
    /// <param name="size">Size to multiply.</param>
    /// <param name="scale">Scalar to multiply the size by.</param>
    /// <returns>Size multiplied by the scalar.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Size2D operator *(Size2D size, uint scale) => new(size.Width * scale, size.Height * scale);

    /// <summary>
    ///     Myltuply a size with a scalar.
    /// </summary>
    /// <param name="scale">Scalar to multiply the size by.</param>
    /// <param name="size">Size to multiply.</param>
    /// <returns>Size multiplied by the scalar.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Size2D operator *(uint scale, Size2D size) => new(scale * size.Width, scale * size.Height);

    /// <summary>
    ///     Divide a size by a scalar.
    /// </summary>
    /// <param name="size">Size to divide.</param>
    /// <param name="scale">Scalar to divide the size by.</param>
    /// <returns>Size divided by the scalar.</returns>
    /// <exception cref="DivideByZeroException">Sizes cannot be divided by a zero scalar.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Size2D operator /(Size2D size, uint scale) => scale == 0
        ? throw new DivideByZeroException("Cannot divide a size by zero.")
        : new(size.Width / scale, size.Height / scale);

    /// <summary>
    ///     Check whether two sizes are equal. They are equal if they have equal width and height components.
    /// </summary>
    /// <param name="a">First size.</param>
    /// <param name="b">Second size.</param>
    /// <returns>Whether the two sizes are equal.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Size2D a, Size2D b) => a.Width == b.Width && a.Height == b.Height;

    /// <summary>
    ///     Check whether the two sizes are not equal. They are not equal if they have dissimimilar width or height components.
    /// </summary>
    /// <param name="a">First size.</param>
    /// <param name="b">Second size.</param>
    /// <returns>Whether the two sizes are not equal.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Size2D a, Size2D b) => !(a == b);

    #endregion

    #region Equality / Overrides

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Size2D other) => Width.Equals(other.Width) && Height.Equals(other.Height);

    public override bool Equals(object? obj) => obj is Size2D other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Width, Height);

    #endregion
}