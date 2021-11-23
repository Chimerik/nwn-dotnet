﻿using Anvil.API;

namespace NWN.Systems
{
  public class CustomSkill
  {
    // Les compétences customs activables sous forme de dons sont numérotées à partir de 10000 afin d'éviter la collision avec les ID de sorts dans le dictionnary global des Learnables
    // Les compétences customs passives sont numérotées à partir de 20000 afin de laisser suffisament de marge pour pouvoir ajouter des actifs sans tout retoucher
    public const int Elfique = 1116;
    public const int Nain = 1117;
    public const int Orc = 1118;
    public const int Géant = 1119;
    public const int Gobelin = 1120;
    public const int Halfelin = 1121;
    public const int Abyssal = 1122;
    public const int Céleste = 1123;
    public const int Draconique = 1124;
    public const int Profond = 1125;
    public const int Infernal = 1126;
    public const int Primordiale = 1127;
    public const int Sylvain = 1128;
    public const int Druidique = 1129;
    public const int Voleur = 1130;
    public const int Gnome = 1131;
    public const int CustomMenuUP = 1132;
    public const int CustomMenuDOWN = 1133;
    public const int CustomMenuSELECT = 1134;
    public const int CustomMenuEXIT = 1135;
    public const int BlueprintCopy = 1136;
    public const int Research = 1137;
    
    public const int ImprovedAnimalEmpathy = 1144;
    public const int ImprovedConcentration = 1145;
    public const int ImprovedDisableTraps = 1146;
    public const int ImprovedDiscipline = 1147;
    public const int ImprovedHeal = 1148;
    public const int ImprovedHide = 1149;
    public const int ImprovedListen = 1150;
    public const int ImprovedLore = 1151;
    public const int ImprovedMoveSilently = 1152;
    public const int ImprovedOpenLock = 1153;
    public const int ImprovedSkillParry = 1154;
    public const int ImprovedPerform = 1155;
    public const int ImprovedPickpocket = 1156;
    public const int ImprovedSearch = 1157;
    public const int ImprovedSetTrap = 1158;
    public const int ImprovedSpellcraft = 1159;
    public const int ImprovedSpot = 1160;
    public const int ImprovedTaunt = 1161;
    public const int ImprovedUseMagicDevice = 1162;
    public const int ImprovedTumble = 1163;
    public const int ImprovedBluff = 1164;
    public const int ImprovedIntimidate = 1165;

