using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace Eze.IO.Application
{
    /// <summary>
    /// Class for handling uri schemes for applications.
    /// </summary>
    /// <remarks>
    /// A scheme is stored under <c>Software\Classes\{protocol}</c> in either HKEY_CURRENT_USER or
    /// HKEY_LOCAL_MACHINE, depending on the <see cref="RecordScheme"/> passed.
    /// </remarks>
    public static partial class UriScheme
    {
        private const string ObsoleteParameters = "Windows only passes the uri to the application as \"%1\", so a parameter count has no effect. Use an overload without it, or pass custom arguments as a string.";

        /// <summary>
        /// Register a new uri scheme
        /// </summary>
        /// <param name="uri">The <see cref="UriSchemeBuilder"/> to be used as a uri scheme</param>
        /// <param name="name">The display name of the application handler, stored as "URL:name Protocol"</param>
        /// <param name="register">The type of <see cref="RecordScheme"/></param>
        /// <exception cref="FormatException">Exception thrown when <see cref="UriSchemeBuilder.Protocol"/> has invalid character(s), or spaces</exception>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when an <see cref="IOException"/> occurs or another method is running simultaneously</exception>
        /// <exception cref="FileNotFoundException">Exception thrown when the <see cref="UriSchemeBuilder.Path"/> or <see cref="UriSchemeBuilder.Icon"/> file doesn't exist</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="uri"/> or <paramref name="name"/> parameter is null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        public static void RegisterScheme(UriSchemeBuilder uri,
            string name, RecordScheme register = RecordScheme.OnCurrentUser)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

#pragma warning disable CS0618 // Parameters is kept so builders from older code still register the same way
            CreateUri(uri.Protocol, uri.Path, name, uri.Icon.File, uri.Icon.Number, register,
                string.IsNullOrEmpty(uri.Parameter) ? null : uri.Parameter, uri.Parameters);
#pragma warning restore CS0618
        }

        /// <summary>
        /// Register new a uri scheme, which launches <c>"path" "%1"</c>
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
        /// <param name="protocol">The uri scheme, with or without a trailing "://"</param>
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
            CreateUri(protocol, path, name, icon, iconNumber, register, null, 1);
        }

        /// <summary>
        /// Register a new uri scheme
        /// </summary>
        /// <param name="protocol">The uri scheme, with or without a trailing "://"</param>
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
        [Obsolete(ObsoleteParameters)]
        public static void RegisterScheme(string protocol, string path,
            string name, int parameters, string icon = null, int iconNumber = 0, RecordScheme register = RecordScheme.OnCurrentUser)
        {
            CreateUri(protocol, path, name, icon, iconNumber, register, null, parameters);
        }

        /// <summary>
        /// Register a new uri scheme, which launches <c>"path" parameter</c>
        /// </summary>
        /// <param name="protocol">The uri scheme, with or without a trailing "://"</param>
        /// <param name="path">The file path to application.</param>
        /// <param name="name">The display name of the application handler, stored as "URL:name Protocol"</param>
        /// <param name="parameter">The custom arguments to pass, use "%1" where the uri should go</param>
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
            if (string.IsNullOrEmpty(parameter))
                throw new ArgumentNullException(nameof(parameter));

            CreateUri(protocol, path, name, icon, iconNumber, register, parameter, 1);
        }

        private static void CreateUri(string protocol, string path, string name, string icon, int iconNumber,
            RecordScheme register, string parameter, int parameters)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(string.Format("Couldn't locate file '{0}'",
                    path), path);

            if (string.IsNullOrEmpty(protocol))
                throw new ArgumentNullException(nameof(protocol));

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            try
            {
                StringBuilder args = new StringBuilder();
                if (parameter == null)
                {
                    for (int i = 1; i <= parameters; i++)
                    {
                        args.AppendFormat(@" ""%{0}""", i);
                    }
                }
                else
                {
                    args.Append(' ').Append(parameter);
                }

                protocol = FindSchemeText(protocol);

                if (!Uri.CheckSchemeName(protocol))
                {
                    throw new FormatException(string.Format(
                        "'{0}' is not a valid scheme", protocol));
                }

                // Validate everything before touching the registry so a failed call leaves nothing behind
                RegistryKey root = GetRoot(register);

                if (UriParser.IsKnownScheme(protocol))
                    throw new ApplicationException("This scheme is already registered");

                if (UriExist(protocol, register))
                    throw new ApplicationException("This scheme is already registered");

                if (!string.IsNullOrEmpty(icon))
                    if (!File.Exists(icon))
                        throw new FileNotFoundException(string.Format("Couldn't locate file '{0}'",
                            icon), icon);

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
            catch (ObjectDisposedException ex)
            {
                throw new InvalidOperationException("Error while writting to registry",
                     new InvalidOperationException(string.Format("A method in '{0}' class may have be running simultaneously", typeof(UriScheme).Name), ex));
            }
            catch (Exception ex) when (ex is SecurityException || ex is UnauthorizedAccessException)
            {
                throw PermissionError("write", register, ex);
            }
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

        private static UnauthorizedAccessException PermissionError(string action, RecordScheme scheme, Exception inner)
        {
            switch (scheme)
            {
                case RecordScheme.OnCurrentUser:
                    return new UnauthorizedAccessException(string.Format("Failed to {0} registry values on current user, check your permissions", action), inner);
                case RecordScheme.OnMachine:
                    return new UnauthorizedAccessException(string.Format("Failed to {0} registry values on the local machine, check your permissions", action), inner);
                default:
                    return new UnauthorizedAccessException(string.Format("Failed to {0} registry values, check your permissions", action), inner);
            }
        }

        private static void ReplaceUri(UriSchemeBuilder find, string name, RecordScheme register,
            Action<UriSchemeBuilder> update)
        {
            // Keep a copy of the current registration so it can be restored if registering the new one fails
            var old = GetUri(find.Protocol, register);

            DeleteUri(find.Protocol, name, register);
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

        /// <summary>
        /// Strips the "://", ":/" or ":" that may follow a scheme, e.g. "test://" becomes "test"
        /// </summary>
        private static String FindSchemeText(string protocol)
        {
            if (protocol == null)
                return null;

            return protocol.Trim().TrimEnd('/').TrimEnd(':').Trim();
        }

        private static UriSchemeBuilder GetUri(string protocol, RecordScheme register)
        {
            try
            {
                UriSchemeBuilder _result = new UriSchemeBuilder();

                using (RegistryKey key = GetRoot(register).OpenSubKey(GetClassesPath(protocol)))
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
                            ParseCommand(command, out string _path, out string _args);
                            _result.Path = _path;

                            // Arguments made only of "%1" "%2"... came from a parameter count, anything else is custom
                            if (Regex.IsMatch(_args, @"^(\s*""?%[0-9]+""?)*\s*$"))
                            {
#pragma warning disable CS0618 // read back so updating an older registration keeps its arguments
                                _result.Parameters = Regex.Matches(_args, @"%[0-9]+").Count;
#pragma warning restore CS0618
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
        /// Splits a shell command into the application path and its arguments,
        /// for both <c>"C:\path\app.exe" args</c> and <c>C:\path\app.exe args</c>
        /// </summary>
        internal static void ParseCommand(string command, out string path, out string args)
        {
            command = command.Trim();

            if (command.StartsWith("\""))
            {
                int end = command.IndexOf('"', 1);
                if (end < 0) end = command.Length;
                path = command.Substring(1, Math.Max(0, end - 1));
                args = end < command.Length ? command.Substring(end + 1).Trim() : string.Empty;
            }
            else
            {
                int space = command.IndexOf(' ');
                path = space < 0 ? command : command.Substring(0, space);
                args = space < 0 ? string.Empty : command.Substring(space + 1).Trim();
            }
        }

        /// <summary>
        /// Unregister a uri scheme
        /// </summary>
        /// <param name="uri">The <see cref="UriSchemeBuilder"/> to be used as a uri scheme</param>
        /// <param name="name">The name of application handler (not used for lookup, the scheme is found by its protocol)</param>
        /// <param name="register">The type of <see cref="RecordScheme"/></param>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when the scheme doesn't exist, or another method is running simultaneously</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="uri"/> or <paramref name="name"/> parameters is null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        public static void UnregisterScheme(UriSchemeBuilder uri, string name, RecordScheme register = RecordScheme.OnCurrentUser)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            DeleteUri(uri.Protocol, name, register);
        }

        /// <summary>
        /// Unregister a uri scheme
        /// </summary>
        /// <param name="protocol">The uri scheme, with or without a trailing "://"</param>
        /// <param name="name">The name of application handler (not used for lookup, the scheme is found by its protocol)</param>
        /// <param name="register">The type of <see cref="RecordScheme"/></param>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when the scheme doesn't exist, or another method is running simultaneously</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="protocol"/> or <paramref name="name"/> parameters is null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        public static void UnregisterScheme(string protocol, string name, RecordScheme register = RecordScheme.OnCurrentUser)
        {
            DeleteUri(protocol, name, register);
        }

        private static void DeleteUri(string protocol, string name, RecordScheme register)
        {
            if (string.IsNullOrEmpty(protocol))
                throw new ArgumentNullException(nameof(protocol));

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (!UriExist(protocol, register))
                throw new InvalidOperationException("The scheme or application name doesn't exist");

            try
            {
                GetRoot(register).DeleteSubKeyTree(GetClassesPath(protocol));
            }
            catch (ObjectDisposedException ex)
            {
                throw new InvalidOperationException("Error while deleting entries in the registry",
                        new InvalidOperationException(string.Format("A method in '{0}' class may have be running simultaneously", typeof(UriScheme).Name), ex));
            }
            catch (Exception ex) when (ex is SecurityException || ex is UnauthorizedAccessException)
            {
                throw PermissionError("delete", register, ex);
            }
        }

        /// <summary>
        /// Determines if uri scheme already exist
        /// </summary>
        /// <param name="protocol">The uri scheme, with or without a trailing "://"</param>
        /// <param name="register">The type of <see cref="RecordScheme"/></param>
        /// <returns>Returns true if the uri scheme is registered</returns>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when an <see cref="IOException"/> occurs</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="protocol"/> parameter is null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        public static Boolean Exists(string protocol, RecordScheme register = RecordScheme.OnCurrentUser)
        {
            return UriExist(protocol, register);
        }

        /// <summary>
        /// Determines if uri scheme already exist
        /// </summary>
        /// <param name="uri">The <see cref="UriSchemeBuilder"/> to be used as a uri scheme</param>
        /// <param name="name">The name of application handler (not used for lookup, the scheme is found by its protocol)</param>
        /// <param name="register">The type of <see cref="RecordScheme"/></param>
        /// <returns>Returns true if the uri scheme exist</returns>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when an <see cref="IOException"/> occurs</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="uri"/> or <paramref name="name"/> parameters is null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        [Obsolete("Use Exists(protocol, register) instead.")]
        public static Boolean DoesItExist(UriSchemeBuilder uri,
            string name, RecordScheme register = RecordScheme.OnCurrentUser)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            return UriExist(uri.Protocol, register);
        }

        /// <summary>
        /// Determines if uri scheme already exist
        /// </summary>
        /// <param name="protocol">The uri scheme</param>
        /// <param name="name">The name of application handler (not used for lookup, the scheme is found by its protocol)</param>
        /// <param name="register">The type of <see cref="RecordScheme"/></param>
        /// <returns>Returns true if the uri scheme exist</returns>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when an <see cref="IOException"/> occurs</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="name"/> or <paramref name="protocol"/> parameters is null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        [Obsolete("Use Exists(protocol, register) instead.")]
        public static Boolean DoesItExist(string protocol,
            string name, RecordScheme register = RecordScheme.OnCurrentUser)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            return UriExist(protocol, register);
        }

        private static Boolean UriExist(string protocol, RecordScheme register)
        {
            if (string.IsNullOrEmpty(protocol))
                throw new ArgumentNullException(nameof(protocol));

            try
            {
                // A key is a uri scheme only when it has the "URL Protocol" value, which also keeps
                // Unregister from deleting an unrelated class key (such as a file type) with the same name
                using (RegistryKey key = GetRoot(register).OpenSubKey(GetClassesPath(protocol)))
                {
                    return key != null && key.GetValue("URL Protocol") != null;
                }
            }
            catch (Exception ex) when (ex is SecurityException || ex is UnauthorizedAccessException)
            {
                throw PermissionError("read", register, ex);
            }
            catch (Exception ex) when (!(ex is InvalidEnumArgumentException))
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Update a existing uri scheme
        /// </summary>
        /// <param name="uri">The <see cref="UriSchemeBuilder"/> to be used as a uri scheme</param>
        /// <param name="name">The display name of the application handler, stored as "URL:name Protocol"</param>
        /// <param name="register">The type of <see cref="RecordScheme"/></param>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when the scheme doesn't exist, or another method is running simultaneously</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="uri"/> or <paramref name="name"/> parameters is null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        /// <exception cref="NullReferenceException">Exception thrown when method fails to find uri scheme</exception>
        public static void UpdateScheme(UriSchemeBuilder uri, string name, RecordScheme register = RecordScheme.OnCurrentUser)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

#pragma warning disable CS0618 // Parameters is kept so builders from older code still update the same way
            UpdateUri(uri.Protocol, uri.Path, name, uri.Icon.File, uri.Icon.Number, register,
                string.IsNullOrEmpty(uri.Parameter) ? null : uri.Parameter, uri.Parameters);
#pragma warning restore CS0618
        }

        /// <summary>
        /// Update a existing uri scheme
        /// </summary>
        /// <param name="protocol">The uri scheme, with or without a trailing "://"</param>
        /// <param name="path">The file path to application.</param>
        /// <param name="name">The display name of the application handler, stored as "URL:name Protocol"</param>
        /// <param name="parameter">The custom arguments to pass, use "%1" where the uri should go</param>
        /// <param name="icon">The icon associated with application, defaults to <paramref name="path"/></param>
        /// <param name="iconNumber">The icon number associated with <paramref name="icon"/></param>
        /// <param name="register">The type of <see cref="RecordScheme"/></param>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when the scheme doesn't exist, or another method is running simultaneously</exception>
        /// <exception cref="FileNotFoundException">Exception thrown when a <paramref name="path"/> or <paramref name="icon"/> file doesn't exist</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="protocol"/>, <paramref name="name"/> or <paramref name="parameter"/> parameters is null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        /// <exception cref="NullReferenceException">Exception thrown when method fails to find uri scheme</exception>
        public static void UpdateScheme(string protocol, string path, string name, string parameter,
            string icon = null, int iconNumber = 0, RecordScheme register = RecordScheme.OnCurrentUser)
        {
            UpdateUri(protocol, path, name, icon ?? path, iconNumber, register, parameter, 1);
        }

        /// <summary>
        /// Update a existing uri scheme
        /// </summary>
        /// <param name="protocol">The uri scheme, with or without a trailing "://"</param>
        /// <param name="path">The file path to application.</param>
        /// <param name="name">The display name of the application handler, stored as "URL:name Protocol"</param>
        /// <param name="parameters" > The number of arguments to pass</param>
        /// <param name="icon">The icon associated with application</param>
        /// <param name="iconNumber">The icon number associated with <paramref name="icon"/></param>
        /// <param name="register">The type of <see cref="RecordScheme"/></param>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when the scheme doesn't exist, or another method is running simultaneously</exception>
        /// <exception cref="FileNotFoundException">Exception thrown when a <paramref name="path"/> or <paramref name="icon"/> file doesn't exist</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="protocol"/> or <paramref name="name"/> parameters is null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        /// <exception cref="NullReferenceException">Exception thrown when method fails to find uri scheme</exception>
        [Obsolete(ObsoleteParameters)]
        public static void UpdateScheme(string protocol, string path, string name, int parameters, string icon = null,
            int iconNumber = 0, RecordScheme register = RecordScheme.OnCurrentUser)
        {
            UpdateUri(protocol, path, name, icon, iconNumber, register, null, parameters);
        }

        private static void UpdateUri(string protocol, string path, string name, string icon, int iconNumber,
            RecordScheme register, string parameter, int parameters)
        {
            if (string.IsNullOrEmpty(protocol))
                throw new ArgumentNullException(nameof(protocol));

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (!UriExist(protocol, register))
                throw new InvalidOperationException("The scheme or application name doesn't exist");

            var find = GetUri(protocol, register);
            if (find == null)
                throw new NullReferenceException("Error failed to find uri scheme");

            ReplaceUri(find, name, register, (u) =>
            {
                u.Icon = new Icon<string, int>(icon, iconNumber);
                u.Path = path;
                u.Parameter = parameter;
#pragma warning disable CS0618
                u.Parameters = parameters;
#pragma warning restore CS0618
                u.Protocol = protocol;
            });
        }
    }
}
