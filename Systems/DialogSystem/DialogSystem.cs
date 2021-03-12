using NLog;
using NWN.API;
using NWN.API.Events;
using NWN.Services;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  [ServiceBinding(typeof(DialogSystem))]
  public class DialogSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public DialogSystem()
    {
      foreach (NwPlaceable plc in NwModule.FindObjectsWithTag<NwPlaceable>("bank_gold"))
        plc.OnUsed += StartGoldStealDialog;

      foreach (NwPlaceable plc in NwModule.FindObjectsWithTag<NwPlaceable>("intro_mirror"))
        plc.OnUsed += StartIntroMirrorDialog;

      foreach (NwPlaceable plc in NwModule.FindObjectsWithTag<NwPlaceable>("refinery"))
        plc.OnUsed += StartRefineryDialog;

      foreach (NwPlaceable plc in NwModule.FindObjectsWithTag<NwPlaceable>("decoupe"))
        plc.OnUsed += StartScierieDialog;

      foreach (NwPlaceable plc in NwModule.FindObjectsWithTag<NwPlaceable>("tannerie_peau"))
        plc.OnUsed += StartTanneryDialog;

      foreach (NwPlaceable plc in NwModule.FindObjectsWithTag<NwPlaceable>("hventes"))
        plc.OnUsed += StartAuctionHouseDialog;
    }

    public static void StartBankerDialog(CreatureEvents.OnConversation onConversation)
    {
      if (Players.TryGetValue(onConversation.LastSpeaker, out Player player))
        new Bank(player);
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
        new Jukebox(player, onConversation.CurrentSpeaker);
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
        new Storage(player);
    }
    public static void StartGoldStealDialog(PlaceableEvents.OnUsed onUsed)
    {
      if (Players.TryGetValue(onUsed.UsedBy, out Player player))
        new BankGold(player);
    }
    public static void StartIntroMirrorDialog(PlaceableEvents.OnUsed onUsed)
    {
      if (Players.TryGetValue(onUsed.UsedBy, out Player player))
        new IntroMirror(player, onUsed.Placeable);
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
