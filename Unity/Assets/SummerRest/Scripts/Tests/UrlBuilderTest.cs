using System.Collections.Generic;
using NUnit.Framework;
using SummerRest.Runtime.Parsers;

namespace SummerRest.Scripts.Tests
{
    public class UrlBuilderTest
    {
        private const string ExampleUrl = "summerrest.com";
        [Test]
        public void Test_Build_Param()
        {
            var table = new (string url, IEnumerable<KeyValuePair<string, ICollection<string>>> parameters, string result)[]
            {
                (
                    ExampleUrl,
                    new []
                    {
                        new KeyValuePair<string, ICollection<string>>("id", new[] {"1"})
                    },
                    $"{ExampleUrl}?id=1"),
                (
                    ExampleUrl,
                    new []
                    {
                        new KeyValuePair<string, ICollection<string>>("id", new[] {"1"}),
                        new KeyValuePair<string, ICollection<string>>("name", new[] {"link"}),
                        new KeyValuePair<string, ICollection<string>>("email", new[] {"link@gmail.com"}),
                    },
                    $"{ExampleUrl}?id=1&name=link&email=link@gmail.com"),
                (
                    ExampleUrl,
                    new []
                    {
                        new KeyValuePair<string, ICollection<string>>("id", new[] {"1", "2", "3"}),
                        new KeyValuePair<string, ICollection<string>>("name", new[] {"link"}),
                    },
                    $"{ExampleUrl}?id=1&id=2&id=3&name=link"),
                (
                    ExampleUrl,
                    new []
                    {
                        new KeyValuePair<string, ICollection<string>>("id", new[] {"1", "2", "3"}),
                        new KeyValuePair<string, ICollection<string>>("name", new[] {"link", "summer"}),
                        new KeyValuePair<string, ICollection<string>>("email", new[] {"link@gmail.com", "summer@gmail.com"}),
                    },
                    $"{ExampleUrl}?id=1&id=2&id=3&name=link&name=summer&email=link@gmail.com&email=summer@gmail.com")
            };
            var builder = IUrlBuilder.Current;
            foreach (var (url, parameters, expect) in table)
            {
                var result = builder.BuildUrl(url, parameters);
                Assert.AreEqual(expect, result);
            }
        }
    }
}