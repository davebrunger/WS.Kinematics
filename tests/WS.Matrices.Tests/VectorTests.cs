public class VectorTests
{
    // -------------------------------------------------------------------------
    // Vector.Create
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_WithCorrectComponentCount_ReturnsSuccess()
    {
        var result = Vector.Create<Two>(1.0, 2.0);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_WithWrongComponentCount_ReturnsError()
    {
        var result = Vector.Create<Three>(1.0, 2.0);

        Assert.True(result.IsError);
    }

    // -------------------------------------------------------------------------
    // Length
    // -------------------------------------------------------------------------

    [Fact]
    public void Length_ReturnsCorrectDimension()
    {
        var vector = CreateVector<Three>(1.0, 2.0, 3.0);

        Assert.Equal(3, vector.Length);
    }

    // -------------------------------------------------------------------------
    // Indexer
    // -------------------------------------------------------------------------

    [Fact]
    public void Indexer_WithValidIndex_ReturnsCorrectValue()
    {
        var vector = CreateVector<Three>(10.0, 20.0, 30.0);

        var result = vector[1];

        Assert.True(result.IsSuccess);
        result.Match(v => { Assert.Equal(20.0, v); return v; }, e => { Assert.Fail(e); return 0; });
    }

    [Fact]
    public void Indexer_WithNegativeIndex_ReturnsError()
    {
        var vector = CreateVector<Two>(1.0, 2.0);

        var result = vector[-1];

        Assert.True(result.IsError);
    }

    [Fact]
    public void Indexer_WithIndexOutOfRange_ReturnsError()
    {
        var vector = CreateVector<Two>(1.0, 2.0);

        var result = vector[2];

        Assert.True(result.IsError);
    }

    // -------------------------------------------------------------------------
    // Add
    // -------------------------------------------------------------------------

    [Fact]
    public void Add_ProducesCorrectSum()
    {
        var a = CreateVector<Three>(1.0, 2.0, 3.0);
        var b = CreateVector<Three>(4.0, 5.0, 6.0);

        var result = a.Add(b);

        Assert.Equal(5.0, result[0].Match(v => v, _ => 0));
        Assert.Equal(7.0, result[1].Match(v => v, _ => 0));
        Assert.Equal(9.0, result[2].Match(v => v, _ => 0));
    }

    [Fact]
    public void AddOperator_ProducesCorrectSum()
    {
        var a = CreateVector<Two>(3.0, 4.0);
        var b = CreateVector<Two>(1.0, 2.0);

        var result = a + b;

        Assert.Equal(4.0, result[0].Match(v => v, _ => 0));
        Assert.Equal(6.0, result[1].Match(v => v, _ => 0));
    }

    // -------------------------------------------------------------------------
    // Subtract
    // -------------------------------------------------------------------------

    [Fact]
    public void Subtract_ProducesCorrectDifference()
    {
        var a = CreateVector<Three>(5.0, 7.0, 9.0);
        var b = CreateVector<Three>(1.0, 2.0, 3.0);

        var result = a.Subtract(b);

        Assert.Equal(4.0, result[0].Match(v => v, _ => 0));
        Assert.Equal(5.0, result[1].Match(v => v, _ => 0));
        Assert.Equal(6.0, result[2].Match(v => v, _ => 0));
    }

    [Fact]
    public void SubtractOperator_ProducesCorrectDifference()
    {
        var a = CreateVector<Two>(5.0, 8.0);
        var b = CreateVector<Two>(2.0, 3.0);

        var result = a - b;

        Assert.Equal(3.0, result[0].Match(v => v, _ => 0));
        Assert.Equal(5.0, result[1].Match(v => v, _ => 0));
    }

    // -------------------------------------------------------------------------
    // SetComponent
    // -------------------------------------------------------------------------

    [Fact]
    public void SetComponent_WithValidIndex_ReturnsVectorWithUpdatedValue()
    {
        var vector = CreateVector<Three>(1.0, 2.0, 3.0);

        var result = vector.SetComponent(1, 99.0);

        Assert.True(result.IsSuccess);
        result.Match(v => { Assert.Equal(99.0, v[1].Match(x => x, _ => 0)); return v; }, e => { Assert.Fail(e); return vector; });
    }

    [Fact]
    public void SetComponent_DoesNotMutateOriginal()
    {
        var vector = CreateVector<Two>(1.0, 2.0);

        vector.SetComponent(0, 99.0);

        Assert.Equal(1.0, vector[0].Match(v => v, _ => 0));
    }

    [Fact]
    public void SetComponent_WithInvalidIndex_ReturnsError()
    {
        var vector = CreateVector<Two>(1.0, 2.0);

        var result = vector.SetComponent(5, 0.0);

        Assert.True(result.IsError);
    }

    // -------------------------------------------------------------------------
    // Equals / GetHashCode
    // -------------------------------------------------------------------------

    [Fact]
    public void Equals_WithIdenticalComponents_ReturnsTrue()
    {
        var a = CreateVector<Two>(1.0, 2.0);
        var b = CreateVector<Two>(1.0, 2.0);

        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Equals_WithDifferentComponents_ReturnsFalse()
    {
        var a = CreateVector<Two>(1.0, 2.0);
        var b = CreateVector<Two>(1.0, 3.0);

        Assert.False(a.Equals(b));
    }

    [Fact]
    public void Equals_WithSameReference_ReturnsTrue()
    {
        var a = CreateVector<Two>(1.0, 2.0);

        Assert.True(a.Equals(a));
    }

    [Fact]
    public void Equals_Object_WithNonVector_ReturnsFalse()
    {
        var a = CreateVector<Two>(1.0, 2.0);

        Assert.False(a.Equals((object)"not a vector"));
    }

    [Fact]
    public void EqualityOperator_WithIdenticalComponents_ReturnsTrue()
    {
        var a = CreateVector<Two>(1.0, 2.0);
        var b = CreateVector<Two>(1.0, 2.0);

        Assert.True(a == b);
    }

    [Fact]
    public void InequalityOperator_WithDifferentComponents_ReturnsTrue()
    {
        var a = CreateVector<Two>(1.0, 2.0);
        var b = CreateVector<Two>(1.0, 9.0);

        Assert.True(a != b);
    }

    [Fact]
    public void GetHashCode_EqualVectors_ReturnsSameHash()
    {
        var a = CreateVector<Three>(1.0, 2.0, 3.0);
        var b = CreateVector<Three>(1.0, 2.0, 3.0);

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    // -------------------------------------------------------------------------
    // ToString
    // -------------------------------------------------------------------------

    [Fact]
    public void ToString_FormatsComponentsInBrackets()
    {
        var vector = CreateVector<Three>(1.0, 2.0, 3.0);

        Assert.Equal("[1, 2, 3]", vector.ToString());
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static Vector<TDimension> CreateVector<TDimension>(params double[] components)
        where TDimension : Dimension
    {
        return Vector.Create<TDimension>(components).Match(v => v, e => throw new InvalidOperationException(e));
    }
}
