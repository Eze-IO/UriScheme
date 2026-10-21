using System;
using System.Text;

namespace Eze.IO.Application
{
    /// <summary>
    /// Icon information, which also can be used in the <see cref="UriSchemeBuilder"/> class
    /// </summary>
    [Serializable]
    public struct Icon<TFile, TNumber>
    {
        private String _file;
        private Int32 _number;

        /// <summary>
        /// Icon information, which also can be used in the <see cref="UriSchemeBuilder"/> class
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
    public sealed class UriSchemeBuilder : IDisposable
    {
        /// <summary>
        /// Gets or sets local path of application file
        /// </summary>
        public String Path { get; set; }

        /// <summary>
        /// Gets or sets the number of parameter(s) or argument(s) to pass on the <seealso cref="Path"/> file
        /// <para>Set to 1 by default, which passes the uri as "%1"</para>
        /// </summary>
        [Obsolete("Windows only passes the uri to the application as \"%1\", so a parameter count has no effect. Leave it at 1, or use Parameter for custom arguments.")]
        public Int32 Parameters { get; set; } = 1;

        /// <summary>
        /// Gets or sets the string parameter(s) or argument(s) to pass on the <seealso cref="Path"/> file
        /// </summary>
        public String Parameter { get; set; }

        /// <summary>
        /// Gets or sets the protocol portion of the uri scheme
        /// </summary>
        public String Protocol { get; set; }

        /// <summary>
        /// Gets or sets the icon information with <see cref="Icon{TFile, TNumber}"/>
        /// </summary>
        public Icon<String, Int32> Icon { get; set; }

        /// <summary>
        /// Class for creating uri(s) which also can be used in the <see cref="UriScheme"/> class
        /// </summary>
        public UriSchemeBuilder() { }

        /// <summary>
        /// Class for creating uri(s) which also can be used in the <see cref="UriScheme"/> class
        /// </summary>
        public UriSchemeBuilder(UriBuilder uri)
        {
            this.Path = uri.Uri.LocalPath;
            this.Protocol = uri.Scheme;
            this.Parameter = uri.Query;
            this.Icon = new Icon<string, int>(this.Path, 0);
        }

        /// <summary>
        /// Does nothing, this class holds no resources
        /// <para>Kept so existing <c>using</c> blocks keep compiling</para>
        /// </summary>
        public void Dispose() { }
    }
}
