public class MatrixTests
{
    // -------------------------------------------------------------------------
    // Matrix.Create
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_WithCorrectDimensions_ReturnsSuccess()
    {
        var values = new double[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };

        var result = Matrix.Create<Two, Three>(values);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_WithWrongDimensions_ReturnsError()
    {
        var values = new double[2, 2] { { 1, 2 }, { 3, 4 } };

        var result = Matrix.Create<Two, Three>(values);

        Assert.True(result.IsError);
    }

    // -------------------------------------------------------------------------
    // Rows / Columns properties
    // -------------------------------------------------------------------------

    [Fact]
    public void Rows_ReturnsCorrectCount()
    {
        var matrix = CreateMatrix<Three, Two>(new double[3, 2]
        {
            { 1, 2 },
            { 3, 4 },
            { 5, 6 }
        });

        Assert.Equal(3, matrix.Rows);
    }

    [Fact]
    public void Columns_ReturnsCorrectCount()
    {
        var matrix = CreateMatrix<Three, Two>(new double[3, 2]
        {
            { 1, 2 },
            { 3, 4 },
            { 5, 6 }
        });

        Assert.Equal(2, matrix.Columns);
    }

    // -------------------------------------------------------------------------
    // Indexer
    // -------------------------------------------------------------------------

    [Fact]
    public void Indexer_WithValidIndices_ReturnsCorrectValue()
    {
        var matrix = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });

        var result = matrix[1, 0];

