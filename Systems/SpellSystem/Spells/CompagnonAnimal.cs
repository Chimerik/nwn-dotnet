using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void CompagnonAnimal(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;

      if (caster.GetAssociate(AssociateType.AnimalCompanion) is not null)
      {
        caster.UnsummonAnimalCompanion();
        caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireFurieBestiale, 0);
        return;
      }

      var rangerClass = caster.GetClassInfo((ClassType)CustomClass.Ranger);
      byte remainingSlots = 0;

      for (byte i = 1; i < 6; i++)
      {
        remainingSlots = rangerClass.GetRemainingSpellSlots(i);

        if (remainingSlots > 0)
        {
          rangerClass.SetRemainingSpellSlots(i, (byte)(remainingSlots - 1));
          break;
        }
      }

      if(remainingSlots < 1)
      {
        caster.LoginPlayer?.SendServerMessage("Emplacement de sort de Rôdeur requis", ColorConstants.Red);
        return;
      }

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      var companion = CreateAnimalCompanion(spell.Id, caster);

      CreaturePlugin.AddAssociate(caster, companion, (int)AssociateType.AnimalCompanion);
      //caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).Value = companion;

      for (int i = 268; i < 287; i++)
        if (caster.KnowsFeat((Feat)i))
          companion.AddFeat((Feat)i);

      if (caster.KnowsFeat((Feat)CustomSkill.BelluaireFurieBestiale))
        caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireFurieBestiale, 100);
    }
    private static NwCreature CreateAnimalCompanion(int spellId, NwCreature caster)
    {
      NwCreature companion = CreatureUtils.SummonAssociate(caster, AssociateType.AnimalCompanion, AnimalCompanion2da.companionTable.FirstOrDefault(f => f.spellId == spellId).resRef);
      byte rangerLevel = caster.GetClassInfo(ClassType.Ranger).Level;
      NwItem weapon; 

      switch (spellId)
      {
        default:

          weapon = companion.GetItemInSlot(InventorySlot.CreatureLeftWeapon);
          weapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Slashing, IPDamageBonus.Plus1d8), EffectDuration.Permanent);
          weapon.GetObjectVariable<LocalVariableInt>(ItemConfig.IsFinesseWeaponVariable).Value = 1;

          if (!caster.KnowsFeat((Feat)CustomSkill.BelluaireRugissementProvoquant))
            caster.AddFeat((Feat)CustomSkill.BelluaireRugissementProvoquant);

          caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireRugissementProvoquant,
            (byte)(caster.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.BelluaireRugissementProvoquant) ? 0 : 100));

          if (!caster.KnowsFeat((Feat)CustomSkill.BelluairePatteMielleuse) && rangerLevel > 4)
            caster.AddFeat((Feat)CustomSkill.BelluairePatteMielleuse);
          else
            caster.SetFeatRemainingUses((Feat)CustomSkill.BelluairePatteMielleuse, 100);

          if (rangerLevel < 8)
          {
            companion.MaxHP = 39;
            companion.BaseAC += 4;
          }
          else if (rangerLevel < 11)
          {
            companion.MaxHP = 69;
            companion.BaseAC += 5;
          }
          else
          {
            companion.MaxHP = 99;
            companion.BaseAC += 7;
          }

          SetCompanionAbilities(companion, caster, new List<Ability> { Ability.Strength, Ability.Constitution, Ability.Dexterity, Ability.Wisdom, Ability.Charisma, Ability.Intelligence });

          break;

        case CustomSpell.BelluaireSanglier:

          weapon = companion.GetItemInSlot(InventorySlot.CreatureLeftWeapon);
          weapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Slashing, IPDamageBonus.Plus1d6), EffectDuration.Permanent);
          weapon.GetObjectVariable<LocalVariableInt>(ItemConfig.IsFinesseWeaponVariable).Value = 1;

          if (!caster.KnowsFeat((Feat)CustomSkill.BelluaireChargeSanglier))
            caster.AddFeat((Feat)CustomSkill.BelluaireChargeSanglier);

          caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireChargeSanglier,
            (byte)(caster.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.BelluaireChargeSanglier) ? 0 : 100));

          if (!caster.KnowsFeat((Feat)CustomSkill.BelluaireRageSanglier) && rangerLevel > 4)
            caster.AddFeat((Feat)CustomSkill.BelluaireRageSanglier);
          else
            caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireRageSanglier, 100);

          if (rangerLevel < 8)
          {
            companion.MaxHP = 27;
            companion.BaseAC += 3;
          }
          else if (rangerLevel < 11)
          {
            companion.MaxHP = 51;
            companion.BaseAC += 4;
          }
          else
          {
            companion.MaxHP = 85;
            companion.BaseAC += 6;
          }

          SetCompanionAbilities(companion, caster, new List<Ability> { Ability.Strength, Ability.Constitution, Ability.Dexterity, Ability.Wisdom, Ability.Charisma, Ability.Intelligence });

          break;

        case CustomSpell.BelluaireCorbeau:

          weapon = companion.GetItemInSlot(InventorySlot.CreatureLeftWeapon);
          weapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Piercing, IPDamageBonus.Plus2d4), EffectDuration.Permanent);
          weapon.GetObjectVariable<LocalVariableInt>(ItemConfig.IsFinesseWeaponVariable).Value = 1;

          if (!caster.KnowsFeat((Feat)CustomSkill.BelluaireCorbeauAveuglement))
            caster.AddFeat((Feat)CustomSkill.BelluaireCorbeauAveuglement);

          if (!caster.KnowsFeat((Feat)CustomSkill.BelluaireCorbeauMauvaisAugure) && rangerLevel > 4)
            caster.AddFeat((Feat)CustomSkill.BelluaireCorbeauMauvaisAugure);
          else
            caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireCorbeauMauvaisAugure, 100);

          if (rangerLevel < 8)
          {
            weapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Piercing, IPDamageBonus.Plus1d4), EffectDuration.Permanent);
            companion.MaxHP = 21;
            companion.BaseAC += 3;
          }
          else if (rangerLevel < 11)
          {
            companion.MaxHP = 32;
            companion.BaseAC += 4;
          }
          else
          {
            weapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Piercing, IPDamageBonus.Plus1d6), EffectDuration.Permanent);
            companion.MaxHP = 44;
            companion.BaseAC += 5;
          }

          SetCompanionAbilities(companion, caster, new List<Ability> { Ability.Dexterity, Ability.Constitution, Ability.Wisdom, Ability.Charisma, Ability.Intelligence, Ability.Strength });

          break;

        case CustomSpell.BelluaireLoup:

          weapon = companion.GetItemInSlot(InventorySlot.CreatureLeftWeapon);
          weapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Piercing, IPDamageBonus.Plus2d4), EffectDuration.Permanent);
          weapon.GetObjectVariable<LocalVariableInt>(ItemConfig.IsFinesseWeaponVariable).Value = 1;

          if (!caster.KnowsFeat((Feat)CustomSkill.BelluaireLoupMorsurePlongeante))
            companion.OnCreatureAttack += RangerUtils.OnAttackMorsurePlongeante;

          if (!caster.KnowsFeat((Feat)CustomSkill.BelluaireLoupEffetDeMeute))
            caster.AddFeat((Feat)CustomSkill.BelluaireLoupEffetDeMeute);

          if (rangerLevel > 5)
          {
            if (!caster.KnowsFeat((Feat)CustomSkill.BelluaireLoupMorsureInfectieuse))
              caster.AddFeat((Feat)CustomSkill.BelluaireLoupMorsureInfectieuse);

            companion.OnCreatureDamage += RangerUtils.OnDamageLoupMorsureInfectieuse;
          }

          if (rangerLevel < 8)
          {
            companion.MaxHP = 31;
            companion.BaseAC += 3;
          }
          else if (rangerLevel < 11)
          {
            companion.MaxHP = 61;
            companion.BaseAC += 5;
          }
          else
          {
            companion.MaxHP = 91;
            companion.BaseAC += 7;
          }


          SetCompanionAbilities(companion, caster, new List<Ability> { Ability.Dexterity, Ability.Constitution, Ability.Strength, Ability.Wisdom, Ability.Charisma, Ability.Intelligence });

          break;

        case CustomSpell.BelluaireAraignee:

          companion.ApplyEffect(EffectDuration.Permanent, Effect.SpellImmunity(Spell.Web));

          weapon = companion.GetItemInSlot(InventorySlot.CreatureLeftWeapon);
          weapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Slashing, IPDamageBonus.Plus1d8), EffectDuration.Permanent);
          weapon.GetObjectVariable<LocalVariableInt>(ItemConfig.IsFinesseWeaponVariable).Value = 1;

          companion.OnCreatureAttack += RangerUtils.OnAttackSpiderPoisonBite;
          companion.ApplyEffect(EffectDuration.Permanent, Effect.Immunity(ImmunityType.Entangle));

          if (!caster.KnowsFeat((Feat)CustomSkill.BelluaireSpiderWeb))
            caster.AddFeat((Feat)CustomSkill.BelluaireSpiderWeb);

          caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireSpiderWeb, 100);

          if (rangerLevel > 4 && !caster.KnowsFeat((Feat)CustomSkill.BelluaireSpiderCocoon))
            caster.AddFeat((Feat)CustomSkill.BelluaireSpiderCocoon);

          caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireSpiderCocoon, 100);

          if (rangerLevel < 8)
          {
            companion.MaxHP = 24;
            companion.BaseAC += 4;
          }
          else if (rangerLevel < 11)
          {
            companion.MaxHP = 42;
            companion.BaseAC += 5;
          }
          else
          {
            companion.MaxHP = 60;
            companion.BaseAC += 7;
          }

          SetCompanionAbilities(companion, caster, new List<Ability> { Ability.Dexterity, Ability.Constitution, Ability.Wisdom, Ability.Strength, Ability.Charisma, Ability.Intelligence });

          break;
      }

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
