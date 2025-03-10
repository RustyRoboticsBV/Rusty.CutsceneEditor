﻿using Rusty.Cutscenes;
using Rusty.EditorUI;

namespace Rusty.CutsceneEditor
{
    /// <summary>
    /// A pre-instruction inspector.
    /// </summary>
    public partial class InstructionRuleInspector : InstructionInspector
    {
        /* Constructors. */
        public InstructionRuleInspector() : base() { }

        public InstructionRuleInspector(InstructionSet instructionSet, InstructionDefinition resource)
            : base(instructionSet, resource) { }

        public InstructionRuleInspector(InstructionRuleInspector other) : base(other) { }

        /* Public methods. */
        public override Element Duplicate()
        {
            return new InstructionRuleInspector(this);
        }
    }
}