using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;

namespace NWN.Systems
{
  class InstantCraft
  {
    public InstantCraft(NwPlayer oPC)
    {
      oPC.SendServerMessage("Veuillez sélectionner la cible du craft instantanné.");
      PlayerSystem.cursorTargetService.EnterTargetMode(oPC, SelectCraftTarget, ObjectTypes.Creature, MouseCursor.Create);
    }
    private void SelectCraftTarget(ModuleEvents.OnPlayerTarget selection)
    {
      if (!PlayerSystem.Players.TryGetValue(selection.TargetObject, out PlayerSystem.Player targetPlayer))
        return;

      if (targetPlayer.craftJob != null && targetPlayer.craftJob.baseItemType != 10)
        targetPlayer.craftJob.remainingTime = 1;
    }
  }
}
