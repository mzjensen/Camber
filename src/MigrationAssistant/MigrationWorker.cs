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
                if (!IsDynamoGraph(graphFilePath))
                {
                    Log($"File {graphFilePath} is not a Dynamo graph. Ignoring.");
                    return MigrationResult.Skipped;
                }

                Log($"Starting to migrate graph {graphFilePath}...");

                var graph = LoadGraph(graphFilePath);
                if (graph == null)
                {
                    Log("No nodes found");
                    return MigrationResult.Skipped;
                }

                UpdateGraphMetadata(graph, graphFilePath);

                var migrated = MigrateNodes(graph);

                Log($"Migrated {migrated} Camber nodes");

                SaveGraph(graph, graphFilePath, outputFolder);

                return MigrationResult.Success;
            }
            catch (Exception ex)
            {
                Log($"ERROR: {ex.Message}");
                return MigrationResult.Error;
            }
        }

        private bool IsDynamoGraph(string graphFilePath)
        {
            return Path.GetExtension(graphFilePath) == ".dyn";
        }

        private JObject LoadGraph(string graphFilePath)
        {
            var contents = File.ReadAllText(graphFilePath);
            var graph = JObject.Parse(contents);
            return graph["Nodes"] is JArray ? graph : null;
        }

        private void UpdateGraphMetadata(JObject graph, string graphFilePath)
        {
            graph["Uuid"] = Guid.NewGuid().ToString();
            var newGraphName = Path.GetFileNameWithoutExtension(graphFilePath) + "_migrated";
            graph["Name"] = newGraphName;
        }

        private int MigrateNodes(JObject graph)
        {
            var nodes = (JArray)graph["Nodes"];
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

                        if (RemoveNodeFromDependencies(graph, nodeId))
                        {
                            migrated++;
                        }
                    }
                }
            }

            return migrated;
        }

        private bool RemoveNodeFromDependencies(JObject graph, string nodeId)
        {
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
                        return true;
                    }
                }
            }
            return false;
        }

        private void SaveGraph(JObject graph, string graphFilePath, string outputFolder)
        {
            var newGraphName = graph["Name"].ToString();
            var outputFile = newGraphName + Path.GetExtension(graphFilePath);
            var outputPath = Path.Combine(outputFolder, outputFile);
            File.WriteAllText(outputPath, graph.ToString(Formatting.Indented));
            Log($"File saved to {outputPath}\n");
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