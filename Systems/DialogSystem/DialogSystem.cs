using NLog;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using static NWN.Systems.PlayerSystem;
using System.Linq;
using NWN.System;

namespace NWN.Systems
{
  [ServiceBinding(typeof(DialogSystem))]
  public class DialogSystem
  {
    public readonly Logger Log = LogManager.GetCurrentClassLogger();
    //private SpellSystem spellSystem;

    public DialogSystem(/*SpellSystem spellSystem*/)
    {
      //this.spellSystem = spellSystem;
      /*foreach (NwPlaceable plc in NwModule.FindObjectsWithTag<NwPlaceable>("bank_gold"))
        plc.OnUsed += StartGoldStealDialog;*/

      foreach (NwPlaceable plc in NwObject.FindObjectsWithTag<NwPlaceable>("hventes"))
        plc.OnUsed += StartAuctionHouseDialog;
    }

    public void StartBankerDialog(CreatureEvents.OnConversation onConversation)
    {
      if (!Players.TryGetValue(onConversation.LastSpeaker, out Player player))
        return;

      if (!player.windows.ContainsKey("bankCounter")) player.windows.Add("bankCounter", new Player.BankCounterWindow(player, onConversation.CurrentSpeaker));
      else ((Player.BankCounterWindow)player.windows["bankCounter"]).CreateWindow();        
    }
    public void StartBlacksmithDialog(CreatureEvents.OnConversation onConversation)
    {
      if (Players.TryGetValue(onConversation.LastSpeaker, out Player player))
        HandleGenericShop(player, onConversation.CurrentSpeaker, "blacksmith_shop", ItemUtils.forgeBasicWeaponBlueprints, SkillSystem.forgeBasicSkillBooks, ItemUtils.forgeBasicArmorBlueprints, new[] { "oreextractor", "forgehammer" });
    }
    public void StartWoodworkerDialog(CreatureEvents.OnConversation onConversation)
    {
      if (Players.TryGetValue(onConversation.LastSpeaker, out Player player))
        HandleGenericShop(player, onConversation.CurrentSpeaker, "woodworker_shop", ItemUtils.woodBasicBlueprints, SkillSystem.woodBasicSkillBooks, new int[] { }, new[] { "oreextractor", "forgehammer" });
    }
    public void StartTanneurDialog(CreatureEvents.OnConversation onConversation)
    {
      if (Players.TryGetValue(onConversation.LastSpeaker, out Player player))
        HandleGenericShop(player, onConversation.CurrentSpeaker, "tannery_shop", ItemUtils.leatherBasicWeaponBlueprints, SkillSystem.leatherBasicSkillBooks, ItemUtils.leatherBasicArmorBlueprints, new[] { "oreextractor", "forgehammer" });
    }
    public void StartBibliothecaireDialog(CreatureEvents.OnConversation onConversation)
    {
      if (Players.TryGetValue(onConversation.LastSpeaker, out Player player))
        new Bibliothecaire(player, onConversation.CurrentSpeaker);
    }
    public void StartJukeboxDialog(CreatureEvents.OnConversation onConversation)
    {
      if (Players.TryGetValue(onConversation.LastSpeaker, out Player player))
      {
        if (player.windows.ContainsKey("jukebox"))
          ((Player.JukeBoxWindow)player.windows["jukebox"]).CreateWindow(onConversation.CurrentSpeaker);
        else
          player.windows.Add("jukebox", new Player.JukeBoxWindow(player, onConversation.CurrentSpeaker));
      }
    }
    public void StartRumorsDialog(CreatureEvents.OnConversation onConversation)
    {
      if (Players.TryGetValue(onConversation.LastSpeaker, out Player player))
      {
        if (player.windows.ContainsKey("rumors"))
          ((Player.RumorsWindow)player.windows["rumors"]).CreateWindow();
        else
          player.windows.Add("rumors", new Player.RumorsWindow(player));
      }
    }
    public void StartTribunalShopDialog(CreatureEvents.OnConversation onConversation)
    {
      if (Players.TryGetValue(onConversation.LastSpeaker, out Player player))
        HandleGenericShop(player, onConversation.CurrentSpeaker, "magic_shop", new BaseItemType[] { }, SkillSystem.shopBasicMagicSkillBooks, ItemUtils.shopBasicMagicScrolls, new string[] { });
    }
    public void StartPvEArenaHostDialog(CreatureEvents.OnConversation onConversation)
    {
      /*if (Players.TryGetValue(onConversation.LastSpeaker, out Player player))
        Arena.WelcomeMenu.DrawMainPage(player, spellSystem);*/
    }
    public void StartMessengerDialog(CreatureEvents.OnConversation onConversation)
    {
      if (Players.TryGetValue(onConversation.LastSpeaker, out Player player))
        new Messenger(player);
    }
    public void StartStorageDialog(CreatureEvents.OnConversation onConversation)
    {
      if (Players.TryGetValue(onConversation.LastSpeaker, out Player player))
      {
        if (player.windows.ContainsKey("materiaStorage"))
          ((Player.MateriaStorageWindow)player.windows["materiaStorage"]).CreateWindow();
        else
          player.windows.Add("materiaStorage", new Player.MateriaStorageWindow(player, onConversation.CurrentSpeaker));
      }
    }
    