    public const int ImprovedAttackBonus = 1167;
    public const int ImprovedSpellSlot0 = 1168;
    public const int ImprovedSpellSlot1 = 1169;
    public const int ImprovedSpellSlot2 = 1170;
    public const int ImprovedSpellSlot3 = 1171;
    public const int ImprovedSpellSlot4 = 1172;
    public const int ImprovedSpellSlot5 = 1173;
    public const int ImprovedSpellSlot6 = 1174;
    public const int ImprovedSpellSlot7 = 1175;
    public const int ImprovedSpellSlot8 = 1176;
    public const int ImprovedSpellSlot9 = 1177;
    public const int ImprovedCasterLevel = 1178;
    public const int ImprovedSavingThrowAll = 1179;
    public const int ImprovedSavingThrowFortitude = 1180;
    public const int ImprovedSavingThrowReflex = 1181;
    public const int ImprovedSavingThrowWill = 1182;
    public const int Metallurgy = 1183;
    public const int AdvancedCraft = 1184;
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
    public const int StripMiner = 1204;
    public const int Reprocessing = 1205;
    public const int ReprocessingEfficiency = 1206;
    public const int Connections = 1207;
    public const int Forge = 1208;
    public const int CraftClothing = 1209;
    public const int CraftFullPlate = 1210;
    public const int CraftHalfPlate = 1211;
    public const int CraftSplintMail = 1212;
    public const int CraftBreastPlate = 1213;
    public const int CraftScaleMail = 1214;
    public const int CraftStuddedLeather = 1215;
    public const int CraftLeatherArmor = 1216;
    public const int CraftPaddedArmor = 1217;
    public const int CraftShortsword = 1218;
    public const int CraftLongsword = 1219;
    public const int CraftBattleAxe = 1220;
    public const int CraftBastardSword = 1221;
    public const int CraftLightFlail = 1222;
    public const int CraftWarHammer = 1223;
    public const int CraftHeavyCrossbow = 1224;
    public const int CraftLightCrossbow = 1225;
    public const int CraftLongBow = 1226;
    public const int CraftLightMace = 1227;
    public const int CraftHalberd = 1228;
    public const int CraftShortBow = 1229;
    public const int CraftTwoBladedSword = 1230;
    public const int CraftGreatSword = 1231;
    public const int CraftSmallShield = 1232;
    public const int CraftTorch = 1233;
    public const int CraftHelmet = 1234;
    public const int CraftGreatAxe = 1235;
    public const int CraftAmulet = 1236;
    public const int CraftArrow = 1237;
    public const int CraftBelt = 1238;
    public const int CraftDagger = 1239;
    public const int CraftBolt = 1240;
    public const int CraftBoots = 1241;
    public const int CraftBullets = 1242;
    public const int CraftClub = 1243;
    public const int CraftDarts = 1244;
    public const int CraftDireMace = 1245;
    public const int CraftHeavyFlail = 1246;
    public const int CraftGloves = 1247;
    public const int CraftLightHammer = 1248;
    public const int CraftHandAxe = 1249;
    public const int CraftKama = 1250;
    public const int CraftKukri = 1251;
    public const int CraftMagicRod = 1252;
    public const int CraftStaff = 1253;
    public const int CraftMagicWand = 1254;
    public const int CraftMorningStar = 1255;
    public const int CraftPotion = 1256;
    public const int CraftQuarterstaff = 1257;
    public const int CraftRapier = 1258;
    public const int CraftRing = 1259;
    public const int CraftScimitar = 1260;
    public const int CraftScythe = 1261;
    public const int CraftLargeShield = 1262;
    public const int CraftTowerShield = 1263;
    public const int CraftShortSpear = 1264;
    public const int CraftShuriken = 1265;
    public const int CraftSickle = 1266;
    public const int CraftSling = 1267;
    public const int CraftThrowingAxe = 1268;
    public const int CraftSpellScroll = 1269;
    public const int CraftBracer = 1270;
    public const int CraftCloak = 1271;
    public const int CraftTrident = 1272;
    public const int CraftDwarvenWarAxe = 1273;
    public const int CraftWhip = 1274;
    public const int CraftDoubleAxe = 1275;
    public const int CraftForgeHammer = 1276;
    public const int CraftKatana = 1277;
    public const int CraftOreExtractor = 1278;
    public const int CustomPositionRight = 1279;
    public const int CustomPositionLeft = 1280;
    public const int CustomPositionRotateRight = 1281;
    public const int CustomPositionRotateLeft = 1282;
    public const int CustomPositionForward = 1283;
    public const int CustomPositionBackward = 1284;
    public const int WoodCutter = 1285;
    public const int WoodExpertise = 1286;
    public const int WoodProspection = 1287;
    public const int Skinning = 1288;
    public const int AnimalExpertise = 1289;
    public const int Hunting = 1290;
    public const int Ebeniste = 1291;
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
    public const int Tanner = 1302;
    public const int PeltReprocessing = 1303;
    public const int PeltReprocessingEfficiency = 1304;
    public const int BadPeltReprocessing = 1305;
    public const int CommonPeltReprocessing = 1306;
    public const int NormalPeltReprocessing = 1307;
    public const int UncommunPeltReprocessing = 1308;
    public const int RarePeltReprocessing = 1309;
    public const int MagicPeltReprocessing = 1310;
    public const int EpicPeltReprocessing = 1311;
    public const int LegendaryPeltReprocessing = 1312;
    public const int Recycler = 1313;
    public const int ContractScience = 1314;
    public const int Marchand = 1315;
    public const int Magnat = 1316;
    public const int Negociateur = 1317;
    public const int BrokerRelations = 1318;
    public const int BrokerAffinity = 1319;
    public const int Comptabilite = 1320;
    public const int Enchanteur = 1321;
    public const int ArtisanExceptionnel = 1322;
    public const int SurchargeArcanique = 1323;
    public const int SurchargeControlee = 1324;
    public const int EnchanteurExpert = 1325;
    public const int EnchanteurChanceux = 1326;
    public const int ArtisanApplique = 1327;
    public const int Renforcement = 1328;
    public const int CombattantPrecautionneux = 1329;
    public const int Sit = 1330;
    public const int MetalRepair = 1331;
    public const int WoodRepair = 1332;
    public const int LeatherRepair = 1333;
    public const int EnchantRepair = 1334;
    public const int ImprovedDodge = 1335;
    public const int AlchemistEfficiency = 1336;
    public const int AlchemistCareful = 1337;
    public const int AlchemistExpert = 1338;
    public const int Alchemist = 1339;
    public const int AlchemistAware = 1340;
    public const int AlchemistAccurate = 1341;
    public const int LongSwordMastery = 1342;
    public const int FistMastery = 1343;
    public const int ShortSwordMastery = 1344;
    public const int BattleAxeMastery = 1345;
    public const int BastardsSwordMastery = 1346;
    public const int LightFlailMastery = 1347;
    public const int WarhammerMastery = 1348;
    public const int HeavyCrossbowMastery = 1349;
    public const int LightCrossbowMastery = 1350;
    public const int LongbowMastery = 1351;
    public const int LightMaceMastery = 1352;
    public const int HalberdMastery = 1353;
    public const int TwoBladedSwordMastery = 1354;
    public const int ShortbowMastery = 1355;
    public const int GreatSwordMastery = 1356;
    public const int GreatAxeMastery = 1357;
    public const int DaggerMastery = 1358;
    public const int ClubMastery = 1359;
    public const int DartMastery = 1360;
    public const int DireMaceMastery = 1361;
    public const int HeavyFlailMastery = 1362;
    public const int LightHammerMastery = 1363;
    public const int HandAxeMastery = 1364;
    public const int KamaMastery = 1365;
    public const int KatanaMastery = 1366;
    public const int KukriMastery = 1367;
    public const int MagicStaffMastery = 1368;
    public const int MorningStarMastery = 1369;
    public const int QuarterStaffMastery = 1370;
    public const int RapierMastery = 1371;
    public const int ScimitarMastery = 1372;
    public const int ScytheMastery = 1373;
    public const int ShortSpearMastery = 1374;
    public const int ShurikenMastery = 1375;
    public const int SickleMastery = 1376;
    public const int SlingMastery = 1377;
    public const int ThrowingAxeMastery = 1378;
    public const int TridentMastery = 1379;
    public const int DwarvenWaraxeMastery = 1380;
    public const int WhipMastery = 1381;
    public const int DoubleAxeMastery = 1382;
    public const int LongSwordScience = 1383;
    public const int FistScience = 1384;
    public const int ShortSwordScience = 1385;
    public const int BattleAxeScience = 1386;
    public const int BastardsSwordScience = 1387;
    public const int LightFlailScience = 1388;
    public const int WarhammerScience = 1389;
    public const int HeavyCrossbowScience = 1390;
    public const int LightCrossbowScience = 1391;
    public const int LongbowScience = 1392;
    public const int LightMaceScience = 1393;
    public const int HalberdScience = 1394;
    public const int TwoBladedSwordScience = 1395;
    public const int ShortbowScience = 1396;
    public const int GreatSwordScience = 1397;
    public const int GreatAxeScience = 1398;
    public const int DaggerScience = 1399;
    public const int ClubScience = 1400;
    public const int DartScience = 1401;
    public const int DireMaceScience = 1402;
    public const int HeavyFlailScience = 1403;
    public const int LightHammerScience = 1404;
    public const int HandAxeScience = 1405;
    public const int KamaScience = 1406;
    public const int KatanaScience = 1407;
    public const int KukriScience = 1408;
    public const int MagicStaffScience = 1409;
    public const int MorningStarScience = 1410;
    public const int QuarterStaffScience = 1411;
    public const int RapierScience = 1412;
    public const int ScimitarScience = 1413;
    public const int ScytheScience = 1414;
    public const int ShortSpearScience = 1415;
    public const int ShurikenScience = 1416;
    public const int SickleScience = 1417;
    public const int SlingScience = 1418;
    public const int ThrowingAxeScience = 1419;
    public const int TridentScience = 1420;
    public const int DwarvenWaraxeScience = 1421;
    public const int WhipScience = 1422;
    public const int DoubleAxeScience = 1423;

    public const int ImprovedStrength = 20000;
    public const int ImprovedDexterity = 20001;
    public const int ImprovedConstitution = 20002;
    public const int ImprovedIntelligence = 20003;
    public const int ImprovedWisdom = 20004;
    public const int ImprovedCharisma = 20005;
    public const int ImprovedHealth = 20006;
    public const int Toughness = 20007;

    public const int Invalid = 65535;
  }
}
