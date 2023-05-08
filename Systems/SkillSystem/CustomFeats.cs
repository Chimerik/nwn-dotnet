﻿namespace NWN.Systems
{
  public class CustomSkill
  {
    // Les compétences customs activables sous forme de dons sont numérotées à partir de 10000 afin d'éviter la collision avec les ID de sorts dans le dictionnary global des Learnables
    // Les compétences customs passives sont numérotées à partir de 20000 afin de laisser suffisament de marge pour pouvoir ajouter des actifs sans tout retoucher
    // Les inscriptions sont numérotées à partir de 50000 afin d'éviter les collions avec les CustomSkills

    public const int CustomMenuUP = 1132;
    public const int CustomMenuDOWN = 1133;
    public const int CustomMenuSELECT = 1134;
    public const int CustomMenuEXIT = 1135;
    
    public const int Miner = 118;
    public const int Geology = 1186;
    public const int Prospection = 1187;
    public const int VeldsparReprocessing = 1188;
    public const int ScorditeReprocessing = 1189;
    public const int PyroxeresReprocessing = 1190;
    public const int PlagioclaseReprocessing = 1191;
    public const int OmberReprocessing = 1192;
    public const int KerniteReprocessing = 1193;
    public const int GneissReprocessing = 1194;
    public const int JaspetReprocessing = 1195;
    public const int HemorphiteReprocessing = 1196;
    public const int HedbergiteReprocessing = 1197;
    public const int DarkOchreReprocessing = 1198;
    public const int CrokiteReprocessing = 1199;
    public const int BistotReprocessing = 1200;
    public const int BezdnacineReprocessing = 1201;
    public const int ArkonorReprocessing = 1202;
    public const int MercoxitReprocessing = 1203;
    //public const int StripMiner = 1204;
    
    public const int CustomPositionRight = 1279;
    public const int CustomPositionLeft = 1280;
    public const int CustomPositionRotateRight = 1281;
    public const int CustomPositionRotateLeft = 1282;
    public const int CustomPositionForward = 1283;
    public const int CustomPositionBackward = 1284;
    /*public const int WoodCutter = 1285;
    public const int WoodExpertise = 1286;
    public const int WoodProspection = 1287;
    public const int Skinning = 1288;
    public const int AnimalExpertise = 1289;
    public const int Hunting = 1290;
    
    public const int WoodReprocessing = 1292;
    public const int WoodReprocessingEfficiency = 1293;
    public const int LaurelinReprocessing = 1294;
    public const int TelperionReprocessing = 1295;
    public const int MallornReprocessing = 1296;
    public const int NimlothReprocessing = 1297;
    public const int OiolaireReprocessing = 1298;
    public const int QlipothReprocessing = 1299;
    public const int FerocheneReprocessing = 1300;
    public const int ValinorReprocessing = 1301;
    
    public const int PeltReprocessing = 1303;
    public const int PeltReprocessingEfficiency = 1304;
    public const int BadPeltReprocessing = 1305;
    public const int CommonPeltReprocessing = 1306;
    public const int NormalPeltReprocessing = 1307;
    public const int UncommunPeltReprocessing = 1308;
    public const int RarePeltReprocessing = 1309;
    public const int MagicPeltReprocessing = 1310;
    public const int EpicPeltReprocessing = 1311;
    public const int LegendaryPeltReprocessing = 1312;*/
    
    public const int ContractScience = 1314;
    public const int Marchand = 1315;
    public const int Magnat = 1316;
    public const int Negociateur = 1317;
    public const int BrokerRelations = 1318;
    public const int BrokerAffinity = 1319;
    public const int Comptabilite = 1320;
    
    public const int Sit = 1330;
    public const int MetalRepair = 1331;
    public const int WoodRepair = 1332;
    public const int LeatherRepair = 1333;
    public const int EnchantRepair = 1334;
    
    public const int AlchemistEfficiency = 1336;
    public const int AlchemistCareful = 1337;
    public const int AlchemistExpert = 1338;
    public const int Alchemist = 1339;
    public const int AlchemistAware = 1340;
    public const int AlchemistAccurate = 1341;

    public const int TwoWeaponFighting = 10041;
    public const int WeaponFinesse = 10042;

    public const int SeverArtery = 11136;

    public const int ImprovedStrength = 20000;
    public const int ImprovedDexterity = 20001;
    public const int ImprovedConstitution = 20002;
    public const int ImprovedIntelligence = 20003;
    public const int ImprovedWisdom = 20004;
    public const int ImprovedCharisma = 20005;
    public const int ImprovedHealth = 20006;
    public const int Toughness = 20007;
    public const int Acolyte = 20008;
    public const int Anthropologist = 20009;
    public const int Archeologist = 20010;
    public const int Athlete = 20011;
    public const int Magistrate = 20012;
    public const int AdventurerScion = 20013;
    public const int Charlatan = 20014;
    public const int CityWatch = 20015;
    public const int Investigator = 20016;
    public const int CloisteredScholar = 20017;
    public const int Courtier = 20018;
    public const int Criminal = 20019;
    public const int Entertainer = 20020;
    public const int Faceless = 20021;
    public const int FailedMerchant = 20022;
    public const int Fisher = 20023;
    public const int FolkHero = 20024;
    public const int Gambler = 20025;
    public const int HauntedOne = 20026;
    public const int Hermit = 20027;
    public const int Heir = 20028;
    public const int Marine = 20029;
    public const int KnightOfTheOrder = 20030;
    public const int Mercenary = 20031;
    public const int Noble = 20032;
    public const int Outlander = 20033;
    public const int SecretIdentity = 20034;
    public const int Refugee = 20035;
    public const int Sage = 20036;
    public const int Sailor = 20037;
    public const int Shipwright = 20038;
    public const int Smuggler = 20039;
    public const int Soldier = 20040;
    public const int StreetUrchin = 20041;
    public const int Prisoner = 20042;
    public const int Wanderer = 20043;
    public const int Taken = 20044;

    public const int Athletics = 20045; // remplace discipline // TODO : il n'est pas possible de renverser les créatures de deux catégories de taille au-dessus de soi + CD de 30 secondes
    public const int Acrobatics = 20046; // remplace discipline
    public const int Escamotage = 20047;
    public const int Stealth = 20048; // Regroupe à la fois discrétion et déplacement silencieux
    public const int Concentration = 20049;
    public const int Arcana = 20050;
    public const int History = 20051;
    public const int Nature = 20052;
    public const int Religion = 20053;
    public const int Investigation = 20054; // Remplace fouille (détection des pièges)
    public const int Dressage = 20055; // Permet de calmer et de gérer un animal. N'assure pas le contrôle. Différent de l'empathie animale des druides
    public const int Insight = 20056;
    public const int Medicine = 20057;
    public const int Perception = 20058; // Perception regroupe à la fois Détection, perception auditive
    public const int Survival = 20059;
    public const int Deception = 20060;
    public const int Intimidation = 20061;
    public const int Performance = 20062;
    public const int Persuasion = 20063;
    public const int OpenLock = 20064; // TODO : déverrouiller une porte nécessite des outils de voleur
    public const int TrapExpertise = 20065; // regroupe désamorçage et pose // TODO : désamorcer un piège nécessite des outils de voleur
    public const int Taunt = 20066; // TODO : sur un PNJ, taunt force la cible à attaquer le taunter. Sur un PJ, taunt diminue la CA et impose un malus d'échec des sorts
    // TODO : pas de compétence "parade" pour le moment. Il serait en revanche possible de réfléchir à un don qui permettrait de réduire les dégâts reçus en fonction de certaines conditions, cf Martial Adept de DD5 : Combat Maneuvers
    // TODO : pas d'empathie animale pour le moment. Mais réfléchir à un système pour jouer à Pokémon

    public const int Elfique = 20067;
    public const int Nain = 20068;
    public const int Orc = 20069;
    public const int Giant = 20070;
    public const int Gobelin = 20071;
    public const int Halfelin = 20072;
    public const int Abyssal = 20073;
    public const int Celestial = 20074;
    public const int Draconique = 20075;
    public const int Profond = 20076;
    public const int Infernal = 20077;
    public const int Primordiale = 20078;
    public const int Sylvain = 20079;
    public const int Druidique = 20080;
    public const int Voleur = 20081;
    public const int Gnome = 20082;

    public const int ImprovedAttackBonus = 20083;
    public const int ImprovedSpellSlot0 = 20084;
    public const int ImprovedSpellSlot1 = 20085;
    public const int ImprovedSpellSlot2 = 20086;
    public const int ImprovedSpellSlot3 = 20087;
    public const int ImprovedSpellSlot4 = 20088;
    public const int ImprovedSpellSlot5 = 20089;
    public const int ImprovedSpellSlot6 = 20090;
    public const int ImprovedSpellSlot7 = 20091;
    public const int ImprovedSpellSlot8 = 20092;
    public const int ImprovedSpellSlot9 = 20093;
    //public const int ImprovedCasterLevel = 20094; // Retiré, car désormais chaque sort dispose de son propre niveau. Pourra être remplacé par autre chose

    public const int ImprovedLightArmorProficiency = 20099;
    public const int ImprovedMediumArmorProficiency = 20100;
    public const int ImprovedHeavyArmorProficiency = 20101;
    public const int ImprovedFullPlateProficiency = 20102;
    public const int ImprovedLightShieldProficiency = 20103;
    public const int ImprovedMediumShieldProficiency = 20104;
    public const int ImprovedHeavyShieldProficiency = 20105;

    public const int ImprovedClubProficiency = 20106;
    public const int ImprovedShortSwordProficiency = 20107;
    public const int ImprovedLightFlailProficiency = 20108;
    public const int ImprovedShortBowProficiency = 20109;
    public const int ImprovedLightCrossBowProficiency = 20110;
    public const int ImprovedLightMaceProficiency = 20111;
    public const int ImprovedDaggerProficiency = 20112;
    public const int ImprovedDartProficiency = 20113;
    public const int ImprovedUnharmedProficiency = 20114;
    public const int ImprovedLightHammerProficiency = 20115;
    public const int ImprovedHandAxeProficiency = 20116;
    public const int ImprovedQuarterStaffProficiency = 20117;
    public const int ImprovedMagicStaffProficiency = 20118;
    public const int ImprovedMorningStarProficiency = 20119;
    public const int ImprovedShortSpearProficiency = 20120;
    public const int ImprovedSlingProficiency = 20121;
    public const int ImprovedSickleProficiency = 20122;

    public const int ImprovedLongSwordProficiency = 20123;
    public const int ImprovedBattleAxeProficiency = 20124;
    public const int ImprovedWarHammerProficiency = 20125;
    public const int ImprovedLongBowProficiency = 20126;
    public const int ImprovedHeavyCrossbowProficiency = 20127;
    public const int ImprovedHalberdProficiency = 20128;
    public const int ImprovedGreatSwordProficiency = 20129;
    public const int ImprovedGreatAxeProficiency = 20130;
    public const int ImprovedHeavyFlailProficiency = 20131;
    public const int ImprovedRapierProficiency = 20132;
    public const int ImprovedScimitarProficiency = 20133;
    public const int ImprovedThrowingAxeProficiency = 20134;
    public const int ImprovedTridentProficiency = 20135;

    public const int ImprovedBastardSwordProficiency = 20136;
    public const int ImprovedTwoBladedSwordProficiency = 20137;
    public const int ImprovedDireMaceProficiency = 20138;
    public const int ImprovedDoubleAxeProficiency = 20139;
    public const int ImprovedKamaProficiency = 20140;
    public const int ImprovedKukriProficiency = 20141;
    public const int ImprovedKatanaProficiency = 20142;
    public const int ImprovedScytheProficiency = 20143;
    public const int ImprovedDwarvenWarAxeProficiency = 20144;
    public const int ImprovedWhipProficiency = 20145;

    public const int ImprovedFortitude = 20146;
    public const int ImprovedReflex = 20147;
    public const int ImprovedWill = 20148;
    public const int ImprovedSavingThrowAll = 20149;

    public const int OreDetection = 20150;
    public const int WoodDetection = 20151;
    public const int PeltDetection = 20152;
    public const int OreDetectionSpeed = 20153;
    public const int WoodDetectionSpeed = 20154;
    public const int PeltDetectionSpeed = 20155;
    public const int OreDetectionRange = 20156;
    public const int WoodDetectionRange = 20157;
    public const int PeltDetectionRange = 20158;
    public const int OreDetectionSafe = 20159;
    public const int WoodDetectionSafe = 20160;
    public const int PeltDetectionSafe = 20161;
    public const int OreDetectionEstimation = 20162;
    public const int WoodDetectionEstimation = 20163;
    public const int PeltDetectionEstimation = 20164;
    public const int OreDetectionAdvanced = 20165;
    public const int WoodDetectionAdvanced = 20166;
    public const int PeltDetectionAdvanced = 20167;
    public const int OreDetectionMastery = 20168;
    public const int WoodDetectionMastery = 20169;
    public const int PeltDetectionMastery = 20170;

    public const int OreExtraction = 20171;
    public const int OreExtractionSpeed = 20172;
    public const int OreExtractionYield = 20173;
    public const int OreExtractionAdvanced = 20174;
    public const int OreExtractionMastery = 20175;

    public const int WoodExtraction = 20176;
    public const int WoodExtractionSpeed = 20177;
    public const int WoodExtractionYield = 20178;
    public const int WoodExtractionAdvanced = 20179;
    public const int WoodExtractionMastery = 20180;

    public const int PeltExtraction = 20181;
    public const int PeltExtractionSpeed = 20182;
    public const int PeltExtractionYield = 20183;
    public const int PeltExtractionAdvanced = 20184;
    public const int PeltExtractionMastery = 20185;

    public const int ReprocessingOre = 20186;
    public const int ReprocessingOreEfficiency = 20187;
    public const int ReprocessingWood = 20188;
    public const int ReprocessingWoodEfficiency = 20189;
    public const int ReprocessingPelt = 20190;
    public const int ReprocessingPeltEfficiency = 20191;
    public const int ReprocessingGrade1Expertise = 20192;
    public const int ReprocessingGrade2Expertise = 20193;
    public const int ReprocessingGrade3Expertise = 20194;
    public const int ReprocessingGrade4Expertise = 20195;
    public const int ReprocessingGrade5Expertise = 20196;
    public const int ReprocessingGrade6Expertise = 20197;
    public const int ReprocessingGrade7Expertise = 20198;
    public const int ReprocessingGrade8Expertise = 20199;

    public const int ConnectionsPromenade = 20200;
    public const int ConnectionsGates = 20201;
    public const int ConnectionsGovernment = 20202;
    public const int ConnectionsTemple = 20203;

    public const int ClubScience = 20204;
    public const int ShortSwordScience = 20205;
    public const int LightFlailScience = 20206;
    public const int ShortBowScience = 20207;
    public const int LightCrossBowScience = 20208;
    public const int LightMaceScience = 20209;
    public const int DaggerScience = 20210;
    public const int DartScience = 20211;
    public const int UnharmedScience = 20212;
    public const int LightHammerScience = 20213;
    public const int HandAxeScience = 20214;
    public const int QuarterStaffScience = 20215;
    public const int MagicStaffScience = 20216;
    public const int MorningStarScience = 20217;
    public const int ShortSpearScience = 20218;
    public const int SlingScience = 20219;
    public const int SickleScience = 20220;

    public const int LongSwordScience = 20221;
    public const int BattleAxeScience = 20222;
    public const int WarHammerScience = 20223;
    public const int LongBowScience = 20224;
    public const int HeavyCrossbowScience = 20225;
    public const int HalberdScience = 20226;
    public const int GreatSwordScience = 20227;
    public const int GreatAxeScience = 20228;
    public const int HeavyFlailScience = 20229;
    public const int RapierScience = 20230;
    public const int ScimitarScience = 20231;
    public const int ThrowingAxeScience = 20232;
    public const int TridentScience = 20233;

    public const int BastardSwordScience = 20234;
    public const int TwoBladedSwordScience = 20235;
    public const int DireMaceScience = 20236;
    public const int DoubleAxeScience = 20237;
    public const int KamaScience = 20238;
    public const int KukriScience = 20239;
    public const int KatanaScience = 20240;
    public const int ScytheScience = 20241;
    public const int DwarvenWarAxeScience = 20242;
    public const int WhipScience = 20243;

    public const int ImprovedShurikenProficiency = 20244;
    public const int ShurikenScience = 20245;

    public const int ImprovedDualWieldDefenseProficiency = 20246;

    public const int MateriaGradeConcentration = 20247;
    public const int ReprocessingOreExpertise = 20248;
    public const int ReprocessingWoodExpertise = 20249;
    public const int ReprocessingPeltExpertise = 20250;
    public const int ReprocessingOreLuck = 20251;
    public const int ReprocessingWoodLuck = 20252;
    public const int ReprocessingPeltLuck = 20253;

    public const int BlueprintCopy = 20254;
    public const int BlueprintEfficiency = 20255;
    public const int BlueprintEconomy = 20256;
    public const int BlueprintResearch = 20257;
    public const int BlueprintMetallurgy = 20258;
    public const int AdvancedCraft = 20259;

    public const int CraftClothing = 20260;
    public const int CraftFullPlate = 20261;
    public const int CraftHalfPlate = 20262;
    public const int CraftSplintMail = 20263;
    public const int CraftBreastPlate = 20264;
    public const int CraftScaleMail = 20265;
    public const int CraftStuddedLeather = 20266;
    public const int CraftLeatherArmor = 20267;
    public const int CraftPaddedArmor = 20268;
    public const int CraftShortsword = 20269;
    public const int CraftLongsword = 20270;
    public const int CraftBattleAxe = 20271;
    public const int CraftBastardSword = 20272;
    public const int CraftLightFlail = 20273;
    public const int CraftWarHammer = 20274;
    public const int CraftHeavyCrossbow = 20275;
    public const int CraftLightCrossbow = 20276;
    public const int CraftLongBow = 20277;
    public const int CraftLightMace = 20278;
    public const int CraftHalberd = 20279;
    public const int CraftShortBow = 20280;
    public const int CraftTwoBladedSword = 20281;
    public const int CraftGreatSword = 20282;
    public const int CraftSmallShield = 20283;
    public const int CraftTorch = 20284;
    public const int CraftHelmet = 20285;
    public const int CraftGreatAxe = 20286;
    public const int CraftAmulet = 20287;
    public const int CraftArrow = 20288;
    public const int CraftBelt = 20289;
    public const int CraftDagger = 20290;
    public const int CraftBolt = 20291;
    public const int CraftBoots = 20292;
    public const int CraftBullets = 20293;
    public const int CraftClub = 20294;
    public const int CraftDarts = 20295;
    public const int CraftDireMace = 20296;
    public const int CraftHeavyFlail = 20297;
    public const int CraftGloves = 20298;
    public const int CraftLightHammer = 20299;
    public const int CraftHandAxe = 20300;
    public const int CraftKama = 20301;
    public const int CraftKukri = 20302;
    public const int CraftMagicRod = 20303;
    public const int CraftStaff = 20304;
    public const int CraftMagicWand = 20305;
    public const int CraftMorningStar = 20306;
    public const int CraftPotion = 20307;
    public const int CraftQuarterstaff = 20308;
    public const int CraftRapier = 20309;
    public const int CraftRing = 20310;
    public const int CraftScimitar = 20311;
    public const int CraftScythe = 20312;
    public const int CraftLargeShield = 20313;
    public const int CraftTowerShield = 20314;
    public const int CraftShortSpear = 20315;
    public const int CraftShuriken = 20316;
    public const int CraftSickle = 20317;
    public const int CraftSling = 20318;
    public const int CraftThrowingAxe = 20319;
    public const int CraftSpellScroll = 20320;
    public const int CraftBracer = 20321;
    public const int CraftCloak = 20322;
    public const int CraftTrident = 20323;
    public const int CraftDwarvenWarAxe = 20324;
    public const int CraftWhip = 20325;
    public const int CraftDoubleAxe = 20326;
    public const int CraftKatana = 20327;

    public const int Blacksmith = 20328;
    public const int Woodworker = 20329;
    public const int Tanner = 20330;
    public const int ArtisanExceptionnel = 20331;
    public const int ArtisanApplique = 20332;

    public const int Renforcement = 20333;
    public const int Recycler = 20334;
    public const int RecyclerFast = 20335;
    public const int RecyclerExpert = 20336;

    public const int CalligraphieSurcharge = 20337;
    public const int CalligraphieSurchargeControlee = 20338;

    public const int Repair = 20339;
    public const int RepairFast = 20340;
    public const int RepairExpert = 20341;
    public const int RepairCareful = 20342;

    public const int CalligrapheArmurier = 20343;
    public const int CalligrapheArmurierMaitre = 20344;
    public const int CalligrapheArmurierScience = 20345;

    public const int CombattantPrecautionneux = 20346;
    public const int ImprovedDodge = 20347;

    public const int CraftOnHandedMeleeWeapon = 20348;
    public const int CraftTwoHandedMeleeWeapon = 20349;
    public const int CraftRangedWeapon = 20350;
    public const int CraftShield = 20351;
    public const int CraftArmor = 20352;
    public const int CraftClothes = 20353;
    public const int CraftAmmunitions = 20354;

    public const int ArtisanFocus = 20355;

    public const int OreExtractionSafe = 20356;
    public const int WoodExtractionSafe = 20357;
    public const int PeltExtractionSafe = 20358;

    public const int MateriaScanning = 20359;
    public const int OreDetectionAccuracy = 20360;
    public const int WoodDetectionAccuracy = 20361;
    public const int PeltDetectionAccuracy = 20362;

    public const int MateriaExtraction = 20363;
    public const int OreExtractionDurable = 20364;
    public const int WoodExtractionDurable = 20365;
    public const int PeltExtractionDurable = 20366;

    public const int ArtisanPrudent = 20367;

    public const int UncannyDodge = 20368;

    public const int LightArmorProficiency = 20369;
    public const int MediumArmorProficiency = 20370;
    public const int HeavyArmorProficiency = 20371;
    public const int FullPlateProficiency = 20372;

    public const int LightShieldProficiency = 20373;
    public const int MediumShieldProficiency = 20374;
    public const int HeavyShieldProficiency = 20375;
    public const int DualWieldDefenseProficiency = 20376;

    public const int ClothingArmorProficiency = 20377;
    public const int ImprovedClothingArmorProficiency = 20378;

    public const int ClubProficiency = 20379;
    public const int ShortSwordProficiency = 20380;
    public const int LightFlailProficiency = 20381;
    public const int ShortBowProficiency = 20382;
    public const int LightCrossBowProficiency = 20383;
    public const int LightMaceProficiency = 20384;
    public const int DaggerProficiency = 20385;
    public const int DartProficiency = 20386;
    public const int UnharmedProficiency = 20387;
    public const int LightHammerProficiency = 20388;
    public const int HandAxeProficiency = 20389;
    public const int QuarterStaffProficiency = 20390;
    public const int MagicStaffProficiency = 20391;
    public const int MorningStarProficiency = 20392;
    public const int ShortSpearProficiency = 20393;
    public const int SlingProficiency = 20394;
    public const int SickleProficiency = 20395;

    public const int LongSwordProficiency = 20396;
    public const int BattleAxeProficiency = 20397;
    public const int WarHammerProficiency = 20398;
    public const int LongBowProficiency = 20399;
    public const int HeavyCrossbowProficiency = 20400;
    public const int HalberdProficiency = 20401;
    public const int GreatSwordProficiency = 20402;
    public const int GreatAxeProficiency = 20403;
    public const int HeavyFlailProficiency = 20404;
    public const int RapierProficiency = 20405;
    public const int ScimitarProficiency = 20406;
    public const int ThrowingAxeProficiency = 20407;
    public const int TridentProficiency = 20408;

    public const int BastardSwordProficiency = 20409;
    public const int TwoBladedSwordProficiency = 20410;
    public const int DireMaceProficiency = 20411;
    public const int DoubleAxeProficiency = 20412;
    public const int KamaProficiency = 20413;
    public const int KukriProficiency = 20414;
    public const int KatanaProficiency = 20415;
    public const int ScytheProficiency = 20416;
    public const int DwarvenWarAxeProficiency = 20417;
    public const int WhipProficiency = 20418;
    public const int ShurikenProficiency = 20419;

    public const int CalligrapheArmurierExpert = 20420;
    public const int CalligrapheBlindeur = 20421;
    public const int CalligrapheBlindeurMaitre = 20422;
    public const int CalligrapheBlindeurScience = 20423;
    public const int CalligrapheBlindeurExpert = 20424;
    public const int CalligrapheCiseleur = 20425;
    public const int CalligrapheCiseleurMaitre = 20426;
    public const int CalligrapheCiseleurScience = 20427;
    public const int CalligrapheCiseleurExpert = 20428;
    public const int CalligrapheFourbisseur = 20429;
    public const int CalligrapheFourbisseurMaitre = 20430;
    public const int CalligrapheFourbisseurScience = 20431;
    public const int CalligrapheFourbisseurExpert = 20432;

    public const int AirMagic = 20433;
    public const int AirMagicMaster = 20434;
    public const int AirMagicScience = 20435;
    public const int AirMagicExpert = 20436;

    public const int FireMagic = 20437;
    public const int FireMagicMaster = 20438;
    public const int FireMagicScience = 20439;
    public const int FireMagicExpert = 20440;

    public const int EarthMagic = 20441;
    public const int EarthMagicMaster = 20442;
    public const int EarthMagicScience = 20443;
    public const int EarthMagicExpert = 20444;

    public const int WaterMagic = 20445;
    public const int WaterMagicMaster = 20446;
    public const int WaterMagicScience = 20447;
    public const int WaterMagicExpert = 20448;

    public const int Invalid = 65535;
  }
}
