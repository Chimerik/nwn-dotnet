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
    public const int FighterAvantageTactique = 1799;
    public const int FighterAttaquesEtudiees = 1800;
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
    public const int PourfendeurDeMages = 1374;
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
    public const int WarMasterConnaisTonEnnemi = 1425;
    public const int FighterChampionImprovedCritical = 1426;

    public const int FighterCombatStyleArchery = 1427;
    public const int FighterCombatStyleDuel = 1428;

    public const int BarbarianUnarmoredDefence = 1450;
    public const int BarbarianRecklessAttack = 1429;
    public const int BersekerFrenziedStrike = 1430;
    public const int BersekerRageAveugle = 1431;
    public const int BersekerRepresailles = 1432;
    public const int BersekerPresenceIntimidante = 1433;

    public const int TotemSpeakAnimal = 1434;
    public const int TotemSensAnimal = 1435;
    public const int TotemRage = 1436;
    public const int TotemAspectSauvage = 1437;
    public const int TotemPuissanceSauvage = 1438;

    public const int Chargeur = 1442;
    
    public const int TotemCommunionAvecLaNature = 1452;

    public const int WildMagicSense = 1457;
    public const int WildMagicTeleportation = 1458;
    public const int WildMagicMagieGalvanisanteBienfait = 1459;
    public const int WildMagicMagieGalvanisanteRecuperation = 1460;

    public const int Stealth = 1461;

    public const int RoublardViseeStable = 1439;
    public const int RoublardFrappeRusee = 1440;
    public const int RoublardSavoirFaire = 1441;
    public const int RoublardFrappeRuseeAmelioree = 1443;
    public const int RoublardCoupDeChance = 1444;
    public const int RoublardFrappePerfide = 1445;
    public const int AssassinInfiltrationExpert = 1446;
    public const int AssassinEnvenimer = 1447;
    public const int FightingStyleCombatAveugle = 1448;
    public const int FightingStyleUnarmed = 1449;
    public const int MainLeste = 1462;
    public const int ThiefDiscretionSupreme = 1463;
    public const int ConspirateurMaitriseTactique = 1465;
    public const int ConspirateurRedirection = 1466;

    public const int AssassinAssassinate = 1467;

    public const int MonkUnarmoredDefence = 1451;
    public const int MonkBonusAttack = 1468;
    public const int MonkPatience = 1469;
    public const int MonkDelugeDeCoups = 1470;
    public const int MonkParade = 1802;
    public const int MonkFrappesRenforcees = 1803;
    public const int MonkSlowFall = 1471;
    public const int MonkStunStrike = 1472;
    public const int MonkSerenity = 1473;
    public const int MonkDiamondSoul = 1474;
    public const int MonkDesertion = 1475;
    public const int MonkPaumeTechnique = 1476;

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

    public const int MonkMetabolismeSurnaturel = 1492;

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

    public const int FighterChampionAthleteAccompli = 1512;
    public const int ChampionGuerrierHeroique = 1801;

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
    public const int TransmutationStone = 1525;
    public const int TemporaryConstitutionSaveProficiency = 1526;
    public const int TransmutationMetamorphose = 1527;
    public const int TransmutationMaitre = 1528;

    public const int VigueurNaine = 1529;

    public const int ThiefReflex = 1530;
    public const int ArcaneTricksterMagicalAmbush = 1531;
    public const int ArcaneTricksterPolyvalent = 1532;

    public const int EldritchKnightChargeArcanique = 1533;
    public const int EldritchKnightArmeLiee = 1534;
    public const int EldritchKnightArmeLiee2 = 1535;
    public const int EldritchKnightArmeLieeInvocation = 1536;
    public const int EldritchKnightArmeLieeInvocation2 = 1537;

    public const int ConstitutionInfernale = 1538;

    public const int BardInspiration = 1539;
    public const int SecretsMagiques = 1540;
    public const int ToucheATout = 1541;
    public const int SourceDinspiration = 1542;
    public const int ContreCharme = 1543;

    public const int MotsCinglants = 1544;
    public const int BardeSavoirCompetenceSansEgale = 1798;

    public const int DefenseVaillante = 1545;
    public const int DegatsVaillants = 1546;
    public const int BotteDefensive = 1547;
    public const int BotteTranchante = 1548;
    public const int BotteDefensiveDeMaitre = 1549;
    public const int BotteTranchanteDeMaitre = 1550;

    public const int RangerEnnemiJuré = 1551;
    public const int RangerExplorationHabile = 1552;
    public const int RangerGuerrierDruidique = 1553;

    public const int SacredFlame = 1554;

    public const int RangerVagabondage = 1555;
    public const int RangerInfatiguable = 1556;
    public const int RangerImplacable = 1557;
    public const int RangerPrecis = 1558;
    public const int RangerVoileNaturel = 1559;
    public const int RangerSensSauvages = 1560;
    public const int RangerPourfendeur = 1561;

    public const int ChasseurMythes = 1562;
    public const int ChasseurProie = 1563;
    public const int ChasseurTactiquesDefensives = 1564;
    public const int ChasseurVolee = 1565;
    public const int ChasseurDefenseSuperieure = 1566;

    public const int EsquiveInstinctive = 1567;
    public const int FighterCombatStyleDefense = 1568;

    public const int ProfondeursFrappeRedoutable = 1569;
    public const int TraqueurRedoutable = 1570;
    public const int TraqueurRafale = 1571;
    public const int TraqueurEsquive = 1572;

    public const int BelluaireCompagnonAnimal = 1573;
    public const int BelluaireAttaqueCoordonnee = 1574;
    public const int BelluaireDefenseDeLaBeteSuperieure = 1575;
    public const int BelluaireFurieBestiale = 1576;
    public const int BelluaireEntrainementExceptionnel = 1577;
    public const int BelluaireRugissementProvoquant = 1578;
    public const int BelluairePatteMielleuse = 1579;
    public const int BelluaireChargeSanglier = 1580;
    public const int BelluaireRageSanglier = 1581;
    public const int BelluaireCorbeauAveuglement = 1582;
    public const int BelluaireCorbeauMauvaisAugure = 1583;
    public const int BelluaireLoupMorsurePlongeante = 1584;
    public const int BelluaireLoupEffetDeMeute = 1585;
    public const int BelluaireLoupMorsureInfectieuse = 1586;
    public const int BelluaireSpiderWeb = 1587;
    public const int BelluaireSpiderCocoon = 1588;

    public const int BarbarianRagePersistante = 1599;

    public const int ImpositionDesMains = 1600;

    public const int SensDivin = 1603;
    public const int ChatimentDivin = 1604;

    public const int AuraDeProtection = 1606;
    public const int AuraDeCourage = 1607;
    public const int DevotionArmeSacree = 1608;
    public const int DevotionSaintesRepresailles = 1609;
    public const int DevotionRenvoiDesImpies = 1610;
    public const int DevotionChatimentProtecteur = 1804;
    public const int DevotionNimbeSacree = 1611;
    public const int AnciensRenvoiDesInfideles = 1612;
    public const int AnciensGuerisonRayonnante = 1613;
    public const int AnciensCourrouxDeLaNature = 1614;
    public const int AnciensChampionAntique = 1615;
    public const int PaladinVoeuHostile = 1616;
    public const int PaladinPuissanceInquisitrice = 1617;
    public const int PaladinConspuerEnnemi = 1618;
    public const int AngeDeLaVengeance = 1619;

    public const int ClercProtecteur = 1624;
    public const int ClercThaumaturge = 1627;
    public const int ClercEtincelleDivine = 1645;
    public const int ClercFrappeDivine = 1650;
    public const int ClercInterventionDivine = 1620;
    public const int ClercBenedictionEscroc = 1621;
    public const int ClercRepliqueInvoquee = 1622;
    public const int ClercLinceulDombre = 1623;
    public const int ClercMartial = 1625;
    public const int ClercFrappeGuidee = 1626;
    public const int ClercIllumination = 1628;
    public const int ClercRadianceDeLaube = 1629;
    public const int ClercIncantationPuissante = 1630;
    public const int ClercHaloDeLumiere = 1631;
    public const int ClercCharmePlanteEtAnimaux = 1632;
    public const int ClercAttenuationElementaire = 1633;
    public const int ClercMaitreDeLaNature = 1635;

    public const int WizardIllusionAmelioree = 1636;

    public const int ClercSavoirAncestral = 1637;

    public const int TraqueurLinceulDombre = 1638;

    public const int ClercDetectionDesPensees = 1639;
    public const int ClercVisionDuPasse = 1640;

    public const int ClercFureurOuragan = 1641;
    public const int ClercFureurDestructrice = 1643;
    public const int ClercElectrocution = 1644;
    public const int ClercEnfantDeLaTempete = 1646;

    public const int ClercDiscipleDeLaVie = 1647;
    public const int ClercPreservationDeLaVie = 1648;
    public const int ClercGuerriseurBeni = 1649;
    public const int ClercGuerisonSupreme = 1651;

    public const int MonkLienElementaire = 1652;
    public const int MonkDagueDeGivre = 1653;
    public const int MonkFrissonDeLaMontagne = 1654;
    public const int MonkCrochetsDuSerpentDeFeu = 1655;
    public const int MonkPoingDesQuatreTonnerres = 1656;
    public const int MonkPoingDeLair = 1657;
    public const int MonkRueeDesEspritsDuVent = 1658;
    public const int MonkSphereDequilibreElementaire = 1659;
    public const int MonkFrappeDesCendres = 1660;
    public const int MonkFrappeDeLaTempete = 1661;
    public const int MonkFouetDeLonde = 1662;
    public const int MonkFaconnageDeLaRiviere = 1663;
    public const int MonkPoigneDuVentDuNord = 1664;
    public const int MonkEtreinteDeLenfer = 1665;
    public const int MonkGongDuSommet = 1666;
    public const int MonkFlammesDuPhenix = 1667;
    public const int MonkPostureBrumeuse = 1668;
    public const int MonkPorteParLeVent = 1669;
    public const int MonkDefenseDeLaMontagne = 1670;
    public const int MonkTorrentDeFlammes = 1671;
    public const int MonkVagueDeTerre = 1672;
    public const int MonkSouffleDeLhiver = 1673;
    public const int MonkIncantationElementaire = 1674;

    public const int Sportif = 1675;
    public const int MonkUnarmoredSpeed = 1676;

    public const int SorcellerieInnee = 1677;
    public const int EnsoPrudence = 1678;
    public const int EnsoAllonge = 1679;
    public const int EnsoExtension = 1680;
    public const int EnsoAmplification = 1681;
    public const int EnsoGemellite = 1682;
    public const int EnsoIntensification = 1683;
    public const int EnsoAcceleration = 1684;
    public const int EnsoGuidage = 1685;
    public const int EnsoSubtilite = 1686;
    public const int EnsoTransmutation = 1687;
    public const int SorcellerieIncarnee = 1688;
    public const int EnsoSourceToSlot = 1689;
    public const int EnsoSlotToSource = 1690;
    public const int RetablissementSorcier = 1691;
    public const int EnsoApotheose = 1692;

    public const int EnsoResistanceDraconique = 1693;
    public const int EnsoDracoAffiniteAcide = 1694;
    public const int EnsoDracoAffiniteFroid = 1695;
    public const int EnsoDracoAffiniteFeu = 1696;
    public const int EnsoDracoAffiniteElec = 1697;
    public const int EnsoDracoAffinitePoison = 1698;
    public const int EnsoDracoWings = 1699;
    public const int EnsoCompagnonDraconique = 1700;

    public const int EnsoMagieTempetueuse = 1701;
    public const int EnsoCoeurDeLaTempete = 1702;
    public const int EnsoGuideTempete = 1703;
    public const int EnsoFureurTempete = 1704;
    public const int EnsoAmeDesVents = 1705;

    public const int DruideSage = 1706;
    public const int DruideCompagnonSauvage = 1707;
    public const int DruideReveilSauvage = 1708;
    public const int DruideIncantationPuissante = 1709;
    public const int DruideFrappePrimordialeFroid = 1710;
    public const int DruideFrappePrimordialeFeu = 1711;
    public const int DruideFrappePrimordialeElec = 1712;
    public const int DruideFrappePrimordialeTonnerre = 1713;
    public const int DruideIncantationBestiale = 1714;
    public const int FormeSauvagePersistante = 1715;
    public const int MageNature = 1716;

    public const int PaladinChatimentAmeliore = 1717;

    public const int FormeSauvageBlaireau = 1718;
    public const int FormeSauvageChat = 1719;
    public const int FormeSauvageAraignee = 1720;
    public const int FormeSauvageLoup = 1721;
    public const int FormeSauvageRothe = 1722;
    public const int FormeSauvagePanthere = 1723;
    public const int FormeSauvageOursHibou = 1724;
    public const int FormeSauvageDilophosaure = 1725;

    public const int DruideAssistanceTerrestre = 1726;
    public const int DruideEconomieNaturelle = 1727;
    public const int DruideRecuperationNaturelle = 1728;
    public const int DruideProtectionNaturelle = 1729;
    public const int DruideSanctuaireNaturel = 1730;

    public const int DruideFormeDeLune = 1731;
    public const int FormeSauvageOurs = 1732;
    public const int DruideResilienceSauvage = 1733;
    public const int DruideLuneRadieuse = 1734;
    public const int FormeSauvageCorbeau = 1735;
    public const int FormeSauvageTigre = 1736;
    public const int FormeSauvageAir= 1737;
    public const int FormeSauvageTerre= 1738;
    public const int FormeSauvageFeu= 1739;
    public const int FormeSauvageEau= 1740;

    public const int DruideFureurDesFlots = 1741;

    public const int OccultisteFourberieMagique = 1742;
    public const int OccultisteContactDoutremonde = 1743;
    public const int DechargeDechirante = 1744;
    public const int ArmureDesOmbres = 1745;
    public const int PasAerien = 1746;
    public const int VisionDiabolique = 1747;
    public const int LameDevorante = 1748;
    public const int EspritOcculte = 1749;
    public const int ChatimentOcculte = 1750;
    public const int VigueurDemoniaque = 1751;
    public const int DoubleVue = 1752;
    public const int DonPelagique = 1753;
    public const int DonDuProtecteur = 1754;
    public const int MaitreDesChaines = 1755;
    public const int BuveuseDeVie = 1756;
    public const int MasqueDesMilleVisages = 1757;
    public const int MaitreDesFormes = 1758;
    public const int VisionsBrumeuses = 1759;
    public const int UnParmiLesOmbres = 1760;
    public const int SautDoutremonde = 1761;
    public const int PacteDeLaLame = 1762;
    public const int PacteDeLaChaine = 1763;
    public const int PacteDuTome = 1764;
    public const int LameAssoiffee = 1765;
    public const int VisionsDesRoyaumesLointains = 1766;
    public const int MurmuresDuTombeau = 1767;
    public const int OeilDeSorciere = 1768;
    public const int DechargeRepulsive = 1769;
    public const int PacteDeLaLameInvoquer = 1770;

    public const int AttaqueSupplementaire = 1771;
    public const int AttaqueSupplementaire2 = 1772;
    public const int AttaqueSupplementaire3 = 1773;

    public const int FouleeRafraichissante = 1774;
    public const int FouleeProvocatrice = 1775;
    public const int FouleeEvanescente = 1776;
    public const int FouleeRedoutable = 1777;
    public const int DefensesEnjoleuses = 1778;
    public const int FouleeEnjoleuse = 1779;

    public const int AmeRadieuse = 1780;
    public const int ResilienceCeleste = 1781;
    public const int LueurDeGuérison = 1782;
    public const int VengeanceCalcinante = 1783;

    public const int BenedictionDuMalin = 1784;
    public const int FaveurDuMalin = 1785;
    public const int ResilienceFielleuse = 1786;
    public const int TraverseeInfernale = 1787;    

    public const int EspritEveille = 1788;    
    public const int SortsPsychiques = 1789;    
    public const int CombattantClairvoyant = 1790;    
    public const int BouclierPsychique = 1791;    

    public const int AilesAngeliques = 1792;    
    public const int MainsGuerisseuses = 1793;    
    public const int RevelationCeleste = 1794;  
    
    public const int FrappeBrutale = 1795;    
    public const int FrappeSiderante = 1796;    
    public const int FrappeDechirante = 1797;

    public const int ExpertiseEraflure = 1085;
    public const int ExpertiseCommotion = 1086;
    public const int ExpertiseAffaiblissement = 1807;
    public const int ExpertiseArretCardiaque = 1808;
    public const int ExpertiseTranspercer = 1809;
    public const int ExpertiseMoulinet = 1810;
    public const int ExpertiseLaceration = 1811;
    public const int ExpertiseMutilation = 1812;
    public const int ExpertiseFendre = 1813;
    public const int ExpertiseCharge = 1814;
    public const int ExpertiseFrappeDuPommeau = 1815;
    public const int ExpertiseDesarmement = 1816;
    public const int ExpertiseBriseEchine = 1817;
    public const int ExpertiseRenforcement = 1818;
    public const int ExpertisePreparation = 1819;
    public const int ExpertiseTirPercant = 1820;
    public const int ExpertiseAttaqueMobile = 1821;
    public const int ExpertiseStabilisation = 1822;
    public const int ExpertiseCoupeJarret = 1823;
    public const int ExpertiseDestabiliser = 1824;
    public const int ExpertiseEntaille = 1825;

    public const int ExpertiseLance = 1826;
    public const int ExpertiseEpeeLongue = 1827;
    public const int ExpertiseEpeeCourte = 1828;
    public const int ExpertiseArcLong = 1829;
    public const int ExpertiseArcCourt = 1830;
    public const int ExpertiseRapiere = 1831;
    public const int ExpertiseMarteauLeger = 1832;
    public const int ExpertiseMarteauDeGuerre = 1833;
    public const int ExpertiseHachette = 1834;
    public const int ExpertiseHacheNaine = 1835;
    public const int ExpertiseShuriken = 1836;
    public const int ExpertiseLameDouble = 1837;
    public const int ExpertiseGourdin = 1838;
    public const int ExpertiseDague = 1839;
    public const int ExpertiseHacheDouble = 1840;
    public const int ExpertiseBaton = 1841;
    public const int ExpertiseMasseLegere = 1842;
    public const int ExpertiseSerpe = 1843;
    public const int ExpertiseArbaleteLegere = 1844;
    public const int ExpertiseDard = 1845;
    public const int ExpertiseFleauLeger = 1846;
    public const int ExpertiseMorgenstern = 1847;
    public const int ExpertiseFronde = 1848;
    public const int ExpertiseHacheDeGuerre = 1849;
    public const int ExpertiseHacheDarmes = 1850;
    public const int ExpertiseEspadon = 1851;
    public const int ExpertiseCimeterre = 1852;
    public const int ExpertiseHallebarde = 1853;
    public const int ExpertiseFleauLourd = 1854;
    public const int ExpertiseHacheDeLancer = 1855;
    public const int ExpertiseFouet = 1856;
    public const int ExpertiseArbaleteLourde = 1857;
    public const int ExpertiseEpeeBatarde = 1858;
    public const int ExpertiseFaux = 1859;
    public const int ExpertiseMasseDouble = 1860;
    public const int ExpertiseKama = 1861;
    public const int ExpertiseKatana = 1862;
    public const int ExpertiseKukri = 1863;

    public const int SpearProficiency = 1864;
    public const int LongSwordProficiency = 1865;
    public const int ShortSwordProficiency = 1866;
    public const int LongBowProficiency = 1867;
    public const int ShortBowProficiency = 1868;
    public const int RapierProficiency = 1869;
    public const int LightHammerProficiency = 1870;
    public const int WarHammerProficiency = 1871;
    public const int HandAxeProficiency = 1872;
    public const int BattleaxeProficiency = 1873;
    public const int DwarvenAxeProficiency = 1874;
    public const int ShurikenProficiency = 1875;
    public const int DoubleBladeProficiency = 1876;
    public const int ClubProficiency = 1877;
    public const int DaggerProficiency = 1878;
    public const int DoubleAxeProficiency = 1879;
    public const int QuarterstaffProficiency = 1880;
    public const int LightMaceProficiency = 1881;
    public const int SickleProficiency = 1882;
    public const int LightCrossbowProficiency = 1883;
    public const int DartProficiency = 1884;
    public const int LightFlailProficiency = 1885;
    public const int MorningstarProficiency = 1886;
    public const int SlingProficiency = 1887;
    public const int GreataxeProficiency = 1888;
    public const int GreatswordProficiency = 1889;
    public const int ScimitarProficiency = 1890;
    public const int HalberdProficiency = 1891;
    public const int HeavyFlailProficiency = 1892;
    public const int ThrowingAxeProficiency = 1893;
    public const int WhipProficiency = 1894;
    public const int HeavyCrossbowProficiency = 1895;
    public const int BastardswordProficiency = 1896;
    public const int ScytheProficiency = 1897;
    public const int DireMaceProficiency = 1898;
    public const int KamaProficiency = 1899;
    public const int KatanaProficiency = 1900;
    public const int KukriProficiency = 1901;

    public const int LightArmorProficiency = 1902;
    public const int MediumArmorProficiency = 1903;
    public const int HeavyArmorProficiency = 1904;
    public const int ShieldProficiency = 1905;
    public const int SimpleWeaponProficiency = 1906;
    public const int MartialWeaponProficiency = 1907;
    public const int ExoticWeaponProficiency = 1908;

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

    public const int FighterCombatStyleProtection = 20479;
    public const int FighterCombatStyleDualWield = 20480;
    public const int FighterInflexible = 20483;
    
    /*public const int FighterEldritchKnightBoundWeapon = 20486;
    public const int FighterEldritchKnightWarMagic = 20487; // 2 niveaux
    public const int FighterEldritchKnightEldritchStrike = 20488;
    public const int FighterEldritchKnightArcaneRush = 20489;*/
  
    public const int AbilityImprovement = 20486;
    public const int Actor = 20487;
    //public const int Vigilant = 20488;
    
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
        
    public const int Healer = 20516;

    public const int FighterChampionBonusCombatStyle = 20517;
    public const int FighterChampionUltimeSurvivant = 20519; // 2 niveaux

    public const int Barbarian = 20520;
    public const int BarbarianInstinctSauvage = 20523;
    public const int BarbarianCritiqueBrutal = 20524;
    public const int BarbarianRageImplacable = 20525;
    
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

    public const int MonkPaume = 20542;  
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

    public const int RogueArcaneTrickster = 20559;

    public const int EldritchKnightMagieDeGuerre = 20560;
    public const int EldritchKnightFrappeOcculte = 20561;

    public const int Bard = 20562;
    public const int BardInspirationSuperieure = 20563;

    public const int BardCollegeDuSavoir = 20564;
    public const int BardCollegeDeLaVaillance = 20565;
    public const int BardCollegeDeLescrime = 20567;

    public const int Ranger = 20569;
    
    //public const int RangerGardienDuVoile = 20570;
    //public const int RangerBriseurDeMages = 20571;
    //public const int RangerSanctifie = 20572;
    //public const int RangerBeastTamer = 20573;
    //public const int RangerUrbanTracker = 20574;

    public const int RangerChasseur = 20576;
    public const int RangerBelluaire = 20578;
    public const int RangerProfondeurs = 20579;
    public const int EsquiveSurnaturelle = 20581;

    /*public const int FavoredEnemyGiant = 20582;
    public const int FavoredEnemyAberration = 20583;
    public const int FavoredEnemyBeast = 20584;
    public const int FavoredEnemyMagicalBeast = 20585;
    public const int FavoredEnemyConstruct = 20586;
    public const int FavoredEnemyDragon = 20587;
    public const int FavoredEnemyGoblinoid = 20588;
    public const int FavoredEnemyMonstrous = 20589;
    public const int FavoredEnemyOrc = 20590;
    public const int FavoredEnemyReptilian = 20591;
    public const int FavoredEnemyElemental = 20592;
    public const int FavoredEnemyFey = 20593;
    public const int FavoredEnemyOutsider = 20594;
    public const int FavoredEnemyShapechanger = 20595;
    public const int FavoredEnemyUndead = 20596;
    public const int FavoredEnemyVermin = 20597;*/

    public const int Paladin = 20598;

    public const int PaladinSermentDevotion = 20600;
    public const int PaladinAuraDeDevotion = 20601;
    public const int PaladinSermentDesAnciens = 20602;
    public const int PaladinAuraDeGarde = 20603;
    public const int PaladinSentinelleImmortelle = 20604;

    public const int PaladinSermentVengeance = 20605;
    public const int PaladinVengeurImplacable = 20606;

    public const int Clerc = 20607;
    public const int ClercRenvoiDesMortsVivants = 20608;
    public const int ClercDestructionDesMortsVivants = 20609;
    public const int ClercDuperie = 20610;
    public const int ClercGuerre = 20611;
    public const int ClercGuerreAvatarDeBataille = 20612;
    public const int ClercLumiere = 20613;
    public const int ClercNature = 20614;
    public const int ClercSavoir = 20615;
    public const int ClercTempete = 20616;
    public const int ClercVie = 20617;

    public const int Ensorceleur = 20618;
    public const int EnsorceleurLigneeDraconique = 20619;
    public const int EnsorceleurTempete = 20620;

    public const int Druide = 20622;
    public const int DruideGardien = 20623;
    public const int DruideFrappePrimordiale = 20624;

    public const int DruideCercleTellurique = 20625;
    public const int DruideCercleTerreAride = 20626;
    public const int DruideCercleTerrePolaire = 20627;
    public const int DruideCercleTerreTempere = 20628;
    public const int DruideCercleTerreTropicale = 20629;

    public const int DruideCercleSelenite = 20630;
    public const int DruideCerclePelagique = 20631;

    public const int Occultiste = 20632;
    public const int OccultisteArchifee = 20633;
    public const int OccultisteCeleste = 20634;
    public const int OccultisteFielon = 20635;
    public const int OccultisteGrandAncien = 20636; 

    public const int Invalid = 65535;
  }
}
