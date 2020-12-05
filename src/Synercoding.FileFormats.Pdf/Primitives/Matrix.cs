using Synercoding.Primitives;
using System;
using System.Runtime.CompilerServices;

namespace Synercoding.FileFormats.Pdf.Primitives
{
    /// <summary>
    /// Struct that represents a <see cref="Matrix"/>
    /// </summary>
    /// <remarks>
    /// Matrix is structured as follows:
    /// <code>
    /// A B 0
    /// C D 0
    /// E F 1
    /// </code>
    /// </remarks>
    public readonly struct Matrix : IEquatable<Matrix>
    {
        /// <summary>
        /// The identity matrix
        /// </summary>
        /// <remarks>
        /// This matrix can be interpreted as:
        /// <list type="bullet">
        ///   <item>translation with (0,0)</item>
        ///   <item>rotation with 0 degrees</item>
        ///   <item>scaling with (1,1)</item>
        /// </list>
        /// </remarks>
        public static Matrix Identity { get; } = new Matrix(1, 0, 0, 1, 0, 0);

        /// <summary>
        /// Copy constructor for the <see cref="Matrix"/>
        /// </summary>
        /// <param name="matrix">The matrix to copy</param>
        public Matrix(Matrix matrix)
        {
            A = matrix.A;
            B = matrix.B;
            C = matrix.C;
            D = matrix.D;
            E = matrix.E;
            F = matrix.F;
        }

        /// <summary>
        /// Constructor for the <see cref="Matrix"/>
        /// </summary>
        /// <param name="a">The <see cref="A"/> value</param>
        /// <param name="b">The <see cref="B"/> value</param>
        /// <param name="c">The <see cref="C"/> value</param>
        /// <param name="d">The <see cref="D"/> value</param>
        /// <param name="e">The <see cref="E"/> value</param>
        /// <param name="f">The <see cref="F"/> value</param>
        public Matrix(double a, double b, double c, double d, double e, double f)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            E = e;
            F = f;
        }

        /// <summary>
        /// The A value
        /// </summary>
        public double A { get; init; }

        /// <summary>
        /// The B value
        /// </summary>
        public double B { get; init; }

        /// <summary>
        /// The C value
        /// </summary>
        public double C { get; init; }

        /// <summary>
        /// The D value
        /// </summary>
        public double D { get; init; }

        /// <summary>
        /// The E value
        /// </summary>
        public double E { get; init; }

