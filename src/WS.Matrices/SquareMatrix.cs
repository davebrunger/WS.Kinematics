namespace WS.Matrices;

/// <summary>
/// An immutable square matrix that lazily computes its determinant and inverse.
/// </summary>
/// <typeparam name="TDimension">A <see cref="Dimension"/> subtype encoding both the row and column count.</typeparam>
public class SquareMatrix<TDimension> : ISquareMatrix<TDimension, SquareMatrix<TDimension>>
    where TDimension : Dimension
{
    private readonly Matrix<TDimension, TDimension> matrix;
    private readonly Lazy<double> determinant;
    private readonly Lazy<Result<SquareMatrix<TDimension>, string>> inverse;

    /// <summary>Gets the number of columns in the matrix.</summary>
    public int Columns => matrix.Columns;

    /// <summary>Gets the determinant of the matrix, computed lazily on first access.</summary>
    public double Determinant => determinant.Value;

    /// <summary>Gets the inverse of the matrix, or an error if the matrix is singular. Computed lazily on first access.</summary>
    public Result<SquareMatrix<TDimension>, string> Inverse => inverse.Value;

    /// <summary>Gets the number of rows in the matrix.</summary>
    public int Rows => matrix.Rows;

    /// <summary>
    /// Gets the value at the specified row and column.
    /// </summary>
    /// <param name="row">Zero-based row index.</param>
    /// <param name="column">Zero-based column index.</param>
    /// <returns>A <see cref="Result{T,TError}"/> containing the value, or an error message if the indices are out of range.</returns>
    public Result<double, string> this[int row, int column] => matrix[row, column];

    internal SquareMatrix(Matrix<TDimension, TDimension> matrix)
    {
        this.matrix = matrix;
        determinant = new Lazy<double>(() => this.matrix.GetDeterminant());
        inverse = new Lazy<Result<SquareMatrix<TDimension>, string>>(() => this.matrix.GetInverse().Map(m => new SquareMatrix<TDimension>(m)));
    }

    internal SquareMatrix(double[,] values) : this(new Matrix<TDimension, TDimension>(values))
    {
    }

    /// <summary>Returns the element-wise sum of this matrix and <paramref name="other"/>.</summary>
    /// <param name="other">The matrix to add.</param>
    /// <returns>A new matrix containing the element-wise sum.</returns>
    public SquareMatrix<TDimension> Add(SquareMatrix<TDimension> other) => new SquareMatrix<TDimension>(matrix.Add(other.matrix));

    /// <summary>
    /// Determines whether this matrix is equal to <paramref name="other"/> by comparing all element values.
    /// </summary>
    /// <param name="other">The matrix to compare with.</param>
    /// <returns><see langword="true"/> if all elements are equal; otherwise <see langword="false"/>.</returns>
    public bool Equals(SquareMatrix<TDimension> other) => matrix.Equals(other.matrix);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => matrix.Equals(obj);

    /// <inheritdoc/>
    public override int GetHashCode() => matrix.GetHashCode();

    /// <summary>
    /// Returns a new matrix with every element multiplied by <paramref name="scalar"/>.
    /// </summary>
    /// <param name="scalar">The scalar value to multiply by.</param>
    /// <returns>A new matrix scaled by <paramref name="scalar"/>.</returns>
    public SquareMatrix<TDimension> Multiply(double scalar) => new SquareMatrix<TDimension>(matrix.Multiply(scalar));

    /// <summary>
    /// Returns the matrix product of this matrix and <paramref name="other"/>.
    /// </summary>
    /// <param name="other">The right-hand square matrix to multiply by.</param>
    /// <returns>A new matrix that is the product of this matrix and <paramref name="other"/>.</returns>
    public SquareMatrix<TDimension> Multiply(SquareMatrix<TDimension> other) => new SquareMatrix<TDimension>(matrix.Multiply(other.matrix));

    /// <summary>
    /// Returns a new matrix with the specified cell replaced by <paramref name="value"/>.
    /// The original matrix is not modified.
    /// </summary>
    /// <param name="row">Zero-based row index.</param>
    /// <param name="column">Zero-based column index.</param>
    /// <param name="value">The value to place at the specified cell.</param>
    /// <returns>A <see cref="Result{T,TError}"/> containing the updated matrix, or an error message if the indices are out of range.</returns>
    public Result<SquareMatrix<TDimension>, string> SetValue(int row, int column, double value) => matrix.SetValue(row, column, value).Map(m => new SquareMatrix<TDimension>(m));

    /// <summary>Returns the element-wise difference of this matrix minus <paramref name="other"/>.</summary>
    /// <param name="other">The matrix to subtract.</param>
    /// <returns>A new matrix containing the element-wise difference.</returns>
    public SquareMatrix<TDimension> Subtract(SquareMatrix<TDimension> other) => new SquareMatrix<TDimension>(matrix.Subtract(other.matrix));

    /// <inheritdoc/>
    public override string ToString() => matrix.ToString();

    /// <summary>
    /// Returns the transpose of this matrix, swapping rows and columns.
    /// </summary>
    /// <returns>A new <see cref="SquareMatrix{TDimension}"/> that is the transpose of this matrix.</returns>
    public SquareMatrix<TDimension> Transpose() => new SquareMatrix<TDimension>(matrix.Transpose());

    /// <summary>Returns <see langword="true"/> if both matrices have identical element values.</summary>
    public static bool operator ==(SquareMatrix<TDimension> left, SquareMatrix<TDimension> right) => left.Equals(right);

    /// <summary>Returns <see langword="true"/> if the matrices differ in any element value.</summary>
    public static bool operator !=(SquareMatrix<TDimension> left, SquareMatrix<TDimension> right) => !left.Equals(right);

    /// <summary>Returns the element-wise sum of <paramref name="left"/> and <paramref name="right"/>.</summary>
    public static SquareMatrix<TDimension> operator +(SquareMatrix<TDimension> left, SquareMatrix<TDimension> right) => left.Add(right);

    /// <summary>Returns the element-wise difference of <paramref name="left"/> minus <paramref name="right"/>.</summary>
    public static SquareMatrix<TDimension> operator -(SquareMatrix<TDimension> left, SquareMatrix<TDimension> right) => left.Subtract(right);

    /// <summary>Returns a new matrix with every element of <paramref name="matrix"/> multiplied by <paramref name="scalar"/>.</summary>
    public static SquareMatrix<TDimension> operator *(SquareMatrix<TDimension> matrix, double scalar) => matrix.Multiply(scalar);

    /// <summary>Returns a new matrix with every element of <paramref name="matrix"/> multiplied by <paramref name="scalar"/>.</summary>
    public static SquareMatrix<TDimension> operator *(double scalar, SquareMatrix<TDimension> matrix) => matrix.Multiply(scalar);

    /// <summary>Returns the matrix product of <paramref name="left"/> and <paramref name="right"/>.</summary>
    public static SquareMatrix<TDimension> operator *(SquareMatrix<TDimension> left, SquareMatrix<TDimension> right) => left.Multiply(right);
}

