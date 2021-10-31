#region references
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
#endregion

namespace Camber.AutoCAD
{
    public sealed class DocumentExtensions
    {
        #region properties
        #endregion

        #region constructors
        internal DocumentExtensions() { }
        #endregion

        #region methods

        /// <summary>
        /// Executes a particular command string. Returns true if the command was executed successfully and false otherwise.
        /// Ensure that a space or \n is included at the end of any string that you wish to be executed.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="echoInCommandLine">Show the command in the command line?</param>
        public static bool SendCommand(acDynNodes.Document document, string command, bool echoInCommandLine = true)
        {
            if (string.IsNullOrEmpty(command)) { throw new ArgumentException("Command string is null or empty."); }

            try
            {
                document.AcDocument.SendStringToExecute(command, false, false, echoInCommandLine);
                return true;
            }
            catch
            {
                throw;
                return false;
            }
        }
        #endregion
    }
}
