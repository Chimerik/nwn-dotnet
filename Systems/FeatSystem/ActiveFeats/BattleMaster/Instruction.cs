using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Instruction(NwCreature caster, NwGameObject targetObject)
    {
      if(targetObject is not NwCreature target)
      {
        caster.ControllingPlayer?.SendServerMessage("Vous devez cibler une créature", ColorConstants.Red);
        return;
      }

      if(target == caster)
      {
        caster.ControllingPlayer?.SendServerMessage("Cette manoeuvre ne permet pas de vous cibler vous même", ColorConstants.Red);
        return;
      }

      if (caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value < 1)
      {
        caster.ControllingPlayer?.SendServerMessage("Vous ne disposez plus d'action bonus", ColorConstants.Red);
        return;
      }

      if (target.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).Value < 1)
      {
        caster.ControllingPlayer?.SendServerMessage("La créature ciblée ne dispose plus de réaction", ColorConstants.Red);
        return;
      }

      caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value -= 1;
      target.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).Value -= 1;

      int warMasterLevel = caster.GetClassInfo(NwClass.FromClassId(CustomClass.Fighter)).Level;
      int superiorityDice = warMasterLevel > 9 ? warMasterLevel > 17 ? 10 : 12 : 8;

      target.GetObjectVariable<LocalVariableInt>(CreatureUtils.ManoeuvreTypeVariable).Value = CustomSkill.WarMasterInstruction;
      target.GetObjectVariable<LocalVariableInt>(CreatureUtils.ManoeuvreDiceVariable).Value = superiorityDice;

      target.ApplyEffect(EffectDuration.Temporary, EffectSystem.increaseNumAttackEffect, NwTimeSpan.FromRounds(1));
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.decreaseNumAttackEffect, NwTimeSpan.FromRounds(1));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"Instruction ({target.Name})", StringUtils.gold);
      FeatUtils.DecrementManoeuvre(caster);
      CreatureUtils.HandleBonusActionCooldown(caster);
    }
  }
}
