﻿namespace Rusty.CutsceneEditor.Compiler
{
    /// <summary>
    /// Compile a graph edit into a code string.
    /// </summary>
    public static class GraphEditCompiler
    {
        /* Public methods. */
        /// <summary>
        /// Compile a graph edit into a compiler graph that represents the cutscene program.
        public static CompilerGraph Compile(CutsceneGraphEdit graphEdit)
        {
            // 1. Create empty graph.
            CompilerGraph graph = new();
            graph.Data.Set = graphEdit.InstructionSet;

            // 2. Create nodes.
            for (int i = 0; i < graphEdit.Nodes.Count; i++)
            {
                var node = GraphEditNodeCompiler.Compile(graphEdit.Nodes[i]);
                graph.AddNode(node);
            }

            // 3. Connect nodes.
            for (int i = 0; i < graphEdit.Nodes.Count; i++)
            {
                // Get editor & compiler node.
                CutsceneGraphNode fromEditorNode = graphEdit.Nodes[i];
                CompilerNode fromCompilerNode = graph[i];

                // For each output slot...
                for (int j = 0; j < fromEditorNode.Slots.Count; j++)
                {
                    // Add empty outputs for non-connected slots.
                    if (fromEditorNode.Slots[j].Output == null)
                        fromCompilerNode.ConnectTo(null);

                    // Else, connect nodes.
                    else
                    {
                        CutsceneGraphNode toEditorNode = fromEditorNode.Slots[j].Output.Node;
                        int toNodeIndex = graphEdit.Nodes.IndexOf(toEditorNode);
                        CompilerNode toCompilerNode = graph[toNodeIndex];

                        fromCompilerNode.ConnectTo(toCompilerNode);
                    }
                }
            }

            return graph;
        }
    }
}