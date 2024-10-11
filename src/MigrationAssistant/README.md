# Camber Migration Assistant

The Camber Migration Assistant is a simple tool to help you upgrade Dynamo graphs with legacy Camber nodes and replace them with nodes from the main Civil 3D library. The tool can be used to migrate an entire folder of Dynamo graphs, which reduces the amount of manual effort required to replace individual nodes one-by-one.

## Background
Camber was originally developed to fill gaps in the Dynamo for Civil 3D node library. However, the Civil 3D 2025.1 update (August 2024) included a significant expansion in the number of nodes available out-of-the-box in Civil 3D. This is a very good thing, and it means that a large percentage of the nodes in Camber can be replaced by the new nodes available in the main product. The Migration Assistant helps with this process.

## Important
**The Migration Assistant does not guarantee that your graphs will run exactly the same as they did before with zero issues. After migration, you will still need to open each graph to verify the results, and potentially make further adjustments.**

## Usage
1. Navigate to the `extra` folder in the Camber package directory. This location will vary depending on your package path settings in Dynamo. By default, it will be in `C:\Users\<username>\AppData\Roaming\Autodesk\C3D <version>\Dynamo\<version\packages\Camber\extra`.
2. Open the `MigrationAssistant` folder and run `MigrationAssistant.exe`
3. Click `Select Source Folder` to select a folder of Dynamo graphs to be migrated
4. Click `Select Output Folder` to select a destination folder for saving the migrated graphs
5. Click `Migrate Graphs` to perform the migration
6. Open the graphs in Dynamo to verify the results
7. If needed, view the log file for additional information

## Notes
- It is not possible to automatically migrate all Camber nodes. Even though the name of the node may be similar to a node in Civil 3D, the arrangement of the ports may be different, or the output of the node is not guaranteed to be exactly the same. In these cases, the Camber node will display a message recommending which node to replace it with from the Civil 3D library. You will need to manually add the new node and reconnect the ports as necessary.
- The Migration Assistant only handles migration of Zero-Touch nodes based on their function signatures. Migration of UI nodes is not supported.
- The mapping between old node name and new node name is provided in the `Camber.Migrations.xml` file. This file must be present in the same location as `MigrationAssistant.exe`.
- The `Camber.Migrations.xml` file is in the same format as required by Dynamo's automatic node migration mechanism. If you want to experiment with this, you can copy the XML file to the `bin` folder of the package directory (same location as `Camber.dll`) and then open a graph in Dynamo. Camber nodes will be automatically migrated by Dynamo when the graph is opened, but Dynamo will not understand that the new nodes are in a different assembly. So Camber will still show in the Workspace References panel, even if no nodes from Camber remain in the graph. The Migration Assistant handles this scenario by cleaning up the old dependencies.