﻿using System;
using System.IO;

namespace Lucene.Net.Store.Azure
{
    /// <summary>
    /// Stream wrapper around IndexInput
    /// </summary>
    public class StreamInput : Stream
    {
        protected IndexInput Input { get; set; }

        public StreamInput(IndexInput input)
        {
            this.Input = input;
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;
        public override void Flush() { }
        public override long Length => Input.Length;

        public override long Position
        {
            get => Input.GetFilePointer();
            set => Input.Seek(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {

            var pos = Input.GetFilePointer();
            try
            {
                var len = Input.Length;
                if (count > (len - pos))
                    count = (int)(len - pos);
                Input.ReadBytes(buffer, offset, count);
            }
            catch (Exception) { }
            return (int)(Input.GetFilePointer() - pos);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Input.Seek(offset);
                    break;
                case SeekOrigin.Current:
                    Input.Seek(Input.GetFilePointer() + offset);
                    break;
                case SeekOrigin.End:
                    throw new System.NotImplementedException();
            }
            return Input.GetFilePointer();
        }

        public override void SetLength(long value) => throw new NotImplementedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();

        public override void Close()
        {
            Input.Dispose();
            base.Close();
        }
    }
}
