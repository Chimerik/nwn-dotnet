﻿using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using System.Linq;
using NLog;
using System.Threading.Tasks;
using static NWN.Systems.PlayerSystem;
using NWN.System;
using System.Collections.Generic;

namespace NWN.Systems
{
  [ServiceBinding(typeof(PlaceableSystem))]
  public class PlaceableSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public PlaceableSystem()
    {
      foreach (NwDoor door in NwObject.FindObjectsOfType<NwDoor>())
        door.OnOpen += HandleDoorAutoClose;

      foreach (NwPlaceable plc in NwObject.FindObjectsWithTag<NwPlaceable>("intro_mirror"))
        plc.OnUsed += StartIntroMirrorDialog;

      foreach (NwPlaceable plc in NwObject.FindObjectsWithTag<NwPlaceable>("body_modifier"))
        plc.OnUsed += StartBodyModifierDialog;

      foreach (NwPlaceable plc in NwObject.FindObjectsWithTag<NwPlaceable>("refinery"))
        plc.OnUsed += OpenRefineryWindow;

      foreach (NwPlaceable plc in NwObject.FindObjectsWithTag<NwPlaceable>("decoupe"))
        plc.OnUsed += OpenWoodworkWindow;

      foreach (NwPlaceable plc in NwObject.FindObjectsWithTag<NwPlaceable>("tannerie_peau"))
        plc.OnUsed += OpenTanneryWindow;

      foreach (NwDoor gate in NwObject.FindObjectsWithTag<NwDoor>("at_gates_slums"))
      {
        gate.GetObjectVariable<LocalVariableObject<NwGameObject>>("_TRANSITION_TARGET").Value = gate.TransitionTarget;
        gate.OnAreaTransitionClick += CheckMateriaInventory;
      }

      foreach (NwPlaceable bank in NwObject.FindObjectsWithTag<NwPlaceable>("player_bank"))
        bank.OnLeftClick += HandleClickBank;

      foreach (NwPlaceable bassin in NwObject.FindObjectsWithTag<NwPlaceable>("ench_bsn"))
        bassin.OnClose += HandleCloseEnchantementBassin;

      foreach (NwPlaceable balancoire in NwObject.FindObjectsWithTag<NwPlaceable>("balancoire"))
        balancoire.OnUsed += OnUsedBalancoire;

      foreach (NwPlaceable dicePoker in NwObject.FindObjectsWithTag<NwPlaceable>("dice_poker"))
        dicePoker.OnUsed += OnUsedDicePoker;

      foreach (NwPlaceable goplouf in NwObject.FindObjectsWithTag<NwPlaceable>("go_plouf"))
        goplouf.OnUsed += OnUsedGoPlouf;

      foreach (NwPlaceable stopplouf in NwObject.FindObjectsWithTag<NwPlaceable>("stop_plouf"))
        stopplouf.OnUsed += OnUsedStopPlouf;

