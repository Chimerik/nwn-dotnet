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
      public int characterId { get; set; }
      public Location location { get; set; }
      public int bonusRolePlay { get; set; }
      public int currentLanguage { get; set; }
      public Boolean DoJournalUpdate { get; set; }
      public int bankGold { get; set; }
      public PlayerJournal playerJournal { get; set; }
      public DateTime dateLastSaved { get; set; }
      public Job craftJob { get; set; }
      public CraftJob newCraftJob { get; set; }
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
        await NwTask.WaitUntil(() => oid.LoginCreature.Location.Area != null);

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
        await NwTask.WaitUntil(() => oid.LoginCreature.Location.Area != null);

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
          oid.LoginCreature.BaseAttackBonus = (byte)(oid.LoginCreature.BaseAttackBonus + GetCustomFeatLevelFromSkillPoints(CustomFeats.ImprovedAttackBonus, learntCustomFeats[CustomFeats.ImprovedAttackBonus]));

        pcState = PcState.Online;
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

      public void AcquireCraftedItem()
      {
        switch (craftJob.type)
        {
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
        }

        craftJob.CloseCraftJournalEntry();
        craftJob = new Job(-10, "", 0, this);
      }
      public void UpdateJournal()
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
      public int GetWeaponMasteryLevel(BaseItemType baseItem)
      {
        int masteryLevel = 0;

        switch (baseItem)
        {
          case BaseItemType.Shortsword:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedShortSwordProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedShortSwordProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Battleaxe:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedBattleAxeProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedBattleAxeProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Bastardsword:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedBastardSwordProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedBastardSwordProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.LightFlail:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedLightFlailProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedLightFlailProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Warhammer:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedWarHammerProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedWarHammerProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.HeavyCrossbow:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedHeavyCrossbowProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedHeavyCrossbowProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.LightCrossbow:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedLightCrossBowProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedLightCrossBowProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Longbow:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedLongBowProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedLongBowProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.LightMace:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedLightMaceProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedLightMaceProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Halberd:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedHalberdProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedHalberdProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.TwoBladedSword:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedTwoBladedSwordProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedTwoBladedSwordProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Shortbow:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedShortBowProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedShortBowProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Greatsword:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedGreatSwordProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedGreatSwordProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Greataxe:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedGreatAxeProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedGreatAxeProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Dagger:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedDaggerProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedDaggerProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Club:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedClubProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedClubProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Dart:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedDartProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedDartProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.DireMace:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedDireMaceProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedDireMaceProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Doubleaxe:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedDoubleAxeProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedDoubleAxeProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.HeavyFlail:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedHeavyFlailProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedHeavyFlailProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.LightHammer:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedLightHammerProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedLightHammerProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Handaxe:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedHandAxeProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedHandAxeProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Kama:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedKamaProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedKamaProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Katana:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedKatanaProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedKatanaProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Kukri:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedKukriProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedKukriProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.MagicStaff:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedMagicStaffProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedMagicStaffProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Morningstar:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedMorningStarProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedMorningStarProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Quarterstaff:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedQuarterStaffProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedQuarterStaffProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Rapier:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedRapierProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedRapierProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Scimitar:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedScimitarProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedScimitarProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Scythe:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedScytheProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedScytheProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.ShortSpear:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedShortSpearProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedShortSpearProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Shuriken:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedShurikenProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedShurikenProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Sickle:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedSickleProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedSickleProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Sling:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedSlingProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedSlingProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.ThrowingAxe:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedThrowingAxeProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedThrowingAxeProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Trident:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedTridentProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedTridentProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.DwarvenWaraxe:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedDwarvenWarAxeProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedDwarvenWarAxeProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Whip:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedWhipProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedWhipProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Longsword:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedLongSwordProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedLongSwordProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.Gloves:
          case BaseItemType.Bracer:
          default:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedUnharmedProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedUnharmedProficiency].totalPoints;
            return masteryLevel;
        }
      }
      public int GetWeaponCritScienceLevel(BaseItemType baseItem)
      {
        int masteryLevel = 0;

        switch (baseItem)
        {
          case BaseItemType.Shortsword:
            if (learnableSkills.ContainsKey(CustomSkill.ShortSwordCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.ShortSwordCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Battleaxe:
            if (learnableSkills.ContainsKey(CustomSkill.BattleAxeCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.BattleAxeCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Bastardsword:
            if (learnableSkills.ContainsKey(CustomSkill.BastardSwordCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.BastardSwordCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.LightFlail:
            if (learnableSkills.ContainsKey(CustomSkill.LightFlailCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.LightFlailCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Warhammer:
            if (learnableSkills.ContainsKey(CustomSkill.WarHammerCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.WarHammerCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.HeavyCrossbow:
            if (learnableSkills.ContainsKey(CustomSkill.HeavyCrossbowCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.HeavyCrossbowCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.LightCrossbow:
            if (learnableSkills.ContainsKey(CustomSkill.LightCrossBowCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.LightCrossBowCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Longbow:
            if (learnableSkills.ContainsKey(CustomSkill.LongBowCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.LongBowCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.LightMace:
            if (learnableSkills.ContainsKey(CustomSkill.LightMaceCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.LightMaceCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Halberd:
            if (learnableSkills.ContainsKey(CustomSkill.HalberdCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.HalberdCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.TwoBladedSword:
            if (learnableSkills.ContainsKey(CustomSkill.TwoBladedSwordCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.TwoBladedSwordCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Shortbow:
            if (learnableSkills.ContainsKey(CustomSkill.ShortBowCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.ShortBowCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Greatsword:
            if (learnableSkills.ContainsKey(CustomSkill.GreatSwordCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.GreatSwordCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Greataxe:
            if (learnableSkills.ContainsKey(CustomSkill.GreatAxeCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.GreatAxeCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Dagger:
            if (learnableSkills.ContainsKey(CustomSkill.DaggerCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.DaggerCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Club:
            if (learnableSkills.ContainsKey(CustomSkill.ClubCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.ClubCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Dart:
            if (learnableSkills.ContainsKey(CustomSkill.DartCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.DartCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.DireMace:
            if (learnableSkills.ContainsKey(CustomSkill.DireMaceCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.DireMaceCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Doubleaxe:
            if (learnableSkills.ContainsKey(CustomSkill.DoubleAxeCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.DoubleAxeCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.HeavyFlail:
            if (learnableSkills.ContainsKey(CustomSkill.HeavyFlailCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.HeavyFlailCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.LightHammer:
            if (learnableSkills.ContainsKey(CustomSkill.LightHammerCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.LightHammerCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Handaxe:
            if (learnableSkills.ContainsKey(CustomSkill.HandAxeCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.HandAxeCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Kama:
            if (learnableSkills.ContainsKey(CustomSkill.KamaCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.KamaCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Katana:
            if (learnableSkills.ContainsKey(CustomSkill.KatanaCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.KatanaCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Kukri:
            if (learnableSkills.ContainsKey(CustomSkill.KukriCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.KukriCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.MagicStaff:
            if (learnableSkills.ContainsKey(CustomSkill.MagicStaffCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.MagicStaffCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Morningstar:
            if (learnableSkills.ContainsKey(CustomSkill.MorningStarCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.MorningStarCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Quarterstaff:
            if (learnableSkills.ContainsKey(CustomSkill.QuarterStaffCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.QuarterStaffCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Rapier:
            if (learnableSkills.ContainsKey(CustomSkill.RapierCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.RapierCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Scimitar:
            if (learnableSkills.ContainsKey(CustomSkill.ScimitarCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.ScimitarCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Scythe:
            if (learnableSkills.ContainsKey(CustomSkill.ScytheCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.ScytheCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.ShortSpear:
            if (learnableSkills.ContainsKey(CustomSkill.ShortSpearCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.ShortSpearCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Shuriken:
            if (learnableSkills.ContainsKey(CustomSkill.ShurikenCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.ShurikenCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Sickle:
            if (learnableSkills.ContainsKey(CustomSkill.SickleCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.SickleCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Sling:
            if (learnableSkills.ContainsKey(CustomSkill.SlingCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.SlingCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.ThrowingAxe:
            if (learnableSkills.ContainsKey(CustomSkill.ThrowingAxeCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.ThrowingAxeCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Trident:
            if (learnableSkills.ContainsKey(CustomSkill.TridentCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.TridentCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.DwarvenWaraxe:
            if (learnableSkills.ContainsKey(CustomSkill.DwarvenWarAxeCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.DwarvenWarAxeCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Whip:
            if (learnableSkills.ContainsKey(CustomSkill.WhipCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.WhipCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Longsword:
            if (learnableSkills.ContainsKey(CustomSkill.LongBowCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.LongBowCriticalScience].totalPoints;
            return masteryLevel;
          case BaseItemType.Gloves:
          case BaseItemType.Bracer:
          default:
            if (learnableSkills.ContainsKey(CustomSkill.UnharmedCriticalScience))
              masteryLevel = learnableSkills[CustomSkill.UnharmedCriticalScience].totalPoints;
            return masteryLevel;
        }
      }
      public int GetArmorProficiencyLevel(int baseACValue)
      {
        int masteryLevel = 0;

        switch (baseACValue)
        {
          case 1:
          case 2:
          case 3:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedLightArmorProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedLightArmorProficiency].totalPoints;
            return masteryLevel;
          case 4:
          case 5:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedMediumArmorProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedMediumArmorProficiency].totalPoints;
            return masteryLevel;
          case 6:
          case 7:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedHeavyArmorProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedHeavyArmorProficiency].totalPoints;
            return masteryLevel;
          case 8:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedFullPlateProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedFullPlateProficiency].totalPoints;
            return masteryLevel;
        }

        return masteryLevel;
      }
      public int GetShieldProficiencyLevel(BaseItemType baseItem)
      {
        int masteryLevel = 0;

        switch (baseItem)
        {
          case BaseItemType.SmallShield:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedLightShieldProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedLightShieldProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.LargeShield:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedMediumShieldProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedMediumShieldProficiency].totalPoints;
            return masteryLevel;
          case BaseItemType.TowerShield:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedHeavyShieldProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedHeavyShieldProficiency].totalPoints;
            return masteryLevel;
          default:
            if (learnableSkills.ContainsKey(CustomSkill.ImprovedDualWieldDefenseProficiency))
              masteryLevel = learnableSkills[CustomSkill.ImprovedDualWieldDefenseProficiency].totalPoints;
            return masteryLevel;
        }
      }
      public double GetItemCraftTime(NwItem item, int materiaCost)
      {
        BaseItemType baseItemType = (BaseItemType)item.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value;
        double timeEfficiency = (1 - (item.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_TIME_EFFICIENCY").Value / 100));

        if (baseItemType == BaseItemType.Armor)
          return GetArmorTimeCost(item.GetObjectVariable<LocalVariableInt>("_ARMOR_BASE_AC").Value, materiaCost) * timeEfficiency;

        return GetWeaponTimeCost(baseItemType, materiaCost) * timeEfficiency;
      }
      public double GetArmorTimeCost(int baseACValue, double materiaCost)
      {
        ArmorTable.Entry entry = Armor2da.armorTable.GetDataEntry(baseACValue);

        if (learnableSkills.ContainsKey(entry.craftLearnable))
          materiaCost *= learnableSkills[entry.craftLearnable].bonusReduction;

        int jobFeat = GetJobLearnableFromWorkshop(entry.workshop);

        if (learnableSkills.ContainsKey(jobFeat))
          materiaCost *= learnableSkills[jobFeat].bonusReduction;

        return materiaCost;
      }
      public double GetWeaponTimeCost(BaseItemType baseItemType, double materiaCost)
      {
        BaseItemTable.Entry entry = BaseItems2da.baseItemTable.GetBaseItemDataEntry(baseItemType);

        if (learnableSkills.ContainsKey(entry.craftLearnable))
          materiaCost *= learnableSkills[entry.craftLearnable].bonusReduction;

        int jobFeat = GetJobLearnableFromWorkshop(entry.workshop);

        if (learnableSkills.ContainsKey(jobFeat))
          materiaCost *= learnableSkills[jobFeat].bonusReduction;

        return materiaCost;
      }
      public double GetItemMateriaCost(NwItem item, int grade = 1)
      {
        BaseItemType baseItemType;

        if (item.Tag == "blueprint")
        {
          baseItemType = (BaseItemType)item.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value;

          if (baseItemType == BaseItemType.Armor)
            return GetArmorMateriaCost(item.GetObjectVariable<LocalVariableInt>("_ARMOR_BASE_AC").Value) * (1 - ((grade - 1) / 10));
        }
        else
          baseItemType = item.BaseItem.ItemType;

        if (baseItemType == BaseItemType.Armor)
          return GetArmorMateriaCost(item.BaseACValue) * (1 - ((grade - 1) / 10));

        return GetWeaponMateriaCost(baseItemType) * (1 - ((grade - 1) / 10));
      }

      public double GetArmorMateriaCost(int baseACValue)
      {
        ArmorTable.Entry entry = Armor2da.armorTable.GetDataEntry(baseACValue);
        double materiaCost = entry.cost * 1000;

        if (learnableSkills.ContainsKey(entry.craftLearnable))
          materiaCost *= learnableSkills[entry.craftLearnable].bonusReduction;

        int jobFeat = GetJobLearnableFromWorkshop(entry.workshop);

        if (learnableSkills.ContainsKey(jobFeat))
          materiaCost *= learnableSkills[jobFeat].bonusReduction;

        return materiaCost;
      }
      public double GetWeaponMateriaCost(BaseItemType baseItemType)
      {
        BaseItemTable.Entry entry = BaseItems2da.baseItemTable.GetBaseItemDataEntry(baseItemType);
        double materiaCost = entry.cost * 1000;

        if (learnableSkills.ContainsKey(entry.craftLearnable))
          materiaCost *= learnableSkills[entry.craftLearnable].bonusReduction;

        int jobFeat = GetJobLearnableFromWorkshop(entry.workshop);

        if (learnableSkills.ContainsKey(jobFeat))
          materiaCost *= learnableSkills[jobFeat].bonusReduction;

        return materiaCost;
      }
      public int GetJobLearnableFromWorkshop(string workshopTag)
      {
        switch(workshopTag)
        {
          case "forge":
            return CustomSkill.Blacksmith;
          case "scierie":
            return CustomSkill.Woodworker;
          case "tannerie":
            return CustomSkill.Tanner;
          case "enchant":
            return CustomSkill.Enchanteur;
          case "alchemy":
            return CustomSkill.Alchemist;
        }

        return -1;
      }

      public void HandleCraftItemChecks(NwItem blueprint, NwItem upgradedItem = null)
      {
        if (newCraftJob != null)
        {
          oid.SendServerMessage("Veuillez annuler votre travail artisanal en cours avant d'en commencer un nouveau.", ColorConstants.Red);
          return;
        }

        if (blueprint == null || blueprint.Possessor == oid.ControlledCreature)
        {
          oid.SendServerMessage($"{blueprint.Name.ColorString(ColorConstants.White)} n'est plus en votre possession. Impossible de démarrer le travail artisanal.", ColorConstants.Red);
          return;
        }

        int grade = upgradedItem != null ? upgradedItem.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").Value + 1 : 1 ;

        int materiaCost = (int)(GetItemMateriaCost(blueprint, grade) * (1 - (blueprint.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value / 100)));
        CraftResource resource = craftResourceStock.FirstOrDefault(r => r.type == ItemUtils.GetResourceTypeFromBlueprint(blueprint) && r.grade == 1 && r.quantity >= materiaCost);
        int availableQuantity = resource != null ? resource.quantity : 0;

        if (availableQuantity < materiaCost)
        {
          oid.SendServerMessage($"Il vous manque {(materiaCost - availableQuantity).ToString().ColorString(ColorConstants.White)} unités de matéria pour pouvoir commencer ce travail artisanal.", ColorConstants.Red);
          return;
        }

        resource.quantity -= materiaCost;

        if (grade < 2)
          newCraftJob = new CraftJob(this, blueprint, GetItemCraftTime(blueprint, materiaCost));
        else
          newCraftJob = new CraftJob(this, blueprint, GetItemCraftTime(blueprint, materiaCost), upgradedItem);

        if (windows.ContainsKey("activeCraftJob"))
          ((ActiveCraftJobWindow)windows["activeCraftJob"]).CreateWindow();
        else
          windows.Add("activeCraftJob", new ActiveCraftJobWindow(this));

        return;
      }
    }
  }
}
