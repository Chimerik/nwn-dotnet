using System;
using System.Collections.Generic;
using NLog;
using NWN.API;
using NWN.API.Constants;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public enum SkillType
    {
      Invalid = 0,
      Skill = 1,
      Spell = 2,
    }

    public static Dictionary<Feat, CustomFeat> customFeatsDictionnary = new Dictionary<Feat, CustomFeat>()
    {
      { CustomFeats.BlueprintCopy, new CustomFeat("Copie patron", "Permet la copie de patrons pour l'artisanat.\n\n Diminue le temps de copie de 5 % par niveau.", 5) },
      { CustomFeats.Research, new CustomFeat("Recherche patron", "Permet de rechercher une amélioration pour un patron.\n\n Diminue le temps de recherche de 5 % par niveau.", 5) },
      { CustomFeats.ImprovedStrength, new CustomFeat("Force accrue", "Augmente la force d'un point par niveau d'entraînement.", 6) },
      { CustomFeats.ImprovedDexterity, new CustomFeat("Dextérité accrue", "Augmente la dextérité d'un point par niveau d'entraînement.", 6) },
      { CustomFeats.ImprovedConstitution, new CustomFeat("Constitution accrue", "Augmente la constitution d'un point par niveau d'entraînement.", 6) },
      { CustomFeats.ImprovedIntelligence, new CustomFeat("Intelligence accrue", "Augmente l'intelligence d'un point par niveau d'entraînement.", 6) },
      { CustomFeats.ImprovedWisdom, new CustomFeat("Sagesse accrue", "Augmente la sagesse d'un point par niveau d'entraînement.", 6) },
      { CustomFeats.ImprovedCharisma, new CustomFeat("Charisme accru", "Augmente le charisme d'un point par niveau d'entraînement.", 6) },
      { CustomFeats.ImprovedAnimalEmpathy, new CustomFeat("Empathie Animale accrue", "Augmente la compétence Empathie Animale d'un point par niveau d'entraînement.", 12) },
      { CustomFeats.ImprovedConcentration, new CustomFeat("Concentration accrue", "Augmente la compétence Concentration d'un point par niveau d'entraînement.", 12) },
      { CustomFeats.ImprovedDisableTraps, new CustomFeat("Désamorçage accrue", "Augmente la compétence Désamorçage d'un point par niveau d'entraînement.", 12) },
      { CustomFeats.ImprovedDiscipline, new CustomFeat("Discipline accrue", "Augmente la compétence Discipline d'un point par niveau d'entraînement.", 12) },
      { CustomFeats.ImprovedHeal, new CustomFeat("Premiers Soins accrue", "Augmente la compétence Premiers Soins d'un point par niveau d'entraînement.", 12) },
      { CustomFeats.ImprovedHide, new CustomFeat("Discrétion accrue", "Augmente la compétence Discrétion d'un point par niveau d'entraînement.", 12) },
      { CustomFeats.ImprovedListen, new CustomFeat("Perception accrue", "Augmente la compétence Perception d'un point par niveau d'entraînement.", 12) },
      { CustomFeats.ImprovedLore, new CustomFeat("Savoir accrue", "Augmente la compétence Savoir d'un point par niveau d'entraînement.", 12) },
      { CustomFeats.ImprovedMoveSilently, new CustomFeat("Déplacement Silencieux accrue", "Augmente la compétence Déplacement Silencieux d'un point par niveau d'entraînement.", 12) },
      { CustomFeats.ImprovedOpenLock, new CustomFeat("Crochetage accrue", "Augmente la compétence Crochetage d'un point par niveau d'entraînement.", 12) },
      { CustomFeats.ImprovedSkillParry, new CustomFeat("Parade accrue", "Augmente la compétence Parade d'un point par niveau d'entraînement.", 12) },
      { CustomFeats.ImprovedPerform, new CustomFeat("Représentation accrue", "Augmente la compétence Représentation d'un point par niveau d'entraînement.", 12) },
      { CustomFeats.ImprovedPickpocket, new CustomFeat("Vol à la tir accrue", "Augmente la compétence Vol à la tir d'un point par niveau d'entraînement.", 12) },
      { CustomFeats.ImprovedSearch, new CustomFeat("Fouille accrue", "Augmente la compétence Fouille d'un point par niveau d'entraînement.", 12) },
      { CustomFeats.ImprovedSetTrap, new CustomFeat("Pose de Piège accrue", "Augmente la compétence Pose de Piège d'un point par niveau d'entraînement.", 12) },
      { CustomFeats.ImprovedSpellcraft, new CustomFeat("Connaissance des Sorts accrue", "Augmente la compétence Connaissance des Sorts d'un point par niveau d'entraînement.", 12) },
      { CustomFeats.ImprovedSpot, new CustomFeat("Détection accrue", "Augmente la compétence Détection d'un point par niveau d'entraînement.", 12) },
      { CustomFeats.ImprovedTaunt, new CustomFeat("Raillerie accrue", "Augmente la compétence Raillerie d'un point par niveau d'entraînement.", 12) },
      { CustomFeats.ImprovedUseMagicDevice, new CustomFeat("Utilisation d'Objets Magiques accrue", "Augmente la compétence Utilisation d'Objets Magiques d'un point par niveau d'entraînement.", 12) },
      { CustomFeats.ImprovedTumble, new CustomFeat("Acrobatie accrue", "Augmente la compétence Acrobatie d'un point par niveau d'entraînement.", 12) },
      { CustomFeats.ImprovedBluff, new CustomFeat("Bluff accrue", "Augmente la compétence Bluff d'un point par niveau d'entraînement.", 12) },
      { CustomFeats.ImprovedIntimidate, new CustomFeat("Intimidation accrue", "Augmente la compétence Intimidation d'un point par niveau d'entraînement.", 12) },
      { CustomFeats.ImprovedHealth, new CustomFeat("Résilience accrue", "Augmente les points de vie de de 1 + (Robustesse + 3 * modificateur de constitution de base) par niveau.\n\n Ce don est rétroactif.", 6) },
      { CustomFeats.ImprovedAttackBonus, new CustomFeat("Attaque accrue", "Améliore le bonus d'attaque de base d'un point par niveau.", 12) },
      { CustomFeats.ImprovedSpellSlot0, new CustomFeat("Emplacement Cercle 0", "Augmente le nombre d'emplacements de sorts de cercle 0 disponibles d'un par niveau.", 10) },
      { CustomFeats.ImprovedSpellSlot1, new CustomFeat("Emplacement Cercle 1", "Augmente le nombre d'emplacements de sorts de cercle 1 disponibles d'un par niveau.", 10) },
      { CustomFeats.ImprovedSpellSlot2, new CustomFeat("Emplacement Cercle 2", "Augmente le nombre d'emplacements de sorts de cercle 2 disponibles d'un par niveau.", 10) },
      { CustomFeats.ImprovedSpellSlot3, new CustomFeat("Emplacement Cercle 3", "Augmente le nombre d'emplacements de sorts de cercle 3 disponibles d'un par niveau.", 10) },
      { CustomFeats.ImprovedSpellSlot4, new CustomFeat("Emplacement Cercle 4", "Augmente le nombre d'emplacements de sorts de cercle 4 disponibles d'un par niveau.", 10) },
      { CustomFeats.ImprovedSpellSlot5, new CustomFeat("Emplacement Cercle 5", "Augmente le nombre d'emplacements de sorts de cercle 5 disponibles d'un par niveau.", 10) },
      { CustomFeats.ImprovedSpellSlot6, new CustomFeat("Emplacement Cercle 6", "Augmente le nombre d'emplacements de sorts de cercle 6 disponibles d'un par niveau.", 10) },
      { CustomFeats.ImprovedSpellSlot7, new CustomFeat("Emplacement Cercle 7", "Augmente le nombre d'emplacements de sorts de cercle 7 disponibles d'un par niveau.", 10) },
      { CustomFeats.ImprovedSpellSlot8, new CustomFeat("Emplacement Cercle 8", "Augmente le nombre d'emplacements de sorts de cercle 8 disponibles d'un par niveau.", 10) },
      { CustomFeats.ImprovedSpellSlot9, new CustomFeat("Emplacement Cercle 9", "Augmente le nombre d'emplacements de sorts de cercle 9 disponibles d'un par niveau.", 10) },
      { CustomFeats.ImprovedCasterLevel, new CustomFeat("Caster Level", "Augmente le niveau de lanceur de sorts de un par niveau.", 12) },
      { CustomFeats.ImprovedSavingThrowAll, new CustomFeat("JdS Universel ", "Augmente le jet de sauvegarde universel d'un point par niveau.", 6) },
      { CustomFeats.ImprovedSavingThrowFortitude, new CustomFeat("JdS Vigueur", "Augmente le jet de sauvegarde de vigueur d'un point par niveau.", 6) },
      { CustomFeats.ImprovedSavingThrowReflex, new CustomFeat("JdS Réflexes", "Augmente le jet de sauvegarde de réflexes d'un point par niveau.", 6) },
      { CustomFeats.ImprovedSavingThrowWill, new CustomFeat("JdS Volonté", "Augmente le jet de sauvegarde de volonté d'un point par niveau.", 6) },
      { CustomFeats.Metallurgy, new CustomFeat("Métallurgie", "Diminue le temps de recherche d'un patron en efficacité matérielle de 5 % par niveau.", 5) },
      { CustomFeats.AdvancedCraft, new CustomFeat("Artisanat Avancé", "Diminue le temps de recherche d'un patron en efficacité de production matérielle et temporelle de 3 % par niveau.", 5) },
      { CustomFeats.Miner, new CustomFeat("Mineur", "Augmente la quantité de minerai extrait par cycle de 5 % par niveau.", 10) },
      { CustomFeats.Geology, new CustomFeat("Géologie", "Augmente la quantité de minerai extrait par cycle de 5 % par niveau.\n\nAugmente les chances de trouver un filon lors de la prospection de 5 % par niveau.", 10) },
      { CustomFeats.Prospection, new CustomFeat("Prospection", "Augmente les chances de trouver un filon de minerai brut lors de la prospection de 5 % par niveau.", 5) },
      { CustomFeats.VeldsparReprocessing, new CustomFeat("Raffinage Veldspar", "Réduit la quantité de Veldspar gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeats.ScorditeReprocessing, new CustomFeat("Raffinage Scordite", "Réduit la quantité de Scordite gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeats.PyroxeresReprocessing, new CustomFeat("Raffinage Pyroxeres", "Réduit la quantité de Pyroxeres gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeats.PlagioclaseReprocessing, new CustomFeat("Raffinage Plagioclase", "Réduit la quantité de Plagioclase gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeats.OmberReprocessing, new CustomFeat("Raffinage Omber", "Réduit la quantité d'Omber gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeats.KerniteReprocessing, new CustomFeat("Raffinage Kernite", "Réduit la quantité de Kernite gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeats.GneissReprocessing, new CustomFeat("Raffinage Gneiss", "Réduit la quantité de Gneiss gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeats.JaspetReprocessing, new CustomFeat("Raffinage Jaspet", "Réduit la quantité de Jaspet gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeats.HemorphiteReprocessing, new CustomFeat("Raffinage Hémorphite", "Réduit la quantité d'Hémorphite gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeats.HedbergiteReprocessing, new CustomFeat("Raffinage Hedbergite", "Réduit la quantité d'Hedbergite gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeats.DarkOchreReprocessing, new CustomFeat("Raffinage Darkochre", "Réduit la quantité de Darkochre gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeats.CrokiteReprocessing, new CustomFeat("Raffinage Crokite", "Réduit la quantité de Crokite gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeats.BistotReprocessing, new CustomFeat("Raffinage Bistot", "Réduit la quantité de Bistot gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeats.BezdnacineReprocessing, new CustomFeat("Raffinage Bezdnacine", "Réduit la quantité de Bezdnacine gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeats.ArkonorReprocessing, new CustomFeat("Raffinage Arkonor", "Réduit la quantité d'Arkonor gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeats.MercoxitReprocessing, new CustomFeat("Raffinage Mercoxit", "Réduit la quantité de Mercoxit gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeats.StripMiner, new CustomFeat("Minage par Fracturation", "Augmente la quantité de minerai extrait par cycle de 5 % par niveau.", 10) },
      { CustomFeats.Reprocessing, new CustomFeat("Raffinage", "Réduit la quantité de minerai gaché lors du raffinage de 3 % par niveau.", 5) },
      { CustomFeats.ReprocessingEfficiency, new CustomFeat("Raffinage efficace", "Réduit la quantité de minerai gaché lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeats.Connections, new CustomFeat("Relations", "Diminue la taxe de raffinage exigée par l'Amirauté de 5 % par niveau.", 5) },
      { CustomFeats.Forge, new CustomFeat("Forge", "Diminue le temps de fabrication et le coût en matériaux d'un objet de la forge de 1 % par niveau.", 10) },
      { CustomFeats.CraftClothing, new CustomFeat("Craft Vêtements", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftFullPlate, new CustomFeat("Craft Harnois", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftHalfPlate, new CustomFeat("Craft Armure de Plaques", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftSplintMail, new CustomFeat("Craft Clibanion", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftBreastPlate, new CustomFeat("Craft Cuirasse", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftScaleMail, new CustomFeat("Craft Chemise de mailles", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftStuddedLeather, new CustomFeat("Craft Cuir clouté", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftLeatherArmor, new CustomFeat("Craft Armure de cuir", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftPaddedArmor, new CustomFeat("Craft Armure matelassée", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftShortsword, new CustomFeat("Craft Epée courte", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftLongsword, new CustomFeat("Craft Epée longue", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftBattleAxe, new CustomFeat("Craft Hache d'armes", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftBastardSword, new CustomFeat("Craft Epée bâtarde", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftLightFlail, new CustomFeat("Craft Fléau léger", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftWarHammer, new CustomFeat("Craft Marteau de guerre", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftHeavyCrossbow, new CustomFeat("Craft Arbalète lourde", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftLightCrossbow, new CustomFeat("Craft Arbalète légère", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftLongBow, new CustomFeat("Craft Arc long", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftLightMace, new CustomFeat("Craft Masse légère", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftHalberd, new CustomFeat("Craft Hallebarde", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftShortBow, new CustomFeat("Craft Arc court", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftTwoBladedSword, new CustomFeat("Craft Double lame", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftGreatSword, new CustomFeat("Craft Epée à deux mains", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftSmallShield, new CustomFeat("Craft Rondache", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftTorch, new CustomFeat("Craft Torche", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftHelmet, new CustomFeat("Craft Heaume", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftGreatAxe, new CustomFeat("Craft Grande Hache", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftAmulet, new CustomFeat("Craft Amulette", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftArrow, new CustomFeat("Craft Flèche", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftBelt, new CustomFeat("Craft Ceinture", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftDagger, new CustomFeat("Craft Dague", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftBolt, new CustomFeat("Craft Carreau", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftBoots, new CustomFeat("Craft Bottes", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftBullets, new CustomFeat("Craft Billes", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftClub, new CustomFeat("Craft Gourdin", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftDarts, new CustomFeat("Craft Dards", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftDireMace, new CustomFeat("Craft Masse double", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftHeavyFlail, new CustomFeat("Craft Fléau lourd", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftGloves, new CustomFeat("Craft Gants", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftLightHammer, new CustomFeat("Craft Marteau léger", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftHandAxe, new CustomFeat("Craft Hachette", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftKama, new CustomFeat("Craft Kama", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftKukri, new CustomFeat("Craft Kukri", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftMagicRod, new CustomFeat("Craft Baguette", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftStaff, new CustomFeat("Craft Bourdon", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftMagicWand, new CustomFeat("Craft Baguette", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftMorningStar, new CustomFeat("Craft Morgenstern", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftPotion, new CustomFeat("Craft Potion", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftQuarterstaff, new CustomFeat("Craft Bâton", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftRapier, new CustomFeat("Craft Rapière", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftRing, new CustomFeat("Craft Anneau", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftScimitar, new CustomFeat("Craft Cimeterre", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftScythe, new CustomFeat("Craft Faux", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftLargeShield, new CustomFeat("Craft Ecu", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftTowerShield, new CustomFeat("Craft Pavois", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftShortSpear, new CustomFeat("Craft Lance", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftShuriken, new CustomFeat("Craft Shuriken", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftSickle, new CustomFeat("Craft Serpe", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftSling, new CustomFeat("Craft Fronde", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftThrowingAxe, new CustomFeat("Craft Hache de jet", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftSpellScroll, new CustomFeat("Craft Parchemin", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftBracer, new CustomFeat("Craft Brassard", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftCloak, new CustomFeat("Craft Cape", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftTrident, new CustomFeat("Craft Trident", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftDwarvenWarAxe, new CustomFeat("Craft Hache naine", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftWhip, new CustomFeat("Craft Fouet", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftDoubleAxe, new CustomFeat("Craft Double Hache", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftForgeHammer, new CustomFeat("Craft Marteau d'artisan", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftKatana, new CustomFeat("Craft Katana", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.CraftOreExtractor, new CustomFeat("Craft Extracteur de ressources", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeats.WoodCutter, new CustomFeat("Bûcheron", "Augmente la quantité de bois brut extrait par cycle de 5 % par niveau.", 10) },
      { CustomFeats.WoodExpertise, new CustomFeat("Dendrologie", "Augmente la quantité de bois brut extrait par cycle de 5 % par niveau.\n\nAugmente les chances d'identifier un arbre exploitable lors de la prospection de 5 % par niveau.", 5) },
      { CustomFeats.WoodProspection, new CustomFeat("Prospection Arboricole", "Augmente les chances d'identifier un arbre exploitable lors de la prospection de 5 % par niveau.", 5) },
      { CustomFeats.Skinning, new CustomFeat("Equarisseur", "Augmente la quantité de viande et de cuir brut extraits par cycle de 5 % par niveau.", 10) },
      { CustomFeats.AnimalExpertise, new CustomFeat("Zoologie", "Augmente la quantité de viande et de cuir brut extraits par cycle de 5 % par niveau.\n\nAugmente les chances de répérer une proie exploitable lors de la prospection de 5 % par niveau.", 5) },
      { CustomFeats.Hunting, new CustomFeat("Traque animale", "Augmente les chances de répérer une proie exploitable lors de la prospection de 5 % par niveau.", 5) },
      { CustomFeats.Ebeniste, new CustomFeat("Ebeniste", "Réduit le coût en matériau raffiné et la durée du travail de 1 % par niveau.", 10) },
      { CustomFeats.WoodReprocessing, new CustomFeat("Sciage", "Réduit la quantité de bois gaché lors du sciage de 3 % par niveau.", 5) },
      { CustomFeats.WoodReprocessingEfficiency, new CustomFeat("Sciage efficace", "Réduit la quantité de bois gaché lors du sciage de 2 % par niveau.", 5) },
      { CustomFeats.LaurelinReprocessing, new CustomFeat("Raffinage Laurelin", "Réduit la quantité de bois gaché lors du sciage de Laurelin de 2 % par niveau.", 5) },
      { CustomFeats.TelperionReprocessing, new CustomFeat("Raffinage Telperion", "Réduit la quantité de bois gaché lors du sciage de Telperion de 2 % par niveau.", 5) },
      { CustomFeats.MallornReprocessing, new CustomFeat("Raffinage Mallorn", "Réduit la quantité de bois gaché lors du sciage de Mallorn de 2 % par niveau.", 5) },
      { CustomFeats.NimlothReprocessing, new CustomFeat("Raffinage Nimloth", "Réduit la quantité de bois gaché lors du sciage de Nimloth de 2 % par niveau.", 5) },
      { CustomFeats.OiolaireReprocessing, new CustomFeat("Raffinage Oiolaire", "Réduit la quantité de bois gaché lors du sciage de Oiolaire de 2 % par niveau.", 5) },
      { CustomFeats.QlipothReprocessing, new CustomFeat("Raffinage Qlipoth", "Réduit la quantité de bois gaché lors du sciage de Qlipoth de 2 % par niveau.", 5) },
      { CustomFeats.FerocheneReprocessing, new CustomFeat("Raffinage Férochêne", "Réduit la quantité de bois gaché lors du sciage de Férochêne de 2 % par niveau.", 5) },
      { CustomFeats.ValinorReprocessing, new CustomFeat("Raffinage Valinor", "Réduit la quantité de bois gaché lors du sciage de Valinor de 2 % par niveau.", 5) },
      { CustomFeats.Tanner, new CustomFeat("Maroquinier", "Réduit le coût en matériau raffiné et la durée du travail de 1 % par niveau.", 5) },
      { CustomFeats.PeltReprocessing, new CustomFeat("Tanneur", "Réduit la quantité de peaux gachées lors du tannage de 3 % par niveau.", 5) },
      { CustomFeats.PeltReprocessingEfficiency, new CustomFeat("Tannage efficace", "Réduit la quantité de peaux gachées lors du tannage de 2 % par niveau.", 5) },
      { CustomFeats.BadPeltReprocessing, new CustomFeat("Tannage mauvaises peaux", "Réduit la quantité de peaux gachées lors du tannage des mauvaises peaux de 2 % par niveau.", 5) },
      { CustomFeats.CommonPeltReprocessing, new CustomFeat("Tannage peaux communes", "Réduit la quantité de peaux gachées lors du tannage des mauvaises peaux de 2 % par niveau.", 5) },
      { CustomFeats.NormalPeltReprocessing, new CustomFeat("Tannage peaux normales", "Réduit la quantité de peaux gachées lors du tannage des mauvaises peaux de 2 % par niveau.", 5) },
      { CustomFeats.UncommunPeltReprocessing, new CustomFeat("Tannage peaux inhabituelles", "Réduit la quantité de peaux gachées lors du tannage des mauvaises peaux de 2 % par niveau.", 5) },
      { CustomFeats.RarePeltReprocessing, new CustomFeat("Tannage peaux rares", "Réduit la quantité de peaux gachées lors du tannage des mauvaises peaux de 2 % par niveau.", 5) },
      { CustomFeats.MagicPeltReprocessing, new CustomFeat("Tannage peaux magiques", "Réduit la quantité de peaux gachées lors du tannage des mauvaises peaux de 2 % par niveau.", 5) },
      { CustomFeats.EpicPeltReprocessing, new CustomFeat("Tannage peaux épiques", "Réduit la quantité de peaux gachées lors du tannage des mauvaises peaux de 2 % par niveau.", 5) },
      { CustomFeats.LegendaryPeltReprocessing, new CustomFeat("Tannage peaux légendaires", "Réduit la quantité de peaux gachées lors du tannage des mauvaises peaux de 2 % par niveau.", 5) },
      { CustomFeats.Recycler, new CustomFeat("Recyclage", "Permet de recycler des objets en matière raffinée.\n\n Diminue le temps nécessaire au recyclage et augmente le rendement de 1 % par niveau.", 20) },
      { CustomFeats.ContractScience, new CustomFeat("Science du contrat", "Permet de créer un contrat supplémentaire par niveau (enchères comprises).", 5) },
      { CustomFeats.Marchand, new CustomFeat("Marchand", "Permet de vendre cinq objets supplémentaires par échoppe.", 20) },
      { CustomFeats.Magnat, new CustomFeat("Magnat", "Permet d'ouvrir une échoppe supplémentaire par niveau.", 5) },
      { CustomFeats.Negociateur, new CustomFeat("Négociateur", "Permet d'enregistrer 3 ordres supplémentaires à l'Hôtel des ventes.", 10) },
      { CustomFeats.BrokerRelations, new CustomFeat("Relations Courtières", "Réduit de 6 % par niveau la taxe de courtage.", 5) },
      { CustomFeats.BrokerAffinity, new CustomFeat("Affinités Courtières", "Réduit de 6 % par niveau la taxe de courtage.", 5) },
      { CustomFeats.Comptabilite, new CustomFeat("Comptabilité", "Réduit de 11 % par niveau la taxe de vente.", 5) },
      { CustomFeats.Enchanteur, new CustomFeat("Enchanteur", "Réduit de 1 % par niveau le coût et le temps nécessaire pour enchanter un objet.", 20) },
      { CustomFeats.ArtisanExceptionnel, new CustomFeat("Artisan exceptionnel", "Augmente de 1 % par niveau la chance de parvenir à produire un objet avec un emplacement d'enchantement supplémentaire.", 10) },
      { CustomFeats.SurchargeArcanique, new CustomFeat("Surcharge Arcanique", "Permet de forcer l'ajout d'emplacements d'enchantements sur un objet au risque de le briser.\n\nAugmente de 1 % par niveau la chance de parvenir à forcer l'ajout d'un emplacement d'enchantement supplémentaire.", 10) },
      { CustomFeats.SurchargeControlee, new CustomFeat("Surcharge Contrôlée", "Augmente de 5 % par niveau la chance de conserver l'objet intact lors de l'échec d'une tentative de surcharge.", 10) },
      { CustomFeats.EnchanteurExpert, new CustomFeat("Enchanteur Expert", "Augmente de 2 % par niveau la chance d'incanter un enchantement plus puissant.", 10) },
      { CustomFeats.EnchanteurChanceux, new CustomFeat("Enchanteur Chanceux", "Augmente de 1 % par niveau la chance de ne pas consommer d'emplacement lors d'un enchantement.", 10) },
      { CustomFeats.ArtisanApplique, new CustomFeat("Artisan Appliqué", "Augmente de 3 % par niveau la chance d'augmenter la durabilité d'un objet lors de sa fabrication.", 10) },
      { CustomFeats.Renforcement, new CustomFeat("Renforcement", "Permet d'augmenter la durabilité d'un objet de 5 % par renforcement. Cumulable 10 fois.\n\nDiminue le temps de travail nécessaire de 5 % par niveau.", 10) },
      { CustomFeats.CombattantPrecautionneux, new CustomFeat("Combattant précautionneux", "Diminue de 1 % par niveau le risque d'usure des objets.", 10) },
    };

    public static Dictionary<Feat, Func<PlayerSystem.Player, Feat, int>> RegisterAddCustomFeatEffect = new Dictionary<Feat, Func<PlayerSystem.Player, Feat, int>>
    {
            { CustomFeats.ImprovedHealth, HandleHealthPoints },
            { CustomFeats.ImprovedStrength, HandleImproveAbility },
            { CustomFeats.ImprovedDexterity, HandleImproveAbility },
            { CustomFeats.ImprovedConstitution, HandleImproveAbility },
            { CustomFeats.ImprovedIntelligence, HandleImproveAbility },
            { CustomFeats.ImprovedWisdom, HandleImproveAbility },
            { CustomFeats.ImprovedCharisma, HandleImproveAbility },
            { CustomFeats.ImprovedAttackBonus, HandleImproveAttack },
            { CustomFeats.ImprovedSavingThrowAll, HandleImproveSavingThrowAll },
            { CustomFeats.ImprovedSavingThrowFortitude, HandleImproveSavingThrowFortitude },
            { CustomFeats.ImprovedSavingThrowWill, HandleImproveSavingThrowWill },
            { CustomFeats.ImprovedSavingThrowReflex, HandleImproveSavingThrowReflex },
            { CustomFeats.ImprovedAnimalEmpathy, HandleImproveAnimalEmpathy },
            { CustomFeats.ImprovedConcentration, HandleImproveConcentration },
            { CustomFeats.ImprovedDisableTraps, HandleImproveDisableTraps },
            { CustomFeats.ImprovedDiscipline, HandleImproveDiscipline },
            { CustomFeats.ImprovedHeal, HandleImproveHeal },
            { CustomFeats.ImprovedHide, HandleImproveHide },
            { CustomFeats.ImprovedListen, HandleImproveListen },
            { CustomFeats.ImprovedLore, HandleImproveLore },
            { CustomFeats.ImprovedMoveSilently, HandleImproveMoveSilently },
            { CustomFeats.ImprovedOpenLock, HandleImproveOpenLock },
            { CustomFeats.ImprovedSkillParry, HandleImproveSkillParry },
            { CustomFeats.ImprovedPerform, HandleImprovePerform },
            { CustomFeats.ImprovedPickpocket, HandleImprovePickpocket },
            { CustomFeats.ImprovedSearch, HandleImproveSearch },
            { CustomFeats.ImprovedSetTrap, HandleImproveSetTrap },
            { CustomFeats.ImprovedSpellcraft, HandleImproveSpellcraft },
            { CustomFeats.ImprovedSpot, HandleImproveSpot },
            { CustomFeats.ImprovedTaunt, HandleImproveTaunt },
            { CustomFeats.ImprovedUseMagicDevice, HandleImproveUseMagicDevice },
            { CustomFeats.ImprovedTumble, HandleImproveTumble },
            { CustomFeats.ImprovedBluff, HandleImproveBluff },
            { CustomFeats.ImprovedIntimidate, HandleImproveIntimidate },
            { CustomFeats.ImprovedSpellSlot0, HandleImproveSpellSlot0 },
            { CustomFeats.ImprovedSpellSlot1, HandleImproveSpellSlot1 },
            { CustomFeats.ImprovedSpellSlot2, HandleImproveSpellSlot2 },
            { CustomFeats.ImprovedSpellSlot3, HandleImproveSpellSlot3 },
            { CustomFeats.ImprovedSpellSlot4, HandleImproveSpellSlot4 },
            { CustomFeats.ImprovedSpellSlot5, HandleImproveSpellSlot5 },
            { CustomFeats.ImprovedSpellSlot6, HandleImproveSpellSlot6 },
            { CustomFeats.ImprovedSpellSlot7, HandleImproveSpellSlot7 },
            { CustomFeats.ImprovedSpellSlot8, HandleImproveSpellSlot8 },
            { CustomFeats.ImprovedSpellSlot9, HandleImproveSpellSlot9 },
    };

    public static Dictionary<Feat, Func<PlayerSystem.Player, Feat, int>> RegisterRemoveCustomFeatEffect = new Dictionary<Feat, Func<PlayerSystem.Player, Feat, int>>
    {
            //{ 1130, HandleRemoveStrengthMalusFeat },
    };

    private static int HandleHealthPoints(PlayerSystem.Player player, Feat feat)
    {
      int improvedHealth = 0;
      if (player.learntCustomFeats.ContainsKey(CustomFeats.ImprovedHealth))
        improvedHealth = GetCustomFeatLevelFromSkillPoints(CustomFeats.ImprovedHealth, player.learntCustomFeats[CustomFeats.ImprovedHealth]);

      player.oid.MaxHP = Int32.Parse(NWScript.Get2DAString("classes", "HitDie", 43))
        + (1 + 3 * ((player.oid.GetAbilityScore(Ability.Constitution, true) - 10) / 2)
        + CreaturePlugin.GetKnowsFeat(player.oid, (int)Feat.Toughness)) * improvedHealth;
      return 0;
    }
    private static int HandleImproveAbility(PlayerSystem.Player player, Feat feat)
    {
      switch(feat)
      {
        case CustomFeats.ImprovedStrength:
          CreaturePlugin.ModifyRawAbilityScore(player.oid, (int)Ability.Strength, 1);
          break;
        case CustomFeats.ImprovedDexterity:
          CreaturePlugin.ModifyRawAbilityScore(player.oid, (int)Ability.Dexterity, 1);
          break;
        case CustomFeats.ImprovedConstitution:
          CreaturePlugin.ModifyRawAbilityScore(player.oid, (int)Ability.Constitution, 1);
          HandleHealthPoints(player, feat);
          break;
        case CustomFeats.ImprovedIntelligence:
          CreaturePlugin.ModifyRawAbilityScore(player.oid, (int)Ability.Intelligence, 1);
          break;
        case CustomFeats.ImprovedWisdom:
          CreaturePlugin.ModifyRawAbilityScore(player.oid, (int)Ability.Wisdom, 1);
          break;
        case CustomFeats.ImprovedCharisma:
          CreaturePlugin.ModifyRawAbilityScore(player.oid, (int)Ability.Charisma, 1);
          break;
      }

      return 0;
    }
    private static int HandleImproveAttack(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetBaseAttackBonus(player.oid, player.oid.BaseAttackBonus + 1);
      return 0;
    }
    private static int HandleImproveSavingThrowAll(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetBaseSavingThrow(player.oid, NWScript.SAVING_THROW_ALL, player.oid.GetBaseSavingThrow(SavingThrow.All) + 1);
      return 0;
    }
    private static int HandleImproveSavingThrowFortitude(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetBaseSavingThrow(player.oid, NWScript.SAVING_THROW_FORT, player.oid.GetBaseSavingThrow(SavingThrow.Fortitude) + 1);
      return 0;
    }
    private static int HandleImproveSavingThrowWill(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetBaseSavingThrow(player.oid, NWScript.SAVING_THROW_WILL, player.oid.GetBaseSavingThrow(SavingThrow.Will) + 1);
      return 0;
    }
    private static int HandleImproveSavingThrowReflex(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetBaseSavingThrow(player.oid, NWScript.SAVING_THROW_REFLEX, player.oid.GetBaseSavingThrow(SavingThrow.Reflex) + 1);
      return 0;
    }
    private static int HandleImproveAnimalEmpathy(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid, NWScript.SKILL_ANIMAL_EMPATHY, player.oid.GetSkillRank(API.Constants.Skill.AnimalEmpathy, true) + 1);
      return 0;
    }
    private static int HandleImproveConcentration(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid, NWScript.SKILL_CONCENTRATION, player.oid.GetSkillRank(API.Constants.Skill.Concentration, true) + 1);
      return 0;
    }
    private static int HandleImproveDisableTraps(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid, NWScript.SKILL_DISABLE_TRAP, player.oid.GetSkillRank(API.Constants.Skill.DisableTrap, true) + 1);
      return 0;
    }
    private static int HandleImproveDiscipline(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid, NWScript.SKILL_DISCIPLINE, player.oid.GetSkillRank(API.Constants.Skill.Discipline, true) + 1);
      return 0;
    }
    private static int HandleImproveHeal(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid, NWScript.SKILL_HEAL, player.oid.GetSkillRank(API.Constants.Skill.Heal, true) + 1);
      return 0;
    }
    private static int HandleImproveHide(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid, NWScript.SKILL_HIDE, player.oid.GetSkillRank(API.Constants.Skill.Hide, true) + 1);
      return 0;
    }
    private static int HandleImproveListen(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid, NWScript.SKILL_LISTEN, player.oid.GetSkillRank(API.Constants.Skill.Listen, true) + 1);
      return 0;
    }
    private static int HandleImproveLore(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid, NWScript.SKILL_LORE, player.oid.GetSkillRank(API.Constants.Skill.Lore, true) + 1);
      return 0;
    }
    private static int HandleImproveMoveSilently(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid, NWScript.SKILL_MOVE_SILENTLY, player.oid.GetSkillRank(API.Constants.Skill.MoveSilently, true) + 1);
      return 0;
    }
    private static int HandleImproveOpenLock(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid, NWScript.SKILL_OPEN_LOCK, player.oid.GetSkillRank(API.Constants.Skill.OpenLock, true) + 1);
      return 0;
    }
    private static int HandleImproveSkillParry(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid, NWScript.SKILL_PARRY, player.oid.GetSkillRank(API.Constants.Skill.Parry, true) + 1);
      return 0;
    }
    private static int HandleImprovePerform(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid, NWScript.SKILL_PERFORM, player.oid.GetSkillRank(API.Constants.Skill.Perform, true) + 1);
      return 0;
    }
    private static int HandleImprovePickpocket(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid, NWScript.SKILL_PICK_POCKET, player.oid.GetSkillRank(API.Constants.Skill.PickPocket, true) + 1);
      return 0;
    }
    private static int HandleImproveSearch(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid, NWScript.SKILL_SEARCH, player.oid.GetSkillRank(API.Constants.Skill.Search, true) + 1);
      return 0;
    }
    private static int HandleImproveSetTrap(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid, NWScript.SKILL_SET_TRAP, player.oid.GetSkillRank(API.Constants.Skill.SetTrap, true) + 1);
      return 0;
    }
    private static int HandleImproveSpellcraft(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid, NWScript.SKILL_ANIMAL_EMPATHY, player.oid.GetSkillRank(API.Constants.Skill.AnimalEmpathy, true) + 1);
      return 0;
    }
    private static int HandleImproveSpot(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid, NWScript.SKILL_SPOT, player.oid.GetSkillRank(API.Constants.Skill.Spot, true) + 1);
      return 0;
    }
    private static int HandleImproveTaunt(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid, NWScript.SKILL_TAUNT, player.oid.GetSkillRank(API.Constants.Skill.Taunt, true) + 1);
      return 0;
    }
    private static int HandleImproveUseMagicDevice(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid, NWScript.SKILL_USE_MAGIC_DEVICE, player.oid.GetSkillRank(API.Constants.Skill.UseMagicDevice, true) + 1);
      return 0;
    }
    private static int HandleImproveTumble(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid, NWScript.SKILL_TUMBLE, player.oid.GetSkillRank(API.Constants.Skill.Tumble, true) + 1);
      return 0;
    }
    private static int HandleImproveBluff(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid, NWScript.SKILL_BLUFF, player.oid.GetSkillRank(API.Constants.Skill.Bluff, true) + 1);
      return 0;
    }
    private static int HandleImproveIntimidate(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid, NWScript.SKILL_INTIMIDATE, player.oid.GetSkillRank(API.Constants.Skill.Intimidate, true) + 1);
      return 0;
    }

    private static int HandleImproveSpellSlot0(PlayerSystem.Player player, Feat feat)
    {
      NwItem skin = player.oid.GetItemInSlot(InventorySlot.CreatureSkin);
      if (skin != null)
        skin.AddItemProperty(API.ItemProperty.BonusLevelSpell((IPClass)43, IPSpellLevel.SL0), EffectDuration.Permanent);
      else
        Utils.LogMessageToDMs($"Skill System - On Improve Spell Slot : {player.oid.Name} is null !");

      return 0;
    }
    private static int HandleImproveSpellSlot1(PlayerSystem.Player player, Feat feat)
    {
      NwItem skin = player.oid.GetItemInSlot(InventorySlot.CreatureSkin);
      if (skin != null)
        skin.AddItemProperty(API.ItemProperty.BonusLevelSpell((IPClass)43, IPSpellLevel.SL1), EffectDuration.Permanent);
      else
        Utils.LogMessageToDMs($"Skill System - On Improve Spell Slot : {player.oid.Name} is null !");

      return 0;
    }
    private static int HandleImproveSpellSlot2(PlayerSystem.Player player, Feat feat)
    {
      NwItem skin = player.oid.GetItemInSlot(InventorySlot.CreatureSkin);
      if (skin != null)
        skin.AddItemProperty(API.ItemProperty.BonusLevelSpell((IPClass)43, IPSpellLevel.SL2), EffectDuration.Permanent);
      else
        Utils.LogMessageToDMs($"Skill System - On Improve Spell Slot : {player.oid.Name} is null !");

      return 0;
    }
    private static int HandleImproveSpellSlot3(PlayerSystem.Player player, Feat feat)
    {
      NwItem skin = player.oid.GetItemInSlot(InventorySlot.CreatureSkin);
      if (skin != null)
        skin.AddItemProperty(API.ItemProperty.BonusLevelSpell((IPClass)43, IPSpellLevel.SL3), EffectDuration.Permanent);
      else
        Utils.LogMessageToDMs($"Skill System - On Improve Spell Slot : {player.oid.Name} is null !");

      return 0;
    }
    private static int HandleImproveSpellSlot4(PlayerSystem.Player player, Feat feat)
    {
      NwItem skin = player.oid.GetItemInSlot(InventorySlot.CreatureSkin);
      if (skin != null)
        skin.AddItemProperty(API.ItemProperty.BonusLevelSpell((IPClass)43, IPSpellLevel.SL4), EffectDuration.Permanent);
      else
        Utils.LogMessageToDMs($"Skill System - On Improve Spell Slot : {player.oid.Name} is null !");

      return 0;
    }
    private static int HandleImproveSpellSlot5(PlayerSystem.Player player, Feat feat)
    {
      NwItem skin = player.oid.GetItemInSlot(InventorySlot.CreatureSkin);
      if (skin != null)
        skin.AddItemProperty(API.ItemProperty.BonusLevelSpell((IPClass)43, IPSpellLevel.SL5), EffectDuration.Permanent);
      else
        Utils.LogMessageToDMs($"Skill System - On Improve Spell Slot : {player.oid.Name} is null !");

      return 0;
    }
    private static int HandleImproveSpellSlot6(PlayerSystem.Player player, Feat feat)
    {
      NwItem skin = player.oid.GetItemInSlot(InventorySlot.CreatureSkin);
      if (skin != null)
        skin.AddItemProperty(API.ItemProperty.BonusLevelSpell((IPClass)43, IPSpellLevel.SL6), EffectDuration.Permanent);
      else
        Utils.LogMessageToDMs($"Skill System - On Improve Spell Slot : {player.oid.Name} is null !");

      return 0;
    }
    private static int HandleImproveSpellSlot7(PlayerSystem.Player player, Feat feat)
    {
      NwItem skin = player.oid.GetItemInSlot(InventorySlot.CreatureSkin);
      if (skin != null)
        skin.AddItemProperty(API.ItemProperty.BonusLevelSpell((IPClass)43, IPSpellLevel.SL7), EffectDuration.Permanent);
      else
        Utils.LogMessageToDMs($"Skill System - On Improve Spell Slot : {player.oid.Name} is null !");

      return 0;
    }
    private static int HandleImproveSpellSlot8(PlayerSystem.Player player, Feat feat)
    {
      NwItem skin = player.oid.GetItemInSlot(InventorySlot.CreatureSkin);
      if (skin != null)
        skin.AddItemProperty(API.ItemProperty.BonusLevelSpell((IPClass)43, IPSpellLevel.SL8), EffectDuration.Permanent);
      else
        Utils.LogMessageToDMs($"Skill System - On Improve Spell Slot : {player.oid.Name} is null !");

      return 0;
    }
    private static int HandleImproveSpellSlot9(PlayerSystem.Player player, Feat feat)
    {
      NwItem skin = player.oid.GetItemInSlot(InventorySlot.CreatureSkin);
      if (skin != null)
        skin.AddItemProperty(API.ItemProperty.BonusLevelSpell((IPClass)43, IPSpellLevel.SL9), EffectDuration.Permanent);
      else
        Utils.LogMessageToDMs($"Skill System - On Improve Spell Slot : {player.oid.Name} is null !");

      return 0;
    }

    private static int HandleRemoveStrengthMalusFeat(PlayerSystem.Player player, Feat idMalusFeat)
    {
      player.removeableMalus.Remove(idMalusFeat);
      CreaturePlugin.SetRawAbilityScore(player.oid, NWScript.ABILITY_STRENGTH, CreaturePlugin.GetRawAbilityScore(player.oid, NWScript.ABILITY_STRENGTH) + 2);

      return 0;
    }

    public static Feat[] forgeBasicSkillBooks = new Feat[] { CustomFeats.Renforcement, CustomFeats.Recycler, CustomFeats.CraftOreExtractor, CustomFeats.CraftForgeHammer, CustomFeats.Metallurgy, CustomFeats.Research, CustomFeats.Miner, CustomFeats.Prospection, CustomFeats.StripMiner, CustomFeats.Reprocessing, CustomFeats.Forge, CustomFeats.CraftScaleMail, CustomFeats.CraftDagger, CustomFeats.CraftLightMace, CustomFeats.CraftMorningStar, CustomFeats.CraftSickle, CustomFeats.CraftShortSpear };
    public static Feat[] woodBasicSkillBooks = new Feat[] { CustomFeats.Renforcement, CustomFeats.Recycler, CustomFeats.CraftOreExtractor, CustomFeats.CraftForgeHammer, CustomFeats.Metallurgy, CustomFeats.Research, CustomFeats.WoodCutter, CustomFeats.WoodProspection, CustomFeats.StripMiner, CustomFeats.WoodReprocessing, CustomFeats.Ebeniste, CustomFeats.CraftSmallShield, CustomFeats.CraftClub, CustomFeats.CraftDarts, CustomFeats.CraftBullets, CustomFeats.CraftHeavyCrossbow, CustomFeats.CraftLightCrossbow, CustomFeats.CraftQuarterstaff, CustomFeats.CraftSling, CustomFeats.CraftArrow, CustomFeats.CraftBolt };
    public static Feat[] leatherBasicSkillBooks = new Feat[] { CustomFeats.Renforcement, CustomFeats.Recycler, CustomFeats.Hunting, CustomFeats.Skinning, CustomFeats.Tanner, CustomFeats.PeltReprocessing, CustomFeats.CraftLeatherArmor, CustomFeats.CraftStuddedLeather, CustomFeats.CraftPaddedArmor, CustomFeats.CraftClothing, CustomFeats.CraftWhip, CustomFeats.CraftBelt, CustomFeats.CraftBoots, CustomFeats.CraftBracer, CustomFeats.CraftCloak, CustomFeats.CraftGloves };
    //public static Feat[] craftSkillBooks = new Feat[] { CustomFeats.Metallurgy, CustomFeats.AdvancedCraft, CustomFeats.Miner, CustomFeats.Geology, CustomFeats.Prospection, CustomFeats.VeldsparReprocessing, CustomFeats.ScorditeReprocessing, CustomFeats.PyroxeresReprocessing, CustomFeats.StripMiner, CustomFeats.Reprocessing, CustomFeats.ReprocessingEfficiency, CustomFeats.Connections, CustomFeats.Forge };
    public static Feat[] languageSkillBooks = new Feat[] { CustomFeats.Abyssal, CustomFeats.Céleste, CustomFeats.Gnome, CustomFeats.Draconique, CustomFeats.Druidique, CustomFeats.Nain, CustomFeats.Elfique, CustomFeats.Géant, CustomFeats.Gobelin, CustomFeats.Halfelin, CustomFeats.Infernal, CustomFeats.Orc, CustomFeats.Primordiale, CustomFeats.Sylvain, CustomFeats.Voleur, CustomFeats.Gnome };

    public static Feat[] lowSkillBooks = new Feat[] { CustomFeats.Renforcement, CustomFeats.ArtisanApplique, CustomFeats.Enchanteur, CustomFeats.Comptabilite, CustomFeats.BrokerRelations, CustomFeats.Negociateur, CustomFeats.Magnat, CustomFeats.Marchand, CustomFeats.Recycler, Feat.Ambidexterity, CustomFeats.Skinning, CustomFeats.Hunting, CustomFeats.ImprovedSpellSlot2, CustomFeats.WoodReprocessing, CustomFeats.Ebeniste, CustomFeats.WoodCutter, CustomFeats.WoodProspection, CustomFeats.CraftOreExtractor, CustomFeats.CraftForgeHammer, CustomFeats.Forge, CustomFeats.Reprocessing, CustomFeats.BlueprintCopy, CustomFeats.Research, CustomFeats.Miner, CustomFeats.Metallurgy, Feat.DeneirsEye, Feat.DirtyFighting, Feat.ResistDisease, Feat.Stealthy, Feat.SkillFocusAnimalEmpathy, Feat.SkillFocusBluff, Feat.SkillFocusConcentration, Feat.SkillFocusDisableTrap, Feat.SkillFocusDiscipline, Feat.SkillFocusHeal, Feat.SkillFocusHide, Feat.SkillFocusIntimidate, Feat.SkillFocusListen, Feat.SkillFocusLore, Feat.SkillFocusMoveSilently, Feat.SkillFocusOpenLock, Feat.SkillFocusParry, Feat.SkillFocusPerform, Feat.SkillFocusPickPocket, Feat.SkillFocusSearch, Feat.SkillFocusSetTrap, Feat.SkillFocusSpellcraft, Feat.SkillFocusSpot, Feat.SkillFocusTaunt, Feat.SkillFocusTumble, Feat.SkillFocusUseMagicDevice, Feat.PointBlankShot, Feat.IronWill, Feat.Alertness, Feat.CombatCasting, Feat.Dodge, Feat.ExtraTurning, Feat.GreatFortitude };
    public static Feat[] mediumSkillBooks = new Feat[] { CustomFeats.CombattantPrecautionneux, CustomFeats.EnchanteurExpert, CustomFeats.BrokerAffinity, CustomFeats.BadPeltReprocessing, CustomFeats.CommonPeltReprocessing, CustomFeats.NormalPeltReprocessing, CustomFeats.UncommunPeltReprocessing, CustomFeats.RarePeltReprocessing, CustomFeats.MagicPeltReprocessing, CustomFeats.EpicPeltReprocessing, CustomFeats.LegendaryPeltReprocessing, CustomFeats.ImprovedSpellSlot3, CustomFeats.ImprovedSpellSlot4, CustomFeats.LaurelinReprocessing, CustomFeats.MallornReprocessing, CustomFeats.TelperionReprocessing, CustomFeats.OiolaireReprocessing, CustomFeats.NimlothReprocessing, CustomFeats.QlipothReprocessing, CustomFeats.FerocheneReprocessing, CustomFeats.ValinorReprocessing, CustomFeats.WoodReprocessingEfficiency, CustomFeats.AnimalExpertise, CustomFeats.CraftTorch, CustomFeats.CraftStuddedLeather, CustomFeats.CraftSling, CustomFeats.CraftSmallShield, CustomFeats.CraftSickle, CustomFeats.CraftShortSpear, CustomFeats.CraftRing, CustomFeats.CraftPaddedArmor, CustomFeats.CraftPotion, CustomFeats.CraftQuarterstaff, CustomFeats.CraftMorningStar, CustomFeats.CraftMagicWand, CustomFeats.CraftLightMace, CustomFeats.CraftLightHammer, CustomFeats.CraftLightFlail, CustomFeats.CraftLightCrossbow, CustomFeats.CraftLeatherArmor, CustomFeats.CraftBullets, CustomFeats.CraftCloak, CustomFeats.CraftClothing, CustomFeats.CraftClub, CustomFeats.CraftDagger, CustomFeats.CraftDarts, CustomFeats.CraftGloves, CustomFeats.CraftHeavyCrossbow, CustomFeats.CraftHelmet, CustomFeats.CraftAmulet, CustomFeats.CraftArrow, CustomFeats.CraftBelt, CustomFeats.CraftBolt, CustomFeats.CraftBoots, CustomFeats.CraftBracer, CustomFeats.ReprocessingEfficiency, CustomFeats.StripMiner, CustomFeats.VeldsparReprocessing, CustomFeats.ScorditeReprocessing, CustomFeats.PyroxeresReprocessing, CustomFeats.PlagioclaseReprocessing, CustomFeats.Geology, CustomFeats.Prospection, Feat.TymorasSmile, Feat.LliirasHeart, Feat.RapidReload, Feat.Expertise, Feat.ImprovedInitiative, Feat.DefensiveRoll, Feat.SneakAttack, Feat.FlurryOfBlows, Feat.WeaponSpecializationHeavyCrossbow, Feat.WeaponSpecializationDagger, Feat.WeaponSpecializationDart, Feat.WeaponSpecializationClub, Feat.StillSpell, Feat.RapidShot, Feat.SilenceSpell, Feat.PowerAttack, Feat.Knockdown, Feat.LightningReflexes, Feat.ImprovedUnarmedStrike, Feat.Cleave, Feat.CalledShot, Feat.DeflectArrows, Feat.WeaponSpecializationLightCrossbow, Feat.WeaponSpecializationLightFlail, Feat.WeaponSpecializationLightMace, Feat.Disarm, Feat.EmpowerSpell, Feat.WeaponSpecializationMorningStar, Feat.ExtendSpell, Feat.SpellFocusAbjuration, Feat.SpellFocusConjuration, Feat.SpellFocusDivination, Feat.SpellFocusEnchantment, Feat.WeaponSpecializationSickle, Feat.WeaponSpecializationSling, Feat.WeaponSpecializationSpear, Feat.WeaponSpecializationStaff, Feat.WeaponSpecializationThrowingAxe, Feat.WeaponSpecializationTrident, Feat.WeaponSpecializationUnarmedStrike, Feat.SpellFocusEvocation, Feat.SpellFocusIllusion, Feat.SpellFocusNecromancy, Feat.SpellFocusTransmutation, Feat.SpellPenetration };
    public static Feat[] highSkillBooks = new Feat[] { CustomFeats.EnchanteurChanceux, CustomFeats.SurchargeControlee, CustomFeats.SurchargeArcanique, CustomFeats.ArtisanExceptionnel, CustomFeats.AdvancedCraft, CustomFeats.CraftWarHammer, CustomFeats.CraftTrident, CustomFeats.CraftThrowingAxe, CustomFeats.CraftStaff, CustomFeats.CraftSplintMail, CustomFeats.CraftSpellScroll, CustomFeats.CraftShortsword, CustomFeats.CraftShortBow, CustomFeats.CraftScimitar, CustomFeats.CraftScaleMail, CustomFeats.CraftRapier, CustomFeats.CraftMagicRod, CustomFeats.CraftLongsword, CustomFeats.CraftLongBow, CustomFeats.CraftLargeShield, CustomFeats.CraftBattleAxe, CustomFeats.OmberReprocessing, CustomFeats.KerniteReprocessing, CustomFeats.GneissReprocessing, CustomFeats.CraftHalberd, CustomFeats.JaspetReprocessing, CustomFeats.CraftHeavyFlail, CustomFeats.CraftHandAxe, CustomFeats.HemorphiteReprocessing, CustomFeats.CraftGreatAxe, CustomFeats.CraftGreatSword, Feat.ArcaneDefenseAbjuration, Feat.ArcaneDefenseConjuration, Feat.ArcaneDefenseDivination, Feat.ArcaneDefenseEnchantment, Feat.ArcaneDefenseEvocation, Feat.ArcaneDefenseIllusion, Feat.ArcaneDefenseNecromancy, Feat.ArcaneDefenseTransmutation, Feat.BlindFight, Feat.SpringAttack, Feat.GreatCleave, Feat.ImprovedExpertise, Feat.SkillMastery, Feat.Opportunist, Feat.Evasion, Feat.WeaponSpecializationDireMace, Feat.WeaponSpecializationDoubleAxe, Feat.WeaponSpecializationDwaxe, Feat.WeaponSpecializationGreatAxe, Feat.WeaponSpecializationGreatSword, Feat.WeaponSpecializationHalberd, Feat.WeaponSpecializationHandAxe, Feat.WeaponSpecializationHeavyFlail, Feat.WeaponSpecializationKama, Feat.WeaponSpecializationKatana, Feat.WeaponSpecializationKukri, Feat.WeaponSpecializationBastardSword, Feat.WeaponSpecializationLightHammer, Feat.WeaponSpecializationLongbow, Feat.WeaponSpecializationLongSword, Feat.WeaponSpecializationRapier, Feat.WeaponSpecializationScimitar, Feat.WeaponSpecializationScythe, Feat.WeaponSpecializationShortbow, Feat.WeaponSpecializationShortSword, Feat.WeaponSpecializationShuriken, Feat.WeaponSpecializationBattleAxe, Feat.QuickenSpell, Feat.MaximizeSpell, Feat.ImprovedTwoWeaponFighting, Feat.ImprovedPowerAttack, Feat.WeaponSpecializationTwoBladedSword, Feat.WeaponSpecializationWarHammer, Feat.WeaponSpecializationWhip, Feat.ImprovedDisarm, Feat.ImprovedKnockdown, Feat.ImprovedParry, Feat.ImprovedCriticalBastardSword, Feat.ImprovedCriticalBattleAxe, Feat.ImprovedCriticalClub, Feat.ImprovedCriticalDagger, Feat.ImprovedCriticalDart, Feat.ImprovedCriticalDireMace, Feat.ImprovedCriticalDoubleAxe, Feat.ImprovedCriticalDwaxe, Feat.ImprovedCriticalGreatAxe, Feat.ImprovedCriticalGreatSword, Feat.ImprovedCriticalHalberd, Feat.ImprovedCriticalHandAxe, Feat.ImprovedCriticalHeavyCrossbow, Feat.ImprovedCriticalHeavyFlail, Feat.ImprovedCriticalKama, Feat.ImprovedCriticalKatana, Feat.ImprovedCriticalKukri, Feat.ImprovedCriticalLightCrossbow, Feat.ImprovedCriticalLightFlail, Feat.ImprovedCriticalLightHammer, Feat.ImprovedCriticalLightMace, Feat.ImprovedCriticalLongbow, Feat.ImprovedCriticalLongSword, Feat.ImprovedCriticalMorningStar, Feat.ImprovedCriticalRapier, Feat.ImprovedCriticalScimitar, Feat.ImprovedCriticalScythe, Feat.ImprovedCriticalShortbow, Feat.ImprovedCriticalShortSword, Feat.ImprovedCriticalShuriken, Feat.ImprovedCriticalSickle, Feat.ImprovedCriticalSling, Feat.ImprovedCriticalSpear, Feat.ImprovedCriticalStaff, Feat.ImprovedCriticalThrowingAxe, Feat.ImprovedCriticalTrident, Feat.ImprovedCriticalTwoBladedSword, Feat.ImprovedCriticalUnarmedStrike, Feat.ImprovedCriticalWarHammer, Feat.ImprovedCriticalWhip };
    public static Feat[] epicSkillBooks = new Feat[] { CustomFeats.CraftWhip, CustomFeats.CraftTwoBladedSword, CustomFeats.CraftTowerShield, CustomFeats.CraftShuriken, CustomFeats.CraftScythe, CustomFeats.CraftKukri, CustomFeats.CraftKatana, CustomFeats.CraftBreastPlate, CustomFeats.CraftDireMace, CustomFeats.CraftDoubleAxe, CustomFeats.CraftDwarvenWarAxe, CustomFeats.CraftFullPlate, CustomFeats.CraftHalfPlate, CustomFeats.CraftBastardSword, CustomFeats.CraftKama, CustomFeats.DarkOchreReprocessing, CustomFeats.CrokiteReprocessing, CustomFeats.BistotReprocessing, Feat.ResistEnergyAcid, Feat.ResistEnergyCold, Feat.ResistEnergyElectrical, Feat.ResistEnergyFire, Feat.ResistEnergySonic, Feat.ZenArchery, Feat.CripplingStrike, Feat.SlipperyMind, Feat.GreaterSpellFocusAbjuration, Feat.GreaterSpellFocusConjuration, Feat.GreaterSpellFocusDivination, Feat.GreaterSpellFocusDiviniation, Feat.GreaterSpellFocusEnchantment, Feat.GreaterSpellFocusEvocation, Feat.GreaterSpellFocusIllusion, Feat.GreaterSpellFocusNecromancy, Feat.GreaterSpellFocusTransmutation, Feat.GreaterSpellPenetration };

    public static int[] shopBasicMagicScrolls = new int[] { NWScript.IP_CONST_CASTSPELL_ACID_SPLASH_1, NWScript.IP_CONST_CASTSPELL_DAZE_1, NWScript.IP_CONST_CASTSPELL_ELECTRIC_JOLT_1, NWScript.IP_CONST_CASTSPELL_FLARE_1, NWScript.IP_CONST_CASTSPELL_RAY_OF_FROST_1, NWScript.IP_CONST_CASTSPELL_RESISTANCE_5, NWScript.IP_CONST_CASTSPELL_BURNING_HANDS_5, NWScript.IP_CONST_CASTSPELL_CHARM_PERSON_2, NWScript.IP_CONST_CASTSPELL_COLOR_SPRAY_2, NWScript.IP_CONST_CASTSPELL_ENDURE_ELEMENTS_2, NWScript.IP_CONST_CASTSPELL_EXPEDITIOUS_RETREAT_5, NWScript.IP_CONST_CASTSPELL_GREASE_2, 459, 478, 460, NWScript.IP_CONST_CASTSPELL_MAGE_ARMOR_2, NWScript.IP_CONST_CASTSPELL_MAGIC_MISSILE_5, NWScript.IP_CONST_CASTSPELL_NEGATIVE_ENERGY_RAY_5, NWScript.IP_CONST_CASTSPELL_RAY_OF_ENFEEBLEMENT_2, NWScript.IP_CONST_CASTSPELL_SCARE_2, 469, NWScript.IP_CONST_CASTSPELL_SHIELD_5, NWScript.IP_CONST_CASTSPELL_SLEEP_5, NWScript.IP_CONST_CASTSPELL_SUMMON_CREATURE_I_5, NWScript.IP_CONST_CASTSPELL_AMPLIFY_5, NWScript.IP_CONST_CASTSPELL_BALAGARNSIRONHORN_7, NWScript.IP_CONST_CASTSPELL_LESSER_DISPEL_5, NWScript.IP_CONST_CASTSPELL_CURE_MINOR_WOUNDS_1, NWScript.IP_CONST_CASTSPELL_INFLICT_MINOR_WOUNDS_1, NWScript.IP_CONST_CASTSPELL_VIRTUE_1, NWScript.IP_CONST_CASTSPELL_BANE_5, NWScript.IP_CONST_CASTSPELL_BLESS_2, NWScript.IP_CONST_CASTSPELL_CURE_LIGHT_WOUNDS_5, NWScript.IP_CONST_CASTSPELL_DIVINE_FAVOR_5, NWScript.IP_CONST_CASTSPELL_DOOM_5, NWScript.IP_CONST_CASTSPELL_ENTROPIC_SHIELD_5, NWScript.IP_CONST_CASTSPELL_INFLICT_LIGHT_WOUNDS_5, NWScript.IP_CONST_CASTSPELL_REMOVE_FEAR_2, NWScript.IP_CONST_CASTSPELL_SANCTUARY_2, NWScript.IP_CONST_CASTSPELL_SHIELD_OF_FAITH_5, NWScript.IP_CONST_CASTSPELL_CAMOFLAGE_5, NWScript.IP_CONST_CASTSPELL_ENTANGLE_5, NWScript.IP_CONST_CASTSPELL_MAGIC_FANG_5, 540, 541, 542, 543, 544 };
    public static Feat[] shopBasicMagicSkillBooks = new Feat[] { CustomFeats.Enchanteur, CustomFeats.Comptabilite, CustomFeats.BrokerRelations, CustomFeats.Negociateur, CustomFeats.ContractScience, CustomFeats.Marchand, CustomFeats.Magnat };
    public static int GetCustomFeatLevelFromSkillPoints(Feat feat, int currentSkillPoints)
    {
      int multiplier = 1;
      int.TryParse(NWScript.Get2DAString("feat", "CRValue", (int)feat), out multiplier);

      return (int)(Math.Log(currentSkillPoints / 250 * multiplier) / Math.Log(Math.Sqrt(32)));
    }
  }
}
