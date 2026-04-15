namespace WS.Matrices;

/// <summary>
/// Represents an immutable fixed-length vector with compile-time-encoded dimension.
/// </summary>
/// <typeparam name="TDimension">A <see cref="Dimension"/> subtype encoding the number of components.</typeparam>
public class Vector<TDimension> : IEquatable<Vector<TDimension>> where TDimension : Dimension
{
    private readonly double[] components;

    /// <summary>Gets the number of components in the vector.</summary>
    public int Length => Dimension.ToInt<TDimension>();

    internal Vector(params double[] components)
    {
        if (components.Length != Length)
        {
            throw new ArgumentException($"Expected {Length} components for dimension {typeof(TDimension).Name}, but got {components.Length}.");
        }
        this.components = components;
    }

    /// <summary>
    /// Gets the component at the specified index.
    /// </summary>
    /// <param name="index">Zero-based component index.</param>
    /// <returns>A <see cref="Result{T,TError}"/> containing the value, or an error message if the index is out of range.</returns>
    public Result<double, string> this[int index]
    {
        get
        {
            if (index < 0 || index >= Length)
            {
                return $"Index {index} is out of range for dimension {typeof(TDimension).Name}.";
            }
            return components[index];
        }
    }

    /// <summary>Returns the element-wise sum of this vector and <paramref name="other"/>.</summary>
    /// <param name="other">The vector to add.</param>
    /// <returns>A new vector containing the element-wise sum.</returns>
    public Vector<TDimension> Add(Vector<TDimension> other)
    {
        var resultComponents = new double[Length];
        for (int i = 0; i < Length; i++)
        {
            resultComponents[i] = components[i] + other.components[i];
        }
        return new Vector<TDimension>(resultComponents);
    }

    /// <summary>
    /// Determines whether this vector is equal to <paramref name="other"/> by comparing all component values.
    /// </summary>
    /// <param name="other">The vector to compare with.</param>
    /// <returns><see langword="true"/> if all components are equal; otherwise <see langword="false"/>.</returns>
    public bool Equals(Vector<TDimension> other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        for (int i = 0; i < Length; i++)
        {
            if (components[i] != other.components[i])
            {
                return false;
            }
        }
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Vector<TDimension> other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hash = 17;
        foreach (var component in components)
        {
            hash = hash * 31 + component.GetHashCode();
        }
        return hash;
    }

    /// <summary>
    /// Returns a new vector with the component at <paramref name="index"/> replaced by <paramref name="value"/>.
    /// The original vector is not modified.
    /// </summary>
    /// <param name="index">Zero-based component index.</param>
    /// <param name="value">The value to set at the specified index.</param>
    /// <returns>A <see cref="Result{T,TError}"/> containing the updated vector, or an error message if the index is out of range.</returns>
    public Result<Vector<TDimension>, string> SetComponent(int index, double value)
    {
        if (index < 0 || index >= Length)
        {
            return $"Index {index} is out of range for dimension {typeof(TDimension).Name}.";
        }
        var newComponents = (double[])components.Clone();
        newComponents[index] = value;
        return new Vector<TDimension>(newComponents);
    }

    /// <summary>Returns the element-wise difference of this vector minus <paramref name="other"/>.</summary>
    /// <param name="other">The vector to subtract.</param>
    /// <returns>A new vector containing the element-wise difference.</returns>
    public Vector<TDimension> Subtract(Vector<TDimension> other)
    {
        var resultComponents = new double[Length];
        for (int i = 0; i < Length; i++)
        {
            resultComponents[i] = components[i] - other.components[i];
        }
        return new Vector<TDimension>(resultComponents);
    }

    /// <inheritdoc/>
    public override string ToString() => $"[{string.Join(", ", components)}]";

    /// <summary>Returns <see langword="true"/> if both vectors have identical component values.</summary>
    public static bool operator ==(Vector<TDimension> left, Vector<TDimension> right) => left.Equals(right);

    /// <summary>Returns <see langword="true"/> if the vectors differ in any component value.</summary>
    public static bool operator !=(Vector<TDimension> left, Vector<TDimension> right) => !left.Equals(right);

    /// <summary>Returns the element-wise sum of <paramref name="left"/> and <paramref name="right"/>.</summary>
    public static Vector<TDimension> operator +(Vector<TDimension> left, Vector<TDimension> right) => left.Add(right);

    /// <summary>Returns the element-wise difference of <paramref name="left"/> minus <paramref name="right"/>.</summary>
    public static Vector<TDimension> operator -(Vector<TDimension> left, Vector<TDimension> right) => left.Subtract(right);
}

/// <summary>
/// Factory methods for constructing <see cref="Vector{TDimension}"/> instances.
/// </summary>
public static class Vector
{
    /// <summary>
    /// Creates a new vector from the provided component values.
    /// </summary>
    /// <typeparam name="TDimension">A <see cref="Dimension"/> subtype encoding the number of components.</typeparam>
    /// <param name="components">The component values. The count must match <typeparamref name="TDimension"/>.</param>
    /// <returns>A <see cref="Result{T,TError}"/> containing the new vector, or an error message if the component count does not match.</returns>
    public static Result<Vector<TDimension>, string> Create<TDimension>(params double[] components)
        where TDimension : Dimension
    {
        try
        {
            return new Vector<TDimension>(components);
        }
        catch (ArgumentException ex)
        {
            return ex.Message;
        }
    }
}