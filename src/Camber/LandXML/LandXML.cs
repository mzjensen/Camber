using Autodesk.AutoCAD.Runtime;
using Dynamo.Graph.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using civDb = Autodesk.Civil.DatabaseServices;
using civDynNodes = Autodesk.Civil.DynamoNodes;

namespace Camber.LandXML
{
    public sealed class LandXML
    {
        #region properties
        internal XDocument XDocument { get; }
        internal FileInfo FileInfo { get; }
        internal XElement Root => XDocument.Root;
        internal XNamespace Namespace => GetRootAttributeValue("xmlns");

        /// <summary>
        /// Gets the name of a LandXML file.
        /// </summary>
        public string Name => FileInfo.Name.Replace(".xml", "");

        /// <summary>
        /// Gets the directory path where a LandXML file is located.
        /// </summary>
        public string Directory => FileInfo.DirectoryName;

        /// <summary>
        /// Gets the full path to a LandXML file.
        /// </summary>
        public string Path => FileInfo.FullName;

        /// <summary>
        /// Gets the schema version of a LandXML file.
        /// </summary>
        public string SchemaVersion => GetRootAttributeValue("version");

        /// <summary>
        /// Gets the language of a LandXML file.
        /// </summary>
        public string Language => GetRootAttributeValue("language");

        /// <summary>
        /// Gets the timestamp when a LandXML file was created.
        /// </summary>
        public DateTime DateCreated
        {
            get
            {
                var date = Root.Attribute("date")?.Value;
                var time = Root.Attribute("time")?.Value;
                return DateTime.Parse($"{date} {time}");
            }
        }

        /// <summary>
        /// Gets the units defined in a LandXML file.
        /// </summary>
        public Dictionary<string, string> Units => GetChildElementAttributes();

        /// <summary>
        /// Gets the name of the project defined in a LandXML file.
        /// </summary>
        public string Project => GetElementAttribute("Project", "name");

        /// <summary>
        /// Gets the author info from a LandXML file.
        /// </summary>
        public Dictionary<string, string> Author => GetChildElementAttributes("Application");

        /// <summary>
        /// Gets info about the application used to create a LandXML file.
        /// </summary>
        public Dictionary<string, string> Application => GetRootElementAttributes();

        /// <summary>
        /// Gets the coordinate system info from a LandXML file.
        /// </summary>
        public Dictionary<string, string> CoordinateSystem => GetRootElementAttributes();
        #endregion

        #region constructors
        internal LandXML(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new InvalidOperationException("File path is null or empty.");
            }

            if (!File.Exists(filePath))
            {
                throw new InvalidOperationException("A file does not exist at the specified path.");
            }

            var fileInfo = new FileInfo(filePath);

            if (fileInfo.Extension.ToLower() != ".xml")
            {
                throw new InvalidOperationException("The specified file is not an XML file.");
            }

            XDocument xdoc = XDocument.Load(filePath);

            if (xdoc.Root.Name.LocalName != "LandXML")
            {
                throw new InvalidOperationException("The file is not a valid LandXML file.");
            }

            XDocument = xdoc;
            FileInfo = new FileInfo(filePath);
        }

        /// <summary>
        /// Loads a LandXML file.
        /// </summary>
        /// <param name="filePath">The path to the LandXML file</param>
        /// <returns></returns>
        [NodeCategory("Actions")]
        public static LandXML LoadFromFile(string filePath)
        {
            try
            {
                return new LandXML(filePath);
            }
            catch (System.Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Exports a LandXML using the settings defined in the drawing. All available objects will be exported.
        /// </summary>
        /// <param name="folderPath">The path to the destination folder</param>
        /// <param name="fileName">The desired name of the LandXML file (do not include the .xml extension)</param>
        /// <returns></returns>
        public static void Export(string folderPath, string fileName)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                throw new InvalidOperationException("The folder path is null or empty.");
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new InvalidOperationException("The file name is null or empty.");
            }

            if (fileName.ToLower().Contains(".xml"))
            {
                throw new InvalidOperationException(
                    "The file name should not include a .xml extension. " +
                    "It will be appended automatically.");
            }

            if (!System.IO.Directory.Exists(folderPath))
            {
                throw new InvalidOperationException("Invalid folder path.");
            }

            if (!DocumentContainsValidLandXmlEntities(acDynNodes.Document.Current))
            {
                throw new InvalidOperationException("No LandXML-compatible objects found.");
            }

            string path = System.IO.Path.Combine(folderPath, fileName + ".xml");

            try
            {
                AutoCAD.Document.SendCommand(
                    acDynNodes.Document.Current,
                    "-landxmlout" + "\n" + path + "\n",
                    false);

                // Ideally we'd return a new instance of a LandXML object once this is complete,
                // but the only way to know when the command has finished firing is to subscribe to the
                // Document.CommandEnded event and write an event handler to track it. I don't have time
                // for that right now, but it's an idea for the future.
            }
            catch
            {
                throw new InvalidOperationException("Failed to export LandXML.");
            }
        }

