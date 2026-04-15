namespace WS.Kinematics.Models;

public record AngleRadians2D
{
    public double Value { get; }

    public AngleRadians2D(double value)
    {
        var remainder = Math.IEEERemainder(value, 2 * Math.PI);
        Value = remainder < 0 ? remainder + 2 * Math.PI : remainder;
    }

    public static implicit operator double(AngleRadians2D angle) => angle.Value;
    public static implicit operator AngleRadians2D(double value) => new(value);

    public static AngleRadians2D operator +(AngleRadians2D a, AngleRadians2D b) => new(a.Value + b.Value);
    public static AngleRadians2D operator -(AngleRadians2D a, AngleRadians2D b) => new(a.Value - b.Value);
}