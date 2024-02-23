using System.Collections.Generic;
using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Ralliement(NwCreature caster, NwGameObject targetObject)
    {
      FeatUtils.ClearPreviousManoeuvre(caster);

      if (targetObject is not NwCreature target)
      {
        caster.ControllingPlayer?.SendServerMessage("Vous devez cibler une créature", ColorConstants.Red);
        return;
      }

      if(target == caster)
      {
        caster.ControllingPlayer?.SendServerMessage("Cette manoeuvre ne permet pas de vous cibler vous même", ColorConstants.Red);
        return;
      }

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      int warMasterLevel = caster.GetClassInfo(NwClass.FromClassId(CustomClass.Fighter)).Level;
      int superiorityDice = warMasterLevel > 9 ? warMasterLevel > 17 ? 10 : 12 : 8;
      int temporaryHP = NwRandom.Roll(Utils.random, superiorityDice) + caster.GetAbilityModifier(Ability.Charisma);
      List<int> highestHPList = new();

      foreach (var eff in target.ActiveEffects)
        if (eff.EffectType == EffectType.TemporaryHitpoints)
          highestHPList.Add(eff.IntParams[3]);

      if(temporaryHP > highestHPList.Max())
        target.ApplyEffect(EffectDuration.Temporary, Effect.TemporaryHitpoints(temporaryHP), NwTimeSpan.FromRounds(1));
      else
        caster.ControllingPlayer?.SendServerMessage("Attention : les points de vie temporaires ne sont pas cumulatifs !", ColorConstants.Red);

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"Ralliement ({target.Name})", StringUtils.gold);
      FeatUtils.DecrementManoeuvre(caster);
    }
  }
}
