namespace NWN.Systems
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
 
    public const int CustomPositionRight = 1279;
    public const int CustomPositionLeft = 1280;
    public const int CustomPositionRotateRight = 1281;
    public const int CustomPositionRotateLeft = 1282;
    public const int CustomPositionForward = 1283;
    public const int CustomPositionBackward = 1284;
    
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

    public const int Sprint = 1342;
    public const int Disengage = 1343;
    public const int FighterSecondWind = 1344;
    public const int FighterSurge = 1345;
    public const int RayOfFrost = 1346;
    public const int AcidSplash = 1347;
    public const int ElectricJolt = 1348;
    public const int BladeWard = 1349;
    public const int FireBolt = 1350;
    public const int Friends = 1351;
    public const int BoneChill = 1352;
    public const int TrueStrike = 1353;
    public const int PoisonSpray = 1354;
    public const int Light = 1355;
    public const int LightDrow = 1356;
    public const int FaerieFireDrow = 1357;
    public const int DarknessDrow = 1358;
    public const int InvisibilityDuergar = 1359;
    public const int EnlargeDuergar = 1360;
    public const int SpeakAnimalGnome = 1361;
    public const int ProduceFlame = 1362;
    public const int HellishRebuke = 1363;
    public const int MageHand = 1364;
    public const int BurningHands = 1365;
    public const int FlameBlade = 1366;
    public const int Thaumaturgy = 1367;
    public const int SearingSmite = 1368;
    public const int BrandingSmite = 1369;
    public const int Dodge = 1370;
    public const int CogneurLourd = 1371;
    public const int MaitreArmureLourde = 1372;
    public const int MaitreBouclier = 1373;
    public const int TueurDeMage = 1374;
    public const int Mobile = 1375;
    public const int AgresseurSauvage = 1376;
    public const int HastMaster = 1377;
    public const int MaitreArbaletrier = 1378;
    public const int Sentinelle = 1379;
    public const int TireurDelite = 1380;
    public const int BagarreurDeTaverne = 1381;
    public const int MageDeGuerre = 1382;
    public const int Broyeur = 1383;
    public const int FureurOrc = 1384;
    public const int Pourfendeur = 1385;
    public const int Empaleur = 1386;
    public const int FighterCombatStyleTwoHanded = 1387;
    public const int Elementaliste = 1388;
    public const int LameDoutretombe = 1389;
    public const int FlammesDePhlegetos = 1390;
    public const int MeneurExaltant = 1391;
    public const int AgressionOrc = 1392;
    public const int Determination = 1393;
    public const int PrecisionElfique = 1394;
    public const int SecondeChance = 1395;
    public const int Chanceux = 1396;
    public const int ArcaneArcherPrestidigitation = 1397;
    public const int ArcaneArcherDruidisme = 1398;
    public const int Prestidigitation = 1399;
    public const int Druidisme = 1400;

    public const int ArcaneArcherTirAffaiblissant = 1401;
    public const int ArcaneArcherTirAgrippant = 1402;
    public const int ArcaneArcherTirBannissement = 1403;
    public const int ArcaneArcherTirChercheur = 1404;
    public const int ArcaneArcherTirOmbres = 1405;
    public const int ArcaneArcherTirEnvoutant = 1406;
    public const int ArcaneArcherTirExplosif = 1407;
    public const int ArcaneArcherTirPerforant = 1408;
    public const int ArcaneArcherTirIncurve = 1409;

    public const int WarMasterAttaqueMenacante = 1410;
    public const int WarMasterAttaquePrecise = 1411;
    public const int WarMasterBalayage = 1412;
    public const int WarMasterRenversement = 1413;
    public const int WarMasterDesarmement = 1414;
    public const int WarMasterDiversion = 1415;
    public const int WarMasterFeinte = 1416;
    public const int WarMasterInstruction = 1417;
    public const int WarMasterJeuDeJambe = 1418;
    public const int WarMasterManoeuvreTactique = 1419;
    public const int WarMasterParade = 1420;
    public const int WarMasterProvocation = 1421;
    public const int WarMasterRalliement = 1422;
    public const int WarMasterRiposte = 1423;
    public const int WarMasterEvaluationTactique = 1424;
    public const int WarMasterObservation = 1425;
    public const int FighterChampionImprovedCritical = 1426;

    public const int FighterCombatStyleArchery = 1427;
    public const int FighterCombatStyleDuel = 1428;

    public const int BarbarianRecklessAttack = 1429;
    public const int BersekerFrenziedStrike = 1430;
    public const int BersekerRageAveugle = 1431;
    public const int BersekerRepresailles = 1432;
    public const int BersekerPresenceIntimidante = 1433;

    public const int TotemSpeakAnimal = 1434;
    public const int TotemSensAnimal = 1435;
    public const int TotemEspritOurs = 1436;
    public const int TotemFerociteIndomptable = 1437;
    public const int TotemEspritAigle = 1438;
    public const int TotemEspritElan = 1439;
    public const int TotemEspritLoup = 1440;
    public const int TotemHurlementGalvanisant = 1441;

    public const int Chargeur = 1442;

    public const int TotemAspectOurs = 1443;
    public const int TotemAspectAigle = 1444;
    public const int TotemAspectLoup = 1445;
    public const int TotemAspectElan = 1446;
    public const int TotemAspectCrocodile = 1447;
    public const int TotemAspectRatel = 1448;
    public const int TotemAspectEtalon = 1449;
    public const int TotemAspectTigre = 1450;
    public const int TotemAspectGlouton = 1451;

    public const int TotemCommunionAvecLaNature = 1452;

    public const int TotemLienLoup = 1453;
    public const int TotemLienElan = 1454;  
    public const int TotemLienTigre = 1455;
    public const int TotemLienOurs = 1456;

    public const int WildMagicSense = 1457;
    public const int WildMagicTeleportation = 1458;
    public const int WildMagicMagieGalvanisanteBienfait = 1459;
    public const int WildMagicMagieGalvanisanteRecuperation = 1460;

    public const int Stealth = 1461;

    public const int MainLeste = 1462;
    public const int DiscretionSupreme = 1463;
    public const int ThiefInvisibility = 1464;
    public const int ConspirateurMaitriseTactique = 1465;
    public const int ConspirateurRedirection = 1466;

    public const int AssassinAssassinate = 1467;

    public const int MonkBonusAttack = 1468;
    public const int MonkPatience = 1469;
    public const int MonkDelugeDeCoups = 1470;
    public const int MonkSlowFall = 1471;
    public const int MonkStunStrike = 1472;
    public const int MonkSerenity = 1473;
    public const int MonkDiamondSoul = 1474;
    public const int MonkDesertion = 1475;

    public const int MonkPlenitude = 1477;
    public const int MonkManifestationAme = 1478;
    public const int MonkManifestationCorps = 1479;
    public const int MonkManifestationEsprit = 1480;
    public const int MonkResonanceKi = 1481;
    public const int MonkExplosionKi = 1482;
    public const int MonkPaumeVibratoire = 1483;

    public const int MonkTenebres = 1484;
    public const int MonkDarkVision = 1485;
    public const int MonkPassageSansTrace = 1486;
    public const int MonkSilence = 1487;
    public const int MonkLinceulDombre = 1488;
    public const int MonkFouleeDombre = 1489;
    public const int MonkFrappeDombre = 1490;
    public const int MonkOpportuniste = 1491;

    public const int MonkHarmony = 1492;

    public const int WizardRestaurationArcanique = 1493;
    public const int AbjurationWard = 1494;
    public const int AbjurationWardProjetee = 1495;
    public const int AbjurationImproved = 1496;
    public const int AbjurationSpellResistance = 1497;

    public const int DivinationPresage = 1498;
    public const int DivinationPresage2 = 1499;
    public const int DivinationPresageSuperieur = 1500;
    public const int DivinationExpert = 1501;
    public const int DivinationDarkVision = 1502;
    public const int DivinationSeeInvisibility = 1503;
    public const int DivinationSeeEthereal = 1504;

    public const int EnchantementCharmeInstinctif = 1505;
    public const int EnchantementRegardHypnotique = 1506;
    public const int EnchantementPartage = 1507;

    public const int EvocateurFaconneurDeSorts = 1508;
    public const int EvocateurToursPuissants = 1509;
    public const int EvocateurSuperieur = 1510;
    public const int EvocateurSurcharge = 1511;

    public const int FighterChampionRemarkableAthlete = 1512;

    public const int IllusionMineure = 1513;
    public const int IllusionVoirLinvisible = 1514;
    public const int IllusionDouble = 1515;

    public const int InvocationMineure = 1516;
    public const int InvocationPermutation = 1517;
    public const int InvocationConcentration = 1518;
    public const int InvocationSupreme = 1519;

    public const int NecromancieMoissonDuFiel = 1520;
    public const int NecromancieUndeadThralls = 1521;
    public const int NecromancieInsensible = 1522;
    public const int NecromancieUndeadControl = 1523;

    public const int TransmutationAlchimieMineure = 1524;

    /*public const int AlchemistEfficiency = 1336;
    public const int AlchemistCareful = 1337;
    public const int AlchemistExpert = 1338;
    public const int Alchemist = 1339;
    public const int AlchemistAware = 1340;
    public const int AlchemistAccurate = 1341;*/

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

    public const int StrengthSavesProficiency = 20045;
    public const int DexteritySavesProficiency = 20046;
    public const int ConstitutionSavesProficiency = 20047;
    public const int IntelligenceSavesProficiency = 20048;
    public const int WisdomSavesProficiency = 20049;
    public const int CharismaSavesProficiency = 20050;
    
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
    public const int ShieldProficiency = 20372;
    public const int SimpleWeaponProficiency = 20373;
    public const int MartialWeaponProficiency = 20374;
    public const int ExoticWeaponProficiency = 20375;

    public const int SpearProficiency = 20376;
    public const int LongSwordProficiency = 20377;
    public const int ShortSwordProficiency = 20378;
    public const int LongBowProficiency = 20379;
    public const int ShortBowProficiency = 20380;
    public const int RapierProficiency = 20381;
    public const int LightHammerProficiency = 20382;
    public const int WarHammerProficiency = 20383;
    public const int HandAxeProficiency = 20384;
    public const int WarAxeProficiency = 20385;
    public const int DwarvenAxeProficiency = 20386;
    public const int ShurikenProficiency = 20387;
    public const int DoubleBladeProficiency = 20388;
    public const int ClubProficiency = 20389;
    public const int DaggerProficiency = 20390;
    public const int DoubleAxeProficiency = 20391;
    public const int QuarterstaffProficiency = 20392;
    public const int LightMaceProficiency = 20393;
    public const int SickleProficiency = 20394;
    public const int LightCrossbowProficiency = 20395;
    public const int DartProficiency = 20396;
    public const int MagicStaffProficiency = 20397;
    public const int LightFlailProficiency = 20398;
    public const int MorningstarProficiency = 20399;
    public const int SlingProficiency = 20400;
    public const int BattleaxeProficiency = 20401;
    public const int GreataxeProficiency = 20402;
    public const int GreatswordProficiency = 20403;
    public const int ScimitarProficiency = 20404;
    public const int HalberdProficiency = 20405;
    public const int HeavyFlailProficiency = 20406;
    public const int ThrowingAxeProficiency = 20407;
    public const int TridentProficiency = 20408;
    public const int WhipProficiency = 20409;
    public const int HeavyCrossbowProficiency = 20410;
    public const int BastardswordProficiency = 20411;
    public const int ScytheProficiency = 20412;
    public const int DireMaceProficiency = 20413;
    public const int DwarvenWaraxeProficiency = 20414;
    public const int KamaProficiency = 20415;
    public const int KatanaProficiency = 20416;
    public const int KukriProficiency = 20417;
  
    public const int HumanVersatility = 20418;
    public const int HighElfLanguage = 20419;

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

    public const int AcrobaticsProficiency = 20433;
    public const int AcrobaticsExpertise = 20434;

    public const int AnimalHandlingProficiency = 20435;
    public const int AnimalHandlingExpertise = 20436;

    public const int ArcanaProficiency = 20437;
    public const int ArcanaExpertise = 20438;

    public const int AthleticsProficiency = 20439;
    public const int AthleticsExpertise = 20440;

    public const int DeceptionProficiency = 20441;
    public const int DeceptionExpertise = 20442;

    public const int HistoryProficiency = 20443;
    public const int HistoryExpertise = 20444;

    public const int InsightProficiency = 20445;
    public const int InsightExpertise = 20446;

    public const int IntimidationProficiency = 20447;
    public const int IntimidationExpertise = 20448;

    public const int InvestigationProficiency = 20449;
    public const int InvestigationExpertise = 20450;

    public const int MedicineProficiency = 20451;
    public const int MedicineExpertise = 20452;

    public const int NatureProficiency = 20453;
    public const int NatureExpertise = 20454;

    public const int PerceptionProficiency = 20455;
    public const int PerceptionExpertise = 20456;

    public const int PerformanceProficiency = 20457;
    public const int PerformanceExpertise = 20458;

    public const int PersuasionProficiency = 20459;
    public const int PersuasionExpertise = 20460;

    public const int ReligionProficiency = 20461;
    public const int ReligionExpertise = 20462;

    public const int SleightOfHandProficiency = 20463;
    public const int SleightOfHandExpertise = 20464;

    public const int StealthProficiency = 20465;
    public const int StealthExpertise = 20466;

    public const int SurvivalProficiency = 20467;
    public const int SurvivalExpertise = 20468;

    public const int Fighter = 20469;
    public const int FighterChampion = 20470;
    public const int FighterWarMaster = 20471;
    public const int FighterEldritchKnight = 20472;
    public const int FighterArcaneArcher = 20473;

    public const int FighterCombatStyleDefense = 20476;
    public const int FighterCombatStyleProtection = 20479;
    public const int FighterCombatStyleDualWield = 20480;
    public const int FighterBonusAttack = 20482; // 3 niveaux
    public const int FighterInflexible = 20483;
    
    /*public const int FighterEldritchKnightBoundWeapon = 20486;
    public const int FighterEldritchKnightWarMagic = 20487; // 2 niveaux
    public const int FighterEldritchKnightEldritchStrike = 20488;
    public const int FighterEldritchKnightArcaneRush = 20489;*/
  
    public const int AbilityImprovement = 20486;
    public const int Actor = 20487;
    public const int Vigilant = 20488;
    public const int Sportif = 20489;
    
    public const int DuellisteDefensif = 20492;
    public const int AmbiMaster = 20493;
    public const int DungeonExpert = 20494;
    public const int Gaillard = 20495;
    public const int ProtectionLegere = 20496;
    public const int ProtectionIntermediaire = 20497;
    public const int ProtectionLourde = 20498;
    public const int MaitreArmureIntermediaire = 20499;
    public const int Resilient = 20500;
    public const int AgiliteDesCourtsSurPattes = 20501;
    public const int Robuste = 20502;
    public const int ConstitutionInfernale = 20503;
    public const int EspritVif = 20504;
    public const int Observateur = 20505;
    public const int MartialInitiate = 20506;
    public const int CreateurMerveilleux = 20507;
    public const int Prodige = 20508;    
    public const int AmiDeToutLeMonde = 20509;    
    public const int Expert = 20510;    
    public const int Linguiste = 20511;    
    public const int Doue = 20512;    
    public const int MaitreDarme = 20513;
    public const int ChanceDebordante = 20514;
    public const int VigueurNaine = 20515;    
    public const int Healer = 20516;

    public const int FighterChampionBonusCombatStyle = 20517;
    public const int FighterChampionUltimeSurvivant = 20519; // 2 niveaux

    public const int Barbarian = 20520;
    public const int BarbarianUnarmoredDefence = 20521;
    public const int BarbarianBonusAttack = 20522;
    public const int BarbarianInstinctSauvage = 20523;
    public const int BarbarianCritiqueBrutal = 20524;
    public const int BarbarianRageImplacable = 20525;
    public const int BarbarianRagePersistante = 20526;
    public const int BarbarianPuissanceIndomptable = 20527;
    public const int BarbarianChampionPrimitif = 20528;
    public const int BarbarianFastMovement = 20529;
    public const int BarbarianRage = 20530;

    public const int BarbarianBerseker = 20531;
    public const int BarbarianTotem = 20532;
    public const int BarbarianWildMagic = 20533;

    public const int Rogue = 20534;
    public const int RogueThief = 20535;
    public const int RogueAssassin = 20536;
    public const int RogueConspirateur = 20537;
    public const int RogueArcane = 20538;

    public const int Monk = 20539;
    public const int MonkUnarmoredDefence = 20540;
    public const int MonkUnarmoredSpeed = 20541;

    public const int MonkPaume = 20542;
    public const int MonkPaumeTechnique = 20543;

    public const int MonkOmbre = 20544;

    public const int MonkElements = 20545;

    public const int Wizard = 20546;
    public const int WizardAbjuration = 20547;
    public const int WizardDivination = 20548;
    public const int WizardEnchantement = 20549;
    public const int EnchantementAlterationMemorielle = 20551;
    public const int WizardEvocation = 20552;
    public const int WizardIllusion = 20553;
    public const int WizardIllusionMalleable = 20554;
    public const int WizardRealiteIllusoire = 20555;
    public const int WizardInvocation = 20556;
    public const int WizardNecromancie = 20557;
    public const int WizardTransmutation = 20558;

    public const int Invalid = 65535;
  }
}
