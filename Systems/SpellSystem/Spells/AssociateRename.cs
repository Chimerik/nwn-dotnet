using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void AssociateRename(NwGameObject oCaster, AssociateType associateType)
    {
      if (Players.TryGetValue(oCaster, out Player player))
      {
        if (!player.windows.TryGetValue("editorFamiliarName", out var value)) player.windows.Add("editorFamiliarName", new EditorFamiliarName(player, associateType));
        else ((EditorFamiliarName)value).CreateWindow(associateType);
      }
    }
  }
}