        /// <summary>
        /// Imports the objects defined in a LandXML file.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public LandXML Import(acDynNodes.Document document)
        {
            try
            {
                AutoCAD.Document.SendCommand(
                    acDynNodes.Document.Current,
                    "-landxmlin" + "\n" + Path + "\n",
                    false);
                return this;
            }
            catch
            {
                throw new InvalidOperationException("Failed to import LandXML file.");
            }
        }
        #endregion

        #region public methods
        public override string ToString() => $"{nameof(LandXML)}(Name = {Name})";
        #endregion

        #region private methods
        /// <summary>
        /// Checks to see if a Document contains any entities that can be exported to LandXML.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private static bool DocumentContainsValidLandXmlEntities(acDynNodes.Document document)
        {
            bool hasCorridors = civDynNodes.Selection.Corridors(document).Count > 0;
            bool hasAlignments = civDynNodes.Selection.Alignments(document).Count > 0;
            bool hasCogoPoints = civDynNodes.Selection.CogoPointGroups(document).Count > 0;
            bool hasSurfaces = civDynNodes.Selection.Surfaces(document).Count > 0;
            bool hasPipeNetworks = Civil.PipeNetworks.PipeNetwork.GetPipeNetworks(document, true).Count > 0;

            RXClass parcelRxCls = RXClass.GetClass(typeof(civDb.Parcel));
            bool hasParcels = acDynNodes.SelectionByQuery.GetAllObjectsOfTypeByRXClass(
                parcelRxCls, 
                document.ModelSpace)
                .Count > 0;

            RXClass featureLineRxCls = RXClass.GetClass(typeof(civDb.FeatureLine));
            bool hasFeatureLines = acDynNodes.SelectionByQuery.GetAllObjectsOfTypeByRXClass(
                featureLineRxCls, 
                document.ModelSpace)
                .Count > 0;

            bool[] checks = {
                hasCorridors,
                hasAlignments,
                hasCogoPoints,
                hasSurfaces,
                hasPipeNetworks,
                hasParcels,
                hasFeatureLines
            };

            if (checks.Any(c => c == true))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the attribute key-value pairs of an XML element as a dictionary.
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetRootElementAttributes([CallerMemberName] string elementName = null)
        {
            return Root.Element(Namespace + elementName)?
                .Attributes()
                .ToDictionary(
                    a => a.Name.ToString(),
                    a => a.Value.ToString());
        }

        /// <summary>
        /// Gets the child element attributes of a specified parent element.
        /// </summary>
        /// <param name="parentElement"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetChildElementAttributes([CallerMemberName] string parentElement = null)
        {
            return Root.Element(Namespace + parentElement)?
                .Elements()
                .Attributes()
                .ToDictionary(
                    a => a.Name.ToString(),
                    a => a.Value.ToString());
        }

        /// <summary>
        /// Gets the value of an attribute by name from the XML root.
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        private string GetRootAttributeValue([CallerMemberName] string attributeName = null)
        {
            return Root.Attribute(attributeName)?.Value;
        }

        /// <summary>
        /// Gets the value of a specific attribute by name from an element.
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        private string GetElementAttribute(string elementName, string attributeName)
        {
            return Root.Element(Namespace + elementName)?.Attribute(attributeName)?.Value;
        }
        #endregion
    }
}
