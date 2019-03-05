using Synercoding.FileFormats.Pdf.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace Synercoding.FileFormats.Pdf.PdfInternals.XRef
{
    internal class TableBuilder
    {
        private readonly IdGenerator _idGen = new IdGenerator();
        private readonly Dictionary<PdfReference, long> _positions = new Dictionary<PdfReference, long>();

        public PdfReference ReserveId()
        {
            var id = _idGen.GetId();
            var reference = new PdfReference(id);
            _positions.Add(reference, -1);
            return reference;
        }

        public PdfReference GetId(uint position)
        {
            var id = _idGen.GetId();
            var reference = new PdfReference(id);
            _positions.Add(reference, position);
            return reference;
        }

        public void SetPosition(PdfReference id, uint position)
        {
            _positions[id] = position;
        }

        public bool Validate()
        {
            foreach(var value in _positions.Values)
            {
                if (value == -1)
                {
                    return false;
                }
            }
            return true;
        }

        public Table GetXRefTable()
        {
            var entries = _positions
                .OrderBy(x => x.Key.ObjectId)
                .Select(x => new Entry((uint)x.Value))
                .ToArray();
            return new Table(entries);
        }
    }
}