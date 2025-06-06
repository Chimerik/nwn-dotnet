﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void HandleLongRest(Player player)
    {
      var nbInspiHeroique = player.oid.LoginCreature.GetFeatRemainingUses((Feat)CustomSkill.InspirationHeroique);

      if (Utils.In(player.oid.LoginCreature.Race.RacialType, RacialType.Human, (RacialType)CustomRace.WoodHalfElf, (RacialType)CustomRace.DrowHalfElf, (RacialType)CustomRace.HighHalfElf)
        && nbInspiHeroique < 3)
        nbInspiHeroique += 1;

      player.shortRest = 0;

      player.oid.LoginCreature.ForceRest();
      player.oid.LoginCreature.GetObjectVariable<LocalVariableString>(RegardHypnotiqueTargetListVariable).Delete();
      player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>(EffectSystem.EvocateurSurchargeVariable).Delete();
      player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_ILLUSION_SEE_INVI_COOLDOWN").Delete();
      player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>(Wizard.TransmutationStoneVariable).Delete();
      player.oid.LoginCreature.GetObjectVariable<LocalVariableString>(CharmeInstinctifVariable).Delete();

      RangerUtils.RestoreEnnemiJure(player.oid.LoginCreature);
      FighterUtils.RestoreManoeuvres(player.oid.LoginCreature);
      FighterUtils.RestoreTirArcanique(player.oid.LoginCreature);
      FighterUtils.RestoreSecondSouffle(player.oid.LoginCreature);
      BarbarianUtils.RestoreBarbarianRage(player.oid.LoginCreature);
      MonkUtils.RestoreKi(player.oid.LoginCreature);
      WizardUtils.RestaurationArcanique(player.oid.LoginCreature);
      DruideUtils.RecuperationNaturelle(player.oid.LoginCreature);
      RangerUtils.RegenerationNaturelle(player.oid.LoginCreature);
      WizardUtils.ResetAbjurationWard(player.oid.LoginCreature);
      WizardUtils.ResetPresage(player.oid);
      FighterUtils.RestoreEldritchKnight(player.oid.LoginCreature);
      BardUtils.RestoreInspirationBardique(player.oid.LoginCreature);
      PaladinUtils.RestorePaladinCharges(player.oid.LoginCreature);
      ClercUtils.RestoreConduitDivin(player.oid.LoginCreature);
      ClercUtils.RestoreInterventionDivine(player.oid.LoginCreature);
      ClercUtils.RestoreClercDomaine(player.oid.LoginCreature);
      EnsoUtils.RestoreSorcerySource(player.oid.LoginCreature);
      DruideUtils.RestoreFormeSauvage(player.oid.LoginCreature);
      OccultisteUtils.RestoreFouleeFeerique(player.oid.LoginCreature);
      OccultisteUtils.RestoreLueurDeGuerison(player.oid.LoginCreature);
      OccultisteUtils.HandleResilienceCeleste(player.oid.LoginCreature);
      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.DonDuProtecteur, (byte)(player.oid.LoginCreature.GetAbilityModifier(Ability.Charisma) > 1 ? player.oid.LoginCreature.GetAbilityModifier(Ability.Charisma) : 1));

      if (player.oid.LoginCreature.Race.Id == CustomRace.HalfOrc)
      {
        player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>(EffectSystem.EnduranceImplacableVariable).Value = 1;
        player.ApplyHalfOrcEndurance();
      }

      if (player.learnableSkills.ContainsKey(CustomSkill.PaladinSentinelleImmortelle))
      {
        player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>(EffectSystem.SentinelleImmortelleVariable).Value = 1;
        player.ApplySentinelleImmortelle();
      }

      if (player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.BersekerFrenziedStrike))
        player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.BersekerFrenziedStrike, 0);

      if (player.oid.LoginCreature.Classes.Any(c => Utils.In(c.Class.ClassType, ClassType.Fighter, (ClassType)CustomClass.EldritchKnight) && c.Level < 17))
        player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.FighterSurge, 1);

      player.oid.LoginCreature.ApplyEffect(EffectDuration.Temporary, EffectSystem.CanPrepareSpells, NwTimeSpan.FromHours(1));

      HandleSpellRestoration(player.oid.LoginCreature);

      if (player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ChatimentDivin))
      {
        byte chatimentLevel = (byte)(player.windows.TryGetValue("chatimentLevelSelection", out var chatimentWindow)
          ? ((ChatimentLevelSelectionWindow)chatimentWindow).selectedSpellLevel : 1);
        player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ChatimentDivin, player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).GetRemainingSpellSlots(chatimentLevel));
      }

      player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>(ClercMartialVariable).Delete();

      EffectUtils.RemoveTaggedEffect(player.oid.LoginCreature, EffectSystem.DivinationVisionEffectTag);
      SpellUtils.DispelConcentrationEffects(player.oid.LoginCreature);

      if (player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ChatimentOcculte))
        player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ChatimentOcculte, (byte)(player.oid.LoginCreature.GetClassInfo((ClassType)CustomClass.Occultiste).GetRemainingSpellSlots(1)));

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ClercIncantationPuissante, 0);
      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.InspirationHeroique, nbInspiHeroique);
    }

    private static void HandleSpellRestoration(NwCreature caster)
    {
      var spellCasterClasses = caster.Classes.Where(c => c.Class.IsSpellCaster && c.Class.ClassType != (ClassType)CustomClass.Occultiste);

      if (spellCasterClasses.Any())
      {
        if (spellCasterClasses.Count() > 1)
        {
          int casterLevel = 0;

          foreach (var classInfo in spellCasterClasses)
          {
            switch (classInfo.Class.ClassType)
            {
              case ClassType.Druid:
              case ClassType.Wizard:
              case ClassType.Bard:
              case ClassType.Cleric:
              case ClassType.Sorcerer: casterLevel += classInfo.Level; break;

              case ClassType.Paladin:
              case ClassType.Ranger: casterLevel += (int)Math.Round((double)classInfo.Level / 2, MidpointRounding.AwayFromZero); break;

              case (ClassType)CustomClass.RogueArcaneTrickster:
              case (ClassType)CustomClass.EldritchKnight: casterLevel += (int)Math.Round((double)classInfo.Level / 3, MidpointRounding.ToZero); break;
            }
          }

          RestoreSpellCasterSlots(casterLevel, spellCasterClasses);
        }
        else
          RestoreSpellCasterSlots(spellCasterClasses.FirstOrDefault().Level, spellCasterClasses);
      }

      spellCasterClasses = caster.Classes.Where(c => c.Class.ClassType == (ClassType)CustomClass.Occultiste);

      if (spellCasterClasses.Any())
        RestoreSpellCasterSlots(spellCasterClasses.FirstOrDefault().Level, spellCasterClasses);
    }

    private static void RestoreSpellCasterSlots(int casterLevel, IEnumerable<CreatureClassInfo> casterClasses)
    {
      foreach (var classInfo in casterClasses)
      {
        var spellGainTable = classInfo.Class.SpellGainTable[casterLevel - 1];
        byte i = 0;

        foreach (var spellGain in spellGainTable)
        {
          classInfo.SetRemainingSpellSlots(i, spellGain);
          i++;
        }
      }
    }
  }
}
