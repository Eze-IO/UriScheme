using Microsoft.Win32;
using System;
using System.Security;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Windows.Forms;

namespace Eze.IO.Application
{
    /// <summary>
    /// Class for handling uri schemes for applications.
    /// </summary>
    public static partial class UriScheme
    {
        /// <summary>
        /// Register a uri scheme
        /// </summary>
        /// <param name="uri">The <see cref="CustomUriBuilder"/> to be used as a uri scheme</param>
        /// <param name="name">The name of application handler</param>
        /// <param name="register">The type of <see cref="RegisterScheme"/></param>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when an <see cref="IOException"/> occurs or another method is running simultaneously</exception>
        /// <exception cref="FileNotFoundException">Exception thrown when the <see cref="CustomUriBuilder"/> <seealso cref="Path"/> property or the <see cref="CustomUriBuilder"/> Icon property doesn't exist</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="name"/> parameter is null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        public static void Register(CustomUriBuilder uri,
            string name, RegisterScheme register = RegisterScheme.OnCurrentUser)
        {
            if(string.IsNullOrEmpty(uri.Query))
                Register(uri.Scheme, uri.Path, uri.Port, name, uri.Queries, uri.Icon.File, uri.Icon.Number, register);
            else
                Register(uri.Scheme, uri.Path, uri.Port, name, uri.Query, uri.Icon.File, uri.Icon.Number, register);
        }

        /// <summary>
        /// Register a uri scheme
        /// <para>throws <see cref="ApplicationException"/> if uri scheme already exist</para>
        /// </summary>
        /// <param name="protocol">The uri scheme</param>
        /// <param name="app">The file path to application.</param>
        /// <param name="port">The port number associated with uri scheme</param>
        /// <param name="name">The name of application handler</param>
        /// <param name="icon">The icon associated with application</param>
        /// <param name="iconNumber">The icon number associated with <paramref name="icon"/></param>
        /// <param name="register">The type of <see cref="RegisterScheme"/></param>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when an <see cref="IOException"/> occurs or another method is running simultaneously</exception>
        /// <exception cref="FileNotFoundException">Exception thrown when a <paramref name="app"/> or <paramref name="icon"/> file doesn't exist</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="protocol"/> or <paramref name="name"/> parameters are null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        /// <example>
        /// Example on how to register uri scheme:
        /// <code>
        /// public class Test
        /// {
        ///     [STAThread]
        ///     static int Main(string[] args)
        ///     {
        ///         UriScheme.Register("test://", "C:/Test/myapp.exe", 3305, "Test", "C:/Test/myapp.exe", 0, RegisterScheme.OnMachine);
        ///     }
        /// }
        /// </code>
        /// </example>
        public static void Register(string protocol, string app, int port,
            string name, string icon = null, int iconNumber = 0, RegisterScheme register = RegisterScheme.OnCurrentUser)
        {
            CreateUri(protocol, app, port, name, icon, iconNumber, register);
        }

