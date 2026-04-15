namespace WS.Matrices;

/// <summary>
/// Extension methods for square matrices (<see cref="Matrix{TSize,TSize}"/>).
/// </summary>
public static class SquareMatrixExtensions
{
    /// <summary>
    /// Computes the determinant of the matrix using Gaussian elimination with partial pivoting.
    /// </summary>
    /// <typeparam name="TSize">A <see cref="Dimension"/> subtype encoding both the row and column count.</typeparam>
    /// <param name="matrix">The square matrix whose determinant is to be computed.</param>
    /// <returns>
    /// The determinant as a <see cref="double"/>. Returns <c>0.0</c> if the matrix is singular.
    /// </returns>
    public static double GetDeterminant<TSize>(this Matrix<TSize, TSize> matrix)
        where TSize : Dimension
    {
        int n = matrix.Rows;

        // Copy values into a local working array via the public indexer
        var a = new double[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                a[i, j] = matrix[i, j].Match(v => v, _ => 0.0);
            }
        }

        // Gaussian elimination with partial pivoting
        int sign = 1;
        for (int col = 0; col < n; col++)
        {
            // Find the row with the largest absolute value in this column (pivot)
            int pivotRow = col;
            double maxAbs = Math.Abs(a[col, col]);
            for (int row = col + 1; row < n; row++)
            {
                double abs = Math.Abs(a[row, col]);
                if (abs > maxAbs)
                {
                    maxAbs = abs;
                    pivotRow = row;
                }
            }

            // Swap pivot row into position, tracking sign change
            if (pivotRow != col)
            {
                for (int k = 0; k < n; k++)
                {
                    (a[col, k], a[pivotRow, k]) = (a[pivotRow, k], a[col, k]);
                }
                sign = -sign;
            }

            // Singular matrix — determinant is zero
            if (a[col, col] == 0.0)
            {
                return 0.0;
            }

            // Eliminate entries below the pivot
            for (int row = col + 1; row < n; row++)
            {
                double factor = a[row, col] / a[col, col];
                for (int k = col; k < n; k++)
                {
                    a[row, k] -= factor * a[col, k];
                }
            }
        }

        // Determinant = sign * product of diagonal elements of the upper-triangular matrix
        double det = sign;
        for (int i = 0; i < n; i++)
        {
            det *= a[i, i];
        }

        return det;
    }

    /// <summary>
    /// Computes the inverse of the matrix using Gauss-Jordan elimination with partial pivoting.
    /// </summary>
    /// <typeparam name="TSize">A <see cref="Dimension"/> subtype encoding both the row and column count.</typeparam>
    /// <param name="matrix">The square matrix to invert.</param>
    /// <returns>
    /// A <see cref="Result{T,TError}"/> containing the inverse matrix on success,
    /// or an error message if the matrix is singular.
    /// </returns>
    public static Result<Matrix<TSize, TSize>, string> GetInverse<TSize>(this Matrix<TSize, TSize> matrix)
        where TSize : Dimension
    {
        double det = matrix.GetDeterminant();
        if (det == 0.0)
        {
            return "Matrix is singular and cannot be inverted.";
        }

        int n = matrix.Rows;

        // Build an augmented matrix [A | I] as a flat array
        var a = new double[n, n * 2];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                a[i, j] = matrix[i, j].Match(v => v, _ => 0.0);
            }
            a[i, n + i] = 1.0;
        }

        // Forward elimination with partial pivoting
        for (int col = 0; col < n; col++)
        {
            // Find pivot row
            int pivotRow = col;
            double maxAbs = Math.Abs(a[col, col]);
            for (int row = col + 1; row < n; row++)
            {
                double abs = Math.Abs(a[row, col]);
                if (abs > maxAbs)
                {
                    maxAbs = abs;
                    pivotRow = row;
                }
            }

            // Swap pivot row into position
            if (pivotRow != col)
            {
                for (int k = 0; k < n * 2; k++)
                {
                    (a[col, k], a[pivotRow, k]) = (a[pivotRow, k], a[col, k]);
                }
            }

            // Scale pivot row so the diagonal becomes 1
            double pivotValue = a[col, col];
            for (int k = 0; k < n * 2; k++)
            {
                a[col, k] /= pivotValue;
            }

            // Eliminate all other rows in this column
            for (int row = 0; row < n; row++)
            {
                if (row == col)
                {
                    continue;
                }

                double factor = a[row, col];
                for (int k = 0; k < n * 2; k++)
                {
                    a[row, k] -= factor * a[col, k];
                }
            }
        }

        // Extract the right-hand side as the inverse
        var result = new double[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                result[i, j] = a[i, n + j];
            }
        }

        return Matrix.Create<TSize, TSize>(result);
    }
}