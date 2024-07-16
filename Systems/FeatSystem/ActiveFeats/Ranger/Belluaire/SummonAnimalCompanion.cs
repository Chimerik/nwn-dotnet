using System.Collections.Generic;
using System.Numerics;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void SummonAnimalCompanion(NwCreature caster, Vector3 targetPosition, int featId)
    {
      if (caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).HasValue)
      {
        var companion = caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).Value;
        caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).Delete();

        companion.VisibilityOverride = Anvil.Services.VisibilityMode.Hidden;
        companion.Destroy();
        companion.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpUnsummon));

        RangerUtils.ClearAnimalCompanion(companion);
      }
      else
      {
        NwCreature companion = CreateAnimalCompanion(featId, caster, caster.Location);

        CreaturePlugin.AddAssociate(caster, companion, (int)AssociateType.AnimalCompanion);
        caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).Value = companion;

        for (int i = 268; i < 287; i++)
          if (caster.KnowsFeat((Feat)i))
            companion.AddFeat((Feat)i);

        if (caster.KnowsFeat((Feat)CustomSkill.RangerGreaterFavoredEnemy))
          companion.AddFeat((Feat)CustomSkill.RangerGreaterFavoredEnemy);

        if (caster.KnowsFeat((Feat)CustomSkill.BelluaireDefenseDeLaBete))
          companion.AddFeat((Feat)CustomSkill.BelluaireDefenseDeLaBete);

        if (caster.KnowsFeat((Feat)CustomSkill.BelluaireDefenseDeLaBeteSuperieure))
          companion.AddFeat((Feat)CustomSkill.ChasseurEsquiveInstinctive);

        if (caster.IsLoginPlayerCharacter)
        {
          caster.LoginPlayer.OnClientLeave -= RangerUtils.OnDisconnectBelluaire;
          caster.LoginPlayer.OnClientLeave += RangerUtils.OnDisconnectBelluaire;
        }

        companion.OnDeath += RangerUtils.OnAnimalCompanionDeath;

        if (caster.KnowsFeat((Feat)CustomSkill.BelluaireFurieBestiale))
          caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireFurieBestiale, 100);

        if (caster.KnowsFeat((Feat)CustomSkill.BelluaireSprint))
          caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireSprint, 100);

        if (caster.KnowsFeat((Feat)CustomSkill.BelluaireDisengage))
          caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireDisengage, 100);
      }
    }
    private static NwCreature CreateAnimalCompanion(int featId, NwCreature caster, Location target)
    {
      byte rangerLevel = caster.GetClassInfo(ClassType.Ranger).Level;
      NwCreature companion;
      VfxType vfx = VfxType.FnfSummonMonster1;

      switch (featId)
      {
        default:

          companion = NwCreature.Create("ourscompagnon", target);

          if (!caster.KnowsFeat((Feat)CustomSkill.BelluaireRugissementProvoquant))
            caster.AddFeat((Feat)CustomSkill.BelluaireRugissementProvoquant);

          if (caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BelluaireRugissementProvoquantCoolDownVariable).HasNothing)
            caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireRugissementProvoquant, 100);

          if (!caster.KnowsFeat((Feat)CustomSkill.BelluairePatteMielleuse) && rangerLevel > 4)
            caster.AddFeat((Feat)CustomSkill.BelluairePatteMielleuse);
          else
            caster.SetFeatRemainingUses((Feat)CustomSkill.BelluairePatteMielleuse, 100);

          if (rangerLevel < 5)
          {
            vfx = VfxType.FnfSummonMonster1;
          }
          else if (rangerLevel < 8)
          {
            vfx = VfxType.FnfSummonMonster2;
            companion.MaxHP = 39;
            companion.BaseAC = 15;
          }
          else if (rangerLevel < 11)
          {
            vfx = VfxType.FnfSummonMonster3;
            companion.MaxHP = 69;
            companion.BaseAC = 16;
          }
          else
          {
            vfx = VfxType.FnfSummonMonster3;
            companion.MaxHP = 99;
            companion.BaseAC = 18;
          }

          SetCompanionAbilities(companion, caster, new List<Ability> { Ability.Strength, Ability.Constitution, Ability.Dexterity, Ability.Wisdom, Ability.Charisma, Ability.Intelligence });

          break;

        case CustomSkill.BelluaireBoar:

          companion = NwCreature.Create("sangliercompagnon", target);

          if (!caster.KnowsFeat((Feat)CustomSkill.BelluaireChargeSanglier))
            caster.AddFeat((Feat)CustomSkill.BelluaireChargeSanglier);

          if (caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BelluaireChargeDuSanglierCoolDownVariable).HasNothing)
            caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireChargeSanglier, 100);

          if (!caster.KnowsFeat((Feat)CustomSkill.BelluaireRageSanglier) && rangerLevel > 4)
            caster.AddFeat((Feat)CustomSkill.BelluaireRageSanglier);
          else
            caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireRageSanglier, 100);

          if (rangerLevel < 5)
          {
            vfx = VfxType.FnfSummonMonster1;
          }
          else if (rangerLevel < 8)
          {
            vfx = VfxType.FnfSummonMonster2;
            companion.MaxHP = 27;
            companion.BaseAC = 14;
          }
          else if (rangerLevel < 11)
          {
            vfx = VfxType.FnfSummonMonster3;
            companion.MaxHP = 51;
            companion.BaseAC = 15;
          }
          else
          {
            vfx = VfxType.FnfSummonMonster3;
            companion.MaxHP = 85;
            companion.BaseAC = 17;
          }

          SetCompanionAbilities(companion, caster, new List<Ability> { Ability.Strength, Ability.Constitution, Ability.Dexterity, Ability.Wisdom, Ability.Charisma, Ability.Intelligence });

          break;

        case CustomSkill.BelluaireDireRaven:

          companion = NwCreature.Create("corbeaucompagnon", target);

          if (!caster.KnowsFeat((Feat)CustomSkill.BelluaireCorbeauAveuglement))
            caster.AddFeat((Feat)CustomSkill.BelluaireCorbeauAveuglement);

          if (!caster.KnowsFeat((Feat)CustomSkill.BelluaireCorbeauMauvaisAugure) && rangerLevel > 4)
            caster.AddFeat((Feat)CustomSkill.BelluaireCorbeauMauvaisAugure);
          else
            caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireCorbeauMauvaisAugure, 100);

          if (rangerLevel < 5)
          {
            vfx = VfxType.FnfSummonMonster1;
          }
          else if (rangerLevel < 8)
          {
            vfx = VfxType.FnfSummonMonster2;
            companion.ApplyEffect(EffectDuration.Permanent, Effect.DamageIncrease((int)DamageBonus.Plus1d4, DamageType.Piercing));
            companion.MaxHP = 21;
            companion.BaseAC = 19;
          }
          else if (rangerLevel < 11)
          {
            vfx = VfxType.FnfSummonMonster3;
            companion.MaxHP = 32;
            companion.BaseAC = 20;
          }
          else
          {
            vfx = VfxType.FnfSummonMonster3;
            companion.ApplyEffect(EffectDuration.Permanent, Effect.DamageIncrease((int)DamageBonus.Plus1d6, DamageType.Piercing));
            companion.MaxHP = 44;
            companion.BaseAC = 21;
          }

          SetCompanionAbilities(companion, caster, new List<Ability> { Ability.Dexterity, Ability.Constitution, Ability.Wisdom, Ability.Charisma, Ability.Intelligence, Ability.Strength });

          break;

        case CustomSkill.BelluaireWolf:

          companion = NwCreature.Create("loupcompagnon", target);

          if (!caster.KnowsFeat((Feat)CustomSkill.BelluaireLoupMorsurePlongeante))
            caster.AddFeat((Feat)CustomSkill.BelluaireLoupMorsurePlongeante);

          if (!caster.KnowsFeat((Feat)CustomSkill.BelluaireLoupEffetDeMeute))
            caster.AddFeat((Feat)CustomSkill.BelluaireLoupEffetDeMeute);

          if (rangerLevel > 5)
          {
            if (!caster.KnowsFeat((Feat)CustomSkill.BelluaireLoupMorsureInfectieuse))
              caster.AddFeat((Feat)CustomSkill.BelluaireLoupMorsureInfectieuse);

            companion.OnCreatureDamage += RangerUtils.OnDamageLoupMorsureInfectieuse;
          }


          SetCompanionAbilities(companion, caster, new List<Ability> { Ability.Dexterity, Ability.Constitution, Ability.Strength, Ability.Wisdom, Ability.Charisma, Ability.Intelligence });

          break;

        case CustomSkill.BelluaireSpider:

          companion = NwCreature.Create("spidercompagnon", target);

          companion.OnCreatureAttack += RangerUtils.OnAttackSpiderPoisonBite;
          companion.ApplyEffect(EffectDuration.Permanent, Effect.Immunity(ImmunityType.Entangle));

          if (!caster.KnowsFeat((Feat)CustomSkill.BelluaireSpiderWeb))
            caster.AddFeat((Feat)CustomSkill.BelluaireSpiderWeb);

          if (rangerLevel > 4 && !caster.KnowsFeat((Feat)CustomSkill.BelluaireSpiderCocoon))
            caster.AddFeat((Feat)CustomSkill.BelluaireSpiderCocoon);

          if (rangerLevel < 5)
          {
            vfx = VfxType.FnfSummonMonster1;
          }
          else if (rangerLevel < 8)
          {
            vfx = VfxType.FnfSummonMonster2;
            companion.MaxHP = 24;
            companion.BaseAC = 18;
          }
          else if (rangerLevel < 11)
          {
            vfx = VfxType.FnfSummonMonster3;
            companion.MaxHP = 42;
            companion.BaseAC = 19;
          }
          else
          {
            vfx = VfxType.FnfSummonMonster3;
            companion.MaxHP = 60;
            companion.BaseAC = 21;
          }

          SetCompanionAbilities(companion, caster, new List<Ability> { Ability.Dexterity, Ability.Constitution, Ability.Wisdom, Ability.Strength, Ability.Charisma, Ability.Intelligence });

          break;
      }
      
      companion.HP = companion.MaxHP;
      companion.Position = CreaturePlugin.ComputeSafeLocation(companion, target.Position, 10, 0);
      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(vfx));
      companion.SetEventScript(EventScriptType.CreatureOnBlockedByDoor, "nw_ch_ace");
      companion.SetEventScript(EventScriptType.CreatureOnEndCombatRound, "nw_ch_ac3");
      companion.SetEventScript(EventScriptType.CreatureOnDamaged, "nw_ch_ac6");
      companion.SetEventScript(EventScriptType.CreatureOnDeath, "nw_ch_ac7");
      companion.SetEventScript(EventScriptType.CreatureOnDisturbed, "nw_ch_ac8");
      companion.SetEventScript(EventScriptType.CreatureOnHeartbeat, "nw_ch_ac1");
      companion.SetEventScript(EventScriptType.CreatureOnNotice, "nw_ch_ac2");
      companion.SetEventScript(EventScriptType.CreatureOnMeleeAttacked, "nw_ch_ac5");
      companion.SetEventScript(EventScriptType.CreatureOnRested, "nw_ch_aca");
      companion.SetEventScript(EventScriptType.CreatureOnSpawnIn, "nw_ch_acani9");
      companion.SetEventScript(EventScriptType.CreatureOnSpellCastAt, "nw_ch_acb");
      companion.SetEventScript(EventScriptType.CreatureOnUserDefinedEvent, "nw_ch_acd");
      companion.SetEventScript(EventScriptType.CreatureOnDialogue, "nw_ch_ac4");

      return companion;
    }
    private static void SetCompanionAbilities(NwCreature bear, NwCreature caster, List<Ability> orderedAbilities)
    {
      if (PlayerSystem.Players.TryGetValue(caster, out PlayerSystem.Player player)
        && player.learnableSkills.TryGetValue(CustomSkill.AbilityImprovement, out LearnableSkill learnable) && learnable.currentLevel > 0)
      {
        int statGain = learnable.currentLevel * 2;

        foreach (Ability ability in orderedAbilities)
        {
          int currentStat = bear.GetAbilityScore(ability, true);

          if (currentStat + statGain > 20)
          {
            statGain -= (currentStat + statGain) - 20;
            bear.SetsRawAbilityScore(ability, 20);
          }
          else
          {
            bear.SetsRawAbilityScore(ability, (byte)(currentStat + statGain));
            return;
          }
        }
      }
    }
  }
}
