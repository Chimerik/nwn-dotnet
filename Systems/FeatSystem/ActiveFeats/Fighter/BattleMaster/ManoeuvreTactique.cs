using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ManoeuvreTactique(NwCreature caster, NwGameObject targetObject)
    {
      if(targetObject is null || targetObject is not NwCreature target || target == caster || caster.IsReactionTypeHostile(target))
      {
        caster.LoginPlayer?.SendServerMessage("Veuillez sélectionner une cible valide", ColorConstants.Red);
        return;
      }

      FeatUtils.ClearPreviousManoeuvre(caster);

      int warMasterLevel = caster.GetClassInfo(ClassType.Fighter).Level;
      int superiorityDice = warMasterLevel > 17 ? 12 : warMasterLevel > 9 ? 10 : 8;

      caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.ManoeuvreTypeVariable).Value = CustomSkill.WarMasterManoeuvreTactique;
      caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.ManoeuvreDiceVariable).Value = superiorityDice;

      target.ApplyEffect(EffectDuration.Temporary, EffectSystem.manoeuvreTactique, NwTimeSpan.FromRounds(1));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"Manoeuvre Tactique ({target.Name})", StringUtils.gold);
      FeatUtils.DecrementManoeuvre(caster);
    }
  }
}
