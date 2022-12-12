using Anvil.API;
using System.Linq;
using System;
using Anvil.API.Events;
using System.Collections.Generic;
using NLog.Fluent;

namespace NWN.Systems
{
  public static class SpellUtils
  {
    public static readonly Dictionary<int, ItemProperty[]> enchantementCategories = new Dictionary<int, ItemProperty[]>()
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
    public static void RemoveAnySpellEffects(Spell spell, NwCreature oTarget)
    {
      foreach (var eff in oTarget.ActiveEffects.Where(e => e.Spell.SpellType == spell))
        oTarget.RemoveEffect(eff);
    }
    public static ClassType GetCastingClass(NwSpell Spell)
    {
      byte clericCastLevel = Spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Cleric));
      byte druidCastLevel = Spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Druid));
      byte paladinCastLevel = Spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Paladin));
      byte rangerCastLevel = Spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Ranger));
      byte bardCastLevel = Spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Bard));

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
    public static void SignalEventSpellCast(NwGameObject target, NwCreature caster, Spell spell, bool harmful = true)
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

    /*public static async void RestoreSpell(NwCreature caster, Spell spell)
    {
      if (caster == null)
        return;

      await NwTask.Delay(TimeSpan.FromSeconds(0.2));

      foreach (MemorizedSpellSlot spellSlot in caster.GetClassInfo((ClassType)43).GetMemorizedSpellSlots(0).Where(s => s.Spell.SpellType == spell && !s.IsReady))
        spellSlot.IsReady = true;
    }*/
    public static async void CancelCastOnMovement(NwCreature caster)
    {
      float posX = caster.Position.X;
      float posY = caster.Position.Y;
      await NwTask.WaitUntil(() => caster.Position.X != posX || caster.Position.Y != posY);

      caster.GetObjectVariable<LocalVariableInt>("_AUTO_SPELL").Delete();
      caster.GetObjectVariable<LocalVariableObject<NwGameObject>>("_AUTO_SPELL_TARGET").Delete();
      caster.OnCombatRoundEnd -= PlayerSystem.HandleCombatRoundEndForAutoSpells;
    }

    public static bool IsSpellBuff(NwSpell spell)
    {
      switch (spell.SpellType)
      {
        case Spell.Aid:
        case Spell.Amplify:
        case Spell.Auraofglory:
        case Spell.AuraOfVitality:
        case Spell.Awaken:
        case Spell.Barkskin:
        case Spell.Battletide:
        case Spell.BladeThirst:
        case Spell.Bless:
        case Spell.BlessWeapon:
        case Spell.BloodFrenzy:
        case Spell.BullsStrength:
        case Spell.Camoflage:
        case Spell.CatsGrace:
        case Spell.Charger:
        case Spell.ClairaudienceAndClairvoyance:
        case Spell.Clarity:
        case Spell.CloakOfChaos:
        case Spell.ContinualFlame:
        case Spell.Darkfire:
        case Spell.Darkvision:
        case Spell.DeathArmor:
        case Spell.DeathWard:
        case Spell.Displacement:
        case Spell.DivineFavor:
        case Spell.DivineMight:
        case Spell.DivinePower:
        case Spell.DivineShield:
        case Spell.EagleSpledor:
        case Spell.ElementalShield:
        case Spell.Endurance:
        case Spell.EndureElements:
        case Spell.EnergyBuffer:
        case Spell.EntropicShield:
        case Spell.EpicMageArmor:
        case Spell.Etherealness:
        case Spell.EtherealVisage:
        case Spell.ExpeditiousRetreat:
        case Spell.FlameWeapon:
        case Spell.FoxsCunning:
        case Spell.FreedomOfMovement:
        case Spell.GhostlyVisage:
        case Spell.GlobeOfInvulnerability:
        case Spell.GlyphOfWarding:
        case Spell.GreaterBullsStrength:
        case Spell.GreaterCatsGrace:
        case Spell.GreaterEagleSplendor:
        case Spell.GreaterEndurance:
        case Spell.GreaterFoxsCunning:
        case Spell.GreaterMagicFang:
        case Spell.GreaterMagicWeapon:
        case Spell.GreaterOwlsWisdom:
        case Spell.GreaterShadowConjurationMinorGlobe:
        case Spell.GreaterShadowConjurationMirrorImage:
        case Spell.GreaterSpellMantle:
        case Spell.GreaterStoneskin:
        case Spell.Stoneskin:
        case Spell.Haste:
        case Spell.HolyAura:
        case Spell.HolySword:
        case Spell.ImprovedInvisibility:
        case Spell.Invisibility:
        case Spell.InvisibilitySphere:
        case Spell.Ironguts:
        case Spell.KeenEdge:
        case Spell.LesserMindBlank:
        case Spell.LesserSpellMantle:
        case Spell.Light:
        case Spell.MageArmor:
        case Spell.MagicCircleAgainstChaos:
        case Spell.MagicCircleAgainstEvil:
        case Spell.MagicCircleAgainstGood:
        case Spell.MagicCircleAgainstLaw:
        case Spell.MagicFang:
        case Spell.MagicVestment:
        case Spell.MagicWeapon:
        case Spell.MassCamoflage:
        case Spell.MassHaste:
        case Spell.MestilsAcidSheath:
        case Spell.MindBlank:
        case Spell.MinorGlobeOfInvulnerability:
        case Spell.MonstrousRegeneration:
        case Spell.NegativeEnergyProtection:
        case Spell.OneWithTheLand:
        case Spell.OwlsInsight:
        case Spell.OwlsWisdom:
        case Spell.PolymorphSelf:
        case Spell.Prayer:
        case Spell.Premonition:
        case Spell.ProtectionFromChaos:
        case Spell.ProtectionFromElements:
        case Spell.ProtectionFromEvil:
        case Spell.ProtectionFromGood:
        case Spell.ProtectionFromLaw:
        case Spell.ProtectionFromSpells:
        case Spell.Regenerate:
        case Spell.Resistance:
        case Spell.ResistElements:
        case Spell.Sanctuary:
        case Spell.SeeInvisibility:
        case Spell.InvisibilityPurge:
        case Spell.ShadesStoneskin:
        case Spell.ShadowConjurationInivsibility:
        case Spell.ShadowConjurationMageArmor:
        case Spell.ShadowShield:
        case Spell.Shapechange:
        case Spell.Shield:
        case Spell.ShieldOfFaith:
        case Spell.ShieldOfLaw:
        case Spell.Silence:
        case Spell.SpellMantle:
        case Spell.SpellResistance:
        case Spell.StoneBones:
        case Spell.StoneToFlesh:
        case Spell.TensersTransformation:
        case Spell.TimeStop:
        case Spell.TrueSeeing:
        case Spell.TrueStrike:
        case Spell.TymorasSmile:
        case Spell.Virtue:
        case Spell.WarCry:
        case Spell.WoundingWhispers:
        case Spell.UnholyAura:
        case Spell.AbilityAuraBlinding:
        case Spell.AbilityAuraCold:
        case Spell.AbilityAuraElectricity:
        case Spell.AbilityAuraFear:
        case Spell.AbilityAuraFire:
        case Spell.AbilityAuraHorrificappearance:
        case Spell.AbilityAuraMenace:
        case Spell.AbilityAuraOfCourage:
        case Spell.AbilityAuraProtection:
        case Spell.AbilityAuraStun:
        case Spell.AbilityAuraUnearthlyVisage:
        case Spell.AbilityAuraUnnatural:
        case Spell.AbilityBarbarianRage:
        case Spell.AbilityFerocity1:
        case Spell.AbilityFerocity2:
        case Spell.AbilityFerocity3:
        case Spell.AbilityIntensity1:
        case Spell.AbilityIntensity2:
        case Spell.AbilityIntensity3:
        case Spell.AbilityHowlConfuse:
        case Spell.AbilityHowlDaze:
        case Spell.AbilityHowlDeath:
        case Spell.AbilityHowlDoom:
        case Spell.AbilityHowlFear:
        case Spell.AbilityHowlParalysis:
        case Spell.AbilityHowlSonic:
        case Spell.AbilityHowlStun:
        case Spell.AbilityDragonWingBuffet:
        case Spell.AbilityMummyBolsterUndead:
        case Spell.AbilityEpicMightyRage:
        case Spell.AbilityRage3:
        case Spell.AbilityRage4:
        case Spell.AbilityRage5:
        case Spell.AbilityTyrantFogMist:
        case Spell.Darkness:
        case Spell.ShadowConjurationDarkness:
        case Spell.LegendLore:
        case Spell.AbilityBattleMastery:
        case Spell.AbilityDivineStrength:
        case Spell.AbilityDivineProtection:
        case Spell.AbilityDivineTrickery:
        case Spell.AbilityRoguesCunning:
        case Spell.AbilityDragonFear:
        case Spell.Dirge:
        case Spell.ShadowEvade:
        case Spell.VineMineCamouflage:
        case Spell.DeafeningClang:
        case Spell.Blackstaff:
        case Spell.IounStoneBlue:
        case Spell.IounStoneDeepRed:
        case Spell.IounStoneDustyRose:
        case Spell.IounStonePaleBlue:
        case Spell.IounStonePink:
        case Spell.IounStonePinkGreen:
        case Spell.IounStoneScarletBlue:
        case Spell.AbilityTroglodyteStench:
          return true;
        default: return false;
      }
    }
    public static bool IgnoredSpellList(Spell spell)
    {
      switch (spell)
      {
        case Spell.AllSpells:
        case Spell.CloakOfChaos:
        case Spell.MagicCircleAgainstChaos:
        case Spell.MagicCircleAgainstLaw:
        case Spell.ProtectionFromChaos:
        case Spell.ProtectionFromLaw:
        case Spell.ShieldOfLaw:
        case Spell.SphereOfChaos:
        case Spell.AbilityDragonWingBuffet:
        case Spell.AbilitySmokeClaw:
        case Spell.AbilityTrumpetBlast:
        case Spell.AbilityQuiveringPalm:
        case Spell.AbilityEmptyBody:
        case Spell.AbilityDetectEvil:
        case Spell.AbilityAuraOfCourage:
        case Spell.AbilitySmiteEvil:
        case Spell.AbilityElementalShape:
        case Spell.AbilityWildShape:
        case Spell.ActivateItemPortal:
        case Spell.ActivateItemSelf2:
        case Spell.AbilityActivateItem:
        case Spell.CraftDyeMetalcolor1:
        case Spell.CraftDyeMetalcolor2:
        case Spell.CraftHarperItem:
        case Spell.CraftPoisonWeaponOrAmmo:
        case Spell.CraftAddItemProperty:
        case Spell.CraftCraftArmorSkill:
        case Spell.CraftCraftWeaponSkill:
        case Spell.CraftDyeClothcolor1:
        case Spell.CraftDyeClothcolor2:
        case Spell.CraftDyeLeathercolor1:
        case Spell.CraftDyeLeathercolor2:
        case Spell.GrenadeAcid:
        case Spell.GrenadeCaltrops:
        case Spell.GrenadeChicken:
        case Spell.GrenadeChoking:
        case Spell.GrenadeFire:
        case Spell.GrenadeHoly:
        case Spell.GrenadeTangle:
        case Spell.GrenadeThunderstone:
        case Spell.DivineMight:
        case Spell.DivineShield:
        case Spell.ShadowDaze:
        case Spell.ShadowEvade:
        case Spell.TymorasSmile:
        case Spell.TrapArrow:
        case Spell.TrapBolt:
        case Spell.TrapDart:
        case Spell.TrapShuriken:
        case Spell.RodOfWonder:
        case Spell.DeckOfManyThings:
        case Spell.ElementalSummoningItem:
        case Spell.DeckAvatar:
        case Spell.DeckButterflyspray:
        case Spell.DeckGemspray:
        case Spell.Powerstone:
        case Spell.KoboldJump:
        case Spell.Healingkit:
        case Spell.Spellstaff:
        case Spell.Decharger:
        case Spell.AbilityWhirlwind:
        case Spell.AbilityCommandTheHorde:
        case Spell.AbilityAaArrowOfDeath:
        case Spell.AbilityAaHailOfArrows:
        case Spell.AbilityAaImbueArrow:
        case Spell.AbilityAaSeekerArrow1:
        case Spell.AbilityAaSeekerArrow2:
        case Spell.AbilityAsDarkness:
        case Spell.AbilityAsGhostlyVisage:
        case Spell.AbilityAsImprovedInvisiblity:
        case Spell.AbilityAsInvisibility:
        case Spell.HorseAssignMount:
        case Spell.AbilityBgCreatedead:
        case Spell.AbilityBgFiendishServant:
        case Spell.AbilityBgInflictSeriousWounds:
        case Spell.AbilityBgInflictCriticalWounds:
        case Spell.AbilityBgContagion:
        case Spell.AbilityBgBullsStrength:
        case Spell.FlyingDebris:
        case Spell.AbilityDcDivineWrath:
        case Spell.AbilityPmAnimateDead:
        case Spell.AbilityPmSummonUndead:
        case Spell.AbilityPmUndeadGraft1:
        case Spell.AbilityPmUndeadGraft2:
        case Spell.AbilityPmSummonGreaterUndead:
        case Spell.AbilityPmDeathlessMasterTouch:
        case Spell.EpicHellball:
        case Spell.EpicMummyDust:
        case Spell.EpicDragonKnight:
        case Spell.EpicMageArmor:
        case Spell.EpicRuin:
        case Spell.AbilityDwDefensiveStance:
        case Spell.AbilityEpicMightyRage:
        case Spell.AbilityEpicCurseSong:
        case Spell.AbilityEpicImprovedWhirlwind:
        case Spell.AbilityEpicShapeDragonkin:
        case Spell.AbilityEpicShapeDragon:
        case Spell.HorseMenu:
        case Spell.HorseDismount:
        case Spell.HorseMount:
        case Spell.HorsePartyDismount:
        case Spell.HorsePartyMount:
        case Spell.PaladinSummonMount:
          return true;
        default: return false;
      }
    }
  }
}
