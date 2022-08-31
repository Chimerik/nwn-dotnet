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
    private readonly Dictionary<string, List<AppearanceTableEntry>> randomAppearanceDictionary = new()
    {
      { "plage", new List<AppearanceTableEntry>() { } },
      { "cave", new List<AppearanceTableEntry>() { } },
      { "city", new List<AppearanceTableEntry>() { } },
      { "civilian", new List<AppearanceTableEntry>() { } },
      { "generic", new List<AppearanceTableEntry>() { } }
    };
    private async void HandleSpawnSpecificBehaviour(NwCreature creature)
    {
      switch (creature.Tag)
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

          //SetRandomAppearance(creature);

          creature.AiLevel = AiLevel.VeryLow;
          _ = creature.ActionRandomWalk();

          await creature.WaitForObjectContext();
          creature.ApplyEffect(EffectDuration.Permanent, runAway);

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

      if (creature.DistanceSquared(creature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("_SPAWN").Value) < 1)
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
      List<AppearanceTableEntry> appearanceArray = randomAppearanceDictionary[appearance];
      AppearanceTableEntry rowId = appearanceArray[Utils.random.Next(0, appearanceArray.Count)];

      creature.Appearance = rowId;
      creature.PortraitResRef = creature.Appearance.Portrait;
      creature.VisualTransform.Scale = Utils.random.NextFloat(0.80f, 1.20f);

      if (appearance == "civilian")
        return;

      creature.Name = creature.Appearance.Name;
      if (creature.Name == "Créature")
        Utils.LogMessageToDMs($"Apparence {rowId} - Nom non défini.");
    }
    private ScriptHandleResult HandleRunAwayFromPlayer(CallInfo callInfo)
    {
      NwAreaOfEffect aoe = (NwAreaOfEffect)callInfo.ObjectSelf;

      Log.Info(callInfo.ObjectSelf is NwAreaOfEffect);
      Log.Info(callInfo.ObjectSelf.Tag);
      Log.Info(NWScript.GetName(NWScript.GetAreaOfEffectCreator(callInfo.ObjectSelf)));
      Log.Info(aoe.Creator);
      Log.Info(aoe.Creator.Name);

      if (!(NWScript.GetAreaOfEffectCreator(callInfo.ObjectSelf).ToNwObject() is NwCreature neutral) || NWScript.GetEnteringObject().ToNwObject<NwGameObject>() is not NwCreature { IsPlayerControlled: true } player || player.DistanceSquared(neutral) < 25)
        return ScriptHandleResult.Handled;

       _ = neutral.ActionMoveAwayFrom(player, true);

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
