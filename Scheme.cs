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
        /// <param name="name">The name of application handler</param>
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
        ///         UriScheme.Register("test://", "C:/Test/myapp.exe", "Test", "C:/Test/myapp.exe", 0, RecordScheme.OnMachine);
        ///         return 0;
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <param name="protocol">The uri scheme</param>
        /// <param name="path">The file path to application.</param>
        /// <param name="name">The name of application handler</param>
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
        /// <param name="name">The name of application handler</param>
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
        /// <param name="name">The name of application handler</param>
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
                RegistryKey key = Registry.LocalMachine;
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

                    switch (_scheme)
                    {
                        case RecordScheme.OnCurrentUser:
                            key = Registry.CurrentUser.OpenSubKey(@"Software\Classes\" + name);
                            break;
                        case RecordScheme.OnMachine:
                            key = Registry.LocalMachine.OpenSubKey(@"Software\Classes\" + name);
                            break;
                        default:
                            throw new InvalidEnumArgumentException("register", (int)_scheme, typeof(RecordScheme));
                    }

                    if (key == null)
                    {
                        switch (_scheme)
                        {
                            case RecordScheme.OnCurrentUser:
                                key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\" + name);
                                break;
                            case RecordScheme.OnMachine:
                                key = Registry.LocalMachine.CreateSubKey(@"Software\Classes\" + name);
                                break;
                            default:
                                throw new InvalidEnumArgumentException("register", (int)_scheme, typeof(RecordScheme));
                        }
                    }

                    if (UriParser.IsKnownScheme(build.Protocol))
                        throw new ApplicationException("This scheme is already registered");

                    if(DoesItExist(protocol, name, _scheme))
                        throw new ApplicationException("This scheme is already registered");

                    if (!string.IsNullOrEmpty(icon))
                        if (!File.Exists(icon))
                            throw new FileNotFoundException(string.Format("Couldn't locate file '{0}'",
                                icon));

                    key.SetValue(string.Empty, string.Format("URL:{0} Protocol", build.Protocol));
                    key.SetValue("URL Protocol", string.Empty);
                    if (!string.IsNullOrEmpty(icon))
                    {
                        var _key = key.CreateSubKey(@"DefaultIcon");
                        _key.SetValue(string.Empty, string.Format("{0},{1}",
                            icon, iconNumber));
                    }
                    key = key.CreateSubKey(@"shell\open\command");
                    key.SetValue(string.Empty, path + args.ToString());
                    key.Close();
                    key.Dispose();
                }
                catch (Exception ex)
                {
                    if (ex is ObjectDisposedException)
                    {
                        throw new InvalidOperationException("Error while writting to registry",
                             new InvalidOperationException(string.Format("A method in '{0}' class may have be running simultaneously", typeof(UriScheme).Name, ex)));
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

                    throw ex;
                }
            };
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

        private static UriSchemeBuilder GetUri(string name, params object[] objects)
        {
            try
            {
                UriSchemeBuilder _result = new UriSchemeBuilder();
                RegistryKey key = Registry.LocalMachine;
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

                switch (_scheme)
                {
                    case RecordScheme.OnCurrentUser:
                        key = Registry.CurrentUser.OpenSubKey(@"Software\Classes\" + name);
                        break;
                    case RecordScheme.OnMachine:
                        key = Registry.LocalMachine.OpenSubKey(@"Software\Classes\" + name);
                        break;
                    default:
                        throw new InvalidEnumArgumentException("register", (int)_scheme, typeof(RecordScheme));
                }

                if(key == null)
                {
                    key.Dispose();
                    throw new SecurityException();
                }

                var _protocol = (string)key.GetValue(null);

                if (_protocol != null)
                {
                    _result.Protocol = _protocol.Replace("URL:", string.Empty).Replace("Protocol", string.Empty).Trim();
                }

                var _key = key.OpenSubKey(@"DefaultIcon");

                if (_key != null)
                {
                    int n = 0;
                    string _file = string.Empty;
                    var icon = (string)_key.GetValue(null);

                    if (icon != null)
                    {
                        foreach (var i in icon.Split(','))
                        {
                            if (Int32.TryParse(i, out int num))
                            {
                                n = num;
                            }
                            else
                            {
                                _file = i;
                            }
                        }
                    }

                    _result.Icon = new Icon<string, int>(_file, n);
                }
                key = key.OpenSubKey(@"shell\open\command");

                if (key == null)
                {
                    var _query = 0;
                    var _key2 = (string)key.GetValue(null);
                    var _app = Regex.Match(_key2, @"^(?:[a-zA-Z]\:|\\\\[\w\.]+\\[\w.$]+)\\(?:[\w]+\\)*\w([\w.])+$", RegexOptions.None);

                    if (_app.Success)
                    {
                        if (!string.IsNullOrEmpty(_app.Value))
                            _result.Path = _app.Value;

                        var q = Regex.Match(_key2.Replace(_app.Value, string.Empty).Trim(), "^%[0-9]+$");

                        if (q.Success)
                        {
                            _query = q.Groups.Count;
                        }
                        else
                        {
                            _result.Parameter = q.Value;
                        }
                    }

                    _result.Parameters = _query;
                }

                key.Dispose();

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
        /// <param name="name">The name of application handler</param>
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
        /// <param name="name">The name of application handler</param>
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
                RegistryKey key = Registry.CurrentConfig;
                switch (_scheme)
                {
                    case RecordScheme.OnCurrentUser:
                        Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\" + name);
                        break;
                    case RecordScheme.OnMachine:
                        Registry.LocalMachine.DeleteSubKeyTree(@"Software\Classes\" + name);
                        break;
                    default:
                        throw new InvalidEnumArgumentException("register", (int)_scheme, typeof(RecordScheme));
                }
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
                    throw ex;
                }
                else if (ex is InvalidEnumArgumentException)
                {
                    throw ex;
                }
                else if (ex is ObjectDisposedException)
                {
                    throw new InvalidOperationException("Error while deleting entries in the registry",
                            new InvalidOperationException(string.Format("A method in '{0}' class may have be running simultaneously", typeof(UriScheme).Name, ex)));
                }

                throw ex;
            }
        }

        /// <summary>
        /// Determines if uri scheme already exist
        /// </summary>
        /// <param name="uri">The <see cref="UriSchemeBuilder"/> to be used as a uri scheme</param>
        /// <param name="name">The name of application handler</param>
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
        /// <param name="name">The name of application handler</param>
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

                RegistryKey key = Registry.CurrentUser;

                switch (_scheme)
                {
                    case RecordScheme.OnCurrentUser:
                        key = Registry.CurrentUser.OpenSubKey(@"Software\Classes\" + name);
                        break;
                    case RecordScheme.OnMachine:
                        key = Registry.LocalMachine.OpenSubKey(@"Software\Classes\" + name);
                        break;
                    default:
                        throw new InvalidEnumArgumentException("register", (int)_scheme, typeof(RecordScheme));
                }

                try
                {
                    if(UriParser.IsKnownScheme(protocol))
                        return true;
                }
                catch { /* ignore */}
                finally { }

                protocol = FindSchemeText(protocol);

                if (key != null)
                {
                    string _key = (string)key.GetValue(null);

                    if(_key == null)
                    {
                        key.Dispose();
                        return false;
                    }
                    else
                    {
                        var _r = _key.Replace("URL:", string.Empty).Replace("Protocol", string.Empty).Trim();
                        if (_r == protocol)
                        {
                            return true;
                        }

                        return false;
                    }
                }
                else
                {
                    return false;
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
                    throw ex;
                }
                else if (ex is InvalidEnumArgumentException)
                {
                    throw ex;
                }
                else
                {
                    throw new InvalidOperationException(ex.Message);
                }
            }
        }

        /// <summary>
        /// Update a existing uri scheme
        /// </summary>
        /// <param name="uri">The <see cref="UriSchemeBuilder"/> to be used as a uri scheme</param>
        /// <param name="name">The name of application handler</param>
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
                    UpdateScheme(uri.Protocol, uri.Path, name, uri.Path, uri.Icon.File, uri.Icon.Number, register);
                else
                    UpdateScheme(uri.Protocol, uri.Path, name, uri.Path, uri.Icon.File, uri.Icon.Number, register);
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
        /// <param name="name">The name of application handler</param>
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
                var find = GetUri(name, register);
                if (find != null)
                {
                    if (icon == null) { icon = path; }
                    UnregisterScheme(find, name, register);
                    find.Icon = new Icon<string, int>(icon, iconNumber);
                    find.Path = path;
                    find.Parameter = parameter;
                    find.Protocol = protocol;
                    RegisterScheme(find, name, register);
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
        /// <param name="name">The name of application handler</param>
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
                var find = GetUri(name, register);
                if (find != null)
                {
                    UnregisterScheme(find, name, register);
                    find.Icon = new Icon<string, int>(icon, iconNumber);
                    find.Path = path;
                    find.Parameters = parameters;
                    find.Protocol = protocol;
                    RegisterScheme(find, name, register);
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