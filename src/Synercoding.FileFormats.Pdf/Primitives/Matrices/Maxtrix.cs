using System;

namespace Synercoding.FileFormats.Pdf.Primitives.Matrices
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
    public readonly struct Matrix
    {
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
        public double A { get; }

        /// <summary>
        /// The B value
        /// </summary>
        public double B { get; }

        /// <summary>
        /// The C value
        /// </summary>
        public double C { get; }

        /// <summary>
        /// The D value
        /// </summary>
        public double D { get; }

        /// <summary>
        /// The E value
        /// </summary>
        public double E { get; }

        /// <summary>
        /// The F value
        /// </summary>
        public double F { get; }

        /// <summary>
        /// Apply a rotation operation on the matrix
        /// </summary>
        /// <param name="degrees">The amount of degrees to rotate</param>
        /// <returns>The new <see cref="Matrix"/></returns>
        public Matrix Rotate(double degrees)
            => Multiply(CreateRotationMatrix(degrees));

        /// <summary>
        /// Apply a scale operation on the matrix
        /// </summary>
        /// <param name="x">The X amount to scale</param>
        /// <param name="y">The Y amount to scale</param>
        /// <returns>The new <see cref="Matrix"/></returns>
        public Matrix Scale(double x, double y)
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
        public Matrix Multiply(in Matrix other)
        {
            double a, b, c, d, e, f;

            a = ( this.A * other.A ) + ( this.B * other.C );
            b = ( this.A * other.B ) + ( this.B * other.D );
            c = ( this.C * other.A ) + ( this.D * other.C );
            d = ( this.C * other.B ) + ( this.D * other.D );
            e = ( this.E * other.A ) + ( this.F * other.C ) + other.E;
            f = ( this.E * other.B ) + ( this.F * other.D ) + other.F;

            return new Matrix(a, b, c, d, e, f);
        }

        /// <summary>
        /// Create a matrix used for rotation
        /// </summary>
        /// <param name="degree">The amount of degrees to rotate by</param>
        /// <returns>A rotated matrix</returns>
        public static Matrix CreateRotationMatrix(double degree)
            => new Matrix(
                Math.Cos(_degreeToRad(degree)), Math.Sin(_degreeToRad(degree)) * -1,
                Math.Sin(_degreeToRad(degree)), Math.Cos(_degreeToRad(degree)),
                0, 0);

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

        private static double _degreeToRad(double degree)
        {
            return degree * Math.PI / 180;
        }
    }

}
