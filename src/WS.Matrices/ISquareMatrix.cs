namespace WS.Matrices;

/// <summary>
/// Defines the contract for an immutable square matrix, extending <see cref="IMatrix{TRows,TColumns,TMatrix}"/>
/// with determinant, inverse, transpose, and square matrix multiplication.
/// </summary>
/// <typeparam name="TDimension">A <see cref="Dimension"/> subtype encoding both the row and column count.</typeparam>
/// <typeparam name="TMatrix">The concrete implementing type, used for self-referential return types.</typeparam>
public interface ISquareMatrix<TDimension, TMatrix> : IMatrix<TDimension, TDimension, TMatrix>
    where TDimension : Dimension
    where TMatrix : ISquareMatrix<TDimension, TMatrix>
{
    /// <summary>Gets the determinant of the matrix.</summary>
    double Determinant { get; }

    /// <summary>
    /// Gets the inverse of the matrix, or an error if the matrix is singular.
    /// </summary>
    Result<TMatrix, string> Inverse { get; }

    /// <summary>
    /// Returns the matrix product of this matrix and <paramref name="other"/>.
    /// </summary>
    /// <param name="other">The right-hand square matrix to multiply by.</param>
    /// <returns>A new matrix that is the product of this matrix and <paramref name="other"/>.</returns>
    TMatrix Multiply(TMatrix other);

    /// <summary>
    /// Returns the transpose of this matrix, swapping rows and columns.
    /// </summary>
    /// <returns>A new matrix that is the transpose of this matrix.</returns>
    TMatrix Transpose();
}