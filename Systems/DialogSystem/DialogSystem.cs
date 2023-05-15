using NLog;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using static NWN.Systems.PlayerSystem;
using System.Linq;

namespace NWN.Systems
{
  [ServiceBinding(typeof(DialogSystem))]
  public class DialogSystem
  {
    public readonly Logger Log = LogManager.GetCurrentClassLogger();
    private byte[] forgeTool;
    private byte[] woodTool;
    private byte[] tannerTool;
    private byte[] extractor;
    private byte[] detector;
    //private SpellSystem spellSystem;

    public DialogSystem(/*SpellSystem spellSystem*/)
    {
      CreateGenericTools();
    }
    public void CreateGenericTools() 
    {
      CreateGenericTool("lighthammer");
      CreateGenericTool("handaxe");
      CreateGenericTool("sickle");
      CreateGenericTool("club");
      CreateGenericTool("helmet");
    }
    public void CreateGenericTool(string resRef)
    {
      NwItem craftTool = NwItem.Create(resRef, NwModule.Instance.StartingLocation);
      craftTool.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").Value = 1;
      craftTool.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value = 10;
      craftTool.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value = 1;
      craftTool.GetObjectVariable<LocalVariableInt>("SLOT0").Value = CustomInscription.MateriaProductionDurabilityMinor;
      craftTool.GetObjectVariable<LocalVariableInt>("SLOT0_DURABILITY").Value = 75;
      craftTool.AddItemProperty(ItemProperty.CastSpell((IPCastSpell)329, IPCastSpellNumUses.UnlimitedUse), EffectDuration.Permanent);

      switch (resRef)
      {
        case "lighthammer":
          craftTool.Name = "Marteau de forgeron";
          craftTool.Description = "Cet outil est calligraphié afin de permettre la manipulation et le durcissement de la matéria raffinée dans le but d'en produire des objets utiles.";
          forgeTool = craftTool.Serialize();
          break;

        case "handaxe":
          craftTool.Name = "Rabot d'ébéniste";
          craftTool.Description = "Cet outil est calligraphié afin de permettre la manipulation et le durcissement de la matéria raffinée dans le but d'en produire des objets utiles.";
          woodTool = craftTool.Serialize();
          break;

        case "dagger":
          craftTool.Name = "Couteau de tanneur";
          craftTool.Description = "Cet outil est calligraphié afin de permettre la manipulation et le durcissement de la matéria raffinée dans le but d'en produire des objets utiles.";
          tannerTool = craftTool.Serialize();
          break;

        case "club":
          craftTool.Name = "Extracteur de matéria";
          craftTool.Description = "Cet outil est calligraphié afin de permettre l'extraction de matéria à partir d'un dépôt naturel.";
          craftTool.GetObjectVariable<LocalVariableInt>($"SLOT0").Value = CustomInscription.MateriaExtractionDurabilityMinor;
          extractor = craftTool.Serialize();
          break;

        case "helmet":
          craftTool.Name = "Détecteur de matéria";
          craftTool.Description = "Cet outil est calligraphié afin de permettre la détection de dépôts naturels de matéria.";
          craftTool.GetObjectVariable<LocalVariableInt>($"SLOT0").Value = CustomInscription.MateriaDetectionDurabilityMinor;
          detector = craftTool.Serialize();
          break;
      }

      craftTool.Destroy();
    }

