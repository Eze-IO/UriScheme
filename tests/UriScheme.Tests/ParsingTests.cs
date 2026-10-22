using System;
using Xunit;

namespace Eze.IO.Application.Tests
{
    public class ParsingTests
    {
        [Theory]
        [InlineData("\"C:\\Program Files\\App\\app.exe\" \"%1\"", "C:\\Program Files\\App\\app.exe", "\"%1\"")]
        [InlineData("C:\\App\\app.exe \"%1\"", "C:\\App\\app.exe", "\"%1\"")]
        [InlineData("C:\\App\\app.exe", "C:\\App\\app.exe", "")]
        [InlineData("  \"C:\\App\\app.exe\"  ", "C:\\App\\app.exe", "")]
        public void ParseCommand_SplitsPathAndArguments(string command, string path, string args)
        {
            UriScheme.ParseCommand(command, out string actualPath, out string actualArgs);

            Assert.Equal(path, actualPath);
            Assert.Equal(args, actualArgs);
        }

        [Fact]
        public void GetUriBuilders_SkipsArgumentsThatAreNotUris()
        {
            var builders = UriSchemeParser.GetUriBuilders(new[] { "myapp://open?id=1", "--verbose", "", "other://x" });

            Assert.Equal(2, builders.Length);
            Assert.Equal("myapp", builders[0].Scheme);
            Assert.Equal("other", builders[1].Scheme);
        }

        [Fact]
        public void GetUriBuilders_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => UriSchemeParser.GetUriBuilders(null));
        }

        [Fact]
        public void GetUriBuilder_ReturnsNullForNonUri()
        {
            Assert.Null(UriSchemeParser.GetUriBuilder("--verbose"));
            Assert.Equal("myapp", UriSchemeParser.GetUriBuilder("myapp://x").Scheme);
        }

        [Fact]
        public void Builder_DefaultsToPassingTheUri()
        {
            var builder = new UriSchemeBuilder();

            Assert.Equal(1, builder.Parameters);
            builder.Dispose();
            builder.Dispose();
        }
    }
}
