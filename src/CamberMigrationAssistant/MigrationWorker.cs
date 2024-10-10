using Newtonsoft.Json;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace Camber.MigrationAssistant
{
    public class MigrationWorker
    {
        private readonly string _logFilePath;
        private readonly string _xmlFilePath;
        private Dictionary<string, string> _functionSignatureMap;

        public MigrationWorker(string logFilePath, string xmlFilePath)
        {
            _logFilePath = logFilePath;
            _xmlFilePath = xmlFilePath;
        }

        public void Initialize()
        {
            _functionSignatureMap = GenerateFunctionSignatureMap(_xmlFilePath);
            File.WriteAllText(_logFilePath, string.Empty);
        }

        public MigrationResult MigrateGraph(string graphFilePath, string outputFolder)
        {
            try
            {
                if (Path.GetExtension(graphFilePath) != ".dyn")
                {
                    Log($"File {graphFilePath} is not a Dynamo graph. Ignoring.");
                    return MigrationResult.Skipped;
                }

                Log($"Starting to migrate graph {graphFilePath}...");

                var contents = File.ReadAllText(graphFilePath);
                var graph = JObject.Parse(contents);

                if (graph["Nodes"] is not JArray nodes)
                {
                    Log("No nodes found");
                    return MigrationResult.Skipped;
                }

                Log($"Found {nodes.Count} nodes total");

                graph["Uuid"] = Guid.NewGuid().ToString();

                var newGraphName = Path.GetFileNameWithoutExtension(graphFilePath) + "_migrated";
                graph["Name"] = newGraphName;

                var migrated = 0;

                foreach (var node in nodes)
                {
                    var functionSignature = node["FunctionSignature"]?.Value<string>();
                    var nodeId = node["Id"]?.Value<string>();

                    if (functionSignature != null)
                    {
                        var oldSignature = _functionSignatureMap.Keys.FirstOrDefault(functionSignature.Contains);
                        if (oldSignature != null)
                        {
                            var newSignature = _functionSignatureMap[oldSignature];
                            node["FunctionSignature"] = newSignature;
                            Log($"Replaced node function signature: {oldSignature} -> {newSignature}");

                            if (graph["NodeLibraryDependencies"] is JArray nodeLibDeps)
                            {
                                var packageDeps = nodeLibDeps.FirstOrDefault(dep => dep["Name"]?.Value<string>() == "Camber");

                                if (packageDeps != null && packageDeps["Nodes"] is JArray nodesArray)
                                {
                                    var nodeIds = nodesArray.Values<string>().ToList();
                                    if (nodeIds.Contains(nodeId))
                                    {
                                        nodeIds.Remove(nodeId);
                                        packageDeps["Nodes"] = new JArray(nodeIds);
                                        Log($"Removed node {nodeId} from NodeLibraryDependencies");
                                        migrated++;
                                    }
                                }
                            }
                        }
                    }
                }

                Log($"Migrated {migrated} Camber nodes");

                var outputFile = newGraphName + Path.GetExtension(graphFilePath);
                var outputPath = Path.Combine(outputFolder, outputFile);
                File.WriteAllText(outputPath, graph.ToString(Formatting.Indented));

                Log($"File saved to {outputPath}\n");
                return MigrationResult.Success;
            }
            catch (Exception ex)
            {
                Log($"ERROR: {ex.Message}");
                return MigrationResult.Error;
            }
        }

        private static Dictionary<string, string> GenerateFunctionSignatureMap(string xmlFilePath)
        {
            if (!File.Exists(xmlFilePath))
            {
                throw new FileNotFoundException($"XML migration file '{xmlFilePath}' not found");
            }

            var functionSignatureMap = new Dictionary<string, string>();

            try
            {
                XDocument doc = XDocument.Load(xmlFilePath);
                var migrations = doc.Descendants("priorNameHint");

                foreach (var migration in migrations)
                {
                    string oldName = migration.Element("oldName")?.Value;
                    string newName = migration.Element("newName")?.Value;

                    if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
                    {
                        functionSignatureMap[oldName] = newName;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error parsing XML migration file: {ex.Message}");
            }

            return functionSignatureMap;
        }

        private void Log(string message)
        {
            File.AppendAllText(_logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
        }
    }
}