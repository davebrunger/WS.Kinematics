namespace WS.Matrices;

/// <summary>
/// Factory methods for constructing 2D homogeneous transformation matrices.
/// All matrices are 3×3, using homogeneous coordinates to represent 2D affine transformations
/// including projections, reflections, rotations, shears, squeezes, stretches, and translations.
/// </summary>
public static class HomogeneousMatrices
{
    /// <summary>
    /// Creates a 3×3 homogeneous orthogonal projection matrix that projects 2D points onto a line
    /// through the origin in the direction of <paramref name="direction"/>.
    /// The vector does not need to be normalised.
    /// </summary>
    /// <param name="direction">A 2D vector defining the direction of the projection line.</param>
    /// <returns>A <see cref="SquareMatrix{Three}"/> representing the orthogonal projection.</returns>
    public static SquareMatrix<Three> Project(Vector<Two> direction)
    {
        var x = direction[0].Match(v => v, _ => 0);
        var y = direction[1].Match(v => v, _ => 0);
        var lenSquared = x * x + y * y;

        return new SquareMatrix<Three>(new double[,]
        {
            { x * x / lenSquared, x * y / lenSquared, 0 },
            { x * y / lenSquared, y * y / lenSquared, 0 },
            { 0,                  0,                  1 }
        });
    }

    /// <summary>
    /// Creates a 3×3 homogeneous reflection matrix across a line through the origin at the given angle.
    /// </summary>
    /// <param name="axisAngle">The angle of the reflection axis, measured counter-clockwise from the positive x-axis.</param>
    /// <returns>A <see cref="SquareMatrix{Three}"/> representing the reflection.</returns>
    public static SquareMatrix<Three> Reflect(AngleRadians axisAngle)
    {
        var cos2 = Math.Cos(2 * axisAngle);
        var sin2 = Math.Sin(2 * axisAngle);

        return new SquareMatrix<Three>(new double[,]
        {
            { cos2,  sin2, 0 },
            { sin2, -cos2, 0 },
            { 0,     0,    1 }
        });
    }

    /// <summary>
    /// Creates a 3×3 homogeneous reflection matrix across a line through the origin in the direction of <paramref name="direction"/>.
    /// The vector does not need to be normalised.
    /// </summary>
    /// <param name="direction">A 2D vector defining the direction of the reflection axis.</param>
    /// <returns>A <see cref="SquareMatrix{Three}"/> representing the reflection.</returns>
    public static SquareMatrix<Three> Reflect(Vector<Two> direction)
    {
        var x = direction[0].Match(v => v, _ => 0);
        var y = direction[1].Match(v => v, _ => 0);
        var lenSquared = x * x + y * y;
        var cos2 = (x * x - y * y) / lenSquared;
        var sin2 = 2 * x * y / lenSquared;

        return new SquareMatrix<Three>(new double[,]
        {
            { cos2,  sin2, 0 },
            { sin2, -cos2, 0 },
            { 0,     0,    1 }
        });
    }

    /// <summary>
    /// Creates a 3×3 homogeneous rotation matrix for the given angle.
    /// </summary>
    /// <param name="angleRadians">The angle of rotation.</param>
    /// <returns>A <see cref="SquareMatrix{Three}"/> representing the rotation.</returns>
    public static SquareMatrix<Three> Rotate(AngleRadians angleRadians)
    {
        var cos = Math.Cos(angleRadians);
        var sin = Math.Sin(angleRadians);

        return new SquareMatrix<Three>(new double[,]
        {
            { cos, -sin, 0 },
            { sin,  cos, 0 },
            { 0,    0,   1 }
        });
    }

    /// <summary>
    /// Creates a 3×3 homogeneous shear matrix.
    /// The x-component of <paramref name="shear"/> shifts the x-coordinate proportionally to y;
    /// the y-component shifts the y-coordinate proportionally to x.
    /// </summary>
    /// <param name="shear">A 2D vector whose x-component is the x-shear factor and y-component is the y-shear factor.</param>
    /// <returns>A <see cref="SquareMatrix{Three}"/> representing the shear.</returns>
    public static SquareMatrix<Three> Shear(Vector<Two> shear)
    {
        var shearX = shear[0].Match(v => v, _ => 0);
        var shearY = shear[1].Match(v => v, _ => 0);

        return new SquareMatrix<Three>(new double[,]
        {
            { 1,      shearX, 0 },
            { shearY, 1,      0 },
            { 0,      0,      1 }
        });
    }

    /// <summary>
    /// Creates a 3×3 homogeneous squeeze matrix that scales the x-axis by <paramref name="factor"/> and the y-axis by its reciprocal,
    /// preserving area.
    /// </summary>
    /// <param name="factor">The scale factor applied to the x-axis. Must be non-zero. The y-axis is scaled by <c>1 / factor</c>.</param>
    /// <returns>A <see cref="SquareMatrix{Three}"/> representing the squeeze.</returns>
    public static SquareMatrix<Three> Squeeze(double factor)
    {
        return new SquareMatrix<Three>(new double[,]
        {
            { factor, 0,          0 },
            { 0,      1 / factor, 0 },
            { 0,      0,          1 }
        });
    }

    /// <summary>
    /// Creates a 3×3 homogeneous stretch (non-uniform scale) matrix.
    /// </summary>
    /// <param name="scale">A 2D vector whose components are the scale factors for the x- and y-axes respectively.</param>
    /// <returns>A <see cref="SquareMatrix{Three}"/> representing the stretch.</returns>
    public static SquareMatrix<Three> Stretch(Vector<Two> scale)
    {
        var sx = scale[0].Match(v => v, _ => 0);
        var sy = scale[1].Match(v => v, _ => 0);

        return new SquareMatrix<Three>(new double[,]
        {
            { sx, 0,  0 },
            { 0,  sy, 0 },
            { 0,  0,  1 }
        });
    }

    /// <summary>
    /// Creates a 3×3 homogeneous translation matrix for the given 2D translation vector.
    /// </summary>
    /// <param name="translation">A 2D vector representing the x and y translation.</param>
    /// <returns>A <see cref="SquareMatrix{Three}"/> representing the translation.</returns>
    public static SquareMatrix<Three> Translate(Vector<Two> translation)
    {
        return new SquareMatrix<Three>(new double[,]
        {
            { 1, 0, translation[0].Match(v => v, _ => 0) },
            { 0, 1, translation[1].Match(v => v, _ => 0) },
            { 0, 0, 1 }
        });
    }

    /// <summary>
    /// Applies a 3×3 homogeneous transformation matrix to a 2D point, returning the transformed point.
    /// The input vector is treated as a homogeneous point with implicit w=1.
    /// </summary>
    /// <param name="matrix">The 3×3 homogeneous transformation matrix to apply.</param>
    /// <param name="vector">The 2D point to transform.</param>
    /// <returns>
    /// A <see cref="Result{T,TError}"/> containing the transformed 2D point, or an error message
    /// if the transformation produces an invalid homogeneous coordinate (w=0).
    /// </returns>
    public static Result<Vector<Two>, string> Apply(this SquareMatrix<Three> matrix, Vector<Two> vector)
    {
        var vector3 = new Vector<Three>([vector[0].Match(v => v, _ => 0), vector[1].Match(v => v, _ => 0), 1]);
        var result3 = matrix.Multiply(vector3);
        var w = result3[2].Match(v => v, _ => 1);

        if (w == 0)
        {
            return "Transformation resulted in invalid homogeneous coordinate w=0.";
        }

        return new Vector<Two>([result3[0].Match(v => v, _ => 0) / w, result3[1].Match(v => v, _ => 0) / w]);
    }
}