        /// <summary>
        /// The F value
        /// </summary>
        public double F { get; init; }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
            => obj is Rectangle other && Equals(other);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Matrix other)
            => A.Equals(other.A)
            && B.Equals(other.B)
            && C.Equals(other.C)
            && D.Equals(other.D)
            && E.Equals(other.E)
            && F.Equals(other.F);

        /// <inheritdoc/>
        public override int GetHashCode()
            => HashCode.Combine(A, B, C, D, E, F);

        /// <inheritdoc/>
        public override string ToString()
            => $"Matrix [ {A}, {B}, {C}, {D}, {E}, {F} ]";

        /// <summary>
        /// Apply a rotation operation on the matrix
        /// </summary>
        /// <param name="degrees">The amount of degrees to rotate</param>
        /// <returns>The new <see cref="Matrix"/></returns>
        public Matrix Rotate(double degrees)
            => Multiply(CreateRotationMatrix(degrees));

        /// <summary>
        /// Apply a horizontal flip operation on the matrix
        /// </summary>
        /// <returns>The new <see cref="Matrix"/></returns>
        public Matrix FlipHorizontal()
            => Multiply(CreateScaleMatrix(-1, 1));

        /// <summary>
        /// Apply a vertical flip operation on the matrix
        /// </summary>
        /// <returns>The new <see cref="Matrix"/></returns>
        public Matrix FlipVertical()
            => Multiply(CreateScaleMatrix(1, -1));

        /// <summary>
        /// Apply a scale operation on the matrix
        /// </summary>
        /// <param name="x">The X amount to scale</param>
        /// <param name="y">The Y amount to scale</param>
        /// <returns>The new <see cref="Matrix"/></returns>
        public Matrix Scale(double x, double y)
            => Multiply(CreateScaleMatrix(x, y));

        /// <summary>
        /// Apply a scale operation on the matrix
        /// </summary>
        /// <param name="x">The X amount to scale</param>
        /// <param name="y">The Y amount to scale</param>
        /// <returns>The new <see cref="Matrix"/></returns>
        public Matrix Scale(Value x, Value y)
            => Multiply(CreateScaleMatrix(x, y));

        /// <summary>
        /// Apply a skew operation on the matrix
        /// </summary>
        /// <param name="a">The amount of degrees to skew (top left direction)</param>
        /// <param name="b">The amount of degrees to skew (bottom right direction)</param>
        /// <returns>The new <see cref="Matrix"/></returns>
        public Matrix Skew(double a, double b)
            => Multiply(CreateSkewMatrix(a, b));

        /// <summary>
        /// Apply a translation operation on the matrix
        /// </summary>
        /// <param name="x">The X distance to translate</param>
        /// <param name="y">The Y distance to translate</param>
        /// <returns>The new <see cref="Matrix"/></returns>
        public Matrix Translate(double x, double y)
            => Multiply(CreateTranslationMatrix(x, y));

        /// <summary>
        /// Apply a translation operation on the matrix
        /// </summary>
        /// <param name="x">The X distance to translate</param>
        /// <param name="y">The Y distance to translate</param>
        /// <returns>The new <see cref="Matrix"/></returns>
        public Matrix Translate(Value x, Value y)
            => Multiply(CreateTranslationMatrix(x, y));

        /// <summary>
        /// Multiply matrix <paramref name="m1"/> with <paramref name="m2"/>
        /// </summary>
        /// <param name="m1">The left side of the matrix multiplication</param>
        /// <param name="m2">The right side of the matrix multiplication</param>
        /// <returns>The new <see cref="Matrix"/></returns>
        public static Matrix operator *(Matrix m1, Matrix m2)
            => m1.Multiply(m2);

        /// <summary>
        /// Multiply this matrix with <paramref name="other"/>
        /// </summary>
        /// <param name="other">The matrix to multiply with</param>
        /// <returns>The new <see cref="Matrix"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix Multiply(in Matrix other)
            => new Matrix(
                a: ( A * other.A ) + ( B * other.C ),
                b: ( A * other.B ) + ( B * other.D ),
                c: ( C * other.A ) + ( D * other.C ),
                d: ( C * other.B ) + ( D * other.D ),
                e: ( E * other.A ) + ( F * other.C ) + other.E,
                f: ( E * other.B ) + ( F * other.D ) + other.F);

        /// <summary>
        /// Create a matrix used for rotation
        /// </summary>
        /// <param name="degree">The amount of degrees to rotate by</param>
        /// <returns>A rotated matrix</returns>
        public static Matrix CreateRotationMatrix(double degree)
        {
            var rads = _degreeToRad(degree);
            return new Matrix(
                Math.Cos(rads), Math.Sin(rads) * -1,
                Math.Sin(rads), Math.Cos(rads),
                0, 0);
        }

        /// <summary>
        /// Create a matrix used for scaling
        /// </summary>
        /// <param name="x">The X scale</param>
        /// <param name="y">The Y scale</param>
        /// <returns>A scaled matrix</returns>
        public static Matrix CreateScaleMatrix(double x, double y)
            => new Matrix(
                x, 0,
                0, y,
                0, 0);

        /// <summary>
        /// Create a matrix used for scaling
        /// </summary>
        /// <param name="x">The X scale</param>
        /// <param name="y">The Y scale</param>
        /// <returns>A scaled matrix</returns>
        public static Matrix CreateScaleMatrix(Value x, Value y)
            => new Matrix(
                x.ConvertTo(Unit.Points).Raw, 0,
                0, y.ConvertTo(Unit.Points).Raw,
                0, 0);

        /// <summary>
        /// Create a matrix used for skewing
        /// </summary>
        /// <param name="a">The amount of degree to skew the top left direction</param>
        /// <param name="b">The amount of degree to skew the bottom right direction</param>
        /// <returns>A skewed matrix</returns>
        public static Matrix CreateSkewMatrix(double a, double b)
            => new Matrix(
                1, Math.Tan(_degreeToRad(a)),
                Math.Tan(_degreeToRad(b)), 1,
                0, 0);

        /// <summary>
        /// Create a matrix used for translation
        /// </summary>
        /// <param name="x">The amount of X translation</param>
        /// <param name="y">The amount of Y translation</param>
        /// <returns>A matrix representing a translation</returns>
        public static Matrix CreateTranslationMatrix(double x, double y)
            => new Matrix(
                1, 0,
                0, 1,
                x, y);

        /// <summary>
        /// Create a matrix used for translation
        /// </summary>
        /// <param name="x">The amount of X translation</param>
        /// <param name="y">The amount of Y translation</param>
        /// <returns>A matrix representing a translation</returns>
        public static Matrix CreateTranslationMatrix(Value x, Value y)
            => new Matrix(
                1, 0,
                0, 1,
                x.ConvertTo(Unit.Points).Raw, y.ConvertTo(Unit.Points).Raw);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double _degreeToRad(double degree)
            => degree * Math.PI / 180;
    }
}