    public static void StartAuctionHouseDialog(PlaceableEvents.OnUsed onUsed)
    {
      if (Players.TryGetValue(onUsed.UsedBy, out Player player))
        new HotelDesVentes(player);
    }

    private async void HandleGenericShop(Player player, NwCreature shopkeeper, string tag, BaseItemType[] basicBluePrints, int[] basicSkillBooks, int[] basicArmorAndScrolls, string[] basicItems)
    {
      NwStore shop = shopkeeper.GetNearestObjectsByType<NwStore>().FirstOrDefault(s => s.Tag == tag);

      shop = NwStore.Create("generic_shop_res", shopkeeper.Location, false, tag);
      shop.GetObjectVariable<LocalVariableObject<NwCreature>>("_STORE_NPC").Value = shopkeeper;

      switch (tag)
      {
        case "blacksmith_shop":
        case "tannery_shop":
          foreach (int baseArmor in basicArmorAndScrolls)
          {
            NwItem oBlueprint = await NwItem.Create("blueprintgeneric", shop);
            ItemUtils.CreateShopArmorBlueprint(oBlueprint, baseArmor);
          }
          break;

        case "magic_shop":
          foreach (int itemPropertyId in basicArmorAndScrolls)
          {
            NwItem oScroll = await NwItem.Create("spellscroll", shop, 1, "scroll");
            NwSpell nwSpell = NwSpell.FromSpellId(NwGameTables.ItemPropertyTable.GetRow(15).SubTypeTable.GetInt(itemPropertyId, "SpellIndex").Value); // 15 = ItemProperty CastSpell
            oScroll.Name = nwSpell.Name.ToString();
            oScroll.Description = nwSpell.Description.ToString();
            oScroll.AddItemProperty(ItemProperty.CastSpell((IPCastSpell)itemPropertyId, IPCastSpellNumUses.SingleUse), EffectDuration.Permanent);
            oScroll.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;
          }
          break;
      }

      foreach (BaseItemType baseItemType in basicBluePrints)
      {
        NwItem oBlueprint = await NwItem.Create("blueprintgeneric", shop);
        ItemUtils.CreateShopWeaponBlueprint(oBlueprint, NwBaseItem.FromItemType(baseItemType));
      }

      foreach (int customSkill in basicSkillBooks)
      {
        NwItem skillBook = await NwItem.Create("skillbookgeneriq", shop, 1, "skillbook");
        ItemUtils.CreateShopSkillBook(skillBook, customSkill);
      }

      foreach (string item in basicItems)
      {
        NwItem craftTool = await NwItem.Create(item, shop, 1, item);
        craftTool.BaseGoldValue = 50;
        craftTool.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value = 10;
        craftTool.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;
      }

      shop.OnOpen += StoreSystem.OnOpenGenericStore;
      shop.Open(player.oid);
    }
  }
}
