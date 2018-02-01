using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AddressProcessing.CSV;
using AddressProcessor.Tests.Extensions;
using AddressProcessor.Tests.TestClasses;
using Xunit;

namespace Csv.Tests
{
    public class CSVReaderWriterTests
    {
        [Fact]
        public void WritingNothingWritesNothing()
        {
            var textWriter = new StringWriter();
            // Given a writer
            var writer = new FakeCSVReaderWriter(f => null,
                f => textWriter);

            // When I write an empty string
            writer.Open("Arbitrary", CSVReaderWriter.Mode.Write);
            writer.Write(null);
            writer.Close();

            // Then nothing is written
            Assert.True(string.IsNullOrEmpty(textWriter.ToString()));
        }

        [Fact]
        public void WriterWritesLines()
        {
            // Given a writer
            var textWriter = new StringWriter();
            var writer = new FakeCSVReaderWriter(f => null,
                f => textWriter);

            // When I write a TSV
            writer.Open("Arbitrary", CSVReaderWriter.Mode.Write);
            writer.Write("First","Second");
            writer.Close();

            // Then the string is written
            Assert.Equal($"First\tSecond{Environment.NewLine}", textWriter.ToString());
        }

        [Fact]
        public void ReaderReadsLines()
        {
            // Given a reader
            var streamReader = $"First\tSecond{Environment.NewLine}Third\tFourth{Environment.NewLine}".ToStream();
            var reader = new FakeCSVReaderWriter(f => streamReader, f => null);

            // When I read
            reader.Open("Arbitrary", CSVReaderWriter.Mode.Read);
            List<(string first, string second)> results = new List<(string first, string second)>();
            string first;
            string second;
            while (reader.Read(out first, out second))
            {
                results.Add((first, second));
            }

            // Then the results are correct
            Assert.Equal(2, results.Count());
            Assert.Equal("First", results[0].first);
            Assert.Equal("Second", results[0].second);
            Assert.Equal("Third", results[1].first);
            Assert.Equal("Fourth", results[1].second);
        }

        [Fact]
        public void TooManyColumnsReadsFirstTwo()
        {
            // Given a reader
            var streamReader = $"First\tSecond\tThird\tFourth{Environment.NewLine}".ToStream();
            var reader = new FakeCSVReaderWriter(f => streamReader, f => null);

            // When I read
            reader.Open("Arbitrary", CSVReaderWriter.Mode.Read);
            List<(string first, string second)> results = new List<(string first, string second)>();
            string first;
            string second;
            while (reader.Read(out first, out second))
            {
                results.Add((first, second));
            }

            // Then the results are correct
            Assert.Equal(1, results.Count());
            Assert.Equal("First", results[0].first);
            Assert.Equal("Second", results[0].second);
        }
    }
}
