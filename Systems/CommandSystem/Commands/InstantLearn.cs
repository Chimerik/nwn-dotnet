using System.Linq;

using Anvil.API;
using Anvil.API.Events;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  class InstantLearn
  {
    public InstantLearn(NwPlayer oPC)
    {
      oPC.SendServerMessage("Veuillez sélectionner la cible de l'apprentissage instantanné.");
      oPC.EnterTargetMode(SelectLearnTarget, ObjectTypes.All, MouseCursor.CreateDown);
    }
    private void SelectLearnTarget(ModuleEvents.OnPlayerTarget selection)
    {
      /*if (selection.IsCancelled || !selection.TargetObject.IsPlayerControlled(out NwPlayer oPC) || !PlayerSystem.Players.TryGetValue(oPC.LoginCreature, out PlayerSystem.Player targetPlayer))
        return;

      Learnable learnable = targetPlayer.learnables.FirstOrDefault(l => l.Value.active).Value;

      if (learnable != null)
        learnable.acquiredPoints = learnable.pointsToNextLevel;
      else
        selection.Player.SendServerMessage($"{targetPlayer.oid.LoginCreature.Name.ColorString(ColorConstants.White)} ne dispose pas d'apprentissage en cours.", ColorConstants.Orange);
    */
    }
  }
}
