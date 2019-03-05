namespace Synercoding.FileFormats.Pdf.Primitives.Matrices
{
    /// <summary>
    /// Class that represents a <see cref="Matrix"/>
    /// </summary>
    /// <remarks>
    /// Matrix is structured as follows:
    /// <code>
    /// a b 0
    /// c d 0
    /// e f 1
    /// </code>
    /// </remarks>
    public class Matrix
    {
        private readonly double[,] _matrix = new double[3, 3];

        private Matrix(double[,] matrix)
        {
            _matrix = matrix;
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
            _matrix[0, 0] = a;
            _matrix[0, 1] = b;
            _matrix[0, 2] = 0;
            _matrix[1, 0] = c;
            _matrix[1, 1] = d;
            _matrix[1, 2] = 0;
            _matrix[2, 0] = e;
            _matrix[2, 1] = f;
            _matrix[2, 2] = 1;
        }

        /// <summary>
        /// The A value
        /// </summary>
        public double A => _matrix[0, 0];

        /// <summary>
        /// The B value
        /// </summary>
        public double B => _matrix[0, 1];

        /// <summary>
        /// The C value
        /// </summary>
        public double C => _matrix[1, 0];

        /// <summary>
        /// The D value
        /// </summary>
        public double D => _matrix[1, 1];

        /// <summary>
        /// The E value
        /// </summary>
        public double E => _matrix[2, 0];

        /// <summary>
        /// The F value
        /// </summary>
        public double F => _matrix[2, 1];

        /// <summary>
        /// Multiply this matrix with <paramref name="other"/>
        /// </summary>
        /// <param name="other">The other matrix to multiply by</param>
        /// <returns>The new <see cref="Matrix"/></returns>
        public Matrix Multiply(Matrix other)
        {
            var a = _matrix;
            var b = other._matrix;
            var resultMatrix = new double[3, 3];

            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    double temp = 0;
                    for (int currentColumn = 0; currentColumn < 3; currentColumn++)
                    {
                        temp += a[row, currentColumn] * b[currentColumn, column];
                    }
                    resultMatrix[row, column] = temp;
                }
            }

            return new Matrix(resultMatrix);
        }
    }
}
