﻿using NLog;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  [ServiceBinding(typeof(DialogSystem))]
  public class DialogSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public DialogSystem()
    {
      /*foreach (NwPlaceable plc in NwModule.FindObjectsWithTag<NwPlaceable>("bank_gold"))
        plc.OnUsed += StartGoldStealDialog;*/

      foreach (NwPlaceable plc in NwObject.FindObjectsWithTag<NwPlaceable>("intro_mirror"))
        plc.OnUsed += StartIntroMirrorDialog;

      foreach (NwPlaceable plc in NwObject.FindObjectsWithTag<NwPlaceable>("body_modifier"))
        plc.OnUsed += StartBodyModifierDialog;

      foreach (NwPlaceable plc in NwObject.FindObjectsWithTag<NwPlaceable>("refinery"))
        plc.OnUsed += StartRefineryDialog;

      foreach (NwPlaceable plc in NwObject.FindObjectsWithTag<NwPlaceable>("decoupe"))
        plc.OnUsed += StartScierieDialog;

      foreach (NwPlaceable plc in NwObject.FindObjectsWithTag<NwPlaceable>("tannerie_peau"))
        plc.OnUsed += StartTanneryDialog;

      foreach (NwPlaceable plc in NwObject.FindObjectsWithTag<NwPlaceable>("hventes"))
        plc.OnUsed += StartAuctionHouseDialog;
    }

    public static void StartBankerDialog(CreatureEvents.OnConversation onConversation)
    {
      if (!Players.TryGetValue(onConversation.LastSpeaker, out Player player))
        return;

      if (player.windows.ContainsKey("bankCounter"))
        ((Player.BankCounterWindow)player.windows["bankCounter"]).CreateWindow();
      else
        player.windows.Add("bankCounter", new Player.BankCounterWindow(player, onConversation.CurrentSpeaker));
    }
    public static void StartBlacksmithDialog(CreatureEvents.OnConversation onConversation)
    {
      if (Players.TryGetValue(onConversation.LastSpeaker, out Player player))
        new Blacksmith(player, onConversation.CurrentSpeaker);
    }
    public static void StartWoodworkerDialog(CreatureEvents.OnConversation onConversation)
    {
      if (Players.TryGetValue(onConversation.LastSpeaker, out Player player))
        new Woodworker(player, onConversation.CurrentSpeaker);
    }
    public static void StartTanneurDialog(CreatureEvents.OnConversation onConversation)
    {
      if (Players.TryGetValue(onConversation.LastSpeaker, out Player player))
        new Tanneur(player, onConversation.CurrentSpeaker);
    }
    public static void StartBibliothecaireDialog(CreatureEvents.OnConversation onConversation)
    {
      if (Players.TryGetValue(onConversation.LastSpeaker, out Player player))
        new Bibliothecaire(player, onConversation.CurrentSpeaker);
    }
    public static void StartJukeboxDialog(CreatureEvents.OnConversation onConversation)
    {
      if (Players.TryGetValue(onConversation.LastSpeaker, out Player player))
      {
        if (player.windows.ContainsKey("jukebox"))
          ((Player.JukeBoxWindow)player.windows["jukebox"]).CreateWindow(onConversation.CurrentSpeaker);
        else
          player.windows.Add("jukebox", new Player.JukeBoxWindow(player, onConversation.CurrentSpeaker));
      }
    }
    public static void StartRumorsDialog(CreatureEvents.OnConversation onConversation)
    {
      if (Players.TryGetValue(onConversation.LastSpeaker, out Player player))
      {
        if (player.windows.ContainsKey("rumors"))
          ((Player.RumorsWindow)player.windows["rumors"]).CreateWindow();
        else
          player.windows.Add("rumors", new Player.RumorsWindow(player, onConversation.CurrentSpeaker));
      }
    }
    public static void StartTribunalShopDialog(CreatureEvents.OnConversation onConversation)
    {
      if (Players.TryGetValue(onConversation.LastSpeaker, out Player player))
        new TribunalHotesse(player, onConversation.CurrentSpeaker);
    }
    public static void StartPvEArenaHostDialog(CreatureEvents.OnConversation onConversation)
    {
      if (Players.TryGetValue(onConversation.LastSpeaker, out Player player))
        Arena.WelcomeMenu.DrawMainPage(player);
    }
    public static void StartMessengerDialog(CreatureEvents.OnConversation onConversation)
    {
      if (Players.TryGetValue(onConversation.LastSpeaker, out Player player))
        new Messenger(player);
    }
    public static void StartStorageDialog(CreatureEvents.OnConversation onConversation)
    {
      if (Players.TryGetValue(onConversation.LastSpeaker, out Player player))
      {
        if (player.windows.ContainsKey("materiaStorage"))
          ((Player.MateriaStorageWindow)player.windows["materiaStorage"]).CreateWindow();
        else
          player.windows.Add("materiaStorage", new Player.MateriaStorageWindow(player, onConversation.CurrentSpeaker));
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
    public static void StartScierieDialog(PlaceableEvents.OnUsed onUsed)
    {
      if (Players.TryGetValue(onUsed.UsedBy, out Player player))
        new Scierie(player);
    }
    public static void StartRefineryDialog(PlaceableEvents.OnUsed onUsed)
    {
      if (Players.TryGetValue(onUsed.UsedBy, out Player player))
        new Refinery(player);
    }
    public static void StartTanneryDialog(PlaceableEvents.OnUsed onUsed)
    {
      if (Players.TryGetValue(onUsed.UsedBy, out Player player))
        new Tannerie(player);
    }
    public static void StartAuctionHouseDialog(PlaceableEvents.OnUsed onUsed)
    {
      if (Players.TryGetValue(onUsed.UsedBy, out Player player))
        new HotelDesVentes(player);
    }
  }
}
