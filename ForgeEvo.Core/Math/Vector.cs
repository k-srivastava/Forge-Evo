using System.Numerics;

namespace ForgeEvo.Core.Math;

/// <summary>
///     Forge's representation of an immutable SIMD 2D vector.
/// </summary>
public readonly struct Vector2D : IEquatable<Vector2D>
{
    /// <summary>
    ///     Internal <c>Vector2</c> storage to enable SIMD operations.
    /// </summary>
    private readonly Vector2 _vector;

    /// <summary>
    ///     Create a new vector using its components.
    /// </summary>
    /// <param name="x">X coordinate of the vector.</param>
    /// <param name="y">Y coordinate of the vector.</param>
    public Vector2D(float x, float y) => _vector = new(x, y);

    /// <summary>
    ///     Create a new vector using <c>Vector2</c> directly.
    /// </summary>
    /// <param name="vector"><c>Vector2</c> to convert into a Forge <c>Vector2D</c>.</param>
    public Vector2D(Vector2 vector) => _vector = vector;

    /// <summary>
    ///     X component of the 2D vector.
    /// </summary>
    public float X => _vector.X;

    /// <summary>
    ///     Y component of the 2D vector.
    /// </summary>
    public float Y => _vector.Y;

    #region Constants

    /// <summary>
    ///     Default zero vector with zero length.
    /// </summary>
    public static readonly Vector2D Zero = new(0F, 0F);

    /// <summary>
    ///     Default unit vector.
    /// </summary>
    public static readonly Vector2D One = new(1F, 1F);

    /// <summary>
    ///     Default up vector with Y component set to 1.
    /// </summary>
    public static readonly Vector2D Up = new(0F, 1F);

    /// <summary>
    ///     Default down vector with Y component set to -1.
    /// </summary>
    public static readonly Vector2D Down = new(0F, -1F);

    /// <summary>
    ///     Default left vector with X component set to 01.
    /// </summary>
    public static readonly Vector2D Left = new(-1F, 0F);

    /// <summary>
    ///     Default right vector with X component set to 1.
    /// </summary>
    public static readonly Vector2D Right = new(1F, 0F);

    #endregion

    #region Conversions

    /// <summary>
    ///     Convert Forge <c>Vector2D</c> into a standard <c>Vector2</c>.
    /// </summary>
    /// <param name="vector">Forge vector to convert.</param>
    /// <returns>Standard <c>Vector2</c> with the same components.</returns>
    public static implicit operator Vector2(Vector2D vector) => vector._vector;

    /// <summary>
    ///     Convert standard <c>Vector2</c> into a Forge <c>Vector2D</c>.
    /// </summary>
    /// <param name="vector">Standard vector to convert.</param>
    /// <returns>Forge <c>Vector2D</c> with the same components.</returns>
    public static implicit operator Vector2D(Vector2 vector) => new(vector);

    #endregion

    #region Operators

    /// <summary>
    ///     Compute the sum of two vectors.
    /// </summary>
    /// <param name="a">First vector.</param>
    /// <param name="b">Second vector.</param>
    /// <returns>Summed vector.</returns>
    public static Vector2D operator +(Vector2D a, Vector2D b) => new(a._vector + b._vector);

    /// <summary>
    ///     Compute the difference between two vectors.
    /// </summary>
    /// <param name="a">First vector.</param>
    /// <param name="b">Second vector.</param>
    /// <returns>Difference vector.</returns>
    public static Vector2D operator -(Vector2D a, Vector2D b) => new(a._vector - b._vector);

    /// <summary>
    ///     Multiply a vector with a scalar.
    /// </summary>
    /// <param name="vector">Vector to multiply.</param>
    /// <param name="scalar">Scalar to multiply the vector by.</param>
    /// <returns>Vector multiplied by the scalar.</returns>
    public static Vector2D operator *(Vector2D vector, float scalar) => new(vector._vector * scalar);

    /// <summary>
    ///     Multiply a vector with a scalar.
    /// </summary>
    /// <param name="scalar">Scalar to multiply the vector by.</param>
    /// <param name="vector">Vector to multiply.</param>
    /// <returns>Vector multiplied by the scalar.</returns>
    public static Vector2D operator *(float scalar, Vector2D vector) => new(vector._vector * scalar);

    /// <summary>
    ///     Divide a vector by a scalar.
    /// </summary>
    /// <param name="vector">Vector to divide.</param>
    /// <param name="scalar">Scalar to divide the vector by.</param>
    /// <returns>Vector divided by the scalar.</returns>
    /// <exception cref="DivideByZeroException">A vector cannot be divided by a zero scalar.</exception>
    public static Vector2D operator /(Vector2D vector, float scalar) => scalar == 0F
        ? throw new DivideByZeroException("Cannot divide a vector by zero.")
        : new(vector._vector / scalar);

    /// <summary>
    ///     Compute the dot product between two vectors.
    /// </summary>
    /// <param name="a">First vector.</param>
    /// <param name="b">Second vector.</param>
    /// <returns>Dot product of the two vectors.</returns>
    public static float operator *(Vector2D a, Vector2D b) => Vector2.Dot(a._vector, b._vector);

    /// <summary>
    ///     Compute the cross product of two vectors.
    /// </summary>
    /// <param name="a">First vector.</param>
    /// <param name="b">Second vector.</param>
    /// <returns>Cross product of the two vectors.</returns>
    public static float operator ^(Vector2D a, Vector2D b) => a._vector.X * b._vector.Y - a._vector.Y * b._vector.X;

    /// <summary>
    ///     Negate the vector.
    /// </summary>
    /// <param name="vector">Vector to negate.</param>
    /// <returns>Negation of the vector.</returns>
    public static Vector2D operator -(Vector2D vector) => new(-vector._vector);

    // ReSharper disable CompareOfFloatsByEqualityOperator
    /// <summary>
    ///     Check whether two vectors are equal. They are equal if they have equal X and Y components.
    /// </summary>
    /// <param name="a">First vector.</param>
    /// <param name="b">Second vector.</param>
    /// <returns>Whether the two vectors are equal.</returns>
    public static bool operator ==(Vector2D a, Vector2D b) => a.X == b.X && a.Y == b.Y;
    // ReSharper restore CompareOfFloatsByEqualityOperator

    /// <summary>
    ///     Check whether the two vectors are not equal. They are not equal if they have dissimilar X or Y components.
    /// </summary>
    /// <param name="a">First vector.</param>
    /// <param name="b">Second vector.</param>
    /// <returns>Whether the two vectors are not equal.</returns>
    public static bool operator !=(Vector2D a, Vector2D b) => !(a == b);

    #endregion

    #region Methods

    /// <summary>
    ///     Compute the length of the vector.
    /// </summary>
    /// <returns>Length, or magnitude of the vector.</returns>
    public float Length() => _vector.Length();

    /// <summary>
    ///     Compute the square of the length of the vector. Useful in hot-loops due to lack of a square-root operation.
    /// </summary>
    /// <returns>Square of the length, or magnitude of the vector.</returns>
    public float LengthSquared() => _vector.LengthSquared();

    /// <summary>
    ///     Compute the normal of the vector, i.e., set its length to 1 while preserving its direction.
    /// </summary>
    /// <returns>Normal of the vector.</returns>
    /// <exception cref="DivideByZeroException">A vector of zero length cannot be normalized.</exception>
    public Vector2D Normal()
    {
        float length = Length();
        return length == 0
            ? throw new DivideByZeroException("Cannot normalize a vector of zero length.")
            : new(_vector / length);
    }

    /// <summary>
    ///     Compute the orthogonal to the given vector.
    /// </summary>
    /// <returns>Orthogonal vector.</returns>
    public Vector2D Orthogonal() => new(_vector.Y, -_vector.X);

    /// <summary>
    ///     Scale a vector to a given length while preserving its direction.
    /// </summary>
    /// <param name="scale">New length of the vector.</param>
    /// <returns>Scaled vector with the given length and same direction.</returns>
    public Vector2D ScaleTo(float scale) => Normal() * scale;

    /// <summary>
    ///     Reflect the vector along a given normal to its direction.
    /// </summary>
    /// <param name="normal">Normal of the reflection.</param>
    /// <returns>Reflected vector with respect to the normal.</returns>
    /// <exception cref="ArgumentException">The normal must be normalized.</exception>
    public Vector2D ReflectTo(Vector2D normal)
    {
        if (!normal.IsNormalized())
            throw new ArgumentException("Cannot reflect a vector along a normal of invalid length.");

        return normal * -2F * (this * normal) + this;
    }

    /// <summary>
    ///     Calculate the unsigned angle between one vector and another.
    /// </summary>
    /// <param name="to">Vector to which the angle is to be measured.</param>
    /// <returns>Unsigned angle between the two vectors in radians.</returns>
    public float Angle(Vector2D to)
    {
        if (this == to)
            return 0;
        return MathF.Acos(this * to) / (Length() * to.Length());
    }

    /// <summary>
    ///     Check whether a vector is normalized or not.
    /// </summary>
    /// <param name="tolerance">Tolerance for the floating-point comparison for normality.</param>
    /// <returns>Whether the vector is normalized.</returns>
    public bool IsNormalized(float tolerance = 0.0001F) => System.Math.Abs(Length() - 1F) < tolerance;

    /// <summary>
    ///     Calculate the distance between two vectors using a distance-type.
    /// </summary>
    /// <param name="a">First vector.</param>
    /// <param name="b">Second vector.</param>
    /// <param name="type">Type of distance to be calculated.</param>
    /// <returns>Distance between the two vectors.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Distance type must be valid.</exception>
    public static float Distance(Vector2D a, Vector2D b, Distance type = Math.Distance.Euclidean)
    {
        return type switch
        {
            Math.Distance.Euclidean => (a - b).Length(),
            Math.Distance.Manhattan => MathF.Abs(a._vector.X - b._vector.X) + MathF.Abs(a._vector.Y - b._vector.Y),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    /// <summary>
    ///     Calculate the square of the distance between two vectors using a distance-type. Most useful for Euclidean distance
    ///     in hot-loops due to lack of a square-root operation.
    /// </summary>
    /// <param name="a">First vector.</param>
    /// <param name="b">Second vector.</param>
    /// <param name="type">Type of distance to be calculated.</param>
    /// <returns>Square of the distance between the two vectors.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Distance type must be valid.</exception>
    public static float DistanceSquared(Vector2D a, Vector2D b, Distance type = Math.Distance.Euclidean)
    {
        return type switch
        {
            Math.Distance.Euclidean => (a - b).LengthSquared(),
            Math.Distance.Manhattan => MathF.Pow(
                MathF.Abs(a._vector.X - b._vector.X) + MathF.Abs(a._vector.Y - b._vector.Y), 2F
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    /// <summary>
    ///     Compute a linear interpolation of a vector with respect to a given weight along both axes.
    /// </summary>
    /// <param name="start">Start vector.</param>
    /// <param name="end">End vector.</param>
    /// <param name="weight">Weight for the interpolation function.</param>
    /// <returns>Linearly interpolated vector between the start and end.</returns>
    public static Vector2D LinearInterpolation(Vector2D start, Vector2D end, float weight) =>
        new(Vector2.Lerp(start._vector, end._vector, weight));

    #endregion

    #region Equality / Overrides

    public bool Equals(Vector2D other) => _vector.X.Equals(other._vector.X) && _vector.Y.Equals(other._vector.Y);

    public override bool Equals(object? obj) => obj is Vector2D other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_vector.X, _vector.Y);

    #endregion
}

/// <summary>
///     Forge's representation of a mutable SIMD 2D vector.
/// </summary>
public struct MutableVector2D : IEquatable<MutableVector2D>
{
    /// <summary>
    ///     Internal <c>Vector2</c> storage to enable SIMD operations.
    /// </summary>
    private Vector2 _vector;

    /// <summary>
    ///     Create a new vector using its components.
    /// </summary>
    /// <param name="x">X coordinate of the vector.</param>
    /// <param name="y">Y coordinate of the vector.</param>
    public MutableVector2D(float x, float y) => _vector = new(x, y);

    /// <summary>
    ///     Create a new vector using <c>Vector2</c> directly.
    /// </summary>
    /// <param name="vector"><c>Vector2</c> to convert into a Forge <c>MutableVector2D</c>.</param>
    public MutableVector2D(Vector2 vector) => _vector = vector;

    /// <summary>
    ///     X component of the 2D vector.
    /// </summary>
    public float X
    {
        get => _vector.X;
        set => _vector.X = value;
    }

    /// <summary>
    ///     Y component of the 2D vector.
    /// </summary>
    public float Y
    {
        get => _vector.Y;
        set => _vector.Y = value;
    }

    #region Conversions

    /// <summary>
    ///     Convert Forge <c>MutableVector2D</c> into a standard <c>Vector2</c>.
    /// </summary>
    /// <param name="vector">Forge vector to convert.</param>
    /// <returns>Standard <c>Vector2</c> with the same components.</returns>
    public static implicit operator Vector2(MutableVector2D vector) => vector._vector;

    /// <summary>
    ///     Convert standard <c>Vector2</c> into a Forge <c>MutableVector2D</c>.
    /// </summary>
    /// <param name="vector">Standard vector to convert.</param>
    /// <returns>Forge <c>MutableVector2D</c> with the same components.</returns>
    public static implicit operator MutableVector2D(Vector2 vector) => new(vector);

    /// <summary>
    ///     Convert a mutable <c>MutableVector2D</c> into an immutable <c>Vector2D</c>.
    /// </summary>
    /// <param name="vector">Mutable vector to convert.</param>
    /// <returns>Immutable vector with the same components.</returns>
    public static implicit operator Vector2D(MutableVector2D vector) => new(vector._vector);

    /// <summary>
    ///     Convert an immutable <c>Vector2D</c> into a mutable <c>MutableVector2D</c>.
    /// </summary>
    /// <param name="vector">Immutable vector to convert.</param>
    /// <returns>Mutable vector with the same components.</returns>
    public static implicit operator MutableVector2D(Vector2D vector) => new(vector.X, vector.Y);

    #endregion

    #region Operators

    /// <summary>
    ///     Compute the sum of two vectors.
    /// </summary>
    /// <param name="a">First vector.</param>
    /// <param name="b">Second vector.</param>
    /// <returns>Summed vector.</returns>
    public static MutableVector2D operator +(MutableVector2D a, MutableVector2D b) => new(a._vector + b._vector);

    /// <summary>
    ///     Compute the difference between two vectors.
    /// </summary>
    /// <param name="a">First vector.</param>
    /// <param name="b">Second vector.</param>
    /// <returns>Difference vector.</returns>
    public static MutableVector2D operator -(MutableVector2D a, MutableVector2D b) => new(a._vector - b._vector);

    /// <summary>
    ///     Multiply a vector with a scalar.
    /// </summary>
    /// <param name="vector">Vector to multiply.</param>
    /// <param name="scalar">Scalar to multiply the vector by.</param>
    /// <returns>Vector multiplied by the scalar.</returns>
    public static MutableVector2D operator *(MutableVector2D vector, float scalar) => new(vector._vector * scalar);

    /// <summary>
    ///     Multiply a vector with a scalar.
    /// </summary>
    /// <param name="scalar">Scalar to multiply the vector by.</param>
    /// <param name="vector">Vector to multiply.</param>
    /// <returns>Vector multiplied by the scalar.</returns>
    public static MutableVector2D operator *(float scalar, MutableVector2D vector) => new(vector._vector * scalar);

    /// <summary>
    ///     Divide a vector by a scalar.
    /// </summary>
    /// <param name="vector">Vector to divide.</param>
    /// <param name="scalar">Scalar to divide the vector by.</param>
    /// <returns>Vector divided by the scalar.</returns>
    /// <exception cref="DivideByZeroException">A vector cannot be divided by a zero scalar.</exception>
    public static MutableVector2D operator /(MutableVector2D vector, float scalar) => scalar == 0F
        ? throw new DivideByZeroException("Cannot divide a vector by zero.")
        : new(vector._vector / scalar);

    /// <summary>
    ///     Compute the dot product between two vectors.
    /// </summary>
    /// <param name="a">First vector.</param>
    /// <param name="b">Second vector.</param>
    /// <returns>Dot product of the two vectors.</returns>
    public static float operator *(MutableVector2D a, MutableVector2D b) => Vector2.Dot(a._vector, b._vector);

    /// <summary>
    ///     Compute the cross product of two vectors.
    /// </summary>
    /// <param name="a">First vector.</param>
    /// <param name="b">Second vector.</param>
    /// <returns>Cross product of the two vectors.</returns>
    public static float operator ^(MutableVector2D a, MutableVector2D b) =>
        a._vector.X * b._vector.Y - a._vector.Y * b._vector.X;

    /// <summary>
    ///     Negate the vector.
    /// </summary>
    /// <param name="vector">Vector to negate.</param>
    /// <returns>Negation of the vector.</returns>
    public static MutableVector2D operator -(MutableVector2D vector) => new(-vector._vector);

    // ReSharper disable CompareOfFloatsByEqualityOperator
    /// <summary>
    ///     Check whether two vectors are equal. They are equal if they have equal X and Y components.
    /// </summary>
    /// <param name="a">First vector.</param>
    /// <param name="b">Second vector.</param>
    /// <returns>Whether the two vectors are equal.</returns>
    public static bool operator ==(MutableVector2D a, MutableVector2D b) => a.X == b.X && a.Y == b.Y;
    // ReSharper restore CompareOfFloatsByEqualityOperator

    /// <summary>
    ///     Check whether the two vectors are not equal. They are not equal if they have dissimilar X or Y components.
    /// </summary>
    /// <param name="a">First vector.</param>
    /// <param name="b">Second vector.</param>
    /// <returns>Whether the two vectors are not equal.</returns>
    public static bool operator !=(MutableVector2D a, MutableVector2D b) => !(a == b);

    #endregion

    #region Methods

    /// <summary>
    ///     Compute the length of the vector.
    /// </summary>
    /// <returns>Length, or magnitude of the vector.</returns>
    public float Length() => _vector.Length();

    /// <summary>
    ///     Compute the square of the length of the vector. Useful in hot-loops due to lack of a square-root operation.
    /// </summary>
    /// <returns>Square of the length, or magnitude of the vector.</returns>
    public float LengthSquared() => _vector.LengthSquared();

    /// <summary>
    ///     Compute the normal of the vector, i.e., set its length to 1 while preserving its direction.
    /// </summary>
    /// <returns>Normal of the vector.</returns>
    /// <exception cref="DivideByZeroException">A vector of zero length cannot be normalized.</exception>
    public Vector2D Normal()
    {
        float length = Length();
        return length == 0
            ? throw new DivideByZeroException("Cannot normalize a vector of zero length.")
            : new(_vector / length);
    }

    /// <summary>
    ///     Normalize the vector itself, i.e., set its length to 1 while preserving its direction.
    /// </summary>
    /// <exception cref="DivideByZeroException">A vector of zero length cannot be normalized.</exception>
    public void Normalize()
    {
        float length = Length();
        if (length == 0)
            throw new DivideByZeroException("Cannot normalize a vector of zero length.");

        _vector /= length;
    }

    /// <summary>
    ///     Compute the orthogonal to the given vector.
    /// </summary>
    /// <returns>Orthogonal vector.</returns>
    public Vector2D Orthogonal() => new(_vector.Y, -_vector.X);

    /// <summary>
    ///     Orthogonalize the vector itself.
    /// </summary>
    public void Orthogonalize()
    {
        _vector = new(_vector.Y, -_vector.X);
    }

    /// <summary>
    ///     Scale the vector itself to a given length while preserving its direction.
    /// </summary>
    /// <param name="scale">New length of the vector itself.</param>
    public void Scale(float scale)
    {
        Normalize();
        _vector *= scale;
    }

    /// <summary>
    ///     Scale a vector to a given length while preserving its direction.
    /// </summary>
    /// <param name="scale">New length of the vector.</param>
    /// <returns>Scaled vector with the given length and same direction.</returns>
    public Vector2D ScaleTo(float scale) => Normal() * scale;

    /// <summary>
    ///     Reflect the vector itself along a given normal to its direction.
    /// </summary>
    /// <param name="normal">Normal of the reflection.</param>
    /// <exception cref="DivideByZeroException">The normal must be normalized.</exception>
    public void Reflect(MutableVector2D normal)
    {
        if (!normal.IsNormalized())
            throw new DivideByZeroException("Cannot reflect a vector along a normal of invalid length.");

        _vector = normal._vector * -2F * (_vector * normal._vector) + _vector;
    }

    /// <summary>
    ///     Reflect the vector along a given normal to its direction.
    /// </summary>
    /// <param name="normal">Normal of the reflection.</param>
    /// <returns>Reflected vector with respect to the normal.</returns>
    /// <exception cref="ArgumentException">The normal must be normalized.</exception>
    public Vector2D ReflectTo(MutableVector2D normal) => !normal.IsNormalized()
        ? throw new DivideByZeroException("Cannot reflect a vector along a normal of invalid length.")
        : new(normal._vector * -2F * (_vector * normal._vector) + _vector);

    /// <summary>
    ///     Calculate the unsigned angle between one vector and another.
    /// </summary>
    /// <param name="to">Vector to which the angle is to be measured.</param>
    /// <returns>Unsigned angle between the two vectors in radians.</returns>
    public float Angle(MutableVector2D to)
    {
        if (this == to)
            return 0;

        return MathF.Acos(this * to) / (Length() * to.Length());
    }

    /// <summary>
    ///     Check whether a vector is normalized or not.
    /// </summary>
    /// <param name="tolerance">Tolerance for the floating-point comparison for normality.</param>
    /// <returns>Whether the vector is normalized.</returns>
    public bool IsNormalized(float tolerance = 0.0001F) => System.Math.Abs(Length() - 1F) < tolerance;

    /// <summary>
    ///     Calculate the distance between two vectors using a distance-type.
    /// </summary>
    /// <param name="a">First vector.</param>
    /// <param name="b">Second vector.</param>
    /// <param name="type">Type of distance to be calculated.</param>
    /// <returns>Distance between the two vectors.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Distance type must be valid.</exception>
    public static float Distance(MutableVector2D a, MutableVector2D b, Distance type = Math.Distance.Euclidean)
    {
        return type switch
        {
            Math.Distance.Euclidean => (a - b).Length(),
            Math.Distance.Manhattan => MathF.Abs(a._vector.X - b._vector.X) + MathF.Abs(a._vector.Y - b._vector.Y),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    /// <summary>
    ///     Calculate the square of the distance between two vectors using a distance-type. Most useful for Euclidean distance
    ///     in hot-loops due to lack of a square-root operation.
    /// </summary>
    /// <param name="a">First vector.</param>
    /// <param name="b">Second vector.</param>
    /// <param name="type">Type of distance to be calculated.</param>
    /// <returns>Square of the distance between the two vectors.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Distance type must be valid.</exception>
    public static float DistanceSquared(MutableVector2D a, MutableVector2D b, Distance type = Math.Distance.Euclidean)
    {
        return type switch
        {
            Math.Distance.Euclidean => (a - b).LengthSquared(),
            Math.Distance.Manhattan => MathF.Pow(
                MathF.Abs(a._vector.X - b._vector.X) + MathF.Abs(a._vector.Y - b._vector.Y), 2F
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    /// <summary>
    ///     Compute a linear interpolation of a vector with respect to a given weight along both axes.
    /// </summary>
    /// <param name="start">Start vector.</param>
    /// <param name="end">End vector.</param>
    /// <param name="weight">Weight for the interpolation function.</param>
    /// <returns>Linearly interpolated vector between the start and end.</returns>
    public static Vector2D LinearInterpolation(MutableVector2D start, MutableVector2D end, float weight) =>
        new(Vector2.Lerp(start._vector, end._vector, weight));

    #endregion

    #region Equality / Overrides

    public bool Equals(MutableVector2D other) => _vector.X.Equals(other._vector.X) && _vector.Y.Equals(other._vector.Y);

    public override bool Equals(object? obj) => obj is MutableVector2D other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_vector.X, _vector.Y);

    #endregion
}