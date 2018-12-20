using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eze.IO.Application
{
    /// <summary>
    /// Class for parsing uri scheme(s) passed in application and also can be used in the <see cref="UriScheme"/> class
    /// </summary>
    public static class UriSchemeParser
    {
        /// <summary>
        /// Parse and return all valid uri scheme(s) in <see cref="UriBuilder"/> object(s)
        /// </summary>
        /// <example>
        /// Example on how to get uri builder in application:
        /// <code>
        /// public class Test
        /// {
        ///     [STAThread]
        ///     static int Main(string[] args)
        ///     {
        ///         UriBuilder[] list = UriSchemeParser.GetUriBuilders(args);
        ///         return 0;
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <param name="args">The commandline argument(s)</param>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="args"/> is null</exception>
        public static UriBuilder[] GetUriBuilders(String[] args)
        {
            List<UriBuilder> list = new List<UriBuilder>();
            foreach (String arg in args)
            {
                try
                {
                    list.Add(new UriBuilder(new Uri(arg)));
                }
                catch { /* skip */}
            }
            return list.ToArray();
        }

        /// <summary>
        /// Parse and return a valid uri scheme in a <see cref="UriBuilder"/> object
        /// </summary>
        /// <example>
        /// Example on how to get uri builder in application:
        /// <code>
        /// public class Test
        /// {
        ///     [STAThread]
        ///     static int Main(string[] args)
        ///     {
        ///         foreach(string arg in args)
        ///             UriBuilder ur = UriSchemeParser.GetUriBuilder(arg);
        ///         return 0;
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <param name="arg">The commandline argument</param>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="arg"/> is null</exception>
        public static UriBuilder GetUriBuilder(String arg)
        {
            if(string.IsNullOrEmpty(arg))
                throw new ArgumentNullException(nameof(arg));
            try
            {
                return new UriBuilder(new Uri(arg));
            }
            catch { return null;  }
        }
    }
}
