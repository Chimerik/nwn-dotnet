using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public enum SkillType
    {
      Invalid = 0,
      Skill = 1,
      Spell = 2,
    }
    public static Dictionary<int, Func<PlayerSystem.Player, int, int>> RegisterAddCustomFeatEffect = new Dictionary<int, Func<PlayerSystem.Player, int, int>>
    {
            { 1286, HandleHealthPoints },
            { 1287, HandleHealthPoints },
            { 1288, HandleHealthPoints },
            { 1289, HandleHealthPoints },
            { 1290, HandleHealthPoints },
            { 1156, HandleHealthPoints },
            { 1157, HandleHealthPoints },
            { 1158, HandleHealthPoints },
            { 1159, HandleHealthPoints },
            { 1160, HandleHealthPoints },
    };

    public static Dictionary<int, Func<PlayerSystem.Player, int, int>> RegisterRemoveCustomFeatEffect = new Dictionary<int, Func<PlayerSystem.Player, int, int>>
    {
            { 1130, HandleRemoveStrengthMalusFeat },
    };

    private static int HandleHealthPoints(PlayerSystem.Player player, int feat)
    {
      int improvedConst = CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.ImprovedConstitution);
      if (improvedConst == (int)Feat.Invalid)
        improvedConst = 0;
      else
        improvedConst = Int32.Parse(NWScript.Get2DAString("feat", "GAINMULTIPLE", improvedConst));

      CreaturePlugin.SetMaxHitPointsByLevel(player.oid, 1, Int32.Parse(NWScript.Get2DAString("classes", "HitDie", 43)) 
        + (1 + 3 * ((NWScript.GetAbilityScore(player.oid, NWScript.ABILITY_CONSTITUTION, 1)
        + improvedConst - 10) / 2) 
        + CreaturePlugin.GetKnowsFeat(player.oid, (int)Feat.Toughness)) * Int32.Parse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.ImprovedHealth))));
      return 0;
    }

    private static int HandleRemoveStrengthMalusFeat(PlayerSystem.Player player, int idMalusFeat)
    {
      player.removeableMalus.Remove(idMalusFeat);
      CreaturePlugin.SetRawAbilityScore(player.oid, NWScript.ABILITY_STRENGTH, CreaturePlugin.GetRawAbilityScore(player.oid, NWScript.ABILITY_STRENGTH) + 2);

      return 0;
    }
    
    public static Feat[] forgeBasicSkillBooks = new Feat[] { Feat.CraftOreExtractor, Feat.CraftForgeHammer, Feat.Metallurgy, Feat.Research, Feat.Miner, Feat.Prospection, Feat.StripMiner, Feat.Reprocessing, Feat.Forge, Feat.CraftScaleMail, Feat.CraftDagger, Feat.CraftLightMace, Feat.CraftMorningStar, Feat.CraftSickle, Feat.CraftShortSpear };
    public static Feat[] woodBasicSkillBooks = new Feat[] { Feat.CraftOreExtractor, Feat.CraftForgeHammer, Feat.Metallurgy, Feat.Research, Feat.WoodCutter, Feat.WoodProspection, Feat.StripMiner, Feat.WoodReprocessing, Feat.Ebeniste, Feat.CraftSmallShield, Feat.CraftClub, Feat.CraftDarts, Feat.CraftBullets, Feat.CraftHeavyCrossbow, Feat.CraftLightCrossbow, Feat.CraftQuarterstaff, Feat.CraftSling, Feat.CraftArrow, Feat.CraftBolt };
    public static Feat[] leatherBasicSkillBooks = new Feat[] { Feat.Hunting, Feat.Skinning, Feat.Tanner, Feat.PeltReprocessing, Feat.CraftLeatherArmor, Feat.CraftStuddedLeather, Feat.CraftPaddedArmor, Feat.CraftClothing, Feat.CraftWhip, Feat.CraftBelt, Feat.CraftBoots, Feat.CraftBracer, Feat.CraftCloak, Feat.CraftGloves };
    //public static Feat[] craftSkillBooks = new Feat[] { Feat.Metallurgy, Feat.AdvancedCraft, Feat.Miner, Feat.Geology, Feat.Prospection, Feat.VeldsparReprocessing, Feat.ScorditeReprocessing, Feat.PyroxeresReprocessing, Feat.StripMiner, Feat.Reprocessing, Feat.ReprocessingEfficiency, Feat.Connections, Feat.Forge };
    public static Feat[] languageSkillBooks = new Feat[] { Feat.LanguageAbyssal, Feat.LanguageCelestial, Feat.LanguageDeep, Feat.LanguageDraconic, Feat.LanguageDruidic, Feat.LanguageDwarf, Feat.LanguageElf, Feat.LanguageGiant, Feat.LanguageGoblin, Feat.LanguageHalfling, Feat.LanguageInfernal, Feat.LanguageOrc, Feat.LanguagePrimodial, Feat.LanguageSylvan, Feat.LanguageThieves, Feat.LanguageGnome };
    
    public static Feat[] lowSkillBooks = new Feat[] { Feat.Ambidexterity, Feat.Skinning, Feat.Hunting, Feat.ImprovedSpellSlot2_1, Feat.WoodReprocessing, Feat.Ebeniste, Feat.WoodCutter, Feat.WoodProspection, Feat.CraftOreExtractor, Feat.CraftForgeHammer, Feat.CraftLance, Feat.Forge, Feat.Reprocessing, Feat.BlueprintCopy, Feat.Research, Feat.Miner, Feat.Metallurgy, Feat.DeneirsEye, Feat.DirtyFighting, Feat.ResistDisease, Feat.Stealthy, Feat.SkillFocusAnimalEmpathy, Feat.SkillFocusBluff, Feat.SkillFocusConcentration, Feat.SkillFocusDisableTrap, Feat.SkillFocusDiscipline, Feat.SkillFocusHeal, Feat.SkillFocusHide, Feat.SkillFocusIntimidate, Feat.SkillFocusListen, Feat.SkillFocusLore, Feat.SkillFocusMoveSilently, Feat.SkillFocusOpenLock, Feat.SkillFocusParry, Feat.SkillFocusPerform, Feat.SkillFocusPickPocket, Feat.SkillFocusSearch, Feat.SkillFocusSetTrap, Feat.SkillFocusSpellcraft, Feat.SkillFocusSpot, Feat.SkillFocusTaunt, Feat.SkillFocusTumble, Feat.SkillFocusUseMagicDevice, Feat.PointBlankShot, Feat.IronWill, Feat.Alertness, Feat.CombatCasting, Feat.Dodge, Feat.ExtraTurning, Feat.GreatFortitude  };
    public static Feat[] mediumSkillBooks = new Feat[] { Feat.BadPeltReprocessing, Feat.CommonPeltReprocessing, Feat.NormalPeltReprocessing, Feat.UncommunPeltReprocessing, Feat.RarePeltReprocessing, Feat.MagicPeltReprocessing, Feat.EpicPeltReprocessing, Feat.LegendaryPeltReprocessing, Feat.ImprovedSpellSlot3_1, Feat.ImprovedSpellSlot4_1, Feat.LaurelinReprocessing, Feat.MallornReprocessing, Feat.TelperionReprocessing, Feat.OiolaireReprocessing, Feat.NimlothReprocessing, Feat.QlipothReprocessing, Feat.FerocheneReprocessing, Feat.ValinorReprocessing, Feat.WoodReprocessingEfficiency, Feat.AnimalExpertise, Feat.CraftTorch, Feat.CraftStuddedLeather, Feat.CraftSling, Feat.CraftSmallShield, Feat.CraftSickle, Feat.CraftShortSpear, Feat.CraftRing, Feat.CraftPaddedArmor , Feat.CraftPotion, Feat.CraftQuarterstaff, Feat.CraftMorningStar, Feat.CraftMagicWand, Feat.CraftLightMace, Feat.CraftLightHammer, Feat.CraftLightFlail, Feat.CraftLightCrossbow, Feat.CraftLeatherArmor, Feat.CraftBullets, Feat.CraftCloak, Feat.CraftClothing, Feat.CraftClub, Feat.CraftDagger, Feat.CraftDarts, Feat.CraftGloves, Feat.CraftHeavyCrossbow, Feat.CraftHelmet, Feat.CraftAmulet, Feat.CraftArrow, Feat.CraftBelt, Feat.CraftBolt, Feat.CraftBoots, Feat.CraftBracer,  Feat.ReprocessingEfficiency, Feat.StripMiner, Feat.VeldsparReprocessing, Feat.ScorditeReprocessing, Feat.PyroxeresReprocessing, Feat.PlagioclaseReprocessing, Feat.Geology, Feat.Prospection, Feat.TymorasSmile, Feat.LliirasHeart, Feat.RapidReload, Feat.Expertise, Feat.ImprovedInitiative, Feat.DefensiveRoll, Feat.SneakAttack, Feat.FlurryOfBlows, Feat.WeaponSpecializationHeavyCrossbow, Feat.WeaponSpecializationDagger, Feat.WeaponSpecializationDart, Feat.WeaponSpecializationClub, Feat.StillSpell, Feat.RapidShot, Feat.SilenceSpell, Feat.PowerAttack, Feat.Knockdown, Feat.LightningReflexes, Feat.ImprovedUnarmedStrike, Feat.Cleave, Feat.CalledShot, Feat.DeflectArrows, Feat.WeaponSpecializationLightCrossbow, Feat.WeaponSpecializationLightFlail, Feat.WeaponSpecializationLightMace, Feat.Disarm, Feat.EmpowerSpell, Feat.WeaponSpecializationMorningStar, Feat.ExtendSpell, Feat.SpellFocusAbjuration, Feat.SpellFocusConjuration, Feat.SpellFocusDivination, Feat.SpellFocusEnchantment, Feat.WeaponSpecializationSickle, Feat.WeaponSpecializationSling, Feat.WeaponSpecializationSpear, Feat.WeaponSpecializationStaff, Feat.WeaponSpecializationThrowingAxe, Feat.WeaponSpecializationTrident, Feat.WeaponSpecializationUnarmedStrike, Feat.SpellFocusEvocation, Feat.SpellFocusIllusion, Feat.SpellFocusNecromancy, Feat.SpellFocusTransmutation, Feat.SpellPenetration };
    public static Feat[] highSkillBooks = new Feat[] { Feat.AdvancedCraft, Feat.CraftWarHammer, Feat.CraftTrident, Feat.CraftThrowingAxe, Feat.CraftStaff, Feat.CraftSplintMail, Feat.CraftSpellScroll, Feat.CraftShortsword, Feat.CraftShortBow, Feat.CraftScimitar, Feat.CraftScaleMail, Feat.CraftRapier, Feat.CraftMagicRod, Feat.CraftLongsword, Feat.CraftLongBow, Feat.CraftLargeShield , Feat.CraftBattleAxe, Feat.OmberReprocessing, Feat.KerniteReprocessing, Feat.GneissReprocessing, Feat.CraftHalberd, Feat.JaspetReprocessing, Feat.CraftHeavyFlail, Feat.CraftHandAxe, Feat.HemorphiteReprocessing, Feat.CraftGreatAxe, Feat.CraftGreatSword, Feat.ArcaneDefenseAbjuration, Feat.ArcaneDefenseConjuration, Feat.ArcaneDefenseDivination, Feat.ArcaneDefenseEnchantment, Feat.ArcaneDefenseEvocation, Feat.ArcaneDefenseIllusion, Feat.ArcaneDefenseNecromancy, Feat.ArcaneDefenseTransmutation, Feat.BlindFight, Feat.SpringAttack, Feat.GreatCleave, Feat.ImprovedExpertise, Feat.SkillMastery, Feat.Opportunist, Feat.Evasion, Feat.WeaponSpecializationDireMace, Feat.WeaponSpecializationDoubleAxe, Feat.WeaponSpecializationDwaxe, Feat.WeaponSpecializationGreatAxe, Feat.WeaponSpecializationGreatSword, Feat.WeaponSpecializationHalberd, Feat.WeaponSpecializationHandAxe, Feat.WeaponSpecializationHeavyFlail, Feat.WeaponSpecializationKama, Feat.WeaponSpecializationKatana, Feat.WeaponSpecializationKukri,  Feat.WeaponSpecializationBastardSword, Feat.WeaponSpecializationLightHammer, Feat.WeaponSpecializationLongbow, Feat.WeaponSpecializationLongSword, Feat.WeaponSpecializationRapier, Feat.WeaponSpecializationScimitar, Feat.WeaponSpecializationScythe, Feat.WeaponSpecializationShortbow, Feat.WeaponSpecializationShortSword, Feat.WeaponSpecializationShuriken, Feat.WeaponSpecializationBattleAxe, Feat.QuickenSpell, Feat.MaximizeSpell, Feat.ImprovedTwoWeaponFighting, Feat.ImprovedPowerAttack, Feat.WeaponSpecializationTwoBladedSword, Feat.WeaponSpecializationWarHammer, Feat.WeaponSpecializationWhip, Feat.ImprovedDisarm, Feat.ImprovedKnockdown, Feat.ImprovedParry, Feat.ImprovedCriticalBastardSword, Feat.ImprovedCriticalBattleAxe, Feat.ImprovedCriticalClub, Feat.ImprovedCriticalDagger, Feat.ImprovedCriticalDart, Feat.ImprovedCriticalDireMace, Feat.ImprovedCriticalDoubleAxe, Feat.ImprovedCriticalDwaxe, Feat.ImprovedCriticalGreatAxe, Feat.ImprovedCriticalGreatSword, Feat.ImprovedCriticalHalberd, Feat.ImprovedCriticalHandAxe, Feat.ImprovedCriticalHeavyCrossbow, Feat.ImprovedCriticalHeavyFlail, Feat.ImprovedCriticalKama, Feat.ImprovedCriticalKatana, Feat.ImprovedCriticalKukri, Feat.ImprovedCriticalLightCrossbow, Feat.ImprovedCriticalLightFlail, Feat.ImprovedCriticalLightHammer, Feat.ImprovedCriticalLightMace, Feat.ImprovedCriticalLongbow, Feat.ImprovedCriticalLongSword, Feat.ImprovedCriticalMorningStar, Feat.ImprovedCriticalRapier, Feat.ImprovedCriticalScimitar, Feat.ImprovedCriticalScythe, Feat.ImprovedCriticalShortbow, Feat.ImprovedCriticalShortSword, Feat.ImprovedCriticalShuriken, Feat.ImprovedCriticalSickle, Feat.ImprovedCriticalSling, Feat.ImprovedCriticalSpear, Feat.ImprovedCriticalStaff, Feat.ImprovedCriticalThrowingAxe, Feat.ImprovedCriticalTrident, Feat.ImprovedCriticalTwoBladedSword, Feat.ImprovedCriticalUnarmedStrike, Feat.ImprovedCriticalWarHammer, Feat.ImprovedCriticalWhip };
    public static Feat[] epicSkillBooks = new Feat[] { Feat.CraftWhip, Feat.CraftTwoBladedSword, Feat.CraftTowerShield, Feat.CraftShuriken, Feat.CraftScythe, Feat.CraftKukri, Feat.CraftKatana, Feat.CraftBreastPlate, Feat.CraftDireMace, Feat.CraftDoubleAxe, Feat.CraftDwarvenWarAxe, Feat.CraftFullPlate, Feat.CraftHalfPlate, Feat.CraftBastardSword, Feat.CraftKama, Feat.DarkOchreReprocessing, Feat.CrokiteReprocessing, Feat.BistotReprocessing, Feat.ResistEnergyAcid, Feat.ResistEnergyCold, Feat.ResistEnergyElectrical, Feat.ResistEnergyFire, Feat.ResistEnergySonic, Feat.ZenArchery, Feat.CripplingStrike, Feat.SlipperyMind, Feat.GreaterSpellFocusAbjuration, Feat.GreaterSpellFocusConjuration, Feat.GreaterSpellFocusDivination, Feat.GreaterSpellFocusDiviniation, Feat.GreaterSpellFocusEnchantment, Feat.GreaterSpellFocusEvocation, Feat.GreaterSpellFocusIllusion, Feat.GreaterSpellFocusNecromancy, Feat.GreaterSpellFocusTransmutation, Feat.GreaterSpellPenetration };

    public static int[] shopBasicMagicScrolls = new int[] { NWScript.IP_CONST_CASTSPELL_ACID_SPLASH_1, NWScript.IP_CONST_CASTSPELL_DAZE_1, NWScript.IP_CONST_CASTSPELL_ELECTRIC_JOLT_1, NWScript.IP_CONST_CASTSPELL_FLARE_1, NWScript.IP_CONST_CASTSPELL_RAY_OF_FROST_1, NWScript.IP_CONST_CASTSPELL_RESISTANCE_5, NWScript.IP_CONST_CASTSPELL_BURNING_HANDS_5, NWScript.IP_CONST_CASTSPELL_CHARM_PERSON_2, NWScript.IP_CONST_CASTSPELL_COLOR_SPRAY_2, NWScript.IP_CONST_CASTSPELL_ENDURE_ELEMENTS_2, NWScript.IP_CONST_CASTSPELL_EXPEDITIOUS_RETREAT_5, NWScript.IP_CONST_CASTSPELL_GREASE_2, 459, 478, 460, NWScript.IP_CONST_CASTSPELL_MAGE_ARMOR_2, NWScript.IP_CONST_CASTSPELL_MAGIC_MISSILE_5, NWScript.IP_CONST_CASTSPELL_NEGATIVE_ENERGY_RAY_5, NWScript.IP_CONST_CASTSPELL_RAY_OF_ENFEEBLEMENT_2, NWScript.IP_CONST_CASTSPELL_SCARE_2, 469, NWScript.IP_CONST_CASTSPELL_SHIELD_5, NWScript.IP_CONST_CASTSPELL_SLEEP_5, NWScript.IP_CONST_CASTSPELL_SUMMON_CREATURE_I_5, NWScript.IP_CONST_CASTSPELL_AMPLIFY_5, NWScript.IP_CONST_CASTSPELL_BALAGARNSIRONHORN_7, NWScript.IP_CONST_CASTSPELL_LESSER_DISPEL_5, NWScript.IP_CONST_CASTSPELL_CURE_MINOR_WOUNDS_1, NWScript.IP_CONST_CASTSPELL_INFLICT_MINOR_WOUNDS_1, NWScript.IP_CONST_CASTSPELL_VIRTUE_1, NWScript.IP_CONST_CASTSPELL_BANE_5, NWScript.IP_CONST_CASTSPELL_BLESS_2, NWScript.IP_CONST_CASTSPELL_CURE_LIGHT_WOUNDS_5, NWScript.IP_CONST_CASTSPELL_DIVINE_FAVOR_5, NWScript.IP_CONST_CASTSPELL_DOOM_5, NWScript.IP_CONST_CASTSPELL_ENTROPIC_SHIELD_5, NWScript.IP_CONST_CASTSPELL_INFLICT_LIGHT_WOUNDS_5, NWScript.IP_CONST_CASTSPELL_REMOVE_FEAR_2, NWScript.IP_CONST_CASTSPELL_SANCTUARY_2, NWScript.IP_CONST_CASTSPELL_SHIELD_OF_FAITH_5, NWScript.IP_CONST_CASTSPELL_CAMOFLAGE_5, NWScript.IP_CONST_CASTSPELL_ENTANGLE_5, NWScript.IP_CONST_CASTSPELL_MAGIC_FANG_5 };
  }
}
