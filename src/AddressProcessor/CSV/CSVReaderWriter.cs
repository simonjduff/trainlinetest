﻿using System;
using System.IO;
using System.Linq;
using System.Text;

namespace AddressProcessing.CSV
{
    /*
        2) Refactor this class into clean, elegant, rock-solid & well performing code, without over-engineering.
           Assume this code is in production and backwards compatibility must be maintained.
    */

    public class CSVReaderWriter : IDisposable
    {
        private StreamReader _readerStream = null;
        private TextWriter _writerStream = null;

        protected virtual Func<string,StreamReader> BuildStreamReader
            => filename => File.OpenText(filename);
        protected virtual Func<string,TextWriter> BuildTextWriter
            => filename => {
                FileInfo fileInfo = new FileInfo(filename);
                return fileInfo.CreateText();
            };

        // Leaving [Flags] in only for backwards compatibility#
        // until we can be certain this change won't cause
        // compilation issues for a caller. This is very unlikely however
        // as the equality (==) checks would make multiple flags meaningless.
        [Flags]
        public enum Mode { Read = 1, Write = 2};
        private Mode? _mode = null;

        public void Open(string fileName, Mode mode)
        {
            switch (mode)
            {
                case Mode.Read:
                    _readerStream = BuildStreamReader(fileName);
                    break;
                case Mode.Write:
                    _writerStream = BuildTextWriter(fileName);
                    break;
                // default should only be possible if mode is Read | Write
                default: throw new Exception($"Unknown file mode for {fileName}");
            }

            _mode = mode;
        }

        public void Write(params string[] columns)
        {
            if (_mode != Mode.Write)
            {
                throw new Exception("Not in write mode. Cannot write to file.");
            }

            if (columns == null)
            {
                return;
            }

            var line = string.Join("\t", columns);

            /*
             * Tt's unclear whether handling an exception here would be appropriate.
             */
            _writerStream.WriteLine(line);
        }

        public bool Read(string column1, string column2)
        {
            if (_mode != Mode.Read)
            {
                throw new Exception("Not in read mode. Cannot read from file");
            }

            // This can't output column1, column2 as they're not ref/out.

            string dummy = string.Empty;
            string dummy2 = string.Empty;

            // For backwards compatibility, we'll maintain the bool return
            // though this is unlikely to be meaningful.
            return Read(out dummy, out dummy2);
            
        }

        public bool Read(out string column1, out string column2)
        {
            const int FIRST_COLUMN = 0;
            const int SECOND_COLUMN = 1;

            column1 = null;
            column2 = null;

            char[] separator = { '\t' };

            try
            {
                var line = _readerStream.ReadLine();

                string[] columns = line?.Split('\t');

                // Changing Length check to 2 should prevent IndexOutOfRangeException
                if (columns == null || columns.Length < 2)
                {
                    return false;
                } 
                else
                {
                    column1 = columns[FIRST_COLUMN];
                    column2 = columns[SECOND_COLUMN];

                    return true;
                }   
            }
            catch
            {
                // This situation should be logged and reported.
                // This will prevent a hard crash and return as if
                // there was no data.

                /* Re-setting the nulls here as
                * column1 = columns[0] could pass and then
                * exception thrown on column2 = columns[1]
                * which would be unexpected behaviour for this class.
                */
                column1 = null;
                column2 = null;
                return false;
            }
        }

        /* Without freedom to change AddressFileProcessor or other callers
         * we cannot rely on IDisposable and assume the caller will dispose.
         * So we maintain the Close() interface
         */
        public void Close()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            _writerStream?.Close();
            _readerStream?.Close();
        }
    }
}
