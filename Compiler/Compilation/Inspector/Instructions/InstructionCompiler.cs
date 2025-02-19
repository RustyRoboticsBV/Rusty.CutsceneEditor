﻿using Rusty.Cutscenes;
using Rusty.Graphs;

namespace Rusty.CutsceneEditor.Compiler
{
    /// <summary>
    /// A compiler that converts instruction inspectors to graph nodes,
    /// </summary>
    public abstract class InstructionCompiler : Compiler
    {
        /* Public methods. */
        /// <summary>
        /// Compile an instruction inspector into a compile node hierarchy.
        /// </summary>
        public static SubNode<NodeData> Compile(InstructionInspector inspector)
        {
            InstructionSet set = inspector.InstructionSet;
            InstructionDefinition definition = inspector.Definition;
            InstructionInstance instance = new(definition);

            // Pre-instruction group.
            SubNode<NodeData> preInstructionGroup = CompilerNodeMaker.GetPreInstructionBlock(set);

            // Compile rules.
            for (int i = 0; i < definition.CompileRules.Length; i++)
            {
                try
                {
                    Inspector ruleInspector = inspector.GetCompileRuleInspector(i);
                    preInstructionGroup.AddChild(RuleCompiler.Compile(ruleInspector));
                }
                catch { }
            }

            // Main instruction.
            for (int i = 0; i < definition.Parameters.Length; i++)
            {
                if (definition.Parameters[i] is OutputParameter)
                    continue;

                try
                {
                    instance.Arguments[i] = inspector.GetParameterInspector(i).ValueObj.ToString();
                }
                catch { }
            }

            preInstructionGroup.AddChild(CompilerNodeMaker.GetInstruction(set, instance));

            // End of pre-instruction group.
            preInstructionGroup.AddChild(CompilerNodeMaker.GetEndOfBlock(set));

            return preInstructionGroup;
        }
    }
}