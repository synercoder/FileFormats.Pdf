namespace Synercoding.FileFormats.Pdf.Primitives.Matrices
{
    /// <remarks>
    /// Matrix is structured as follows:
    /// a b 0
    /// c d 0
    /// e f 1
    /// </remarks>
    public class Matrix
    {
        private readonly double[,] _matrix = new double[3, 3];

        private Matrix(double[,] matrix)
        {
            _matrix = matrix;
        }

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

        public double A => _matrix[0, 0];
        public double B => _matrix[0, 1];
        public double C => _matrix[1, 0];
        public double D => _matrix[1, 1];
        public double E => _matrix[2, 0];
        public double F => _matrix[2, 1];

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
