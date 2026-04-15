namespace WS.Matrices;

/// <summary>
/// Defines the contract for an immutable matrix with compile-time-encoded row and column dimensions.
/// </summary>
/// <typeparam name="TRows">A <see cref="Dimension"/> subtype encoding the number of rows.</typeparam>
/// <typeparam name="TColumns">A <see cref="Dimension"/> subtype encoding the number of columns.</typeparam>
/// <typeparam name="TMatrix">The concrete implementing type, used for self-referential return types.</typeparam>
public interface IMatrix<TRows, TColumns, TMatrix> : IEquatable<TMatrix>
    where TRows : Dimension
    where TColumns : Dimension
    where TMatrix : IMatrix<TRows, TColumns, TMatrix>
{
    /// <summary>Gets the number of columns in the matrix.</summary>
    int Columns { get; }

    /// <summary>Gets the number of rows in the matrix.</summary>
    int Rows { get; }

    /// <summary>
    /// Gets the value at the specified row and column.
    /// </summary>
    /// <param name="row">Zero-based row index.</param>
    /// <param name="column">Zero-based column index.</param>
    /// <returns>A <see cref="Result{T,TError}"/> containing the value, or an error message if the indices are out of range.</returns>
    Result<double, string> this[int row, int column] { get; }

    /// <summary>Returns the element-wise sum of this matrix and <paramref name="other"/>.</summary>
    /// <param name="other">The matrix to add.</param>
    /// <returns>A new matrix containing the element-wise sum.</returns>
    TMatrix Add(TMatrix other);

    /// <summary>Returns a new matrix with every element multiplied by <paramref name="scalar"/>.</summary>
    /// <param name="scalar">The scalar value to multiply by.</param>
    /// <returns>A new matrix scaled by <paramref name="scalar"/>.</returns>
    TMatrix Multiply(double scalar);

    /// <summary>
    /// Returns a new matrix with the specified cell replaced by <paramref name="value"/>.
    /// The original matrix is not modified.
    /// </summary>
    /// <param name="row">Zero-based row index.</param>
    /// <param name="column">Zero-based column index.</param>
    /// <param name="value">The value to place at the specified cell.</param>
    /// <returns>A <see cref="Result{T,TError}"/> containing the updated matrix, or an error message if the indices are out of range.</returns>
    Result<TMatrix, string> SetValue(int row, int column, double value);

    /// <summary>Returns the element-wise difference of this matrix minus <paramref name="other"/>.</summary>
    /// <param name="other">The matrix to subtract.</param>
    /// <returns>A new matrix containing the element-wise difference.</returns>
    TMatrix Subtract(TMatrix other);
}