using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Eze.IO.Application
{
    public static partial class UriScheme
    {
        /// <summary>
        /// Update a existing uri scheme
        /// </summary>
        /// <param name="uri">The <see cref="CustomUriBuilder"/> to be used as a uri scheme</param>
        /// <param name="name">The name of application handler</param>
        /// <param name="register">The type of <see cref="RegisterScheme"/></param>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when invalid object is passed, scheme doesn't exist, or another method is running simultaneously</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="name"/> parameters are null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        /// <exception cref="NullReferenceException">Exception thrown when method fails to find uri scheme</exception>
        public static void UpdateScheme(CustomUriBuilder uri, string name, RegisterScheme register = RegisterScheme.OnCurrentUser)
        {
            if(Exist(uri, name, register))
            {
                UpdateScheme(uri.Scheme, uri.Port, name, register);
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
        /// <param name="port">The port number associated with uri scheme</param>
        /// <param name="name">The name of application handler</param>
        /// <param name="register">The type of <see cref="RegisterScheme"/></param>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when invalid object is passed, scheme doesn't exist, or another method is running simultaneously</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="protocol"/> or <paramref name="name"/> parameters are null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        /// <exception cref="NullReferenceException">Exception thrown when method fails to find uri scheme</exception>
        public static void UpdateScheme(string protocol, int port, string name, RegisterScheme register = RegisterScheme.OnCurrentUser)
        {
            if(Exist(protocol, port, name, register))
            {
                var find = GetUri(name, register);
                if(find != null)
                {
                    Unregister(find, name, register);
                    find.Scheme = protocol;
                    find.Port = port;
                    Register(find, name, register);
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
        /// <param name="app">The file path to application.</param>
        /// <param name="port">The port number associated with uri scheme</param>
        /// <param name="name">The name of application handler</param>
        /// <param name="customQueries">The custom arguments to pass</param>
        /// <param name="icon">The icon associated with application</param>
        /// <param name="iconNumber">The icon number associated with <paramref name="icon"/></param>
        /// <param name="register">The type of <see cref="RegisterScheme"/></param>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when invalid object is passed, scheme doesn't exist, or another method is running simultaneously</exception>
        /// <exception cref="FileNotFoundException">Exception thrown when a <paramref name="app"/> or <paramref name="icon"/> file doesn't exist</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="protocol"/>, <paramref name="name"/> or <paramref name="customQueries"/> parameters are null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        /// <exception cref="NullReferenceException">Exception thrown when method fails to find uri scheme</exception>
        public static void UpdateScheme(string protocol, string app, int port, string name, string customQueries, 
            string icon = null, int iconNumber = 0, RegisterScheme register = RegisterScheme.OnCurrentUser)
        {
            if(Exist(protocol, port, name, register))
            {
                var find = GetUri(name, register);
                if(find != null)
                {
                    Unregister(find, name, register);
                    find.Icon = new Icon<string, int>(icon, iconNumber);
                    find.Path = app;
                    find.Port = port;
                    find.Query = customQueries;
                    find.Scheme = protocol;
                    Register(find, name, register);
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
        /// <param name="app">The file path to application.</param>
        /// <param name="port">The port number associated with uri scheme</param>
        /// <param name="name">The name of application handler</param>
        /// <param name="queries" > The number of arguments to pass</param>
        /// <param name="icon">The icon associated with application</param>
        /// <param name="iconNumber">The icon number associated with <paramref name="icon"/></param>
        /// <param name="register">The type of <see cref="RegisterScheme"/></param>
        /// <exception cref="InvalidEnumArgumentException">Exception thrown when the <paramref name="register"/> parameter has a invalid value</exception>
        /// <exception cref="InvalidOperationException">Exception thrown when invalid object is passed, scheme doesn't exist, or another method is running simultaneously</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="protocol"/> or <paramref name="name"/> parameters are null</exception>
        /// <exception cref="FileNotFoundException">Exception thrown when a <paramref name="app"/> or <paramref name="icon"/> file doesn't exist</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="protocol"/> or <paramref name="name"/> parameters are null</exception>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        /// <exception cref="NullReferenceException">Exception thrown when method fails to find uri scheme</exception>
        public static void UpdateScheme(string protocol, string app, int port, string name, int queries, string icon = null, 
            int iconNumber = 0, RegisterScheme register = RegisterScheme.OnCurrentUser)
        {
            if (Exist(protocol, port, name, register))
            {
                var find = GetUri(name, register);
                if (find != null)
                {
                    Unregister(find, name, register);
                    find.Icon = new Icon<string, int>(icon, iconNumber);
                    find.Path = app;
                    find.Port = port;
                    find.Queries = queries;
                    find.Scheme = protocol;
                    Register(find, name, register);
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

    /// <summary>
    /// Icon information, which also can be used in the <see cref="CustomUriBuilder"/> class
    /// </summary>
    [Serializable]
    public struct Icon<TFile, TNumber>
    {
        private String _file;
        private Int32 _number;

        /// <summary>
        /// Icon information, which also can be used in the <see cref="CustomUriBuilder"/> class
        /// </summary>
        /// <param name="file">The icon file path <para>for file extenstion: .exe, .dll, .ico and other supported image files</para></param>
        /// <param name="number">The icon index number</param>
        public Icon(String file, Int32 number)
        {
            this._file = file;
            this._number = number;
        }

        /// <summary>
        /// The icon file path
        /// <para>for file extenstion: .exe, .dll, .ico and other supported image files</para>
        /// </summary>
        public String File { get { return _file; } }
        /// <summary>
        /// The icon index number
        /// </summary>
        public Int32 Number { get { return _number; } }

        /// <summary>
        /// The icon index number
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            if(!string.IsNullOrEmpty(this.File))
            {
                sb.Append(File.ToString());
            }
            sb.Append(", "+Number.ToString());
            sb.Append("]");
            return sb.ToString();
        }
    }

    /// <summary>
    /// Class for creating uri(s) which also can be used in the <see cref="UriScheme"/> class
    /// </summary>
    public sealed class CustomUriBuilder : UriBuilder, IDisposable
    {
        private bool _disposed = false;

        /// <summary>
        /// Gets or sets local path of application file
        /// </summary>
        public new String Path { get; set; }

        /// <summary>
        /// Gets or sets the number of parameter(s) or argument(s) to pass on the <seealso cref="Path"/> file
        /// </summary>
        public Int32 Queries { get; set; }

        /// <summary>
        /// Gets or sets the string parameter(s) or argument(s) to pass on the <seealso cref="Path"/> file
        /// </summary>
        public new String Query { get; set; }

        /// <summary>
        /// Gets or sets the icon information with <see cref="Icon{TFile, TNumber}"/>
        /// </summary>
        public Icon<String, Int32> Icon { get; set; }

        /// <summary>
        /// Class for creating uri(s) which also can be used in the <see cref="UriScheme"/> class
        /// </summary>
        public CustomUriBuilder() { }

        /// <summary>
        /// Class for creating uri(s) which also can be used in the <see cref="UriScheme"/> class
        /// </summary>
        /// <exception cref="InvalidOperationException">The <see cref="Uri"/> class can thrown this exception</exception>
        public CustomUriBuilder(Uri uri)
        {
            this.Path = uri.LocalPath;
            this.Port = uri.Port;
            this.Scheme = uri.Scheme;
            this.Host = Uri.Host;
            this.Query = uri.Query;
            this.Fragment = uri.Fragment;
            this.Path = uri.AbsolutePath;
        }

        /// <summary>
        /// Release information in <seealso cref="UriBuilder.Uri"/> in this instance
        /// </summary>
        ~CustomUriBuilder()
        {
            this.Dispose(false);
        }

        internal void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    foreach(PropertyInfo pi in this.GetType().GetProperties())
                    {
                        if(pi.PropertyType == typeof(String))
                        {
                            pi.SetValue(this, null);
                        }
                        else if(pi.PropertyType == typeof(Int32))
                        {
                            pi.SetValue(this, 0);
                        }
                    }

                    foreach (PropertyInfo pi in this.GetType().GetProperties())
                    {
                        if(pi.PropertyType == typeof(String))
                        {
                            var _get = (string)pi.GetValue(this);
                            if(!string.IsNullOrEmpty(_get))
                                _disposed = false;
                        }
                        else if (pi.PropertyType == typeof(Int32))
                        {
                            var _get = (int)pi.GetValue(this);
                            if(_get != 0)
                                _disposed = false;
                        }
                    }

                    _disposed = true;
                }
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
