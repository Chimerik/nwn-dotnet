﻿using System;
using System.Collections.Generic;
using static NWN.Systems.SkillSystem;
using NWN.Systems.Craft;
using Anvil.API;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Anvil.Services;
using NWN.Systems.Alchemy;
using NWN.Core.NWNX;
using JournalEntry = Anvil.API.JournalEntry;
using Anvil.API.Events;
using System.IO;
using System.Text.Json;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public NwPlayer oid { get; set; }
      public DateTime mapLoadingTime { get; set; }
      public readonly int accountId;
      public readonly int characterId;
      public Location location { get; set; }
      public int bonusRolePlay { get; set; }
      public int currentLanguage { get; set; }
      public Boolean DoJournalUpdate { get; set; }
      public int bankGold { get; set; }
      public PlayerJournal playerJournal { get; set; }
      public DateTime dateLastSaved { get; set; }
      public Job craftJob { get; set; }
      public Location previousLocation { get; set; }
      public Menu menu { get; }
      public NwCreature deathCorpse { get; set; }
      public QuickbarType loadedQuickBar { get; set; }
      public string serializedQuickbar { get; set; }
      public Arena.PlayerData pveArena { get; set; }
      public Cauldron alchemyCauldron { get; set; }
      public PcState pcState { get; set; }
      public DateTime? previousSPCalculation { get; set; }
      public DateTime? lastCraftUpdate { get; set; }

      public List<NwPlayer> listened = new List<NwPlayer>();
      public List<int> mutedList = new List<int>();
      public Dictionary<uint, Player> blocked = new Dictionary<uint, Player>();
      public Dictionary<Feat, int> learntCustomFeats = new Dictionary<Feat, int>();
      public Dictionary<int, LearnableSkill> learnableSkills = new Dictionary<int, LearnableSkill>();
      public Dictionary<int, LearnableSpell> learnableSpells = new Dictionary<int, LearnableSpell>();
      public Dictionary<int, MapPin> mapPinDictionnary = new Dictionary<int, MapPin>();
      public Dictionary<string, byte[]> areaExplorationStateDictionnary = new Dictionary<string, byte[]>();
      public Dictionary<ChatChannel, Color> chatColors = new Dictionary<ChatChannel, Color>();
      public Dictionary<string, PlayerWindow> windows = new Dictionary<string, PlayerWindow>();
      public Dictionary<string, NuiRect> windowRectangles = new Dictionary<string, NuiRect>();
      public Dictionary<string, int> openedWindows = new Dictionary<string, int>();
      public List<ChatLine> readChatLines = new List<ChatLine>();

      public Dictionary<string, int> materialStock = new Dictionary<string, int>();
      public List<CraftResource> craftResourceStock = new List<CraftResource>();

      public enum PcState
      {
        Offline,
        Online,
        AFK
      }

      public Player(NwPlayer nwobj)
      {
        oid = nwobj;
        menu = new PrivateMenu(this);
        pveArena = new Arena.PlayerData();
        
        Log.Info($"accountID : {this.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("accountId").Value}");

        if(!oid.IsDM)
        {
          if (this.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("accountId").HasNothing && !oid.IsDM)
            InitializeNewPlayer();

          this.accountId = this.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("accountId").Value;

          if (this.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("characterId").HasNothing && !oid.IsDM)
            InitializeNewCharacter();

          this.characterId = this.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("characterId").Value;

          InitializePlayer();
        }
        else
          InitializeDM();

        Players.Add(nwobj.LoginCreature, this);

        Log.Info($"Player first initialization : DONE");
      }

      public void EmitKeydown(MenuFeatEventArgs e)
      {
        OnKeydown(this, e);
      }

      public event EventHandler<MenuFeatEventArgs> OnKeydown = delegate { };

      public class MenuFeatEventArgs : EventArgs
      {
        public Feat feat { get; }

        public MenuFeatEventArgs(Feat feat)
        {
          this.feat = feat;
        }
      }
      public void LoadMenuQuickbar(QuickbarType type)
      {
        if (this.loadedQuickBar == QuickbarType.Invalid)
        {
          PlayerQuickBarButton emptyQBS = new PlayerQuickBarButton();

          switch (type)
          {
            case QuickbarType.Menu:

              this.serializedQuickbar = oid.ControlledCreature.SerializeQuickbar().ToBase64EncodedString();
              emptyQBS.ObjectType = QuickBarButtonType.Empty;

              if (menu.choices.Count > 0)
              {
                oid.ControlledCreature.AddFeat(CustomFeats.CustomMenuDOWN);
                oid.ControlledCreature.AddFeat(CustomFeats.CustomMenuUP);
                oid.ControlledCreature.AddFeat(CustomFeats.CustomMenuSELECT);
                oid.ControlledCreature.AddFeat(CustomFeats.CustomMenuEXIT);

                emptyQBS.ObjectType = QuickBarButtonType.Feat;
                
                if (this.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_MENU_HOTKEYS_SWAPPED").HasNothing)
                {
                  emptyQBS.Param1 = (int)CustomFeats.CustomMenuDOWN;
                  oid.ControlledCreature.SetQuickBarButton(0, emptyQBS);
                  emptyQBS.Param1 = (int)CustomFeats.CustomMenuUP;
                  oid.ControlledCreature.SetQuickBarButton(1, emptyQBS);
                }
                else
                {
                  emptyQBS.Param1 = (int)CustomFeats.CustomMenuDOWN;
                  oid.ControlledCreature.SetQuickBarButton(1, emptyQBS);
                  emptyQBS.Param1 = (int)CustomFeats.CustomMenuUP;
                  oid.ControlledCreature.SetQuickBarButton(0, emptyQBS);
                }
                
                emptyQBS.Param1 = (int)CustomFeats.CustomMenuSELECT;
                oid.ControlledCreature.SetQuickBarButton(2, emptyQBS);
              }

              emptyQBS.Param1 = (int)CustomFeats.CustomMenuEXIT;
              oid.ControlledCreature.SetQuickBarButton(3, emptyQBS);

              this.loadedQuickBar = QuickbarType.Menu;
              break;
            case QuickbarType.Sit:
              oid.ControlledCreature.AddFeat(CustomFeats.CustomMenuDOWN);
              oid.ControlledCreature.AddFeat(CustomFeats.CustomMenuUP);
              oid.ControlledCreature.AddFeat(CustomFeats.CustomPositionRight);
              oid.ControlledCreature.AddFeat(CustomFeats.CustomPositionLeft);
              oid.ControlledCreature.AddFeat(CustomFeats.CustomPositionForward);
              oid.ControlledCreature.AddFeat(CustomFeats.CustomPositionBackward);
              oid.ControlledCreature.AddFeat(CustomFeats.CustomPositionRotateRight);
              oid.ControlledCreature.AddFeat(CustomFeats.CustomPositionRotateLeft);
              oid.ControlledCreature.AddFeat(CustomFeats.CustomMenuEXIT);

              this.serializedQuickbar = oid.ControlledCreature.SerializeQuickbar().ToBase64EncodedString();

              for (int i = 0; i < 12; i++)
                oid.ControlledCreature.SetQuickBarButton((byte)i, emptyQBS);

              emptyQBS.ObjectType = QuickBarButtonType.Feat;
              emptyQBS.Param1 = (int)CustomFeats.CustomMenuDOWN;
              oid.ControlledCreature.SetQuickBarButton(0, emptyQBS);
              emptyQBS.Param1 = (int)CustomFeats.CustomMenuUP;
              oid.ControlledCreature.SetQuickBarButton(1, emptyQBS);
              emptyQBS.Param1 = (int)CustomFeats.CustomPositionLeft;
              oid.ControlledCreature.SetQuickBarButton(2, emptyQBS);
              emptyQBS.Param1 = (int)CustomFeats.CustomPositionRight;
              oid.ControlledCreature.SetQuickBarButton(3, emptyQBS);
              emptyQBS.Param1 = (int)CustomFeats.CustomPositionForward;
              oid.ControlledCreature.SetQuickBarButton(4, emptyQBS);
              emptyQBS.Param1 = (int)CustomFeats.CustomPositionBackward;
              oid.ControlledCreature.SetQuickBarButton(5, emptyQBS);
              emptyQBS.Param1 = (int)CustomFeats.CustomPositionRotateLeft;
              oid.ControlledCreature.SetQuickBarButton(6, emptyQBS);
              emptyQBS.Param1 = (int)CustomFeats.CustomPositionRotateRight;
              oid.ControlledCreature.SetQuickBarButton(7, emptyQBS);
              emptyQBS.Param1 = (int)CustomFeats.CustomMenuEXIT;
              oid.ControlledCreature.SetQuickBarButton(8, emptyQBS);

              this.loadedQuickBar = QuickbarType.Sit;
              this.OnKeydown += this.menu.HandleMenuFeatUsed;
              break;
          }
        }
      }
      public async void TeleportPlayerToSavedLocation()
      {
        await NwTask.WaitUntil(() => oid.LoginCreature.Location.Area != null);
        oid.LoginCreature.Location = location;
      }
      public async void InitializePlayerOpenedWindows()
      {
        await NwTask.WaitUntil(() => oid.LoginCreature.GetObjectVariable<LocalVariableBool>("_ASYNC_INIT_DONE").HasValue);

        foreach (string window in openedWindows.Keys)
          CreatePlayerWindow(window);
      }
      public void CreatePlayerWindow(string window)
      {
        switch (window)
        {
          case "chat":
           /* if (windows.ContainsKey(window))
              ((ChatWriterWindow)windows[window]).CreateWindow();
            else
              windows.Add(window, new ChatWriterWindow(this));*/
            break;
          case "chatReader":
            if (windows.ContainsKey(window))
              ((ChatReaderWindow)windows[window]).CreateWindow();
            else
              windows.Add(window, new ChatReaderWindow(this));
            break;
          case "activeLearnable":
            if (windows.ContainsKey(window))
              ((ActiveLearnableWindow)windows[window]).CreateWindow();
            else
              windows.Add(window, new ActiveLearnableWindow(this));
            break;
        }
      }
      public async void InitializePlayerLearnableJobs()
      {
        await NwTask.WaitUntil(() => oid.LoginCreature.GetObjectVariable<LocalVariableBool>("_ASYNC_INIT_DONE").HasValue);

        if (learnableSkills.Any(l => l.Value.active) )
          learnableSkills.First(l => l.Value.active).Value.AwaitPlayerStateChangeToCalculateSPGain(this);

        else if(learnableSpells.Any(l => l.Value.active))
          learnableSpells.First(l => l.Value.active).Value.AwaitPlayerStateChangeToCalculateSPGain(this);

        /*int improvedHealth = 0;
      if (player.learnableSkills.ContainsKey(CustomSkill.ImprovedHealth))
        improvedHealth = player.learnableSkills[CustomSkill.ImprovedHealth].currentLevel;

      int toughness = 0;
      if (player.learnableSkills.ContainsKey(CustomSkill.Toughness))
        toughness = player.learnableSkills[CustomSkill.Toughness].currentLevel;

      player.oid.LoginCreature.LevelInfo[0].HitDie = (byte)(10
        + (1 + 3 * ((player.oid.LoginCreature.GetAbilityScore(Ability.Constitution, true) - 10) / 2)
        + toughness) * improvedHealth);*/

        if (oid.LoginCreature.HP <= 0)
          oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.Death());

        if (learntCustomFeats.ContainsKey(CustomFeats.ImprovedAttackBonus))
          oid.LoginCreature.BaseAttackBonus = (byte)(oid.LoginCreature.BaseAttackBonus + SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ImprovedAttackBonus, learntCustomFeats[CustomFeats.ImprovedAttackBonus]));

        pcState = Player.PcState.Online;

        await NwTask.Delay(TimeSpan.FromSeconds(5));
        oid.LoginCreature.GetObjectVariable<LocalVariableBool>("_ASYNC_INIT_DONE").Delete();
      }
      public void UnloadMenuQuickbar()
      {
        oid.ControlledCreature.RemoveFeat(CustomFeats.CustomMenuUP);
        oid.ControlledCreature.RemoveFeat(CustomFeats.CustomMenuDOWN);
        oid.ControlledCreature.RemoveFeat(CustomFeats.CustomMenuSELECT);
        oid.ControlledCreature.RemoveFeat(CustomFeats.CustomMenuEXIT);
        oid.ControlledCreature.RemoveFeat(CustomFeats.CustomPositionLeft);
        oid.ControlledCreature.RemoveFeat(CustomFeats.CustomPositionRight);
        oid.ControlledCreature.RemoveFeat(CustomFeats.CustomPositionForward);
        oid.ControlledCreature.RemoveFeat(CustomFeats.CustomPositionBackward);
        oid.ControlledCreature.RemoveFeat(CustomFeats.CustomPositionRotateLeft);
        oid.ControlledCreature.RemoveFeat(CustomFeats.CustomPositionRotateRight);

        bool returned = oid.ControlledCreature.DeserializeQuickbar(this.serializedQuickbar.ToByteArray());
        loadedQuickBar = QuickbarType.Invalid;
      }
      public void CraftJobProgression()
      {
        if (craftJob.IsActive())
        {
          craftJob.remainingTime = craftJob.remainingTime - (float)(DateTime.Now - dateLastSaved).TotalSeconds;

          if (craftJob.remainingTime < 0)
          {
            Log.Info($"craft job done. Acquiring item - Type : {craftJob.type} - BaseItem : {craftJob.baseItemType}");
            AcquireCraftedItem();
          }
        }
      }

      public async void AcquireCraftedItem()
      {
        switch (craftJob.type)
        {
          case Job.JobType.BlueprintCopy:
            NwItem bpCopy = ItemUtils.DeserializeAndAcquireItem(craftJob.craftedItem, oid.LoginCreature);
            bpCopy.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_RUNS").Value = 10;
            bpCopy.Name = $"Copie de {bpCopy.Name}";         
            break;
          case Job.JobType.BlueprintResearchMaterialEfficiency:
            NwItem improvedMEBP = ItemUtils.DeserializeAndAcquireItem(craftJob.craftedItem, oid.LoginCreature);
            improvedMEBP.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value = improvedMEBP.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value + 1;
            break;
          case Job.JobType.BlueprintResearchTimeEfficiency:
            NwItem researchedBP = ItemUtils.DeserializeAndAcquireItem(craftJob.craftedItem, oid.LoginCreature);
            researchedBP.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_TIME_EFFICIENCY").Value += 1;
            break;
          case Job.JobType.Enchantement:
            NwItem enchantedItem = ItemUtils.DeserializeAndAcquireItem(craftJob.craftedItem, oid.LoginCreature);

            int enchanteurChanceuxLevel = 0;
            if (learntCustomFeats.ContainsKey(CustomFeats.EnchanteurChanceux))
              enchanteurChanceuxLevel += GetCustomFeatLevelFromSkillPoints(CustomFeats.EnchanteurChanceux, learntCustomFeats[CustomFeats.EnchanteurChanceux]);

            if (NwRandom.Roll(Utils.random, 100) > enchanteurChanceuxLevel)
            {
              enchantedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value -= 1;
              if (enchantedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value <= 0)
                enchantedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Delete();
            }

            int enchanteurExpertLevel = 0;
            if (learntCustomFeats.ContainsKey(CustomFeats.EnchanteurExpert))
              enchanteurExpertLevel += GetCustomFeatLevelFromSkillPoints(CustomFeats.EnchanteurExpert, learntCustomFeats[CustomFeats.EnchanteurExpert]);

            int boost = 0;
            if (NwRandom.Roll(Utils.random, 100) <= enchanteurExpertLevel * 2)
              boost = 1;

            Craft.Collect.System.AddCraftedEnchantementProperties(enchantedItem, craftJob.material, boost, characterId);

            break;
          case Job.JobType.EnchantementReactivation:

            NwItem reactivatedItem = ItemUtils.DeserializeAndAcquireItem(craftJob.craftedItem, oid.LoginCreature);

            ItemProperty reactivatedIP = reactivatedItem.ItemProperties.FirstOrDefault(ip => ip.Tag.StartsWith($"ENCHANTEMENT_{craftJob.material}") && ip.Tag.Contains("INACTIVE"));

            Task waitLoopEnd = NwTask.Run(async () =>
            {
              ItemProperty deactivatedIP = reactivatedIP;
              await NwTask.Delay(TimeSpan.FromSeconds(0.1f));
              reactivatedItem.RemoveItemProperty(deactivatedIP);
              await NwTask.Delay(TimeSpan.FromSeconds(0.1f));
              deactivatedIP.Tag = reactivatedIP.Tag.Replace("_INACTIVE", "");
              reactivatedItem.AddItemProperty(deactivatedIP, EffectDuration.Permanent);
              await NwTask.Delay(TimeSpan.FromSeconds(0.1f));

              if (!reactivatedItem.ItemProperties.Any(ip => ip.Tag.Contains("_INACTIVE")) && reactivatedItem.GetObjectVariable<LocalVariableInt>("_REPAIR_DONE").HasValue)
              {
                reactivatedItem.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value = reactivatedItem.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value;
                reactivatedItem.GetObjectVariable<LocalVariableInt>("_REPAIR_DONE").Delete();
                oid.SendServerMessage($"Réactivation de {reactivatedItem.Name.ColorString(ColorConstants.White)} terminée. L'objet est comme neuf !", new Color(32, 255, 32));
              }
            });

            break;
          case Job.JobType.Recycling:
            NwItem recycledItem = NwItem.Deserialize(craftJob.craftedItem.ToByteArray());
            int recycledValue = recycledItem.GetObjectVariable<LocalVariableInt>("_BASE_COST").Value;

            if (learntCustomFeats.ContainsKey(CustomFeats.Recycler))
              recycledValue +=  recycledValue * 1 * GetCustomFeatLevelFromSkillPoints(CustomFeats.Recycler, learntCustomFeats[CustomFeats.Recycler]) / 100;

            if (materialStock.ContainsKey(craftJob.material))
              materialStock[craftJob.material] += recycledValue;
            else
              materialStock.Add(craftJob.material, recycledValue);

            oid.SendServerMessage($"Recyclage de {recycledItem.Name.ColorString(ColorConstants.White)} terminé. Vous en retirez {recycledValue} unité(s) de {craftJob.material}", new Color(32, 255, 32)) ;
            recycledItem.Destroy();

            break;
          case Job.JobType.Renforcement:
            NwItem reinforcedItem = ItemUtils.DeserializeAndAcquireItem(craftJob.craftedItem, oid.LoginCreature);

            reinforcedItem.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value += reinforcedItem.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value * 5 / 100;
            reinforcedItem.GetObjectVariable<LocalVariableInt>("_REINFORCEMENT_LEVEL").Value += 1;

            oid.SendServerMessage($"Renforcement de {reinforcedItem.Name.ColorString(ColorConstants.White)} terminé.", new Color(32, 255, 32));

            break;
          case Job.JobType.Repair:
            NwItem repairedItem = ItemUtils.DeserializeAndAcquireItem(craftJob.craftedItem, oid.LoginCreature);

            if(!repairedItem.ItemProperties.Any(ip => ip.Tag.Contains("_INACTIVE")))
            {
              repairedItem.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value = repairedItem.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value;
              oid.SendServerMessage($"Réparation de {repairedItem.Name.ColorString(ColorConstants.White)} terminée. L'objet est comme neuf !", new Color(32, 255, 32));
            }
            else
            {
              repairedItem.GetObjectVariable<LocalVariableInt>("_REPAIR_DONE").Value = 1;
              oid.SendServerMessage($"Réparation de {repairedItem.Name.ColorString(ColorConstants.White)} terminée. Reste cependant à réactiver les enchantements.", ColorConstants.Orange);
            }
            break;
          case Job.JobType.Alchemy:
            NwItem potion = ItemUtils.DeserializeAndAcquireItem(craftJob.craftedItem, oid.LoginCreature);
            potion.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").Value = oid.LoginCreature.Name;
            potion.GetObjectVariable<LocalVariableString>("_SERIALIZED_PROPERTIES").Value = craftJob.material;

            break;
          default:
            if (Craft.Collect.System.blueprintDictionnary.TryGetValue(craftJob.baseItemType, out Blueprint blueprint))
            {
              NwItem craftedItem;
              if (craftJob.craftedItem != "")
                craftedItem = ItemUtils.DeserializeAndAcquireItem(craftJob.craftedItem, oid.LoginCreature);
              else
              {
                craftedItem = await NwItem.Create(blueprint.craftedItemTag, oid.LoginCreature);
                craftedItem.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;
              }

              if (craftedItem == null)
              {
                oid.SendServerMessage($"Votre fabrication artisanale est terminée. Ouvrez votre journal pour obtenir le résultat de votre travail !");
                return;
              }

              Craft.Collect.System.AddCraftedItemProperties(craftedItem, craftJob.material);
              craftedItem.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").Value = oid.LoginCreature.Name;

              int artisanExceptionnelLevel = 0;
              if (learntCustomFeats.ContainsKey(CustomFeats.ArtisanExceptionnel))
                artisanExceptionnelLevel += GetCustomFeatLevelFromSkillPoints(CustomFeats.ArtisanExceptionnel, learntCustomFeats[CustomFeats.ArtisanExceptionnel]);

              if (NwRandom.Roll(Utils.random, 100) <= artisanExceptionnelLevel)
              {
                craftedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value += 1;
                oid.SendServerMessage("Votre talent d'artisan vous a permis de créer un objet exceptionnel disposant d'un emplacement d'enchantement supplémentaire !", ColorConstants.Navy);
              }

              int artisanAppliqueLevel = 0;
              if (learntCustomFeats.ContainsKey(CustomFeats.ArtisanApplique))
                artisanAppliqueLevel += GetCustomFeatLevelFromSkillPoints(CustomFeats.ArtisanApplique, learntCustomFeats[CustomFeats.ArtisanApplique]);

              if (NwRandom.Roll(Utils.random, 100) <= artisanAppliqueLevel * 3)
              {
                craftedItem.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value += craftedItem.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value * 20 / 100;
                oid.SendServerMessage("En travaillant de manière particulièrement appliquée, vous parvenez à fabriquer un objet plus résistant !", ColorConstants.Navy);
              }

              craftedItem.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value = craftedItem.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value;
              craftedItem.GetObjectVariable<LocalVariableInt>("_REPAIR_DONE").Delete();

              foreach (ItemProperty ip in craftedItem.ItemProperties.Where(ip => ip.Tag.Contains("INACTIVE")))
              {
                Task waitLoop = NwTask.Run(async () =>
                {
                  ItemProperty deactivatedIP = ip;
                  await NwTask.Delay(TimeSpan.FromSeconds(0.1f));
                  craftedItem.RemoveItemProperty(deactivatedIP);
                  await NwTask.Delay(TimeSpan.FromSeconds(0.1f));
                  deactivatedIP.Tag = deactivatedIP.Tag.Replace("_INACTIVE", "");
                  craftedItem.AddItemProperty(deactivatedIP, EffectDuration.Permanent);
                });
              }
            }
            else
            {
              oid.SendServerMessage("[ERREUR HRP] Il semble que votre dernière création soit invalide. Le staff a été informé du problème.");
              Utils.LogMessageToDMs($"AcquireCraftedItem : {oid.LoginCreature.Name} - Blueprint invalid - {craftJob.baseItemType} - For {oid.LoginCreature.Name}");
            }
            break;
        }

        craftJob.CloseCraftJournalEntry();
        craftJob = new Job(-10, "", 0, this);
      }
      public async void UpdateJournal()
      {
        if (oid.LoginCreature == null) // Si le joueur n'est pas co, pas la peine de mettre à jour son journal
          return;

        JournalEntry journalEntry;

        if (oid.LoginCreature.Location.Area == null && DoJournalUpdate)
        {
          Task waitAreaLoaded = NwTask.Run(async () =>
          {
            await NwTask.WaitUntil(() => oid.LoginCreature == null || oid.LoginCreature.Location.Area != null);
            await NwTask.Delay(TimeSpan.FromSeconds(1));
            UpdateJournal();
          });

          return;
        }

        if (playerJournal.craftJobCountDown != null && oid.LoginCreature.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value == 0)
        {
          journalEntry = oid.GetJournalEntry("craft_job");

          if (journalEntry != null)
          {
            journalEntry.Name = $"Travail artisanal - {Utils.StripTimeSpanMilliseconds((TimeSpan)(playerJournal.craftJobCountDown - DateTime.Now))}";
            oid.AddCustomJournalEntry(journalEntry, true);
          }
          this.CraftJobProgression();
          dateLastSaved = DateTime.Now;
        }

        /*if (learnables.Any(l => l.Value.active))
        {
          journalEntry = oid.GetJournalEntry("skill_job");

          if (journalEntry != null)
          {
            journalEntry.Name = $"Apprentissage - {Utils.StripTimeSpanMilliseconds((learnables.First(l => l.Value.active).Value.levelUpDate - DateTime.Now))}";
            oid.AddCustomJournalEntry(journalEntry, true);
            Log.Info("update journal : adding journal entry");
          }
        }

        if (DoJournalUpdate)
        {
          await NwTask.Delay(TimeSpan.FromSeconds(1));
          UpdateJournal();
        }*/
      }
      /*public async void rebootUpdate(int countDown)
      {
        await NwTask.Delay(TimeSpan.FromSeconds(1));

        if (!this.oid.LoginCreature.IsValid)
          return;

        JournalEntry journalEntry = oid.GetJournalEntry("reboot");
        journalEntry.Name = $"REBOOT SERVEUR - {countDown}";
        oid.AddCustomJournalEntry(journalEntry);
        
        if (countDown >= 0)
          this.rebootUpdate(countDown - 1);
      }*/
      public string CheckDBPlayerAccount()
      {
        var result = SqLiteUtils.SelectQuery("PlayerAccounts",
          new List<string>() { { "accountName" } },
          new List<string[]>() { new string[] { "rowId", accountId.ToString() } });

        if (result.Result == null)
          return "";

        return result.Result.GetString(0);
      }
      // Take gold from the PC or from his bank account
      public void PayOrBorrowGold(int price)
      {
        int pocketGold = (int)oid.LoginCreature.Gold;

        if (pocketGold >= price)
        {
          oid.LoginCreature.TakeGold(price);
          return;
        }

        var borrowedGold = price - pocketGold;
        bankGold -= borrowedGold;

        oid.SendServerMessage($"Vous ne disposez pas de la somme requise. {price} pièces d'or ont donc été prélevées sur votre compte.");
      }
      public async Task<bool> WaitForPlayerInputInt()
      {
        this.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_AWAITING_PLAYER_INPUT").Value = 1;

        this.oid.OnPlayerChat -= ChatSystem.HandlePlayerInputInt;
        this.oid.OnPlayerChat += ChatSystem.HandlePlayerInputInt;

        CancellationTokenSource tokenSource = new CancellationTokenSource();
        
        Task awaitPlayerCancellation = NwTask.WaitUntil(() => !this.oid.IsValid || this.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_PLAYER_INPUT_CANCELLED").HasValue, tokenSource.Token);
        Task awaitPlayerInput = NwTask.WaitUntil(() => this.oid.IsValid && this.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").HasValue, tokenSource.Token);

        await NwTask.WhenAny(awaitPlayerInput, awaitPlayerCancellation);
        tokenSource.Cancel();

        if (this.oid.IsValid)
          this.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_PLAYER_INPUT_CANCELLED").Delete();

        if (awaitPlayerInput.IsCompletedSuccessfully)
          return true;
        else
          return false;
      }
      public async Task<bool> WaitForPlayerInputByte()
      {
        oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_AWAITING_PLAYER_INPUT").Value = 1;

        oid.OnPlayerChat -= ChatSystem.HandlePlayerInputByte;
        oid.OnPlayerChat += ChatSystem.HandlePlayerInputByte;

        CancellationTokenSource tokenSource = new CancellationTokenSource();
        Task awaitPlayerCancellation = NwTask.WaitUntil(() => !oid.IsValid || oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_PLAYER_INPUT_CANCELLED").HasValue, tokenSource.Token);
        Task awaitPlayerInput = NwTask.WaitUntil(() => oid.IsValid && oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").HasValue, tokenSource.Token);

        await NwTask.WhenAny(awaitPlayerInput, awaitPlayerCancellation);
        tokenSource.Cancel();

        if (oid.IsValid)
          oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_PLAYER_INPUT_CANCELLED").Delete();

        if (awaitPlayerInput.IsCompletedSuccessfully)
          return true;
        else
          return false;
      }
      public async Task<bool> WaitForPlayerInputString()
      {
        this.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_AWAITING_PLAYER_INPUT").Value = 1;

        this.oid.OnPlayerChat -= ChatSystem.HandlePlayerInputString;
        this.oid.OnPlayerChat += ChatSystem.HandlePlayerInputString;

        CancellationTokenSource tokenSource = new CancellationTokenSource();
        Task awaitPlayerCancellation = NwTask.WaitUntil(() => !this.oid.IsValid || this.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_PLAYER_INPUT_CANCELLED").HasValue, tokenSource.Token);
        Task awaitPlayerInput = NwTask.WaitUntil(() => this.oid.IsValid && this.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").HasValue, tokenSource.Token);

        await NwTask.WhenAny(awaitPlayerInput, awaitPlayerCancellation);
        tokenSource.Cancel();

        if (this.oid.IsValid)
          this.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_PLAYER_INPUT_CANCELLED").Delete();

        if (awaitPlayerInput.IsCompletedSuccessfully)
          return true;
        else
          return false;
      }

      public async void HandleAsyncQueryFeedback(bool awaitedQuery, string messageOK, string messageKO)
      {
        await NwTask.SwitchToMainThread();

        if (awaitedQuery)
          oid.SendServerMessage(messageOK, new Color(32, 255, 32));
        else
          oid.SendServerMessage(messageKO, ColorConstants.Red);
      }
      
      public double GetSkillPointsPerSecond(Learnable learnable)
      {
        double pointsPerSecond = (oid.LoginCreature.GetAbilityScore(learnable.primaryAbility) + (oid.LoginCreature.GetAbilityScore(learnable.secondaryAbility) / 2.0)) / 60.0; // Il faut laisser les .0 sinon ce ne sont plus des doubles et KABOOM

        switch (bonusRolePlay)
        {
          case 0:
            pointsPerSecond = pointsPerSecond * 0.1;
            break;
          case 1:
            pointsPerSecond = pointsPerSecond * 0.9;
            break;
          case 3:
            pointsPerSecond = pointsPerSecond * 1.1;
            break;
          case 4:
            pointsPerSecond = pointsPerSecond * 1.2;
            break;
          case 100:
            pointsPerSecond = pointsPerSecond * 10;
            break;
        }

        if (pcState == PcState.Offline)
        {
          pointsPerSecond = pointsPerSecond * 0.6;
          Log.Info($"{oid.LoginCreature.Name} was not connected. Applying 40 % malus.");
        }
        else if (pcState == PcState.AFK)
        {
          pointsPerSecond = pointsPerSecond * 0.8;
          Log.Info($"{oid.LoginCreature.Name} was afk. Applying 20 % malus.");
        }

        if (oid.LoginCreature.KnowsFeat(Feat.QuickToMaster))
          pointsPerSecond = pointsPerSecond * 1.1;

        //Log.Info($"SP CALCULATION - {player.oid.Name} - {SP} SP.");

        return pointsPerSecond;
      }

      private void HandleGainedGold(OnInventoryGoldAdd onGainedGold)
      {
        if (Players.TryGetValue(onGainedGold.Creature, out Player player))
          player.oid.ExportCharacter();
      }
      private void HandleLostGold(OnInventoryGoldRemove onLostGold)
      {
        if (Players.TryGetValue(onLostGold.Creature, out Player player))
          player.oid.ExportCharacter();
      }
      public async void SaveMapPinsToDatabase()
      {
        using (var stream = new MemoryStream())
        {
          await JsonSerializer.SerializeAsync(stream, mapPinDictionnary);
          stream.Position = 0;
          using var reader = new StreamReader(stream);
          string serializedJson = await reader.ReadToEndAsync();

          SqLiteUtils.UpdateQuery("PlayerAccounts",
            new List<string[]>() { new string[] { "mapPins", serializedJson } },
            new List<string[]>() { new string[] { "rowid", accountId.ToString() } });
        }
      }
      private void DisableItemAppearanceFeedbackMessages()
      {
        feedbackService.AddFeedbackMessageFilter(FeedbackMessage.ItemReceived, oid);
        feedbackService.AddFeedbackMessageFilter(FeedbackMessage.ItemLost, oid);
        feedbackService.AddFeedbackMessageFilter(FeedbackMessage.EquipWeaponSwappedOut, oid);
        feedbackService.AddFeedbackMessageFilter(FeedbackMessage.EquipSkillSpellModifiers, oid);
        feedbackService.AddFeedbackMessageFilter(FeedbackMessage.InventoryFull, oid);
        feedbackService.AddFeedbackMessageFilter(FeedbackMessage.WeightTooEncumberedToRun, oid);
        feedbackService.AddFeedbackMessageFilter(FeedbackMessage.WeightTooEncumberedWalkSlow, oid);
        //feedbackService.AddFeedbackMessageFilter(FeedbackMessage.SendMessageToPc, player.oid);
      }
      private void EnableItemAppearanceFeedbackMessages()
      {
        feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.ItemReceived, oid);
        feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.ItemLost, oid);
        feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.EquipWeaponSwappedOut, oid);
        feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.EquipSkillSpellModifiers, oid);
        feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.InventoryFull, oid);
        feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.WeightTooEncumberedToRun, oid);
        feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.WeightTooEncumberedWalkSlow, oid);
        //feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.SendMessageToPc, oid);
      }
      private Learnable GetActiveLearnable()
      {
        if (learnableSpells.Any(l => l.Value.active))
          return learnableSpells.FirstOrDefault(l => l.Value.active).Value;
        else if (learnableSkills.Any(l => l.Value.active))
          return learnableSkills.FirstOrDefault(l => l.Value.active).Value;
        else
          return null;
      }
      private void ActivateSpotLight()
      {
        if (oid.ControlledCreature.GetObjectVariable<LocalVariableBool>("SPOTLIGHT_ON").HasNothing)
        {
          PlayerPlugin.ApplyLoopingVisualEffectToObject(oid.ControlledCreature, oid.ControlledCreature, 173);
          oid.ControlledCreature.GetObjectVariable<LocalVariableBool>("SPOTLIGHT_ON").Value = true;
        }
      }
      private void RemoveSpotLight()
      {
        if (oid.ControlledCreature.GetObjectVariable<LocalVariableBool>("SPOTLIGHT_ON").HasValue)
        {
          PlayerPlugin.ApplyLoopingVisualEffectToObject(oid.ControlledCreature, oid.ControlledCreature, 173);
          oid.ControlledCreature.GetObjectVariable<LocalVariableBool>("SPOTLIGHT_ON").Delete();
        }
      }
    }
  }
}
