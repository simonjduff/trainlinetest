using System;
using System.IO;
using AddressProcessing.CSV;

namespace AddressProcessor.Tests.TestClasses
{
    public class FakeCSVReaderWriter : CSVReaderWriter
    {
        private readonly Func<string, StreamReader> _reader;
        private readonly Func<string, TextWriter> _writer;

        public FakeCSVReaderWriter(Func<string, StreamReader> reader,
            Func<string, TextWriter> writer)
        {
            _reader = reader;
            _writer = writer;
        }

        protected override Func<string,StreamReader> BuildStreamReader => _reader;
        protected override Func<string, TextWriter> BuildTextWriter => _writer;
    }
}