using Anvil.API;
using System.Collections.Generic;
using System;
using System.Linq;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public void ApplyClassLevelUp(LearnableSkill playerClass, byte customClass)
      {
        if (oid.LoginCreature.Level > 2 || playerClass.currentLevel > 1)
        {
          List<Effect> polymorphEffects = new();

          foreach (var eff in oid.LoginCreature.ActiveEffects.Where(e => e.EffectType == EffectType.Polymorph))
          {
            polymorphEffects.Add(eff);
            oid.LoginCreature.RemoveEffect(eff);
          }

          oid.LoginCreature.ForceLevelUp(NwClass.FromClassId(customClass), RollClassHitDie(oid.LoginCreature.Level,
          customClass, oid.LoginCreature.GetAbilityModifier(Ability.Constitution)
          + (customClass == CustomClass.Ensorceleur && oid.LoginCreature.KnowsFeat((Feat)CustomSkill.EnsoResistanceDraconique)).ToInt()));

          foreach (var eff in polymorphEffects)
            oid.LoginCreature.ApplyEffect(eff.DurationType, eff, TimeSpan.FromSeconds(eff.DurationRemaining));
        }

        GiveRacialBonusOnLevelUp();
      }
    }
  }
}
