﻿using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void HandleShortRest(Player player)
    {
      player.shortRest += 1;

      ShortRestHeal(player.oid.LoginCreature);
      player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_ILLUSION_SEE_INVI_COOLDOWN").Delete();

      FighterUtils.RestoreManoeuvres(player.oid.LoginCreature);
      FighterUtils.RestoreTirArcanique(player.oid.LoginCreature);
      MonkUtils.RestoreKi(player.oid.LoginCreature);
      WizardUtils.AbjurationSuperieure(player.oid.LoginCreature);
      FighterUtils.RestoreEldritchKnight(player.oid.LoginCreature);
      EnsoUtils.RetablissementSorcier(player.oid.LoginCreature);
      DruideUtils.RestoreFormeSauvage(player.oid.LoginCreature, shortRest:true);
      OccultisteUtils.HandleResilienceCeleste(player.oid.LoginCreature);
      BarbarianUtils.RestoreBarbarianRage(player.oid.LoginCreature, true);
      ClercUtils.RestoreClercDomaine(player.oid.LoginCreature, true);
      FighterUtils.RestoreSecondSouffle(player.oid.LoginCreature, true);

      if (player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.SourceDinspiration))
        BardUtils.RestoreInspirationBardique(player.oid.LoginCreature);

      foreach (var skill in player.learnableSkills.Values.Where(l => l.restoreOnShortRest && l.currentLevel > 0))
      {
        byte nbCharge = 1;

        switch (skill.id)
        {
          case CustomSkill.VigueurNaine:
            player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>(VigueurNaineHDVariable).Value = player.oid.LoginCreature.Level;
            return;

          case CustomSkill.FighterSurge:
            if (player.oid.LoginCreature.Classes.Any(c => Utils.In(c.Class.ClassType, ClassType.Fighter, (ClassType)CustomClass.EldritchKnight) && c.Level > 16))
              nbCharge++;
            break;
        }

        player.oid.LoginCreature.SetFeatRemainingUses((Feat)skill.id, nbCharge);
      }

      BarbarianUtils.RestoreImplacableRage(player.oid.LoginCreature);
      player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_RAGE_IMPLACABLE_DD").Value = 10;
      player.oid.LoginCreature.GetObjectVariable<LocalVariableString>(CharmeInstinctifVariable).Delete();
    }

    private static void ShortRestHeal(NwCreature creature)
    {
      int missingHP = creature.MaxHP - creature.HP;

      if (missingHP < 1)
        return;

      int bonusHeal = creature.KnowsFeat((Feat)CustomSkill.Survivant2) ? GetAbilityModifierMin1(creature, Ability.Constitution): 0;
      int remainingDie = creature.GetFeatRemainingUses((Feat)CustomSkill.ShortRest);
      int consumedDie = creature.LevelInfo.Count - remainingDie;
      int healAmount = 0;

      for(int i = consumedDie; i < creature.LevelInfo.Count; i++) 
      {
        healAmount += Utils.Roll(creature.LevelInfo[i].HitDie) + bonusHeal;
        creature.DecrementRemainingFeatUses((Feat)CustomSkill.ShortRest);

        if (missingHP - healAmount < 1)
          break;
      }

      creature.ApplyEffect(EffectDuration.Instant, Effect.Heal(healAmount));
      creature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingL));
    }
  }
}
