using Autodesk.DesignScript.Runtime;
using System;

namespace Camber
{
    [SupressImportIntoVM]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class NodeMigrationMappingAttribute : Attribute
    {
        public string OldName { get; }
        public string NewName { get; }

        public NodeMigrationMappingAttribute(string oldName, string newName)
        {
            OldName = oldName;
            NewName = newName;
        }
    }

}
