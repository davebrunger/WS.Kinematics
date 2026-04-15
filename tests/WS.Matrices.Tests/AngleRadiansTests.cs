public class AngleRadiansTests
{
    private const double TwoPi = 2 * Math.PI;

    // -------------------------------------------------------------------------
    // Constructor / normalisation
    // -------------------------------------------------------------------------

    [Fact]
    public void Constructor_WithValueInRange_ReturnsSameValue()
    {
        var angle = new AngleRadians(1.0);

        Assert.Equal(1.0, angle.Value, precision: 10);
    }

    [Fact]
    public void Constructor_WithZero_ReturnsZero()
    {
        var angle = new AngleRadians(0.0);

        Assert.Equal(0.0, angle.Value, precision: 10);
    }

    [Fact]
    public void Constructor_WithExactlyTwoPi_NormalisesToZero()
    {
        var angle = new AngleRadians(TwoPi);

        Assert.Equal(0.0, angle.Value, precision: 10);
    }

    [Fact]
    public void Constructor_WithValueBeyondTwoPi_Wraps()
    {
        var angle = new AngleRadians(TwoPi + 1.0);

        Assert.Equal(1.0, angle.Value, precision: 10);
    }

    [Fact]
    public void Constructor_WithNegativeValue_NormalisesToPositive()
    {
        var angle = new AngleRadians(-Math.PI);

        Assert.Equal(Math.PI, angle.Value, precision: 10);
    }

    [Fact]
    public void Constructor_WithNegativeFullRotation_NormalisesToZero()
    {
        var angle = new AngleRadians(-TwoPi);

        Assert.Equal(0.0, angle.Value, precision: 10);
    }

    // -------------------------------------------------------------------------
    // Implicit conversions
    // -------------------------------------------------------------------------

    [Fact]
    public void ImplicitToDouble_ReturnsNormalisedValue()
    {
        var angle = new AngleRadians(Math.PI);

        double value = angle;

        Assert.Equal(Math.PI, value, precision: 10);
    }

    [Fact]
    public void ImplicitFromDouble_CreatesNormalisedAngle()
    {
        AngleRadians angle = Math.PI;

        Assert.Equal(Math.PI, angle.Value, precision: 10);
    }

    // -------------------------------------------------------------------------
    // Operator +
    // -------------------------------------------------------------------------

    [Fact]
    public void AddOperator_SumsAndNormalises()
    {
        var a = new AngleRadians(Math.PI);
        var b = new AngleRadians(Math.PI);

        var result = a + b;

        // PI + PI = 2PI → normalises to 0
        Assert.Equal(0.0, result.Value, precision: 10);
    }

    [Fact]
    public void AddOperator_WithNonWrappingSum_ReturnsCorrectValue()
    {
        var a = new AngleRadians(0.5);
        var b = new AngleRadians(0.5);

        var result = a + b;

        Assert.Equal(1.0, result.Value, precision: 10);
    }

    // -------------------------------------------------------------------------
    // Operator -
    // -------------------------------------------------------------------------

    [Fact]
    public void SubtractOperator_DifferenceAndNormalises()
    {
        var a = new AngleRadians(Math.PI);
        var b = new AngleRadians(Math.PI);

        var result = a - b;

        Assert.Equal(0.0, result.Value, precision: 10);
    }

    [Fact]
    public void SubtractOperator_WithNegativeResult_NormalisesToPositive()
    {
        var a = new AngleRadians(0.5);
        var b = new AngleRadians(1.5);

        var result = a - b;

        // 0.5 - 1.5 = -1.0 → normalised to 2PI - 1
        Assert.Equal(TwoPi - 1.0, result.Value, precision: 10);
    }

    // -------------------------------------------------------------------------
    // Record equality
    // -------------------------------------------------------------------------

    [Fact]
    public void Equality_WithSameNormalisedValue_ReturnsTrue()
    {
        var a = new AngleRadians(1.0);
        var b = new AngleRadians(1.0);

        Assert.Equal(a, b);
    }

    [Fact]
    public void Equality_EquivalentAngles_ReturnsTrue()
    {
        var a = new AngleRadians(TwoPi + 1.0);
        var b = new AngleRadians(1.0);

        Assert.Equal(a, b);
    }
}
