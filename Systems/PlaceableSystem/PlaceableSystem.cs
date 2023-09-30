using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using System.Linq;
using NLog;
using System.Threading.Tasks;
using static NWN.Systems.PlayerSystem;
using NWN.Core.NWNX;
using System.Numerics;

namespace NWN.Systems
{
  [ServiceBinding(typeof(PlaceableSystem))]
  public class PlaceableSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public readonly SchedulerService scheduler;
    public PlaceableSystem(SchedulerService schedulerService)
    {
      scheduler = schedulerService;

      foreach (NwPlaceable plc in NwObject.FindObjectsOfType<NwPlaceable>())
      {
        if (plc.GetObjectVariable<LocalVariableBool>("_SPAWN_ID").HasNothing)
          plc.GetObjectVariable<LocalVariableBool>("_EDITOR_PLACEABLE").Value = true;

        if (plc.IsStatic)
          continue;

        switch (plc.Tag)
        {
          case "intro_mirror": plc.OnLeftClick += StartIntroMirrorDialog; break;
          case "body_modifier": plc.OnUsed += StartBodyModifierDialog; break;
          case "refinery": plc.OnUsed += OpenRefineryWindow; break;
          case "decoupe": plc.OnUsed += OpenWoodworkWindow; break;
          case "tannerie_peau": plc.OnUsed += OpenTanneryWindow; break;
          case "player_bank": plc.OnLeftClick += HandleClickBank; break;
          case "balancoire": plc.OnUsed += OnUsedBalancoire; break;
          case "dice_poker": plc.OnUsed += OnUsedDicePoker; break;
          case "go_plouf": plc.OnUsed += OnUsedGoPlouf; break;
          case "stop_plouf": plc.OnUsed += OnUsedStopPlouf; break;
          case "portal_start": plc.OnUsed += HandleStartPortalUsed; break;
          case "respawn_neutral": plc.OnUsed += HandlePlayerRespawn; break;
          case "respawn_dire": plc.OnUsed += HandlePlayerRespawn; break;
          case "respawn_radiant": plc.OnUsed += HandlePlayerRespawn; break;
          case "theater_rope": plc.OnUsed += HandleTheaterCurtains; break;
          case "forge": 
          case "scierie":
          case "tannerie": plc.OnUsed += OpenWorkshopWindow; break;
          //case "bank_gold": plc.OnUsed += Give1000Gold; break;
        }
        
        if (plc.VisualTransform.Scale != 1 || plc.VisualTransform.Translation != Vector3.Zero || plc.VisualTransform.Rotation != Vector3.Zero)
          plc.VisibilityOverride = VisibilityMode.AlwaysVisible;
        else if (!plc.Useable && plc.AnimationState != AnimationState.PlaceableActivated)
          plc.IsStatic = true;
      }

      foreach (NwDoor door in NwObject.FindObjectsOfType<NwDoor>())
      {
        door.OnOpen += HandleDoorAutoClose;

        if (door.Tag == "at_gates_slums")
        {
          door.GetObjectVariable<LocalVariableObject<NwGameObject>>("_TRANSITION_TARGET").Value = door.TransitionTarget;
          door.OnAreaTransitionClick += CheckMateriaInventory;
        }

        if (door.Tag == "at_tour_collines") // TODO : temporaire, à supprimer après mise à jour des haks
          door.Locked = false;

        if (door.VisualTransform.Scale != 1 || door.VisualTransform.Translation != Vector3.Zero || door.VisualTransform.Rotation != Vector3.Zero)
          door.VisibilityOverride = VisibilityMode.AlwaysVisible;
      }

      foreach (NwCreature corpse in NwObject.FindObjectsWithTag<NwCreature>("dead_wererat"))
      {
        corpse.OnConversation += HandleCancelStatueConversation;
        corpse.OnSpawn += HandleSetUpDeadCreatureCorpse;
      }

      foreach (NwCreature trainer in NwObject.FindObjectsWithTag<NwCreature>("damage_trainer"))
      {
        trainer.MaxHP = 9999;
        trainer.HP = 9999;
        trainer.BaseAC = (sbyte)trainer.GetObjectVariable<LocalVariableInt>("AC").Value;

        trainer.OnConversation += HandleCancelStatueConversation;
        trainer.OnSpawn += HandleSpawnTrainingDummy;
        trainer.OnDamaged += HandleTrainingDummyDamaged;

        trainer.GetObjectVariable<LocalVariableFloat>("_PERSONNAL_SPACE").Value = CreaturePlugin.GetPersonalSpace(trainer);
        trainer.GetObjectVariable<LocalVariableFloat>("_HEIGHT").Value = CreaturePlugin.GetHeight(trainer);
        trainer.GetObjectVariable<LocalVariableFloat>("_HIT_DISTANCE").Value = CreaturePlugin.GetHitDistance(trainer);
        trainer.GetObjectVariable<LocalVariableFloat>("_CREATURE_PERSONNAL_SPACE").Value = CreaturePlugin.GetCreaturePersonalSpace(trainer);
      }
    }
    private void HandleTrainingDummyDamaged(CreatureEvents.OnDamaged onDamaged)
    {
      Task HealAfterDamage = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0.1f));
        onDamaged.Creature.HP = onDamaged.Creature.MaxHP;
      });
    }
    public async void OnUsedBalancoire(PlaceableEvents.OnUsed onUsed)
    {
      await onUsed.UsedBy.ActionSit(onUsed.Placeable.GetNearestObjectsByType<NwPlaceable>().FirstOrDefault(p => p.Tag == "balancoiresitter"));

      new Swing(onUsed.Placeable, onUsed.UsedBy, this);
    }

    private void HandlePlayerRespawn(PlaceableEvents.OnUsed onUsed)
    {
      if (!Players.TryGetValue(onUsed.UsedBy.ControllingPlayer.ControlledCreature, out Player player))
        return;

      player.Respawn();
    }
    private void HandleStartPortalUsed(PlaceableEvents.OnUsed onUsed)
    {
      onUsed.UsedBy.Location = NwObject.FindObjectsWithTag<NwWaypoint>("WP_START_NEW_CHAR").FirstOrDefault().Location;
    }
    public static void HandleTheaterCurtains(PlaceableEvents.OnUsed onUsed)
    {
      foreach (NwPlaceable plc in NwObject.FindObjectsWithTag<NwPlaceable>("theater_curtain"))
        if (plc.Area == onUsed.Placeable.Area)
          plc.VisibilityOverride = plc.VisibilityOverride == VisibilityMode.Visible ? VisibilityMode.Hidden : VisibilityMode.Visible;
    }
    public static void HandleCancelStatueConversation(CreatureEvents.OnConversation onConversation)
    {

    }
    private void HandleSpawnTrainingDummy(CreatureEvents.OnSpawn onSpawn)
    {
      Effect eff = Effect.LinkEffects(Effect.CutsceneGhost(), Effect.CutsceneParalyze());
      eff.SubType = EffectSubType.Supernatural;
      eff.Tag = "_FREEZE_EFFECT";
      onSpawn.Creature.ApplyEffect(EffectDuration.Permanent, eff);
      onSpawn.Creature.AiLevel = AiLevel.VeryLow;
    }
    private async void HandleDoorAutoClose(DoorEvents.OnOpen onOpen)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(10));
      await onOpen.Door.Close();
      if (onOpen.Door.TransitionTarget is NwDoor door)
        await door.Close();
    }
    private void HandleSetUpDeadCreatureCorpse(CreatureEvents.OnSpawn onSpawn)
    {
      onSpawn.Creature.ApplyEffect(EffectDuration.Instant, Effect.Death());
    }
    private void OnUsedDicePoker(PlaceableEvents.OnUsed onUsed)
    {
      if (!Players.TryGetValue(onUsed.UsedBy, out Player player))
        return;

      if (onUsed.Placeable.GetObjectVariable<LocalVariableInt>("_AVAILABLE_SLOTS").HasNothing)
        onUsed.Placeable.GetObjectVariable<LocalVariableInt>("_AVAILABLE_SLOTS").Value = 2;

      int availableSlots = onUsed.Placeable.GetObjectVariable<LocalVariableInt>("_AVAILABLE_SLOTS").Value;

      if (availableSlots == 0)
      {
        player.oid.SendServerMessage("Aucune place n'est disponible sur ce plateau de dés !");
        return;
      }
      else if (availableSlots == 1)
      {
        onUsed.Placeable.GetObjectVariable<LocalVariableInt>("_AVAILABLE_SLOTS").Value = 0;
        onUsed.Placeable.GetObjectVariable<LocalVariableObject<NwCreature>>("_PLAYER_TWO").Value = player.oid.ControlledCreature;
      }
      else if (availableSlots == 2)
      {
        new DicePoker.DicePoker(player, onUsed.Placeable);
      }
    }
    private void OnUsedGoPlouf(PlaceableEvents.OnUsed onUsed)
    {
      NwPlaceable stopplouf = NwObject.FindObjectsWithTag<NwPlaceable>("stop_plouf").FirstOrDefault();
      onUsed.UsedBy.Location = stopplouf.Location;
    }
    private void OnUsedStopPlouf(PlaceableEvents.OnUsed onUsed)
    {
      NwPlaceable goplouf = NwObject.FindObjectsWithTag<NwPlaceable>("go_plouf").FirstOrDefault();
      onUsed.UsedBy.Location = goplouf.Location;
    }
    private async void HandleClickBank(PlaceableEvents.OnLeftClick onClick)
    {
      if (!Players.TryGetValue(onClick.ClickedBy.LoginCreature, out Player player))
        return;

      await onClick.ClickedBy.ControlledCreature.ClearActionQueue();

      if (onClick.Placeable.GetObjectVariable<LocalVariableInt>("ownerId").Value != player.characterId)
      {
        player.oid.SendServerMessage("Vous avez beau avancer la main, le coffre semble rester hors d'atteinte, inaccessible. De plus, vous entendez un lointain cliquetis de chaînes dans les tréfonds de votre esprit. Quelque chose vient de se mettre en marche.", ColorConstants.Red);
        onClick.ClickedBy.ControlledCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSpellMantleUse));
        return;
      }

      if (!player.windows.ContainsKey("bankStorage")) player.windows.Add("bankStorage", new Player.BankStorageWindow(player));
      else ((Player.BankStorageWindow)player.windows["bankStorage"]).CreateWindow();
    }
    private async void CheckMateriaInventory(DoorEvents.OnAreaTransitionClick onClick)
    {
      try
      {
        if (!onClick.ClickedBy.ControlledCreature.Inventory.Items.Any(i => i.Tag == "craft_resource"))
          await onClick.ClickedBy.ControlledCreature.JumpToObject(onClick.Door.TransitionTarget);
        else
        {
          onClick.ClickedBy.SendServerMessage("Une sorte de mur de force vous empêche de pénétrer plus avant dans la cité.\nUn frisson remonte le long de votre moëlle épinière alors que vous avez la certitude que quelque chose puissant porte un regard soupçonneux sur vous.\n\nLes miliciens vous signaleront que la Loi interdit d'entrer en possession de matéria et qu'il vous faut les déposer à l'entrepôt derrière vous.", ColorConstants.Red);
          await onClick.ClickedBy.ControlledCreature.ClearActionQueue();
          onClick.ClickedBy.ControlledCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSpellMantleUse));
        }
      }
      catch(Exception e) 
      {
        Utils.LogMessageToDMs($"SLUMS TRANSITION ERROR - {e.Message}\n{e.StackTrace}");
        Utils.LogMessageToDMs($"ClickedBy - {onClick.ClickedBy.PlayerName}");
        Utils.LogMessageToDMs($"ControlledCreature - {onClick.ClickedBy.ControlledCreature.Name}");
        Utils.LogMessageToDMs($"ControlledCreature - {onClick.ClickedBy.ControlledCreature.Inventory}");
        Utils.LogMessageToDMs($"ControlledCreature - {onClick.ClickedBy.ControlledCreature.Inventory.Items}");
      }
    }
    public static void StartIntroMirrorDialog(PlaceableEvents.OnLeftClick onUsed)
    {
      if (Players.TryGetValue(onUsed.ClickedBy.LoginCreature, out Player player))
      {
        if (!player.windows.ContainsKey("introMirror")) player.windows.Add("introMirror", new Player.IntroMirroWindow(player));
        else ((Player.IntroMirroWindow)player.windows["introMirror"]).CreateWindow();
      }
    }
    public static void StartBodyModifierDialog(PlaceableEvents.OnUsed onUsed)
    {
      if (Players.TryGetValue(onUsed.UsedBy, out Player player))
      {
        if (!player.windows.ContainsKey("bodyAppearanceModifier")) player.windows.Add("bodyAppearanceModifier", new Player.BodyAppearanceWindow(player, player.oid.LoginCreature));
        else ((Player.BodyAppearanceWindow)player.windows["bodyAppearanceModifier"]).CreateWindow(player.oid.LoginCreature);
      }
    }
    public static void OpenRefineryWindow(PlaceableEvents.OnUsed onUsed)
    {
      if (Players.TryGetValue(onUsed.UsedBy, out Player player))
      {
        if (!player.windows.ContainsKey("refinery")) player.windows.Add("refinery", new Player.RefineryWindow(player, ResourceType.Ingot));
        else ((Player.RefineryWindow)player.windows["refinery"]).CreateWindow(ResourceType.Ingot);
      }
    }
    public static void OpenWoodworkWindow(PlaceableEvents.OnUsed onUsed)
    {
      if (Players.TryGetValue(onUsed.UsedBy, out Player player))
      {
        if (!player.windows.ContainsKey("refinery")) player.windows.Add("refinery", new Player.RefineryWindow(player, ResourceType.Plank));
        else ((Player.RefineryWindow)player.windows["refinery"]).CreateWindow(ResourceType.Plank);
      }
    }
    public static void OpenTanneryWindow(PlaceableEvents.OnUsed onUsed)
    {
      if (Players.TryGetValue(onUsed.UsedBy, out Player player))
      {
        if (!player.windows.ContainsKey("refinery")) player.windows.Add("refinery", new Player.RefineryWindow(player, ResourceType.Leather));
        else ((Player.RefineryWindow)player.windows["refinery"]).CreateWindow(ResourceType.Leather);
      }
    }
    public static void OpenWorkshopWindow(PlaceableEvents.OnUsed onUsed)
    {
      onUsed.UsedBy.ControllingPlayer?.SendServerMessage("Afin de commencer un travail artisanal, il vous faut utiliser sur cet atelier un objet disposant d'un enchantement de manipulation de matéria raffinée.");
    }
    public static void Give1000Gold(PlaceableEvents.OnUsed onUsed)
    {
      onUsed.UsedBy.GiveGold(1000);
    }
  }
}
