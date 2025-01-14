using System.Collections.Generic;

using UnityEditor.ShaderGraph;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;

[UnityEditor.AssetImporters.ScriptedImporter(3, Extension)]
class ShaderSubGraphImporter : UnityEditor.AssetImporters.ScriptedImporter
{
    public const string Extension = "shadersubgraph";

    public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx)
    {
        var textGraph = File.ReadAllText(ctx.assetPath, Encoding.UTF8);
        var graph = JsonUtility.FromJson<GraphData>(textGraph);

        if (graph == null)
            return;
        
        graph.isSubGraph = true;

        var sourceAssetDependencyPaths = new List<string>();
        foreach (var node in graph.GetNodes<AbstractMaterialNode>())
            node.GetSourceAssetDependencies(sourceAssetDependencyPaths);

        var graphAsset = ScriptableObject.CreateInstance<MaterialSubGraphAsset>();
        graphAsset.subGraph = graph;

        ctx.AddObjectToAsset("MainAsset", graphAsset);
        ctx.SetMainObject(graphAsset);

        foreach (var sourceAssetDependencyPath in sourceAssetDependencyPaths.Distinct())
            ctx.DependsOnSourceAsset(sourceAssetDependencyPath);
    }
}
