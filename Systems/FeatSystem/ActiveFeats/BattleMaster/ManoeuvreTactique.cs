using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ManoeuvreTactique(NwCreature caster)
    {
      caster?.ControllingPlayer.EnterTargetMode(SelectManoeuvreTactiqueTarget, Config.selectCreatureTargetMode);
      caster?.ControllingPlayer.SendServerMessage("Veuillez sélectionner une cible alliée", ColorConstants.Orange);
    }
    private static void SelectManoeuvreTactiqueTarget(Anvil.API.Events.ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled || selection.TargetObject is not NwCreature target)
        return;

      if (target == selection.Player.ControlledCreature)
      {
        selection.Player.SendServerMessage("Cette manoeuvre ne permet pas de vous cibler vous même", ColorConstants.Red);
        return;
      }

      int warMasterLevel = selection.Player.ControlledCreature.GetClassInfo(NwClass.FromClassId(CustomClass.Fighter)).Level;
      int superiorityDice = warMasterLevel > 9 ? warMasterLevel > 17 ? 10 : 12 : 8;

      selection.Player.ControlledCreature.GetObjectVariable<LocalVariableInt>(CreatureUtils.ManoeuvreTypeVariable).Value = CustomSkill.WarMasterManoeuvreTactique;
      selection.Player.ControlledCreature.GetObjectVariable<LocalVariableInt>(CreatureUtils.ManoeuvreDiceVariable).Value = superiorityDice;

      target.ApplyEffect(EffectDuration.Temporary, EffectSystem.manoeuvreTactique, NwTimeSpan.FromRounds(1));

      StringUtils.DisplayStringToAllPlayersNearTarget(selection.Player.ControlledCreature, $"Manoeuvre Tactique ({target.Name})", StringUtils.gold);
      FeatUtils.DecrementManoeuvre(selection.Player.ControlledCreature);
    }
  }
}
