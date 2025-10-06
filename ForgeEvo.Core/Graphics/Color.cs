using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using Veldrid;

namespace ForgeEvo.Core.Graphics;

/// <summary>
///     Represents a color in ARGB format with 32-bit precision.
/// </summary>
/// <remarks>
///     This struct encodes the alpha, red, green, and blue components of a color into a single 32-bit unsigned integer.
///     Each color component is stored in 8 bits, with values ranging from 0 to 255. The alpha component determines the
///     transparency of the color.
/// </remarks>
[StructLayout(LayoutKind.Sequential, Size = 4)]
public readonly struct Color : IEquatable<Color>
{
    /// <summary>
    ///     Internal representation of the color as an ARGB value.
    /// </summary>
    private readonly uint _argb;

    /// <summary>
    ///     Create a new color using its channels.
    /// </summary>
    /// <param name="red">Red channel of the color.</param>
    /// <param name="green">Green channel of the color.</param>
    /// <param name="blue">Blue channel of the color.</param>
    /// <param name="alpha">Alpha (transparency) channel of the color.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Color(byte red, byte green, byte blue, byte alpha = 255) =>
        _argb = ((uint)alpha << 24) | ((uint)red << 16) | ((uint)green << 8) | blue;

    /// <summary>
    ///     Create a new color directly using its ARGB value.
    /// </summary>
    /// <param name="argb">ARGB value of the color.</param>
    private Color(uint argb) => _argb = argb;

    /// <summary>
    ///     Red channel of the color.
    /// </summary>
    public byte Red
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (byte)((_argb >> 16) & 0xFF);
    }

    /// <summary>
    ///     Green channel of the color.
    /// </summary>
    public byte Green
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (byte)((_argb >> 8) & 0xFF);
    }

    /// <summary>
    ///     Blue channel of the color.
    /// </summary>
    public byte Blue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (byte)(_argb & 0xFF);
    }

    /// <summary>
    ///     Alpha channel of the color which represents the transparency.
    /// </summary>
    public byte Alpha
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (byte)((_argb >> 24) & 0xFF);
    }

    #region Constants

    /// <summary>
    ///     Default transparent color with all its channels set to 0.
    /// </summary>
    public static readonly Color Transparent = new(0, 0, 0, 0);

    /// <summary>
    ///     Default white color with all its channels set to 255.
    /// </summary>
    public static readonly Color White = new(255, 255, 255);

    /// <summary>
    ///     Default black color with all its channels, except alpha, set to 0. The alpha channel is set to 255 for a solid
    ///     black.
    /// </summary>
    public static readonly Color Black = new(0, 0, 0);

    #endregion

    #region Operators

    /// <summary>
    ///     Compute the sum of two colors, clamped to a maximum of 255 on each channel.
    /// </summary>
    /// <param name="a">First color.</param>
    /// <param name="b">Second color.</param>
    /// <returns>Summed color.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color operator +(Color a, Color b) => new(SaturatingAdd(a._argb, b._argb));

    /// <summary>
    ///     Compute the difference between two colors, clamped to a minimum of 0 on each channel.
    /// </summary>
    /// <param name="a">First color.</param>
    /// <param name="b">Second color.</param>
    /// <returns>Difference color.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color operator -(Color a, Color b) => new(SaturatingSubtract(a._argb, b._argb));

    /// <summary>
    ///     Multiply a color by a scalar, clamped to a maximum of 255 on each channel.
    /// </summary>
    /// <param name="color">Color to multiply.</param>
    /// <param name="scalar">Scalar to multiply by the color by.</param>
    /// <returns>Color multiplied by the scalar.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color operator *(Color color, float scalar)
    {
        var red = (int)(color.Red * scalar + 0.5F);
        var green = (int)(color.Green * scalar + 0.5F);
        var blue = (int)(color.Blue * scalar + 0.5F);
        var alpha = (int)(color.Alpha * scalar + 0.5F);

        if ((uint)red > 255)
            red = red < 0 ? 0 : 255;

        if ((uint)green > 255)
            green = green < 0 ? 0 : 255;

        if ((uint)blue > 255)
            blue = blue < 0 ? 0 : 255;

        if ((uint)alpha > 255)
            alpha = alpha < 0 ? 0 : 255;

        return new((byte)red, (byte)green, (byte)blue, (byte)alpha);
    }

    /// <summary>
    ///     Check whether two colors are equal. They are equal if they have equal internal ARGB representations.
    /// </summary>
    /// <param name="a">First color.</param>
    /// <param name="b">Second color.</param>
    /// <returns>Whether the two colors are equal.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Color a, Color b) => a._argb == b._argb;

    /// <summary>
    ///     Check whether two colors are not equal. They are not equal if they have different internal ARGB representations.
    /// </summary>
    /// <param name="a">First color.</param>
    /// <param name="b">Second color.</param>
    /// <returns>Whether the two colors are not equal.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Color a, Color b) => !(a == b);

    #endregion

    #region Methods

    /// <summary>
    ///     Convert the color to a Veldrid <see cref="RgbaFloat" />.
    /// </summary>
    /// <returns><see cref="RgbaFloat" /> with channel values.</returns>
    internal RgbaFloat ToRgbaFloat() => new(Red / 255F, Green / 255F, Blue / 255F, Alpha / 255F);

    #endregion

    #region Equality / Overrides

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Color other) => _argb == other._argb;

    public override bool Equals(object? obj) => obj is Color other && Equals(other);

    public override string ToString() => $"Color(Red: {Red}, Green: {Green}, Blue: {Blue}, Alpha: {Alpha})";

    public override int GetHashCode() => (int)_argb;

    #endregion

    #region Intrinsic Helpers

    /// <summary>
    ///     Performs a saturating addition of two 32-bit ARGB color values. Ensures that each channel (red, green, blue, and
    ///     alpha) does not exceed the maximum value of 255. Uses hardware acceleration where supported.
    /// </summary>
    /// <param name="a">The first ARGB color value.</param>
    /// <param name="b">The second ARGB color value.</param>
    /// <returns>A 32-bit ARGB color value resulting from the saturating addition of the input values.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint SaturatingAdd(uint a, uint b)
    {
        if (Sse2.IsSupported)
        {
            Vector128<byte> vectorA = Vector128.CreateScalarUnsafe(a).AsByte();
            Vector128<byte> vectorB = Vector128.CreateScalarUnsafe(b).AsByte();

            Vector128<byte> result = Sse2.AddSaturate(vectorA, vectorB);
            return result.AsUInt32().GetElement(0);
        }

        if (AdvSimd.IsSupported)
        {
            Vector128<byte> vectorA = Vector128.CreateScalarUnsafe(a).AsByte();
            Vector128<byte> vectorB = Vector128.CreateScalarUnsafe(b).AsByte();

            Vector128<byte> result = AdvSimd.AddSaturate(vectorA, vectorB);
            return result.AsUInt32().GetElement(0);
        }

        int red = a.ExtractByte(2) + b.ExtractByte(2);
        int green = a.ExtractByte(1) + b.ExtractByte(1);
        int blue = a.ExtractByte(0) + b.ExtractByte(0);
        int alpha = a.ExtractByte(3) + b.ExtractByte(3);

        if (red > 255)
            red = 255;

        if (green > 255)
            green = 255;

        if (blue > 255)
            blue = 255;

        if (alpha > 255)
            alpha = 255;

        return PackArgb((byte)red, (byte)green, (byte)blue, (byte)alpha);
    }

    /// <summary>
    ///     Performs a saturating subtraction of two 32-bit ARGB color values. Ensures that each channel (red, green, blue, and
    ///     alpha) does not exceed below the minimum value of 0. Uses hardware acceleration where supported.
    /// </summary>
    /// <param name="a">The first ARGB color value.</param>
    /// <param name="b">The second ARGB color value.</param>
    /// <returns>A 32-bit ARGB color value resulting from the saturating subtraction of the input values.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint SaturatingSubtract(uint a, uint b)
    {
        if (Sse2.IsSupported)
        {
            Vector128<byte> vectorA = Vector128.CreateScalarUnsafe(a).AsByte();
            Vector128<byte> vectorB = Vector128.CreateScalarUnsafe(b).AsByte();

            Vector128<byte> result = Sse2.SubtractSaturate(vectorA, vectorB);
            return result.AsUInt32().GetElement(0);
        }

        if (AdvSimd.IsSupported)
        {
            Vector128<byte> vectorA = Vector128.CreateScalarUnsafe(a).AsByte();
            Vector128<byte> vectorB = Vector128.CreateScalarUnsafe(b).AsByte();

            Vector128<byte> result = AdvSimd.SubtractSaturate(vectorA, vectorB);
            return result.AsUInt32().GetElement(0);
        }

        int red = a.ExtractByte(2) - b.ExtractByte(2);
        int green = a.ExtractByte(1) - b.ExtractByte(1);
        int blue = a.ExtractByte(0) - b.ExtractByte(0);
        int alpha = a.ExtractByte(3) - b.ExtractByte(3);

        if (red < 0)
            red = 0;

        if (green < 0)
            green = 0;

        if (blue < 0)
            blue = 0;

        if (alpha < 0)
            alpha = 0;

        return PackArgb((byte)red, (byte)green, (byte)blue, (byte)alpha);
    }

    /// <summary>
    ///     Combines the individual red, green, blue, and alpha channel values into a single ARGB value.
    /// </summary>
    /// <param name="red">The red channel of the color.</param>
    /// <param name="green">The green channel of the color.</param>
    /// <param name="blue">The blue channel of the color.</param>
    /// <param name="alpha">The alpha (transparency) channel of the color.</param>
    /// <returns>A single 32-bit unsigned integer representing the combined ARGB value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint PackArgb(byte red, byte green, byte blue, byte alpha) =>
        ((uint)alpha << 24) | ((uint)red << 16) | ((uint)green << 8) | blue;

    #endregion
}

/// <summary>
///     Provides extension methods for operations on packed color data represented as unsigned 32-bit integers.
/// </summary>
/// <remarks>
///     This static class contains utility methods for extracting and manipulating individual color components (e.g., red,
///     green,
///     blue, and alpha) from a packed 32-bit ARGB or related color format. These operations facilitate low-level
///     interaction
///     with packed color data.
/// </remarks>
file static class ColorPackedExtensions
{
    /// <summary>
    ///     Extracts a specific byte from a packed 32-bit unsigned integer.
    /// </summary>
    /// <param name="value">The 32-bit unsigned integer from which a byte will be extracted.</param>
    /// <param name="index">
    ///     The index of the byte to extract (3 for the alpha byte, 2 for the red byte, 1 for the green byte,
    ///     and, 0 for the blue byte).
    /// </param>
    /// <returns>The extracted byte from the specified index.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static byte ExtractByte(this uint value, int index) => (byte)((value >> (index * 8)) & 0xFF);
}