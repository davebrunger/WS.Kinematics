using System;

namespace WS.Kinematics
{
    /// <summary>
    /// Represents a 3-dimensional vector used in kinematics calculations.
    /// </summary>
    public struct Vector3D
    {
        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3D Zero => new Vector3D(0, 0, 0);

        public double Magnitude => Math.Sqrt(X * X + Y * Y + Z * Z);

        public Vector3D Normalize()
        {
            double mag = Magnitude;
            if (mag == 0)
            {
                throw new InvalidOperationException("Cannot normalize a zero vector.");
            }
            return new Vector3D(X / mag, Y / mag, Z / mag);
        }

        public static Vector3D operator +(Vector3D a, Vector3D b) =>
            new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static Vector3D operator -(Vector3D a, Vector3D b) =>
            new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static Vector3D operator *(Vector3D v, double scalar) =>
            new Vector3D(v.X * scalar, v.Y * scalar, v.Z * scalar);

        public static Vector3D operator *(double scalar, Vector3D v) => v * scalar;

        public static double Dot(Vector3D a, Vector3D b) =>
            a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        public static Vector3D Cross(Vector3D a, Vector3D b) =>
            new Vector3D(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X);

        public override string ToString() => $"({X}, {Y}, {Z})";
    }
}
