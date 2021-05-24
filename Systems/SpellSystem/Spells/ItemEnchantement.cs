using NWN.Core;
using NWN.Services;
using NWN.API;
using NWN.API.Constants;
using System.Collections.Generic;
using static NWN.Systems.PlayerSystem;
using System;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    private static Dictionary<int, API.ItemProperty[]> enchantementCategories = new Dictionary<int, API.ItemProperty[]>()
    {
      //NIVEAU 0
      {840, new API.ItemProperty[] { API.ItemProperty.Light(IPLightBrightness.Dim, IPLightColor.Blue), API.ItemProperty.Light(IPLightBrightness.Dim, IPLightColor.Green), API.ItemProperty.Light(IPLightBrightness.Dim, IPLightColor.Orange), API.ItemProperty.Light(IPLightBrightness.Dim, IPLightColor.Purple), API.ItemProperty.Light(IPLightBrightness.Dim, IPLightColor.Red), API.ItemProperty.Light(IPLightBrightness.Dim, IPLightColor.White), API.ItemProperty.Light(IPLightBrightness.Dim, IPLightColor.Yellow), API.ItemProperty.Light(IPLightBrightness.Low, IPLightColor.Blue), API.ItemProperty.Light(IPLightBrightness.Low, IPLightColor.Green), API.ItemProperty.Light(IPLightBrightness.Low, IPLightColor.Orange), API.ItemProperty.Light(IPLightBrightness.Low, IPLightColor.Purple), API.ItemProperty.Light(IPLightBrightness.Low, IPLightColor.Red), API.ItemProperty.Light(IPLightBrightness.Low, IPLightColor.White), API.ItemProperty.Light(IPLightBrightness.Low, IPLightColor.Yellow) } },
      {841, new API.ItemProperty[] { API.ItemProperty.ACBonusVsRace(IPRacialType.HumanoidGoblinoid, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Animal, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.HumanoidReptilian, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Vermin, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.HumanoidGoblinoid, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Animal, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.HumanoidReptilian, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Vermin, 1) } },
      {842, new API.ItemProperty[] { API.ItemProperty.MassiveCritical(IPDamageBonus.Plus2) } },
      {843, new API.ItemProperty[] { API.ItemProperty.SkillBonus(Skill.AnimalEmpathy, 1), API.ItemProperty.SkillBonus(Skill.Appraise, 1), API.ItemProperty.SkillBonus(Skill.Bluff, 1), API.ItemProperty.SkillBonus(Skill.Concentration, 1), API.ItemProperty.SkillBonus(Skill.DisableTrap, 1), API.ItemProperty.SkillBonus(Skill.Discipline, 1), API.ItemProperty.SkillBonus(Skill.Heal, 1), API.ItemProperty.SkillBonus(Skill.Hide, 1), API.ItemProperty.SkillBonus(Skill.Intimidate, 1), API.ItemProperty.SkillBonus(Skill.Listen, 1), API.ItemProperty.SkillBonus(Skill.Lore, 1), API.ItemProperty.SkillBonus(Skill.MoveSilently, 1), API.ItemProperty.SkillBonus(Skill.OpenLock, 1), API.ItemProperty.SkillBonus(Skill.Parry, 1), API.ItemProperty.SkillBonus(Skill.Perform, 1), API.ItemProperty.SkillBonus(Skill.Persuade, 1), API.ItemProperty.SkillBonus(Skill.PickPocket, 1), API.ItemProperty.SkillBonus(Skill.Search, 1), API.ItemProperty.SkillBonus(Skill.SetTrap, 1), API.ItemProperty.SkillBonus(Skill.Spellcraft, 1), API.ItemProperty.SkillBonus(Skill.Spot, 1), API.ItemProperty.SkillBonus(Skill.Taunt, 1), API.ItemProperty.SkillBonus(Skill.Taunt, 1), API.ItemProperty.SkillBonus(Skill.Tumble, 1), API.ItemProperty.SkillBonus(Skill.UseMagicDevice, 1) } },
      {844, new API.ItemProperty[] { API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.Fear, 1) } },
      
      //NIVEAU 1
      {845, new API.ItemProperty[] { API.ItemProperty.AbilityBonus(IPAbility.Constitution, 1) } },
      {846, new API.ItemProperty[] { API.ItemProperty.DamageBonus(IPDamageType.Acid, IPDamageBonus.Plus1), API.ItemProperty.DamageBonus(IPDamageType.Fire, IPDamageBonus.Plus1), API.ItemProperty.DamageBonus(IPDamageType.Cold, IPDamageBonus.Plus1), API.ItemProperty.DamageBonus(IPDamageType.Electrical, IPDamageBonus.Plus1), API.ItemProperty.DamageBonus(IPDamageType.Negative, IPDamageBonus.Plus1) , API.ItemProperty.DamageBonus(IPDamageType.Positive, IPDamageBonus.Plus1) } },
      {847, new API.ItemProperty[] { API.ItemProperty.VisualEffect(ItemVisual.Acid), API.ItemProperty.VisualEffect(ItemVisual.Cold), API.ItemProperty.VisualEffect(ItemVisual.Electrical), API.ItemProperty.VisualEffect(ItemVisual.Fire) } },
      {848, new API.ItemProperty[] { API.ItemProperty.Light(IPLightBrightness.Normal, IPLightColor.Blue), API.ItemProperty.Light(IPLightBrightness.Normal, IPLightColor.Green), API.ItemProperty.Light(IPLightBrightness.Normal, IPLightColor.Orange), API.ItemProperty.Light(IPLightBrightness.Normal, IPLightColor.Purple), API.ItemProperty.Light(IPLightBrightness.Normal, IPLightColor.Red), API.ItemProperty.Light(IPLightBrightness.Dim, IPLightColor.White), API.ItemProperty.Light(IPLightBrightness.Normal, IPLightColor.Yellow), API.ItemProperty.Light(IPLightBrightness.Bright, IPLightColor.Blue), API.ItemProperty.Light(IPLightBrightness.Bright, IPLightColor.Green), API.ItemProperty.Light(IPLightBrightness.Bright, IPLightColor.Orange), API.ItemProperty.Light(IPLightBrightness.Bright, IPLightColor.Purple), API.ItemProperty.Light(IPLightBrightness.Bright, IPLightColor.Red), API.ItemProperty.Light(IPLightBrightness.Bright, IPLightColor.White), API.ItemProperty.Light(IPLightBrightness.Bright, IPLightColor.Yellow) } },
      {849, new API.ItemProperty[] { API.ItemProperty.ACBonusVsRace(IPRacialType.HumanoidOrc, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Undead, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Beast, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.HumanoidMonstrous, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.ShapeChanger, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.HumanoidOrc, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Undead, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Beast, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.HumanoidMonstrous, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.ShapeChanger, 1) } },
      //NIVEAU 2
      {850, new API.ItemProperty[] { API.ItemProperty.ACBonusVsRace(IPRacialType.Elemental, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Fey, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Giant, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Construct, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Elemental, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Fey, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Giant, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Construct, 1) } },
      {851, new API.ItemProperty[] { API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.Death, 1), API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.MindAffecting, 1), API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.Acid, 1), API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.Cold, 1), API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.Disease, 1), API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.Divine, 1), API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.Electrical, 1), API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.Fire, 1), API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.Negative, 1), API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.Poison, 1), API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.Positive, 1), API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.Sonic, 1) } },
      {852, new API.ItemProperty[] { API.ItemProperty.ACBonusVsDmgType(IPDamageType.Bludgeoning, 1), API.ItemProperty.ACBonusVsDmgType(IPDamageType.Piercing, 1), API.ItemProperty.ACBonusVsDmgType(IPDamageType.Slashing, 1) } },
      {853, new API.ItemProperty[] { API.ItemProperty.MassiveCritical(IPDamageBonus.Plus1d4) } },
      {854, new API.ItemProperty[] { API.ItemProperty.DamageBonus(IPDamageType.Divine, IPDamageBonus.Plus1), API.ItemProperty.DamageBonus(IPDamageType.Sonic, IPDamageBonus.Plus1), API.ItemProperty.DamageBonus(IPDamageType.Magical, IPDamageBonus.Plus1), API.ItemProperty.DamageBonus(IPDamageType.Piercing, IPDamageBonus.Plus1), API.ItemProperty.DamageBonus(IPDamageType.Slashing, IPDamageBonus.Plus1) , API.ItemProperty.DamageBonus(IPDamageType.Bludgeoning, IPDamageBonus.Plus1) } },
      //NIVEAU 3
      {855, new API.ItemProperty[] { API.ItemProperty.AbilityBonus(IPAbility.Strength, 1), API.ItemProperty.AbilityBonus(IPAbility.Dexterity, 1) } },
      {856, new API.ItemProperty[] { API.ItemProperty.Regeneration(1) } },
      {857, new API.ItemProperty[] { API.ItemProperty.DamageImmunity(IPDamageType.Acid, IPDamageImmunityType.Immunity5Pct), API.ItemProperty.DamageImmunity(IPDamageType.Divine, IPDamageImmunityType.Immunity5Pct), API.ItemProperty.DamageImmunity(IPDamageType.Negative, IPDamageImmunityType.Immunity5Pct), API.ItemProperty.DamageImmunity(IPDamageType.Electrical, IPDamageImmunityType.Immunity5Pct), API.ItemProperty.DamageImmunity(IPDamageType.Positive, IPDamageImmunityType.Immunity5Pct), API.ItemProperty.DamageImmunity(IPDamageType.Fire, IPDamageImmunityType.Immunity5Pct), API.ItemProperty.DamageImmunity(IPDamageType.Cold, IPDamageImmunityType.Immunity5Pct), API.ItemProperty.DamageImmunity(IPDamageType.Magical, IPDamageImmunityType.Immunity5Pct), API.ItemProperty.DamageImmunity(IPDamageType.Sonic, IPDamageImmunityType.Immunity5Pct) } },
      {858, new API.ItemProperty[] { API.ItemProperty.EnhancementBonusVsRace(IPRacialType.HumanoidGoblinoid, 1), API.ItemProperty.EnhancementBonusVsRace(IPRacialType.Animal, 1), API.ItemProperty.EnhancementBonusVsRace(IPRacialType.HumanoidReptilian, 1), API.ItemProperty.EnhancementBonusVsRace(IPRacialType.Vermin, 1) } },
      {859, new API.ItemProperty[] { API.ItemProperty.VisualEffect(ItemVisual.Sonic), API.ItemProperty.VisualEffect(ItemVisual.Holy), API.ItemProperty.VisualEffect(ItemVisual.Evil) } },
      //NIVEAU 4
      {860, new API.ItemProperty[] { API.ItemProperty.ACBonusVsRace(IPRacialType.Halfling, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Human, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.HalfElf, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.HalfOrc, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Elf, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Gnome, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Dwarf, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.MagicalBeast, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Dragon, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Outsider, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Aberration, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Halfling, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Human, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.HalfElf, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.HalfOrc, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Elf, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Gnome, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Dwarf, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.MagicalBeast, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Dragon, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Outsider, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Aberration, 1) } },
      {861, new API.ItemProperty[] { API.ItemProperty.SkillBonus(Skill.AnimalEmpathy, 2), API.ItemProperty.SkillBonus(Skill.Appraise, 2), API.ItemProperty.SkillBonus(Skill.Bluff, 2), API.ItemProperty.SkillBonus(Skill.Concentration, 2), API.ItemProperty.SkillBonus(Skill.DisableTrap, 2), API.ItemProperty.SkillBonus(Skill.Discipline, 2), API.ItemProperty.SkillBonus(Skill.Heal, 2), API.ItemProperty.SkillBonus(Skill.Hide, 2), API.ItemProperty.SkillBonus(Skill.Intimidate, 2), API.ItemProperty.SkillBonus(Skill.Listen, 2), API.ItemProperty.SkillBonus(Skill.Lore, 2), API.ItemProperty.SkillBonus(Skill.MoveSilently, 2), API.ItemProperty.SkillBonus(Skill.OpenLock, 2), API.ItemProperty.SkillBonus(Skill.Parry, 2), API.ItemProperty.SkillBonus(Skill.Perform, 2), API.ItemProperty.SkillBonus(Skill.Persuade, 2), API.ItemProperty.SkillBonus(Skill.PickPocket, 2), API.ItemProperty.SkillBonus(Skill.Search, 2), API.ItemProperty.SkillBonus(Skill.SetTrap, 2), API.ItemProperty.SkillBonus(Skill.Spellcraft, 2), API.ItemProperty.SkillBonus(Skill.Spot, 2), API.ItemProperty.SkillBonus(Skill.Taunt, 2), API.ItemProperty.SkillBonus(Skill.Taunt, 2), API.ItemProperty.SkillBonus(Skill.Tumble, 2), API.ItemProperty.SkillBonus(Skill.UseMagicDevice, 2) } },
      {862, new API.ItemProperty[] { API.ItemProperty.EnhancementBonusVsRace(IPRacialType.HumanoidOrc, 1), API.ItemProperty.EnhancementBonusVsRace(IPRacialType.Undead, 1), API.ItemProperty.EnhancementBonusVsRace(IPRacialType.Beast, 1), API.ItemProperty.EnhancementBonusVsRace(IPRacialType.HumanoidMonstrous, 1), API.ItemProperty.EnhancementBonusVsRace(IPRacialType.ShapeChanger, 1) } },
      //NIVEAU 5
      {863, new API.ItemProperty[] { API.ItemProperty.ACBonusVsAlign(IPAlignmentGroup.Good, 1), API.ItemProperty.ACBonusVsAlign(IPAlignmentGroup.Chaotic, 1), API.ItemProperty.ACBonusVsAlign(IPAlignmentGroup.Neutral, 1), API.ItemProperty.ACBonusVsAlign(IPAlignmentGroup.Evil, 1), API.ItemProperty.ACBonusVsAlign(IPAlignmentGroup.Lawful, 1), API.ItemProperty.AttackBonusVsAlign(IPAlignmentGroup.Good, 1), API.ItemProperty.AttackBonusVsAlign(IPAlignmentGroup.Chaotic, 1), API.ItemProperty.AttackBonusVsAlign(IPAlignmentGroup.Neutral, 1), API.ItemProperty.AttackBonusVsAlign(IPAlignmentGroup.Evil, 1), API.ItemProperty.AttackBonusVsAlign(IPAlignmentGroup.Lawful, 1) } },
      {864, new API.ItemProperty[] { API.ItemProperty.BonusSavingThrow(IPSaveBaseType.Reflex, 1), API.ItemProperty.BonusSavingThrow(IPSaveBaseType.Fortitude, 1), API.ItemProperty.BonusSavingThrow(IPSaveBaseType.Will, 1) } },
      {865, new API.ItemProperty[] { API.ItemProperty.AbilityBonus(IPAbility.Intelligence, 1), API.ItemProperty.AbilityBonus(IPAbility.Wisdom, 1), API.ItemProperty.AbilityBonus(IPAbility.Charisma, 1) } },
      {866, new API.ItemProperty[] { API.ItemProperty.EnhancementBonusVsRace(IPRacialType.Elemental, 1), API.ItemProperty.EnhancementBonusVsRace(IPRacialType.Fey, 1), API.ItemProperty.EnhancementBonusVsRace(IPRacialType.Giant, 1), API.ItemProperty.EnhancementBonusVsRace(IPRacialType.Construct, 1) } },
      //NIVEAU 6
      {867, new API.ItemProperty[] { API.ItemProperty.DamageBonus(IPDamageType.Acid, IPDamageBonus.Plus1d4), API.ItemProperty.DamageBonus(IPDamageType.Fire, IPDamageBonus.Plus1d4), API.ItemProperty.DamageBonus(IPDamageType.Cold, IPDamageBonus.Plus1d4), API.ItemProperty.DamageBonus(IPDamageType.Electrical, IPDamageBonus.Plus1d4), API.ItemProperty.DamageBonus(IPDamageType.Negative, IPDamageBonus.Plus1d4) , API.ItemProperty.DamageBonus(IPDamageType.Positive, IPDamageBonus.Plus1d4) } },
      {868, new API.ItemProperty[] { API.ItemProperty.MassiveCritical(IPDamageBonus.Plus1d8) } },
      //NIVEAU 7
      {869, new API.ItemProperty[] { API.ItemProperty.EnhancementBonusVsRace(IPRacialType.Halfling, 1), API.ItemProperty.EnhancementBonusVsRace(IPRacialType.Human, 1), API.ItemProperty.EnhancementBonusVsRace(IPRacialType.HalfElf, 1), API.ItemProperty.EnhancementBonusVsRace(IPRacialType.HalfOrc, 1), API.ItemProperty.EnhancementBonusVsRace(IPRacialType.Elf, 1), API.ItemProperty.EnhancementBonusVsRace(IPRacialType.Gnome, 1), API.ItemProperty.EnhancementBonusVsRace(IPRacialType.Dwarf, 1), API.ItemProperty.EnhancementBonusVsRace(IPRacialType.MagicalBeast, 1), API.ItemProperty.EnhancementBonusVsRace(IPRacialType.Dragon, 1), API.ItemProperty.EnhancementBonusVsRace(IPRacialType.Outsider, 1), API.ItemProperty.EnhancementBonusVsRace(IPRacialType.Aberration, 1) } },
      {870, new API.ItemProperty[] { API.ItemProperty.DamageImmunity(IPDamageType.Acid, IPDamageImmunityType.Immunity10Pct), API.ItemProperty.DamageImmunity(IPDamageType.Divine, IPDamageImmunityType.Immunity10Pct), API.ItemProperty.DamageImmunity(IPDamageType.Negative, IPDamageImmunityType.Immunity10Pct), API.ItemProperty.DamageImmunity(IPDamageType.Electrical, IPDamageImmunityType.Immunity10Pct), API.ItemProperty.DamageImmunity(IPDamageType.Positive, IPDamageImmunityType.Immunity10Pct), API.ItemProperty.DamageImmunity(IPDamageType.Fire, IPDamageImmunityType.Immunity10Pct), API.ItemProperty.DamageImmunity(IPDamageType.Cold, IPDamageImmunityType.Immunity10Pct), API.ItemProperty.DamageImmunity(IPDamageType.Magical, IPDamageImmunityType.Immunity10Pct), API.ItemProperty.DamageImmunity(IPDamageType.Sonic, IPDamageImmunityType.Immunity10Pct) } },
      {871, new API.ItemProperty[] { API.ItemProperty.DamageImmunity(IPDamageType.Piercing, IPDamageImmunityType.Immunity5Pct), API.ItemProperty.DamageImmunity(IPDamageType.Slashing, IPDamageImmunityType.Immunity5Pct), API.ItemProperty.DamageImmunity(IPDamageType.Bludgeoning, IPDamageImmunityType.Immunity5Pct) } },
      {872, new API.ItemProperty[] { API.ItemProperty.BonusSpellResistance(IPSpellResistanceBonus.Plus10) } },
      //NIVEAU 8
      {873, new API.ItemProperty[] { API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.Universal, 1) } },
      {874, new API.ItemProperty[] { API.ItemProperty.ACBonus(1) } },
      {875, new API.ItemProperty[] { API.ItemProperty.AttackBonus(1) } },
      {876, new API.ItemProperty[] { API.ItemProperty.DamageBonus(IPDamageType.Divine, IPDamageBonus.Plus1d4), API.ItemProperty.DamageBonus(IPDamageType.Sonic, IPDamageBonus.Plus1d4), API.ItemProperty.DamageBonus(IPDamageType.Magical, IPDamageBonus.Plus1d4), API.ItemProperty.DamageBonus(IPDamageType.Piercing, IPDamageBonus.Plus1d4), API.ItemProperty.DamageBonus(IPDamageType.Slashing, IPDamageBonus.Plus1d4) , API.ItemProperty.DamageBonus(IPDamageType.Bludgeoning, IPDamageBonus.Plus1d4) } },
      {877, new API.ItemProperty[] { API.ItemProperty.EnhancementBonusVsAlign(IPAlignmentGroup.Good, 1), API.ItemProperty.EnhancementBonusVsAlign(IPAlignmentGroup.Chaotic, 1), API.ItemProperty.EnhancementBonusVsAlign(IPAlignmentGroup.Neutral, 1), API.ItemProperty.EnhancementBonusVsAlign(IPAlignmentGroup.Evil, 1), API.ItemProperty.EnhancementBonusVsAlign(IPAlignmentGroup.Lawful, 1) } },
      //NIVEAU 9
      {878, new API.ItemProperty[] { API.ItemProperty.DamageImmunity(IPDamageType.Piercing, IPDamageImmunityType.Immunity10Pct), API.ItemProperty.DamageImmunity(IPDamageType.Slashing, IPDamageImmunityType.Immunity10Pct), API.ItemProperty.DamageImmunity(IPDamageType.Bludgeoning, IPDamageImmunityType.Immunity10Pct) } },
      {879, new API.ItemProperty[] { API.ItemProperty.VampiricRegeneration(1) } },
      {880, new API.ItemProperty[] { API.ItemProperty.EnhancementBonus(1) } },
      {881, new API.ItemProperty[] { API.ItemProperty.Keen() } },
      {882, new API.ItemProperty[] { API.ItemProperty.Haste() } },
    };

    [ScriptHandler("on_ench_cast")]
    private void HandleItemEnchantement(CallInfo callInfo)
    {
      NwItem oTarget = NWScript.GetSpellTargetObject().ToNwObject<NwItem>();

      if (!(callInfo.ObjectSelf is NwCreature { IsPlayerControlled: true } oCaster) || oTarget == null || !Players.TryGetValue(oCaster.ControllingPlayer.LoginCreature, out Player player))
        return;

      if (!player.craftJob.CanStartJob(player.oid, null, Craft.Job.JobType.Enchantement))
        return;

      if(oTarget.GetLocalVariable<int>("_AVAILABLE_ENCHANTEMENT_SLOT").HasNothing)
      {
        player.oid.SendServerMessage($"{oTarget.Name} ne dispose d'aucun emplacement d'enchantement disponible !");
        return;
      }

      int spellId = NWScript.GetSpellId();

      if (enchantementCategories.ContainsKey(spellId))
        DrawEnchantementChoicePage(player, oTarget.Name, spellId, oTarget);
      else
      {
        player.oid.SendServerMessage("HRP - Propriétés de cet enchantement incorrectement définies. L'erreur a été remontée au staff");
        Utils.LogMessageToDMs($"ENCHANTEMENT - {spellId} - ItemProperties non présentes dans le dictionnaire.");
        return;
      }
    }
    private void DrawEnchantementChoicePage(Player player, string itemName, int spellId, NwItem oItem)
    {
      if (enchantementCategories[spellId].Length == 1)
      {
        HandleEnchantementChoice(player, enchantementCategories[spellId][0], spellId, oItem);
        return;
      }
      
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"Quel enchantement souhaitez-vous appliquer sur votre {itemName} ?"
      };

      foreach (API.ItemProperty ip in enchantementCategories[spellId])
        player.menu.choices.Add(($"{NWScript.GetStringByStrRef(Int32.Parse(NWScript.Get2DAString("itempropdef", "Name", NWScript.GetItemPropertyType(ip))))} - " +
          $"{NWScript.GetStringByStrRef(Int32.Parse(NWScript.Get2DAString(NWScript.Get2DAString("itempropdef", "SubTypeResRef", NWScript.GetItemPropertyType(ip)), "Name", NWScript.GetItemPropertySubType(ip))))}", () => HandleEnchantementChoice(player, ip, spellId, oItem)));

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }

    private static void HandleEnchantementChoice(Player player, API.ItemProperty ip, int spellId, NwItem oItem)
    {
      //player.oid.SendServerMessage($"ip string : {$"{spellId}_{(int)ip.PropertyType}_{ip.SubType}_{ip.CostTable}_{ip.CostTableValue}"}");

      player.craftJob.Start(Craft.Job.JobType.Enchantement, null, player, null, oItem, $"{spellId}_{(int)ip.PropertyType}_{ip.SubType}_{ip.CostTable}_{ip.CostTableValue}");
      player.oid.ControlledCreature.ApplyEffect(EffectDuration.Instant, API.Effect.VisualEffect(VfxType.ImpSuperHeroism));

      player.menu.Close();
    }
  }
}
