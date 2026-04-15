namespace WS.Matrices;

/// <summary>
/// Represents an immutable matrix with compile-time-encoded row and column counts.
/// </summary>
/// <typeparam name="TRows">A <see cref="Dimension"/> subtype encoding the number of rows.</typeparam>
/// <typeparam name="TColumns">A <see cref="Dimension"/> subtype encoding the number of columns.</typeparam>
public class Matrix<TRows, TColumns> : IMatrix<TRows, TColumns, Matrix<TRows, TColumns>>
    where TRows : Dimension
    where TColumns : Dimension
{
    private readonly double[,] values;

    /// <summary>Gets the number of rows in the matrix.</summary>
    public int Rows => Dimension.ToInt<TRows>();

    /// <summary>Gets the number of columns in the matrix.</summary>
    public int Columns => Dimension.ToInt<TColumns>();

    internal Matrix(double[,] values)
    {
        int rows = values.GetLength(0);
        int columns = values.GetLength(1);

        if (rows != Rows || columns != Columns)
        {
            throw new ArgumentException($"The dimensions of the provided array do not match the specified matrix dimensions. Expected: {Rows}x{Columns}, Actual: {rows}x{columns}");
        }

        this.values = values;
    }

    /// <summary>
    /// Gets the value at the specified row and column.
    /// </summary>
    /// <param name="row">Zero-based row index.</param>
    /// <param name="column">Zero-based column index.</param>
    /// <returns>A <see cref="Result{T,TError}"/> containing the value, or an error message if the indices are out of range.</returns>
    public Result<double, string> this[int row, int column]
    {
        get
        {
            if (row < 0 || row >= Rows || column < 0 || column >= Columns)
            {
                return $"Index out of range. Valid row indices: 0 to {Rows - 1}, Valid column indices: 0 to {Columns - 1}";
            }
            return values[row, column];
        }
    }

    /// <summary>
    /// Returns the element-wise sum of this matrix and <paramref name="other"/>.
    /// </summary>
    /// <param name="other">The matrix to add.</param>
    /// <returns>A new matrix containing the element-wise sum.</returns>
    public Matrix<TRows, TColumns> Add(Matrix<TRows, TColumns> other)
    {
        var resultValues = new double[Rows, Columns];
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                resultValues[i, j] = values[i, j] + other.values[i, j];
            }
        }
        return new Matrix<TRows, TColumns>(resultValues);
    }

    /// <summary>
    /// Determines whether this matrix is equal to <paramref name="other"/> by comparing all element values.
    /// </summary>
    /// <param name="other">The matrix to compare with.</param>
    /// <returns><see langword="true"/> if all elements are equal; otherwise <see langword="false"/>.</returns>
    public bool Equals(Matrix<TRows, TColumns> other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                if (values[i, j] != other.values[i, j])
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Matrix<TRows, TColumns> other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hash = 17;
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                hash = hash * 31 + values[i, j].GetHashCode();
            }
        }
        return hash;
    }

    /// <summary>
    /// Returns a new matrix with every element multiplied by <paramref name="scalar"/>.
    /// </summary>
    /// <param name="scalar">The scalar value to multiply by.</param>
    /// <returns>A new matrix scaled by <paramref name="scalar"/>.</returns>
    public Matrix<TRows, TColumns> Multiply(double scalar)
    {
        var resultValues = new double[Rows, Columns];
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                resultValues[i, j] = values[i, j] * scalar;
            }
        }
        return new Matrix<TRows, TColumns>(resultValues);
    }

    /// <summary>
    /// Returns the matrix product of this matrix and <paramref name="other"/>.
    /// </summary>
    /// <typeparam name="TOtherColumns">The column dimension of <paramref name="other"/> and the result.</typeparam>
    /// <param name="other">The right-hand matrix to multiply by.</param>
    /// <returns>A new matrix that is the product of this matrix and <paramref name="other"/>.</returns>
    public Matrix<TRows, TOtherColumns> Multiply<TOtherColumns>(Matrix<TColumns, TOtherColumns> other)
        where TOtherColumns : Dimension
    {
        var resultValues = new double[Rows, other.Columns];
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < other.Columns; j++)
            {
                double sum = 0;
                for (int k = 0; k < Columns; k++)
                {
                    sum += values[i, k] * other[k, j].Match(v => v, _ => 0);
                }
                resultValues[i, j] = sum;
            }
        }
        return new Matrix<TRows, TOtherColumns>(resultValues);
    }

    /// <summary>
    /// Returns a new matrix with the specified cell replaced by <paramref name="value"/>.
    /// The original matrix is not modified.
    /// </summary>
    /// <param name="row">Zero-based row index.</param>
    /// <param name="column">Zero-based column index.</param>
    /// <param name="value">The value to place at the specified cell.</param>
    /// <returns>A <see cref="Result{T,TError}"/> containing the updated matrix, or an error message if the indices are out of range.</returns>
    public Result<Matrix<TRows, TColumns>, string> SetValue(int row, int column, double value)
    {
        if (row < 0 || row >= Rows || column < 0 || column >= Columns)
        {
            return $"Index out of range. Valid row indices: 0 to {Rows - 1}, Valid column indices: 0 to {Columns - 1}";
        }

        var newValues = (double[,])values.Clone();
        newValues[row, column] = value;
        return new Matrix<TRows, TColumns>(newValues);
    }

    /// <summary>
    /// Returns the element-wise difference of this matrix minus <paramref name="other"/>.
    /// </summary>
    /// <param name="other">The matrix to subtract.</param>
    /// <returns>A new matrix containing the element-wise difference.</returns>
    public Matrix<TRows, TColumns> Subtract(Matrix<TRows, TColumns> other)
    {
        var resultValues = new double[Rows, Columns];
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                resultValues[i, j] = values[i, j] - other.values[i, j];
            }
        }
        return new Matrix<TRows, TColumns>(resultValues);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < Rows; i++)
        {
            sb.Append("[ ");
            for (int j = 0; j < Columns; j++)
            {
                sb.Append(values[i, j].ToString("G4")).Append(" ");
            }
            sb.AppendLine("]");
        }
        return sb.ToString();
    }

    /// <summary>
    /// Returns the transpose of this matrix, swapping rows and columns.
    /// </summary>
    /// <returns>A new <see cref="Matrix{TColumns, TRows}"/> that is the transpose of this matrix.</returns>
    public Matrix<TColumns, TRows> Transpose()
    {
        var transposedValues = new double[Columns, Rows];
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                transposedValues[j, i] = values[i, j];
            }
        }
        return new Matrix<TColumns, TRows>(transposedValues);
    }

    /// <summary>Returns <see langword="true"/> if both matrices have identical dimensions and element values.</summary>
    public static bool operator ==(Matrix<TRows, TColumns> left, Matrix<TRows, TColumns> right) => left.Equals(right);

    /// <summary>Returns <see langword="true"/> if the matrices differ in any element value.</summary>
    public static bool operator !=(Matrix<TRows, TColumns> left, Matrix<TRows, TColumns> right) => !left.Equals(right);

    /// <summary>Returns the element-wise sum of <paramref name="left"/> and <paramref name="right"/>.</summary>
    public static Matrix<TRows, TColumns> operator +(Matrix<TRows, TColumns> left, Matrix<TRows, TColumns> right) => left.Add(right);

    /// <summary>Returns the element-wise difference of <paramref name="left"/> minus <paramref name="right"/>.</summary>
    public static Matrix<TRows, TColumns> operator -(Matrix<TRows, TColumns> left, Matrix<TRows, TColumns> right) => left.Subtract(right);

    /// <summary>Returns a new matrix with every element of <paramref name="matrix"/> multiplied by <paramref name="scalar"/>.</summary>
    public static Matrix<TRows, TColumns> operator *(Matrix<TRows, TColumns> matrix, double scalar) => matrix.Multiply(scalar);

    /// <summary>Returns a new matrix with every element of <paramref name="matrix"/> multiplied by <paramref name="scalar"/>.</summary>
    public static Matrix<TRows, TColumns> operator *(double scalar, Matrix<TRows, TColumns> matrix) => matrix.Multiply(scalar);
}

/// <summary>
/// Factory methods for constructing <see cref="Matrix{TRows,TColumns}"/> instances.
/// </summary>
public static class Matrix
{
    /// <summary>
    /// Creates a new matrix from a two-dimensional array.
    /// </summary>
    /// <typeparam name="TRows">A <see cref="Dimension"/> subtype encoding the expected number of rows.</typeparam>
    /// <typeparam name="TColumns">A <see cref="Dimension"/> subtype encoding the expected number of columns.</typeparam>
    /// <param name="values">A two-dimensional array whose dimensions must match <typeparamref name="TRows"/> and <typeparamref name="TColumns"/>.</param>
    /// <returns>A <see cref="Result{T,TError}"/> containing the new matrix, or an error message if the array dimensions do not match.</returns>
    public static Result<Matrix<TRows, TColumns>, string> Create<TRows, TColumns>(double[,] values)
        where TRows : Dimension
        where TColumns : Dimension
    {
        try
        {
            var matrix = new Matrix<TRows, TColumns>((double[,])values.Clone());
            return matrix;
        }
        catch (ArgumentException ex)
        {
            return ex.Message;
        }
    }
}