using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Eze.IO.Application
{
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
        public UriSchemeManager() { this.ThrowError = true; }

        /// <summary>
        /// Gets or sets if <see cref="Exception"/>(s) should be thrown in this instance
        /// <para>Set to true by default</para>
        /// </summary>
        [ComVisible(true)]
        public Boolean ThrowError { get; set; }

        /// <summary>
        /// Calls static method <see cref="UriScheme.RegisterScheme(string, string, string, string, string, int, RecordScheme)"/>
        /// </summary>
        [ComVisible(true)]
        public virtual void RegisterScheme(string protocol, string app, string name, string customQueries,
            string icon = null, int iconNumber = 0, RecordScheme register = RecordScheme.OnCurrentUser)
        {
            try
            {
                UriScheme.RegisterScheme(protocol, app, name, customQueries, icon, iconNumber, register);
            }
            catch (Exception)
            {
                if (this.ThrowError)
                    throw;
            }
        }

        /// <summary>
        /// Calls static method <see cref="UriScheme.UnregisterScheme(string, string, RecordScheme)"/>
        /// </summary>
        [ComVisible(true)]
        public virtual void UnregisterScheme(string protocol, string name,
            RecordScheme register = RecordScheme.OnCurrentUser)
        {
            try
            {
                UriScheme.UnregisterScheme(protocol, name, register);
            }
            catch (Exception)
            {
                if (this.ThrowError)
                    throw;
            }
        }

        /// <summary>
        /// Calls static method <see cref="UriScheme.UpdateScheme(string, string, string, string, string, int, RecordScheme)"/>
        /// </summary>
        [ComVisible(true)]
        public virtual void UpdateScheme(string protocol, string app, string name, string customQueries,
            string icon = null, int iconNumber = 0, RecordScheme register = RecordScheme.OnCurrentUser)
        {
            try
            {
                UriScheme.UpdateScheme(protocol, app, name, customQueries, icon, iconNumber, register);
            }
            catch (Exception)
            {
                if (this.ThrowError)
                    throw;
            }
        }

        /// <summary>
        /// Calls static method <see cref="UriScheme.Exists(string, RecordScheme)"/>
        /// </summary>
        [ComVisible(true)]
        public virtual Boolean Exist(string protocol, string name,
            RecordScheme register = RecordScheme.OnCurrentUser)
        {
            try
            {
                return UriScheme.Exists(protocol, register);
            }
            catch (Exception)
            {
                if (this.ThrowError)
                    throw;

                return false;
            }
        }

        ///<summary>Returns class name</summary>
        ///<returns>Returns class name</returns>
        [ComVisible(false)]
        public override string ToString()
        {
            return string.Format("[{0}]", this.GetType().Name);
        }
    }
}
