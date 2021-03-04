using Synercoding.FileFormats.Pdf.LowLevel;
using Synercoding.FileFormats.Pdf.LowLevel.Graphics;
using System;

namespace Synercoding.FileFormats.Pdf.Internals
{
    internal sealed class ShapeContext : IShapeContext, IDisposable
    {
        private readonly ContentStream _contentStream;

        internal GraphicsState State { get; } = new GraphicsState();

        internal Path? CurrentPath { get; set; } = null;

        internal ShapeContext(ContentStream contentStream)
        {
            _contentStream = contentStream;

            _contentStream.SaveState();
        }

        public void Dispose()
        {
            _finishPathIfNeeded();

            _contentStream.RestoreState();
        }

        public IShapeContext DefaultState(Action<GraphicsState> configureState)
        {
            configureState(State);

            return this;
        }
        public IPath NewPath()
            => NewPath(_ => { });

        public IPath NewPath(Action<GraphicsState> configureState)
        {
            _finishPathIfNeeded();

            var state = State.Clone();
            configureState(state);

            var path = new Path(this, _contentStream, state);

            CurrentPath = path;

            return path;
        }

        private void _finishPathIfNeeded()
        {
            if (CurrentPath != null)
            {
                CurrentPath.FinishPath();
                CurrentPath = null;
            }
        }
    }
}
