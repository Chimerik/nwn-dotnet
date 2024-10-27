using Anvil.API;
using System.Linq;
using System;
using Anvil.API.Events;
using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public enum SpellData
    {
      EnergyCost = 0,
      Cooldown = 1,
      Attribute = 2,
      Type = 3
    }

    public static readonly Dictionary<int, ItemProperty[]> enchantementCategories = new()
    {
      //NIVEAU 0
      {840, new ItemProperty[] { ItemProperty.Light(IPLightBrightness.Dim, IPLightColor.Blue), ItemProperty.Light(IPLightBrightness.Dim, IPLightColor.Green), ItemProperty.Light(IPLightBrightness.Dim, IPLightColor.Orange), ItemProperty.Light(IPLightBrightness.Dim, IPLightColor.Purple), ItemProperty.Light(IPLightBrightness.Dim, IPLightColor.Red), ItemProperty.Light(IPLightBrightness.Dim, IPLightColor.White), ItemProperty.Light(IPLightBrightness.Dim, IPLightColor.Yellow), ItemProperty.Light(IPLightBrightness.Low, IPLightColor.Blue), ItemProperty.Light(IPLightBrightness.Low, IPLightColor.Green), ItemProperty.Light(IPLightBrightness.Low, IPLightColor.Orange), ItemProperty.Light(IPLightBrightness.Low, IPLightColor.Purple), ItemProperty.Light(IPLightBrightness.Low, IPLightColor.Red), ItemProperty.Light(IPLightBrightness.Low, IPLightColor.White), ItemProperty.Light(IPLightBrightness.Low, IPLightColor.Yellow) } },
      {841, new ItemProperty[] { ItemProperty.ACBonusVsRace(RacialType.HumanoidGoblinoid, 4), ItemProperty.ACBonusVsRace(RacialType.Animal, 4), ItemProperty.ACBonusVsRace(RacialType.HumanoidReptilian, 4), ItemProperty.ACBonusVsRace(RacialType.Vermin, 4), ItemProperty.AttackBonusVsRace(RacialType.HumanoidGoblinoid, 4), ItemProperty.AttackBonusVsRace(RacialType.Animal, 4), ItemProperty.AttackBonusVsRace(RacialType.HumanoidReptilian, 4), ItemProperty.AttackBonusVsRace(RacialType.Vermin, 4) } },
      {842, new ItemProperty[] { ItemProperty.MassiveCritical(IPDamageBonus.Plus2) } },
      {843, new ItemProperty[] { ItemProperty.SkillBonus(Skill.AnimalEmpathy, 1), ItemProperty.SkillBonus(Skill.Appraise, 1), ItemProperty.SkillBonus(Skill.Bluff, 1), ItemProperty.SkillBonus(Skill.Concentration, 1), ItemProperty.SkillBonus(Skill.DisableTrap, 1), ItemProperty.SkillBonus(Skill.Discipline, 1), ItemProperty.SkillBonus(Skill.Heal, 1), ItemProperty.SkillBonus(Skill.Hide, 1), ItemProperty.SkillBonus(Skill.Intimidate, 1), ItemProperty.SkillBonus(Skill.Listen, 1), ItemProperty.SkillBonus(Skill.Lore, 1), ItemProperty.SkillBonus(Skill.MoveSilently, 1), ItemProperty.SkillBonus(Skill.OpenLock, 1), ItemProperty.SkillBonus(Skill.Parry, 1), ItemProperty.SkillBonus(Skill.Perform, 1), ItemProperty.SkillBonus(Skill.Persuade, 1), ItemProperty.SkillBonus(Skill.PickPocket, 1), ItemProperty.SkillBonus(Skill.Search, 1), ItemProperty.SkillBonus(Skill.SetTrap, 1), ItemProperty.SkillBonus(Skill.Spellcraft, 1), ItemProperty.SkillBonus(Skill.Spot, 1), ItemProperty.SkillBonus(Skill.Taunt, 1), ItemProperty.SkillBonus(Skill.Taunt, 1), ItemProperty.SkillBonus(Skill.Tumble, 1), ItemProperty.SkillBonus(Skill.UseMagicDevice, 1) } },
      {844, new ItemProperty[] { ItemProperty.BonusSavingThrowVsX(IPSaveVs.Fear, 1) } },
      
      //NIVEAU 1
      {845, new ItemProperty[] { ItemProperty.AbilityBonus(IPAbility.Constitution, 1) } },
      {846, new ItemProperty[] { ItemProperty.DamageBonus(IPDamageType.Acid, IPDamageBonus.Plus1), ItemProperty.DamageBonus(IPDamageType.Fire, IPDamageBonus.Plus1), ItemProperty.DamageBonus(IPDamageType.Cold, IPDamageBonus.Plus1), ItemProperty.DamageBonus(IPDamageType.Electrical, IPDamageBonus.Plus1), ItemProperty.DamageBonus(IPDamageType.Negative, IPDamageBonus.Plus1) , ItemProperty.DamageBonus(IPDamageType.Positive, IPDamageBonus.Plus1) } },
      {847, new ItemProperty[] { ItemProperty.VisualEffect(ItemVisual.Acid), ItemProperty.VisualEffect(ItemVisual.Cold), ItemProperty.VisualEffect(ItemVisual.Electrical), ItemProperty.VisualEffect(ItemVisual.Fire) } },
      {848, new ItemProperty[] { ItemProperty.Light(IPLightBrightness.Normal, IPLightColor.Blue), ItemProperty.Light(IPLightBrightness.Normal, IPLightColor.Green), ItemProperty.Light(IPLightBrightness.Normal, IPLightColor.Orange), ItemProperty.Light(IPLightBrightness.Normal, IPLightColor.Purple), ItemProperty.Light(IPLightBrightness.Normal, IPLightColor.Red), ItemProperty.Light(IPLightBrightness.Dim, IPLightColor.White), ItemProperty.Light(IPLightBrightness.Normal, IPLightColor.Yellow), ItemProperty.Light(IPLightBrightness.Bright, IPLightColor.Blue), ItemProperty.Light(IPLightBrightness.Bright, IPLightColor.Green), ItemProperty.Light(IPLightBrightness.Bright, IPLightColor.Orange), ItemProperty.Light(IPLightBrightness.Bright, IPLightColor.Purple), ItemProperty.Light(IPLightBrightness.Bright, IPLightColor.Red), ItemProperty.Light(IPLightBrightness.Bright, IPLightColor.White), ItemProperty.Light(IPLightBrightness.Bright, IPLightColor.Yellow) } },
      {849, new ItemProperty[] { ItemProperty.ACBonusVsRace(RacialType.HumanoidOrc, 4), ItemProperty.ACBonusVsRace(RacialType.Undead, 4), ItemProperty.ACBonusVsRace(RacialType.Beast, 4), ItemProperty.ACBonusVsRace(RacialType.HumanoidMonstrous, 4), ItemProperty.ACBonusVsRace(RacialType.ShapeChanger, 4), ItemProperty.AttackBonusVsRace(RacialType.HumanoidOrc, 4), ItemProperty.AttackBonusVsRace(RacialType.Undead, 4), ItemProperty.AttackBonusVsRace(RacialType.Beast, 4), ItemProperty.AttackBonusVsRace(RacialType.HumanoidMonstrous, 4), ItemProperty.AttackBonusVsRace(RacialType.ShapeChanger, 4) } },
      //NIVEAU 2
      {850, new ItemProperty[] { ItemProperty.ACBonusVsRace(RacialType.Elemental, 1), ItemProperty.ACBonusVsRace(RacialType.Fey, 1), ItemProperty.ACBonusVsRace(RacialType.Giant, 1), ItemProperty.ACBonusVsRace(RacialType.Construct, 1), ItemProperty.AttackBonusVsRace(RacialType.Elemental, 1), ItemProperty.AttackBonusVsRace(RacialType.Fey, 1), ItemProperty.AttackBonusVsRace(RacialType.Giant, 1), ItemProperty.AttackBonusVsRace(RacialType.Construct, 1) } },
      {851, new ItemProperty[] { ItemProperty.BonusSavingThrowVsX(IPSaveVs.Death, 1), ItemProperty.BonusSavingThrowVsX(IPSaveVs.MindAffecting, 1), ItemProperty.BonusSavingThrowVsX(IPSaveVs.Acid, 1), ItemProperty.BonusSavingThrowVsX(IPSaveVs.Cold, 1), ItemProperty.BonusSavingThrowVsX(IPSaveVs.Disease, 1), ItemProperty.BonusSavingThrowVsX(IPSaveVs.Divine, 1), ItemProperty.BonusSavingThrowVsX(IPSaveVs.Electrical, 1), ItemProperty.BonusSavingThrowVsX(IPSaveVs.Fire, 1), ItemProperty.BonusSavingThrowVsX(IPSaveVs.Negative, 1), ItemProperty.BonusSavingThrowVsX(IPSaveVs.Poison, 1), ItemProperty.BonusSavingThrowVsX(IPSaveVs.Positive, 1), ItemProperty.BonusSavingThrowVsX(IPSaveVs.Sonic, 1) } },
      {852, new ItemProperty[] { ItemProperty.ACBonusVsDmgType(IPDamageType.Bludgeoning, 4), ItemProperty.ACBonusVsDmgType(IPDamageType.Piercing, 4), ItemProperty.ACBonusVsDmgType(IPDamageType.Slashing, 4) } },
      {853, new ItemProperty[] { ItemProperty.MassiveCritical(IPDamageBonus.Plus1d4) } },
      {854, new ItemProperty[] { ItemProperty.DamageBonus(IPDamageType.Divine, IPDamageBonus.Plus1), ItemProperty.DamageBonus(IPDamageType.Sonic, IPDamageBonus.Plus1), ItemProperty.DamageBonus(IPDamageType.Magical, IPDamageBonus.Plus1), ItemProperty.DamageBonus(IPDamageType.Piercing, IPDamageBonus.Plus1), ItemProperty.DamageBonus(IPDamageType.Slashing, IPDamageBonus.Plus1) , ItemProperty.DamageBonus(IPDamageType.Bludgeoning, IPDamageBonus.Plus1) } },
      //NIVEAU 3
      {855, new ItemProperty[] { ItemProperty.AbilityBonus(IPAbility.Strength, 1), ItemProperty.AbilityBonus(IPAbility.Dexterity, 1) } },
      {856, new ItemProperty[] { ItemProperty.Regeneration(1) } },
      {857, new ItemProperty[] { ItemProperty.ACBonusVsDmgType((IPDamageType)14, 2) } }, // VS ELEMENTAL
      {858, new ItemProperty[] { ItemProperty.EnhancementBonusVsRace(RacialType.HumanoidGoblinoid, 4), ItemProperty.EnhancementBonusVsRace(RacialType.Animal, 4), ItemProperty.EnhancementBonusVsRace(RacialType.HumanoidReptilian, 4), ItemProperty.EnhancementBonusVsRace(RacialType.Vermin, 4) } },
      {859, new ItemProperty[] { ItemProperty.VisualEffect(ItemVisual.Sonic), ItemProperty.VisualEffect(ItemVisual.Holy), ItemProperty.VisualEffect(ItemVisual.Evil) } },
      //NIVEAU 4
      {860, new ItemProperty[] { ItemProperty.ACBonusVsRace(RacialType.Halfling, 4), ItemProperty.ACBonusVsRace(RacialType.Human, 4), ItemProperty.ACBonusVsRace(RacialType.HalfElf, 4), ItemProperty.ACBonusVsRace(RacialType.HalfOrc, 4), ItemProperty.ACBonusVsRace(RacialType.Elf, 4), ItemProperty.ACBonusVsRace(RacialType.Gnome, 4), ItemProperty.ACBonusVsRace(RacialType.Dwarf, 4), ItemProperty.ACBonusVsRace(RacialType.MagicalBeast, 4), ItemProperty.ACBonusVsRace(RacialType.Dragon, 4), ItemProperty.ACBonusVsRace(RacialType.Outsider, 4), ItemProperty.ACBonusVsRace(RacialType.Aberration, 4), ItemProperty.AttackBonusVsRace(RacialType.Halfling, 4), ItemProperty.AttackBonusVsRace(RacialType.Human, 4), ItemProperty.AttackBonusVsRace(RacialType.HalfElf, 4), ItemProperty.AttackBonusVsRace(RacialType.HalfOrc, 4), ItemProperty.AttackBonusVsRace(RacialType.Elf, 4), ItemProperty.AttackBonusVsRace(RacialType.Gnome, 4), ItemProperty.AttackBonusVsRace(RacialType.Dwarf, 4), ItemProperty.AttackBonusVsRace(RacialType.MagicalBeast, 4), ItemProperty.AttackBonusVsRace(RacialType.Dragon, 4), ItemProperty.AttackBonusVsRace(RacialType.Outsider, 4), ItemProperty.AttackBonusVsRace(RacialType.Aberration, 4) } },
      {861, new ItemProperty[] { ItemProperty.SkillBonus(Skill.AnimalEmpathy, 2), ItemProperty.SkillBonus(Skill.Appraise, 2), ItemProperty.SkillBonus(Skill.Bluff, 2), ItemProperty.SkillBonus(Skill.Concentration, 2), ItemProperty.SkillBonus(Skill.DisableTrap, 2), ItemProperty.SkillBonus(Skill.Discipline, 2), ItemProperty.SkillBonus(Skill.Heal, 2), ItemProperty.SkillBonus(Skill.Hide, 2), ItemProperty.SkillBonus(Skill.Intimidate, 2), ItemProperty.SkillBonus(Skill.Listen, 2), ItemProperty.SkillBonus(Skill.Lore, 2), ItemProperty.SkillBonus(Skill.MoveSilently, 2), ItemProperty.SkillBonus(Skill.OpenLock, 2), ItemProperty.SkillBonus(Skill.Parry, 2), ItemProperty.SkillBonus(Skill.Perform, 2), ItemProperty.SkillBonus(Skill.Persuade, 2), ItemProperty.SkillBonus(Skill.PickPocket, 2), ItemProperty.SkillBonus(Skill.Search, 2), ItemProperty.SkillBonus(Skill.SetTrap, 2), ItemProperty.SkillBonus(Skill.Spellcraft, 2), ItemProperty.SkillBonus(Skill.Spot, 2), ItemProperty.SkillBonus(Skill.Taunt, 2), ItemProperty.SkillBonus(Skill.Taunt, 2), ItemProperty.SkillBonus(Skill.Tumble, 2), ItemProperty.SkillBonus(Skill.UseMagicDevice, 2) } },
      {862, new ItemProperty[] { ItemProperty.EnhancementBonusVsRace(RacialType.HumanoidOrc, 4), ItemProperty.EnhancementBonusVsRace(RacialType.Undead, 4), ItemProperty.EnhancementBonusVsRace(RacialType.Beast, 4), ItemProperty.EnhancementBonusVsRace(RacialType.HumanoidMonstrous, 4), ItemProperty.EnhancementBonusVsRace(RacialType.ShapeChanger, 4) } },
      //NIVEAU 5
      {863, new ItemProperty[] { ItemProperty.ACBonusVsAlign(IPAlignmentGroup.Good, 4), ItemProperty.ACBonusVsAlign(IPAlignmentGroup.Chaotic, 4), ItemProperty.ACBonusVsAlign(IPAlignmentGroup.Neutral, 4), ItemProperty.ACBonusVsAlign(IPAlignmentGroup.Evil, 4), ItemProperty.ACBonusVsAlign(IPAlignmentGroup.Lawful, 4), ItemProperty.AttackBonusVsAlign(IPAlignmentGroup.Good, 4), ItemProperty.AttackBonusVsAlign(IPAlignmentGroup.Chaotic, 4), ItemProperty.AttackBonusVsAlign(IPAlignmentGroup.Neutral, 4), ItemProperty.AttackBonusVsAlign(IPAlignmentGroup.Evil, 4), ItemProperty.AttackBonusVsAlign(IPAlignmentGroup.Lawful, 4) } },
      {864, new ItemProperty[] { ItemProperty.BonusSavingThrow(IPSaveBaseType.Reflex, 1), ItemProperty.BonusSavingThrow(IPSaveBaseType.Fortitude, 1), ItemProperty.BonusSavingThrow(IPSaveBaseType.Will, 1) } },
      {865, new ItemProperty[] { ItemProperty.AbilityBonus(IPAbility.Intelligence, 1), ItemProperty.AbilityBonus(IPAbility.Wisdom, 1), ItemProperty.AbilityBonus(IPAbility.Charisma, 1) } },
      {866, new ItemProperty[] { ItemProperty.EnhancementBonusVsRace(RacialType.Elemental, 4), ItemProperty.EnhancementBonusVsRace(RacialType.Fey, 4), ItemProperty.EnhancementBonusVsRace(RacialType.Giant, 4), ItemProperty.EnhancementBonusVsRace(RacialType.Construct, 4) } },
      //NIVEAU 6
      {867, new ItemProperty[] { ItemProperty.DamageBonus(IPDamageType.Acid, IPDamageBonus.Plus1d4), ItemProperty.DamageBonus(IPDamageType.Fire, IPDamageBonus.Plus1d4), ItemProperty.DamageBonus(IPDamageType.Cold, IPDamageBonus.Plus1d4), ItemProperty.DamageBonus(IPDamageType.Electrical, IPDamageBonus.Plus1d4), ItemProperty.DamageBonus(IPDamageType.Negative, IPDamageBonus.Plus1d4) , ItemProperty.DamageBonus(IPDamageType.Positive, IPDamageBonus.Plus1d4) } },
      {868, new ItemProperty[] { ItemProperty.MassiveCritical(IPDamageBonus.Plus1d8) } },
      //NIVEAU 7
      {869, new ItemProperty[] { ItemProperty.EnhancementBonusVsRace(RacialType.Halfling, 4), ItemProperty.EnhancementBonusVsRace(RacialType.Human, 4), ItemProperty.EnhancementBonusVsRace(RacialType.HalfElf, 4), ItemProperty.EnhancementBonusVsRace(RacialType.HalfOrc, 4), ItemProperty.EnhancementBonusVsRace(RacialType.Elf, 4), ItemProperty.EnhancementBonusVsRace(RacialType.Gnome, 4), ItemProperty.EnhancementBonusVsRace(RacialType.Dwarf, 4), ItemProperty.EnhancementBonusVsRace(RacialType.MagicalBeast, 4), ItemProperty.EnhancementBonusVsRace(RacialType.Dragon, 4), ItemProperty.EnhancementBonusVsRace(RacialType.Outsider, 4), ItemProperty.EnhancementBonusVsRace(RacialType.Aberration, 4) } },
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
      {882, new ItemProperty[] { ItemProperty.Haste() } }
    };

    public static int MaximizeOrEmpower(int nDice, int nNumberOfDice, MetaMagic nMeta, int nBonus = 0)
    {
      int nDamage = 0;
      for (int i = 1; i <= nNumberOfDice; i++)
      {
        nDamage = nDamage + Utils.random.Next(nDice) + 1;
      }
      //Resolve metamagic
      if (nMeta == MetaMagic.Maximize)
      {
        nDamage = nDice * nNumberOfDice;
      }
      else if (nMeta == MetaMagic.Empower)
      {
        nDamage += nDamage / 2;
      }
      return nDamage + nBonus;
    }
    public static ClassType GetCastingClass(NwSpell Spell)
    {
      byte clericCastLevel = Spell.GetSpellLevelForClass(ClassType.Cleric);
      byte druidCastLevel = Spell.GetSpellLevelForClass(ClassType.Druid);
      byte paladinCastLevel = Spell.GetSpellLevelForClass(ClassType.Paladin);
      byte rangerCastLevel = Spell.GetSpellLevelForClass(ClassType.Ranger);
      byte bardCastLevel = Spell.GetSpellLevelForClass(ClassType.Bard);

      Dictionary<ClassType, byte> classSorter = new Dictionary<ClassType, byte>()
      {
        { ClassType.Cleric, clericCastLevel },
        { ClassType.Druid, druidCastLevel },
        { ClassType.Paladin, paladinCastLevel },
        { ClassType.Ranger, rangerCastLevel },
        { ClassType.Bard, bardCastLevel },
      };

      try
      {
        ClassType castingClass = classSorter.Where(c => c.Value < 255).Max().Key;
        return castingClass;
      }
      catch (Exception)
      {
        return (ClassType)43;
      }
    }
    public static void SignalEventSpellCast(NwGameObject target, NwGameObject caster, Spell spell, bool harmful = true)
    {
      if (target is NwCreature oTargetCreature)
        CreatureEvents.OnSpellCastAt.Signal(caster, oTargetCreature, spell, harmful);
      else if (target is NwPlaceable oTargetPlc)
        PlaceableEvents.OnSpellCastAt.Signal(caster, oTargetPlc, spell, harmful);
      else if (target is NwDoor oTargetDoor)
        DoorEvents.OnSpellCastAt.Signal(caster, oTargetDoor, spell, harmful);
    }
    public static int GetSpellIDFromScroll(NwItem oScroll)
    {
      ItemProperty ip = oScroll.ItemProperties.FirstOrDefault(ip => ip.Property.PropertyType == ItemPropertyType.CastSpell);

      if (ip != null)
        return ip.SubTypeTable.GetInt(ip.SubType.RowIndex, "SpellIndex").Value;

      return 0;
    }
    public static byte GetSpellLevelFromScroll(NwItem oScroll)
    {
      ItemProperty ip = oScroll.ItemProperties.FirstOrDefault(ip => ip.Property.PropertyType == ItemPropertyType.CastSpell);

      if (ip != null)
      {
        try
        {
          return (byte)ip.SubTypeTable.GetInt(ip.SubType.RowIndex, "InnateLvl").Value;
        }
        catch(Exception)
        {
          return 0;
        }
      }

      return 255;
    }
    public static int GetSpellSchoolFromString(string school)
    {
      return "GACDEVINT".IndexOf(school);
    }
    public static bool IsSpellBuff(NwSpell spell)
    {
      return spell.SpellType switch
      {
        Spell.Aid or Spell.Amplify or Spell.Auraofglory or Spell.AuraOfVitality or Spell.Awaken or Spell.Barkskin or Spell.Battletide or Spell.BladeThirst or Spell.Bless or Spell.BlessWeapon or Spell.BloodFrenzy or Spell.BullsStrength or Spell.Camoflage or Spell.CatsGrace or Spell.Charger or Spell.ClairaudienceAndClairvoyance or Spell.Clarity or Spell.CloakOfChaos or Spell.ContinualFlame or Spell.Darkfire or Spell.Darkvision or Spell.DeathArmor or Spell.DeathWard or Spell.Displacement or Spell.DivineFavor or Spell.DivineMight or Spell.DivinePower or Spell.DivineShield or Spell.EagleSpledor or Spell.ElementalShield or Spell.Endurance or Spell.EndureElements or Spell.EnergyBuffer or Spell.EntropicShield or Spell.EpicMageArmor or Spell.Etherealness or Spell.EtherealVisage or Spell.ExpeditiousRetreat or Spell.FlameWeapon or Spell.FoxsCunning or Spell.FreedomOfMovement or Spell.GhostlyVisage or Spell.GlobeOfInvulnerability or Spell.GlyphOfWarding or Spell.GreaterBullsStrength or Spell.GreaterCatsGrace or Spell.GreaterEagleSplendor or Spell.GreaterEndurance or Spell.GreaterFoxsCunning or Spell.GreaterMagicFang or Spell.GreaterMagicWeapon or Spell.GreaterOwlsWisdom or Spell.GreaterShadowConjurationMinorGlobe or Spell.GreaterShadowConjurationMirrorImage or Spell.GreaterSpellMantle or Spell.GreaterStoneskin or Spell.Stoneskin or Spell.Haste or Spell.HolyAura or Spell.HolySword or Spell.ImprovedInvisibility or Spell.Invisibility or Spell.InvisibilitySphere or Spell.Ironguts or Spell.KeenEdge or Spell.LesserMindBlank or Spell.LesserSpellMantle or Spell.Light or Spell.MageArmor or Spell.MagicCircleAgainstChaos or Spell.MagicCircleAgainstEvil or Spell.MagicCircleAgainstGood or Spell.MagicCircleAgainstLaw or Spell.MagicFang or Spell.MagicVestment or Spell.MagicWeapon or Spell.MassCamoflage or Spell.MassHaste or Spell.MestilsAcidSheath or Spell.MindBlank or Spell.MinorGlobeOfInvulnerability or Spell.MonstrousRegeneration or Spell.NegativeEnergyProtection or Spell.OneWithTheLand or Spell.OwlsInsight or Spell.OwlsWisdom or Spell.PolymorphSelf or Spell.Prayer or Spell.Premonition or Spell.ProtectionFromChaos or Spell.ProtectionFromElements or Spell.ProtectionFromEvil or Spell.ProtectionFromGood or Spell.ProtectionFromLaw or Spell.ProtectionFromSpells or Spell.Regenerate or Spell.Resistance or Spell.ResistElements or Spell.Sanctuary or Spell.SeeInvisibility or Spell.InvisibilityPurge or Spell.ShadesStoneskin or Spell.ShadowConjurationInivsibility or Spell.ShadowConjurationMageArmor or Spell.ShadowShield or Spell.Shapechange or Spell.Shield or Spell.ShieldOfFaith or Spell.ShieldOfLaw or Spell.Silence or Spell.SpellMantle or Spell.SpellResistance or Spell.StoneBones or Spell.StoneToFlesh or Spell.TensersTransformation or Spell.TimeStop or Spell.TrueSeeing or Spell.TrueStrike or Spell.TymorasSmile or Spell.Virtue or Spell.WarCry or Spell.WoundingWhispers or Spell.UnholyAura or Spell.AbilityAuraBlinding or Spell.AbilityAuraCold or Spell.AbilityAuraElectricity or Spell.AbilityAuraFear or Spell.AbilityAuraFire or Spell.AbilityAuraHorrificappearance or Spell.AbilityAuraMenace or Spell.AbilityAuraOfCourage or Spell.AbilityAuraProtection or Spell.AbilityAuraStun or Spell.AbilityAuraUnearthlyVisage or Spell.AbilityAuraUnnatural or Spell.AbilityBarbarianRage or Spell.AbilityFerocity1 or Spell.AbilityFerocity2 or Spell.AbilityFerocity3 or Spell.AbilityIntensity1 or Spell.AbilityIntensity2 or Spell.AbilityIntensity3 or Spell.AbilityHowlConfuse or Spell.AbilityHowlDaze or Spell.AbilityHowlDeath or Spell.AbilityHowlDoom or Spell.AbilityHowlFear or Spell.AbilityHowlParalysis or Spell.AbilityHowlSonic or Spell.AbilityHowlStun or Spell.AbilityDragonWingBuffet or Spell.AbilityMummyBolsterUndead or Spell.AbilityEpicMightyRage or Spell.AbilityRage3 or Spell.AbilityRage4 or Spell.AbilityRage5 or Spell.AbilityTyrantFogMist or Spell.Darkness or Spell.ShadowConjurationDarkness or Spell.LegendLore or Spell.AbilityBattleMastery or Spell.AbilityDivineStrength or Spell.AbilityDivineProtection or Spell.AbilityDivineTrickery or Spell.AbilityRoguesCunning or Spell.AbilityDragonFear or Spell.Dirge or Spell.ShadowEvade or Spell.VineMineCamouflage or Spell.DeafeningClang or Spell.Blackstaff or Spell.IounStoneBlue or Spell.IounStoneDeepRed or Spell.IounStoneDustyRose or Spell.IounStonePaleBlue or Spell.IounStonePink or Spell.IounStonePinkGreen or Spell.IounStoneScarletBlue or Spell.AbilityTroglodyteStench => true,
        _ => false,
      };
    }
    public static bool IgnoredSpellList(Spell spell)
    {
      return spell switch
      {
        Spell.AllSpells or Spell.CloakOfChaos or Spell.MagicCircleAgainstChaos or Spell.MagicCircleAgainstLaw or Spell.ProtectionFromChaos or Spell.ProtectionFromLaw or Spell.ShieldOfLaw or Spell.SphereOfChaos or Spell.AbilityDragonWingBuffet or Spell.AbilitySmokeClaw or Spell.AbilityTrumpetBlast or Spell.AbilityQuiveringPalm or Spell.AbilityEmptyBody or Spell.AbilityDetectEvil or Spell.AbilityAuraOfCourage or Spell.AbilitySmiteEvil or Spell.AbilityElementalShape or Spell.AbilityWildShape or Spell.ActivateItemPortal or Spell.ActivateItemSelf2 or Spell.AbilityActivateItem or Spell.CraftDyeMetalcolor1 or Spell.CraftDyeMetalcolor2 or Spell.CraftHarperItem or Spell.CraftPoisonWeaponOrAmmo or Spell.CraftAddItemProperty or Spell.CraftCraftArmorSkill or Spell.CraftCraftWeaponSkill or Spell.CraftDyeClothcolor1 or Spell.CraftDyeClothcolor2 or Spell.CraftDyeLeathercolor1 or Spell.CraftDyeLeathercolor2 or Spell.GrenadeAcid or Spell.GrenadeCaltrops or Spell.GrenadeChicken or Spell.GrenadeChoking or Spell.GrenadeFire or Spell.GrenadeHoly or Spell.GrenadeTangle or Spell.GrenadeThunderstone or Spell.DivineMight or Spell.DivineShield or Spell.ShadowDaze or Spell.ShadowEvade or Spell.TymorasSmile or Spell.TrapArrow or Spell.TrapBolt or Spell.TrapDart or Spell.TrapShuriken or Spell.RodOfWonder or Spell.DeckOfManyThings or Spell.ElementalSummoningItem or Spell.DeckAvatar or Spell.DeckButterflyspray or Spell.DeckGemspray or Spell.Powerstone or Spell.KoboldJump or Spell.Healingkit or Spell.Spellstaff or Spell.Decharger or Spell.AbilityWhirlwind or Spell.AbilityCommandTheHorde or Spell.AbilityAaArrowOfDeath or Spell.AbilityAaHailOfArrows or Spell.AbilityAaImbueArrow or Spell.AbilityAaSeekerArrow1 or Spell.AbilityAaSeekerArrow2 or Spell.AbilityAsDarkness or Spell.AbilityAsGhostlyVisage or Spell.AbilityAsImprovedInvisiblity or Spell.AbilityAsInvisibility or Spell.HorseAssignMount or Spell.AbilityBgCreatedead or Spell.AbilityBgFiendishServant or Spell.AbilityBgInflictSeriousWounds or Spell.AbilityBgInflictCriticalWounds or Spell.AbilityBgContagion or Spell.AbilityBgBullsStrength or Spell.FlyingDebris or Spell.AbilityDcDivineWrath or Spell.AbilityPmAnimateDead or Spell.AbilityPmSummonUndead or Spell.AbilityPmUndeadGraft1 or Spell.AbilityPmUndeadGraft2 or Spell.AbilityPmSummonGreaterUndead or Spell.AbilityPmDeathlessMasterTouch or Spell.EpicHellball or Spell.EpicMummyDust or Spell.EpicDragonKnight or Spell.EpicMageArmor or Spell.EpicRuin or Spell.AbilityDwDefensiveStance or Spell.AbilityEpicMightyRage or Spell.AbilityEpicCurseSong or Spell.AbilityEpicImprovedWhirlwind or Spell.AbilityEpicShapeDragonkin or Spell.AbilityEpicShapeDragon or Spell.HorseMenu or Spell.HorseDismount or Spell.HorseMount or Spell.HorsePartyDismount or Spell.HorsePartyMount or Spell.PaladinSummonMount => true,
        _ => false,
      };
    }
    public static double GetReduceCastTimeFromItem(NwItem item, int inscription)
    {
      double castTimeModifier = 0;

      if (item is not null)
        for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
          if (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value == inscription)
            castTimeModifier += 1;

      return castTimeModifier;
    }
    public static double GetIncreaseDurationFromItem(NwItem item, int inscription)
    {
      double increaseDuration = 0;

      if (item is not null)
        for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
          if (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value == inscription)
            increaseDuration += 0.06;

      return increaseDuration;
    }
  }
}
