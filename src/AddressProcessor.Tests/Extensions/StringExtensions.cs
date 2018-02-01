using System.IO;
using System.Text;

namespace AddressProcessor.Tests.Extensions
{
    public static class StringExtensions
    {
        public static StreamReader ToStream(this string input)
        {
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(input));
            return new StreamReader(memoryStream);
        }
    }
}