        /// <summary>
        /// Register a uri scheme
        /// </summary>
        /// <param name="protocol">The uri scheme</param>
        /// <param name="app">The file path to application.</param>
        /// <param name="port">The port number associated with uri scheme</param>
        /// <param name="name">The name of application handler</param>
        /// <param name="queries">The number of arguments to pass</param>
        /// <param name="icon">The icon associated with application</param>
        /// <param name="iconNumber">The icon number associated with <paramref name="icon"/></param>
        /// <param name="register">The type of <see cref="RegisterScheme"/></param>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when an <see cref="IOException"/> occurs or another method is running simultaneously</exception>
        /// <exception cref="FileNotFoundException">Exception thrown when a <paramref name="app"/> or <paramref name="icon"/> file doesn't exist</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="protocol"/> or <paramref name="name"/> parameters are null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        public static void Register(string protocol, string app, int port,
            string name, int queries, string icon = null, int iconNumber = 0, RegisterScheme register = RegisterScheme.OnCurrentUser)
        {
            CreateUri(protocol, app, port, name, icon, iconNumber, register, queries);
        }

        /// <summary>
        /// Register a uri scheme
        /// </summary>
        /// <param name="protocol">The uri scheme</param>
        /// <param name="app">The file path to application.</param>
        /// <param name="port">The port number associated with uri scheme</param>
        /// <param name="name">The name of application handler</param>
        /// <param name="customQueries">The custom arguments to pass</param>
        /// <param name="icon">The icon associated with application</param>
        /// <param name="iconNumber">The icon number associated with <paramref name="icon"/></param>
        /// <param name="register">The type of <see cref="RegisterScheme"/></param>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when an <see cref="IOException"/> occurs or another method is running simultaneously</exception> 
        /// <exception cref="FileNotFoundException">Exception thrown when a <paramref name="app"/> or <paramref name="icon"/> file doesn't exist</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="protocol"/>, <paramref name="name"/> or <paramref name="customQueries"/> parameters are null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        public static void Register(string protocol, string app, int port,
            string name, string customQueries, string icon = null, int iconNumber = 0,RegisterScheme register = RegisterScheme.OnCurrentUser)
        {
            CreateUri(protocol, app, port, name, icon, iconNumber, register, customQueries);
        }

        private static void CreateUri(string protocol, string app, int port,
            string name, string icon = null, int iconNumber = 0, params object[] objects)
        {
            if(!File.Exists(app))
                throw new FileNotFoundException(string.Format("Couldn't locate file '{0}'",
                    app));

            if (string.IsNullOrEmpty(protocol))
                throw new ArgumentNullException(protocol.GetType().Name);

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(name.GetType().Name);

            using (CustomUriBuilder build = new CustomUriBuilder())
            {
                StringBuilder args = new StringBuilder();
                RegistryKey key = Registry.LocalMachine;
                RegisterScheme _scheme = RegisterScheme.OnCurrentUser;
                String _custom = null;
                Int32 _argstoPass = 1;

                foreach (var o in objects)
                {
                    var t = o.GetType();
                    if (t == typeof(RegisterScheme))
                    {
                        _scheme = (RegisterScheme)o;
                    }
                    else if (t == typeof(Int32))
                    {
                        _argstoPass = (int)o;
                    }
                    else if (t == typeof(String))
                    {
                        _argstoPass = 0;
                        _custom = (string)o;
                        if(string.IsNullOrEmpty(_custom))
                            throw new ArgumentNullException(_custom.GetType().Name);
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid parameter passed");
                    }
                }

                try
                {
                    if(_custom == null)
                    {
                        for (int i = 1; i <= _argstoPass; i++)
                        {
                            args.Append(string.Format(
                                " %{0}", i));
                        }
                    }
                    else
                    {
                        args.Append(string.Format(
                                " {0}", _custom));
                    }

                    var _protocol = Regex.Match(protocol, @"(?<=://).+?(?=:|/|\Z)");
                    if(_protocol.Success)
                    {
                        protocol = _protocol.Value;
                    }

                    if(!Uri.CheckSchemeName(protocol))
                    {
                        throw new FormatException(string.Format(
                            "'{0}' is not a valid scheme", protocol));
                    }

                    build.Scheme = protocol;

                    switch (_scheme)
                    {
                        case RegisterScheme.OnCurrentUser:
                            key = Registry.CurrentUser.OpenSubKey(@"Software\Classes\" + name);
                            break;
                        case RegisterScheme.OnMachine:
                            key = Registry.LocalMachine.OpenSubKey(@"Software\Classes\" + name);
                            break;
                        default:
                            throw new InvalidEnumArgumentException("register", (int)_scheme, typeof(RegisterScheme));
                    }

                    if(key == null)
                    {
                        switch(_scheme)
                        {
                            case RegisterScheme.OnCurrentUser:
                                key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\" + name);
                                break;
                            case RegisterScheme.OnMachine:
                                key = Registry.LocalMachine.CreateSubKey(@"Software\Classes\" + name);
                                break;
                            default:
                                throw new InvalidEnumArgumentException("register", (int)_scheme, typeof(RegisterScheme));
                        }
                        
                    }

                    build.Port = 0;
                    if(UriParser.IsKnownScheme(build.Scheme))
                    {
                        throw new ApplicationException("This scheme is already registered");
                    }

                    if(!string.IsNullOrEmpty(icon))
                        if(!File.Exists(icon))
                            throw new FileNotFoundException(string.Format("Couldn't locate file '{0}'",
                                icon));

                    key.SetValue(string.Empty, string.Format("URL:{0} Protocol", build.Scheme));
                    key.SetValue("URL Protocol", string.Empty);
                    if(!string.IsNullOrEmpty(icon))
                    {
                        var _key = key.CreateSubKey(@"DefaultIcon");
                        _key.SetValue(string.Empty, string.Format("{0},{1}",
                            icon,iconNumber));
                    }
                    key = key.CreateSubKey(@"shell\open\command");
                    key.SetValue(string.Empty, app + args.ToString());
                    key.Close();
                    key.Dispose();

                }
                catch(Exception ex)
                {
                    if(ex is ObjectDisposedException)
                    {
                        throw new InvalidOperationException("Error while writting to registry",
                             new InvalidOperationException(string.Format("A method in '{0}' class may have be running simultaneously", typeof(UriScheme).Name, ex)));
                    }
                    else if (ex is SecurityException || ex is UnauthorizedAccessException)
                    {
                        switch (_scheme)
                        {
                            case RegisterScheme.OnCurrentUser:
                                throw new UnauthorizedAccessException("Failed to write registry values on current user, check your permissions");
                            case RegisterScheme.OnMachine:
                                throw new UnauthorizedAccessException("Failed to write registry values on the local machine, check your permissions");
                        }

                        throw new UnauthorizedAccessException("Failed to write registry values, check your permissions");
                    }

                    throw ex;
                }
            };
            
        }

        private static CustomUriBuilder GetUri(string name, params object[] objects)
        {
            try
            {
                CustomUriBuilder _result = new CustomUriBuilder();
                RegistryKey key = Registry.LocalMachine;
                RegisterScheme _scheme = RegisterScheme.OnCurrentUser;
                foreach(var o in objects)
                {
                    var t = o.GetType();
                    if (t == typeof(RegisterScheme))
                    {
                        _scheme = (RegisterScheme)o;
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid parameter passed");
                    }
                }

                switch(_scheme)
                {
                    case RegisterScheme.OnCurrentUser:
                        key = Registry.CurrentUser.OpenSubKey(@"Software\Classes\" + name);
                        break;
                    case RegisterScheme.OnMachine:
                        key = Registry.LocalMachine.OpenSubKey(@"Software\Classes\" + name);
                        break;
                    default:
                        throw new InvalidEnumArgumentException("register", (int)_scheme, typeof(RegisterScheme));
                }

                if(key == null)
                {
                    key.Dispose();
                    throw new InvalidOperationException();
                }

                var _protocol = (string)key.GetValue(null);
                if(_protocol != null)
                {
                    _result.Scheme = _protocol.Replace("URL:", string.Empty).Replace("Protocol", string.Empty).Trim();
                }

                var _key = key.OpenSubKey(@"DefaultIcon");
                if(_key != null)
                {
                    int n = 0;
                    string _file = string.Empty;
                    var icon = (string)_key.GetValue(null);
                    if(icon != null)
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

                if(key == null)
                {
                    var _query = 0;
                    var _key2 = (string)key.GetValue(null);
                    var _app = Regex.Match(_key2, @"^(?:[a-zA-Z]\:|\\\\[\w\.]+\\[\w.$]+)\\(?:[\w]+\\)*\w([\w.])+$", RegexOptions.None);
                    if(_app.Success)
                    {
                        if(!string.IsNullOrEmpty(_app.Value))
                            _result.Path = _app.Value;

                        var q = Regex.Match(_key2.Replace(_app.Value, string.Empty).Trim(), "^%[0-9]+$");
                        if(q.Success)
                        {
                            _query = q.Groups.Count;
                        }
                        else
                        {
                            _result.Query = q.Value;
                        }
                    }

                    _result.Queries = _query;
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
        /// <param name="uri">The <see cref="CustomUriBuilder"/> to be used as a uri scheme</param>
        /// <param name="name">The name of application handler</param>
        /// <param name="register">The type of <see cref="RegisterScheme"/></param>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when invalid object is passed, scheme doesn't exist,or another method is running simultaneously</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="name"/> parameters are null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        public static void Unregister(CustomUriBuilder uri, string name, RegisterScheme register = RegisterScheme.OnCurrentUser)
        {
            DeleteUri(uri.Scheme, uri.Port, name, register);
        }

        /// <summary>
        /// Unregister a uri scheme
        /// </summary>
        /// <param name="protocol">The uri scheme</param>
        /// <param name="port">The port number associated with uri scheme</param>
        /// <param name="name">The name of application handler</param>
        /// <param name="register">The type of <see cref="RegisterScheme"/></param>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when invalid object is passed, scheme doesn't exist, or another method is running simultaneously</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="protocol"/> or <paramref name="name"/> parameters are null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        public static void Unregister(string protocol, int port, string name, RegisterScheme register = RegisterScheme.OnCurrentUser)
        {
            DeleteUri(protocol, port, name, register);
        }

        private static void DeleteUri(string protocol, int port,
            string name, params object[] objects)
        {
            if (string.IsNullOrEmpty(protocol))
                throw new ArgumentNullException(protocol.GetType().Name);

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(name.GetType().Name);

            RegisterScheme _scheme = RegisterScheme.OnCurrentUser;
            foreach (var o in objects)
            {
                var t = o.GetType();
                if (t == typeof(RegisterScheme))
                {
                    _scheme = (RegisterScheme)o;
                }
                else
                {
                    throw new InvalidOperationException("Invalid parameter passed");
                }
            }

            if(!Exist(protocol, port, name, _scheme))
            {
                throw new InvalidOperationException("The scheme or application name doesn't exist");
            }

            try
            {
                switch (_scheme)
                {
                    case RegisterScheme.OnCurrentUser:
                        Registry.CurrentUser.DeleteSubKey(@"Software\Classes\" + name);
                        break;
                    case RegisterScheme.OnMachine:
                        Registry.LocalMachine.DeleteSubKey(@"Software\Classes\" + name);
                        break;
                    default:
                        throw new InvalidEnumArgumentException("register", (int)_scheme, typeof(RegisterScheme));
                }
            }
            catch (Exception ex)
            {
                if (ex is SecurityException || ex is UnauthorizedAccessException)
                {
                    switch (_scheme)
                    {
                        case RegisterScheme.OnCurrentUser:
                            throw new UnauthorizedAccessException("Failed to delete registry values on current user, check your permissions");
                        case RegisterScheme.OnMachine:
                            throw new UnauthorizedAccessException("Failed to delete registry values on the local machine, check your permissions");
                    }

                    throw new UnauthorizedAccessException("Failed to delete registry values, check your permissions");
                }
                else if(ex is ObjectDisposedException)
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
        /// <param name="uri">The <see cref="CustomUriBuilder"/> to be used as a uri scheme</param>
        /// <param name="name">The name of application handler</param>
        /// <param name="register">The type of <see cref="RegisterScheme"/></param>
        /// <returns>Returns true if the uri scheme exist</returns>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when an <see cref="IOException"/></exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="name"/> parameters are null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        public static Boolean Exist(CustomUriBuilder uri,
            string name, RegisterScheme register = RegisterScheme.OnCurrentUser)
        {
            return UriExist(uri.Scheme, uri.Port, name, register);
        }

        /// <summary>
        /// Determines if uri scheme already exist
        /// </summary>
        /// <param name="protocol">The uri scheme</param>
        /// <param name="port">The port number associated with uri scheme</param>
        /// <param name="name">The name of application handler</param>
        /// <param name="register">The type of <see cref="RegisterScheme"/></param>
        /// <returns>Returns true if the uri scheme exist</returns>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when an <see cref="IOException"/></exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="name"/> or <paramref name="protocol"/> parameters are null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        public static Boolean Exist(string protocol, int port,
            string name, RegisterScheme register = RegisterScheme.OnCurrentUser)
        {
           return UriExist(protocol, port, name, register);
        }

        private static Boolean UriExist(string protocol, int port,
            string name, params object[] objects)
        {
            RegisterScheme _scheme = RegisterScheme.OnCurrentUser;

            foreach(var o in objects)
            {
                var t = o.GetType();
                if(t == typeof(RegisterScheme))
                {
                    _scheme = (RegisterScheme)o;
                }
            }

            try
            {
                if (string.IsNullOrEmpty(protocol))
                    throw new ArgumentNullException(protocol.GetType().Name);

                if(string.IsNullOrEmpty(name))
                    throw new ArgumentNullException(name.GetType().Name);

                RegistryKey key = Registry.CurrentUser;
                switch (_scheme)
                {
                    case RegisterScheme.OnCurrentUser:
                        key = Registry.CurrentUser.OpenSubKey(@"Software\Classes\" + name);
                        break;
                    case RegisterScheme.OnMachine:
                        key = Registry.LocalMachine.OpenSubKey(@"Software\Classes\" + name);
                        break;
                    default:
                        throw new InvalidEnumArgumentException("register", (int)_scheme, typeof(RegisterScheme));
                }

                if(key == null)
                {
                    string _key = key.GetValue(null).ToString();
                    if( _key == null)
                    {
                        key.Dispose();
                        return false;
                    }
                    else
                    {
                       if(_key.Replace("URL:", string.Empty).Replace("Protocol", string.Empty).Trim() == protocol)
                       {
                            return true;
                       }

                       return false;
                    }
                }
                else
                {
                    key.Dispose();
                    return false;
                }
            }
            catch(Exception ex)
            {
                if(ex is SecurityException || ex is UnauthorizedAccessException)
                {
                    switch(_scheme)
                    {
                        case RegisterScheme.OnCurrentUser:
                            throw new UnauthorizedAccessException("Failed to read registry values on current user, check your permissions");
                        case RegisterScheme.OnMachine:
                            throw new UnauthorizedAccessException("Failed to read registry values on the local machine, check your permissions");
                    }

                    throw new UnauthorizedAccessException("Failed to read registry values, check your permissions");
                }
                else
                {
                    throw new InvalidOperationException(ex.Message);
                }
            }
        }
    }

    /// <summary>
    /// Class that allows use for <see cref="UriScheme"/> class for COM objects
    /// </summary>
    [ComVisible(true)]
    [ProgId("Eze.IO.Application.UriSchemeManager")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class UriSchemeManager
    {
        /// <summary>
        /// Class that allows use for <see cref="UriScheme"/> class for COM objects
        /// </summary>
        protected UriSchemeManager() { this.ThrowError = true; }

        /// <summary>
        /// Gets or sets if <see cref="Exception"/>(s) should be thrown in this instance
        /// <para>Set to true by default</para>
        /// </summary>
        [ComVisible(true)]
        protected Boolean ThrowError { get; set; }

        /// <summary>
        /// Calls static method <see cref="UriScheme.Register(string, string, int, string, string, string, int, RegisterScheme)"/>
        /// </summary>
        [ComVisible(true)]
        public virtual void Register(string protocol, string app, int port, string name, string customQueries, 
            string icon = null, int iconNumber = 0, RegisterScheme register = RegisterScheme.OnCurrentUser)
        {
            try
            {
                UriScheme.Register(protocol, app, port, name, customQueries, icon, iconNumber, register);
            }
            catch(Exception ex)
            {
                if(this.ThrowError)
                    throw ex;
            }
        }

        /// <summary>
        /// Calls static method <see cref="UriScheme.Unregister(string, int, string, RegisterScheme)"/>
        /// </summary>
        [ComVisible(true)]
        public virtual void Unregister(string protocol, int port, string name, 
            RegisterScheme register = RegisterScheme.OnCurrentUser)
        {
            try
            {
                UriScheme.Unregister(protocol, port, name, register);
            }
            catch (Exception ex)
            {
                if (this.ThrowError)
                    throw ex;
            }
        }

        /// <summary>
        /// Calls static method <see cref="UriScheme.UpdateScheme(string, string, int, string, string, string, int, RegisterScheme)"/>
        /// </summary>
        [ComVisible(true)]
        public virtual void UpdateScheme(string protocol, string app, int port, string name, string customQueries,
            string icon = null, int iconNumber = 0, RegisterScheme register = RegisterScheme.OnCurrentUser)
        {
            try
            {
                UriScheme.UpdateScheme(protocol, app, port, name, customQueries, icon, iconNumber, register);
            }
            catch (Exception ex)
            {
                if (this.ThrowError)
                    throw ex;
            }
        }

        /// <summary>
        /// Calls static method <see cref="UriScheme.Exist(string, int, string, RegisterScheme)"/>
        /// </summary>
        [ComVisible(true)]
        public virtual Boolean Exist(string protocol, int port, string name,
            RegisterScheme register = RegisterScheme.OnCurrentUser)
        {
            try
            {
                return UriScheme.Exist(protocol, port, name, register);
            }
            catch (Exception ex)
            {
                if(this.ThrowError)
                    throw ex;

                return false;
            }
        }

        ///<summary>Returns class name</summary>
        ///<returns>Returns class name</returns>
        [ComVisible(false)]
        protected new string ToString()
        {
            try
            {
                return string.Format("{0}", this.GetType().Name);
            }
            catch (Exception ex)
            {
                if(this.ThrowError)
                    throw ex;

                return null;
            }
        }
    }

    /// <summary>
    /// Type of registration for <see cref="UriScheme"/> class
    /// </summary>
    [ComVisible(true)]
    public enum RegisterScheme : Byte
    {
        /// <summary> Register on the current user</summary>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        OnCurrentUser = 0x20,
        /// <summary> On the entire machine</summary>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        OnMachine = 0x40
    }
}
