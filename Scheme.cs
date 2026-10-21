using Microsoft.Win32;
using System;
using System.Security;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace Eze.IO.Application
{
    /// <summary>
    /// Class for handling uri schemes for applications.
    /// </summary>
    public static partial class UriScheme
    {
        /// <summary>
        /// Register a new uri scheme
        /// </summary>
        /// <param name="uri">The <see cref="UriSchemeBuilder"/> to be used as a uri scheme</param>
        /// <param name="name">The display name of the application handler, stored as "URL:name Protocol"</param>
        /// <param name="register">The type of <see cref="RecordScheme"/></param>
        /// <exception cref="FormatException">Exception thrown when <see cref="UriSchemeBuilder.Protocol"/> has invalid character(s), or spaces</exception>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when an <see cref="IOException"/> occurs or another method is running simultaneously</exception>
        /// <exception cref="FileNotFoundException">Exception thrown when the <see cref="UriSchemeBuilder"/> <seealso cref="Path"/> property or the <see cref="UriSchemeBuilder"/> Icon property doesn't exist</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="name"/> parameter is null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        public static void RegisterScheme(UriSchemeBuilder uri,
            string name, RecordScheme register = RecordScheme.OnCurrentUser)
        {
            if (string.IsNullOrEmpty(uri.Parameter))
                RegisterScheme(uri.Protocol, uri.Path, name, uri.Parameters, uri.Icon.File, uri.Icon.Number, register);
            else
                RegisterScheme(uri.Protocol, uri.Path, name, uri.Parameter, uri.Icon.File, uri.Icon.Number, register);
        }

        /// <summary>
        /// Register new a uri scheme
        /// </summary>
        /// <example>
        /// Example on how to register uri scheme:
        /// <code>
        /// public class Test
        /// {
        ///     [STAThread]
        ///     static int Main(string[] args)
        ///     {
        ///         UriScheme.RegisterScheme("test://", "C:/Test/myapp.exe", "Test", "C:/Test/myapp.exe", 0, RecordScheme.OnMachine);
        ///         return 0;
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <param name="protocol">The uri scheme</param>
        /// <param name="path">The file path to application.</param>
        /// <param name="name">The display name of the application handler, stored as "URL:name Protocol"</param>
        /// <param name="icon">The icon associated with application</param>
        /// <param name="iconNumber">The icon number associated with <paramref name="icon"/></param>
        /// <param name="register">The type of <see cref="RecordScheme"/></param>
        /// <exception cref="FormatException">Exception thrown when <paramref name="protocol"/> has invalid character(s), or spaces</exception>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when an <see cref="IOException"/> occurs or another method is running simultaneously</exception>
        /// <exception cref="FileNotFoundException">Exception thrown when a <paramref name="path"/> or <paramref name="icon"/> file doesn't exist</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="protocol"/> or <paramref name="name"/> parameters is null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        public static void RegisterScheme(string protocol, string path,
            string name, string icon = null, int iconNumber = 0, RecordScheme register = RecordScheme.OnCurrentUser)
        {
            CreateUri(protocol, path, name, icon, iconNumber, register);
        }

        /// <summary>
        /// Register a new uri scheme
        /// </summary>
        /// <param name="protocol">The uri scheme</param>
        /// <param name="path">The file path to application.</param>
        /// <param name="name">The display name of the application handler, stored as "URL:name Protocol"</param>
        /// <param name="parameters">The number of arguments to pass</param>
        /// <param name="icon">The icon associated with application</param>
        /// <param name="iconNumber">The icon number associated with <paramref name="icon"/></param>
        /// <param name="register">The type of <see cref="RecordScheme"/></param>
        /// <exception cref="FormatException">Exception thrown when <paramref name="protocol"/> has invalid character(s), or spaces</exception>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when an <see cref="IOException"/> occurs or another method is running simultaneously</exception>
        /// <exception cref="FileNotFoundException">Exception thrown when a <paramref name="path"/> or <paramref name="icon"/> file doesn't exist</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="protocol"/> or <paramref name="name"/> parameters is null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        public static void RegisterScheme(string protocol, string path,
            string name, int parameters, string icon = null, int iconNumber = 0, RecordScheme register = RecordScheme.OnCurrentUser)
        {
            CreateUri(protocol, path, name, icon, iconNumber, register, parameters);
        }

        /// <summary>
        /// Register a new uri scheme
        /// </summary>
        /// <param name="protocol">The uri scheme</param>
        /// <param name="path">The file path to application.</param>
        /// <param name="name">The display name of the application handler, stored as "URL:name Protocol"</param>
        /// <param name="parameter">The custom arguments to pass</param>
        /// <param name="icon">The icon associated with application</param>
        /// <param name="iconNumber">The icon number associated with <paramref name="icon"/></param>
        /// <param name="register">The type of <see cref="RecordScheme"/></param>
        /// <exception cref="FormatException">Exception thrown when <paramref name="protocol"/> has invalid character(s), or spaces</exception>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when an <see cref="IOException"/> occurs or another method is running simultaneously</exception> 
        /// <exception cref="FileNotFoundException">Exception thrown when a <paramref name="path"/> or <paramref name="icon"/> file doesn't exist</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="protocol"/>, <paramref name="name"/> or <paramref name="parameter"/> parameters is null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        public static void RegisterScheme(string protocol, string path,
            string name, string parameter, string icon = null, int iconNumber = 0, RecordScheme register = RecordScheme.OnCurrentUser)
        {
            CreateUri(protocol, path, name, icon, iconNumber, register, parameter);
        }

        private static void CreateUri(string protocol, string path,
            string name, string icon = null, int iconNumber = 0, params object[] objects)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(string.Format("Couldn't locate file '{0}'",
                    path));

            if (string.IsNullOrEmpty(protocol))
                throw new ArgumentNullException(nameof(protocol));

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            using (UriSchemeBuilder build = new UriSchemeBuilder())
            {
                StringBuilder args = new StringBuilder();
                RecordScheme _scheme = RecordScheme.OnCurrentUser;
                String _custom = null;
                Int32 _argstoPass = 1;

                foreach (var o in objects)
                {
                    var t = o.GetType();

                    if (t == typeof(RecordScheme))
                    {
                        _scheme = (RecordScheme)o;
                    }
                    else if (t == typeof(Int32))
                    {
                        _argstoPass = (int)o;
                    }
                    else if (t == typeof(String))
                    {
                        _argstoPass = 1;
                        _custom = (string)o;
                        if (string.IsNullOrEmpty(_custom))
                            throw new ArgumentNullException(nameof(o));
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid parameter passed");
                    }
                }

                try
                {
                    if (_custom == null)
                    {
                        for (int i = 1; i <= _argstoPass; i++)
                        {
                            args.Append(string.Format(
                                @" ""%{0}""", i));
                        }
                    }
                    else
                    {
                        args.Append(string.Format(
                                " {0}", _custom));
                    }

                    protocol = FindSchemeText(protocol);

                    if (!Uri.CheckSchemeName(protocol))
                    {
                        throw new FormatException(string.Format(
                            "'{0}' is not a valid scheme", protocol));
                    }

                    build.Protocol = protocol;

                    // Validate everything before touching the registry so a failed call leaves nothing behind
                    RegistryKey root = GetRoot(_scheme);

                    if (UriParser.IsKnownScheme(build.Protocol))
                        throw new ApplicationException("This scheme is already registered");

                    if(DoesItExist(protocol, name, _scheme))
                        throw new ApplicationException("This scheme is already registered");

                    if (!string.IsNullOrEmpty(icon))
                        if (!File.Exists(icon))
                            throw new FileNotFoundException(string.Format("Couldn't locate file '{0}'",
                                icon));

                    // Windows resolves a uri scheme by the key name, so the key must be the protocol itself
                    string keyPath = GetClassesPath(protocol);
                    bool existed;
                    using (RegistryKey _existing = root.OpenSubKey(keyPath))
                        existed = _existing != null;

                    try
                    {
                        // CreateSubKey opens the key for writing, even when it already exists
                        using (RegistryKey key = root.CreateSubKey(keyPath))
                        {
                            key.SetValue(string.Empty, string.Format("URL:{0} Protocol", name));
                            key.SetValue("URL Protocol", string.Empty);
                            if (!string.IsNullOrEmpty(icon))
                            {
                                using (RegistryKey _key = key.CreateSubKey(@"DefaultIcon"))
                                {
                                    _key.SetValue(string.Empty, string.Format("{0},{1}",
                                        icon, iconNumber));
                                }
                            }
                            using (RegistryKey _key = key.CreateSubKey(@"shell\open\command"))
                            {
                                // Quote the path so paths containing spaces are launched correctly
                                _key.SetValue(string.Empty, string.Format(@"""{0}""{1}", path, args));
                            }
                        }
                    }
                    catch
                    {
                        // Only clean up a key this call created, never one that was already there
                        if (!existed)
                        {
                            try { root.DeleteSubKeyTree(keyPath, false); }
                            catch { /* ignore, report the original error */ }
                        }
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    if (ex is ObjectDisposedException)
                    {
                        throw new InvalidOperationException("Error while writting to registry",
                             new InvalidOperationException(string.Format("A method in '{0}' class may have be running simultaneously", typeof(UriScheme).Name), ex));
                    }
                    else if (ex is SecurityException || ex is UnauthorizedAccessException)
                    {
                        switch (_scheme)
                        {
                            case RecordScheme.OnCurrentUser:
                                throw new UnauthorizedAccessException("Failed to write registry values on current user, check your permissions");
                            case RecordScheme.OnMachine:
                                throw new UnauthorizedAccessException("Failed to write registry values on the local machine, check your permissions");
                        }

                        throw new UnauthorizedAccessException("Failed to write registry values, check your permissions");
                    }

                    throw;
                }
            };
        }

        private static RegistryKey GetRoot(RecordScheme scheme)
        {
            switch (scheme)
            {
                case RecordScheme.OnCurrentUser:
                    return Registry.CurrentUser;
                case RecordScheme.OnMachine:
                    return Registry.LocalMachine;
                default:
                    throw new InvalidEnumArgumentException("register", (int)scheme, typeof(RecordScheme));
            }
        }

        private static String GetClassesPath(string protocol)
        {
            return @"Software\Classes\" + FindSchemeText(protocol);
        }

        private static void ReplaceUri(UriSchemeBuilder find, string name, RecordScheme register,
            Action<UriSchemeBuilder> update)
        {
            // Keep a copy of the current registration so it can be restored if registering the new one fails
            var old = GetUri(find.Protocol, register);

            UnregisterScheme(find, name, register);
            update(find);

            try
            {
                RegisterScheme(find, name, register);
            }
            catch
            {
                if (old != null)
                {
                    try { RegisterScheme(old, name, register); }
                    catch { /* ignore, report the original error */ }
                }
                throw;
            }
        }

        private static String FindSchemeText(string protocol)
        {
            try
            {
                var _protocol = protocol;
                if (_protocol.Contains("://"))
                    _protocol = _protocol.Replace("://", string.Empty).Trim();
                return _protocol;
            }
            catch { return protocol; }
        }

        private static UriSchemeBuilder GetUri(string protocol, params object[] objects)
        {
            try
            {
                UriSchemeBuilder _result = new UriSchemeBuilder();
                RecordScheme _scheme = RecordScheme.OnCurrentUser;

                foreach (var o in objects)
                {
                    var t = o.GetType();

                    if (t == typeof(RecordScheme))
                    {
                        _scheme = (RecordScheme)o;
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid parameter passed");
                    }
                }

                using (RegistryKey key = GetRoot(_scheme).OpenSubKey(GetClassesPath(protocol)))
                {
                    if (key == null)
                        return null;

                    _result.Protocol = FindSchemeText(protocol);

                    using (RegistryKey _key = key.OpenSubKey(@"DefaultIcon"))
                    {
                        var icon = _key == null ? null : _key.GetValue(null) as string;

                        if (!string.IsNullOrEmpty(icon))
                        {
                            // The icon index follows the last comma, e.g. "C:\app.exe,0"
                            int n = 0;
                            string _file = icon;
                            int comma = icon.LastIndexOf(',');

                            if (comma >= 0 && Int32.TryParse(icon.Substring(comma + 1), out int num))
                            {
                                n = num;
                                _file = icon.Substring(0, comma);
                            }

                            _result.Icon = new Icon<string, int>(_file, n);
                        }
                    }

                    using (RegistryKey _key = key.OpenSubKey(@"shell\open\command"))
                    {
                        var command = _key == null ? null : _key.GetValue(null) as string;

                        if (!string.IsNullOrEmpty(command))
                        {
                            // The command is either "C:\path\app.exe" args or C:\path\app.exe args
                            command = command.Trim();
                            string _args;

                            if (command.StartsWith("\""))
                            {
                                int end = command.IndexOf('"', 1);
                                if (end < 0) end = command.Length;
                                _result.Path = command.Substring(1, Math.Max(0, end - 1));
                                _args = end < command.Length ? command.Substring(end + 1).Trim() : string.Empty;
                            }
                            else
                            {
                                int space = command.IndexOf(' ');
                                _result.Path = space < 0 ? command : command.Substring(0, space);
                                _args = space < 0 ? string.Empty : command.Substring(space + 1).Trim();
                            }

                            // Arguments made only of "%1" "%2"... came from a parameter count, anything else is custom
                            var placeholders = Regex.Matches(_args, @"""?%[0-9]+""?");
                            if (Regex.IsMatch(_args, @"^(\s*""?%[0-9]+""?)*\s*$"))
                            {
                                _result.Parameters = placeholders.Count;
                            }
                            else
                            {
                                _result.Parameter = _args;
                            }
                        }
                    }
                }

                return _result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Unregister a uri scheme
        /// </summary>
        /// <param name="uri">The <see cref="UriSchemeBuilder"/> to be used as a uri scheme</param>
        /// <param name="name">The name of application handler (not used for lookup, the scheme is found by its protocol)</param>
        /// <param name="register">The type of <see cref="RecordScheme"/></param>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when invalid object is passed, scheme doesn't exist, or another method is running simultaneously</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="name"/> parameters is null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        public static void UnregisterScheme(UriSchemeBuilder uri, string name, RecordScheme register = RecordScheme.OnCurrentUser)
        {
            DeleteUri(uri.Protocol, name, register);
        }

        /// <summary>
        /// Unregister a uri scheme
        /// </summary>
        /// <param name="protocol">The uri scheme</param>
        /// <param name="name">The name of application handler (not used for lookup, the scheme is found by its protocol)</param>
        /// <param name="register">The type of <see cref="RecordScheme"/></param>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when invalid object is passed, scheme doesn't exist, or another method is running simultaneously</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="protocol"/> or <paramref name="name"/> parameters is null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        public static void UnregisterScheme(string protocol, string name, RecordScheme register = RecordScheme.OnCurrentUser)
        {
            DeleteUri(protocol, name, register);
        }

        private static void DeleteUri(string protocol,
            string name, params object[] objects)
        {
            if (string.IsNullOrEmpty(protocol))
                throw new ArgumentNullException(nameof(protocol));

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            RecordScheme _scheme = RecordScheme.OnCurrentUser;

            foreach (var o in objects)
            {
                var t = o.GetType();

                if (t == typeof(RecordScheme))
                {
                    _scheme = (RecordScheme)o;
                }
                else
                {
                    throw new InvalidOperationException("Invalid parameter passed");
                }
            }

            if(!DoesItExist(protocol, name, _scheme))
                throw new InvalidOperationException("The scheme or application name doesn't exist");

            try
            {
                GetRoot(_scheme).DeleteSubKeyTree(GetClassesPath(protocol));
            }
            catch (Exception ex)
            {
                if (ex is SecurityException || ex is UnauthorizedAccessException)
                {
                    switch (_scheme)
                    {
                        case RecordScheme.OnCurrentUser:
                            throw new UnauthorizedAccessException("Failed to delete registry values on current user, check your permissions");
                        case RecordScheme.OnMachine:
                            throw new UnauthorizedAccessException("Failed to delete registry values on the local machine, check your permissions");
                    }

                    throw new UnauthorizedAccessException("Failed to delete registry values, check your permissions");
                }
                else if (ex is ArgumentNullException)
                {
                    throw;
                }
                else if (ex is InvalidEnumArgumentException)
                {
                    throw;
                }
                else if (ex is ObjectDisposedException)
                {
                    throw new InvalidOperationException("Error while deleting entries in the registry",
                            new InvalidOperationException(string.Format("A method in '{0}' class may have be running simultaneously", typeof(UriScheme).Name), ex));
                }

                throw;
            }
        }

        /// <summary>
        /// Determines if uri scheme already exist
        /// </summary>
        /// <param name="uri">The <see cref="UriSchemeBuilder"/> to be used as a uri scheme</param>
        /// <param name="name">The name of application handler (not used for lookup, the scheme is found by its protocol)</param>
        /// <param name="register">The type of <see cref="RecordScheme"/></param>
        /// <returns>Returns true if the uri scheme exist</returns>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when an <see cref="IOException"/></exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="name"/> parameters is null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        public static Boolean DoesItExist(UriSchemeBuilder uri,
            string name, RecordScheme register = RecordScheme.OnCurrentUser)
        {
            return UriExist(uri.Protocol, name, register);
        }

        /// <summary>
        /// Determines if uri scheme already exist
        /// </summary>
        /// <param name="protocol">The uri scheme</param>
        /// <param name="name">The name of application handler (not used for lookup, the scheme is found by its protocol)</param>
        /// <param name="register">The type of <see cref="RecordScheme"/></param>
        /// <returns>Returns true if the uri scheme exist</returns>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when an <see cref="IOException"/></exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="name"/> or <paramref name="protocol"/> parameters is null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        public static Boolean DoesItExist(string protocol,
            string name, RecordScheme register = RecordScheme.OnCurrentUser)
        {
            return UriExist(protocol, name, register);
        }

        private static Boolean UriExist(string protocol,
            string name, params object[] objects)
        {
            RecordScheme _scheme = RecordScheme.OnCurrentUser;

            foreach (var o in objects)
            {
                var t = o.GetType();

                if (t == typeof(RecordScheme))
                {
                    _scheme = (RecordScheme)o;
                }
            }

            try
            {
                if (string.IsNullOrEmpty(protocol))
                    throw new ArgumentNullException(nameof(protocol));

                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException(nameof(name));

                // A key is a uri scheme only when it has the "URL Protocol" value, which also keeps
                // Unregister from deleting an unrelated class key (such as a file type) with the same name
                using (RegistryKey key = GetRoot(_scheme).OpenSubKey(GetClassesPath(protocol)))
                {
                    return key != null && key.GetValue("URL Protocol") != null;
                }
            }
            catch (Exception ex)
            {
                if (ex is SecurityException || ex is UnauthorizedAccessException)
                {
                    switch (_scheme)
                    {
                        case RecordScheme.OnCurrentUser:
                            throw new UnauthorizedAccessException("Failed to read registry values on current user, check your permissions");
                        case RecordScheme.OnMachine:
                            throw new UnauthorizedAccessException("Failed to read registry values on the local machine, check your permissions");
                    }

                    throw new UnauthorizedAccessException("Failed to read registry values, check your permissions");
                }
                else if(ex is ArgumentNullException)
                {
                    throw;
                }
                else if (ex is InvalidEnumArgumentException)
                {
                    throw;
                }
                else
                {
                    throw new InvalidOperationException(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Update a existing uri scheme
        /// </summary>
        /// <param name="uri">The <see cref="UriSchemeBuilder"/> to be used as a uri scheme</param>
        /// <param name="name">The display name of the application handler, stored as "URL:name Protocol"</param>
        /// <param name="register">The type of <see cref="RecordScheme"/></param>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when invalid object is passed, scheme doesn't exist, or another method is running simultaneously</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="name"/> parameters is null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        /// <exception cref="NullReferenceException">Exception thrown when method fails to find uri scheme</exception>
        public static void UpdateScheme(UriSchemeBuilder uri, string name, RecordScheme register = RecordScheme.OnCurrentUser)
        {
            if (DoesItExist(uri, name, register))
            {
                if(String.IsNullOrEmpty(uri.Parameter))
                    UpdateScheme(uri.Protocol, uri.Path, name, uri.Parameters, uri.Icon.File, uri.Icon.Number, register);
                else
                    UpdateScheme(uri.Protocol, uri.Path, name, uri.Parameter, uri.Icon.File, uri.Icon.Number, register);
            }
            else
            {
                throw new InvalidOperationException("The scheme or application name doesn't exist");
            }
        }


        /// <summary>
        /// Update a existing uri scheme
        /// </summary>
        /// <param name="protocol">The uri scheme</param>
        /// <param name="path">The file path to application.</param>
        /// <param name="name">The display name of the application handler, stored as "URL:name Protocol"</param>
        /// <param name="parameter">The custom arguments to pass</param>
        /// <param name="icon">The icon associated with application</param>
        /// <param name="iconNumber">The icon number associated with <paramref name="icon"/></param>
        /// <param name="register">The type of <see cref="RecordScheme"/></param>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when invalid object is passed, scheme doesn't exist, or another method is running simultaneously</exception>
        /// <exception cref="FileNotFoundException">Exception thrown when a <paramref name="path"/> or <paramref name="icon"/> file doesn't exist</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="protocol"/>, <paramref name="name"/> or <paramref name="parameter"/> parameters is null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        /// <exception cref="NullReferenceException">Exception thrown when method fails to find uri scheme</exception>
        public static void UpdateScheme(string protocol, string path, string name, string parameter,
            string icon = null, int iconNumber = 0, RecordScheme register = RecordScheme.OnCurrentUser)
        {
            if (DoesItExist(protocol, name, register))
            {
                var find = GetUri(protocol, register);
                if (find != null)
                {
                    if (icon == null) { icon = path; }
                    ReplaceUri(find, name, register, (u) =>
                    {
                        u.Icon = new Icon<string, int>(icon, iconNumber);
                        u.Path = path;
                        u.Parameter = parameter;
                        u.Protocol = protocol;
                    });
                }
                else
                {
                    throw new NullReferenceException("Error failed to find uri scheme");
                }
            }
            else
            {
                throw new InvalidOperationException("The scheme or application name doesn't exist");
            }
        }

        /// <summary>
        /// Update a existing uri scheme
        /// </summary>
        /// <param name="protocol">The uri scheme</param>
        /// <param name="path">The file path to application.</param>
        /// <param name="name">The display name of the application handler, stored as "URL:name Protocol"</param>
        /// <param name="parameters" > The number of arguments to pass</param>
        /// <param name="icon">The icon associated with application</param>
        /// <param name="iconNumber">The icon number associated with <paramref name="icon"/></param>
        /// <param name="register">The type of <see cref="RecordScheme"/></param>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when invalid object is passed, scheme doesn't exist, or another method is running simultaneously</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="protocol"/> or <paramref name="name"/> parameters is null</exception>
        /// <exception cref="FileNotFoundException">Exception thrown when a <paramref name="path"/> or <paramref name="icon"/> file doesn't exist</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="protocol"/> or <paramref name="name"/> parameters is null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        /// <exception cref="NullReferenceException">Exception thrown when method fails to find uri scheme</exception>
        public static void UpdateScheme(string protocol, string path, string name, int parameters, string icon = null,
            int iconNumber = 0, RecordScheme register = RecordScheme.OnCurrentUser)
        {
            if (DoesItExist(protocol, name, register))
            {
                var find = GetUri(protocol, register);
                if (find != null)
                {
                    ReplaceUri(find, name, register, (u) =>
                    {
                        u.Icon = new Icon<string, int>(icon, iconNumber);
                        u.Path = path;
                        u.Parameter = null;
                        u.Parameters = parameters;
                        u.Protocol = protocol;
                    });
                }
                else
                {
                    throw new NullReferenceException("Error failed to find uri scheme");
                }
            }
            else
            {
                throw new InvalidOperationException("The scheme or application name doesn't exist");
            }
        }
    }
}