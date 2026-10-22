using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Eze.IO.Application
{
    /// <summary>
    /// Representation for how the scheme should be recorded in the registry using the <see cref="UriScheme"/> class
    /// </summary>
    [ComVisible(true)]
    public enum RecordScheme : byte
    {
        /// <summary> Register on the current user</summary>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        OnCurrentUser = 0x20,
        /// <summary> Register on the entire machine</summary>
        /// <exception cref="UnauthorizedAccessException">Exception thrown when the proper permissions aren't given</exception>
        OnMachine = 0x40
    }
}
