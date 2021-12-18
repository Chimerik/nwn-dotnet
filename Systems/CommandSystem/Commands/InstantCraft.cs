using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  class InstantCraft
  {
    public InstantCraft(NwPlayer oPC)
    {
      oPC.SendServerMessage("Veuillez sélectionner la cible du craft instantanné.");
      oPC.EnterTargetMode(SelectCraftTarget, ObjectTypes.Creature, MouseCursor.CreateDown);
    }
    private void SelectCraftTarget(ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled || !selection.TargetObject.IsPlayerControlled(out NwPlayer oPC) || !PlayerSystem.Players.TryGetValue(oPC.LoginCreature, out PlayerSystem.Player targetPlayer))
        return;

      if (targetPlayer.craftJob != null && targetPlayer.craftJob.baseItemType != 10)
        targetPlayer.craftJob.remainingTime = 1;
    }
  }
}
