using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

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
    private async void HandleSpawnSpecificBehaviour(NwCreature creature, NwWaypoint spawnPoint)
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
            await creature.PlayAnimation((Animation)spawnPoint.GetObjectVariable<LocalVariableInt>("animation").Value, 5, false, TimeSpan.FromDays(365));
            await NwTask.Delay(TimeSpan.FromSeconds(0.2));

            Effect freeze = Effect.VisualEffect(VfxType.DurFreezeAnimation);
            freeze.Tag = "_FREEZE_EFFECT";
            creature.ApplyEffect(EffectDuration.Permanent, freeze);
          });

          return;
      }

      string spawnType = spawnPoint.GetObjectVariable<LocalVariableString>("_SPAWN_TYPE").Value;

      switch (spawnType)
      {
        case "npc":

          SetNPCEvents(creature);
          creature.AiLevel = AiLevel.VeryLow;

          if (creature.Tag == "marten_forgenfer")
            GenerateMartenRandomBanter(creature);

          break;

        case "walker":
          creature.OnHeartbeat += CheckActionRandomWalk;
          break;

        case "civilian":
          SetRandomAppearance(creature, spawnType);

          //creature.AiLevel = AiLevel.VeryLow;
          if (creature.Area.Tag == "SimilisseTransitionPromenadeport")
          {
            creature.AiLevel = AiLevel.VeryLow;
            GenerateInfectedRandomBanter(creature);
          }
          else
            creature.OnHeartbeat += CheckActionRandomWalk;

          break;

        case "plage":
        case "cave":
        case "city":    
        case "generic":

          await creature.WaitForObjectContext();
          SetRandomAppearance(creature, spawnType);
          
          creature.OnHeartbeat += CheckActionRandomWalk;

          Effect runAway = Effect.AreaOfEffect((PersistentVfxType)190, mobRunAway);
          runAway.SubType = EffectSubType.Supernatural;
          runAway.Tag = "_NEUTRALS_RUN_AWAY_EFFECT";
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
      CreatureUtils.InitThreatRange(creature);
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
        EffectUtils.RemoveTaggedEffect(creature, "mob_reset_regen");
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

          creature.OnConversation += dialogSystem.StartBankerDialog;

          NwItem contract = await NwItem.Create("shop_clearance", creature);
          //contract.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;
          contract.Tag = "bank_contract";
          contract.Name = "Contrat d'ouverture de compte Skalsgard";
          contract.Description = "Le contrat que vous avez entre les mains déclare sur des pages et des pages des conditions d'ouverture de compte et de services somme toute classiques.\n\n" +
          "Les suivantes sortent tout de même sensiblement de l'ordinaire :\n" +
          " - La banque autorise un découvert illimité et automatique avec intérêts de 30 %.\n" +
          " - La banque se réserve la possibilité de demander le remboursement d'un prêt à n'importe quel moment.\n" +
          " - En cas de défaut de paiement, le signataire s'engage à rembourser sa dette sous forme de Substance Pure, récoltée dans les tréfonds de Similisse.\n" +
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
    public async void SetRandomAppearance(NwCreature creature, string spawnType)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(0.3));

      List<AppearanceTableEntry> appearanceArray = randomAppearanceDictionary[spawnType];
      AppearanceTableEntry appearanceEntry = appearanceArray[Utils.random.Next(0, appearanceArray.Count)];

      creature.Appearance = appearanceEntry;
      creature.PortraitResRef = creature.Appearance.Portrait;
      creature.VisualTransform.Scale = Utils.random.NextFloat(0.80f, 1.20f);

      if (spawnType == "civilian")
        creature.Name = "Habitant de Similisse";
      else
        creature.Name = StringUtils.ConvertToUTF8(creature.Appearance.Label);
    }
    private ScriptHandleResult HandleRunAwayFromPlayer(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData))
        return ScriptHandleResult.Handled;

      if (eventData.Effect.Creator is not NwCreature neutral)
        return ScriptHandleResult.Handled;

      if (eventData.Entering is not NwCreature oEntering)
        return ScriptHandleResult.Handled;

      if (!(oEntering.IsPlayerControlled || (oEntering.Master != null && oEntering.Master.IsPlayerControlled)))
        return ScriptHandleResult.Handled;

      MoveAway(neutral, oEntering);

      return ScriptHandleResult.Handled;
    }
    private static async void MoveAway(NwCreature creature, NwCreature oEntering)
    {
      await creature.ClearActionQueue();
      await creature.ActionMoveAwayFrom(oEntering, true, 10);
    }
    private async void CheckActionRandomWalk(CreatureEvents.OnHeartbeat onHB)
    {
      if(onHB.Creature.CurrentAction != Anvil.API.Action.RandomWalk && onHB.Creature.CurrentAction != Anvil.API.Action.MoveToPoint)
        await onHB.Creature.ActionRandomWalk();
        //onHB.Creature.AiLevel = AiLevel.VeryLow;
    }
    /*private async void AwaitRandomWalk(NwCreature creature)
    {
      await creature.WaitForObjectContext();
      await NwTask.Delay(TimeSpan.FromSeconds(0.5));
      await creature.ClearActionQueue();
      await creature.ActionRandomWalk();
    }*/
    private readonly string[] randomInfectedBanterArray = new string[] 
    {
      "S'il vous plait, laissez-moi passer !",
      "J'ai besoin de voir ma famille de l'autre côté !",
      "Je connais des gens importants, vous allez le regretter !",
      "Pitié, vous ne pouvez pas nous garder de ce côté indéfiniment !",
      "On a plus de nourriture, plus rien pour nous soigner !",
      "Laissez nous sortir !",
      "On veut passer !",
      "Je suis pas malade ! Regardez-moi, j'ai l'air malade ?",
      "Tout ça est ridicule !",
      "Il n'y a pas d'épidémie, vous n'avez aucune preuve !",
      "Au secours !",
      "Ayez pitié !",
      "Nous n'y sommes pour rien !",
      "On veut juste passer !",
      "C'est un outrage !",
      "A l'aide !",
      "Par pitié !",
      "Comprenez-nous, ça ne peut pas durer !",
      "Combien de temps allez vous nous garder comme ça ?",
      "Les gens ne sont pas fait pour vivre enfermés !",
      "Laissez-nous passer !",
      "Il faut nous aider !",
      "Nous avons le droit de savoir ce qui se passe !",
      "Pourquoi nous maintenez-vous enfermés ici ? C'est inhumain !",
      "Comment pouvons-nous faire confiance à votre Empereur si vous ne nous dites rien ?",
      "Nous avons des familles et des vies à l'extérieur de ce quartier !",
      "Cette barricade ne nous protège pas, elle nous emprisonne !",
      "Nous ne sommes pas des animaux ! Nous méritons d'être traités avec respect et dignité !",
      "Nous demandons simplement des réponses claires et honnêtes !",
      "Si vous ne nous dites pas ce qui se passe, nous allons devoir prendre les choses en main !",
      "Cette quarantaine est une violation de nos droits ! Nous ne laisserons pas cela se produire !",
      "Nous ne pouvons pas rester ici indéfiniment ! Donnez-nous une solution ou laissez-nous partir !",
      "Nous ne voulons pas mourir ici, laisser-nous sortir !",
      "Comment allons-nous survivre sans accès à la nourriture et aux médicaments ?",
      "Je ne veux pas perdre ma famille à cause de cette maladie !",
      "Vous nous condamnez en nous enfermant ici !",
      "Je ne veux pas être infecté par cette maladie !",
      "Nous avons besoin d'une solution, sinon nous sommes tous perdus !",
      "Je ne veux pas rester ici et regarder mes proches mourir lentement !",
      "Nous ne pouvons pas faire face seuls, nous avons besoin d'aide !",
      "Je ne veux pas finir comme ces pauvres âmes que nous avons vues tomber malades !",
      "Nous ne pouvons pas être abandonnés ici, nous méritons une chance de survie !",
      "Si vous ne nous laissez pas partir, nous allons détruire cette barricade !",
      "Nous ne pouvons pas rester coincés ici indéfiniment !",
      "Nous ne laisserons pas cette maladie nous condamner à mort !",
      "Nous allons devoir prendre les choses en main si vous ne nous donnez pas des réponses bientôt !",
      "Si nous mourrons ici, vous devrez répondre de vos actes devant les dieux !",
      "Nous ne pouvons pas être traités comme des prisonniers !",
      "Nous avons le droit de protéger nos proches et nos maisons !",
      "Nous avons des armes, nous pouvons briser cette barricade et sortir !",
      "Nous ne laisserons pas cette maladie nous gouverner, nous allons nous battre pour notre liberté !"
    };
    private readonly string[] randomMartenBanterArray = new string[]
    {
      "Personne ne passe.",
      "Gardez votre calme.",
      "L'épidémie est trop dangereuse : personne ne sort pour le moment.",
      "Nous vous tiendrons informés dès que nous aurons des nouvelles.",
      "Ceux qui tentent de forcer le barrage seront abattus.",
      "Aucun geste brusque.",
      "Pas d'agitation.",
      "Les agitateurs seront fermement réprimandés.",
      "Reculez !",
      "Ne vous tenez pas trop près du barrage.",
      "Eloignez-vous",
      "Arrêtez immédiatement ! Vous risquez d'aggraver la situation !",
      "Nous faisons tout ce que nous pouvons pour protéger les habitants de cette île !",
      "Reculez ! Cette barricade est là pour votre sécurité de tous !",
      "Je suis désolé, mais nous ne pouvons pas prendre de risques avec une épidémie aussi dangereuse !",
      "Vous mettez en danger la vie de tous les habitants de Similisse en agissant ainsi !",
      "Nous avons besoin de votre coopération et de votre patience pour gérer cette situation difficile !",
      "Je comprends que vous soyez en colère, mais nous devons tous faire notre part pour éviter une catastrophe !",
      "Veuillez gardez votre calme et restez derrière la barricade !",
      "Il y a des vies en jeu ici ! Nous ne pouvons pas nous permettre de prendre des risques inutiles !",
      "Nous ne pouvons pas laisser la peur et la panique prendre le dessus ! Nous devons rester forts et solidaires !"
    };
    private async void GenerateInfectedRandomBanter(NwCreature creature)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(Utils.random.Next(20, 120)));
      if (creature != null && creature.IsValid)
      {
        await creature.SpeakString(randomInfectedBanterArray[Utils.random.Next(0, randomInfectedBanterArray.Length)].ColorString(ColorConstants.Red));
        GenerateInfectedRandomBanter(creature);
      }
    }
    private async void GenerateMartenRandomBanter(NwCreature creature)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(Utils.random.Next(15, 60)));
      if (creature != null && creature.IsValid)
      {
        await creature.SpeakString(randomMartenBanterArray[Utils.random.Next(0, randomMartenBanterArray.Length)].ColorString(ColorConstants.Silver));
        GenerateMartenRandomBanter(creature);
      }
    }
  }
}
