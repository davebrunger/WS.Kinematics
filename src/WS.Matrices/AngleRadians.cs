namespace WS.Matrices;

/// <summary>
/// Represents an angle in radians, normalised to the range [0, 2π).
/// </summary>
public record AngleRadians
{
    /// <summary>Gets the normalised angle value in radians.</summary>
    public double Value { get; }

    /// <summary>
    /// Initialises a new <see cref="AngleRadians"/> with the given value, normalised to [0, 2π).
    /// </summary>
    /// <param name="value">The angle in radians.</param>
    public AngleRadians(double value)
    {
        var remainder = Math.IEEERemainder(value, 2 * Math.PI);
        Value = remainder < 0 ? remainder + 2 * Math.PI : remainder;
    }

    /// <summary>Implicitly converts an <see cref="AngleRadians"/> to a <see cref="double"/>.</summary>
    public static implicit operator double(AngleRadians angle) => angle.Value;

    /// <summary>Implicitly converts a <see cref="double"/> to an <see cref="AngleRadians"/>.</summary>
    public static implicit operator AngleRadians(double value) => new(value);

    /// <summary>Returns the sum of two angles.</summary>
    public static AngleRadians operator +(AngleRadians a, AngleRadians b) => new(a.Value + b.Value);

    /// <summary>Returns the difference of two angles.</summary>
    public static AngleRadians operator -(AngleRadians a, AngleRadians b) => new(a.Value - b.Value);
}