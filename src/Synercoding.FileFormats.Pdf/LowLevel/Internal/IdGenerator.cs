namespace Synercoding.FileFormats.Pdf.LowLevel.Internal
{
    internal sealed class IdGenerator
    {
        private int _value;

        public IdGenerator()
            : this(1)
        { }

        public IdGenerator(int start)
        {
            _value = start - 1;
        }

        public int GetId()
        {
            return System.Threading.Interlocked.Increment(ref _value);
        }
    }
}
