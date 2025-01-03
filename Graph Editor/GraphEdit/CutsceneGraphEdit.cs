using Godot;
using Rusty.Cutscenes;

namespace Rusty.CutsceneEditor
{
    [GlobalClass]
    public partial class CutsceneGraphEdit : GraphEdit
    {
        /* Public properties. */
        public static int LineHeight => 32;

        [Export] public VBoxContainer PropertiesInspector { get; set; }
        [Export] public InstructionSet InstructionSet { get; set; }

        /* Private properties. */
        private AddNodePopup Popup { get; set; }
        private Vector2 MustOpenPopupAt { get; set; }
        private Vector2 CreatePos { get; set; }

        /* Public methods. */
        public void ConnectNode(CutsceneGraphNode fromNode, int fromSlot, CutsceneGraphNode toNode)
        {
            // Ensure that slots are available.
            fromNode.EnsureSlots(fromSlot + 1);
            toNode.EnsureSlots(1);

            // Connect.
            Error error = ConnectNode(fromNode.Name, fromSlot, toNode.Name, 0);
            OnConnect(fromNode.Name, fromSlot, toNode.Name, 0);

            if (error != Error.Ok)
                GD.PrintErr($"Could not connect node {fromNode} (slot {fromSlot}) to {toNode} (slot 0) (error {error})!");
        }

        /* Godot overrides. */
        public override void _EnterTree()
        {
            // Create popup.
            Popup = new(InstructionSet);
            AddChild(Popup);
            Popup.Hide();

            Popup.SelectedInstruction += OnPopupSelectedInstruction;

            // Set up events.
            ConnectionRequest += OnConnect;
            DisconnectionRequest += OnDisconnect;
            DeleteNodesRequest += OnDelete;
        }

        public override void _Process(double delta)
        {
            if (MustOpenPopupAt != Vector2.Zero)
            {
                ShowPopup((int)MustOpenPopupAt.X, (int)MustOpenPopupAt.Y);
                MustOpenPopupAt = Vector2.Zero;
            }
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.ButtonIndex == MouseButton.Right)
            {
                MustOpenPopupAt = eventMouseButton.Position;
            }
        }

        public CutsceneGraphNode Spawn(InstructionDefinition definition, Vector2 positionOffset,
            InstructionInstance[] instances = null)
        {
            CutsceneGraphNode node = new();
            AddChild(node);

            node.PositionOffset = positionOffset;

            node.InspectorWindow = PropertiesInspector;
            if (definition != null)
                node.Populate(InstructionSet, definition);
            else
                GD.PrintErr("Tried to spawn a node without an instruction definition.");

            if (instances != null)
                node.Set(instances);

            return node;
        }

        /* Private methods. */
        private void OnPopupSelectedInstruction(InstructionDefinition definition)
        {
            GD.Print(definition);
            if (definition != null)
            {
                Vector2 positionOffset = (GetGlobalMousePosition() - GlobalPosition + ScrollOffset) / Zoom;
                Spawn(definition, positionOffset);
            }
        }

        private void ShowPopup(int x, int y)
        {
            Popup.Popup();
            Popup.Position = new Vector2I(x, y);
            CreatePos = Popup.Position;
        }

        private void OnConnect(StringName fromNode, long fromPort, StringName toNode, long toPort)
        {
            CutsceneGraphNode from = GetNode(fromNode);
            NodeSlotPair fromSlot = from.Slots[(int)fromPort];

            // Disconnect if it this connection was already used.
            if (fromSlot.Output != null)
            {
                StringName previousToNode = fromSlot.Output.Node.Name;
                OnDisconnect(fromNode, fromPort, previousToNode, toPort);
            }

            // Connect nodes from the perspective of the graph edit.
            ConnectNode(fromNode, (int)fromPort, toNode, (int)toPort);

            // Connect nodes from the perspective of the node slots.
            CutsceneGraphNode to = GetNode(toNode);
            NodeSlotPair toSlot = to.Slots[(int)toPort];
            fromSlot.ConnectOutput(toSlot);
        }

        private void OnDisconnect(StringName fromNode, long fromPort, StringName toNode, long toPort)
        {
            CutsceneGraphNode from = GetNode(fromNode);
            NodeSlotPair slot = from.Slots[(int)fromPort];

            DisconnectNode(fromNode, (int)fromPort, toNode, (int)toPort);
            slot.DisconnectOutput();
        }

        private void OnDelete(Godot.Collections.Array nodes)
        {
            foreach (Variant obj in nodes)
            {
                StringName nodeName = obj.AsStringName();
                CutsceneGraphNode node = GetNode(nodeName);
                for (int i = 0; i < node.Slots.Count; i++)
                {
                    NodeSlotPair slot = node.Slots[i];

                    // Disconnect inputs.
                    while (slot.Inputs.Count > 0)
                    {
                        NodeSlotPair fromSlot = slot.Inputs[0];
                        CutsceneGraphNode fromNode = fromSlot.Node;
                        int fromIndex = fromNode.Slots.IndexOf(fromSlot);
                        OnDisconnect(fromSlot.Node.Name, fromIndex, nodeName, 0);
                    }

                    // Disconnect output.
                    NodeSlotPair toSlot = slot.Output;
                    if (toSlot != null)
                        OnDisconnect(nodeName, i, toSlot.Node.Name, 0);
                }
            }
        }

        private CutsceneGraphNode GetNode(StringName nodeName)
        {
            foreach (Node child in GetChildren())
            {
                if (child.Name == nodeName && child is CutsceneGraphNode node)
                    return node;
            }
            return null;
        }
    }
}