using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using NWN.Core;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AreaSystem))]
  partial class AreaSystem
  {
    private readonly Dictionary<string, int[]> randomAppearanceDictionary = new ()
    {
      { "plage", new int[] { 1984, 1985, 1986, 3160, 3155, 3156, 1956, 1957, 1958, 1959, 1960, 1961, 1962, 291, 292, 1964, 4310, 1428, 1430, 1980, 1981, 3261, 3262, 3263 } },
      { "cave", new int[] { 3197, 3198, 3199, 3200, 3202, 3204, 3205, 3206, 3207, 3208, 3209, 3210, 3999, 6425, 6426, 6427, 6428, 6429, 6430, 6431, 6432, 6433, 6434, 6435, 6436, 3397, 3398, 3400, 3434 } },
      { "city", new int[] { 1983, 1390, 1391, 1392, 1393, 1394, 1395, 1396, 1397, 1398, 1399, 1400, 1401, 1402, 1403, 1404, 1405, 1406, 1407, 1408, 1409, 1410, 1411, 4385, 4408, 4112, 4113, 2505 } },
      { "civilian", new int[] { 220, 221, 222, 224, 225, 226, 227, 228, 229, 231, 4357, 4358, 238, 239, 240, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 278, 279, 280, 281, 282, 283, 284, 212, 4379 } },
      { "generic", new int[] { 3259, 1181, 1182, 1183, 1184, 4370, 4371, 1794, 1988, 3213, 3214, 3215, 3216, 3222, 3223, 1339, 3445, 3520, 4338, 1335, 1336, 1966, 1967, 1968, 1969, 1970, 1971, 1972, 4221, 1025, 1026, 1797, 3237, 3238, 3239, 3240, 3241, 1328, 1941, 1330, 1438, 496, 509, 522, 535, 1784, 1785, 1787, 1788, 1789, 1791, 1855, 1856, 1857, 1858, 1859, 1860, 2589, 1334, 1973, 1974, 4309, 4310, 4320, 4321, 4322, 34, 142, 1796, 1340, 3192, 3193, 3194, 3195, 3196, 1341, 1005, 1006, 1007, 1008, 1009, 1010, 1011, 1013, 1014, 1015, 1016, 1017, 1019, 1020, 1255, 3152, 3153, 3157, 3158, 3159, 3161, 3162, 3163, 3164, 3165, 3166, 3167, 3168, 3154, 3148, 3149, 1975, 1976, 1977, 1978, 1979, 2506, 3043, 3044, 3045, 3046, 3047, 1275, 1947, 1949, 1950, 1951, 1952, 6365, 6408, 31, 145, 144, 3305, 4364, 1982, 1749, 1750, 1751, 1332, 1333, 1987, 1863, 1337, 1295, 1329, 3310, 3311, 1802, 1803, 1804, 1805, 8, 35, 37, 4115, 4116, 4117, 4118, 4119, 4120, 4121, 4122, 4123, 3138, 1338 } }
    };
    private void HandleSpawnSpecificBehaviour(NwCreature creature)
    {
      switch(creature.Tag)
      {
        case "statue_tiamat":

          creature.OnConversation += PlaceableSystem.HandleCancelStatueConversation;

          Effect eff = Effect.CutsceneGhost();
          eff.SubType = EffectSubType.Supernatural;
          eff.Tag = "_GHOST_EFFECT";

          creature.ApplyEffect(EffectDuration.Permanent, eff);

          creature.MouseCursor = MouseCursor.Walk;
          creature.AiLevel = AiLevel.VeryLow;

          Task waitForCreatureToSpawn = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.5));
            await creature.PlayAnimation(Animation.LoopingPause, 0.0001f, false, TimeSpan.FromDays(365));
          });

          return;

        case "statue_reptilienne":

          creature.OnConversation += PlaceableSystem.HandleCancelStatueConversation;

          Effect eff2 = Effect.LinkEffects(Effect.CutsceneGhost(), Effect.VisualEffect((VfxType)927));
          eff2.SubType = EffectSubType.Supernatural;
          eff2.Tag = "_STATUE_EFFECT";
          creature.ApplyEffect(EffectDuration.Permanent, eff2);

          creature.MouseCursor = MouseCursor.Walk;
          creature.AiLevel = AiLevel.VeryLow;
          creature.HighlightColor = ColorConstants.Black;

          Task waitForCreatureToSpawn2 = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.5));
            await creature.PlayAnimation((Animation)creature.GetObjectVariable<LocalVariableInt>("animation").Value, 5, false, TimeSpan.FromDays(365));
            await NwTask.Delay(TimeSpan.FromSeconds(0.2));

            Effect freeze = Effect.VisualEffect(VfxType.DurFreezeAnimation);
            freeze.Tag = "_FREEZE_EFFECT";
            creature.ApplyEffect(EffectDuration.Permanent, freeze);
          });

          return;
      }

      switch (creature.GetObjectVariable<LocalVariableString>("_SPAWN_TYPE").Value)
      {
        case "npc":
          SetNPCEvents(creature);
          creature.AiLevel = AiLevel.VeryLow;
          break;

        case "walker":
          creature.AiLevel = AiLevel.VeryLow;
          _ = creature.ActionRandomWalk();
          break;

        case "civilian":
          SetRandomAppearance(creature);

          creature.AiLevel = AiLevel.VeryLow;
          _ = creature.ActionRandomWalk();
          break;
        case "plage":
        case "cave":
        case "city":
        case "generic":

          SetRandomAppearance(creature);

          creature.AiLevel = AiLevel.VeryLow;
          _ = creature.ActionRandomWalk();

          Effect runAway = Effect.AreaOfEffect((PersistentVfxType)190, scriptHandleFactory.CreateUniqueHandler(HandleRunAwayFromPlayer), null, scriptHandleFactory.CreateUniqueHandler(HandleNoOneAroundNeutral));

          break;

        default:
          HandleMobSpecificBehaviour(creature);
          break;
      }
    }
    private void HandleMobSpecificBehaviour(NwCreature creature)
    {
      switch (creature.Tag)
      {
        case "ratgarou":
        case "wereboar":
          creature.OnDeath += HandleWereInfiniteSpawn;
          break;
        case "sim_wraith":
          creature.OnDeath += HandleWraithInfiniteSpawn;
          break;
        default:
          creature.AiLevel = AiLevel.Low;
          creature.OnDeath += LootSystem.HandleLoot;
          creature.OnPerception += CreatureUtils.OnMobPerception;
          break;
      }

      creature.OnHeartbeat += CheckDistanceFromSpawn;
    }

    private void CheckDistanceFromSpawn(CreatureEvents.OnHeartbeat onHB)
    {
      //Log.Info("start checking distance from spawn");
      if (onHB.Creature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("_SPAWN").Value.DistanceSquared(onHB.Creature) < 1600)
      {
        //Log.Info("end checking distance from spawn");
        return;
      }

      onHB.Creature.AiLevel = AiLevel.VeryLow;
      _ = onHB.Creature.ClearActionQueue();
      _ = onHB.Creature.ActionForceMoveTo(onHB.Creature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("_SPAWN").Value, true, 0, TimeSpan.FromSeconds(30));

      Effect regen = Effect.RunAction(null, null, mobRegenIntervalHandle, TimeSpan.FromSeconds(1));
      regen.Tag = "mob_reset_regen";
      regen.SubType = EffectSubType.Supernatural;
      onHB.Creature.ApplyEffect(EffectDuration.Permanent, regen);

     // Log.Info("end checking distance from spawn");
    }

    /*private static void OnDeathSpawnNPCWaypoint(CreatureEvents.OnDeath onDeath)
    {
      NwWaypoint waypoint = NwWaypoint.Create(onDeath.KilledCreature.GetObjectVariable<LocalVariableString>("_WAYPOINT_TEMPLATE"), onDeath.KilledCreature.GetObjectVariable<LocalVariableLocation>("_SPAWN_LOCATION").Value);
      waypoint.GetObjectVariable<LocalVariableString>("_CREATURE_TEMPLATE").Value = onDeath.KilledCreature.ResRef;
    }*/
    private ScriptHandleResult onMobRegenInterval(CallInfo _)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature creature || !creature.IsValid)
        return ScriptHandleResult.Handled;

      creature.HP += (int)(creature.MaxHP * 0.2) + 1;

      if (creature.HP > creature.MaxHP)
        creature.HP = creature.MaxHP;

      //Log.Info($"creature distance from reset : {creature.Distance(creature.GetObjectVariable<LocalVariableObject<NwAreaOfEffect>>("reset_aoe").Value)}");

      if (creature.DistanceSquared(creature.GetObjectVariable<LocalVariableObject<NwAreaOfEffect>>("_SPAWN").Value) < 1)
      {
        //Log.Info($"{creature.Name} is on reset position !");
        creature.AiLevel = AiLevel.Default;
        foreach (Effect eff in creature.ActiveEffects.Where(e => e.Tag == "mob_reset_regen"))
          creature.RemoveEffect(eff);
      }

      return ScriptHandleResult.Handled;
    }
    private void HandleWereInfiniteSpawn(CreatureEvents.OnDeath onDeath)
    {
      NwWaypoint wp = onDeath.KilledCreature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("_SPAWN").Value;
      NwCreature.Create(onDeath.KilledCreature.ResRef, wp.Location).OnDeath += HandleWereInfiniteSpawn;
    }
    private void HandleWraithInfiniteSpawn(CreatureEvents.OnDeath onDeath)
    {
      NwWaypoint wp = onDeath.KilledCreature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("_SPAWN").Value;
      NwCreature.Create(onDeath.KilledCreature.ResRef, wp.Location).OnDeath += HandleWraithInfiniteSpawn;
      NwCreature.Create(onDeath.KilledCreature.ResRef, wp.Location).OnDeath += HandleWraithInfiniteSpawn;
    }
    private async void SetNPCEvents(NwCreature creature)
    {
      //creature.OnDeath += OnDeathSpawnNPCWaypoint;

      switch (creature.Tag)
      {
        case "bank_npc":
          creature.OnConversation += DialogSystem.StartBankerDialog;

          NwItem contract = await NwItem.Create("shop_clearance", creature);
          contract.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;
          contract.Tag = "bank_contract";
          contract.Name = "Contrat d'ouverture de compte Skalsgard";
          contract.Description = "Le contrat que vous avez entre les mains déclare sur des pages et des pages des conditions d'ouverture de compte et de services sommes toutes classiques.\n\n" +
          "Les suivantes sortent tout de même sensiblement de l'ordinaire :\n" +
          " - La banque autorise un découvert illimité et automatique avec intérêts de 30 %.\n" +
          " - La banque se réserve la possibilité de demander le remboursement d'un prêt à n'importe quel moment.\n" +
          " - En cas de défaut de paiement, le signataire s'engage à rembourser sa dette sous forme de Substance Pure, récoltée dans les tréfons de Similisse.\n" +
          " - La banque assure la sécurité des coffres : seuls les clients sont autorisés à voir et interagir au coffre qui leur a été attribué et à son contenu.\n" +
          " - Le client signataire s'engage à ne pas tenter de voir ou d'interagir avec les coffres d'autres clients, ou leur contenu.\n\n" +
          "Bon pour accord,\n" +
          "Signature : ";

          break;
        case "blacksmith":
          creature.OnConversation += dialogSystem.StartBlacksmithDialog;
          break;
        case "woodworker":
          creature.OnConversation += dialogSystem.StartWoodworkerDialog;
          break;
        case "tanneur":
          creature.OnConversation += dialogSystem.StartTanneurDialog;
          break;
        case "le_bibliothecaire":
          creature.OnConversation += dialogSystem.StartBibliothecaireDialog;
          break;
        case "jukebox":
        case "jukebox2":
          creature.OnConversation += dialogSystem.StartJukeboxDialog;
          break;
        case "rumors":
          creature.OnConversation += dialogSystem.StartRumorsDialog;
          break;
        case "tribunal_hotesse":
          creature.OnConversation += dialogSystem.StartTribunalShopDialog;
          break;
        case "pve_arena_host":
          creature.OnConversation += dialogSystem.StartPvEArenaHostDialog;
          break;
        case "bal_system":
          creature.OnConversation += dialogSystem.StartMessengerDialog;
          break;
        case "storage_npc":
          creature.OnConversation += dialogSystem.StartStorageDialog;
          break;
      }
    }
    public void SetRandomAppearance(NwCreature creature)
    {
      string appearance = creature.GetObjectVariable<LocalVariableString>("_SPAWN_TYPE").HasValue ? creature.GetObjectVariable<LocalVariableString>("_SPAWN_TYPE").Value : "city";
      int[] appearanceArray = randomAppearanceDictionary[appearance];
      int rowId = appearanceArray[Utils.random.Next(0, appearanceArray.Length)];

      creature.Appearance = NwGameTables.AppearanceTable.GetRow(rowId);
      creature.PortraitResRef = creature.Appearance.Portrait;

      if (appearance == "civilian")
        return;

      creature.Name = creature.Appearance.Name;
      if (creature.Name == "Créature")
        Utils.LogMessageToDMs($"Apparence {rowId} - Nom non défini.");
    }
    private ScriptHandleResult HandleRunAwayFromPlayer(CallInfo callInfo)
    {
      NwAreaOfEffect aoe = (NwAreaOfEffect)callInfo.ObjectSelf;

      if (!(aoe.Creator is NwCreature { IsPlayerControlled: true } neutral))
        return ScriptHandleResult.Handled;

      NwCreature closestPlayer = neutral.GetNearestCreatures(CreatureTypeFilter.PlayerChar(true)).FirstOrDefault();

      if (closestPlayer != null && neutral.DistanceSquared(closestPlayer) < 25)
        _ = neutral.ActionMoveAwayFrom(closestPlayer, true);

      return ScriptHandleResult.Handled;
    }
    private ScriptHandleResult HandleNoOneAroundNeutral(CallInfo callInfo)
    {
      NwAreaOfEffect aoe = (NwAreaOfEffect)callInfo.ObjectSelf;

      if (!(aoe.Creator is NwCreature { IsPlayerControlled: true } neutral))
        return ScriptHandleResult.Handled;

      NwCreature closestPlayer = neutral.GetNearestCreatures(CreatureTypeFilter.PlayerChar(true)).FirstOrDefault();

      if (closestPlayer == null && neutral.DistanceSquared(closestPlayer) > 30)
        _ = neutral.ActionRandomWalk();

      return ScriptHandleResult.Handled;
    }
  }
}
