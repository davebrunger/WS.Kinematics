# WS.Matrices

A .NET matrix mathematics library targeting `netstandard2.0`.

Dimensions are encoded as generic type parameters at compile time, so operations such as multiplying incompatibly-sized matrices are caught as build errors rather than runtime exceptions. All fallible operations return `Result<T, string>` from [WS.DomainModelling.Common](https://www.nuget.org/packages/WS.DomainModelling.Common) and are never thrown as exceptions.

## Installation

```shell
dotnet add package WS.Matrices
```

## Core Concepts

### Dimension tokens

Matrix sizes are expressed as type parameters using the sealed `Dimension` subclasses `One` through `Five`. A `Matrix<Two, Three>` is guaranteed at compile time to have exactly 2 rows and 3 columns.

```csharp
// Dimensions available: One, Two, Three, Four, Five
int size = Dimension.ToInt<Three>(); // 3
```

### Reading values

All indexers return `Result<double, string>`. Unwrap with `.Match`:

```csharp
double value = matrix[0, 1].Match(
    success => success,
    error   => throw new InvalidOperationException(error));
```

---

## Matrix\<TRows, TColumns\>

A general-purpose immutable rectangular matrix.

### Creating a matrix

```csharp
var values = new double[,]
{
    { 1, 2, 3 },
    { 4, 5, 6 }
};

Result<Matrix<Two, Three>, string> result = Matrix.Create<Two, Three>(values);

Matrix<Two, Three> matrix = result.Match(m => m, e => throw new InvalidOperationException(e));
```

### Arithmetic

```csharp
Matrix<Two, Two> a = /* ... */;
Matrix<Two, Two> b = /* ... */;

Matrix<Two, Two> sum        = a.Add(b);        // or a + b
Matrix<Two, Two> difference = a.Subtract(b);   // or a - b
Matrix<Two, Two> scaled     = a.Multiply(2.0); // or a * 2.0

// Matrix multiplication — columns of left must equal rows of right
Matrix<Two, Three> c = /* ... */;
Matrix<Three, Two> d = /* ... */;
Matrix<Two, Two>   product = c.Multiply(d);
```

### Transpose

```csharp
Matrix<Two, Three> matrix    = /* ... */;
Matrix<Three, Two> transposed = matrix.Transpose();
```

### Immutable update

`SetValue` returns a new matrix with one cell changed; the original is unmodified.

```csharp
Result<Matrix<Two, Two>, string> updated = matrix.SetValue(row: 0, column: 1, value: 99.0);
```

### Operators

| Operator | Description |
|---|---|
| `a + b` | Element-wise addition |
| `a - b` | Element-wise subtraction |
| `a * scalar` | Scalar multiplication |
| `scalar * a` | Scalar multiplication (commutative) |
| `a == b` | Equality (compares all elements) |
| `a != b` | Inequality |

---

## SquareMatrix\<TDimension\>

A square matrix that lazily computes its **determinant** and **inverse** on first access.

### Creating a square matrix

```csharp
var values = new double[,]
{
    { 1, 2 },
    { 3, 4 }
};

Result<SquareMatrix<Two>, string> result = SquareMatrix.Create<Two>(values);

SquareMatrix<Two> sq = result.Match(m => m, e => throw new InvalidOperationException(e));
```

You can also wrap an existing `Matrix<T, T>`:

```csharp
Matrix<Two, Two> matrix = /* ... */;
Result<SquareMatrix<Two>, string> result = SquareMatrix.Create<Two>(matrix);
```

### Determinant

```csharp
double det = sq.Determinant; // computed once, cached
```

### Inverse

```csharp
Result<SquareMatrix<Two>, string> inverseResult = sq.Inverse;

inverseResult.Match(
    inv   => Console.WriteLine(inv),
    error => Console.WriteLine($"Singular matrix: {error}"));
```

### Arithmetic

Square matrix arithmetic works the same as `Matrix<T,T>` and additionally supports matrix multiplication returning the same square type:

```csharp
SquareMatrix<Two> product   = a.Multiply(b); // or a * b
SquareMatrix<Two> sum       = a.Add(b);      // or a + b
SquareMatrix<Two> transpose = a.Transpose();
```

### Operators

| Operator | Description |
|---|---|
| `a + b` | Element-wise addition |
| `a - b` | Element-wise subtraction |
| `a * b` | Matrix multiplication |
| `a * scalar` | Scalar multiplication |
| `scalar * a` | Scalar multiplication (commutative) |
| `a == b` | Equality (compares all elements) |
| `a != b` | Inequality |

---

## Vector\<TDimension\>

An immutable fixed-length vector backed by a `Dimension` type parameter.

### Creating a vector

```csharp
Result<Vector<Three>, string> result = Vector.Create<Three>(1.0, 2.0, 3.0);

Vector<Three> v = result.Match(v => v, e => throw new InvalidOperationException(e));
```

### Reading components

```csharp
double x = v[0].Match(val => val, _ => 0.0);
```

### Arithmetic

```csharp
Vector<Three> sum        = a.Add(b);      // or a + b
Vector<Three> difference = a.Subtract(b); // or a - b
```

### Immutable update

```csharp
Result<Vector<Three>, string> updated = v.SetComponent(index: 1, value: 99.0);
```

### Operators

| Operator | Description |
|---|---|
| `a + b` | Element-wise addition |
| `a - b` | Element-wise subtraction |
| `a == b` | Equality (compares all components) |
| `a != b` | Inequality |

---

## AngleRadians

A value type for angles that is always normalised to **[0, 2π)**. Implicit conversions to and from `double` allow it to be used naturally in arithmetic expressions.

```csharp
AngleRadians a = Math.PI;          // implicit from double
AngleRadians b = new AngleRadians(3 * Math.PI); // normalises to π
double radians = a;                // implicit to double

AngleRadians sum        = a + b;
AngleRadians difference = a - b;
```

### Operators

| Operator | Description |
|---|---|
| `a + b` | Adds two angles (result normalised to [0, 2π)) |
| `a - b` | Subtracts two angles (result normalised to [0, 2π)) |
| `(double) a` | Implicit conversion to `double` |
| `(AngleRadians) d` | Implicit conversion from `double` (normalises) |

---

## Homogeneous transformation matrices (2D)

`HomogeneousMatrices` provides factory methods for 3×3 homogeneous matrices that represent 2D affine transformations. Compose them by multiplying:

```csharp
// Rotate 45° then translate by (10, 5)
SquareMatrix<Three> transform =
    HomogeneousMatrices.Translate(Vector.Create<Two>(10.0, 5.0).Match(v => v, _ => throw new()))
    .Multiply(HomogeneousMatrices.Rotate(Math.PI / 4));
```

### Rotation

```csharp
SquareMatrix<Three> r = HomogeneousMatrices.Rotate(Math.PI / 2); // 90° counter-clockwise
```

### Translation

```csharp
var translation = Vector.Create<Two>(3.0, -1.0).Match(v => v, e => throw new InvalidOperationException(e));
SquareMatrix<Three> t = HomogeneousMatrices.Translate(translation);
```

### Reflection

Reflect across a line through the origin, specified either by angle or by a direction vector (which need not be normalised):

```csharp
// By angle (measured counter-clockwise from the x-axis)
SquareMatrix<Three> reflectXAxis     = HomogeneousMatrices.Reflect(0.0);          // across x-axis
SquareMatrix<Three> reflectDiagonal  = HomogeneousMatrices.Reflect(Math.PI / 4);  // across y = x

// By direction vector
var direction = Vector.Create<Two>(1.0, 1.0).Match(v => v, e => throw new InvalidOperationException(e));
SquareMatrix<Three> reflectDiagonal2 = HomogeneousMatrices.Reflect(direction);    // same as above
```

### Shear

```csharp
// shear[0] = x-shear factor (shifts x by shear[0] * y)
// shear[1] = y-shear factor (shifts y by shear[1] * x)
var shear = Vector.Create<Two>(2.0, 0.0).Match(v => v, e => throw new InvalidOperationException(e));
SquareMatrix<Three> s = HomogeneousMatrices.Shear(shear);
```

### Squeeze

Scales the x-axis by `k` and the y-axis by `1/k`, preserving area:

```csharp
SquareMatrix<Three> sq = HomogeneousMatrices.Squeeze(factor: 2.0); // x ×2, y ×0.5
```

### Stretch (non-uniform scale)

Scales each axis independently:

```csharp
var scale = Vector.Create<Two>(3.0, 0.5).Match(v => v, e => throw new InvalidOperationException(e));
SquareMatrix<Three> st = HomogeneousMatrices.Stretch(scale); // x ×3, y ×0.5
```

---

## License

MIT
