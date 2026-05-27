public class SquareMatrixTests
{
    // -------------------------------------------------------------------------
    // GetDeterminant — 1×1
    // -------------------------------------------------------------------------

    [Fact]
    public void GetDeterminant_1x1_ReturnsScalarValue()
    {
        var matrix = CreateMatrix<One, One>(new double[1, 1] { { 7 } });

        var det = matrix.GetDeterminant();

        Assert.Equal(7.0, det);
    }

    // -------------------------------------------------------------------------
    // GetDeterminant — 2×2
    // -------------------------------------------------------------------------

    [Fact]
    public void GetDeterminant_2x2_ReturnsCorrectValue()
    {
        // det([1 2; 3 4]) = 1*4 - 2*3 = -2
        var matrix = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });

        var det = matrix.GetDeterminant();

        Assert.Equal(-2.0, det, precision: 10);
    }

    [Fact]
    public void GetDeterminant_2x2Identity_ReturnsOne()
    {
        var matrix = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 0 }, { 0, 1 } });

        var det = matrix.GetDeterminant();

        Assert.Equal(1.0, det, precision: 10);
    }

    [Fact]
    public void GetDeterminant_2x2Singular_ReturnsZero()
    {
        // Rows are multiples of each other → singular
        var matrix = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 2, 4 } });

        var det = matrix.GetDeterminant();

        Assert.Equal(0.0, det, precision: 10);
    }

    // -------------------------------------------------------------------------
    // GetDeterminant — 3×3
    // -------------------------------------------------------------------------

    [Fact]
    public void GetDeterminant_3x3_ReturnsCorrectValue()
    {
        // det([1 2 3; 4 5 6; 7 8 10]) = 1*(5*10-6*8) - 2*(4*10-6*7) + 3*(4*8-5*7)
        //                              = 1*(50-48) - 2*(40-42) + 3*(32-35)
        //                              = 2 + 4 - 9 = -3
        var matrix = CreateMatrix<Three, Three>(new double[3, 3]
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 10 }
        });

        var det = matrix.GetDeterminant();

        Assert.Equal(-3.0, det, precision: 10);
    }

    [Fact]
    public void GetDeterminant_3x3Identity_ReturnsOne()
    {
        var matrix = CreateMatrix<Three, Three>(new double[3, 3]
        {
            { 1, 0, 0 },
            { 0, 1, 0 },
            { 0, 0, 1 }
        });

        var det = matrix.GetDeterminant();

        Assert.Equal(1.0, det, precision: 10);
    }

    // -------------------------------------------------------------------------
    // GetDeterminant — sign (row swap)
    // -------------------------------------------------------------------------

    [Fact]
    public void GetDeterminant_MatrixRequiringRowSwap_ReturnsCorrectSignedValue()
    {
        // det([0 1; 1 0]) = 0*0 - 1*1 = -1
        var matrix = CreateMatrix<Two, Two>(new double[2, 2] { { 0, 1 }, { 1, 0 } });

        var det = matrix.GetDeterminant();

        Assert.Equal(-1.0, det, precision: 10);
    }

    // -------------------------------------------------------------------------
    // GetInverse — success cases
    // -------------------------------------------------------------------------

    [Fact]
    public void GetInverse_2x2_ReturnsCorrectInverse()
    {
        // inv([1 2; 3 4]) = 1/det * [4 -2; -3 1] = 1/-2 * [4 -2; -3 1]
        //                 = [-2 1; 1.5 -0.5]
        var matrix = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });

        var result = matrix.GetInverse();

        Assert.True(result.IsSuccess);
        result.Match(inv =>
        {
            Assert.Equal(-2.0, inv[0, 0].Match(v => v, _ => 0), precision: 10);
            Assert.Equal(1.0, inv[0, 1].Match(v => v, _ => 0), precision: 10);
            Assert.Equal(1.5, inv[1, 0].Match(v => v, _ => 0), precision: 10);
            Assert.Equal(-0.5, inv[1, 1].Match(v => v, _ => 0), precision: 10);
            return inv;
        },
        e => { Assert.Fail(e); return matrix; });
    }

    [Fact]
    public void GetInverse_3x3_MultiplyByOriginalGivesIdentity()
    {
        var matrix = CreateMatrix<Three, Three>(new double[3, 3]
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 10 }
        });

        var result = matrix.GetInverse();

        Assert.True(result.IsSuccess);
        result.Match(inv =>
        {
            var product = matrix.Multiply(inv);
            AssertIsIdentity(product);
            return inv;
        },
        e => { Assert.Fail(e); return matrix; });
    }

    [Fact]
    public void GetInverse_OfIdentity_ReturnsIdentity()
    {
        var identity = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 0 }, { 0, 1 } });

        var result = identity.GetInverse();

        Assert.True(result.IsSuccess);
        result.Match(inv =>
        {
            AssertIsIdentity(inv);
            return inv;
        },
        e => { Assert.Fail(e); return identity; });
    }

    // -------------------------------------------------------------------------
    // Multiply (vector)
    // -------------------------------------------------------------------------

    [Fact]
    public void MultiplyVector_ProducesCorrectProduct()
    {
        // [1 2] * [2] = [1*2+2*3] = [8]
        // [3 4]   [3]   [3*2+4*3]   [18]
        var sq = CreateSquareMatrix<Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });
        var vector = CreateVector<Two>(2.0, 3.0);

        var result = sq.Multiply(vector);

        Assert.Equal(8.0, result[0].Match(v => v, _ => 0));
        Assert.Equal(18.0, result[1].Match(v => v, _ => 0));
    }

    [Fact]
    public void MultiplyVector_WithIdentityMatrix_ReturnsUnchangedVector()
    {
        var identity = CreateSquareMatrix<Three>(new double[3, 3]
        {
            { 1, 0, 0 },
            { 0, 1, 0 },
            { 0, 0, 1 }
        });
        var vector = CreateVector<Three>(4.0, -2.0, 7.0);

        var result = identity.Multiply(vector);

        Assert.Equal(4.0, result[0].Match(v => v, _ => 0));
        Assert.Equal(-2.0, result[1].Match(v => v, _ => 0));
        Assert.Equal(7.0, result[2].Match(v => v, _ => 0));
    }

    [Fact]
    public void MultiplyVector_Rotation90Degrees_TransformsCorrectly()
    {
        // 90° rotation matrix applied to (1, 0) should give (0, 1)
        var rotation = HomogeneousMatrices.Rotate(Math.PI / 2);
        var vector = CreateVector<Three>(1.0, 0.0, 1.0); // homogeneous point

        var result = rotation.Multiply(vector);

        Assert.Equal(0.0, result[0].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(1.0, result[1].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(1.0, result[2].Match(v => v, _ => double.NaN), precision: 10);
    }

    [Fact]
    public void MultiplyVectorOperator_ProducesCorrectProduct()
    {
        var matrix = CreateSquareMatrix<Two>(new double[2, 2] { { 1, 2 }, { 3, 4 } });
        var vector = CreateVector<Two>(2.0, 3.0);

        var result = matrix * vector;

        Assert.Equal(8.0, result[0].Match(v => v, _ => 0));
        Assert.Equal(18.0, result[1].Match(v => v, _ => 0));
    }

    // -------------------------------------------------------------------------
    // GetInverse — singular matrix
    // -------------------------------------------------------------------------

    [Fact]
    public void GetInverse_SingularMatrix_ReturnsError()
    {
        var matrix = CreateMatrix<Two, Two>(new double[2, 2] { { 1, 2 }, { 2, 4 } });

        var result = matrix.GetInverse();

        Assert.True(result.IsError);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static Matrix<TRows, TColumns> CreateMatrix<TRows, TColumns>(double[,] values)
        where TRows : Dimension
        where TColumns : Dimension
    {
        return Matrix.Create<TRows, TColumns>(values).Match(m => m, e => throw new InvalidOperationException(e));
    }

    private static SquareMatrix<TDimension> CreateSquareMatrix<TDimension>(double[,] values)
        where TDimension : Dimension
    {
        return SquareMatrix.Create<TDimension>(values).Match(m => m, e => throw new InvalidOperationException(e));
    }

    private static Vector<TDimension> CreateVector<TDimension>(params double[] components)
        where TDimension : Dimension
    {
        return Vector.Create<TDimension>(components).Match(v => v, e => throw new InvalidOperationException(e));
    }

    private static void AssertIsIdentity<TSize>(Matrix<TSize, TSize> matrix)
        where TSize : Dimension
    {
        int n = matrix.Rows;
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                double expected = i == j ? 1.0 : 0.0;
                double actual = matrix[i, j].Match(v => v, _ => 0.0);
                Assert.Equal(expected, actual, precision: 8);
            }
        }
    }
}
