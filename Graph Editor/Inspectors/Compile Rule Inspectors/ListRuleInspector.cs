﻿using Rusty.Cutscenes;
using Rusty.EditorUI;

namespace Rusty.CutsceneEditor
{
    /// <summary>
    /// A list rule inspector.
    /// </summary>
    public partial class ListRuleInspector : CompileRuleInspector
    {
        /* Public methods. */
        /// <summary>
        /// The compile rule visualized by this inspector.
        /// </summary>
        public ListRule Rule => Resource as ListRule;

        /* Private properties. */
        private ListElement ListElement { get; set; }

        /* Constructors. */
        public ListRuleInspector() : base() { }

        public ListRuleInspector(InstructionSet instructionSet, ListRule compileRule)
            : base(instructionSet, compileRule) { }

        public ListRuleInspector(ListRuleInspector other) : base(other) { }

        /* Public methods. */
        public override Element Duplicate()
        {
            return new ListRuleInspector(this);
        }

        public override bool CopyStateFrom(Element other)
        {
            if (base.CopyStateFrom(other))
            {
                ListElement = this[0] as ListElement;
                return true;
            }
            else
                return false;
        }

        public override CompileRuleInspector[] GetActiveSubInspectors()
        {
            CompileRuleInspector[] childInspectors = new CompileRuleInspector[ListElement.Count];
            for (int i = 0; i < ListElement.Count; i++)
            {
                childInspectors[i] = ListElement[i][0] as CompileRuleInspector;
            }
            return childInspectors;
        }

        /* Protected methods. */
        protected override void Init()
        {
            // Base compile rule inspector init.
            base.Init();

            // Set name.
            Name = $"ListRule ({Rule.Id})";

            // Add list element.
            ListElement = new ListElement();
            ListElement.HeaderText = Rule.DisplayName;
            ListElement.Template = Create(InstructionSet, Rule.Type);
            Add(ListElement);
        }
    }
}