      foreach (NwPlaceable plc in NwObject.FindObjectsWithTag<NwPlaceable>("portal_start", "respawn_neutral", "respawn_dire", "respawn_radiant", "theater_rope"))
        plc.OnUsed += HandlePlaceableUsed;

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
      }
    }
    private static void HandleTrainingDummyDamaged(CreatureEvents.OnDamaged onDamaged)
    {
      Task HealAfterDamage = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0.1f));
        onDamaged.Creature.HP = onDamaged.Creature.MaxHP;
      });
    }
    public static void OnUsedStoragePortalOut(PlaceableEvents.OnUsed onUsed)
    {
      onUsed.UsedBy.Location = NwModule.FindObjectsWithTag<NwWaypoint>("wp_outentrepot").FirstOrDefault().Location;
    }
    public static async void OnUsedBalancoire(PlaceableEvents.OnUsed onUsed)
    {
      await onUsed.UsedBy.ActionSit(onUsed.Placeable.GetNearestObjectsByType<NwPlaceable>().FirstOrDefault(p => p.Tag == "balancoiresitter"));

      new Swing(onUsed.Placeable, onUsed.UsedBy);
    }
    
    public static void HandleCleanDMPLC(PlaceableEvents.OnDeath onDeath)
    {
      NwPlaceable plc = onDeath.KilledObject;
      int plcID = plc.GetObjectVariable<LocalVariableInt>("_ID").Value;
      if (plcID > 0)
      {
        SqLiteUtils.DeletionQuery("dm_persistant_placeable",
          new Dictionary<string, string>() { { "rowid", plcID.ToString() } });
      }
      else
        Utils.LogMessageToDMs($"Persistent placeable {plc.Name} in area {plc.Area.Name} does not have a valid ID !");
    }
    public static void HandlePlaceableUsed(PlaceableEvents.OnUsed onUsed)
    {
      Log.Info($"{onUsed.UsedBy.Name} used {onUsed.Placeable.Tag}");
      if (!Players.TryGetValue(onUsed.UsedBy.ControllingPlayer.ControlledCreature, out Player player))
        return;

      switch (onUsed.Placeable.Tag)
      {
        case "respawn_neutral":
          player.Respawn();
          break;
        case "respawn_radiant":
          player.Respawn();
          break;
        case "respawn_dire":
          player.Respawn();
          break;
        case "theater_rope":

          VisibilityMode visibilty;
          if (onUsed.UsedBy.Area.GetObjectVariable<LocalVariableInt>("_THEATER_CURTAIN_OPEN").HasNothing)
          {
            visibilty = VisibilityMode.Hidden;
            onUsed.UsedBy.Area.GetObjectVariable<LocalVariableInt>("_THEATER_CURTAIN_OPEN").Value = 1;
          }
          else
          {
            visibilty = VisibilityMode.Visible;
            onUsed.UsedBy.Area.GetObjectVariable<LocalVariableInt>("_THEATER_CURTAIN_OPEN").Delete();
          }

          foreach (NwPlaceable plc in onUsed.UsedBy.Area.FindObjectsOfTypeInArea<NwPlaceable>().Where(o => o.Tag == "theater_curtain"))
            plc.VisibilityOverride = visibilty;

          break;
        case "portal_start":
          onUsed.UsedBy.Location = NwObject.FindObjectsWithTag<NwWaypoint>("WP_START_NEW_CHAR").FirstOrDefault().Location;
          break;
      }
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
      if (onOpen.Door.TransitionTarget is NwDoor)
        await ((NwDoor)onOpen.Door.TransitionTarget).Close();
    }
    private void HandleCloseEnchantementBassin(PlaceableEvents.OnClose onClose)
    {
      NwCreature oPC = onClose.ClosedBy;

      if (!Players.TryGetValue(oPC, out Player player))
        return;

      NwItem oItem = onClose.Placeable.Inventory.Items.FirstOrDefault();

      if (oItem == null)
      {
        player.oid.SendServerMessage("Aucun objet valide n'a été déposé dans le bassin.");
        return;
      }

      if (oItem.BaseItem.EquipmentSlots == EquipmentSlots.None)
      {
        player.oid.SendServerMessage("Impossible d'enchanter un objet non équippable.");
        return;
      }

      if (oItem.PlotFlag)
      {
        player.oid.SendServerMessage("Impossible d'enchanter cet objet.");
        return;
      }

      EnchantmentBasinSystem.GetEnchantmentBasinFromTag(oItem.Tag).DrawMenu(player, oItem, onClose.Placeable);
    }
    private void HandleSetUpDeadCreatureCorpse(CreatureEvents.OnSpawn onSpawn)
    {
      onSpawn.Creature.ApplyEffect(EffectDuration.Instant, Effect.Death());
    }
    public static void OnUsedPlayerOwnedShop(PlaceableEvents.OnUsed onUsed)
    {
      if (!Players.TryGetValue(onUsed.UsedBy, out Player player))
        return;

      if (onUsed.Placeable.GetObjectVariable<LocalVariableInt>("_OWNER_ID").Value == player.characterId)
      {
        PlayerOwnedShop.DrawMainPage(player, onUsed.Placeable);
      }
      else
      {
        NwStore shop = onUsed.Placeable.GetNearestObjectsByType<NwStore>().FirstOrDefault(s => s.GetObjectVariable<LocalVariableInt>("_SHOP_ID").Value == onUsed.Placeable.GetObjectVariable<LocalVariableInt>("_SHOP_ID").Value);
        
        if (shop == null)
        {
          player.oid.SendServerMessage("Cette boutique n'est pas accessible pour le moment.", ColorConstants.Orange);
          return;
        }

        shop.OnOpen -= StoreSystem.OnOpenOtherPlayerShop;
        shop.OnOpen += StoreSystem.OnOpenOtherPlayerShop;
        shop.Open(player.oid);
      }
    }
    public static void OnUsedPlayerOwnedAuction(PlaceableEvents.OnUsed onUsed)
    {
      if (!Players.TryGetValue(onUsed.UsedBy, out Player player))
        return;

      if (onUsed.Placeable.GetObjectVariable<LocalVariableInt>("_OWNER_ID").Value == player.characterId)
      {
        PlayerOwnedAuction.DrawMainPage(player, onUsed.Placeable);
      }
      else
      {
        NwStore shop = onUsed.Placeable.GetNearestObjectsByType<NwStore>().FirstOrDefault(s => s.GetObjectVariable<LocalVariableInt>("_AUCTION_ID").Value == onUsed.Placeable.GetObjectVariable<LocalVariableInt>("_AUCTION_ID").Value);

        if (shop == null)
        {
          player.oid.SendServerMessage("Cette enchère n'est pas accessible pour le moment.", ColorConstants.Orange);
          return;
        }

        shop.OnOpen -= StoreSystem.OnOpenOtherPlayerAuction;
        shop.OnOpen += StoreSystem.OnOpenOtherPlayerAuction;
        shop.Open(player.oid);
        PlayerOwnedAuction.GetAuctionPrice(player, shop, onUsed.Placeable);
      }
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
      else if(availableSlots == 1)
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

      if(onClick.Placeable.GetObjectVariable<LocalVariableInt>("ownerId").Value != player.characterId)
      {
        player.oid.SendServerMessage("Vous avez beau avancer la main, le coffre semble rester hors d'atteinte, inaccessible. De plus, vous entendez un lointain cliquetis de chaînes dans les tréfonds de votres esprit. Quelque chose vient de se mettre en marche.", ColorConstants.Red);
        onClick.ClickedBy.ControlledCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSpellMantleUse));
        return;
      }

      if (player.windows.ContainsKey("bankStorage"))
        ((Player.BankStorageWindow)player.windows["bankStorage"]).CreateWindow();
      else
        player.windows.Add("bankStorage", new Player.BankStorageWindow(player));
    }
    private async void CheckMateriaInventory(DoorEvents.OnAreaTransitionClick onClick)
    {
      if (!onClick.ClickedBy.ControlledCreature.Inventory.Items.Any(i => i.Tag == "craft_resource"))
        await onClick.ClickedBy.ControlledCreature.JumpToObject(onClick.Door.TransitionTarget);
      else
      {
        onClick.ClickedBy.SendServerMessage("Une sorte de mur de force vous empêche de pénétrer plus avant dans la cité.\nUn frisson remonte le long de votre moëlle épinière alors que vous avez la certitude que quelque chose puissant porte un regard soupçonneux sur vous.\n\nLes miliciens vous signaleront que la Loi interdit d'entrer en possession de matéria et qu'il vous faut les déposer à l'entrepôt derrière vous.", ColorConstants.Red);
        await onClick.ClickedBy.ControlledCreature.ClearActionQueue();
        onClick.ClickedBy.ControlledCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSpellMantleUse));
        return;
      }
    }
    public static void StartIntroMirrorDialog(PlaceableEvents.OnUsed onUsed)
    {
      if (Players.TryGetValue(onUsed.UsedBy, out Player player))
      {
        if (player.windows.ContainsKey("introMirror"))
          ((Player.IntroMirroWindow)player.windows["introMirror"]).CreateWindow();
        else
          player.windows.Add("introMirror", new Player.IntroMirroWindow(player));
      }
    }
    public static void StartBodyModifierDialog(PlaceableEvents.OnUsed onUsed)
    {
      if (Players.TryGetValue(onUsed.UsedBy, out Player player))
      {
        if (player.windows.ContainsKey("bodyAppearanceModifier"))
          ((Player.BodyAppearanceWindow)player.windows["bodyAppearanceModifier"]).CreateWindow();
        else
          player.windows.Add("bodyAppearanceModifier", new Player.BodyAppearanceWindow(player));
      }
    }
    public static void OpenRefineryWindow(PlaceableEvents.OnUsed onUsed)
    {
      if (Players.TryGetValue(onUsed.UsedBy, out Player player))
      {
        if (player.windows.ContainsKey("refinery"))
          ((Player.RefineryWindow)player.windows["refinery"]).CreateWindow(ResourceType.Ore);
        else
          player.windows.Add("refinery", new Player.RefineryWindow(player, ResourceType.Ore));
      }
    }
    public static void OpenWoodworkWindow(PlaceableEvents.OnUsed onUsed)
    {
      if (Players.TryGetValue(onUsed.UsedBy, out Player player))
      {
        if (player.windows.ContainsKey("refinery"))
          ((Player.RefineryWindow)player.windows["refinery"]).CreateWindow(ResourceType.Wood);
        else
          player.windows.Add("refinery", new Player.RefineryWindow(player, ResourceType.Wood));
      }
    }
    public static void OpenTanneryWindow(PlaceableEvents.OnUsed onUsed)
    {
      if (Players.TryGetValue(onUsed.UsedBy, out Player player))
      {
        if (player.windows.ContainsKey("refinery"))
          ((Player.RefineryWindow)player.windows["refinery"]).CreateWindow(ResourceType.Pelt);
        else
          player.windows.Add("refinery", new Player.RefineryWindow(player, ResourceType.Pelt));
      }
    }
  }
}
