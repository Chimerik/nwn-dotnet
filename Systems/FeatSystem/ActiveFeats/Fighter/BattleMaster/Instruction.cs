using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Instruction(NwCreature caster, NwGameObject targetObject)
    {
      FeatUtils.ClearPreviousManoeuvre(caster);

      if (targetObject is null || targetObject is not NwCreature target || target == caster || target.IsReactionTypeHostile(caster))
      {
        caster.ControllingPlayer?.SendServerMessage("Veuillez sélectionner une cible valide", ColorConstants.Red);
        return;
      }

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      if (target.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).Value < 1)
      {
        caster.ControllingPlayer?.SendServerMessage("La créature ciblée ne dispose plus de réaction", ColorConstants.Red);
        return;
      }

      target.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).Value -= 1;

      int warMasterLevel = caster.GetClassInfo(ClassType.Fighter).Level;
      int superiorityDice = warMasterLevel > 17 ? 12 : warMasterLevel > 9 ? 10 : 8;

      target.GetObjectVariable<LocalVariableInt>(CreatureUtils.ManoeuvreTypeVariable).Value = CustomSkill.WarMasterInstruction;
      target.GetObjectVariable<LocalVariableInt>(CreatureUtils.ManoeuvreDiceVariable).Value = superiorityDice;

      target.ApplyEffect(EffectDuration.Temporary, EffectSystem.increaseNumAttackEffect, NwTimeSpan.FromRounds(1));
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.decreaseNumAttackEffect, NwTimeSpan.FromRounds(1));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"Instruction ({target.Name})", StringUtils.gold, true);
      FeatUtils.DecrementManoeuvre(caster);
    }
  }
}
