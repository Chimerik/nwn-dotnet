
using System;
using System.Collections.Generic;

using Anvil.API;

using NWN.Core;
using NWN.Core.NWNX;
using NWN.Systems;

namespace Utils
{
  public class CustomFeatConfig
  {
    public static Dictionary<Feat, CustomFeat> customFeatsDictionnary = new Dictionary<Feat, CustomFeat>()
    {
      { CustomFeatList.BlueprintCopy, new CustomFeat("Copie patron", "Permet la copie de patrons pour l'artisanat.\n\n Diminue le temps de copie de 5 % par niveau.\n\n Le travail artisanal ne peut progresser que dans les zones sécurisées de Similisse.", 5) },
      { CustomFeatList.Research, new CustomFeat("Recherche patron", "Permet de rechercher une amélioration pour un patron.\n\n Diminue le temps de recherche de 5 % par niveau.\n\n Le travail artisanal ne peut progresser que dans les zones sécurisées de Similisse.", 5) },
      { CustomFeatList.ImprovedStrength, new CustomFeat("Force accrue", "Augmente la force d'un point par niveau d'entraînement.", 6) },
      { CustomFeatList.ImprovedDexterity, new CustomFeat("Dextérité accrue", "Augmente la dextérité d'un point par niveau d'entraînement.", 6) },
      { CustomFeatList.ImprovedConstitution, new CustomFeat("Constitution accrue", "Augmente la constitution d'un point par niveau d'entraînement.", 6) },
      { CustomFeatList.ImprovedIntelligence, new CustomFeat("Intelligence accrue", "Augmente l'intelligence d'un point par niveau d'entraînement.", 6) },
      { CustomFeatList.ImprovedWisdom, new CustomFeat("Sagesse accrue", "Augmente la sagesse d'un point par niveau d'entraînement.", 6) },
      { CustomFeatList.ImprovedCharisma, new CustomFeat("Charisme accru", "Augmente le charisme d'un point par niveau d'entraînement.", 6) },
      { CustomFeatList.ImprovedAnimalEmpathy, new CustomFeat("Empathie Animale accrue", "Augmente la compétence Empathie Animale d'un point par niveau d'entraînement.", 12) },
      { CustomFeatList.ImprovedConcentration, new CustomFeat("Concentration accrue", "Augmente la compétence Concentration d'un point par niveau d'entraînement.", 12) },
      { CustomFeatList.ImprovedDisableTraps, new CustomFeat("Désamorçage accrue", "Augmente la compétence Désamorçage d'un point par niveau d'entraînement.", 12) },
      { CustomFeatList.ImprovedDiscipline, new CustomFeat("Discipline accrue", "Augmente la compétence Discipline d'un point par niveau d'entraînement.", 12) },
      { CustomFeatList.ImprovedHeal, new CustomFeat("Premiers Soins accrue", "Augmente la compétence Premiers Soins d'un point par niveau d'entraînement.", 12) },
      { CustomFeatList.ImprovedHide, new CustomFeat("Discrétion accrue", "Augmente la compétence Discrétion d'un point par niveau d'entraînement.", 12) },
      { CustomFeatList.ImprovedListen, new CustomFeat("Perception accrue", "Augmente la compétence Perception d'un point par niveau d'entraînement.", 12) },
      { CustomFeatList.ImprovedLore, new CustomFeat("Savoir accrue", "Augmente la compétence Savoir d'un point par niveau d'entraînement.", 12) },
      { CustomFeatList.ImprovedMoveSilently, new CustomFeat("Déplacement Silencieux accrue", "Augmente la compétence Déplacement Silencieux d'un point par niveau d'entraînement.", 12) },
      { CustomFeatList.ImprovedOpenLock, new CustomFeat("Crochetage accrue", "Augmente la compétence Crochetage d'un point par niveau d'entraînement.", 12) },
      { CustomFeatList.ImprovedSkillParry, new CustomFeat("Parade accrue", "Augmente la compétence Parade d'un point par niveau d'entraînement.", 12) },
      { CustomFeatList.ImprovedPerform, new CustomFeat("Représentation accrue", "Augmente la compétence Représentation d'un point par niveau d'entraînement.", 12) },
      { CustomFeatList.ImprovedPickpocket, new CustomFeat("Vol à la tir accrue", "Augmente la compétence Vol à la tir d'un point par niveau d'entraînement.", 12) },
      { CustomFeatList.ImprovedSearch, new CustomFeat("Fouille accrue", "Augmente la compétence Fouille d'un point par niveau d'entraînement.", 12) },
      { CustomFeatList.ImprovedSetTrap, new CustomFeat("Pose de Piège accrue", "Augmente la compétence Pose de Piège d'un point par niveau d'entraînement.", 12) },
      { CustomFeatList.ImprovedSpellcraft, new CustomFeat("Connaissance des Sorts accrue", "Augmente la compétence Connaissance des Sorts d'un point par niveau d'entraînement.", 12) },
      { CustomFeatList.ImprovedSpot, new CustomFeat("Détection accrue", "Augmente la compétence Détection d'un point par niveau d'entraînement.", 12) },
      { CustomFeatList.ImprovedTaunt, new CustomFeat("Raillerie accrue", "Augmente la compétence Raillerie d'un point par niveau d'entraînement.", 12) },
      { CustomFeatList.ImprovedUseMagicDevice, new CustomFeat("Utilisation d'Objets Magiques accrue", "Augmente la compétence Utilisation d'Objets Magiques d'un point par niveau d'entraînement.", 12) },
      { CustomFeatList.ImprovedTumble, new CustomFeat("Acrobatie accrue", "Augmente la compétence Acrobatie d'un point par niveau d'entraînement.", 12) },
      { CustomFeatList.ImprovedBluff, new CustomFeat("Bluff accrue", "Augmente la compétence Bluff d'un point par niveau d'entraînement.", 12) },
      { CustomFeatList.ImprovedIntimidate, new CustomFeat("Intimidation accrue", "Augmente la compétence Intimidation d'un point par niveau d'entraînement.", 12) },
      { CustomFeatList.ImprovedHealth, new CustomFeat("Résilience accrue", "Augmente les points de vie de de 1 + (Robustesse + 3 * modificateur de constitution de base) par niveau.\n\n Ce don est rétroactif.", 6) },
      { CustomFeatList.ImprovedAttackBonus, new CustomFeat("Attaque accrue", "Améliore le bonus d'attaque de base d'un point par niveau.", 12) },
      { CustomFeatList.ImprovedSpellSlot0, new CustomFeat("Emplacement Cercle 0", "Augmente le nombre d'emplacements de sorts de cercle 0 disponibles d'un par niveau.", 10) },
      { CustomFeatList.ImprovedSpellSlot1, new CustomFeat("Emplacement Cercle 1", "Augmente le nombre d'emplacements de sorts de cercle 1 disponibles d'un par niveau.", 10) },
      { CustomFeatList.ImprovedSpellSlot2, new CustomFeat("Emplacement Cercle 2", "Augmente le nombre d'emplacements de sorts de cercle 2 disponibles d'un par niveau.", 10) },
      { CustomFeatList.ImprovedSpellSlot3, new CustomFeat("Emplacement Cercle 3", "Augmente le nombre d'emplacements de sorts de cercle 3 disponibles d'un par niveau.", 10) },
      { CustomFeatList.ImprovedSpellSlot4, new CustomFeat("Emplacement Cercle 4", "Augmente le nombre d'emplacements de sorts de cercle 4 disponibles d'un par niveau.", 10) },
      { CustomFeatList.ImprovedSpellSlot5, new CustomFeat("Emplacement Cercle 5", "Augmente le nombre d'emplacements de sorts de cercle 5 disponibles d'un par niveau.", 10) },
      { CustomFeatList.ImprovedSpellSlot6, new CustomFeat("Emplacement Cercle 6", "Augmente le nombre d'emplacements de sorts de cercle 6 disponibles d'un par niveau.", 10) },
      { CustomFeatList.ImprovedSpellSlot7, new CustomFeat("Emplacement Cercle 7", "Augmente le nombre d'emplacements de sorts de cercle 7 disponibles d'un par niveau.", 10) },
      { CustomFeatList.ImprovedSpellSlot8, new CustomFeat("Emplacement Cercle 8", "Augmente le nombre d'emplacements de sorts de cercle 8 disponibles d'un par niveau.", 10) },
      { CustomFeatList.ImprovedSpellSlot9, new CustomFeat("Emplacement Cercle 9", "Augmente le nombre d'emplacements de sorts de cercle 9 disponibles d'un par niveau.", 10) },
      { CustomFeatList.ImprovedCasterLevel, new CustomFeat("Caster Level", "Augmente le niveau de lanceur de sorts de un par niveau.", 12) },
      { CustomFeatList.ImprovedSavingThrowAll, new CustomFeat("JdS Universel ", "Augmente le jet de sauvegarde universel d'un point par niveau.", 6) },
      { CustomFeatList.ImprovedSavingThrowFortitude, new CustomFeat("JdS Vigueur", "Augmente le jet de sauvegarde de vigueur d'un point par niveau.", 6) },
      { CustomFeatList.ImprovedSavingThrowReflex, new CustomFeat("JdS Réflexes", "Augmente le jet de sauvegarde de réflexes d'un point par niveau.", 6) },
      { CustomFeatList.ImprovedSavingThrowWill, new CustomFeat("JdS Volonté", "Augmente le jet de sauvegarde de volonté d'un point par niveau.", 6) },
      { CustomFeatList.Metallurgy, new CustomFeat("Métallurgie", "Diminue le temps de recherche d'un patron en efficacité matérielle de 5 % par niveau.\n\n Le travail artisanal ne peut progresser que dans les zones sécurisées de Similisse.", 5) },
      { CustomFeatList.AdvancedCraft, new CustomFeat("Artisanat Avancé", "Diminue le temps de recherche d'un patron en efficacité de production matérielle et temporelle de 3 % par niveau.", 5) },
      { CustomFeatList.Miner, new CustomFeat("Mineur", "Augmente la quantité de minerai extrait par cycle de 5 % par niveau.", 10) },
      { CustomFeatList.Geology, new CustomFeat("Géologie", "Augmente la quantité de minerai extrait par cycle de 5 % par niveau.\n\nAugmente les chances de trouver un filon lors de la prospection de 5 % par niveau.", 10) },
      { CustomFeatList.Prospection, new CustomFeat("Prospection", "Augmente les chances de trouver un filon de minerai brut lors de la prospection de 5 % par niveau.", 5) },
      { CustomFeatList.VeldsparReprocessing, new CustomFeat("Raffinage Veldspar", "Réduit la quantité de Veldspar gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeatList.ScorditeReprocessing, new CustomFeat("Raffinage Scordite", "Réduit la quantité de Scordite gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeatList.PyroxeresReprocessing, new CustomFeat("Raffinage Pyroxeres", "Réduit la quantité de Pyroxeres gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeatList.PlagioclaseReprocessing, new CustomFeat("Raffinage Plagioclase", "Réduit la quantité de Plagioclase gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeatList.OmberReprocessing, new CustomFeat("Raffinage Omber", "Réduit la quantité d'Omber gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeatList.KerniteReprocessing, new CustomFeat("Raffinage Kernite", "Réduit la quantité de Kernite gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeatList.GneissReprocessing, new CustomFeat("Raffinage Gneiss", "Réduit la quantité de Gneiss gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeatList.JaspetReprocessing, new CustomFeat("Raffinage Jaspet", "Réduit la quantité de Jaspet gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeatList.HemorphiteReprocessing, new CustomFeat("Raffinage Hémorphite", "Réduit la quantité d'Hémorphite gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeatList.HedbergiteReprocessing, new CustomFeat("Raffinage Hedbergite", "Réduit la quantité d'Hedbergite gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeatList.DarkOchreReprocessing, new CustomFeat("Raffinage Darkochre", "Réduit la quantité de Darkochre gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeatList.CrokiteReprocessing, new CustomFeat("Raffinage Crokite", "Réduit la quantité de Crokite gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeatList.BistotReprocessing, new CustomFeat("Raffinage Bistot", "Réduit la quantité de Bistot gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeatList.BezdnacineReprocessing, new CustomFeat("Raffinage Bezdnacine", "Réduit la quantité de Bezdnacine gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeatList.ArkonorReprocessing, new CustomFeat("Raffinage Arkonor", "Réduit la quantité d'Arkonor gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeatList.MercoxitReprocessing, new CustomFeat("Raffinage Mercoxit", "Réduit la quantité de Mercoxit gachée lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeatList.StripMiner, new CustomFeat("Minage par Fracturation", "Augmente la quantité de minerai extrait par cycle de 5 % par niveau.", 10) },
      { CustomFeatList.Reprocessing, new CustomFeat("Raffinage", "Réduit la quantité de minerai gaché lors du raffinage de 3 % par niveau.", 5) },
      { CustomFeatList.ReprocessingEfficiency, new CustomFeat("Raffinage efficace", "Réduit la quantité de minerai gaché lors du raffinage de 2 % par niveau.", 5) },
      { CustomFeatList.Connections, new CustomFeat("Relations", "Diminue la taxe de raffinage exigée par l'Amirauté de 5 % par niveau.", 5) },
      { CustomFeatList.Forge, new CustomFeat("Forge", "Diminue le temps de fabrication et le coût en matériaux d'un objet de la forge de 1 % par niveau.", 10) },
      { CustomFeatList.CraftClothing, new CustomFeat("Craft Vêtements", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftFullPlate, new CustomFeat("Craft Harnois", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftHalfPlate, new CustomFeat("Craft Armure de Plaques", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftSplintMail, new CustomFeat("Craft Clibanion", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftBreastPlate, new CustomFeat("Craft Cuirasse", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftScaleMail, new CustomFeat("Craft Chemise de mailles", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftStuddedLeather, new CustomFeat("Craft Cuir clouté", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftLeatherArmor, new CustomFeat("Craft Armure de cuir", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftPaddedArmor, new CustomFeat("Craft Armure matelassée", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftShortsword, new CustomFeat("Craft Epée courte", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftLongsword, new CustomFeat("Craft Epée longue", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftBattleAxe, new CustomFeat("Craft Hache d'armes", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftBastardSword, new CustomFeat("Craft Epée bâtarde", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftLightFlail, new CustomFeat("Craft Fléau léger", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftWarHammer, new CustomFeat("Craft Marteau de guerre", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftHeavyCrossbow, new CustomFeat("Craft Arbalète lourde", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftLightCrossbow, new CustomFeat("Craft Arbalète légère", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftLongBow, new CustomFeat("Craft Arc long", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftLightMace, new CustomFeat("Craft Masse légère", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftHalberd, new CustomFeat("Craft Hallebarde", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftShortBow, new CustomFeat("Craft Arc court", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftTwoBladedSword, new CustomFeat("Craft Double lame", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftGreatSword, new CustomFeat("Craft Epée à deux mains", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftSmallShield, new CustomFeat("Craft Rondache", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftTorch, new CustomFeat("Craft Torche", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftHelmet, new CustomFeat("Craft Heaume", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftGreatAxe, new CustomFeat("Craft Grande Hache", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftAmulet, new CustomFeat("Craft Amulette", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftArrow, new CustomFeat("Craft Flèche", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftBelt, new CustomFeat("Craft Ceinture", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftDagger, new CustomFeat("Craft Dague", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftBolt, new CustomFeat("Craft Carreau", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftBoots, new CustomFeat("Craft Bottes", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftBullets, new CustomFeat("Craft Billes", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftClub, new CustomFeat("Craft Gourdin", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftDarts, new CustomFeat("Craft Dards", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftDireMace, new CustomFeat("Craft Masse double", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftHeavyFlail, new CustomFeat("Craft Fléau lourd", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftGloves, new CustomFeat("Craft Gants", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftLightHammer, new CustomFeat("Craft Marteau léger", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftHandAxe, new CustomFeat("Craft Hachette", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftKama, new CustomFeat("Craft Kama", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftKukri, new CustomFeat("Craft Kukri", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftMagicRod, new CustomFeat("Craft Baguette", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftStaff, new CustomFeat("Craft Bourdon", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftMagicWand, new CustomFeat("Craft Baguette", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftMorningStar, new CustomFeat("Craft Morgenstern", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftPotion, new CustomFeat("Craft Potion", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftQuarterstaff, new CustomFeat("Craft Bâton", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftRapier, new CustomFeat("Craft Rapière", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftRing, new CustomFeat("Craft Anneau", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftScimitar, new CustomFeat("Craft Cimeterre", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftScythe, new CustomFeat("Craft Faux", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftLargeShield, new CustomFeat("Craft Ecu", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftTowerShield, new CustomFeat("Craft Pavois", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftShortSpear, new CustomFeat("Craft Lance", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftShuriken, new CustomFeat("Craft Shuriken", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftSickle, new CustomFeat("Craft Serpe", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftSling, new CustomFeat("Craft Fronde", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftThrowingAxe, new CustomFeat("Craft Hache de jet", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftSpellScroll, new CustomFeat("Craft Parchemin", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftBracer, new CustomFeat("Craft Brassard", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftCloak, new CustomFeat("Craft Cape", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftTrident, new CustomFeat("Craft Trident", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftDwarvenWarAxe, new CustomFeat("Craft Hache naine", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftWhip, new CustomFeat("Craft Fouet", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftDoubleAxe, new CustomFeat("Craft Double Hache", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftForgeHammer, new CustomFeat("Craft Marteau d'artisan", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftKatana, new CustomFeat("Craft Katana", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.CraftOreExtractor, new CustomFeat("Craft Extracteur de ressources", "Diminue le temps de fabrication et le coût en matériaux de l'objet concerné de 1 % par niveau.", 10) },
      { CustomFeatList.WoodCutter, new CustomFeat("Bûcheron", "Augmente la quantité de bois brut extrait par cycle de 5 % par niveau.", 10) },
      { CustomFeatList.WoodExpertise, new CustomFeat("Dendrologie", "Augmente la quantité de bois brut extrait par cycle de 5 % par niveau.\n\nAugmente les chances d'identifier un arbre exploitable lors de la prospection de 5 % par niveau.", 5) },
      { CustomFeatList.WoodProspection, new CustomFeat("Prospection Arboricole", "Augmente les chances d'identifier un arbre exploitable lors de la prospection de 5 % par niveau.", 5) },
      { CustomFeatList.Skinning, new CustomFeat("Equarisseur", "Augmente la quantité de viande et de cuir brut extraits par cycle de 5 % par niveau.", 10) },
      { CustomFeatList.AnimalExpertise, new CustomFeat("Zoologie", "Augmente la quantité de viande et de cuir brut extraits par cycle de 5 % par niveau.\n\nAugmente les chances de répérer une proie exploitable lors de la prospection de 5 % par niveau.", 5) },
      { CustomFeatList.Hunting, new CustomFeat("Traque animale", "Augmente les chances de répérer une proie exploitable lors de la prospection de 5 % par niveau.", 5) },
      { CustomFeatList.Ebeniste, new CustomFeat("Ebeniste", "Réduit le coût en matériau raffiné et la durée du travail de 1 % par niveau.", 10) },
      { CustomFeatList.WoodReprocessing, new CustomFeat("Sciage", "Réduit la quantité de bois gaché lors du sciage de 3 % par niveau.", 5) },
      { CustomFeatList.WoodReprocessingEfficiency, new CustomFeat("Sciage efficace", "Réduit la quantité de bois gaché lors du sciage de 2 % par niveau.", 5) },
      { CustomFeatList.LaurelinReprocessing, new CustomFeat("Raffinage Laurelin", "Réduit la quantité de bois gaché lors du sciage de Laurelin de 2 % par niveau.", 5) },
      { CustomFeatList.TelperionReprocessing, new CustomFeat("Raffinage Telperion", "Réduit la quantité de bois gaché lors du sciage de Telperion de 2 % par niveau.", 5) },
      { CustomFeatList.MallornReprocessing, new CustomFeat("Raffinage Mallorn", "Réduit la quantité de bois gaché lors du sciage de Mallorn de 2 % par niveau.", 5) },
      { CustomFeatList.NimlothReprocessing, new CustomFeat("Raffinage Nimloth", "Réduit la quantité de bois gaché lors du sciage de Nimloth de 2 % par niveau.", 5) },
      { CustomFeatList.OiolaireReprocessing, new CustomFeat("Raffinage Oiolaire", "Réduit la quantité de bois gaché lors du sciage de Oiolaire de 2 % par niveau.", 5) },
      { CustomFeatList.QlipothReprocessing, new CustomFeat("Raffinage Qlipoth", "Réduit la quantité de bois gaché lors du sciage de Qlipoth de 2 % par niveau.", 5) },
      { CustomFeatList.FerocheneReprocessing, new CustomFeat("Raffinage Férochêne", "Réduit la quantité de bois gaché lors du sciage de Férochêne de 2 % par niveau.", 5) },
      { CustomFeatList.ValinorReprocessing, new CustomFeat("Raffinage Valinor", "Réduit la quantité de bois gaché lors du sciage de Valinor de 2 % par niveau.", 5) },
      { CustomFeatList.Tanner, new CustomFeat("Maroquinier", "Réduit le coût en matériau raffiné et la durée du travail de 1 % par niveau.", 5) },
      { CustomFeatList.PeltReprocessing, new CustomFeat("Tanneur", "Réduit la quantité de peaux gachées lors du tannage de 3 % par niveau.", 5) },
      { CustomFeatList.PeltReprocessingEfficiency, new CustomFeat("Tannage efficace", "Réduit la quantité de peaux gachées lors du tannage de 2 % par niveau.", 5) },
      { CustomFeatList.BadPeltReprocessing, new CustomFeat("Tannage mauvaises peaux", "Réduit la quantité de peaux gachées lors du tannage des mauvaises peaux de 2 % par niveau.", 5) },
      { CustomFeatList.CommonPeltReprocessing, new CustomFeat("Tannage peaux communes", "Réduit la quantité de peaux gachées lors du tannage des mauvaises peaux de 2 % par niveau.", 5) },
      { CustomFeatList.NormalPeltReprocessing, new CustomFeat("Tannage peaux normales", "Réduit la quantité de peaux gachées lors du tannage des mauvaises peaux de 2 % par niveau.", 5) },
      { CustomFeatList.UncommunPeltReprocessing, new CustomFeat("Tannage peaux inhabituelles", "Réduit la quantité de peaux gachées lors du tannage des mauvaises peaux de 2 % par niveau.", 5) },
      { CustomFeatList.RarePeltReprocessing, new CustomFeat("Tannage peaux rares", "Réduit la quantité de peaux gachées lors du tannage des mauvaises peaux de 2 % par niveau.", 5) },
      { CustomFeatList.MagicPeltReprocessing, new CustomFeat("Tannage peaux magiques", "Réduit la quantité de peaux gachées lors du tannage des mauvaises peaux de 2 % par niveau.", 5) },
      { CustomFeatList.EpicPeltReprocessing, new CustomFeat("Tannage peaux épiques", "Réduit la quantité de peaux gachées lors du tannage des mauvaises peaux de 2 % par niveau.", 5) },
      { CustomFeatList.LegendaryPeltReprocessing, new CustomFeat("Tannage peaux légendaires", "Réduit la quantité de peaux gachées lors du tannage des mauvaises peaux de 2 % par niveau.", 5) },
      { CustomFeatList.Recycler, new CustomFeat("Recyclage", "Permet de recycler des objets en matière raffinée.\n\n Diminue le temps nécessaire au recyclage et augmente le rendement de 1 % par niveau.\n\n Le travail artisanal ne peut progresser que dans les zones sécurisées de Similisse.", 20) },
      { CustomFeatList.ContractScience, new CustomFeat("Science du contrat", "Permet de créer un contrat supplémentaire par niveau (enchères comprises).", 5) },
      { CustomFeatList.Marchand, new CustomFeat("Marchand", "Permet de vendre cinq objets supplémentaires par échoppe.", 20) },
      { CustomFeatList.Magnat, new CustomFeat("Magnat", "Permet d'ouvrir une échoppe supplémentaire par niveau.", 5) },
      { CustomFeatList.Negociateur, new CustomFeat("Négociateur", "Permet d'enregistrer 3 ordres supplémentaires à l'Hôtel des ventes.", 10) },
      { CustomFeatList.BrokerRelations, new CustomFeat("Relations Courtières", "Réduit de 6 % par niveau la taxe de courtage.", 5) },
      { CustomFeatList.BrokerAffinity, new CustomFeat("Affinités Courtières", "Réduit de 6 % par niveau la taxe de courtage.", 5) },
      { CustomFeatList.Comptabilite, new CustomFeat("Comptabilité", "Réduit de 11 % par niveau la taxe de vente.", 5) },
      { CustomFeatList.Enchanteur, new CustomFeat("Enchanteur", "Réduit de 1 % par niveau le coût et le temps nécessaire pour enchanter un objet.", 20) },
      { CustomFeatList.ArtisanExceptionnel, new CustomFeat("Artisan exceptionnel", "Augmente de 1 % par niveau la chance de parvenir à produire un objet avec un emplacement d'enchantement supplémentaire.", 10) },
      { CustomFeatList.SurchargeArcanique, new CustomFeat("Surcharge Arcanique", "Permet de forcer l'ajout d'emplacements d'enchantements sur un objet au risque de le briser.\n\nAugmente de 1 % par niveau la chance de parvenir à forcer l'ajout d'un emplacement d'enchantement supplémentaire.", 10) },
      { CustomFeatList.SurchargeControlee, new CustomFeat("Surcharge Contrôlée", "Augmente de 5 % par niveau la chance de conserver l'objet intact lors de l'échec d'une tentative de surcharge.", 10) },
      { CustomFeatList.EnchanteurExpert, new CustomFeat("Enchanteur Expert", "Augmente de 2 % par niveau la chance d'incanter un enchantement plus puissant.", 10) },
      { CustomFeatList.EnchanteurChanceux, new CustomFeat("Enchanteur Chanceux", "Augmente de 1 % par niveau la chance de ne pas consommer d'emplacement lors d'un enchantement.", 10) },
      { CustomFeatList.ArtisanApplique, new CustomFeat("Artisan Appliqué", "Augmente de 3 % par niveau la chance d'augmenter la durabilité d'un objet lors de sa fabrication.", 10) },
      { CustomFeatList.Renforcement, new CustomFeat("Renforcement", "Permet d'augmenter la durabilité d'un objet de 5 % par renforcement. Cumulable 10 fois.\n\nDiminue le temps de travail nécessaire de 5 % par niveau.", 10) },
      { CustomFeatList.CombattantPrecautionneux, new CustomFeat("Combattant précautionneux", "Diminue de 1 % par niveau le risque d'usure des objets.", 10) },
      { CustomFeatList.Sit, new CustomFeat("S'asseoir", "Ce don vous permet de vous asseoir puis d'ajuster l'affichage de votre personnage (mais pas sa position réelle). \n\nIl est possible de choisir une autre emote afin de s'afficher sous une autre posture.", 1) },
      { CustomFeatList.MetalRepair, new CustomFeat("Réparation Forge", "Permet de réparer les objets métalliques. Diminue de 1 % par niveau le temps de réparation et le coût en matériaux.", 10) },
      { CustomFeatList.WoodRepair, new CustomFeat("Réparation Ebenisterie", "Permet de réparer les objets en bois. Diminue de 1 % par niveau le temps de réparation et le coût en matériaux.", 10) },
      { CustomFeatList.LeatherRepair, new CustomFeat("Réparation Tannerie", "Permet de réparer les objets en cuir. Diminue de 1 % par niveau le temps de réparation et le coût en matériaux.", 10) },
      { CustomFeatList.EnchantRepair, new CustomFeat("Réenchantement", "Permet de réactiver les enchantements d'un objet ruiné. Diminue de 1 % par niveau le temps de réactivation.", 10) },
      { CustomFeatList.ImprovedDodge, new CustomFeat("Esquive améliorée", "Augmente la probabilité d'esquiver une attaque de 2% par niveau.", 10) },
      { CustomFeatList.AlchemistEfficiency, new CustomFeat("Alchimiste économe", "Permet l'utilisation du mortier et produit 1 * [niveau] de poudre à partir d'un ingrédient d'alchimie.", 5) },
      { CustomFeatList.AlchemistCareful, new CustomFeat("Alchimiste prudent", "Permet d'ajouter de l'eau à un mélange alchimique afin d'adoucir le mélange et de retourner vers l'état neutre.", 1) },
      { CustomFeatList.AlchemistExpert, new CustomFeat("Alchimiste expert", "Permet d'ajouter un effet supplémentaire à une potion par niveau.", 5) },
      { CustomFeatList.Alchemist, new CustomFeat("Alchimiste", "Diminue le temps d'infusion d'une potion de 2 % par niveau.", 10) },
      { CustomFeatList.AlchemistAware, new CustomFeat("Alchimiste attentif", "Permet de distinguer les changements de couleurs lors d'un mélange alchimique.\n\n Donne des indications sur la proximité d'un effet.", 5) },
      { CustomFeatList.AlchemistAccurate, new CustomFeat("Alchimiste précis", "Permet de distinguer les changements d'odeurs lors d'un mélange alchimique.\n\n Donne des indications sur le type de solution vers lequel tendre pour obtenir l'effet le plus proche.", 5) },
      { CustomFeatList.LongSwordMastery, new CustomFeat("Maîtrise de l'épée longue", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.FistMastery, new CustomFeat("Maîtrise du combat à main nue", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.ShortSwordMastery, new CustomFeat("Maîtrise de l'épée courte", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.BattleAxeMastery, new CustomFeat("Maîtrise de la hache d'armes", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.BastardsSwordMastery, new CustomFeat("Maîtrise de l'épée bâtarde", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.LightFlailMastery, new CustomFeat("Maîtrise du fléau léger", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.WarhammerMastery, new CustomFeat("Maîtrise du combat marteau de guerre", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.HeavyCrossbowMastery, new CustomFeat("Maîtrise de l'arbalète lourde", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.LightCrossbowMastery, new CustomFeat("Maîtrise de l'arbalète légère", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.LongbowMastery, new CustomFeat("Maîtrise de l'arc long", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.LightMaceMastery, new CustomFeat("Maîtrise de la masse légère", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.HalberdMastery, new CustomFeat("Maîtrise de la hallebarde", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.TwoBladedSwordMastery, new CustomFeat("Maîtrise de la double lame", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.ShortbowMastery, new CustomFeat("Maîtrise de l'arc court", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.GreatSwordMastery, new CustomFeat("Maîtrise de l'épée à deux mains", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.GreatAxeMastery, new CustomFeat("Maîtrise de la grande hache", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.DaggerMastery, new CustomFeat("Maîtrise de la dague", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.ClubMastery, new CustomFeat("Maîtrise du gourdin", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.DartMastery, new CustomFeat("Maîtrise des dards", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.DireMaceMastery, new CustomFeat("Maîtrise de la masse double", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.DoubleAxeMastery, new CustomFeat("Maîtrise de la hache double", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.HeavyFlailMastery, new CustomFeat("Maîtrise du fléau lourd", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.LightHammerMastery, new CustomFeat("Maîtrise du marteau léger", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.HandAxeMastery, new CustomFeat("Maîtrise de la hachette", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.KamaMastery, new CustomFeat("Maîtrise du kama", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.KatanaMastery, new CustomFeat("Maîtrise du katana", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.KukriMastery, new CustomFeat("Maîtrise du kukri", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.MagicStaffMastery, new CustomFeat("Maîtrise du bourdon", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.MorningStarMastery, new CustomFeat("Maîtrise de l'étoile du matin", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.QuarterStaffMastery, new CustomFeat("Maîtrise du bâton", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.RapierMastery, new CustomFeat("Maîtrise de la rapière", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.ScimitarMastery, new CustomFeat("Maîtrise du cimeterre", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.ScytheMastery, new CustomFeat("Maîtrise de la faux", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.ShortSpearMastery, new CustomFeat("Maîtrise de la lance", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.ShurikenMastery, new CustomFeat("Maîtrise du shuriken", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.SickleMastery, new CustomFeat("Maîtrise de la serpe", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.SlingMastery, new CustomFeat("Maîtrise de la fronde", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.ThrowingAxeMastery, new CustomFeat("Maîtrise de la hache de lancer", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.TridentMastery, new CustomFeat("Maîtrise du trident", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.DwarvenWaraxeMastery, new CustomFeat("Maîtrise de la hache naine", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.WhipMastery, new CustomFeat("Maîtrise du fouet", "Permet d'effectuer 10 * [niveau] % des dégâts de base avec ce type d'arme.", 20) },
      { CustomFeatList.LongSwordScience, new CustomFeat("Science de l'épée longue", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.FistScience, new CustomFeat("Science du combat à main nue", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.ShortSwordScience, new CustomFeat("Science de l'épée courte", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.BattleAxeScience, new CustomFeat("Science de la hache d'armes", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.BastardsSwordScience, new CustomFeat("Science de l'épée bâtarde", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.LightFlailScience, new CustomFeat("Science du fléau léger", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.WarhammerScience, new CustomFeat("Science du combat marteau de guerre", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.HeavyCrossbowScience, new CustomFeat("Science de l'arbalète lourde", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.LightCrossbowScience, new CustomFeat("Science de l'arbalète légère", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.LongbowScience, new CustomFeat("Science de l'arc long", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.LightMaceScience, new CustomFeat("Science de la masse légère", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.HalberdScience, new CustomFeat("Science de la hallebarde", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.TwoBladedSwordScience, new CustomFeat("Science de la double lame", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.ShortbowScience, new CustomFeat("Science de l'arc court", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.GreatSwordScience, new CustomFeat("Science de l'épée à deux mains", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.GreatAxeScience, new CustomFeat("Science de la grande hache", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.DaggerScience, new CustomFeat("Science de la dague", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.ClubScience, new CustomFeat("Science du gourdin", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.DartScience, new CustomFeat("Science des dards", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.DireMaceScience, new CustomFeat("Science de la masse double", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.DoubleAxeScience, new CustomFeat("Science de la hache double", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.HeavyFlailScience, new CustomFeat("Science du fléau lourd", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.LightHammerScience, new CustomFeat("Science du marteau léger", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.HandAxeScience, new CustomFeat("Science de la hachette", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.KamaScience, new CustomFeat("Science du kama", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.KatanaScience, new CustomFeat("Science du katana", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.KukriScience, new CustomFeat("Science du kukri", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.MagicStaffScience, new CustomFeat("Science du bourdon", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.MorningStarScience, new CustomFeat("Science de l'étoile du matin", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.QuarterStaffScience, new CustomFeat("Science du bâton", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.RapierScience, new CustomFeat("Science de la rapière", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.ScimitarScience, new CustomFeat("Science du cimeterre", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.ScytheScience, new CustomFeat("Science de la faux", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.ShortSpearScience, new CustomFeat("Science de la lance", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.ShurikenScience, new CustomFeat("Science du shuriken", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.SickleScience, new CustomFeat("Science de la serpe", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.SlingScience, new CustomFeat("Science de la fronde", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.ThrowingAxeScience, new CustomFeat("Science de la hache de lancer", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.TridentScience, new CustomFeat("Science du trident", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.DwarvenWaraxeScience, new CustomFeat("Science de la hache naine", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
      { CustomFeatList.WhipScience, new CustomFeat("Science du fouet", "Augmente les chances de coup critique avec cette arme de 1% par niveau.", 20) },
    };

    public static Dictionary<Feat, Func<PlayerSystem.Player, Feat, int>> RegisterAddCustomFeatEffect = new Dictionary<Feat, Func<PlayerSystem.Player, Feat, int>>
    {
            { CustomFeatList.ImprovedHealth, HandleHealthPoints },
            { CustomFeatList.ImprovedStrength, HandleImproveAbility },
            { CustomFeatList.ImprovedDexterity, HandleImproveAbility },
            { CustomFeatList.ImprovedConstitution, HandleImproveAbility },
            { CustomFeatList.ImprovedIntelligence, HandleImproveAbility },
            { CustomFeatList.ImprovedWisdom, HandleImproveAbility },
            { CustomFeatList.ImprovedCharisma, HandleImproveAbility },
            { CustomFeatList.ImprovedAttackBonus, HandleImproveAttack },
            { CustomFeatList.ImprovedSavingThrowAll, HandleImproveSavingThrowAll },
            { CustomFeatList.ImprovedSavingThrowFortitude, HandleImproveSavingThrowFortitude },
            { CustomFeatList.ImprovedSavingThrowWill, HandleImproveSavingThrowWill },
            { CustomFeatList.ImprovedSavingThrowReflex, HandleImproveSavingThrowReflex },
            { CustomFeatList.ImprovedAnimalEmpathy, HandleImproveAnimalEmpathy },
            { CustomFeatList.ImprovedConcentration, HandleImproveConcentration },
            { CustomFeatList.ImprovedDisableTraps, HandleImproveDisableTraps },
            { CustomFeatList.ImprovedDiscipline, HandleImproveDiscipline },
            { CustomFeatList.ImprovedHeal, HandleImproveHeal },
            { CustomFeatList.ImprovedHide, HandleImproveHide },
            { CustomFeatList.ImprovedListen, HandleImproveListen },
            { CustomFeatList.ImprovedLore, HandleImproveLore },
            { CustomFeatList.ImprovedMoveSilently, HandleImproveMoveSilently },
            { CustomFeatList.ImprovedOpenLock, HandleImproveOpenLock },
            { CustomFeatList.ImprovedSkillParry, HandleImproveSkillParry },
            { CustomFeatList.ImprovedPerform, HandleImprovePerform },
            { CustomFeatList.ImprovedPickpocket, HandleImprovePickpocket },
            { CustomFeatList.ImprovedSearch, HandleImproveSearch },
            { CustomFeatList.ImprovedSetTrap, HandleImproveSetTrap },
            { CustomFeatList.ImprovedSpellcraft, HandleImproveSpellcraft },
            { CustomFeatList.ImprovedSpot, HandleImproveSpot },
            { CustomFeatList.ImprovedTaunt, HandleImproveTaunt },
            { CustomFeatList.ImprovedUseMagicDevice, HandleImproveUseMagicDevice },
            { CustomFeatList.ImprovedTumble, HandleImproveTumble },
            { CustomFeatList.ImprovedBluff, HandleImproveBluff },
            { CustomFeatList.ImprovedIntimidate, HandleImproveIntimidate },
            { CustomFeatList.ImprovedSpellSlot0, HandleImproveSpellSlot0 },
            { CustomFeatList.ImprovedSpellSlot1, HandleImproveSpellSlot1 },
            { CustomFeatList.ImprovedSpellSlot2, HandleImproveSpellSlot2 },
            { CustomFeatList.ImprovedSpellSlot3, HandleImproveSpellSlot3 },
            { CustomFeatList.ImprovedSpellSlot4, HandleImproveSpellSlot4 },
            { CustomFeatList.ImprovedSpellSlot5, HandleImproveSpellSlot5 },
            { CustomFeatList.ImprovedSpellSlot6, HandleImproveSpellSlot6 },
            { CustomFeatList.ImprovedSpellSlot7, HandleImproveSpellSlot7 },
            { CustomFeatList.ImprovedSpellSlot8, HandleImproveSpellSlot8 },
            { CustomFeatList.ImprovedSpellSlot9, HandleImproveSpellSlot9 },
    };

    public static Dictionary<Feat, Func<PlayerSystem.Player, Feat, int>> RegisterRemoveCustomFeatEffect = new Dictionary<Feat, Func<PlayerSystem.Player, Feat, int>>
    {
      //{ 1130, HandleRemoveStrengthMalusFeat },
    };

    private static int HandleHealthPoints(PlayerSystem.Player player, Feat feat)
    {
      int improvedHealth = 0;
      if (player.learntCustomFeats.ContainsKey(CustomFeatList.ImprovedHealth))
        improvedHealth = GetCustomFeatLevelFromSkillPoints(CustomFeatList.ImprovedHealth, player.learntCustomFeats[CustomFeatList.ImprovedHealth]);

      player.oid.LoginCreature.LevelInfo[0].HitDie = (byte)(10
        + (1 + 3 * ((player.oid.LoginCreature.GetAbilityScore(Ability.Constitution, true) - 10) / 2)
        + Convert.ToInt32(player.oid.LoginCreature.KnowsFeat(Feat.Toughness))) * improvedHealth);

      return 0;
    }
    private static int HandleImproveAbility(PlayerSystem.Player player, Feat feat)
    {
      switch (feat)
      {
        case CustomFeatList.ImprovedStrength:
          player.oid.LoginCreature.SetsRawAbilityScore(Ability.Strength, (byte)(player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength) + 1));
          break;
        case CustomFeatList.ImprovedDexterity:
          player.oid.LoginCreature.SetsRawAbilityScore(Ability.Dexterity, (byte)(player.oid.LoginCreature.GetRawAbilityScore(Ability.Dexterity) + 1));
          break;
        case CustomFeatList.ImprovedConstitution:
          player.oid.LoginCreature.SetsRawAbilityScore(Ability.Constitution, (byte)(player.oid.LoginCreature.GetRawAbilityScore(Ability.Constitution) + 1));
          HandleHealthPoints(player, feat);
          break;
        case CustomFeatList.ImprovedIntelligence:
          player.oid.LoginCreature.SetsRawAbilityScore(Ability.Intelligence, (byte)(player.oid.LoginCreature.GetRawAbilityScore(Ability.Intelligence) + 1));
          break;
        case CustomFeatList.ImprovedWisdom:
          player.oid.LoginCreature.SetsRawAbilityScore(Ability.Wisdom, (byte)(player.oid.LoginCreature.GetRawAbilityScore(Ability.Wisdom) + 1));
          break;
        case CustomFeatList.ImprovedCharisma:
          player.oid.LoginCreature.SetsRawAbilityScore(Ability.Charisma, (byte)(player.oid.LoginCreature.GetRawAbilityScore(Ability.Charisma) + 1));
          break;
      }

      return 0;
    }
    private static int HandleImproveAttack(PlayerSystem.Player player, Feat feat)
    {
      player.oid.LoginCreature.BaseAttackBonus += 1;
      return 0;
    }
    private static int HandleImproveSavingThrowAll(PlayerSystem.Player player, Feat feat)
    {
      player.oid.LoginCreature.SetBaseSavingThrow(SavingThrow.All, (sbyte)(player.oid.LoginCreature.GetBaseSavingThrow(SavingThrow.All) + 1));
      return 0;
    }
    private static int HandleImproveSavingThrowFortitude(PlayerSystem.Player player, Feat feat)
    {
      player.oid.LoginCreature.SetBaseSavingThrow(SavingThrow.Fortitude, (sbyte)(player.oid.LoginCreature.GetBaseSavingThrow(SavingThrow.Fortitude) + 1));
      return 0;
    }
    private static int HandleImproveSavingThrowWill(PlayerSystem.Player player, Feat feat)
    {
      player.oid.LoginCreature.SetBaseSavingThrow(SavingThrow.Will, (sbyte)(player.oid.LoginCreature.GetBaseSavingThrow(SavingThrow.Will) + 1));
      return 0;
    }
    private static int HandleImproveSavingThrowReflex(PlayerSystem.Player player, Feat feat)
    {
      player.oid.LoginCreature.SetBaseSavingThrow(SavingThrow.Reflex, (sbyte)(player.oid.LoginCreature.GetBaseSavingThrow(SavingThrow.Reflex) + 1));
      return 0;
    }
    private static int HandleImproveAnimalEmpathy(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid.LoginCreature, NWScript.SKILL_ANIMAL_EMPATHY, player.oid.LoginCreature.GetSkillRank(Anvil.API.Skill.AnimalEmpathy, true) + 1);
      return 0;
    }
    private static int HandleImproveConcentration(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid.LoginCreature, NWScript.SKILL_CONCENTRATION, player.oid.LoginCreature.GetSkillRank(Anvil.API.Skill.Concentration, true) + 1);
      return 0;
    }
    private static int HandleImproveDisableTraps(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid.LoginCreature, NWScript.SKILL_DISABLE_TRAP, player.oid.LoginCreature.GetSkillRank(Anvil.API.Skill.DisableTrap, true) + 1);
      return 0;
    }
    private static int HandleImproveDiscipline(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid.LoginCreature, NWScript.SKILL_DISCIPLINE, player.oid.LoginCreature.GetSkillRank(Anvil.API.Skill.Discipline, true) + 1);
      return 0;
    }
    private static int HandleImproveHeal(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid.LoginCreature, NWScript.SKILL_HEAL, player.oid.LoginCreature.GetSkillRank(Anvil.API.Skill.Heal, true) + 1);
      return 0;
    }
    private static int HandleImproveHide(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid.LoginCreature, NWScript.SKILL_HIDE, player.oid.LoginCreature.GetSkillRank(Anvil.API.Skill.Hide, true) + 1);
      return 0;
    }
    private static int HandleImproveListen(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid.LoginCreature, NWScript.SKILL_LISTEN, player.oid.LoginCreature.GetSkillRank(Anvil.API.Skill.Listen, true) + 1);
      return 0;
    }
    private static int HandleImproveLore(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid.LoginCreature, NWScript.SKILL_LORE, player.oid.LoginCreature.GetSkillRank(Anvil.API.Skill.Lore, true) + 1);
      return 0;
    }
    private static int HandleImproveMoveSilently(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid.LoginCreature, NWScript.SKILL_MOVE_SILENTLY, player.oid.LoginCreature.GetSkillRank(Anvil.API.Skill.MoveSilently, true) + 1);
      return 0;
    }
    private static int HandleImproveOpenLock(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid.LoginCreature, NWScript.SKILL_OPEN_LOCK, player.oid.LoginCreature.GetSkillRank(Anvil.API.Skill.OpenLock, true) + 1);
      return 0;
    }
    private static int HandleImproveSkillParry(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid.LoginCreature, NWScript.SKILL_PARRY, player.oid.LoginCreature.GetSkillRank(Anvil.API.Skill.Parry, true) + 1);
      return 0;
    }
    private static int HandleImprovePerform(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid.LoginCreature, NWScript.SKILL_PERFORM, player.oid.LoginCreature.GetSkillRank(Anvil.API.Skill.Perform, true) + 1);
      return 0;
    }
    private static int HandleImprovePickpocket(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid.LoginCreature, NWScript.SKILL_PICK_POCKET, player.oid.LoginCreature.GetSkillRank(Anvil.API.Skill.PickPocket, true) + 1);
      return 0;
    }
    private static int HandleImproveSearch(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid.LoginCreature, NWScript.SKILL_SEARCH, player.oid.LoginCreature.GetSkillRank(Anvil.API.Skill.Search, true) + 1);
      return 0;
    }
    private static int HandleImproveSetTrap(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid.LoginCreature, NWScript.SKILL_SET_TRAP, player.oid.LoginCreature.GetSkillRank(Anvil.API.Skill.SetTrap, true) + 1);
      return 0;
    }
    private static int HandleImproveSpellcraft(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid.LoginCreature, NWScript.SKILL_SPELLCRAFT, player.oid.LoginCreature.GetSkillRank(Anvil.API.Skill.AnimalEmpathy, true) + 1);
      return 0;
    }
    private static int HandleImproveSpot(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid.LoginCreature, NWScript.SKILL_SPOT, player.oid.LoginCreature.GetSkillRank(Anvil.API.Skill.Spot, true) + 1);
      return 0;
    }
    private static int HandleImproveTaunt(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid.LoginCreature, NWScript.SKILL_TAUNT, player.oid.LoginCreature.GetSkillRank(Anvil.API.Skill.Taunt, true) + 1);
      return 0;
    }
    private static int HandleImproveUseMagicDevice(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid.LoginCreature, NWScript.SKILL_USE_MAGIC_DEVICE, player.oid.LoginCreature.GetSkillRank(Anvil.API.Skill.UseMagicDevice, true) + 1);
      return 0;
    }
    private static int HandleImproveTumble(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid.LoginCreature, NWScript.SKILL_TUMBLE, player.oid.LoginCreature.GetSkillRank(Anvil.API.Skill.Tumble, true) + 1);
      return 0;
    }
    private static int HandleImproveBluff(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid.LoginCreature, NWScript.SKILL_BLUFF, player.oid.LoginCreature.GetSkillRank(Anvil.API.Skill.Bluff, true) + 1);
      return 0;
    }
    private static int HandleImproveIntimidate(PlayerSystem.Player player, Feat feat)
    {
      CreaturePlugin.SetSkillRank(player.oid.LoginCreature, NWScript.SKILL_INTIMIDATE, player.oid.LoginCreature.GetSkillRank(Anvil.API.Skill.Intimidate, true) + 1);
      return 0;
    }

    private static int HandleImproveSpellSlot0(PlayerSystem.Player player, Feat feat)
    {
      NwItem skin = player.oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin);
      if (skin != null)
        skin.AddItemProperty(ItemProperty.BonusLevelSpell((IPClass)43, IPSpellLevel.SL0), EffectDuration.Permanent);
      else
        MiscUtils.LogMessageToDMs($"Skill System - On Improve Spell Slot : {player.oid.LoginCreature.Name} creature skin is null !");

      return 0;
    }
    private static int HandleImproveSpellSlot1(PlayerSystem.Player player, Feat feat)
    {
      NwItem skin = player.oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin);
      if (skin != null)
        skin.AddItemProperty(ItemProperty.BonusLevelSpell((IPClass)43, IPSpellLevel.SL1), EffectDuration.Permanent);
      else
        MiscUtils.LogMessageToDMs($"Skill System - On Improve Spell Slot : {player.oid.LoginCreature.Name} creature skin is null !");

      return 0;
    }
    private static int HandleImproveSpellSlot2(PlayerSystem.Player player, Feat feat)
    {
      NwItem skin = player.oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin);
      if (skin != null)
        skin.AddItemProperty(ItemProperty.BonusLevelSpell((IPClass)43, IPSpellLevel.SL2), EffectDuration.Permanent);
      else
        MiscUtils.LogMessageToDMs($"Skill System - On Improve Spell Slot : {player.oid.LoginCreature.Name} creature skin is null !");

      return 0;
    }
    private static int HandleImproveSpellSlot3(PlayerSystem.Player player, Feat feat)
    {
      NwItem skin = player.oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin);
      if (skin != null)
        skin.AddItemProperty(ItemProperty.BonusLevelSpell((IPClass)43, IPSpellLevel.SL3), EffectDuration.Permanent);
      else
        MiscUtils.LogMessageToDMs($"Skill System - On Improve Spell Slot : {player.oid.LoginCreature.Name} creature skin is null !");

      return 0;
    }
    private static int HandleImproveSpellSlot4(PlayerSystem.Player player, Feat feat)
    {
      NwItem skin = player.oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin);
      if (skin != null)
        skin.AddItemProperty(ItemProperty.BonusLevelSpell((IPClass)43, IPSpellLevel.SL4), EffectDuration.Permanent);
      else
        MiscUtils.LogMessageToDMs($"Skill System - On Improve Spell Slot : {player.oid.LoginCreature.Name} creature skin is null !");

      return 0;
    }
    private static int HandleImproveSpellSlot5(PlayerSystem.Player player, Feat feat)
    {
      NwItem skin = player.oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin);
      if (skin != null)
        skin.AddItemProperty(ItemProperty.BonusLevelSpell((IPClass)43, IPSpellLevel.SL5), EffectDuration.Permanent);
      else
        MiscUtils.LogMessageToDMs($"Skill System - On Improve Spell Slot : {player.oid.LoginCreature.Name} creature skin is null !");

      return 0;
    }
    private static int HandleImproveSpellSlot6(PlayerSystem.Player player, Feat feat)
    {
      NwItem skin = player.oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin);
      if (skin != null)
        skin.AddItemProperty(ItemProperty.BonusLevelSpell((IPClass)43, IPSpellLevel.SL6), EffectDuration.Permanent);
      else
        MiscUtils.LogMessageToDMs($"Skill System - On Improve Spell Slot : {player.oid.LoginCreature.Name} creature skin is null !");

      return 0;
    }
    private static int HandleImproveSpellSlot7(PlayerSystem.Player player, Feat feat)
    {
      NwItem skin = player.oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin);
      if (skin != null)
        skin.AddItemProperty(ItemProperty.BonusLevelSpell((IPClass)43, IPSpellLevel.SL7), EffectDuration.Permanent);
      else
        MiscUtils.LogMessageToDMs($"Skill System - On Improve Spell Slot : {player.oid.LoginCreature.Name} creature skin is null !");

      return 0;
    }
    private static int HandleImproveSpellSlot8(PlayerSystem.Player player, Feat feat)
    {
      NwItem skin = player.oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin);
      if (skin != null)
        skin.AddItemProperty(ItemProperty.BonusLevelSpell((IPClass)43, IPSpellLevel.SL8), EffectDuration.Permanent);
      else
        MiscUtils.LogMessageToDMs($"Skill System - On Improve Spell Slot : {player.oid.LoginCreature.Name} creature skin is null !");

      return 0;
    }
    private static int HandleImproveSpellSlot9(PlayerSystem.Player player, Feat feat)
    {
      NwItem skin = player.oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin);
      if (skin != null)
        skin.AddItemProperty(ItemProperty.BonusLevelSpell((IPClass)43, IPSpellLevel.SL9), EffectDuration.Permanent);
      else
        MiscUtils.LogMessageToDMs($"Skill System - On Improve Spell Slot : {player.oid.LoginCreature.Name} creature skin is null !");

      return 0;
    }

    public static Feat[] forgeBasicSkillBooks = new Feat[] { CustomFeatList.Renforcement, CustomFeatList.Recycler, CustomFeatList.CraftOreExtractor, CustomFeatList.CraftForgeHammer, CustomFeatList.Metallurgy, CustomFeatList.Research, CustomFeatList.Miner, CustomFeatList.Prospection, CustomFeatList.StripMiner, CustomFeatList.Reprocessing, CustomFeatList.Forge, CustomFeatList.CraftScaleMail, CustomFeatList.CraftDagger, CustomFeatList.CraftLightMace, CustomFeatList.CraftMorningStar, CustomFeatList.CraftSickle, CustomFeatList.CraftShortSpear };
    public static Feat[] woodBasicSkillBooks = new Feat[] { CustomFeatList.Renforcement, CustomFeatList.Recycler, CustomFeatList.CraftOreExtractor, CustomFeatList.CraftForgeHammer, CustomFeatList.Metallurgy, CustomFeatList.Research, CustomFeatList.WoodCutter, CustomFeatList.WoodProspection, CustomFeatList.StripMiner, CustomFeatList.WoodReprocessing, CustomFeatList.Ebeniste, CustomFeatList.CraftSmallShield, CustomFeatList.CraftClub, CustomFeatList.CraftDarts, CustomFeatList.CraftBullets, CustomFeatList.CraftHeavyCrossbow, CustomFeatList.CraftLightCrossbow, CustomFeatList.CraftQuarterstaff, CustomFeatList.CraftSling, CustomFeatList.CraftArrow, CustomFeatList.CraftBolt };
    public static Feat[] leatherBasicSkillBooks = new Feat[] { CustomFeatList.Renforcement, CustomFeatList.Recycler, CustomFeatList.Hunting, CustomFeatList.Skinning, CustomFeatList.Tanner, CustomFeatList.PeltReprocessing, CustomFeatList.CraftLeatherArmor, CustomFeatList.CraftStuddedLeather, CustomFeatList.CraftPaddedArmor, CustomFeatList.CraftClothing, CustomFeatList.CraftWhip, CustomFeatList.CraftBelt, CustomFeatList.CraftBoots, CustomFeatList.CraftBracer, CustomFeatList.CraftCloak, CustomFeatList.CraftGloves };
    //public static Feat[] craftSkillBooks = new Feat[] { CustomFeatList.Metallurgy, CustomFeatList.AdvancedCraft, CustomFeatList.Miner, CustomFeatList.Geology, CustomFeatList.Prospection, CustomFeatList.VeldsparReprocessing, CustomFeatList.ScorditeReprocessing, CustomFeatList.PyroxeresReprocessing, CustomFeatList.StripMiner, CustomFeatList.Reprocessing, CustomFeatList.ReprocessingEfficiency, CustomFeatList.Connections, CustomFeatList.Forge };
    public static Feat[] alchemyBasicSkillBooks = new Feat[] { CustomFeatList.Alchemist, CustomFeatList.AlchemistCareful, CustomFeatList.AlchemistEfficiency };
    public static Feat[] languageSkillBooks = new Feat[] { CustomFeatList.Abyssal, CustomFeatList.Céleste, CustomFeatList.Gnome, CustomFeatList.Draconique, CustomFeatList.Druidique, CustomFeatList.Nain, CustomFeatList.Elfique, CustomFeatList.Géant, CustomFeatList.Gobelin, CustomFeatList.Halfelin, CustomFeatList.Infernal, CustomFeatList.Orc, CustomFeatList.Primordiale, CustomFeatList.Sylvain, CustomFeatList.Voleur, CustomFeatList.Gnome };

    public static Feat[] lowSkillBooks = new Feat[] { CustomFeatList.AlchemistExpert, CustomFeatList.Renforcement, CustomFeatList.ArtisanApplique, CustomFeatList.Enchanteur, CustomFeatList.Comptabilite, CustomFeatList.BrokerRelations, CustomFeatList.Negociateur, CustomFeatList.Magnat, CustomFeatList.Marchand, CustomFeatList.Recycler, Feat.Ambidexterity, CustomFeatList.Skinning, CustomFeatList.Hunting, CustomFeatList.ImprovedSpellSlot2, CustomFeatList.WoodReprocessing, CustomFeatList.Ebeniste, CustomFeatList.WoodCutter, CustomFeatList.WoodProspection, CustomFeatList.CraftOreExtractor, CustomFeatList.CraftForgeHammer, CustomFeatList.Forge, CustomFeatList.Reprocessing, CustomFeatList.BlueprintCopy, CustomFeatList.Research, CustomFeatList.Miner, CustomFeatList.Metallurgy, Feat.DeneirsEye, Feat.DirtyFighting, Feat.ResistDisease, Feat.Stealthy, Feat.SkillFocusAnimalEmpathy, Feat.SkillFocusBluff, Feat.SkillFocusConcentration, Feat.SkillFocusDisableTrap, Feat.SkillFocusDiscipline, Feat.SkillFocusHeal, Feat.SkillFocusHide, Feat.SkillFocusIntimidate, Feat.SkillFocusListen, Feat.SkillFocusLore, Feat.SkillFocusMoveSilently, Feat.SkillFocusOpenLock, Feat.SkillFocusParry, Feat.SkillFocusPerform, Feat.SkillFocusPickPocket, Feat.SkillFocusSearch, Feat.SkillFocusSetTrap, Feat.SkillFocusSpellcraft, Feat.SkillFocusSpot, Feat.SkillFocusTaunt, Feat.SkillFocusTumble, Feat.SkillFocusUseMagicDevice, Feat.Mobility, Feat.PointBlankShot, Feat.IronWill, Feat.Alertness, Feat.CombatCasting, Feat.Dodge, Feat.ExtraTurning, Feat.GreatFortitude };
    public static Feat[] mediumSkillBooks = new Feat[] { CustomFeatList.AlchemistAccurate, CustomFeatList.AlchemistAware, CustomFeatList.CombattantPrecautionneux, CustomFeatList.EnchanteurExpert, CustomFeatList.BrokerAffinity, CustomFeatList.BadPeltReprocessing, CustomFeatList.CommonPeltReprocessing, CustomFeatList.NormalPeltReprocessing, CustomFeatList.UncommunPeltReprocessing, CustomFeatList.RarePeltReprocessing, CustomFeatList.MagicPeltReprocessing, CustomFeatList.EpicPeltReprocessing, CustomFeatList.LegendaryPeltReprocessing, CustomFeatList.ImprovedSpellSlot3, CustomFeatList.ImprovedSpellSlot4, CustomFeatList.LaurelinReprocessing, CustomFeatList.MallornReprocessing, CustomFeatList.TelperionReprocessing, CustomFeatList.OiolaireReprocessing, CustomFeatList.NimlothReprocessing, CustomFeatList.QlipothReprocessing, CustomFeatList.FerocheneReprocessing, CustomFeatList.ValinorReprocessing, CustomFeatList.WoodReprocessingEfficiency, CustomFeatList.AnimalExpertise, CustomFeatList.CraftTorch, CustomFeatList.CraftStuddedLeather, CustomFeatList.CraftSling, CustomFeatList.CraftSmallShield, CustomFeatList.CraftSickle, CustomFeatList.CraftShortSpear, CustomFeatList.CraftRing, CustomFeatList.CraftPaddedArmor, CustomFeatList.CraftPotion, CustomFeatList.CraftQuarterstaff, CustomFeatList.CraftMorningStar, CustomFeatList.CraftMagicWand, CustomFeatList.CraftLightMace, CustomFeatList.CraftLightHammer, CustomFeatList.CraftLightFlail, CustomFeatList.CraftLightCrossbow, CustomFeatList.CraftLeatherArmor, CustomFeatList.CraftBullets, CustomFeatList.CraftCloak, CustomFeatList.CraftClothing, CustomFeatList.CraftClub, CustomFeatList.CraftDagger, CustomFeatList.CraftDarts, CustomFeatList.CraftGloves, CustomFeatList.CraftHeavyCrossbow, CustomFeatList.CraftHelmet, CustomFeatList.CraftAmulet, CustomFeatList.CraftArrow, CustomFeatList.CraftBelt, CustomFeatList.CraftBolt, CustomFeatList.CraftBoots, CustomFeatList.CraftBracer, CustomFeatList.ReprocessingEfficiency, CustomFeatList.StripMiner, CustomFeatList.VeldsparReprocessing, CustomFeatList.ScorditeReprocessing, CustomFeatList.PyroxeresReprocessing, CustomFeatList.PlagioclaseReprocessing, CustomFeatList.Geology, CustomFeatList.Prospection, Feat.TymorasSmile, Feat.LliirasHeart, Feat.RapidReload, Feat.Expertise, Feat.ImprovedInitiative, Feat.DefensiveRoll, Feat.SneakAttack, Feat.FlurryOfBlows, Feat.WeaponSpecializationHeavyCrossbow, Feat.WeaponSpecializationDagger, Feat.WeaponSpecializationDart, Feat.WeaponSpecializationClub, Feat.StillSpell, Feat.RapidShot, Feat.SilenceSpell, Feat.PowerAttack, Feat.Knockdown, Feat.LightningReflexes, Feat.ImprovedUnarmedStrike, Feat.Cleave, Feat.CalledShot, Feat.DeflectArrows, Feat.WeaponSpecializationLightCrossbow, Feat.WeaponSpecializationLightFlail, Feat.WeaponSpecializationLightMace, Feat.Disarm, Feat.EmpowerSpell, Feat.WeaponSpecializationMorningStar, Feat.ExtendSpell, Feat.SpellFocusAbjuration, Feat.SpellFocusConjuration, Feat.SpellFocusDivination, Feat.SpellFocusEnchantment, Feat.WeaponSpecializationSickle, Feat.WeaponSpecializationSling, Feat.WeaponSpecializationSpear, Feat.WeaponSpecializationStaff, Feat.WeaponSpecializationThrowingAxe, Feat.WeaponSpecializationTrident, Feat.WeaponSpecializationUnarmedStrike, Feat.SpellFocusEvocation, Feat.SpellFocusIllusion, Feat.SpellFocusNecromancy, Feat.SpellFocusTransmutation, Feat.SpellPenetration };
    public static Feat[] highSkillBooks = new Feat[] { CustomFeatList.ImprovedDodge, CustomFeatList.EnchanteurChanceux, CustomFeatList.SurchargeControlee, CustomFeatList.SurchargeArcanique, CustomFeatList.ArtisanExceptionnel, CustomFeatList.AdvancedCraft, CustomFeatList.CraftWarHammer, CustomFeatList.CraftTrident, CustomFeatList.CraftThrowingAxe, CustomFeatList.CraftStaff, CustomFeatList.CraftSplintMail, CustomFeatList.CraftSpellScroll, CustomFeatList.CraftShortsword, CustomFeatList.CraftShortBow, CustomFeatList.CraftScimitar, CustomFeatList.CraftScaleMail, CustomFeatList.CraftRapier, CustomFeatList.CraftMagicRod, CustomFeatList.CraftLongsword, CustomFeatList.CraftLongBow, CustomFeatList.CraftLargeShield, CustomFeatList.CraftBattleAxe, CustomFeatList.OmberReprocessing, CustomFeatList.KerniteReprocessing, CustomFeatList.GneissReprocessing, CustomFeatList.CraftHalberd, CustomFeatList.JaspetReprocessing, CustomFeatList.CraftHeavyFlail, CustomFeatList.CraftHandAxe, CustomFeatList.HemorphiteReprocessing, CustomFeatList.CraftGreatAxe, CustomFeatList.CraftGreatSword, Feat.ArcaneDefenseAbjuration, Feat.ArcaneDefenseConjuration, Feat.ArcaneDefenseDivination, Feat.ArcaneDefenseEnchantment, Feat.ArcaneDefenseEvocation, Feat.ArcaneDefenseIllusion, Feat.ArcaneDefenseNecromancy, Feat.ArcaneDefenseTransmutation, Feat.BlindFight, Feat.SpringAttack, Feat.GreatCleave, Feat.ImprovedExpertise, Feat.SkillMastery, Feat.Opportunist, Feat.Evasion, Feat.WeaponSpecializationDireMace, Feat.WeaponSpecializationDoubleAxe, Feat.WeaponSpecializationDwaxe, Feat.WeaponSpecializationGreatAxe, Feat.WeaponSpecializationGreatSword, Feat.WeaponSpecializationHalberd, Feat.WeaponSpecializationHandAxe, Feat.WeaponSpecializationHeavyFlail, Feat.WeaponSpecializationKama, Feat.WeaponSpecializationKatana, Feat.WeaponSpecializationKukri, Feat.WeaponSpecializationBastardSword, Feat.WeaponSpecializationLightHammer, Feat.WeaponSpecializationLongbow, Feat.WeaponSpecializationLongSword, Feat.WeaponSpecializationRapier, Feat.WeaponSpecializationScimitar, Feat.WeaponSpecializationScythe, Feat.WeaponSpecializationShortbow, Feat.WeaponSpecializationShortSword, Feat.WeaponSpecializationShuriken, Feat.WeaponSpecializationBattleAxe, Feat.QuickenSpell, Feat.MaximizeSpell, Feat.ImprovedTwoWeaponFighting, Feat.ImprovedPowerAttack, Feat.WeaponSpecializationTwoBladedSword, Feat.WeaponSpecializationWarHammer, Feat.WeaponSpecializationWhip, Feat.ImprovedDisarm, Feat.ImprovedKnockdown, Feat.ImprovedParry, Feat.ImprovedCriticalBastardSword, Feat.ImprovedCriticalBattleAxe, Feat.ImprovedCriticalClub, Feat.ImprovedCriticalDagger, Feat.ImprovedCriticalDart, Feat.ImprovedCriticalDireMace, Feat.ImprovedCriticalDoubleAxe, Feat.ImprovedCriticalDwaxe, Feat.ImprovedCriticalGreatAxe, Feat.ImprovedCriticalGreatSword, Feat.ImprovedCriticalHalberd, Feat.ImprovedCriticalHandAxe, Feat.ImprovedCriticalHeavyCrossbow, Feat.ImprovedCriticalHeavyFlail, Feat.ImprovedCriticalKama, Feat.ImprovedCriticalKatana, Feat.ImprovedCriticalKukri, Feat.ImprovedCriticalLightCrossbow, Feat.ImprovedCriticalLightFlail, Feat.ImprovedCriticalLightHammer, Feat.ImprovedCriticalLightMace, Feat.ImprovedCriticalLongbow, Feat.ImprovedCriticalLongSword, Feat.ImprovedCriticalMorningStar, Feat.ImprovedCriticalRapier, Feat.ImprovedCriticalScimitar, Feat.ImprovedCriticalScythe, Feat.ImprovedCriticalShortbow, Feat.ImprovedCriticalShortSword, Feat.ImprovedCriticalShuriken, Feat.ImprovedCriticalSickle, Feat.ImprovedCriticalSling, Feat.ImprovedCriticalSpear, Feat.ImprovedCriticalStaff, Feat.ImprovedCriticalThrowingAxe, Feat.ImprovedCriticalTrident, Feat.ImprovedCriticalTwoBladedSword, Feat.ImprovedCriticalUnarmedStrike, Feat.ImprovedCriticalWarHammer, Feat.ImprovedCriticalWhip };
    public static Feat[] epicSkillBooks = new Feat[] { CustomFeatList.CraftWhip, CustomFeatList.CraftTwoBladedSword, CustomFeatList.CraftTowerShield, CustomFeatList.CraftShuriken, CustomFeatList.CraftScythe, CustomFeatList.CraftKukri, CustomFeatList.CraftKatana, CustomFeatList.CraftBreastPlate, CustomFeatList.CraftDireMace, CustomFeatList.CraftDoubleAxe, CustomFeatList.CraftDwarvenWarAxe, CustomFeatList.CraftFullPlate, CustomFeatList.CraftHalfPlate, CustomFeatList.CraftBastardSword, CustomFeatList.CraftKama, CustomFeatList.DarkOchreReprocessing, CustomFeatList.CrokiteReprocessing, CustomFeatList.BistotReprocessing, Feat.ResistEnergyAcid, Feat.ResistEnergyCold, Feat.ResistEnergyElectrical, Feat.ResistEnergyFire, Feat.ResistEnergySonic, Feat.ZenArchery, Feat.CripplingStrike, Feat.SlipperyMind, Feat.GreaterSpellFocusAbjuration, Feat.GreaterSpellFocusConjuration, Feat.GreaterSpellFocusDivination, Feat.GreaterSpellFocusDiviniation, Feat.GreaterSpellFocusEnchantment, Feat.GreaterSpellFocusEvocation, Feat.GreaterSpellFocusIllusion, Feat.GreaterSpellFocusNecromancy, Feat.GreaterSpellFocusTransmutation, Feat.GreaterSpellPenetration };

    public static int[] shopBasicMagicScrolls = new int[] { NWScript.IP_CONST_CASTSPELL_ACID_SPLASH_1, NWScript.IP_CONST_CASTSPELL_DAZE_1, NWScript.IP_CONST_CASTSPELL_ELECTRIC_JOLT_1, NWScript.IP_CONST_CASTSPELL_FLARE_1, NWScript.IP_CONST_CASTSPELL_RAY_OF_FROST_1, NWScript.IP_CONST_CASTSPELL_RESISTANCE_5, NWScript.IP_CONST_CASTSPELL_BURNING_HANDS_5, NWScript.IP_CONST_CASTSPELL_CHARM_PERSON_2, NWScript.IP_CONST_CASTSPELL_COLOR_SPRAY_2, NWScript.IP_CONST_CASTSPELL_ENDURE_ELEMENTS_2, NWScript.IP_CONST_CASTSPELL_EXPEDITIOUS_RETREAT_5, NWScript.IP_CONST_CASTSPELL_GREASE_2, 459, 478, 460, NWScript.IP_CONST_CASTSPELL_MAGE_ARMOR_2, NWScript.IP_CONST_CASTSPELL_MAGIC_MISSILE_5, NWScript.IP_CONST_CASTSPELL_NEGATIVE_ENERGY_RAY_5, NWScript.IP_CONST_CASTSPELL_RAY_OF_ENFEEBLEMENT_2, NWScript.IP_CONST_CASTSPELL_SCARE_2, 469, NWScript.IP_CONST_CASTSPELL_SHIELD_5, NWScript.IP_CONST_CASTSPELL_SLEEP_5, NWScript.IP_CONST_CASTSPELL_SUMMON_CREATURE_I_5, NWScript.IP_CONST_CASTSPELL_AMPLIFY_5, NWScript.IP_CONST_CASTSPELL_BALAGARNSIRONHORN_7, NWScript.IP_CONST_CASTSPELL_LESSER_DISPEL_5, NWScript.IP_CONST_CASTSPELL_CURE_MINOR_WOUNDS_1, NWScript.IP_CONST_CASTSPELL_INFLICT_MINOR_WOUNDS_1, NWScript.IP_CONST_CASTSPELL_VIRTUE_1, NWScript.IP_CONST_CASTSPELL_BANE_5, NWScript.IP_CONST_CASTSPELL_BLESS_2, NWScript.IP_CONST_CASTSPELL_CURE_LIGHT_WOUNDS_5, NWScript.IP_CONST_CASTSPELL_DIVINE_FAVOR_5, NWScript.IP_CONST_CASTSPELL_DOOM_5, NWScript.IP_CONST_CASTSPELL_ENTROPIC_SHIELD_5, NWScript.IP_CONST_CASTSPELL_INFLICT_LIGHT_WOUNDS_5, NWScript.IP_CONST_CASTSPELL_REMOVE_FEAR_2, NWScript.IP_CONST_CASTSPELL_SANCTUARY_2, NWScript.IP_CONST_CASTSPELL_SHIELD_OF_FAITH_5, NWScript.IP_CONST_CASTSPELL_CAMOFLAGE_5, NWScript.IP_CONST_CASTSPELL_ENTANGLE_5, NWScript.IP_CONST_CASTSPELL_MAGIC_FANG_5, 540, 541, 542, 543, 544 };
    public static Feat[] shopBasicMagicSkillBooks = new Feat[] { CustomFeatList.Enchanteur, CustomFeatList.Comptabilite, CustomFeatList.BrokerRelations, CustomFeatList.Negociateur, CustomFeatList.ContractScience, CustomFeatList.Marchand, CustomFeatList.Magnat };
    public static int GetCustomFeatLevelFromSkillPoints(Feat feat, int currentSkillPoints)
    {
      int multiplier = Feat2da.featTable.GetFeatDataEntry(feat).CRValue;
      var result = Math.Log(currentSkillPoints / (250 * multiplier)) / Math.Log(5);

      if (result > 4)
      {
        result = 4;
        result += (currentSkillPoints - (int)(250 * multiplier * Math.Pow(5, 4))) / (int)(250 * multiplier * Math.Pow(5, 4));
      }

      if (result < 0)
        return 0;
      else
        return 1 + (int)result;
    }
  }
}