/// <summary>
/// Factory methods for constructing <see cref="SquareMatrix{TDimension}"/> instances.
/// </summary>
public static class SquareMatrix
{
    /// <summary>
    /// Creates a new square matrix wrapping an existing <see cref="Matrix{TRows,TColumns}"/> instance.
    /// </summary>
    /// <typeparam name="TDimension">A <see cref="Dimension"/> subtype encoding both the row and column count.</typeparam>
    /// <param name="matrix">The square matrix to wrap.</param>
    /// <returns>A <see cref="Result{T,TError}"/> containing the new square matrix, or an error message if construction fails.</returns>
    public static Result<SquareMatrix<TDimension>, string> Create<TDimension>(Matrix<TDimension, TDimension> matrix)
        where TDimension : Dimension
    {
        try
        {
            return new SquareMatrix<TDimension>(matrix);
        }
        catch (ArgumentException ex)
        {
            return ex.Message;
        }
    }

    /// <summary>
    /// Creates a new square matrix from a two-dimensional array.
    /// </summary>
    /// <typeparam name="TDimension">A <see cref="Dimension"/> subtype encoding both the row and column count.</typeparam>
    /// <param name="values">A two-dimensional array whose dimensions must match <typeparamref name="TDimension"/> x <typeparamref name="TDimension"/>.</param>
    /// <returns>A <see cref="Result{T,TError}"/> containing the new square matrix, or an error message if the array dimensions do not match.</returns>
    public static Result<SquareMatrix<TDimension>, string> Create<TDimension>(double[,] values)
        where TDimension : Dimension
    {
        try
        {
            return new SquareMatrix<TDimension>(values);
        }
        catch (ArgumentException ex)
        {
            return ex.Message;
        }
    }
}