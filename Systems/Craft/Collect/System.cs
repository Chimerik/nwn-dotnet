using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.Craft.Collect.Config;
using Anvil.API;
using NLog;
using System.Linq;
using Action = System.Action;
using System.Threading.Tasks;
using Anvil.API.Events;
using System.Threading;

namespace NWN.Systems.Craft.Collect
{
  public static class System
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public static string[] badPelts = new string[] { "paraceratherium", "ankheg", "gorille", "giantlizard" };
    public static string[] commonPelts = new string[] { "alligator", "crocodile", "crocblinde", "varan" };
    public static string[] normalPelts = new string[] { "basilisk", "jhakar", "gorgon", "bulette", "dagon" };

    public static CraftResource[] craftResourceArray = new CraftResource[] 
    { 
      new CraftResource(ResourceType.Ore, "Un phénomène mystérieux provoque l'agglomération de Substance à certains minerais bruts, qu'on appelle alors 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 82, 1, new decimal(0.5)),
      new CraftResource(ResourceType.Ore, "Un phénomène mystérieux provoque l'agglomération de Substance à certains minerais bruts, qu'on appelle alors 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 82, 2, new decimal(0.5)),
      new CraftResource(ResourceType.Ore, "Un phénomène mystérieux provoque l'agglomération de Substance à certains minerais bruts, qu'on appelle alors 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 82, 3, new decimal(0.5)),
      new CraftResource(ResourceType.Ore, "Un phénomène mystérieux provoque l'agglomération de Substance à certains minerais bruts, qu'on appelle alors 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 82, 4, new decimal(0.5)),
      new CraftResource(ResourceType.Ore, "Un phénomène mystérieux provoque l'agglomération de Substance à certains minerais bruts, qu'on appelle alors 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 82, 5, new decimal(0.5)),
      new CraftResource(ResourceType.Ore, "Un phénomène mystérieux provoque l'agglomération de Substance à certains minerais bruts, qu'on appelle alors 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 82, 6, new decimal(0.5)),
      new CraftResource(ResourceType.Ore, "Un phénomène mystérieux provoque l'agglomération de Substance à certains minerais bruts, qu'on appelle alors 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 82, 7, new decimal(0.5)),
      new CraftResource(ResourceType.Ore, "Un phénomène mystérieux provoque l'agglomération de Substance à certains minerais bruts, qu'on appelle alors 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 82, 8, new decimal(0.5)),
      new CraftResource(ResourceType.Ingot, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes lingots de métal gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 106, 1, new decimal(0.2)),
      new CraftResource(ResourceType.Ingot, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes lingots de métal gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 106, 2, new decimal(0.2)),
      new CraftResource(ResourceType.Ingot, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes lingots de métal gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 106, 3, new decimal(0.2)),
      new CraftResource(ResourceType.Ingot, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes lingots de métal gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 106, 4, new decimal(0.2)),
      new CraftResource(ResourceType.Ingot, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes lingots de métal gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 106, 5, new decimal(0.2)),
      new CraftResource(ResourceType.Ingot, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes lingots de métal gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 106, 6, new decimal(0.2)),
      new CraftResource(ResourceType.Ingot, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes lingots de métal gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 106, 7, new decimal(0.2)),
      new CraftResource(ResourceType.Ingot, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes lingots de métal gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 106, 8, new decimal(0.2)),
      new CraftResource(ResourceType.Wood, "Un phénomène mystérieux provoque l'agglomération de Substance à certains arbres. Le bois que l'on en retire est alors appelé 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 114, 1, new decimal(0.4)),
      new CraftResource(ResourceType.Wood, "Un phénomène mystérieux provoque l'agglomération de Substance à certains arbres. Le bois que l'on en retire est alors appelé 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 114, 2, new decimal(0.4)),
      new CraftResource(ResourceType.Wood, "Un phénomène mystérieux provoque l'agglomération de Substance à certains arbres. Le bois que l'on en retire est alors appelé 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 114, 3, new decimal(0.4)),
      new CraftResource(ResourceType.Wood, "Un phénomène mystérieux provoque l'agglomération de Substance à certains arbres. Le bois que l'on en retire est alors appelé 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 114, 4, new decimal(0.4)),
      new CraftResource(ResourceType.Wood, "Un phénomène mystérieux provoque l'agglomération de Substance à certains arbres. Le bois que l'on en retire est alors appelé 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 114, 5, new decimal(0.4)),
      new CraftResource(ResourceType.Wood, "Un phénomène mystérieux provoque l'agglomération de Substance à certains arbres. Le bois que l'on en retire est alors appelé 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 114, 6, new decimal(0.4)),
      new CraftResource(ResourceType.Wood, "Un phénomène mystérieux provoque l'agglomération de Substance à certains arbres. Le bois que l'on en retire est alors appelé 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 114, 7, new decimal(0.4)),
      new CraftResource(ResourceType.Wood, "Un phénomène mystérieux provoque l'agglomération de Substance à certains arbres. Le bois que l'on en retire est alors appelé 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 114, 8, new decimal(0.4)),
      new CraftResource(ResourceType.Plank, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes planches de bois gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 117, 1, new decimal(0.3)),
      new CraftResource(ResourceType.Plank, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes planches de bois gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 117, 2, new decimal(0.3)),
      new CraftResource(ResourceType.Plank, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes planches de bois gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 117, 3, new decimal(0.3)),
      new CraftResource(ResourceType.Plank, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes planches de bois gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 117, 4, new decimal(0.3)),
      new CraftResource(ResourceType.Plank, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes planches de bois gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 117, 5, new decimal(0.3)),
      new CraftResource(ResourceType.Plank, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes planches de bois gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 117, 6, new decimal(0.3)),
      new CraftResource(ResourceType.Plank, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes planches de bois gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 117, 7, new decimal(0.3)),
      new CraftResource(ResourceType.Plank, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes planches de bois gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 117, 8, new decimal(0.3)),
      new CraftResource(ResourceType.Pelt, "Un phénomène mystérieux provoque l'agglomération de Substance chez certaines créatures aquatiques. Les peaux que l'on en retire sont alors appelées 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 6, 1, new decimal(0.3)),
      new CraftResource(ResourceType.Pelt, "Un phénomène mystérieux provoque l'agglomération de Substance chez certaines créatures aquatiques. Les peaux que l'on en retire sont alors appelées 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 6, 2, new decimal(0.3)),
      new CraftResource(ResourceType.Pelt, "Un phénomène mystérieux provoque l'agglomération de Substance chez certaines créatures aquatiques. Les peaux que l'on en retire sont alors appelées 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 6, 3, new decimal(0.3)),
      new CraftResource(ResourceType.Pelt, "Un phénomène mystérieux provoque l'agglomération de Substance chez certaines créatures aquatiques. Les peaux que l'on en retire sont alors appelées 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 6, 4, new decimal(0.3)),
      new CraftResource(ResourceType.Pelt, "Un phénomène mystérieux provoque l'agglomération de Substance chez certaines créatures aquatiques. Les peaux que l'on en retire sont alors appelées 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 6, 5, new decimal(0.3)),
      new CraftResource(ResourceType.Pelt, "Un phénomène mystérieux provoque l'agglomération de Substance chez certaines créatures aquatiques. Les peaux que l'on en retire sont alors appelées 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 6, 6, new decimal(0.3)),
      new CraftResource(ResourceType.Pelt, "Un phénomène mystérieux provoque l'agglomération de Substance chez certaines créatures aquatiques. Les peaux que l'on en retire sont alors appelées 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 6, 7, new decimal(0.3)),
      new CraftResource(ResourceType.Pelt, "Un phénomène mystérieux provoque l'agglomération de Substance chez certaines créatures aquatiques. Les peaux que l'on en retire sont alors appelées 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 6, 8, new decimal(0.3)),
      new CraftResource(ResourceType.Pelt, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes cuirs tannés et gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 87, 1, new decimal(0.2)),
      new CraftResource(ResourceType.Pelt, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes cuirs tannés et gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 87, 2, new decimal(0.2)),
      new CraftResource(ResourceType.Pelt, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes cuirs tannés et gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 87, 3, new decimal(0.2)),
      new CraftResource(ResourceType.Pelt, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes cuirs tannés et gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 87, 4, new decimal(0.2)),
      new CraftResource(ResourceType.Pelt, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes cuirs tannés et gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 87, 5, new decimal(0.2)),
      new CraftResource(ResourceType.Pelt, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes cuirs tannés et gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 87, 6, new decimal(0.2)),
      new CraftResource(ResourceType.Pelt, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes cuirs tannés et gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 87, 7, new decimal(0.2)),
      new CraftResource(ResourceType.Pelt, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes cuirs tannés et gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 87, 8, new decimal(0.2)),
    }; 

    public static Dictionary<int, Feat> craftBaseItemFeatDictionnary = new Dictionary<int, Feat>()
    {
      {-13, CustomFeats.Research },
      {-12, CustomFeats.Metallurgy },
      {-11, CustomFeats.BlueprintCopy },
      {-9, CustomFeats.CraftClothing },
      {-8, CustomFeats.CraftFullPlate },
      {-7, CustomFeats.CraftHalfPlate },
      {-6, CustomFeats.CraftSplintMail },
      {-5, CustomFeats.CraftBreastPlate },
      {-4, CustomFeats.CraftScaleMail },
      {-3, CustomFeats.CraftStuddedLeather },
      {-2, CustomFeats.CraftLeatherArmor },
      {-1, CustomFeats.CraftPaddedArmor },
      {0, CustomFeats.CraftShortsword },
      {1, CustomFeats.CraftLongsword },
      {2, CustomFeats.CraftBattleAxe },
      {3, CustomFeats.CraftBastardSword },
      {4, CustomFeats.CraftLightFlail },
      {5, CustomFeats.CraftWarHammer },
      {6, CustomFeats.CraftHeavyCrossbow },
      {7, CustomFeats.CraftLightCrossbow },
      {8, CustomFeats.CraftLongBow },
      {9, CustomFeats.CraftLightMace },
      {10, CustomFeats.CraftHalberd },
      {11, CustomFeats.CraftShortBow },
      {12, CustomFeats.CraftTwoBladedSword },
      {13, CustomFeats.CraftGreatSword },
      {14, CustomFeats.CraftSmallShield },
      {15, CustomFeats.CraftTorch },
      {17, CustomFeats.CraftHelmet },
      {18, CustomFeats.CraftGreatAxe },
      {19, CustomFeats.CraftAmulet },
      {20, CustomFeats.CraftArrow },
      {21, CustomFeats.CraftBelt },
      {22, CustomFeats.CraftDagger },
      {25, CustomFeats.CraftBolt },
      {26, CustomFeats.CraftBoots },
      {27, CustomFeats.CraftBullets },
      {28, CustomFeats.CraftClub },
      {31, CustomFeats.CraftDarts },
      {32, CustomFeats.CraftDireMace },
      {33, CustomFeats.CraftDoubleAxe },
      {35, CustomFeats.CraftHeavyFlail },
      {36, CustomFeats.CraftGloves },
      {37, CustomFeats.CraftLightHammer },
      {38, CustomFeats.CraftHandAxe },
      {40, CustomFeats.CraftKama },
      {41, CustomFeats.CraftKatana },
      {42, CustomFeats.CraftKukri },
      {44, CustomFeats.CraftMagicRod },
      {45, CustomFeats.CraftStaff },
      {46, CustomFeats.CraftMagicWand },
      {47, CustomFeats.CraftMorningStar },
      {49, CustomFeats.CraftPotion },
      {50, CustomFeats.CraftQuarterstaff },
      {51, CustomFeats.CraftRapier },
      {52, CustomFeats.CraftRing },
      {53, CustomFeats.CraftScimitar },
      {55, CustomFeats.CraftScythe },
      {56, CustomFeats.CraftLargeShield },
      {57, CustomFeats.CraftTowerShield },
      {58, CustomFeats.CraftShortSpear },
      {59, CustomFeats.CraftShuriken },
      {60, CustomFeats.CraftSickle },
      {61, CustomFeats.CraftSling },
      {63, CustomFeats.CraftThrowingAxe },
      {75, CustomFeats.CraftSpellScroll },
      {78, CustomFeats.CraftBracer },
      {80, CustomFeats.CraftCloak },
      {95, CustomFeats.CraftTrident },
      {108, CustomFeats.CraftDwarvenWarAxe },
      {111, CustomFeats.CraftWhip },
      {114, CustomFeats.CraftForgeHammer },
      {115, CustomFeats.CraftOreExtractor },
    };

    public static int[] lowArmorBlueprints = new int[] { 0, 1, 2, 3 };
    public static BaseItemType[] lowWeaponBlueprints = new BaseItemType[] { BaseItemType.Whip, BaseItemType.LightMace, BaseItemType.Helmet, BaseItemType.Dagger, BaseItemType.Morningstar, BaseItemType.ShortSpear, BaseItemType.Sickle, BaseItemType.Arrow, BaseItemType.Belt, BaseItemType.Amulet, BaseItemType.Bolt, BaseItemType.Boots, BaseItemType.Bracer, BaseItemType.Bullet, BaseItemType.Cloak, BaseItemType.Club, BaseItemType.Dart, BaseItemType.Gloves, BaseItemType.HeavyCrossbow, BaseItemType.LightCrossbow, BaseItemType.Quarterstaff, BaseItemType.Ring, BaseItemType.Shuriken, BaseItemType.Sling, BaseItemType.SmallShield, BaseItemType.Torch };
    public static int[] mediumArmorBlueprints = new int[] { 4, 5 };
    public static BaseItemType[] mediumWeaponBlueprints = new BaseItemType[] { BaseItemType.Battleaxe, BaseItemType.Greatsword, BaseItemType.Greataxe, BaseItemType.Halberd, BaseItemType.Handaxe, BaseItemType.HeavyFlail, BaseItemType.LargeShield, BaseItemType.LightFlail, BaseItemType.LightHammer, BaseItemType.LightMace, BaseItemType.Longbow, BaseItemType.Longsword, BaseItemType.Rapier, BaseItemType.Scimitar, BaseItemType.Shortbow, BaseItemType.Shortsword, BaseItemType.Shuriken, BaseItemType.ThrowingAxe, BaseItemType.Trident, BaseItemType.Warhammer };
    public static int[] highArmorBlueprints = new int[] { 6, 7, 8 };
    public static BaseItemType[] highWeapônBlueprints = new BaseItemType[] { BaseItemType.TwoBladedSword, BaseItemType.TowerShield, BaseItemType.Scythe, BaseItemType.Kukri, BaseItemType.Katana, BaseItemType.Kama, BaseItemType.DwarvenWaraxe, BaseItemType.DireMace, BaseItemType.Doubleaxe, BaseItemType.Bastardsword };

    public static async void UpdateResourceBlockInfo(NwPlaceable resourceBlock)
    {
      if (resourceBlock.GetObjectVariable<DateTimeLocalVariable>("_LAST_CHECK").HasNothing)
        resourceBlock.GetObjectVariable<DateTimeLocalVariable>("_LAST_CHECK").Value = DateTime.Now.AddDays(-3);

      double totalSeconds = (DateTime.Now - resourceBlock.GetObjectVariable<DateTimeLocalVariable>("_LAST_CHECK").Value).TotalSeconds;
      double materiaGrowth = totalSeconds / (5 * resourceBlock.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value);
      resourceBlock.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value += (int)materiaGrowth;
      resourceBlock.GetObjectVariable<DateTimeLocalVariable>("_LAST_CHECK").Value = DateTime.Now;

      string resourceId = resourceBlock.GetObjectVariable<LocalVariableInt>("id").Value.ToString();
      string areaTag = resourceBlock.Area.Tag;
      string resourceType = resourceBlock.GetObjectVariable<LocalVariableString>("_RESOURCE_TYPE").Value;
      string resourceQuantity = resourceBlock.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value.ToString();

      await SqLiteUtils.InsertQueryAsync("areaResourceStock",
        new List<string[]>() { new string[] { "id", resourceId }, new string[] { "areaTag", areaTag }, new string[] { "type", resourceType }, new string[] { "quantity", resourceQuantity }, new string[] { "lastChecked", DateTime.Now.ToString() } },
        new List<string>() { "id", "areaTag", "type" },
        new List<string[]>() { new string[] { "quantity" }, new string[] { "lastChecked" } });
    }

    public static int GetResourceDetectionTime(PlayerSystem.Player player, int detectionSkill, int speedSkill)
    {
      int scanDuration = 120;
      scanDuration -= scanDuration * (int)(player.learnableSkills[detectionSkill].totalPoints * 0.05);
      scanDuration -= player.learnableSkills.ContainsKey(speedSkill) ? scanDuration * (int)(player.learnableSkills[speedSkill].totalPoints * 0.05) : 0;
      return scanDuration;
    }
    public static async void CreateSelectedResourceInInventory(CraftResource selection, PlayerSystem.Player player, int quantity)
    {
      NwItem pcResource = await NwItem.Create("craft_resource", player.oid.LoginCreature);
      pcResource.GetObjectVariable<LocalVariableString>("CRAFT_RESOURCE").Value = selection.type.ToString();
      pcResource.GetObjectVariable<LocalVariableInt>("CRAFT_GRADE").Value = selection.grade;
      pcResource.Name = selection.name;
      pcResource.Description = selection.description;
      pcResource.Weight = selection.weight;
      pcResource.Appearance.SetSimpleModel(selection.icon);
      pcResource.StackSize = quantity;
    }

    public static void StartCollectCycle(PlayerSystem.Player player, Action completeCallback, NwGameObject oTarget = null)
    {
      if (player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_COLLECT_IN_PROGRESS").HasValue)
      {
        player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_COLLECT_CANCELLED").Value = 1;
        player.oid.SendServerMessage("Annulation de la tâche en cours.", ColorConstants.Orange);
        return;
      }
      
      NwItem resourceExtractor = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
      float cycleDuration = 180.0f;
      if (Systems.Config.env == Systems.Config.Env.Chim)
        cycleDuration = 10.0f;
      
      if (resourceExtractor != null) // TODO : Idée pour plus tard, le strip miner le plus avancé pourra équipper un cristal de spécialisation pour extraire deux fois plus de minerai en un cycle sur son minerai de spécialité
      {
        cycleDuration = cycleDuration - (cycleDuration * resourceExtractor.GetObjectVariable<LocalVariableInt>("_ITEM_LEVEL").Value * 2 / 100);
      }
      
      if (oTarget != null)
      {
        Effect eRay = Effect.Beam(VfxType.BeamDisintegrate, resourceExtractor, BodyNode.Hand);
        eRay.Tag = $"_{player.oid.CDKey}_MINING_BEAM";
        oTarget.ApplyEffect(EffectDuration.Temporary, eRay, TimeSpan.FromSeconds(cycleDuration));
      }
     
      PlayerPlugin.StartGuiTimingBar(player.oid.LoginCreature, cycleDuration);

      player.oid.OnClientDisconnect += OnDisconnectCancelCollect;
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_TIMING_BAR_CANCEL_BEFORE", "collect_cancel", player.oid.LoginCreature);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_ITEM_EQUIP_BEFORE", "collect_cancel", player.oid.LoginCreature);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_ITEM_UNEQUIP_BEFORE", "collect_cancel", player.oid.LoginCreature);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_START_COMBAT_ROUND_AFTER", "collect_cancel", player.oid.LoginCreature);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "collect_cancel", player.oid.LoginCreature);
      
      Task waitForCollectCompletion = NwTask.Run(async () =>
      {
        player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_COLLECT_IN_PROGRESS").Value = 1;

        CancellationTokenSource tokenSource = new CancellationTokenSource();
        Task collectCancelled = NwTask.WaitUntil(() => player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_COLLECT_CANCELLED").Value == 1, tokenSource.Token);
        Task onMovementCancelCollect = NwTask.WaitUntilValueChanged(() => player.oid.LoginCreature.Position, tokenSource.Token);
        Task collectCompleted = NwTask.Delay(TimeSpan.FromSeconds(cycleDuration), tokenSource.Token);
        await NwTask.WhenAny(collectCancelled, onMovementCancelCollect, collectCompleted);
        tokenSource.Cancel();

        if (collectCancelled.IsCompletedSuccessfully || onMovementCancelCollect.IsCompletedSuccessfully)
        {
          player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_COLLECT_CANCELLED").Delete();

          if (oTarget != null)
            Utils.RemoveTaggedEffect(oTarget, $"_{player.oid.CDKey}_MINING_BEAM");

          RemoveCollectCycleCallbacks(player.oid);
          PlayerPlugin.StopGuiTimingBar(player.oid.LoginCreature);
    
          player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_COLLECT_IN_PROGRESS").Delete();
          return;
        }

        completeCallback();
        RemoveCollectCycleCallbacks(player.oid);
        PlayerPlugin.StopGuiTimingBar(player.oid.LoginCreature);
        player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_COLLECT_IN_PROGRESS").Delete();
      });
    }
    private static void OnDisconnectCancelCollect(OnClientDisconnect onDisconnect)
    {
      onDisconnect.Player.LoginCreature.GetObjectVariable<LocalVariableInt>("_COLLECT_CANCELLED").Delete();
      onDisconnect.Player.LoginCreature.GetObjectVariable<LocalVariableInt>("_COLLECT_IN_PROGRESS").Delete();
      RemoveCollectCycleCallbacks(onDisconnect.Player);
      PlayerPlugin.StopGuiTimingBar(onDisconnect.Player.LoginCreature);
    }
    private static void RemoveCollectCycleCallbacks(NwPlayer oPC)
    {
      oPC.OnClientDisconnect -= OnDisconnectCancelCollect;
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_TIMING_BAR_CANCEL_BEFORE", "collect_cancel", oPC.LoginCreature);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_ITEM_EQUIP_BEFORE", "collect_cancel", oPC.LoginCreature);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_ITEM_UNEQUIP_BEFORE", "collect_cancel", oPC.LoginCreature);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_START_COMBAT_ROUND_AFTER", "collect_cancel", oPC.LoginCreature);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "collect_cancel", oPC.LoginCreature);
    }
    public static void AddCraftedItemProperties(NwItem craftedItem, int grade)
    {
      craftedItem.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").Value = grade;

      foreach (ItemProperty ip in GetCraftItemProperties(craftedItem, grade))
      {
        ItemProperty existingIP = craftedItem.ItemProperties.FirstOrDefault(i => i.DurationType == EffectDuration.Permanent && i.PropertyType == ip.PropertyType && i.SubType == ip.SubType && i.Param1Table == ip.Param1Table);

        if (existingIP != null)
        {
          craftedItem.RemoveItemProperty(existingIP);

          if (ip.PropertyType == ItemPropertyType.DamageBonus
            || ip.PropertyType == ItemPropertyType.DamageBonusVsAlignmentGroup
            || ip.PropertyType == ItemPropertyType.DamageBonusVsRacialGroup
            || ip.PropertyType == ItemPropertyType.DamageBonusVsSpecificAlignment)
          {
            int newRank = ItemPropertyDamageCost2da.ipDamageCost.GetRankFromCostValue(ip.CostTableValue);
            int existingRank = ItemPropertyDamageCost2da.ipDamageCost.GetRankFromCostValue(existingIP.CostTableValue);

            if (existingRank > newRank)
              newRank = existingRank + 1;
            else
              newRank += 1;

            ip.CostTableValue = ItemPropertyDamageCost2da.ipDamageCost.GetDamageCostValueFromRank(newRank);
          }
          else if (ip.PropertyType == ItemPropertyType.AcBonus
            || ip.PropertyType == ItemPropertyType.AcBonusVsAlignmentGroup
            || ip.PropertyType == ItemPropertyType.AcBonusVsDamageType
            || ip.PropertyType == ItemPropertyType.AcBonusVsRacialGroup
            || ip.PropertyType == ItemPropertyType.AcBonusVsSpecificAlignment
            || ip.PropertyType == ItemPropertyType.AttackBonus
            || ip.PropertyType == ItemPropertyType.AttackBonusVsAlignmentGroup
            || ip.PropertyType == ItemPropertyType.AttackBonusVsRacialGroup
            || ip.PropertyType == ItemPropertyType.AttackBonusVsSpecificAlignment)
          {
            ip.CostTableValue += existingIP.CostTableValue;
          }
          else
          {
            if (existingIP.CostTableValue > ip.CostTableValue)
              ip.CostTableValue = existingIP.CostTableValue + 1;
            else
              ip.CostTableValue += 1;
          }
        }
        
        craftedItem.AddItemProperty(ip, EffectDuration.Permanent);
      }
    }
    public static bool IsItemCraftMaterial(string itemTag)
    {
      if (Enum.TryParse(itemTag, out OreType myOreType) && myOreType != OreType.Invalid)
        return true;
      if (Enum.TryParse(itemTag, out MineralType myMineralType) && myMineralType != MineralType.Invalid)
        return true;
      if (Enum.TryParse(itemTag, out WoodType myWoodType) && myWoodType != WoodType.Invalid)
        return true;
      if (Enum.TryParse(itemTag, out PlankType myPlankType) && myPlankType != PlankType.Invalid)
        return true;
      if (Array.FindIndex(badPelts, x => x == itemTag) > -1 || Array.FindIndex(commonPelts, x => x == itemTag) > -1
        || Array.FindIndex(normalPelts, x => x == itemTag) > -1)
        return true;

      return false;
    }
    public static string GetCraftMaterialItemTemplate(string itemTag)
    {
      if (Enum.TryParse(itemTag, out OreType myOreType) && myOreType != OreType.Invalid)
        return "ore";
      if (Enum.TryParse(itemTag, out MineralType myMineralType) && myMineralType != MineralType.Invalid)
        return "mineral";
      if (Enum.TryParse(itemTag, out WoodType myWoodType) && myWoodType != WoodType.Invalid)
        return "wood";
      if (Enum.TryParse(itemTag, out PlankType myPlankType) && myPlankType != PlankType.Invalid)
        return "plank";
      if (Array.FindIndex(badPelts, x => x == itemTag) > -1 || Array.FindIndex(commonPelts, x => x == itemTag) > -1
        || Array.FindIndex(normalPelts, x => x == itemTag) > -1)
        return "pelt";

      Utils.LogMessageToDMs($"Could not find item template for tag : {itemTag}");
      return "";
    }
  }
}
