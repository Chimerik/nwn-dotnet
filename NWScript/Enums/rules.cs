﻿namespace NWN.Enums {
  public enum ClericDomain {
    Air = 0,
    Animal = 1,
    Death = 3,
    Destruction = 4,
    Earth = 5,
    Evil = 6,
    Fire = 7,
    Good = 8,
    Healing = 9,
    Knowledge = 10,
    Magic = 13,
    Plant = 14,
    Protection = 15,
    Strength = 16,
    Sun = 17,
    Travel = 18,
    Trickery = 19,
    War = 20,
    Water = 21
  }

  public enum ClassType {
    Barbarian = 0,
    Bard = 1,
    Cleric = 2,
    Druid = 3,
    Fighter = 4,
    Monk = 5,
    Paladin = 6,
    Ranger = 7,
    Rogue = 8,
    Sorcerer = 9,
    Wizard = 10,
    Aberration = 11,
    Animal = 12,
    Construct = 13,
    Humanoid = 14,
    Monstrous = 15,
    Elemental = 16,
    Fey = 17,
    Dragon = 18,
    Undead = 19,
    Commoner = 20,
    Beast = 21,
    Giant = 22,
    MagicalBeast = 23,
    Outsider = 24,
    Shapechanger = 25,
    Vermin = 26,
    Shadowdancer = 27,
    Harper = 28,
    ArcaneArcher = 29,
    Assassin = 30,
    Blackguard = 31,
    DivineChampion = 32,
    WeaponMaster = 33,
    Palemaster = 34,
    Shifter = 35,
    DwarvenDefender = 36,
    DragonDisciple = 37,
    Ooze = 38,
    EyeOfGruumsh = 39,
    ShouDisciple = 40,
    PDK = 41,
    Invalid = 255
  }

  public enum RacialType {
    Dwarf = 0,
    Elf = 1,
    Gnome = 2,
    Halfling = 3,
    Halfelf = 4,
    Halforc = 5,
    Human = 6,
    Aberration = 7,
    Animal = 8,
    Beast = 9,
    Construct = 10,
    Dragon = 11,
    HumanoidGoblinoid = 12,
    HumanoidMonstrous = 13,
    HumanoidOrc = 14,
    HumanoidReptilian = 15,
    Elemental = 16,
    Fey = 17,
    Giant = 18,
    MagicalBeast = 19,
    Outsider = 20,
    Shapechanger = 23,
    Undead = 24,
    Vermin = 25,
    All = 28, // Oh bioware..
    Invalid = 28, // yep...
    Ooze = 29, // oh yep..
    Plant = 52 // wtf.
  }

  public enum Skill {
    Invalid = -1,
    AnimalEmpathy = 0,
    Concentration = 1,
    DisableTrap = 2,
    Discipline = 3,
    Heal = 4,
    Hide = 5,
    Listen = 6,
    Lore = 7,
    MoveSilently = 8,
    OpenLock = 9,
    Parry = 10,
    Perform = 11,
    Persuade = 12,
    PickPocket = 13,
    Search = 14,
    SetTrap = 15,
    Spellcraft = 16,
    Spot = 17,
    Taunt = 18,
    UseMagicDevice = 19,
    Appraise = 20,
    Tumble = 21,
    CraftTrap = 22,
    Bluff = 23,
    Intimidate = 24,
    CraftArmor = 25,
    CraftWeapon = 26,
    Ride = 27,
    AllSkills = 255
  }

  public enum Feat {
    Alertness = 0,
    Ambidexterity = 1,
    ArmorProficiencyHeavy = 2,
    ArmorProficiencyLight = 3,
    ArmorProficiencyMedium = 4,
    CalledShot = 5,
    Cleave = 6,
    CombatCasting = 7,
    DeflectArrows = 8,
    Disarm = 9,
    Dodge = 10,
    EmpowerSpell = 11,
    ExtendSpell = 12,
    ExtraTurning = 13,
    GreatFortitude = 14,
    ImprovedCritical_Club = 15,
    ImprovedDisarm = 16,
    ImprovedKnockdown = 17,
    ImprovedParry = 18,
    ImprovedPowerAttack = 19,
    ImprovedTwoWeaponFighting = 20,
    ImprovedUnarmedStrike = 21,
    IronWill = 22,
    Knockdown = 23,
    LightningReflexes = 24,
    MaximizeSpell = 25,
    Mobility = 26,
    PointBlankShot = 27,
    PowerAttack = 28,
    QuickenSpell = 29,
    RapidShot = 30,
    Sap = 31,
    ShieldProficiency = 32,
    SilenceSpell = 33,
    SkillFocus_AnimalEmpathy = 34,
    SpellFocus_Abjuration = 35,
    SpellPenetration = 36,
    StillSpell = 37,
    StunningFist = 39,
    Toughness = 40,
    TwoWeaponFighting = 41,
    WeaponFinesse = 42,
    WeaponFocus_Club = 43,
    WeaponProficiencyExotic = 44,
    WeaponProficiencyMartial = 45,
    WeaponProficiencySimple = 46,
    WeaponSpecialization_Club = 47,
    WeaponProficiencyDruid = 48,
    WeaponProficiencyMonk = 49,
    WeaponProficiencyRogue = 50,
    WeaponProficiencyWizard = 51,
    ImprovedCritical_Dagger = 52,
    ImprovedCritical_Dart = 53,
    ImprovedCritical_HeavyCrossbow = 54,
    ImprovedCritical_LightCrossbow = 55,
    ImprovedCritical_LightMace = 56,
    ImprovedCritical_MorningStar = 57,
    ImprovedCritical_Staff = 58,
    ImprovedCritical_Spear = 59,
    ImprovedCritical_Sickle = 60,
    ImprovedCritical_Sling = 61,
    ImprovedCritical_UnarmedStrike = 62,
    ImprovedCritical_Longbow = 63,
    ImprovedCritical_Shortbow = 64,
    ImprovedCritical_ShortSword = 65,
    ImprovedCritical_Rapier = 66,
    ImprovedCritical_Scimitar = 67,
    ImprovedCritical_LongSword = 68,
    ImprovedCritical_GreatSword = 69,
    ImprovedCritical_HandAxe = 70,
    ImprovedCritical_ThrowingAxe = 71,
    ImprovedCritical_BattleAxe = 72,
    ImprovedCritical_GreatAxe = 73,
    ImprovedCritical_Halberd = 74,
    ImprovedCritical_LightHammer = 75,
    ImprovedCritical_LightFlail = 76,
    ImprovedCritical_WarHammer = 77,
    ImprovedCritical_HeavyFlail = 78,
    ImprovedCritical_Kama = 79,
    ImprovedCritical_Kukri = 80,
    ImprovedCritical_Shuriken = 82,
    ImprovedCritical_Scythe = 83,
    ImprovedCritical_Katana = 84,
    ImprovedCritical_BastardSword = 85,
    ImprovedCritical_DireMace = 87,
    ImprovedCritical_DoubleAxe = 88,
    ImprovedCritical_TwoBladedSword = 89,
    WeaponFocus_Dagger = 90,
    WeaponFocus_Dart = 91,
    WeaponFocus_HeavyCrossbow = 92,
    WeaponFocus_LightCrossbow = 93,
    WeaponFocus_LightMace = 94,
    WeaponFocus_MorningStar = 95,
    WeaponFocus_Staff = 96,
    WeaponFocus_Spear = 97,
    WeaponFocus_Sickle = 98,
    WeaponFocus_Sling = 99,
    WeaponFocus_UnarmedStrike = 100,
    WeaponFocus_Longbow = 101,
    WeaponFocus_Shortbow = 102,
    WeaponFocus_ShortSword = 103,
    WeaponFocus_Rapier = 104,
    WeaponFocus_Scimitar = 105,
    WeaponFocus_LongSword = 106,
    WeaponFocus_GreatSword = 107,
    WeaponFocus_HandAxe = 108,
    WeaponFocus_ThrowingAxe = 109,
    WeaponFocus_BattleAxe = 110,
    WeaponFocus_GreatAxe = 111,
    WeaponFocus_Halberd = 112,
    WeaponFocus_LightHammer = 113,
    WeaponFocus_LightFlail = 114,
    WeaponFocus_WarHammer = 115,
    WeaponFocus_HeavyFlail = 116,
    WeaponFocus_Kama = 117,
    WeaponFocus_Kukri = 118,
    WeaponFocus_Shuriken = 120,
    WeaponFocus_Scythe = 121,
    WeaponFocus_Katana = 122,
    WeaponFocus_BastardSword = 123,
    WeaponFocus_DireMace = 125,
    WeaponFocus_DoubleAxe = 126,
    WeaponFocus_TwoBladedSword = 127,
    WeaponSpecialization_Dagger = 128,
    WeaponSpecialization_Dart = 129,
    WeaponSpecialization_HeavyCrossbow = 130,
    WeaponSpecialization_LightCrossbow = 131,
    WeaponSpecialization_LightMace = 132,
    WeaponSpecialization_MorningStar = 133,
    WeaponSpecialization_Staff = 134,
    WeaponSpecialization_Spear = 135,
    WeaponSpecialization_Sickle = 136,
    WeaponSpecialization_Sling = 137,
    WeaponSpecialization_UnarmedStrike = 138,
    WeaponSpecialization_Longbow = 139,
    WeaponSpecialization_Shortbow = 140,
    WeaponSpecialization_ShortSword = 141,
    WeaponSpecialization_Rapier = 142,
    WeaponSpecialization_Scimitar = 143,
    WeaponSpecialization_LongSword = 144,
    WeaponSpecialization_GreatSword = 145,
    WeaponSpecialization_HandAxe = 146,
    WeaponSpecialization_ThrowingAxe = 147,
    WeaponSpecialization_BattleAxe = 148,
    WeaponSpecialization_GreatAxe = 149,
    WeaponSpecialization_Halberd = 150,
    WeaponSpecialization_LightHammer = 151,
    WeaponSpecialization_LightFlail = 152,
    WeaponSpecialization_WarHammer = 153,
    WeaponSpecialization_HeavyFlail = 154,
    WeaponSpecialization_Kama = 155,
    WeaponSpecialization_Kukri = 156,
    WeaponSpecialization_Shuriken = 158,
    WeaponSpecialization_Scythe = 159,
    WeaponSpecialization_Katana = 160,
    WeaponSpecialization_BastardSword = 161,
    WeaponSpecialization_DireMace = 163,
    WeaponSpecialization_DoubleAxe = 164,
    WeaponSpecialization_TwoBladedSword = 165,
    SpellFocus_Conjuration = 166,
    SpellFocus_Divination = 167,
    SpellFocus_Enchantment = 168,
    SpellFocus_Evocation = 169,
    SpellFocus_Illusion = 170,
    SpellFocus_Necromancy = 171,
    SpellFocus_Transmutation = 172,
    SkillFocus_Concentration = 173,
    SkillFocus_DisableTrap = 174,
    SkillFocus_Discipline = 175,
    SkillFocus_Heal = 177,
    SkillFocus_Hide = 178,
    SkillFocus_Listen = 179,
    SkillFocus_Lore = 180,
    SkillFocus_MoveSilently = 181,
    SkillFocus_OpenLock = 182,
    SkillFocus_Parry = 183,
    SkillFocus_Perform = 184,
    SkillFocus_Persuade = 185,
    SkillFocus_PickPocket = 186,
    SkillFocus_Search = 187,
    SkillFocus_SetTrap = 188,
    SkillFocus_Spellcraft = 189,
    SkillFocus_Spot = 190,
    SkillFocus_Taunt = 192,
    SkillFocus_UseMagicDevice = 193,
    BarbarianEndurance = 194,
    UncannyDodge1 = 195,
    DamageReduction = 196,
    BardicKnowledge = 197,
    NatureSense = 198,
    AnimalCompanion = 199,
    WoodlandStride = 200,
    TracklessStep = 201,
    ResistNaturesLure = 202,
    VenomImmunity = 203,
    FlurryOfBlows = 204,
    Evasion = 206,
    MonkEndurance = 207,
    StillMind = 208,
    PurityOfBody = 209,
    WholenessOfBody = 211,
    ImprovedEvasion = 212,
    KiStrike = 213,
    DiamondBody = 214,
    DiamondSoul = 215,
    PerfectSelf = 216,
    DivineGrace = 217,
    DivineHealth = 219,
    SneakAttack = 221,
    CripplingStrike = 222,
    DefensiveRoll = 223,
    Opportunist = 224,
    SkillMastery = 225,
    UncannyReflex = 226,
    Stonecunning = 227,
    Darkvision = 228,
    HardinessVersusPoisons = 229,
    HardinessVersusSpells = 230,
    BattleTrainingVersusOrcs = 231,
    BattleTrainingVersusGoblins = 232,
    BattleTrainingVersusGiants = 233,
    SkillAffinityLore = 234,
    ImmunityToSleep = 235,
    HardinessVersusEnchantments = 236,
    SkillAffinityListen = 237,
    SkillAffinitySearch = 238,
    SkillAffinitySpot = 239,
    KeenSense = 240,
    HardinessVersusIllusions = 241,
    BattleTrainingVersusReptilians = 242,
    SkillAffinityConcentration = 243,
    PartialSkillAffinityListen = 244,
    PartialSkillAffinitySearch = 245,
    PartialSkillAffinitySpot = 246,
    SkillAffinityMoveSilently = 247,
    Lucky = 248,
    Fearless = 249,
    GoodAim = 250,
    UncannyDodge2 = 251,
    UncannyDodge3 = 252,
    UncannyDodge4 = 253,
    UncannyDodge5 = 254,
    UncannyDodge6 = 255,
    WeaponProficiencyElf = 256,
    BardSongs = 257,
    QuickToMaster = 258,
    SlipperyMind = 259,
    MonkAcBonus = 260,
    FavoredEnemy_Dwarf = 261,
    FavoredEnemy_Elf = 262,
    FavoredEnemy_Gnome = 263,
    FavoredEnemy_Halfling = 264,
    FavoredEnemy_Halfelf = 265,
    FavoredEnemy_Halforc = 266,
    FavoredEnemy_Human = 267,
    FavoredEnemy_Aberration = 268,
    FavoredEnemy_Animal = 269,
    FavoredEnemy_Beast = 270,
    FavoredEnemy_Construct = 271,
    FavoredEnemy_Dragon = 272,
    FavoredEnemy_Goblinoid = 273,
    FavoredEnemy_Monstrous = 274,
    FavoredEnemy_Orc = 275,
    FavoredEnemy_Reptilian = 276,
    FavoredEnemy_Elemental = 277,
    FavoredEnemy_Fey = 278,
    FavoredEnemy_Giant = 279,
    FavoredEnemy_MagicalBeast = 280,
    FavoredEnemy_Outsider = 281,
    FavoredEnemy_Shapechanger = 284,
    FavoredEnemy_Undead = 285,
    FavoredEnemy_Vermin = 286,
    WeaponProficiencyCreature = 289,
    WeaponSpecialization_Creature = 290,
    WeaponFocus_Creature = 291,
    ImprovedCritical_Creature = 292,
    BarbarianRage = 293,
    TurnUndead = 294,
    QuiveringPalm = 296,
    EmptyBody = 297,
    LayOnHands = 299,
    AuraOfCourage = 300,
    SmiteEvil = 301,
    RemoveDisease = 302,
    SummonFamiliar = 303,
    ElementalShape = 304,
    WildShape = 305,
    WarDomainPower = 306,
    StrengthDomainPower = 307,
    ProtectionDomainPower = 308,
    LuckDomainPower = 309,
    DeathDomainPower = 310,
    AirDomainPower = 311,
    AnimalDomainPower = 312,
    DestructionDomainPower = 313,
    EarthDomainPower = 314,
    EvilDomainPower = 315,
    FireDomainPower = 316,
    GoodDomainPower = 317,
    HealingDomainPower = 318,
    KnowledgeDomainPower = 319,
    MagicDomainPower = 320,
    PlantDomainPower = 321,
    SunDomainPower = 322,
    TravelDomainPower = 323,
    TrickeryDomainPower = 324,
    WaterDomainPower = 325,
    BarbarianRage2 = 326,
    BarbarianRage3 = 327,
    BarbarianRage4 = 328,
    BarbarianRage5 = 329,
    BarbarianRage6 = 330,
    BarbarianRage7 = 331,
    DamageReduction2 = 332,
    DamageReduction3 = 333,
    DamageReduction4 = 334,
    WildShape2 = 335,
    WildShape3 = 336,
    WildShape4 = 337,
    WildShape5 = 338,
    WildShape6 = 339,
    ElementalShape2 = 340,
    ElementalShape3 = 341,
    ElementalShape4 = 342,
    KiStrike2 = 343,
    KiStrike3 = 344,
    SneakAttack2 = 345,
    SneakAttack3 = 346,
    SneakAttack4 = 347,
    SneakAttack5 = 348,
    SneakAttack6 = 349,
    SneakAttack7 = 350,
    SneakAttack8 = 351,
    SneakAttack9 = 352,
    SneakAttack10 = 353,
    Lowlightvision = 354,
    BardSongs2 = 355,
    BardSongs3 = 356,
    BardSongs4 = 357,
    BardSongs5 = 358,
    BardSongs6 = 359,
    BardSongs7 = 360,
    BardSongs8 = 361,
    BardSongs9 = 362,
    BardSongs10 = 363,
    BardSongs11 = 364,
    BardSongs12 = 365,
    BardSongs13 = 366,
    BardSongs14 = 367,
    BardSongs15 = 368,
    BardSongs16 = 369,
    BardSongs17 = 370,
    BardSongs18 = 371,
    BardSongs19 = 372,
    BardSongs20 = 373,
    RangerDualWield = 374,
    SmallStature = 375,
    ImprovedInitiative = 377,
    Artist = 378,
    Blooded = 379,
    Bullheaded = 380,
    CourteousMagocracy = 381,
    LuckOfHeroes = 382,
    ResistPoison = 383,
    SilverPalm = 384,
    SmoothTalk = 385,
    SnakeBlood = 386,
    Stealthy = 387,
    StrongSoul = 388,
    Expertise = 389,
    ImprovedExpertise = 390,
    GreatCleave = 391,
    SpringAttack = 392,
    GreaterSpellFocus_Abjuration = 393,
    GreaterSpellFocus_Conjuration = 394,
    GreaterSpellFocus_Divination = 395,
    GreaterSpellFocus_Enchantment = 396,
    GreaterSpellFocus_Evocation = 397,
    GreaterSpellFocus_Illusion = 398,
    GreaterSpellFocus_Necromancy = 399,
    GreaterSpellFocus_Transmutation = 400,
    GreaterSpellPenetration = 401,
    Thug = 402,
    MercantileBackground = 403,
    SkillFocus_Appraise = 404,
    SkillFocus_Tumble = 406,
    SkillFocus_CraftTrap = 407,
    BlindFight = 408,
    CircleKick = 409,
    ExtraStunningAttack = 410,
    RapidReload = 411,
    ZenArchery = 412,
    DivineMight = 413,
    DivineShield = 414,
    ArcaneDefense_Abjuration = 415,
    ArcaneDefense_Conjuration = 416,
    ArcaneDefense_Divination = 417,
    ArcaneDefense_Enchatment = 418,
    ArcaneDefense_Evocation = 419,
    ArcaneDefense_Illusion = 420,
    ArcaneDefense_Necromancy = 421,
    ArcaneDefense_Transmutation = 422,
    ExtraMusic = 423,
    LingeringSong = 424,
    DirtyFighting = 425,
    ResistDisease = 426,
    ResistEnergy_Cold = 427,
    ResistEnergy_Acid = 428,
    ResistEnergy_Fire = 429,
    ResistEnergy_Electrical = 430,
    ResistEnergy_Sonic = 431,
    HideInPlainSight = 433,
    ShadowDaze = 434,
    SummonShadow = 435,
    ShadowEvade = 436,
    DeneirsEye = 437,
    TymorasSmile = 438,
    LliirasHeart = 439,
    CraftHarperItem = 440,
    HarperSleep = 441,
    HarperCatsGrace = 442,
    HarperEaglesSplendor = 443,
    HarperInvisibility = 444,
    PrestigeEnchantArrow1 = 445,
    PrestigeEnchantArrow2 = 446,
    PrestigeEnchantArrow3 = 447,
    PrestigeEnchantArrow4 = 448,
    PrestigeEnchantArrow5 = 449,
    PrestigeEmbueArrow = 450,
    PrestigeSeekerArrow1 = 451,
    PrestigeSeekerArrow2 = 452,
    PrestigeHailOfArrows = 453,
    PrestigeArrowOfDeath = 454,
    PrestigeDeathAttack1 = 455,
    PrestigeDeathAttack2 = 456,
    PrestigeDeathAttack3 = 457,
    PrestigeDeathAttack4 = 458,
    PrestigeDeathAttack5 = 459,
    BlackguardSneakAttack1d6 = 460,
    BlackguardSneakAttack2d6 = 461,
    BlackguardSneakAttack3d6 = 462,
    PrestigePoisonSave1 = 463,
    PrestigePoisonSave2 = 464,
    PrestigePoisonSave3 = 465,
    PrestigePoisonSave4 = 466,
    PrestigePoisonSave5 = 467,
    PrestigeSpellGhostlyVisage = 468,
    PrestigeDarkness = 469,
    PrestigeInvisibility1 = 470,
    PrestigeInvisibility2 = 471,
    SmiteGood = 472,
    PrestigeDarkBlessing = 473,
    InflictLightWounds = 474,
    InflictModerateWounds = 475,
    InflictSeriousWounds = 476,
    InflictCriticalWounds = 477,
    BullsStrength = 478,
    Contagion = 479,
    Blindsight60Feet = 488,
    EpicArmorSkin = 490,
    EpicBlindingSpeed = 491,
    EpicDamageReduction3 = 492,
    EpicDamageReduction6 = 493,
    EpicDamageReduction9 = 494,
    EpicDevastatingCritical_Club = 495,
    EpicDevastatingCritical_Dagger = 496,
    EpicDevastatingCritical_Dart = 497,
    EpicDevastatingCritical_Heavycrossbow = 498,
    EpicDevastatingCritical_Lightcrossbow = 499,
    EpicDevastatingCritical_Lightmace = 500,
    EpicDevastatingCritical_Morningstar = 501,
    EpicDevastatingCritical_Quarterstaff = 502,
    EpicDevastatingCritical_Shortspear = 503,
    EpicDevastatingCritical_Sickle = 504,
    EpicDevastatingCritical_Sling = 505,
    EpicDevastatingCritical_Unarmed = 506,
    EpicDevastatingCritical_Longbow = 507,
    EpicDevastatingCritical_Shortbow = 508,
    EpicDevastatingCritical_Shortsword = 509,
    EpicDevastatingCritical_Rapier = 510,
    EpicDevastatingCritical_Scimitar = 511,
    EpicDevastatingCritical_Longsword = 512,
    EpicDevastatingCritical_Greatsword = 513,
    EpicDevastatingCritical_Handaxe = 514,
    EpicDevastatingCritical_Throwingaxe = 515,
    EpicDevastatingCritical_Battleaxe = 516,
    EpicDevastatingCritical_Greataxe = 517,
    EpicDevastatingCritical_Halberd = 518,
    EpicDevastatingCritical_Lighthammer = 519,
    EpicDevastatingCritical_Lightflail = 520,
    EpicDevastatingCritical_Warhammer = 521,
    EpicDevastatingCritical_Heavyflail = 522,
    EpicDevastatingCritical_Kama = 523,
    EpicDevastatingCritical_Kukri = 524,
    EpicDevastatingCritical_Shuriken = 525,
    EpicDevastatingCritical_Scythe = 526,
    EpicDevastatingCritical_Katana = 527,
    EpicDevastatingCritical_Bastardsword = 528,
    EpicDevastatingCritical_Diremace = 529,
    EpicDevastatingCritical_Doubleaxe = 530,
    EpicDevastatingCritical_Twobladedsword = 531,
    EpicDevastatingCritical_Creature = 532,
    EpicEnergyResistance_Cold1 = 533,
    EpicEnergyResistance_Cold2 = 534,
    EpicEnergyResistance_Cold3 = 535,
    EpicEnergyResistance_Cold4 = 536,
    EpicEnergyResistance_Cold5 = 537,
    EpicEnergyResistance_Cold6 = 538,
    EpicEnergyResistance_Cold7 = 539,
    EpicEnergyResistance_Cold8 = 540,
    EpicEnergyResistance_Cold9 = 541,
    EpicEnergyResistance_Cold10 = 542,
    EpicEnergyResistance_Acid1 = 543,
    EpicEnergyResistance_Acid2 = 544,
    EpicEnergyResistance_Acid3 = 545,
    EpicEnergyResistance_Acid4 = 546,
    EpicEnergyResistance_Acid5 = 547,
    EpicEnergyResistance_Acid6 = 548,
    EpicEnergyResistance_Acid7 = 549,
    EpicEnergyResistance_Acid8 = 550,
    EpicEnergyResistance_Acid9 = 551,
    EpicEnergyResistance_Acid10 = 552,
    EpicEnergyResistance_Fire1 = 553,
    EpicEnergyResistance_Fire2 = 554,
    EpicEnergyResistance_Fire3 = 555,
    EpicEnergyResistance_Fire4 = 556,
    EpicEnergyResistance_Fire5 = 557,
    EpicEnergyResistance_Fire6 = 558,
    EpicEnergyResistance_Fire7 = 559,
    EpicEnergyResistance_Fire8 = 560,
    EpicEnergyResistance_Fire9 = 561,
    EpicEnergyResistance_Fire10 = 562,
    EpicEnergyResistance_Electrical1 = 563,
    EpicEnergyResistance_Electrical2 = 564,
    EpicEnergyResistance_Electrical3 = 565,
    EpicEnergyResistance_Electrical4 = 566,
    EpicEnergyResistance_Electrical5 = 567,
    EpicEnergyResistance_Electrical6 = 568,
    EpicEnergyResistance_Electrical7 = 569,
    EpicEnergyResistance_Electrical8 = 570,
    EpicEnergyResistance_Electrical9 = 571,
    EpicEnergyResistance_Electrical10 = 572,
    EpicEnergyResistance_Sonic1 = 573,
    EpicEnergyResistance_Sonic2 = 574,
    EpicEnergyResistance_Sonic3 = 575,
    EpicEnergyResistance_Sonic4 = 576,
    EpicEnergyResistance_Sonic5 = 577,
    EpicEnergyResistance_Sonic6 = 578,
    EpicEnergyResistance_Sonic7 = 579,
    EpicEnergyResistance_Sonic8 = 580,
    EpicEnergyResistance_Sonic9 = 581,
    EpicEnergyResistance_Sonic10 = 582,
    EpicFortitude = 583,
    EpicProwess = 584,
    EpicReflexes = 585,
    EpicReputation = 586,
    EpicSkillFocus_AnimalEmpathy = 587,
    EpicSkillFocus_Appraise = 588,
    EpicSkillFocus_Concentration = 589,
    EpicSkillFocus_CraftTrap = 590,
    EpicSkillFocus_Disabletrap = 591,
    EpicSkillFocus_Discipline = 592,
    EpicSkillFocus_Heal = 593,
    EpicSkillFocus_Hide = 594,
    EpicSkillFocus_Listen = 595,
    EpicSkillFocus_Lore = 596,
    EpicSkillFocus_Movesilently = 597,
    EpicSkillFocus_Openlock = 598,
    EpicSkillFocus_Parry = 599,
    EpicSkillFocus_Perform = 600,
    EpicSkillFocus_Persuade = 601,
    EpicSkillFocus_Pickpocket = 602,
    EpicSkillFocus_Search = 603,
    EpicSkillFocus_Settrap = 604,
    EpicSkillFocus_Spellcraft = 605,
    EpicSkillFocus_Spot = 606,
    EpicSkillFocus_Taunt = 607,
    EpicSkillFocus_Tumble = 608,
    EpicSkillFocus_Usemagicdevice = 609,
    EpicSpellFocus_Abjuration = 610,
    EpicSpellFocus_Conjuration = 611,
    EpicSpellFocus_Divination = 612,
    EpicSpellFocus_Enchantment = 613,
    EpicSpellFocus_Evocation = 614,
    EpicSpellFocus_Illusion = 615,
    EpicSpellFocus_Necromancy = 616,
    EpicSpellFocus_Transmutation = 617,
    EpicSpellPenetration = 618,
    EpicWeaponFocus_Club = 619,
    EpicWeaponFocus_Dagger = 620,
    EpicWeaponFocus_Dart = 621,
    EpicWeaponFocus_Heavycrossbow = 622,
    EpicWeaponFocus_Lightcrossbow = 623,
    EpicWeaponFocus_Lightmace = 624,
    EpicWeaponFocus_Morningstar = 625,
    EpicWeaponFocus_Quarterstaff = 626,
    EpicWeaponFocus_Shortspear = 627,
    EpicWeaponFocus_Sickle = 628,
    EpicWeaponFocus_Sling = 629,
    EpicWeaponFocus_Unarmed = 630,
    EpicWeaponFocus_Longbow = 631,
    EpicWeaponFocus_Shortbow = 632,
    EpicWeaponFocus_Shortsword = 633,
    EpicWeaponFocus_Rapier = 634,
    EpicWeaponFocus_Scimitar = 635,
    EpicWeaponFocus_Longsword = 636,
    EpicWeaponFocus_Greatsword = 637,
    EpicWeaponFocus_Handaxe = 638,
    EpicWeaponFocus_Throwingaxe = 639,
    EpicWeaponFocus_Battleaxe = 640,
    EpicWeaponFocus_Greataxe = 641,
    EpicWeaponFocus_Halberd = 642,
    EpicWeaponFocus_Lighthammer = 643,
    EpicWeaponFocus_Lightflail = 644,
    EpicWeaponFocus_Warhammer = 645,
    EpicWeaponFocus_Heavyflail = 646,
    EpicWeaponFocus_Kama = 647,
    EpicWeaponFocus_Kukri = 648,
    EpicWeaponFocus_Shuriken = 649,
    EpicWeaponFocus_Scythe = 650,
    EpicWeaponFocus_Katana = 651,
    EpicWeaponFocus_Bastardsword = 652,
    EpicWeaponFocus_Diremace = 653,
    EpicWeaponFocus_Doubleaxe = 654,
    EpicWeaponFocus_Twobladedsword = 655,
    EpicWeaponFocus_Creature = 656,
    EpicWeaponSpecialization_Club = 657,
    EpicWeaponSpecialization_Dagger = 658,
    EpicWeaponSpecialization_Dart = 659,
    EpicWeaponSpecialization_Heavycrossbow = 660,
    EpicWeaponSpecialization_Lightcrossbow = 661,
    EpicWeaponSpecialization_Lightmace = 662,
    EpicWeaponSpecialization_Morningstar = 663,
    EpicWeaponSpecialization_Quarterstaff = 664,
    EpicWeaponSpecialization_Shortspear = 665,
    EpicWeaponSpecialization_Sickle = 666,
    EpicWeaponSpecialization_Sling = 667,
    EpicWeaponSpecialization_Unarmed = 668,
    EpicWeaponSpecialization_Longbow = 669,
    EpicWeaponSpecialization_Shortbow = 670,
    EpicWeaponSpecialization_Shortsword = 671,
    EpicWeaponSpecialization_Rapier = 672,
    EpicWeaponSpecialization_Scimitar = 673,
    EpicWeaponSpecialization_Longsword = 674,
    EpicWeaponSpecialization_Greatsword = 675,
    EpicWeaponSpecialization_Handaxe = 676,
    EpicWeaponSpecialization_Throwingaxe = 677,
    EpicWeaponSpecialization_Battleaxe = 678,
    EpicWeaponSpecialization_Greataxe = 679,
    EpicWeaponSpecialization_Halberd = 680,
    EpicWeaponSpecialization_Lighthammer = 681,
    EpicWeaponSpecialization_Lightflail = 682,
    EpicWeaponSpecialization_Warhammer = 683,
    EpicWeaponSpecialization_Heavyflail = 684,
    EpicWeaponSpecialization_Kama = 685,
    EpicWeaponSpecialization_Kukri = 686,
    EpicWeaponSpecialization_Shuriken = 687,
    EpicWeaponSpecialization_Scythe = 688,
    EpicWeaponSpecialization_Katana = 689,
    EpicWeaponSpecialization_Bastardsword = 690,
    EpicWeaponSpecialization_Diremace = 691,
    EpicWeaponSpecialization_Doubleaxe = 692,
    EpicWeaponSpecialization_Twobladedsword = 693,
    EpicWeaponSpecialization_Creature = 694,
    EpicWill = 695,
    EpicImprovedCombatCasting = 696,
    EpicImprovedKiStrike4 = 697,
    EpicImprovedKiStrike5 = 698,
    EpicImprovedSpellResistance1 = 699,
    EpicImprovedSpellResistance2 = 700,
    EpicImprovedSpellResistance3 = 701,
    EpicImprovedSpellResistance4 = 702,
    EpicImprovedSpellResistance5 = 703,
    EpicImprovedSpellResistance6 = 704,
    EpicImprovedSpellResistance7 = 705,
    EpicImprovedSpellResistance8 = 706,
    EpicImprovedSpellResistance9 = 707,
    EpicImprovedSpellResistance10 = 708,
    EpicOverwhelmingCritical_Club = 709,
    EpicOverwhelmingCritical_Dagger = 710,
    EpicOverwhelmingCritical_Dart = 711,
    EpicOverwhelmingCritical_Heavycrossbow = 712,
    EpicOverwhelmingCritical_Lightcrossbow = 713,
    EpicOverwhelmingCritical_Lightmace = 714,
    EpicOverwhelmingCritical_Morningstar = 715,
    EpicOverwhelmingCritical_Quarterstaff = 716,
    EpicOverwhelmingCritical_Shortspear = 717,
    EpicOverwhelmingCritical_Sickle = 718,
    EpicOverwhelmingCritical_Sling = 719,
    EpicOverwhelmingCritical_Unarmed = 720,
    EpicOverwhelmingCritical_Longbow = 721,
    EpicOverwhelmingCritical_Shortbow = 722,
    EpicOverwhelmingCritical_Shortsword = 723,
    EpicOverwhelmingCritical_Rapier = 724,
    EpicOverwhelmingCritical_Scimitar = 725,
    EpicOverwhelmingCritical_Longsword = 726,
    EpicOverwhelmingCritical_Greatsword = 727,
    EpicOverwhelmingCritical_Handaxe = 728,
    EpicOverwhelmingCritical_Throwingaxe = 729,
    EpicOverwhelmingCritical_Battleaxe = 730,
    EpicOverwhelmingCritical_Greataxe = 731,
    EpicOverwhelmingCritical_Halberd = 732,
    EpicOverwhelmingCritical_Lighthammer = 733,
    EpicOverwhelmingCritical_Lightflail = 734,
    EpicOverwhelmingCritical_Warhammer = 735,
    EpicOverwhelmingCritical_Heavyflail = 736,
    EpicOverwhelmingCritical_Kama = 737,
    EpicOverwhelmingCritical_Kukri = 738,
    EpicOverwhelmingCritical_Shuriken = 739,
    EpicOverwhelmingCritical_Scythe = 740,
    EpicOverwhelmingCritical_Katana = 741,
    EpicOverwhelmingCritical_Bastardsword = 742,
    EpicOverwhelmingCritical_Diremace = 743,
    EpicOverwhelmingCritical_Doubleaxe = 744,
    EpicOverwhelmingCritical_Twobladedsword = 745,
    EpicOverwhelmingCritical_Creature = 746,
    EpicPerfectHealth = 747,
    EpicSelfConcealment10 = 748,
    EpicSelfConcealment20 = 749,
    EpicSelfConcealment30 = 750,
    EpicSelfConcealment40 = 751,
    EpicSelfConcealment50 = 752,
    EpicSuperiorInitiative = 753,
    EpicToughness1 = 754,
    EpicToughness2 = 755,
    EpicToughness3 = 756,
    EpicToughness4 = 757,
    EpicToughness5 = 758,
    EpicToughness6 = 759,
    EpicToughness7 = 760,
    EpicToughness8 = 761,
    EpicToughness9 = 762,
    EpicToughness10 = 763,
    EpicGreatCharisma1 = 764,
    EpicGreatCharisma2 = 765,
    EpicGreatCharisma3 = 766,
    EpicGreatCharisma4 = 767,
    EpicGreatCharisma5 = 768,
    EpicGreatCharisma6 = 769,
    EpicGreatCharisma7 = 770,
    EpicGreatCharisma8 = 771,
    EpicGreatCharisma9 = 772,
    EpicGreatCharisma10 = 773,
    EpicGreatConstitution1 = 774,
    EpicGreatConstitution2 = 775,
    EpicGreatConstitution3 = 776,
    EpicGreatConstitution4 = 777,
    EpicGreatConstitution5 = 778,
    EpicGreatConstitution6 = 779,
    EpicGreatConstitution7 = 780,
    EpicGreatConstitution8 = 781,
    EpicGreatConstitution9 = 782,
    EpicGreatConstitution10 = 783,
    EpicGreatDexterity1 = 784,
    EpicGreatDexterity2 = 785,
    EpicGreatDexterity3 = 786,
    EpicGreatDexterity4 = 787,
    EpicGreatDexterity5 = 788,
    EpicGreatDexterity6 = 789,
    EpicGreatDexterity7 = 790,
    EpicGreatDexterity8 = 791,
    EpicGreatDexterity9 = 792,
    EpicGreatDexterity10 = 793,
    EpicGreatIntelligence1 = 794,
    EpicGreatIntelligence2 = 795,
    EpicGreatIntelligence3 = 796,
    EpicGreatIntelligence4 = 797,
    EpicGreatIntelligence5 = 798,
    EpicGreatIntelligence6 = 799,
    EpicGreatIntelligence7 = 800,
    EpicGreatIntelligence8 = 801,
    EpicGreatIntelligence9 = 802,
    EpicGreatIntelligence10 = 803,
    EpicGreatWisdom1 = 804,
    EpicGreatWisdom2 = 805,
    EpicGreatWisdom3 = 806,
    EpicGreatWisdom4 = 807,
    EpicGreatWisdom5 = 808,
    EpicGreatWisdom6 = 809,
    EpicGreatWisdom7 = 810,
    EpicGreatWisdom8 = 811,
    EpicGreatWisdom9 = 812,
    EpicGreatWisdom10 = 813,
    EpicGreatStrength1 = 814,
    EpicGreatStrength2 = 815,
    EpicGreatStrength3 = 816,
    EpicGreatStrength4 = 817,
    EpicGreatStrength5 = 818,
    EpicGreatStrength6 = 819,
    EpicGreatStrength7 = 820,
    EpicGreatStrength8 = 821,
    EpicGreatStrength9 = 822,
    EpicGreatStrength10 = 823,
    EpicGreatSmiting1 = 824,
    EpicGreatSmiting2 = 825,
    EpicGreatSmiting3 = 826,
    EpicGreatSmiting4 = 827,
    EpicGreatSmiting5 = 828,
    EpicGreatSmiting6 = 829,
    EpicGreatSmiting7 = 830,
    EpicGreatSmiting8 = 831,
    EpicGreatSmiting9 = 832,
    EpicGreatSmiting10 = 833,
    EpicImprovedSneakAttack1 = 834,
    EpicImprovedSneakAttack2 = 835,
    EpicImprovedSneakAttack3 = 836,
    EpicImprovedSneakAttack4 = 837,
    EpicImprovedSneakAttack5 = 838,
    EpicImprovedSneakAttack6 = 839,
    EpicImprovedSneakAttack7 = 840,
    EpicImprovedSneakAttack8 = 841,
    EpicImprovedSneakAttack9 = 842,
    EpicImprovedSneakAttack10 = 843,
    EpicImprovedStunningFist1 = 844,
    EpicImprovedStunningFist2 = 845,
    EpicImprovedStunningFist3 = 846,
    EpicImprovedStunningFist4 = 847,
    EpicImprovedStunningFist5 = 848,
    EpicImprovedStunningFist6 = 849,
    EpicImprovedStunningFist7 = 850,
    EpicImprovedStunningFist8 = 851,
    EpicImprovedStunningFist9 = 852,
    EpicImprovedStunningFist10 = 853,
    EpicPlanarTurning = 854,
    EpicBaneOfEnemies = 855,
    EpicDodge = 856,
    EpicAutomaticQuicken1 = 857,
    EpicAutomaticQuicken2 = 858,
    EpicAutomaticQuicken3 = 859,
    EpicAutomaticSilentSpell1 = 860,
    EpicAutomaticSilentSpell2 = 861,
    EpicAutomaticSilentSpell3 = 862,
    EpicAutomaticStillSpell1 = 863,
    EpicAutomaticStillSpell2 = 864,
    EpicAutomaticStillSpell3 = 865,
    MartialFlurryLight = 866,
    WhirlwindAttack = 867,
    ImprovedWhirlwind = 868,
    MightyRage = 869,
    EpicLastingInspiration = 870,
    CurseSong = 871,
    WildShapeUndead = 872,
    WildShapeDragon = 873,
    EpicSpellMummyDust = 874,
    EpicSpellDragonKnight = 875,
    EpicSpellHellball = 876,
    EpicSpellMageArmour = 877,
    EpicSpellRuin = 878,
    WeaponOfChoice_Sickle = 879,
    WeaponOfChoice_Kama = 880,
    WeaponOfChoice_Kukri = 881,
    KiDamage = 882,
    IncreaseMultiplier = 883,
    SuperiorWeaponFocus = 884,
    KiCritical = 885,
    BoneSkin2 = 886,
    AnimateDead = 889,
    SummonUndead = 890,
    DeathlessVigor = 891,
    UndeadGraft1 = 892,
    UndeadGraft2 = 893,
    ToughAsBone = 894,
    SummonGreaterUndead = 895,
    DeathlessMastery = 896,
    DeathlessMasterTouch = 897,
    GreaterWildshape1 = 898,
    MartialFlurryAny = 899,
    GreaterWildshape2 = 900,
    GreaterWildshape3 = 901,
    HumanoidShape = 902,
    GreaterWildshape4 = 903,
    SacredDefense1 = 904,
    SacredDefense2 = 905,
    SacredDefense3 = 906,
    SacredDefense4 = 907,
    SacredDefense5 = 908,
    DivineWrath = 909,
    ExtraSmiting = 910,
    SkillFocus_CraftArmor = 911,
    SkillFocus_CraftWeapon = 912,
    EpicSkillFocus_CraftArmor = 913,
    EpicSkillFocus_CraftWeapon = 914,
    SkillFocus_Bluff = 915,
    SkillFocus_Intimidate = 916,
    EpicSkillFocus_Bluff = 917,
    EpicSkillFocus_Intimidate = 918,
    WeaponOfChoice_Club = 919,
    WeaponOfChoice_Dagger = 920,
    WeaponOfChoice_Lightmace = 921,
    WeaponOfChoice_Morningstar = 922,
    WeaponOfChoice_Quarterstaff = 923,
    WeaponOfChoice_Shortspear = 924,
    WeaponOfChoice_Shortsword = 925,
    WeaponOfChoice_Rapier = 926,
    WeaponOfChoice_Scimitar = 927,
    WeaponOfChoice_Longsword = 928,
    WeaponOfChoice_Greatsword = 929,
    WeaponOfChoice_Handaxe = 930,
    WeaponOfChoice_Battleaxe = 931,
    WeaponOfChoice_Greataxe = 932,
    WeaponOfChoice_Halberd = 933,
    WeaponOfChoice_Lighthammer = 934,
    WeaponOfChoice_Lightflail = 935,
    WeaponOfChoice_Warhammer = 936,
    WeaponOfChoice_Heavyflail = 937,
    WeaponOfChoice_Scythe = 938,
    WeaponOfChoice_Katana = 939,
    WeaponOfChoice_Bastardsword = 940,
    WeaponOfChoice_Diremace = 941,
    WeaponOfChoice_Doubleaxe = 942,
    WeaponOfChoice_Twobladedsword = 943,
    BrewPotion = 944,
    ScribeScroll = 945,
    CraftWand = 946,
    DwarvenDefenderDefensiveStance = 947,
    DamageReduction6 = 948,
    PrestigeDefensiveAwareness1 = 949,
    PrestigeDefensiveAwareness2 = 950,
    PrestigeDefensiveAwareness3 = 951,
    WeaponFocus_Dwaxe = 952,
    WeaponSpecialization_Dwaxe = 953,
    ImprovedCritical_Dwaxe = 954,
    EpicDevastatingCritical_Dwaxe = 955,
    EpicWeaponFocus_Dwaxe = 956,
    EpicWeaponSpecialization_Dwaxe = 957,
    EpicOverwhelmingCritical_Dwaxe = 958,
    WeaponOfChoice_Dwaxe = 959,
    UsePoison = 960,
    DragonArmor = 961,
    DragonAbilities = 962,
    DragonImmuneParalysis = 963,
    DragonImmuneFire = 964,
    DragonDisBreath = 965,
    EpicFighter = 966,
    EpicBarbarian = 967,
    EpicBard = 968,
    EpicCleric = 969,
    EpicDruid = 970,
    EpicMonk = 971,
    EpicPaladin = 972,
    EpicRanger = 973,
    EpicRogue = 974,
    EpicSorcerer = 975,
    EpicWizard = 976,
    EpicArcaneArcher = 977,
    EpicAssassin = 978,
    EpicBlackguard = 979,
    EpicShadowdancer = 980,
    EpicHarperScout = 981,
    EpicDivineChampion = 982,
    EpicWeaponMaster = 983,
    EpicPaleMaster = 984,
    EpicDwarvenDefender = 985,
    EpicShifter = 986,
    EpicRedDragonDisc = 987,
    EpicThunderingRage = 988,
    EpicTerrifyingRage = 989,
    EpicEpicWarding = 990,
    PrestigeMasterCrafter = 991,
    PrestigeScrounger = 992,
    WeaponFocus_Whip = 993,
    WeaponSpecialization_Whip = 994,
    ImprovedCritical_Whip = 995,
    EpicDevastatingCritical_Whip = 996,
    EpicWeaponFocus_Whip = 997,
    EpicWeaponSpecialization_Whip = 998,
    EpicOverwhelmingCritical_Whip = 999,
    WeaponOfChoice_Whip = 1000,
    EpicCharacter = 1001,
    EpicEpicShadowlord = 1002,
    EpicEpicFiend = 1003,
    PrestigeDeathAttack6 = 1004,
    PrestigeDeathAttack7 = 1005,
    PrestigeDeathAttack8 = 1006,
    BlackguardSneakAttack4d6 = 1007,
    BlackguardSneakAttack5d6 = 1008,
    BlackguardSneakAttack6d6 = 1009,
    BlackguardSneakAttack7d6 = 1010,
    BlackguardSneakAttack8d6 = 1011,
    BlackguardSneakAttack9d6 = 1012,
    BlackguardSneakAttack10d6 = 1013,
    BlackguardSneakAttack11d6 = 1014,
    BlackguardSneakAttack12d6 = 1015,
    BlackguardSneakAttack13d6 = 1016,
    BlackguardSneakAttack14d6 = 1017,
    BlackguardSneakAttack15d6 = 1018,
    PrestigeDeathAttack9 = 1019,
    PrestigeDeathAttack10 = 1020,
    PrestigeDeathAttack11 = 1021,
    PrestigeDeathAttack12 = 1022,
    PrestigeDeathAttack13 = 1023,
    PrestigeDeathAttack14 = 1024,
    PrestigeDeathAttack15 = 1025,
    PrestigeDeathAttack16 = 1026,
    PrestigeDeathAttack17 = 1027,
    PrestigeDeathAttack18 = 1028,
    PrestigeDeathAttack19 = 1029,
    PrestigeDeathAttack20 = 1030,
    SneakAttack11 = 1032,
    SneakAttack12 = 1033,
    SneakAttack13 = 1034,
    SneakAttack14 = 1035,
    SneakAttack15 = 1036,
    SneakAttack16 = 1037,
    SneakAttack17 = 1038,
    SneakAttack18 = 1039,
    SneakAttack19 = 1040,
    SneakAttack20 = 1041,
    DragonHdincreaseD6 = 1042,
    DragonHdincreaseD8 = 1043,
    DragonHdincreaseD10 = 1044,
    PrestigeEnchantArrow6 = 1045,
    PrestigeEnchantArrow7 = 1046,
    PrestigeEnchantArrow8 = 1047,
    PrestigeEnchantArrow9 = 1048,
    PrestigeEnchantArrow10 = 1049,
    PrestigeEnchantArrow11 = 1050,
    PrestigeEnchantArrow12 = 1051,
    PrestigeEnchantArrow13 = 1052,
    PrestigeEnchantArrow14 = 1053,
    PrestigeEnchantArrow15 = 1054,
    PrestigeEnchantArrow16 = 1055,
    PrestigeEnchantArrow17 = 1056,
    PrestigeEnchantArrow18 = 1057,
    PrestigeEnchantArrow19 = 1058,
    PrestigeEnchantArrow20 = 1059,
    EpicOutsiderShape = 1060,
    EpicConstructShape = 1061,
    EpicShifterInfiniteWildshape1 = 1062,
    EpicShifterInfiniteWildshape2 = 1063,
    EpicShifterInfiniteWildshape3 = 1064,
    EpicShifterInfiniteWildshape4 = 1065,
    EpicShifterInfiniteHumanoidShape = 1066,
    EpicBarbarianDamageReduction = 1067,
    EpicDruidInfiniteWildshape = 1068,
    EpicDruidInfiniteElementalShape = 1069,
    PrestigePoisonSaveEpic = 1070,
    EpicSuperiorWeaponFocus_ = 1071,
    WeaponFocus_Trident = 1072,
    WeaponSpecialization_Trident = 1073,
    ImprovedCritical_Trident = 1074,
    EpicDevastatingCritical_Trident = 1075,
    EpicWeaponFocus_Trident = 1076,
    EpicWeaponSpecialization_Trident = 1077,
    EpicOverwhelmingCritical_Trident = 1078,
    WeaponOfChoice_Trident = 1079,
    PdkRally = 1080,
    PdkShield = 1081,
    PdkFear = 1082,
    PdkWrath = 1083,
    PdkStand = 1084,
    PdkInspire1 = 1085,
    PdkInspire2 = 1086,
    MountedCombat = 1087,
    MountedArchery = 1088,
    HorseMenu = 1089,
    HorseMount = 1090,
    HorseDismount = 1091,
    HorsePartyMount = 1092,
    HorseGatherMounts = 1093,
    HorseAssignMount = 1094,
    PaladinSummonMount = 1095,
    DmTool01 = 1096,
    DmTool02 = 1097,
    DmTool03 = 1098,
    DmTool04 = 1099,
    DmTool05 = 1100,
    DmTool06 = 1101,
    DmTool07 = 1102,
    DmTool08 = 1103,
    DmTool09 = 1104,
    DmTool10 = 1105,
    PlayerTool01 = 1106,
    PlayerTool02 = 1107,
    PlayerTool03 = 1108,
    PlayerTool04 = 1109,
    PlayerTool05 = 1110,
    PlayerTool06 = 1111,
    PlayerTool07 = 1112,
    PlayerTool08 = 1113,
    PlayerTool09 = 1114,
    PlayerTool10 = 1115,
    LanguageElf = 1116,
    LanguageDwarf = 1117,
    LanguageOrc = 1118,
    LanguageGiant = 1119,
    LanguageGoblin = 1120,
    LanguageHalfling = 1121,
    LanguageAbyssal = 1122,
    LanguageCelestial = 1123,
    LanguageDraconic = 1124,
    LanguageDeep = 1125,
    LanguageInfernal = 1126,
    LanguagePrimodial = 1127,
    LanguageSylvan = 1128,
    LanguageDruidic = 1129,
    LanguageThieves = 1130,
    STRENGTH_LOSS = 1131,
    test = 1132,
    test2 = 1133,
    test3 = 1134,
    test4 = 1135,
    test5 = 1136,
    INVALID_FEAT = 65535
  }
}
