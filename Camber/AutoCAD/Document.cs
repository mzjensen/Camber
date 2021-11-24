#region references
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acApp = Autodesk.AutoCAD.ApplicationServices;
using Dynamo.Graph.Nodes;
#endregion

namespace Camber.AutoCAD
{
    public sealed class Document
    {
        #region properties
        #endregion

        #region constructors
        internal Document() { }
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

        /// <summary>
        /// Gets the value of a system variable by name.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public static object GetSystemVariable(acDynNodes.Document document, string variableName)
        {
            return acApp.Application.GetSystemVariable(variableName);
        }

        /// <summary>
        /// Gets if a Document is a named file on the disk (as opposed to a new drawing that has not yet been saved).
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static bool IsNamedDrawing(acDynNodes.Document document)
        {
            return document.AcDocument.IsNamedDrawing;
        }

        /// <summary>
        /// Gets if a Document is read-only.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static bool IsReadOnly(acDynNodes.Document document)
        {
            return document.AcDocument.IsReadOnly;
        }
        #endregion
    }
}