    public void StartBankerDialog(CreatureEvents.OnConversation onConversation)
    {
      if (!Players.TryGetValue(onConversation.LastSpeaker, out Player player))
        return;

      if (!player.windows.ContainsKey("bankCounter")) player.windows.Add("bankCounter", new Player.BankCounterWindow(player, onConversation.Creature));
      else ((Player.BankCounterWindow)player.windows["bankCounter"]).CreateWindow();        
    }
    public void StartBlacksmithDialog(CreatureEvents.OnConversation onConversation) // TODO => Mettre à dispo des outils avec enchantement de craft de base
    {
      if (Players.TryGetValue(onConversation.LastSpeaker, out Player player))
        HandleGenericShop(player, onConversation.Creature, "blacksmith_shop", ItemUtils.forgeBasicWeaponBlueprints, SkillSystem.forgeBasicSkillBooks, ItemUtils.forgeBasicArmorBlueprints, forgeTool);
    }
    public void StartWoodworkerDialog(CreatureEvents.OnConversation onConversation)
    {
      if (Players.TryGetValue(onConversation.LastSpeaker, out Player player))
        HandleGenericShop(player, onConversation.Creature, "woodworker_shop", ItemUtils.woodBasicBlueprints, SkillSystem.woodBasicSkillBooks, new int[] { }, woodTool);
    }
    public void StartTanneurDialog(CreatureEvents.OnConversation onConversation)
    {
      if (Players.TryGetValue(onConversation.LastSpeaker, out Player player))
        HandleGenericShop(player, onConversation.Creature, "tannery_shop", ItemUtils.leatherBasicWeaponBlueprints, SkillSystem.leatherBasicSkillBooks, ItemUtils.leatherBasicArmorBlueprints, tannerTool);
    }
    public void StartBibliothecaireDialog(CreatureEvents.OnConversation onConversation)
    {
      if (Players.TryGetValue(onConversation.LastSpeaker, out Player player))
        new Bibliothecaire(player, onConversation.Creature);
    }
    public void StartJukeboxDialog(CreatureEvents.OnConversation onConversation)
    {
      if (Players.TryGetValue(onConversation.LastSpeaker, out Player player))
      {
        if (player.windows.ContainsKey("jukebox"))
          ((Player.JukeBoxWindow)player.windows["jukebox"]).CreateWindow(onConversation.Creature);
        else
          player.windows.Add("jukebox", new Player.JukeBoxWindow(player, onConversation.Creature));
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
        HandleGenericShop(player, onConversation.Creature, "magic_shop", System.Array.Empty<BaseItemType>(), SkillSystem.shopBasicMagicSkillBooks, ItemUtils.shopBasicMagicScrolls);
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
          player.windows.Add("materiaStorage", new Player.MateriaStorageWindow(player, onConversation.Creature));
      }
    }
    private async void HandleGenericShop(Player player, NwCreature shopkeeper, string tag, BaseItemType[] basicBluePrints, int[] basicSkillBooks, int[] basicArmorAndScrolls, byte[] craftTool = null)
    {
      NwStore shop = shopkeeper.GetNearestObjectsByType<NwStore>().FirstOrDefault(s => s.Tag == tag);

      if (shop is null)
      {
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
              oScroll.GetObjectVariable<LocalVariableInt>("_ONE_USE_ONLY").Value = 1;
            }

            NwItem potion = await NwItem.Create("potion_cure_frog", shop, 1, "potion_core_influx");
            potion.Name = "Mélange mineur";
            potion.Description = "Une dose d'influx pur a été savamment conditionnée pour en retirer ses effets les plus néfastes et mettre en exergue ses incroyables bienfaits.\n\n" +
              "Quiconque absorbe le Mélange se voit doter d'une résilience permettant de résister aux plus rudes combats.\n" +
              "L'accès à la magie devient naturellement possible à tous, alors que celle-ci est d'ordinaire réservée aux créatures les plus fantastiques.\n" +
              "On dit aussi que ce fameux Mélange rend beau et prolonge la vie. Mais ces assertions restent discutables" +
              "En tout cas, qu'attendez-vous ? Mais attention, tout cela n'est que temporaire !\n\n" +
              "Note à la clientèle : le manque éventuellement ressentis suite à l'absence de ré-ingestion de Mélange pendant une période prolongée ne peut en aucun cas être retenu contre le vendeur.";

            potion.GetObjectVariable<LocalVariableInt>("_CORE_MAX_HP").Value = 80;
            potion.GetObjectVariable<LocalVariableInt>("_CORE_MAX_MANA").Value = 20;
            potion.GetObjectVariable<LocalVariableInt>("_CORE_REMAINING_HP").Value = 240;
            potion.GetObjectVariable<LocalVariableInt>("_CORE_REMAINING_MANA").Value = 120;
            potion.GetObjectVariable<LocalVariableInt>("_CORE_DURATION").Value = 28800;

            potion.BaseGoldValue = 15000;
            potion.AddGoldValue = 0;
            potion.Tag = "potion_core_influx";

            potion = await NwItem.Create("potion_cure_frog", shop, 1, "dose_influx_pur");
            potion.Name = "Dose d'influx pur";
            potion.Description = "L'influx est substance mystérieuse qu'on trouve plus facilement au sein des profondeurs de Similisse.\n\n" +
              "Il est certain que nous sommes encore loin d'avoir découvert toutes ses utilités.\n" +
              "Restez cependant prudent, car si le contact prolongé de cette substance à tendance à rendre la matière malléable et plus facile à travailler, elle rend aussi malléable la chair, les os et les organes, ce qui peut avoir des conséquences désastreuses.";

            potion.BaseGoldValue = 1;
            potion.AddGoldValue = 0;
            potion.Tag = "potion_core_influx";

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

        if (craftTool is not null)
        {
          NwItem tool = NwItem.Deserialize(extractor);
          shop.AcquireItem(tool);
          tool.BaseGoldValue = 5000;

          tool = NwItem.Deserialize(detector);
          shop.AcquireItem(tool);
          tool.BaseGoldValue = 5000;

          tool = NwItem.Deserialize(craftTool);
          shop.AcquireItem(tool);
          tool.BaseGoldValue = 5000;
        }

        shop.OnOpen += StoreSystem.OnOpenGenericStore;
        shop.OnClose += StoreSystem.OnCloseGenericStore;
      }

      shop.MarkDown = 0;
      shop.MarkDown = 0;
      shop.MarkDownStolen = 0;
      shop.Open(player.oid);
    }
  }
}