        Assert.True(result.IsSuccess);
        result.Match(v => { Assert.Equal(3.0, v); return v; }, e => { Assert.Fail(e); return 0; });
    }

    [Fact]
    public void Indexer_WithNegativeRow_ReturnsError()
    {
        var matrix = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });

        var result = matrix[-1, 0];

        Assert.True(result.IsError);
    }

    [Fact]
    public void Indexer_WithRowOutOfRange_ReturnsError()
    {
        var matrix = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });

        var result = matrix[2, 0];

        Assert.True(result.IsError);
    }

    [Fact]
    public void Indexer_WithNegativeColumn_ReturnsError()
    {
        var matrix = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });

        var result = matrix[0, -1];

        Assert.True(result.IsError);
    }

    [Fact]
    public void Indexer_WithColumnOutOfRange_ReturnsError()
    {
        var matrix = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });

        var result = matrix[0, 2];

        Assert.True(result.IsError);
    }

    // -------------------------------------------------------------------------
    // SetValue
    // -------------------------------------------------------------------------

    [Fact]
    public void SetValue_WithValidIndex_ReturnsMatrixWithUpdatedValue()
    {
        var matrix = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });

        var result = matrix.SetValue(0, 1, 99.0);

        Assert.True(result.IsSuccess);
        result.Match(
            m => { Assert.Equal(99.0, m[0, 1].Match(v => v, _ => 0)); return m; },
            e => { Assert.Fail(e); return matrix; });
    }

    [Fact]
    public void SetValue_DoesNotMutateOriginal()
    {
        var matrix = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });

        matrix.SetValue(0, 1, 99.0);

        Assert.Equal(2.0, matrix[0, 1].Match(v => v, _ => 0));
    }

    [Fact]
    public void SetValue_WithInvalidIndex_ReturnsError()
    {
        var matrix = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });

        var result = matrix.SetValue(5, 0, 1.0);

        Assert.True(result.IsError);
    }

    // -------------------------------------------------------------------------
    // Transpose
    // -------------------------------------------------------------------------

    [Fact]
    public void Transpose_FlipsRowsAndColumns()
    {
        var matrix = CreateMatrix<Two, Three>(new double[2, 3]
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        });

        var transposed = matrix.Transpose();

        Assert.Equal(3, transposed.Rows);
        Assert.Equal(2, transposed.Columns);
        Assert.Equal(matrix[0, 1].Match(v => v, _ => 0), transposed[1, 0].Match(v => v, _ => 0));
        Assert.Equal(matrix[1, 2].Match(v => v, _ => 0), transposed[2, 1].Match(v => v, _ => 0));
    }

    [Fact]
    public void Transpose_OfSquareMatrix_IsCorrect()
    {
        var matrix = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });

        var transposed = matrix.Transpose();

        // Original [0,1]=2 → transposed [1,0]=2
        Assert.Equal(2.0, transposed[1, 0].Match(v => v, _ => 0));
        // Original [1,0]=3 → transposed [0,1]=3
        Assert.Equal(3.0, transposed[0, 1].Match(v => v, _ => 0));
    }

    // -------------------------------------------------------------------------
    // Add
    // -------------------------------------------------------------------------

    [Fact]
    public void Add_ProducesCorrectSum()
    {
        var a = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });
        var b = CreateMatrix<Two, Two>(new double[2, 2] { { 5, 6 }, { 7, 8 } });

        var result = a.Add(b);

        Assert.Equal(6.0, result[0, 0].Match(v => v, _ => 0));
        Assert.Equal(8.0, result[0, 1].Match(v => v, _ => 0));
        Assert.Equal(10.0, result[1, 0].Match(v => v, _ => 0));
        Assert.Equal(12.0, result[1, 1].Match(v => v, _ => 0));
    }

    [Fact]
    public void AddOperator_ProducesCorrectSum()
    {
        var a = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });
        var b = CreateMatrix<Two, Two>(new double[2, 2] { { 5, 6 }, { 7, 8 } });

        var result = a + b;

        Assert.Equal(6.0, result[0, 0].Match(v => v, _ => 0));
    }

    // -------------------------------------------------------------------------
    // Subtract
    // -------------------------------------------------------------------------

    [Fact]
    public void Subtract_ProducesCorrectDifference()
    {
        var a = CreateMatrix<Two, Two>(new double[2, 2] { { 5, 6 }, { 7, 8 } });
        var b = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });

        var result = a.Subtract(b);

        Assert.Equal(4.0, result[0, 0].Match(v => v, _ => 0));
        Assert.Equal(4.0, result[0, 1].Match(v => v, _ => 0));
        Assert.Equal(4.0, result[1, 0].Match(v => v, _ => 0));
        Assert.Equal(4.0, result[1, 1].Match(v => v, _ => 0));
    }

    [Fact]
    public void SubtractOperator_ProducesCorrectDifference()
    {
        var a = CreateMatrix<Two, Two>(new double[2, 2] { { 5, 6 }, { 7, 8 } });
        var b = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });

        var result = a - b;

        Assert.Equal(4.0, result[0, 0].Match(v => v, _ => 0));
    }

    // -------------------------------------------------------------------------
    // Multiply (scalar)
    // -------------------------------------------------------------------------

    [Fact]
    public void MultiplyScalar_ScalesAllElements()
    {
        var matrix = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });

        var result = matrix.Multiply(3.0);

        Assert.Equal(3.0, result[0, 0].Match(v => v, _ => 0));
        Assert.Equal(6.0, result[0, 1].Match(v => v, _ => 0));
        Assert.Equal(9.0, result[1, 0].Match(v => v, _ => 0));
        Assert.Equal(12.0, result[1, 1].Match(v => v, _ => 0));
    }

    [Fact]
    public void MultiplyScalarOperator_ScalesAllElements()
    {
        var matrix = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });

        var result = matrix * 2.0;

        Assert.Equal(2.0, result[0, 0].Match(v => v, _ => 0));
        Assert.Equal(4.0, result[0, 1].Match(v => v, _ => 0));
    }

    // -------------------------------------------------------------------------
    // Multiply (matrix)
    // -------------------------------------------------------------------------

    [Fact]
    public void MultiplyMatrix_ProducesCorrectProduct()
    {
        // [1 2] * [5 6] = [1*5+2*7  1*6+2*8] = [19 22]
        // [3 4]   [7 8]   [3*5+4*7  3*6+4*8]   [43 50]
        var a = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });
        var b = CreateMatrix<Two, Two>(new double[2, 2] { { 5, 6 }, { 7, 8 } });

        var result = a.Multiply(b);

        Assert.Equal(19.0, result[0, 0].Match(v => v, _ => 0));
        Assert.Equal(22.0, result[0, 1].Match(v => v, _ => 0));
        Assert.Equal(43.0, result[1, 0].Match(v => v, _ => 0));
        Assert.Equal(50.0, result[1, 1].Match(v => v, _ => 0));
    }

    [Fact]
    public void MultiplyMatrix_NonSquare_ProducesCorrectShape()
    {
        // 2x3 * 3x2 = 2x2
        var a = CreateMatrix<Two, Three>(new double[2, 3]
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        });
        var b = CreateMatrix<Three, Two>(new double[3, 2]
        {
            { 7,  8 },
            { 9,  10 },
            { 11, 12 }
        });

        var result = a.Multiply(b);

        Assert.Equal(2, result.Rows);
        Assert.Equal(2, result.Columns);
        // Row 0: 1*7+2*9+3*11=58, 1*8+2*10+3*12=64
        Assert.Equal(58.0, result[0, 0].Match(v => v, _ => 0));
        Assert.Equal(64.0, result[0, 1].Match(v => v, _ => 0));
    }

    // -------------------------------------------------------------------------
    // Equals / GetHashCode / operators == and !=
    // -------------------------------------------------------------------------

    [Fact]
    public void Equals_IdenticalMatrices_ReturnsTrue()
    {
        var a = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });
        var b = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });

        Assert.Equal(a, b);
    }

    [Fact]
    public void Equals_DifferentMatrices_ReturnsFalse()
    {
        var a = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });
        var b = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 5 } });

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void EqualityOperator_IdenticalMatrices_ReturnsTrue()
    {
        var a = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });
        var b = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });

        Assert.True(a == b);
    }

    [Fact]
    public void InequalityOperator_DifferentMatrices_ReturnsTrue()
    {
        var a = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });
        var b = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 5 } });

        Assert.True(a != b);
    }

    [Fact]
    public void GetHashCode_IdenticalMatrices_ReturnsSameHash()
    {
        var a = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });
        var b = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    // -------------------------------------------------------------------------
    // ToString
    // -------------------------------------------------------------------------

    [Fact]
    public void ToString_ContainsExpectedValues()
    {
        var matrix = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });

        var text = matrix.ToString();

        Assert.Contains("1", text);
        Assert.Contains("4", text);
    }

    // -------------------------------------------------------------------------
    // Helper
    // -------------------------------------------------------------------------

    private static Matrix<TRows, TColumns> CreateMatrix<TRows, TColumns>(double[,] values)
        where TRows : Dimension
        where TColumns : Dimension
    {
        return Matrix.Create<TRows, TColumns>(values).Match(m => m, e => throw new InvalidOperationException(e));
    }
}
