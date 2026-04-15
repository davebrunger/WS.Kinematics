public class HomogeneousMatricesTests
{
    // -------------------------------------------------------------------------
    // Reflect
    // -------------------------------------------------------------------------

    [Fact]
    public void Reflect_AcrossXAxis_InvertsYCoordinate()
    {
        // Axis angle = 0 (x-axis): cos(0)=1, sin(0)=0 â†’ [1,0;0,-1]
        var matrix = HomogeneousMatrices.Reflect(0.0);

        AssertMatrixValues(matrix, new double[,]
        {
            {  1, 0, 0 },
            {  0, -1, 0 },
            {  0, 0, 1 }
        });
    }

    [Fact]
    public void Reflect_AcrossYAxis_InvertsXCoordinate()
    {
        // Axis angle = Ï€/2 (y-axis): cos(Ï€)=-1, sin(Ï€)=0 â†’ [-1,0;0,1]
        var matrix = HomogeneousMatrices.Reflect(Math.PI / 2);

        Assert.Equal(-1.0, matrix[0, 0].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(0.0, matrix[0, 1].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(0.0, matrix[1, 0].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(1.0, matrix[1, 1].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(1.0, matrix[2, 2].Match(v => v, _ => double.NaN), precision: 10);
    }

    [Fact]
    public void Reflect_AcrossDiagonal_SwapsCoordinates()
    {
        // Axis angle = Ï€/4 (y=x line): cos(Ï€/2)=0, sin(Ï€/2)=1 â†’ [0,1;1,0]
        var matrix = HomogeneousMatrices.Reflect(Math.PI / 4);

        Assert.Equal(0.0, matrix[0, 0].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(1.0, matrix[0, 1].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(1.0, matrix[1, 0].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(0.0, matrix[1, 1].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(1.0, matrix[2, 2].Match(v => v, _ => double.NaN), precision: 10);
    }

    [Fact]
    public void Reflect_AppliedTwice_ReturnsIdentity()
    {
        var reflection = HomogeneousMatrices.Reflect(0.7);

        var product = reflection.Multiply(reflection);

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                var expected = i == j ? 1.0 : 0.0;
                Assert.Equal(expected, product[i, j].Match(v => v, _ => double.NaN), precision: 10);
            }
        }
    }

    [Fact]
    public void Reflect_MaintainsHomogeneousRow()
    {
        var matrix = HomogeneousMatrices.Reflect(1.0);

        Assert.Equal(0.0, matrix[2, 0].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(0.0, matrix[2, 1].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(1.0, matrix[2, 2].Match(v => v, _ => double.NaN), precision: 10);
    }

    // -------------------------------------------------------------------------
    // Reflect (vector direction)
    // -------------------------------------------------------------------------

    [Fact]
    public void Reflect_DirectionAlongXAxis_InvertsYCoordinate()
    {
        // Direction (1, 0) → x-axis → [1,0;0,-1]
        var matrix = HomogeneousMatrices.Reflect(CreateVector<Two>(1.0, 0.0));

        AssertMatrixValues(matrix, new double[,]
        {
            {  1,  0, 0 },
            {  0, -1, 0 },
            {  0,  0, 1 }
        });
    }

    [Fact]
    public void Reflect_DirectionAlongYAxis_InvertsXCoordinate()
    {
        // Direction (0, 1) → y-axis → [-1,0;0,1]
        var matrix = HomogeneousMatrices.Reflect(CreateVector<Two>(0.0, 1.0));

        AssertMatrixValues(matrix, new double[,]
        {
            { -1, 0, 0 },
            {  0, 1, 0 },
            {  0, 0, 1 }
        });
    }

    [Fact]
    public void Reflect_DirectionAlongDiagonal_SwapsCoordinates()
    {
        // Direction (1, 1) → y=x line → [0,1;1,0]
        var matrix = HomogeneousMatrices.Reflect(CreateVector<Two>(1.0, 1.0));

        Assert.Equal(0.0, matrix[0, 0].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(1.0, matrix[0, 1].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(1.0, matrix[1, 0].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(0.0, matrix[1, 1].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(1.0, matrix[2, 2].Match(v => v, _ => double.NaN), precision: 10);
    }

    [Fact]
    public void Reflect_DirectionVectorScaleIsIrrelevant()
    {
        // Direction (2, 0) should give same result as (1, 0)
        var unit = HomogeneousMatrices.Reflect(CreateVector<Two>(1.0, 0.0));
        var scaled = HomogeneousMatrices.Reflect(CreateVector<Two>(2.0, 0.0));

        Assert.Equal(unit, scaled);
    }

    [Fact]
    public void Reflect_Direction_AgreesWith_AngleOverload_ForSameAxis()
    {
        // angle = π/6; direction = (cos(π/6), sin(π/6))
        var angle = Math.PI / 6;
        var byAngle = HomogeneousMatrices.Reflect(angle);
        var byDirection = HomogeneousMatrices.Reflect(CreateVector<Two>(Math.Cos(angle), Math.Sin(angle)));

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                var expected = byAngle[i, j].Match(v => v, _ => double.NaN);
                var actual = byDirection[i, j].Match(v => v, _ => double.NaN);
                Assert.Equal(expected, actual, precision: 10);
            }
        }
    }

    [Fact]
    public void Reflect_Direction_AppliedTwice_ReturnsIdentity()
    {
        var reflection = HomogeneousMatrices.Reflect(CreateVector<Two>(3.0, 1.0));

        var product = reflection.Multiply(reflection);

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                var expected = i == j ? 1.0 : 0.0;
                Assert.Equal(expected, product[i, j].Match(v => v, _ => double.NaN), precision: 10);
            }
        }
    }

    [Fact]
    public void Reflect_Direction_MaintainsHomogeneousRow()
    {
        var matrix = HomogeneousMatrices.Reflect(CreateVector<Two>(1.0, 2.0));

        Assert.Equal(0.0, matrix[2, 0].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(0.0, matrix[2, 1].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(1.0, matrix[2, 2].Match(v => v, _ => double.NaN), precision: 10);
    }

    // -------------------------------------------------------------------------
    // Rotate
    // -------------------------------------------------------------------------

    [Fact]
    public void Rotate_ByZero_ReturnsIdentity()
    {
        var matrix = HomogeneousMatrices.Rotate(0.0);

        AssertMatrixValues(matrix, new double[,]
        {
            { 1, 0, 0 },
            { 0, 1, 0 },
            { 0, 0, 1 }
        });
    }

    [Fact]
    public void Rotate_ByHalfPi_ReturnsCorrectRotation()
    {
        // 90-degree rotation: cos=0, sin=1
        var matrix = HomogeneousMatrices.Rotate(Math.PI / 2);

        Assert.Equal(0.0, matrix[0, 0].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(-1.0, matrix[0, 1].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(0.0, matrix[0, 2].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(1.0, matrix[1, 0].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(0.0, matrix[1, 1].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(0.0, matrix[1, 2].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(0.0, matrix[2, 0].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(0.0, matrix[2, 1].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(1.0, matrix[2, 2].Match(v => v, _ => double.NaN), precision: 10);
    }

    [Fact]
    public void Rotate_ByPi_ReturnsCorrectRotation()
    {
        // 180-degree rotation: cos=-1, sinâ‰ˆ0
        var matrix = HomogeneousMatrices.Rotate(Math.PI);

        Assert.Equal(-1.0, matrix[0, 0].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(0.0, matrix[0, 1].Match(v => v, _ => double.NaN), precision: 6);
        Assert.Equal(-1.0, matrix[1, 1].Match(v => v, _ => double.NaN), precision: 6);
        Assert.Equal(1.0, matrix[2, 2].Match(v => v, _ => double.NaN), precision: 10);
    }

    [Fact]
    public void Rotate_AngleMaintainsHomogeneousRow()
    {
        var matrix = HomogeneousMatrices.Rotate(1.23);

        // Bottom row must always be [0, 0, 1]
        Assert.Equal(0.0, matrix[2, 0].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(0.0, matrix[2, 1].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(1.0, matrix[2, 2].Match(v => v, _ => double.NaN), precision: 10);
    }

    [Fact]
    public void Rotate_IsOrthogonal_MultiplyByTransposeGivesIdentity()
    {
        var rotation = HomogeneousMatrices.Rotate(0.75);
        var transpose = rotation.Transpose();

        var product = rotation.Multiply(transpose);

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                var expected = i == j ? 1.0 : 0.0;
                var actual = product[i, j].Match(v => v, _ => double.NaN);
                Assert.Equal(expected, actual, precision: 10);
            }
        }
    }

    // -------------------------------------------------------------------------
    // Shear
    // -------------------------------------------------------------------------

    [Fact]
    public void Shear_WithZeroFactors_ReturnsIdentity()
    {
        var matrix = HomogeneousMatrices.Shear(CreateVector<Two>(0.0, 0.0));

        AssertMatrixValues(matrix, new double[,]
        {
            { 1, 0, 0 },
            { 0, 1, 0 },
            { 0, 0, 1 }
        });
    }

    [Fact]
    public void Shear_InXOnly_ProducesCorrectMatrix()
    {
        // shear.x=2, shear.y=0 â†’ x shifted by 2*y, y unchanged
        var matrix = HomogeneousMatrices.Shear(CreateVector<Two>(2.0, 0.0));

        AssertMatrixValues(matrix, new double[,]
        {
            { 1, 2, 0 },
            { 0, 1, 0 },
            { 0, 0, 1 }
        });
    }

    [Fact]
    public void Shear_InYOnly_ProducesCorrectMatrix()
    {
        // shear.x=0, shear.y=3 â†’ y shifted by 3*x, x unchanged
        var matrix = HomogeneousMatrices.Shear(CreateVector<Two>(0.0, 3.0));

        AssertMatrixValues(matrix, new double[,]
        {
            { 1, 0, 0 },
            { 3, 1, 0 },
            { 0, 0, 1 }
        });
    }

    [Fact]
    public void Shear_MaintainsHomogeneousRow()
    {
        var matrix = HomogeneousMatrices.Shear(CreateVector<Two>(1.5, 2.5));

        Assert.Equal(0.0, matrix[2, 0].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(0.0, matrix[2, 1].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(1.0, matrix[2, 2].Match(v => v, _ => double.NaN), precision: 10);
    }

    // -------------------------------------------------------------------------
    // Squeeze
    // -------------------------------------------------------------------------

    [Fact]
    public void Squeeze_ByOne_ReturnsIdentity()
    {
        var matrix = HomogeneousMatrices.Squeeze(1.0);

        AssertMatrixValues(matrix, new double[,]
        {
            { 1, 0, 0 },
            { 0, 1, 0 },
            { 0, 0, 1 }
        });
    }

    [Fact]
    public void Squeeze_ScalesXByFactorAndYByReciprocal()
    {
        var matrix = HomogeneousMatrices.Squeeze(2.0);

        Assert.Equal(2.0, matrix[0, 0].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(0.5, matrix[1, 1].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(1.0, matrix[2, 2].Match(v => v, _ => double.NaN), precision: 10);
    }

    [Fact]
    public void Squeeze_IsAreaPreserving_DeterminantIsOne()
    {
        var matrix = HomogeneousMatrices.Squeeze(3.0);

        Assert.Equal(1.0, matrix.Determinant, precision: 10);
    }

    [Fact]
    public void Squeeze_MaintainsHomogeneousRow()
    {
        var matrix = HomogeneousMatrices.Squeeze(4.0);

        Assert.Equal(0.0, matrix[2, 0].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(0.0, matrix[2, 1].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(1.0, matrix[2, 2].Match(v => v, _ => double.NaN), precision: 10);
    }

    // -------------------------------------------------------------------------
    // Stretch
    // -------------------------------------------------------------------------

    [Fact]
    public void Stretch_ByUnity_ReturnsIdentity()
    {
        var matrix = HomogeneousMatrices.Stretch(CreateVector<Two>(1.0, 1.0));

        AssertMatrixValues(matrix, new double[,]
        {
            { 1, 0, 0 },
            { 0, 1, 0 },
            { 0, 0, 1 }
        });
    }

    [Fact]
    public void Stretch_ScalesXAndYIndependently()
    {
        var matrix = HomogeneousMatrices.Stretch(CreateVector<Two>(3.0, 5.0));

        Assert.Equal(3.0, matrix[0, 0].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(0.0, matrix[0, 1].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(0.0, matrix[1, 0].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(5.0, matrix[1, 1].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(1.0, matrix[2, 2].Match(v => v, _ => double.NaN), precision: 10);
    }

    [Fact]
    public void Stretch_MaintainsHomogeneousRow()
    {
        var matrix = HomogeneousMatrices.Stretch(CreateVector<Two>(2.0, 4.0));

        Assert.Equal(0.0, matrix[2, 0].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(0.0, matrix[2, 1].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(1.0, matrix[2, 2].Match(v => v, _ => double.NaN), precision: 10);
    }

    // -------------------------------------------------------------------------
    // Translate
    // -------------------------------------------------------------------------

    [Fact]
    public void Translate_ProducesCorrectTranslationMatrix()
    {
        var translation = CreateVector<Two>(3.0, 5.0);

        var matrix = HomogeneousMatrices.Translate(translation);

        AssertMatrixValues(matrix, new double[,]
        {
            { 1, 0, 3 },
            { 0, 1, 5 },
            { 0, 0, 1 }
        });
    }

    [Fact]
    public void Translate_ByZero_ReturnsIdentity()
    {
        var translation = CreateVector<Two>(0.0, 0.0);

        var matrix = HomogeneousMatrices.Translate(translation);

        AssertMatrixValues(matrix, new double[,]
        {
            { 1, 0, 0 },
            { 0, 1, 0 },
            { 0, 0, 1 }
        });
    }

    [Fact]
    public void Translate_MaintainsLinearPart()
    {
        var translation = CreateVector<Two>(7.0, -2.0);

        var matrix = HomogeneousMatrices.Translate(translation);

        // Top-left 2x2 must be identity
        Assert.Equal(1.0, matrix[0, 0].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(0.0, matrix[0, 1].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(0.0, matrix[1, 0].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(1.0, matrix[1, 1].Match(v => v, _ => double.NaN), precision: 10);
    }

    // -------------------------------------------------------------------------
    // Compose: Rotate then Translate
    // -------------------------------------------------------------------------

    [Fact]
    public void RotateThenTranslate_ComposedMatrixIsCorrect()
    {
        // Rotation by 0 followed by translation (3, 4) should equal the translation matrix
        var rotate = HomogeneousMatrices.Rotate(0.0);
        var translate = HomogeneousMatrices.Translate(CreateVector<Two>(3.0, 4.0));

        var composed = translate.Multiply(rotate);

        Assert.Equal(3.0, composed[0, 2].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(4.0, composed[1, 2].Match(v => v, _ => double.NaN), precision: 10);
        Assert.Equal(1.0, composed[2, 2].Match(v => v, _ => double.NaN), precision: 10);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static Vector<TDimension> CreateVector<TDimension>(params double[] components)
        where TDimension : Dimension
    {
        return Vector.Create<TDimension>(components).Match(v => v, e => throw new InvalidOperationException(e));
    }

    private static void AssertMatrixValues(SquareMatrix<Three> matrix, double[,] expected)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                var actual = matrix[i, j].Match(v => v, _ => double.NaN);
                Assert.Equal(expected[i, j], actual, precision: 10);
            }
        }
    }
}
