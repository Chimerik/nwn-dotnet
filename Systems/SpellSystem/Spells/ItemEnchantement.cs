using System.Collections.Generic;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public Dictionary<int, ItemProperty[]> enchantementCategories = new Dictionary<int, ItemProperty[]>()
    {
      //NIVEAU 0
      {840, new ItemProperty[] { ItemProperty.Light(IPLightBrightness.Dim, IPLightColor.Blue), ItemProperty.Light(IPLightBrightness.Dim, IPLightColor.Green), ItemProperty.Light(IPLightBrightness.Dim, IPLightColor.Orange), ItemProperty.Light(IPLightBrightness.Dim, IPLightColor.Purple), ItemProperty.Light(IPLightBrightness.Dim, IPLightColor.Red), ItemProperty.Light(IPLightBrightness.Dim, IPLightColor.White), ItemProperty.Light(IPLightBrightness.Dim, IPLightColor.Yellow), ItemProperty.Light(IPLightBrightness.Low, IPLightColor.Blue), ItemProperty.Light(IPLightBrightness.Low, IPLightColor.Green), ItemProperty.Light(IPLightBrightness.Low, IPLightColor.Orange), ItemProperty.Light(IPLightBrightness.Low, IPLightColor.Purple), ItemProperty.Light(IPLightBrightness.Low, IPLightColor.Red), ItemProperty.Light(IPLightBrightness.Low, IPLightColor.White), ItemProperty.Light(IPLightBrightness.Low, IPLightColor.Yellow) } },
      {841, new ItemProperty[] { ItemProperty.ACBonusVsRace(IPRacialType.HumanoidGoblinoid, 4), ItemProperty.ACBonusVsRace(IPRacialType.Animal, 4), ItemProperty.ACBonusVsRace(IPRacialType.HumanoidReptilian, 4), ItemProperty.ACBonusVsRace(IPRacialType.Vermin, 4), ItemProperty.AttackBonusVsRace(IPRacialType.HumanoidGoblinoid, 4), ItemProperty.AttackBonusVsRace(IPRacialType.Animal, 4), ItemProperty.AttackBonusVsRace(IPRacialType.HumanoidReptilian, 4), ItemProperty.AttackBonusVsRace(IPRacialType.Vermin, 4) } },
      {842, new ItemProperty[] { ItemProperty.MassiveCritical(IPDamageBonus.Plus2) } },
      {843, new ItemProperty[] { ItemProperty.SkillBonus(Skill.AnimalEmpathy, 1), ItemProperty.SkillBonus(Skill.Appraise, 1), ItemProperty.SkillBonus(Skill.Bluff, 1), ItemProperty.SkillBonus(Skill.Concentration, 1), ItemProperty.SkillBonus(Skill.DisableTrap, 1), ItemProperty.SkillBonus(Skill.Discipline, 1), ItemProperty.SkillBonus(Skill.Heal, 1), ItemProperty.SkillBonus(Skill.Hide, 1), ItemProperty.SkillBonus(Skill.Intimidate, 1), ItemProperty.SkillBonus(Skill.Listen, 1), ItemProperty.SkillBonus(Skill.Lore, 1), ItemProperty.SkillBonus(Skill.MoveSilently, 1), ItemProperty.SkillBonus(Skill.OpenLock, 1), ItemProperty.SkillBonus(Skill.Parry, 1), ItemProperty.SkillBonus(Skill.Perform, 1), ItemProperty.SkillBonus(Skill.Persuade, 1), ItemProperty.SkillBonus(Skill.PickPocket, 1), ItemProperty.SkillBonus(Skill.Search, 1), ItemProperty.SkillBonus(Skill.SetTrap, 1), ItemProperty.SkillBonus(Skill.Spellcraft, 1), ItemProperty.SkillBonus(Skill.Spot, 1), ItemProperty.SkillBonus(Skill.Taunt, 1), ItemProperty.SkillBonus(Skill.Taunt, 1), ItemProperty.SkillBonus(Skill.Tumble, 1), ItemProperty.SkillBonus(Skill.UseMagicDevice, 1) } },
      {844, new ItemProperty[] { ItemProperty.BonusSavingThrowVsX(IPSaveVs.Fear, 1) } },
      
      //NIVEAU 1
      {845, new ItemProperty[] { ItemProperty.AbilityBonus(IPAbility.Constitution, 1) } },
      {846, new ItemProperty[] { ItemProperty.DamageBonus(IPDamageType.Acid, IPDamageBonus.Plus1), ItemProperty.DamageBonus(IPDamageType.Fire, IPDamageBonus.Plus1), ItemProperty.DamageBonus(IPDamageType.Cold, IPDamageBonus.Plus1), ItemProperty.DamageBonus(IPDamageType.Electrical, IPDamageBonus.Plus1), ItemProperty.DamageBonus(IPDamageType.Negative, IPDamageBonus.Plus1) , ItemProperty.DamageBonus(IPDamageType.Positive, IPDamageBonus.Plus1) } },
      {847, new ItemProperty[] { ItemProperty.VisualEffect(ItemVisual.Acid), ItemProperty.VisualEffect(ItemVisual.Cold), ItemProperty.VisualEffect(ItemVisual.Electrical), ItemProperty.VisualEffect(ItemVisual.Fire) } },
      {848, new ItemProperty[] { ItemProperty.Light(IPLightBrightness.Normal, IPLightColor.Blue), ItemProperty.Light(IPLightBrightness.Normal, IPLightColor.Green), ItemProperty.Light(IPLightBrightness.Normal, IPLightColor.Orange), ItemProperty.Light(IPLightBrightness.Normal, IPLightColor.Purple), ItemProperty.Light(IPLightBrightness.Normal, IPLightColor.Red), ItemProperty.Light(IPLightBrightness.Dim, IPLightColor.White), ItemProperty.Light(IPLightBrightness.Normal, IPLightColor.Yellow), ItemProperty.Light(IPLightBrightness.Bright, IPLightColor.Blue), ItemProperty.Light(IPLightBrightness.Bright, IPLightColor.Green), ItemProperty.Light(IPLightBrightness.Bright, IPLightColor.Orange), ItemProperty.Light(IPLightBrightness.Bright, IPLightColor.Purple), ItemProperty.Light(IPLightBrightness.Bright, IPLightColor.Red), ItemProperty.Light(IPLightBrightness.Bright, IPLightColor.White), ItemProperty.Light(IPLightBrightness.Bright, IPLightColor.Yellow) } },
      {849, new ItemProperty[] { ItemProperty.ACBonusVsRace(IPRacialType.HumanoidOrc, 4), ItemProperty.ACBonusVsRace(IPRacialType.Undead, 4), ItemProperty.ACBonusVsRace(IPRacialType.Beast, 4), ItemProperty.ACBonusVsRace(IPRacialType.HumanoidMonstrous, 4), ItemProperty.ACBonusVsRace(IPRacialType.ShapeChanger, 4), ItemProperty.AttackBonusVsRace(IPRacialType.HumanoidOrc, 4), ItemProperty.AttackBonusVsRace(IPRacialType.Undead, 4), ItemProperty.AttackBonusVsRace(IPRacialType.Beast, 4), ItemProperty.AttackBonusVsRace(IPRacialType.HumanoidMonstrous, 4), ItemProperty.AttackBonusVsRace(IPRacialType.ShapeChanger, 4) } },
      //NIVEAU 2
      {850, new ItemProperty[] { ItemProperty.ACBonusVsRace(IPRacialType.Elemental, 1), ItemProperty.ACBonusVsRace(IPRacialType.Fey, 1), ItemProperty.ACBonusVsRace(IPRacialType.Giant, 1), ItemProperty.ACBonusVsRace(IPRacialType.Construct, 1), ItemProperty.AttackBonusVsRace(IPRacialType.Elemental, 1), ItemProperty.AttackBonusVsRace(IPRacialType.Fey, 1), ItemProperty.AttackBonusVsRace(IPRacialType.Giant, 1), ItemProperty.AttackBonusVsRace(IPRacialType.Construct, 1) } },
      {851, new ItemProperty[] { ItemProperty.BonusSavingThrowVsX(IPSaveVs.Death, 1), ItemProperty.BonusSavingThrowVsX(IPSaveVs.MindAffecting, 1), ItemProperty.BonusSavingThrowVsX(IPSaveVs.Acid, 1), ItemProperty.BonusSavingThrowVsX(IPSaveVs.Cold, 1), ItemProperty.BonusSavingThrowVsX(IPSaveVs.Disease, 1), ItemProperty.BonusSavingThrowVsX(IPSaveVs.Divine, 1), ItemProperty.BonusSavingThrowVsX(IPSaveVs.Electrical, 1), ItemProperty.BonusSavingThrowVsX(IPSaveVs.Fire, 1), ItemProperty.BonusSavingThrowVsX(IPSaveVs.Negative, 1), ItemProperty.BonusSavingThrowVsX(IPSaveVs.Poison, 1), ItemProperty.BonusSavingThrowVsX(IPSaveVs.Positive, 1), ItemProperty.BonusSavingThrowVsX(IPSaveVs.Sonic, 1) } },
      {852, new ItemProperty[] { ItemProperty.ACBonusVsDmgType(IPDamageType.Bludgeoning, 4), ItemProperty.ACBonusVsDmgType(IPDamageType.Piercing, 4), ItemProperty.ACBonusVsDmgType(IPDamageType.Slashing, 4) } },
      {853, new ItemProperty[] { ItemProperty.MassiveCritical(IPDamageBonus.Plus1d4) } },
      {854, new ItemProperty[] { ItemProperty.DamageBonus(IPDamageType.Divine, IPDamageBonus.Plus1), ItemProperty.DamageBonus(IPDamageType.Sonic, IPDamageBonus.Plus1), ItemProperty.DamageBonus(IPDamageType.Magical, IPDamageBonus.Plus1), ItemProperty.DamageBonus(IPDamageType.Piercing, IPDamageBonus.Plus1), ItemProperty.DamageBonus(IPDamageType.Slashing, IPDamageBonus.Plus1) , ItemProperty.DamageBonus(IPDamageType.Bludgeoning, IPDamageBonus.Plus1) } },
      //NIVEAU 3
      {855, new ItemProperty[] { ItemProperty.AbilityBonus(IPAbility.Strength, 1), ItemProperty.AbilityBonus(IPAbility.Dexterity, 1) } },
      {856, new ItemProperty[] { ItemProperty.Regeneration(1) } },
      {857, new ItemProperty[] { ItemProperty.ACBonusVsDmgType((IPDamageType)14, 2) } }, // VS ELEMENTAL
      {858, new ItemProperty[] { ItemProperty.EnhancementBonusVsRace(IPRacialType.HumanoidGoblinoid, 4), ItemProperty.EnhancementBonusVsRace(IPRacialType.Animal, 4), ItemProperty.EnhancementBonusVsRace(IPRacialType.HumanoidReptilian, 4), ItemProperty.EnhancementBonusVsRace(IPRacialType.Vermin, 4) } },
      {859, new ItemProperty[] { ItemProperty.VisualEffect(ItemVisual.Sonic), ItemProperty.VisualEffect(ItemVisual.Holy), ItemProperty.VisualEffect(ItemVisual.Evil) } },
      //NIVEAU 4
      {860, new ItemProperty[] { ItemProperty.ACBonusVsRace(IPRacialType.Halfling, 4), ItemProperty.ACBonusVsRace(IPRacialType.Human, 4), ItemProperty.ACBonusVsRace(IPRacialType.HalfElf, 4), ItemProperty.ACBonusVsRace(IPRacialType.HalfOrc, 4), ItemProperty.ACBonusVsRace(IPRacialType.Elf, 4), ItemProperty.ACBonusVsRace(IPRacialType.Gnome, 4), ItemProperty.ACBonusVsRace(IPRacialType.Dwarf, 4), ItemProperty.ACBonusVsRace(IPRacialType.MagicalBeast, 4), ItemProperty.ACBonusVsRace(IPRacialType.Dragon, 4), ItemProperty.ACBonusVsRace(IPRacialType.Outsider, 4), ItemProperty.ACBonusVsRace(IPRacialType.Aberration, 4), ItemProperty.AttackBonusVsRace(IPRacialType.Halfling, 4), ItemProperty.AttackBonusVsRace(IPRacialType.Human, 4), ItemProperty.AttackBonusVsRace(IPRacialType.HalfElf, 4), ItemProperty.AttackBonusVsRace(IPRacialType.HalfOrc, 4), ItemProperty.AttackBonusVsRace(IPRacialType.Elf, 4), ItemProperty.AttackBonusVsRace(IPRacialType.Gnome, 4), ItemProperty.AttackBonusVsRace(IPRacialType.Dwarf, 4), ItemProperty.AttackBonusVsRace(IPRacialType.MagicalBeast, 4), ItemProperty.AttackBonusVsRace(IPRacialType.Dragon, 4), ItemProperty.AttackBonusVsRace(IPRacialType.Outsider, 4), ItemProperty.AttackBonusVsRace(IPRacialType.Aberration, 4) } },
      {861, new ItemProperty[] { ItemProperty.SkillBonus(Skill.AnimalEmpathy, 2), ItemProperty.SkillBonus(Skill.Appraise, 2), ItemProperty.SkillBonus(Skill.Bluff, 2), ItemProperty.SkillBonus(Skill.Concentration, 2), ItemProperty.SkillBonus(Skill.DisableTrap, 2), ItemProperty.SkillBonus(Skill.Discipline, 2), ItemProperty.SkillBonus(Skill.Heal, 2), ItemProperty.SkillBonus(Skill.Hide, 2), ItemProperty.SkillBonus(Skill.Intimidate, 2), ItemProperty.SkillBonus(Skill.Listen, 2), ItemProperty.SkillBonus(Skill.Lore, 2), ItemProperty.SkillBonus(Skill.MoveSilently, 2), ItemProperty.SkillBonus(Skill.OpenLock, 2), ItemProperty.SkillBonus(Skill.Parry, 2), ItemProperty.SkillBonus(Skill.Perform, 2), ItemProperty.SkillBonus(Skill.Persuade, 2), ItemProperty.SkillBonus(Skill.PickPocket, 2), ItemProperty.SkillBonus(Skill.Search, 2), ItemProperty.SkillBonus(Skill.SetTrap, 2), ItemProperty.SkillBonus(Skill.Spellcraft, 2), ItemProperty.SkillBonus(Skill.Spot, 2), ItemProperty.SkillBonus(Skill.Taunt, 2), ItemProperty.SkillBonus(Skill.Taunt, 2), ItemProperty.SkillBonus(Skill.Tumble, 2), ItemProperty.SkillBonus(Skill.UseMagicDevice, 2) } },
      {862, new ItemProperty[] { ItemProperty.EnhancementBonusVsRace(IPRacialType.HumanoidOrc, 4), ItemProperty.EnhancementBonusVsRace(IPRacialType.Undead, 4), ItemProperty.EnhancementBonusVsRace(IPRacialType.Beast, 4), ItemProperty.EnhancementBonusVsRace(IPRacialType.HumanoidMonstrous, 4), ItemProperty.EnhancementBonusVsRace(IPRacialType.ShapeChanger, 4) } },
      //NIVEAU 5
      {863, new ItemProperty[] { ItemProperty.ACBonusVsAlign(IPAlignmentGroup.Good, 4), ItemProperty.ACBonusVsAlign(IPAlignmentGroup.Chaotic, 4), ItemProperty.ACBonusVsAlign(IPAlignmentGroup.Neutral, 4), ItemProperty.ACBonusVsAlign(IPAlignmentGroup.Evil, 4), ItemProperty.ACBonusVsAlign(IPAlignmentGroup.Lawful, 4), ItemProperty.AttackBonusVsAlign(IPAlignmentGroup.Good, 4), ItemProperty.AttackBonusVsAlign(IPAlignmentGroup.Chaotic, 4), ItemProperty.AttackBonusVsAlign(IPAlignmentGroup.Neutral, 4), ItemProperty.AttackBonusVsAlign(IPAlignmentGroup.Evil, 4), ItemProperty.AttackBonusVsAlign(IPAlignmentGroup.Lawful, 4) } },
      {864, new ItemProperty[] { ItemProperty.BonusSavingThrow(IPSaveBaseType.Reflex, 1), ItemProperty.BonusSavingThrow(IPSaveBaseType.Fortitude, 1), ItemProperty.BonusSavingThrow(IPSaveBaseType.Will, 1) } },
      {865, new ItemProperty[] { ItemProperty.AbilityBonus(IPAbility.Intelligence, 1), ItemProperty.AbilityBonus(IPAbility.Wisdom, 1), ItemProperty.AbilityBonus(IPAbility.Charisma, 1) } },
      {866, new ItemProperty[] { ItemProperty.EnhancementBonusVsRace(IPRacialType.Elemental, 4), ItemProperty.EnhancementBonusVsRace(IPRacialType.Fey, 4), ItemProperty.EnhancementBonusVsRace(IPRacialType.Giant, 4), ItemProperty.EnhancementBonusVsRace(IPRacialType.Construct, 4) } },
      //NIVEAU 6
      {867, new ItemProperty[] { ItemProperty.DamageBonus(IPDamageType.Acid, IPDamageBonus.Plus1d4), ItemProperty.DamageBonus(IPDamageType.Fire, IPDamageBonus.Plus1d4), ItemProperty.DamageBonus(IPDamageType.Cold, IPDamageBonus.Plus1d4), ItemProperty.DamageBonus(IPDamageType.Electrical, IPDamageBonus.Plus1d4), ItemProperty.DamageBonus(IPDamageType.Negative, IPDamageBonus.Plus1d4) , ItemProperty.DamageBonus(IPDamageType.Positive, IPDamageBonus.Plus1d4) } },
      {868, new ItemProperty[] { ItemProperty.MassiveCritical(IPDamageBonus.Plus1d8) } },
      //NIVEAU 7
      {869, new ItemProperty[] { ItemProperty.EnhancementBonusVsRace(IPRacialType.Halfling, 4), ItemProperty.EnhancementBonusVsRace(IPRacialType.Human, 4), ItemProperty.EnhancementBonusVsRace(IPRacialType.HalfElf, 4), ItemProperty.EnhancementBonusVsRace(IPRacialType.HalfOrc, 4), ItemProperty.EnhancementBonusVsRace(IPRacialType.Elf, 4), ItemProperty.EnhancementBonusVsRace(IPRacialType.Gnome, 4), ItemProperty.EnhancementBonusVsRace(IPRacialType.Dwarf, 4), ItemProperty.EnhancementBonusVsRace(IPRacialType.MagicalBeast, 4), ItemProperty.EnhancementBonusVsRace(IPRacialType.Dragon, 4), ItemProperty.EnhancementBonusVsRace(IPRacialType.Outsider, 4), ItemProperty.EnhancementBonusVsRace(IPRacialType.Aberration, 4) } },
      {870, new ItemProperty[] { ItemProperty.ACBonusVsDmgType((IPDamageType)14, 4) } }, // ELEMENTAL
      {871, new ItemProperty[] { ItemProperty.ACBonusVsDmgType((IPDamageType)4, 2) } }, // PHYSICAL
      {872, new ItemProperty[] { ItemProperty.BonusSpellResistance(IPSpellResistanceBonus.Plus10) } },
      //NIVEAU 8
      {873, new ItemProperty[] { ItemProperty.BonusSavingThrowVsX(IPSaveVs.Universal, 1) } },
      {874, new ItemProperty[] { ItemProperty.ACBonus(4) } },
      {875, new ItemProperty[] { ItemProperty.AttackBonus(4) } },
      {876, new ItemProperty[] { ItemProperty.DamageBonus(IPDamageType.Divine, IPDamageBonus.Plus1d4), ItemProperty.DamageBonus(IPDamageType.Sonic, IPDamageBonus.Plus1d4), ItemProperty.DamageBonus(IPDamageType.Magical, IPDamageBonus.Plus1d4), ItemProperty.DamageBonus(IPDamageType.Piercing, IPDamageBonus.Plus1d4), ItemProperty.DamageBonus(IPDamageType.Slashing, IPDamageBonus.Plus1d4) , ItemProperty.DamageBonus(IPDamageType.Bludgeoning, IPDamageBonus.Plus1d4) } },
      {877, new ItemProperty[] { ItemProperty.EnhancementBonusVsAlign(IPAlignmentGroup.Good, 4), ItemProperty.EnhancementBonusVsAlign(IPAlignmentGroup.Chaotic, 4), ItemProperty.EnhancementBonusVsAlign(IPAlignmentGroup.Neutral, 4), ItemProperty.EnhancementBonusVsAlign(IPAlignmentGroup.Evil, 4), ItemProperty.EnhancementBonusVsAlign(IPAlignmentGroup.Lawful, 4) } },
      //NIVEAU 9
      {878, new ItemProperty[] { ItemProperty.ACBonusVsDmgType((IPDamageType)4, 4) } }, // PHYSICAL
      {879, new ItemProperty[] { ItemProperty.VampiricRegeneration(1) } },
      {880, new ItemProperty[] { ItemProperty.EnhancementBonus(4) } },
      {881, new ItemProperty[] { ItemProperty.Keen() } },
      {882, new ItemProperty[] { ItemProperty.Haste() } },
    };
    private static void Enchantement(SpellEvents.OnSpellCast onSpellCast)
    {
      if (!(onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oCaster) || !PlayerSystem.Players.TryGetValue(oCaster.ControllingPlayer.LoginCreature, out PlayerSystem.Player player))
        return;

      if(!(onSpellCast.TargetObject is NwItem targetItem) || targetItem == null || targetItem.Possessor != oCaster)
      {
        player.oid.SendServerMessage("Cible invalide.", ColorConstants.Red);
        return;
      }

      if (player.windows.ContainsKey("enchantementSelection"))
        ((PlayerSystem.Player.EnchantementSelectionWindow)player.windows["enchantementSelection"]).CreateWindow(onSpellCast.Spell, targetItem);
      else
        player.windows.Add("enchantementSelection", new PlayerSystem.Player.EnchantementSelectionWindow(player, onSpellCast.Spell, targetItem));

      // TODO : la réactivation fonctionnera via OnExamineItem. Mais gardons ça dans un coin en attendant
      /*if (oTarget.ItemProperties.Any(ip => ip.Tag.StartsWith($"ENCHANTEMENT_{spellId}") && ip.Tag.Contains("INACTIVE")))
      {
        string inactiveIPTag = oTarget.ItemProperties.FirstOrDefault(ip => ip.Tag.StartsWith($"ENCHANTEMENT_{spellId}") && ip.Tag.Contains("INACTIVE")).Tag;
        string[] IPproperties = inactiveIPTag.Split("_");
        player.craftJob.Start(Craft.Job.JobType.EnchantementReactivation, player, null, oTarget, $"{spellId}_{IPproperties[5]}_{IPproperties[6]}");
        return;
      }*/
    }
  }
}
