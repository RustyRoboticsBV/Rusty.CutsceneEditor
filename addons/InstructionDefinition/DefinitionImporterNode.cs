#if TOOLS
using Godot;
using Godot.Collections;

namespace Rusty.Cutscenes.Importer.XmlLoader
{
    /// <summary>
    /// An XML cutscene definition importer. Needed as an interface between the gdscript import plugin and the C# module, as we
    /// cannot get non-node classes by type.
    /// </summary>
    [Tool]
    [GlobalClass]
    public partial class DefinitionImporterNode : Node
    {
        /* Public methods. */
        public static InstructionDefinition Import(string xmlFilePath, Dictionary importOptions)
        {
            return Importer.InstructionDefinitionImporter.Load(xmlFilePath);
        }
    }
}
#endif