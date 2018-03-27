using System;
using System.IO;

namespace CoreTail.Test.Helpers
{
    internal class ProducerConsumerStream : Stream
    {
        private readonly MemoryStream _innerStream;
        private long _readPosition;
        private long _writePosition;
        
        public ProducerConsumerStream() => _innerStream = new MemoryStream();

        public override bool CanRead => true;
        public override bool CanWrite => true;
        public override bool CanSeek => true;
 
        public override long Length
        {
            get 
            {
                lock (_innerStream)
                    return _innerStream.Length;
            }
        }

        // ! - only Read position is returned / updated - correct only for specific usage
        public override long Position
        {
            get => _readPosition;
            set => _readPosition = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (_innerStream)
            {
                _innerStream.Position = _readPosition;
                var byteCount = _innerStream.Read(buffer, offset, count);
                _readPosition = _innerStream.Position;
                return byteCount;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (_innerStream)
            {
                _innerStream.Position = _writePosition;
                _innerStream.Write(buffer, offset, count);
                _writePosition = _innerStream.Position;
            }
        }

        public override void Flush()
        {
            lock (_innerStream)
                _innerStream.Flush();
        }

        // ! - only Read position is updated - correct only for specific usage
        public override long Seek(long offset, SeekOrigin origin)
        {
            _innerStream.Position = _readPosition;
            var position = _innerStream.Seek(offset, origin);
            _readPosition = _innerStream.Position;
            return position;
        }

        public override void SetLength(long value) => throw new NotImplementedException();
    }
}