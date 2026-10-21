using Microsoft.Win32;
using System;
using System.IO;
using Xunit;

namespace Eze.IO.Application.Tests
{
    /// <summary>
    /// Registry tests, run against HKEY_CURRENT_USER with a unique scheme per test so they need no admin rights
    /// </summary>
    public sealed class UriSchemeTests : IDisposable
    {
        private const string Name = "Test App";

        private readonly string _scheme = "urischemetest" + Guid.NewGuid().ToString("N").Substring(0, 12);
        private readonly string _dir;
        private readonly string _app;
        private readonly string _otherApp;

        public UriSchemeTests()
        {
            // A folder with a space in it, to check the command is quoted
            _dir = Path.Combine(Path.GetTempPath(), "uri scheme " + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_dir);
            _app = Path.Combine(_dir, "app.exe");
            _otherApp = Path.Combine(_dir, "other app.exe");
            File.WriteAllText(_app, string.Empty);
            File.WriteAllText(_otherApp, string.Empty);
        }

        public void Dispose()
        {
            Registry.CurrentUser.DeleteSubKeyTree(KeyPath, false);
            Directory.Delete(_dir, true);
        }

        private string KeyPath => @"Software\Classes\" + _scheme;

        private object ReadValue(string subKey, string name)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(subKey == null ? KeyPath : KeyPath + "\\" + subKey))
                return key?.GetValue(name);
        }

        private string Command => (string)ReadValue(@"shell\open\command", null);

        [Fact]
        public void Register_WritesKeyNamedAfterProtocol()
        {
            UriScheme.RegisterScheme(_scheme + "://", _app, Name);

            Assert.Equal("URL:Test App Protocol", ReadValue(null, null));
            Assert.Equal(string.Empty, ReadValue(null, "URL Protocol"));
        }

        [Fact]
        public void Register_QuotesApplicationPath()
        {
            UriScheme.RegisterScheme(_scheme, _app, Name);

            Assert.Equal("\"" + _app + "\" \"%1\"", Command);
        }

        [Fact]
        public void Register_WithCustomParameter_UsesItAsArguments()
        {
            UriScheme.RegisterScheme(_scheme, _app, Name, parameter: "--open \"%1\"");

            Assert.Equal("\"" + _app + "\" --open \"%1\"", Command);
        }

        [Fact]
        public void Register_WritesIcon()
        {
            UriScheme.RegisterScheme(_scheme, _app, Name, _app, 2);

            Assert.Equal(_app + ",2", ReadValue("DefaultIcon", null));
        }

        [Theory]
        [InlineData("://")]
        [InlineData(":")]
        [InlineData("")]
        public void Register_AcceptsSchemeSuffixes(string suffix)
        {
            UriScheme.RegisterScheme(_scheme + suffix, _app, Name);

            Assert.True(UriScheme.Exists(_scheme));
        }

        [Fact]
        public void Register_Twice_Throws()
        {
            UriScheme.RegisterScheme(_scheme, _app, Name);

            Assert.Throws<ApplicationException>(() => UriScheme.RegisterScheme(_scheme, _app, Name));
        }

        [Fact]
        public void Register_KnownScheme_Throws()
        {
            Assert.Throws<ApplicationException>(() => UriScheme.RegisterScheme("http", _app, Name));
        }

        [Fact]
        public void Register_MissingIcon_ThrowsAndLeavesNoKey()
        {
            Assert.Throws<FileNotFoundException>(() =>
                UriScheme.RegisterScheme(_scheme, _app, Name, icon: Path.Combine(_dir, "missing.ico")));

            Assert.Null(Registry.CurrentUser.OpenSubKey(KeyPath));
        }

        [Fact]
        public void Register_InvalidScheme_Throws()
        {
            Assert.Throws<FormatException>(() => UriScheme.RegisterScheme("not valid", _app, Name));
        }

        [Fact]
        public void Exists_TracksRegisterAndUnregister()
        {
            Assert.False(UriScheme.Exists(_scheme));

            UriScheme.RegisterScheme(_scheme, _app, Name);
            Assert.True(UriScheme.Exists(_scheme));
            Assert.True(UriScheme.DoesItExist(_scheme, Name));

            UriScheme.UnregisterScheme(_scheme, Name);
            Assert.False(UriScheme.Exists(_scheme));
            Assert.Null(Registry.CurrentUser.OpenSubKey(KeyPath));
        }

        [Fact]
        public void Unregister_LeavesKeysThatAreNotSchemes()
        {
            // e.g. a file type class key that happens to share the name
            using (var key = Registry.CurrentUser.CreateSubKey(KeyPath))
                key.SetValue(string.Empty, "Some file type");

            Assert.False(UriScheme.Exists(_scheme));
            Assert.Throws<InvalidOperationException>(() => UriScheme.UnregisterScheme(_scheme, Name));
            Assert.NotNull(Registry.CurrentUser.OpenSubKey(KeyPath));
        }

        [Fact]
        public void Update_ChangesPathAndKeepsUriArgument()
        {
            UriScheme.RegisterScheme(_scheme, _app, Name);

            UriScheme.UpdateScheme(new UriSchemeBuilder { Protocol = _scheme, Path = _otherApp }, Name);

            Assert.Equal("\"" + _otherApp + "\" \"%1\"", Command);
        }

        [Fact]
        public void Update_WithCustomParameter()
        {
            UriScheme.RegisterScheme(_scheme, _app, Name);

            UriScheme.UpdateScheme(_scheme, _otherApp, Name, "-u \"%1\"");

            Assert.Equal("\"" + _otherApp + "\" -u \"%1\"", Command);
        }

        [Fact]
        public void Update_WhenNewRegistrationFails_RestoresOldOne()
        {
            UriScheme.RegisterScheme(_scheme, _app, Name);

            Assert.Throws<FileNotFoundException>(() =>
                UriScheme.UpdateScheme(_scheme, Path.Combine(_dir, "missing.exe"), Name, "\"%1\""));

            Assert.True(UriScheme.Exists(_scheme));
            Assert.Equal("\"" + _app + "\" \"%1\"", Command);
        }

        [Fact]
        public void Update_MissingScheme_Throws()
        {
            Assert.Throws<InvalidOperationException>(() =>
                UriScheme.UpdateScheme(_scheme, _app, Name, "\"%1\""));
        }

        [Fact]
        public void ObsoleteParameterCount_StillWritesPlaceholders()
        {
            UriScheme.RegisterScheme(_scheme, _app, Name, 2);

            Assert.Equal("\"" + _app + "\" \"%1\" \"%2\"", Command);
        }

        [Fact]
        public void Manager_CanBeCreatedAndSwallowsErrors()
        {
            var manager = new UriSchemeManager { ThrowError = false };

            manager.UnregisterScheme(_scheme, Name);
            Assert.False(manager.Exist(_scheme, Name));
        }
    }
}
