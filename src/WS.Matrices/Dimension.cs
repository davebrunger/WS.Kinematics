namespace WS.Matrices;

/// <summary>
/// Base class for type-safe compile-time matrix dimension tokens.
/// Subtypes (<see cref="One"/> through <see cref="Five"/>) are used as generic type
/// parameters to encode matrix dimensions in the type system.
/// </summary>
public abstract class Dimension
{
    private static readonly Dictionary<Type, int> numberValues = new()
    {
        { typeof(One), One.Value },
        { typeof(Two), Two.Value },
        { typeof(Three), Three.Value },
        { typeof(Four), Four.Value },
        { typeof(Five), Five.Value }    
    };

    internal Dimension()
    {
    }

    /// <summary>
    /// Returns the integer value corresponding to the dimension type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">A concrete <see cref="Dimension"/> subtype.</typeparam>
    /// <returns>The integer size represented by <typeparamref name="T"/>.</returns>
    public static int ToInt<T>() where T : Dimension
    {
        return numberValues[typeof(T)];
    }
}

/// <summary>Dimension token representing a size of 1.</summary>
public sealed class One : Dimension
{
    /// <summary>The integer value of this dimension.</summary>
    public static readonly int Value = 1;

    private One()
    {
    }
}

/// <summary>Dimension token representing a size of 2.</summary>
public sealed class Two : Dimension
{
    /// <summary>The integer value of this dimension.</summary>
    public static readonly int Value = 2;

    private Two()
    {
    }
}

/// <summary>Dimension token representing a size of 3.</summary>
public sealed class Three : Dimension
{
    /// <summary>The integer value of this dimension.</summary>
    public static readonly int Value = 3;

    private Three()
    {
    }
}

/// <summary>Dimension token representing a size of 4.</summary>
public sealed class Four : Dimension
{
    /// <summary>The integer value of this dimension.</summary>
    public static readonly int Value = 4;

    private Four()
    {
    }
}

/// <summary>Dimension token representing a size of 5.</summary>
public sealed class Five : Dimension
{
    /// <summary>The integer value of this dimension.</summary>
    public static readonly int Value = 5;

    private Five()
    {
    }
}




