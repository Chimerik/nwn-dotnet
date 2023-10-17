using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnAcquireItemSavePlayer(ModuleEvents.OnAcquireItem onAcquireItem)
    {
      NwItem oItem = onAcquireItem.Item;

      if (onAcquireItem.AcquiredBy is null || onAcquireItem.AcquiredBy is not NwCreature oPC || oPC.ControllingPlayer == null || oItem == null
        || !PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player) || player.pcState == PlayerSystem.Player.PcState.Offline)
        return;

      player.oid.ExportCharacter(); 
    }
    public static void OnUnacquireItemSavePlayer(ModuleEvents.OnUnacquireItem onUnacquireItem)
    {
      NwItem oItem = onUnacquireItem.Item;

      if (onUnacquireItem.LostBy.ControllingPlayer == null || oItem == null
        || !PlayerSystem.Players.TryGetValue(onUnacquireItem.LostBy, out PlayerSystem.Player player) || player.pcState == PlayerSystem.Player.PcState.Offline)
        return;

      player.oid.ExportCharacter();
    }
  }
}
