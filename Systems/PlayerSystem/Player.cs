using System;
using System.Collections.Generic;
using Anvil.API;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Anvil.Services;
using NWN.Systems.Alchemy;
using NWN.Core.NWNX;
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
      public int bankGold { get; set; }
      public int shortRest { get; set; }
      public CraftJob craftJob { get; set; }
      public Location previousLocation { get; set; }
      public Menu menu { get; }
      public NwCreature deathCorpse { get; set; }
      public string serializedQuickbar { get; set; }
      public Arena.PlayerData pveArena { get; set; }
      public Cauldron alchemyCauldron { get; set; }
      public PcState pcState { get; set; }
      public bool hideFromPlayerList { get; set; }
      public Endurance endurance = new();

      public List<NwPlayer> listened = new();
      public List<int> mutedList = new();
      public Dictionary<uint, Player> blocked = new();
      public Dictionary<int, LearnableSkill> learnableSkills = new();
      public Dictionary<int, LearnableSpell> learnableSpells = new();
      public float guiHeight { get => oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight); }
      public float guiWidth { get => oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth); }
      public float guiScaledHeight => oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / (oid.GetDeviceProperty(PlayerDeviceProperty.GuiScale) / 100);
      public float guiScaledWidth => oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / (oid.GetDeviceProperty(PlayerDeviceProperty.GuiScale) / 100);
      public Dictionary<StrRef, string> tlkOverrides = new();
      public Dictionary<string, string> iconOverrides = new();
      public int MaxHP { get => oid.LoginCreature.LevelInfo[0].HitDie + ((oid.LoginCreature.GetAbilityScore(Ability.Constitution, true) - 10) / 2); }
      public double energyRegen { get; set; }
      public int healthRegen { get; set; }
      public bool wasHPGreaterThan50 { get; set; }
      public int soulReapTriggers { get; set; }
      public Guid transmutationStone { get; set; }
      public Learnable activeLearnable { get; set; }
      public Dictionary<int, MapPin> mapPinDictionnary = new();
      public Dictionary<string, byte[]> areaExplorationStateDictionnary = new();
      public Dictionary<int, byte[]> chatColors = new();
      public readonly Dictionary<string, PlayerWindow> windows = new();
      public Dictionary<string, NuiRect> windowRectangles = new();
      //public Dictionary<string, int> openedWindows = new ();
      public readonly List<ChatLine> readChatLines = new();
      public readonly List<Mail> mails = new();
      public readonly List<Subscription> subscriptions = new();

      public List<CraftResource> craftResourceStock = new();
      public List<Grimoire> grimoires = new();
      public List<Quickbar> quickbars = new();
      public CooldownPosition cooldownPositions = new();
      public List<ItemAppearance> itemAppearances = new();
      public List<CharacterDescription> descriptions = new();
      public List<CustomDMVisualEffect> customDMVisualEffects = new();
      public List<NwGameObject> effectTargets = new();

      private readonly SpellSystem spellSystem;
      private readonly AreaSystem areaSystem;
      private readonly FeedbackService feedbackService;
      public readonly SchedulerService scheduler;
      public readonly EventService eventService;

      public enum PcState
      {
        Offline,
        Online,
        AFK
      }

      public Player(NwPlayer nwobj, AreaSystem areaSystem, SpellSystem spellSystem, FeedbackService feedbackService, SchedulerService schedulerService, EventService eventService)
      {
        oid = nwobj;
        menu = new PrivateMenu(this);
        pveArena = new Arena.PlayerData();
        scheduler = schedulerService;
        this.spellSystem = spellSystem;
        this.areaSystem = areaSystem;
        this.feedbackService = feedbackService;
        this.eventService = eventService;

        if (!oid.IsDM)
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
      public void LoadMenuQuickbar()
      {
        PlayerQuickBarButton emptyQBS = new PlayerQuickBarButton();

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
      }
      /*public async void InitializePlayerOpenedWindows()
      {
        await NwTask.WaitUntil(() => oid.LoginCreature.Location.Area != null);

        foreach (string window in openedWindows.Keys)
          CreatePlayerWindow(window);
      }*/
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
        }
      }
      public void UnloadMenuQuickbar()
      {
        oid.ControlledCreature.RemoveFeat(CustomFeats.CustomMenuUP);
        oid.ControlledCreature.RemoveFeat(CustomFeats.CustomMenuDOWN);
        oid.ControlledCreature.RemoveFeat(CustomFeats.CustomMenuSELECT);
        oid.ControlledCreature.RemoveFeat(CustomFeats.CustomMenuEXIT);
        oid.ControlledCreature.DeserializeQuickbar(this.serializedQuickbar.ToByteArray());
      }
      public string CheckDBPlayerAccount()
      {
        var result = SqLiteUtils.SelectQuery("PlayerAccounts",
          new List<string>() { { "accountName" } },
          new List<string[]>() { new string[] { "rowId", accountId.ToString() } });

        if (result == null || result.Count < 1)
          return "";

        return result.FirstOrDefault()[0];
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
            pointsPerSecond *= 0.1;
            break;
          case 1:
            pointsPerSecond *= 0.9;
            break;
          case 3:
            pointsPerSecond *= 1.1;
            break;
          case 4:
            pointsPerSecond *= 1.2;
            break;
          case 100:
            pointsPerSecond *= 10.0;
            break;
        }          

        if (pcState == PcState.Offline)
        {
          pointsPerSecond *= 0.6;
          LogUtils.LogMessage($"{oid.LoginCreature.Name} was not connected. Applying 40 % malus.", LogUtils.LogType.Learnables);
        }
        else if (pcState == PcState.AFK)
          pointsPerSecond *= 0.8;

        //ModuleSystem.Log.Info($"SP CALCULATION - {oid.LoginCreature.Name} - {pointsPerSecond} SP.");

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
        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, mapPinDictionnary);
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        string serializedJson = await reader.ReadToEndAsync();

        SqLiteUtils.UpdateQuery("PlayerAccounts",
          new List<string[]>() { new string[] { "mapPins", serializedJson } },
          new List<string[]>() { new string[] { "rowid", accountId.ToString() } });
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
      private void ActivateSpotLight(NwCreature target)
      {
        if (target.GetObjectVariable<LocalVariableBool>("SPOTLIGHT_ON").HasNothing)
        {
          PlayerPlugin.ApplyLoopingVisualEffectToObject(oid.ControlledCreature, target, 173);
          target.GetObjectVariable<LocalVariableBool>("SPOTLIGHT_ON").Value = true;
        }
      }
      private void RemoveSpotLight(NwCreature target)
      {
        if (target.GetObjectVariable<LocalVariableBool>("SPOTLIGHT_ON").HasValue)
        {
          PlayerPlugin.ApplyLoopingVisualEffectToObject(oid.ControlledCreature, target, 173);
          target.GetObjectVariable<LocalVariableBool>("SPOTLIGHT_ON").Delete();
        }
      }
      public double GetItemCraftTime(NwItem item, int materiaCost, NwItem tool = null)
      {
        BaseItemType baseItemType = (BaseItemType)item.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value;
        double timeEfficiency = (1 - (item.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_TIME_EFFICIENCY").Value / 100));

        if (baseItemType == BaseItemType.Armor)
          return GetArmorTimeCost(item.GetObjectVariable<LocalVariableInt>("_ARMOR_BASE_AC").Value, materiaCost, tool) * timeEfficiency;

        return GetWeaponTimeCost(baseItemType, materiaCost, tool) * timeEfficiency;
      }
      public double GetArmorTimeCost(int baseACValue, double materiaCost, NwItem tool)
      {
        var entry = Armor2da.armorTable[baseACValue];
        int jobFeat = GetJobLearnableFromWorkshop(entry.workshop);

        materiaCost *= learnableSkills.ContainsKey(entry.craftLearnable) ? learnableSkills[entry.craftLearnable].bonusReduction : 1;
        materiaCost *= learnableSkills.ContainsKey(jobFeat) ? learnableSkills[jobFeat].bonusReduction : 1;

        if (tool is not null)
        {
          for (int i = 0; i < tool.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
          {
            if (tool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
              continue;

            switch (tool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
            {
              case CustomInscription.MateriaProductionSpeedMinor: materiaCost *= 0.98; break;
              case CustomInscription.MateriaProductionSpeed: materiaCost *= 0.96; break;
              case CustomInscription.MateriaProductionSpeedMajor: materiaCost *= 0.94; break;
              case CustomInscription.MateriaProductionSpeedSupreme: materiaCost *= 0.92; break;
            }
          }
        }

        return materiaCost;
      }
      public double GetWeaponTimeCost(BaseItemType baseItemType, double materiaCost, NwItem tool)
      {
        var entry = BaseItems2da.baseItemTable[(int)baseItemType];
        int jobFeat = GetJobLearnableFromWorkshop(entry.workshop);

        materiaCost *= learnableSkills.ContainsKey(entry.craftLearnable) ? learnableSkills[entry.craftLearnable].bonusReduction : 1;
        materiaCost *= learnableSkills.ContainsKey(jobFeat) ? learnableSkills[jobFeat].bonusReduction : 1;

        if (tool is not null)
        {
          for (int i = 0; i < tool.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
          {
            if (tool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
              continue;

            switch (tool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
            {
              case CustomInscription.MateriaProductionSpeedMinor: materiaCost *= 0.98; break;
              case CustomInscription.MateriaProductionSpeed: materiaCost *= 0.96; break;
              case CustomInscription.MateriaProductionSpeedMajor: materiaCost *= 0.94; break;
              case CustomInscription.MateriaProductionSpeedSupreme: materiaCost *= 0.92; break;
            }
          }
        }

        return materiaCost;
      }
      public double GetItemMateriaCost(NwItem item, NwItem tool = null, double grade = 1)
      {
        BaseItemType baseItemType;

        if (item.Tag == "blueprint")
        {
          baseItemType = (BaseItemType)item.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value;

          if (baseItemType == BaseItemType.Armor)
            return GetArmorMateriaCost(item.GetObjectVariable<LocalVariableInt>("_ARMOR_BASE_AC").Value, tool) * (1 - ((grade - 1) / 10));
        }
        else
          baseItemType = item.BaseItem.ItemType;

        if (baseItemType == BaseItemType.Armor)
          return GetArmorMateriaCost(item.BaseACValue, tool) * (1 - ((grade - 1) / 10));

        return GetWeaponMateriaCost(baseItemType, tool) * (1 - ((grade - 1) / 10));
      }

      public double GetArmorMateriaCost(int baseACValue, NwItem tool)
      {
        var entry = Armor2da.armorTable[baseACValue];
        double materiaCost = entry.cost * 1000;
        int jobFeat = GetJobLearnableFromWorkshop(entry.workshop);

        materiaCost *= learnableSkills.ContainsKey(entry.craftLearnable) ? learnableSkills[entry.craftLearnable].bonusReduction : 1;
        materiaCost *= baseACValue < 1 && learnableSkills.ContainsKey(CustomSkill.CraftClothes) ? learnableSkills[CustomSkill.CraftClothes].bonusReduction : 1;
        materiaCost *= baseACValue > 0 && learnableSkills.ContainsKey(CustomSkill.CraftArmor) ? learnableSkills[CustomSkill.CraftArmor].bonusReduction : 1;
        materiaCost *= learnableSkills.ContainsKey(jobFeat) ? learnableSkills[jobFeat].bonusReduction : 1;

        if (tool is not null)
        {
          for (int i = 0; i < tool.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
          {
            if (tool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
              continue;

            switch (tool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
            {
              case CustomInscription.MateriaProductionYieldMinor: materiaCost *= 0.98; break;
              case CustomInscription.MateriaProductionYield: materiaCost *= 0.96; break;
              case CustomInscription.MateriaProductionYieldMajor: materiaCost *= 0.94; break;
              case CustomInscription.MateriaProductionYieldSupreme: materiaCost *= 0.92; break;
            }
          }
        }

        return materiaCost;
      }
      public double GetWeaponMateriaCost(BaseItemType baseItemType, NwItem tool)
      {
        var entry = BaseItems2da.baseItemTable[(int)baseItemType];
        double materiaCost = entry.cost * 1000;
        int jobFeat = GetJobLearnableFromWorkshop(entry.workshop);

        materiaCost *= learnableSkills.ContainsKey(entry.craftLearnable) ? learnableSkills[entry.craftLearnable].bonusReduction : 1;
        materiaCost *= learnableSkills.ContainsKey(jobFeat) ? learnableSkills[jobFeat].bonusReduction : 1;

        if (tool is not null)
        {
          for (int i = 0; i < tool.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
          {
            if (tool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
              continue;

            switch (tool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
            {
              case CustomInscription.MateriaProductionYieldMinor: materiaCost *= 0.98; break;
              case CustomInscription.MateriaProductionYield: materiaCost *= 0.96; break;
              case CustomInscription.MateriaProductionYieldMajor: materiaCost *= 0.94; break;
              case CustomInscription.MateriaProductionYieldSupreme: materiaCost *= 0.92; break;
            }
          }
        }

        switch (ItemUtils.GetItemCategory(baseItemType))
        {
          case ItemUtils.ItemCategory.OneHandedMeleeWeapon: materiaCost *= learnableSkills.ContainsKey(CustomSkill.CraftOnHandedMeleeWeapon) ? learnableSkills[CustomSkill.CraftOnHandedMeleeWeapon].bonusReduction : 1; break;
          case ItemUtils.ItemCategory.TwoHandedMeleeWeapon: materiaCost *= learnableSkills.ContainsKey(CustomSkill.CraftTwoHandedMeleeWeapon) ? learnableSkills[CustomSkill.CraftTwoHandedMeleeWeapon].bonusReduction : 1; break;
          case ItemUtils.ItemCategory.RangedWeapon: materiaCost *= learnableSkills.ContainsKey(CustomSkill.CraftRangedWeapon) ? learnableSkills[CustomSkill.CraftRangedWeapon].bonusReduction : 1; break;
          case ItemUtils.ItemCategory.Shield: materiaCost *= learnableSkills.ContainsKey(CustomSkill.CraftShield) ? learnableSkills[CustomSkill.CraftShield].bonusReduction : 1; break;
          case ItemUtils.ItemCategory.Ammunition: materiaCost *= learnableSkills.ContainsKey(CustomSkill.CraftAmmunitions) ? learnableSkills[CustomSkill.CraftAmmunitions].bonusReduction : 1; break;
        }

        return materiaCost;
      }
      public int GetJobLearnableFromWorkshop(string workshopTag)
      {
        return workshopTag switch
        {
          "forge" => CustomSkill.Blacksmith,
          "scierie" => CustomSkill.Woodworker,
          "tannerie" => CustomSkill.Tanner,
          //"alchemy" => CustomSkill.Alchemist,
          _ => -1,
        };
      }
      public void HandleRepairItemChecks(NwItem repairedItem, NwItem tool)
      {
        if (craftJob != null)
        {
          oid.SendServerMessage("Veuillez annuler votre travail artisanal en cours avant d'en commencer un nouveau.", ColorConstants.Red);
          return;
        }

        if (tool is null || tool.RootPossessor != oid.LoginCreature)
        {
          oid.SendServerMessage("L'outil que vous utilisez actuellement n'est plus valide. Veuillez en utiliser un autre.", ColorConstants.Red);
          return;
        }

        for (int i = 0; i < tool.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
        {
          if (tool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
            continue;

          int inscriptionId = tool.GetObjectVariable<LocalVariableInt>($"SLOT{i}");

          if (inscriptionId >= CustomInscription.MateriaProductionDurabilityMinor && inscriptionId <= CustomInscription.MateriaProductionQualitySupreme)
            break;

          oid.SendServerMessage("L'outil utilisé pour votre travail ne dispose plus d'inscription permettant la manipulation de matéria, pensez à faire appliquer de nouvelles inscriptions !", ColorConstants.Red);
          return;
        }

        int grade = repairedItem.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").HasValue ? repairedItem.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").Value : 1;

        int materiaCost = (int)GetItemRepairMateriaCost(repairedItem, tool);
        CraftResource resource = craftResourceStock.FirstOrDefault(r => r.type == ItemUtils.GetResourceTypeFromItem(repairedItem) && r.quantity >= materiaCost);
        int availableQuantity = resource != null ? resource.quantity : 0;

        if (availableQuantity < materiaCost)
        {
          oid.SendServerMessage($"Il vous manque {(materiaCost - availableQuantity).ToString().ColorString(ColorConstants.White)} unités de matéria pour pouvoir commencer ce travail artisanal.", ColorConstants.Red);
          return;
        }

        resource.quantity -= materiaCost;
        craftJob = new CraftJob(this, repairedItem, GetItemRepairTime(repairedItem, materiaCost, tool), JobType.Repair);

        ItemUtils.HandleCraftToolDurability(this, tool, CustomInscription.MateriaProductionDurability, CustomSkill.ArtisanPrudent);

        return;
      }
      public void HandleCraftItemChecks(NwItem blueprint, NwItem tool, NwItem upgradedItem = null)
      {
        if (craftJob != null)
        {
          oid.SendServerMessage("Veuillez annuler votre travail artisanal en cours avant d'en commencer un nouveau.", ColorConstants.Red);
          return;
        }

        if (blueprint == null || blueprint.RootPossessor != oid.ControlledCreature)
        {
          oid.SendServerMessage($"{blueprint.Name.ColorString(ColorConstants.White)} n'est plus en votre possession. Impossible de commencer le travail artisanal.", ColorConstants.Red);
          return;
        }

        if (tool is null || tool.RootPossessor != oid.LoginCreature)
        {
          oid.SendServerMessage("L'outil utilisé pour votre travail n'est plus valide, veuillez en utiliser un autre !", ColorConstants.Red);
          return;
        }

        for (int i = 0; i < tool.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
        {
          if (tool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
            continue;

          int inscriptionId = tool.GetObjectVariable<LocalVariableInt>($"SLOT{i}");

          if (inscriptionId >= CustomInscription.MateriaProductionDurabilityMinor && inscriptionId <= CustomInscription.MateriaProductionQualitySupreme)
            break; 

          oid.SendServerMessage("L'outil utilisé pour votre travail ne dispose plus d'inscription permettant la manipulation de matéria, pensez à faire appliquer de nouvelles inscriptions !", ColorConstants.Red);
          return;
        }
       
        int grade = 1;

        if (upgradedItem != null)
        {
          if (upgradedItem.RootPossessor != oid.ControlledCreature)
          {
            oid.SendServerMessage($"{upgradedItem.Name.ColorString(ColorConstants.White)} n'est plus en votre possession. Impossible de commencer le travail artisanal.", ColorConstants.Red);
            return;
          }

          grade = upgradedItem.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").Value + 1;
        }

        int materiaCost = (int)(GetItemMateriaCost(blueprint, tool, grade) * (1 - (blueprint.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value / 100)));
        CraftResource resource = craftResourceStock.FirstOrDefault(r => r.type == ItemUtils.GetResourceTypeFromBlueprint(blueprint) && r.quantity >= materiaCost);
        int availableQuantity = resource != null ? resource.quantity : 0;

        if (availableQuantity < materiaCost)
        {
          oid.SendServerMessage($"Il vous manque {(materiaCost - availableQuantity).ToString().ColorString(ColorConstants.White)} unités de matéria pour pouvoir commencer ce travail artisanal.", ColorConstants.Red);
          return;
        }

        resource.quantity -= materiaCost;
        craftJob = grade < 2 ? new CraftJob(this, blueprint, GetItemCraftTime(blueprint, materiaCost, tool), tool) : new CraftJob(this, blueprint, GetItemCraftTime(blueprint, materiaCost, tool), upgradedItem, tool);
        
        ItemUtils.HandleCraftToolDurability(this, tool, CustomInscription.MateriaProductionDurability, CustomSkill.ArtisanPrudent);
      }
      public void HandlePassiveJobChecks(string worshop)
      {
        if (craftJob != null)
        {
          oid.SendServerMessage("Veuillez annuler votre travail artisanal en cours avant d'en commencer un nouveau.", ColorConstants.Red);
          return;
        }

        craftJob = new CraftJob(this, ItemUtils.GetResourceFromWorkshopTag(worshop), 0, "beam");
      }
      public static string GetReadableTimeSpan(double timeCost)
      {
        TimeSpan timespan = TimeSpan.FromSeconds(timeCost);
        return new TimeSpan(timespan.Days, timespan.Hours, timespan.Minutes, timespan.Seconds).ToString();
      }
      public double GetItemReinforcementTime(NwItem item, NwItem tool)
      {
        double remainingTime = ItemUtils.GetBaseItemCost(item) * 100.0;
        remainingTime *= learnableSkills.ContainsKey(CustomSkill.Renforcement) ? (1.0 - (learnableSkills[CustomSkill.Renforcement].totalPoints * 5.0 / 100.0)) : 1;

        return remainingTime;
      }
      public double GetItemRecycleTime(NwItem item, NwItem tool)
      {
        double remainingTime = ItemUtils.GetBaseItemCost(item) * 125.0;

        remainingTime *= learnableSkills.ContainsKey(CustomSkill.Recycler) ? learnableSkills[CustomSkill.Recycler].bonusReduction : 1;
        remainingTime *= learnableSkills.ContainsKey(CustomSkill.RecyclerFast) ? learnableSkills[CustomSkill.RecyclerFast].bonusReduction : 1;

        if (tool is not null)
        {
          for (int i = 0; i < tool.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
          {
            if (tool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
              continue;

            switch (tool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
            {
              case CustomInscription.MateriaProductionSpeedMinor: remainingTime *= 0.98; break;
              case CustomInscription.MateriaProductionSpeed: remainingTime *= 0.96; break;
              case CustomInscription.MateriaProductionSpeedMajor: remainingTime *= 0.94; break;
              case CustomInscription.MateriaProductionSpeedSupreme: remainingTime *= 0.92; break;
            }
          }
        }

        return remainingTime;
      }
      public double GetItemRecycleGain(NwItem item)
      {
        double quantity = item.BaseItem.BaseCost * 125.0;
        quantity *= learnableSkills.ContainsKey(CustomSkill.Recycler) ? learnableSkills[CustomSkill.Recycler].bonusMultiplier : 1;
        quantity *= learnableSkills.ContainsKey(CustomSkill.RecyclerExpert) ? learnableSkills[CustomSkill.RecyclerExpert].bonusMultiplier : 1;

        /*if (tool is not null)
        {
          for (int i = 0; i < tool.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
          {
            if (tool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
              continue;

            switch (tool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
            {
              case CustomInscription.MateriaProductionYieldMinor: quantity *= 1.02; break;
              case CustomInscription.MateriaProductionYield: quantity *= 1.04; break;
              case CustomInscription.MateriaProductionYieldMajor: quantity *= 1.06; break;
              case CustomInscription.MateriaProductionYieldSupreme: quantity *= 1.08; break;
            }
          }
        }*/

        return quantity;
      }
      public double GetItemRepairMateriaCost(NwItem item, NwItem tool)
      {
        double materiaCost = GetItemMateriaCost(item, tool, item.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").Value);
        materiaCost *= 1 + (5 * item.GetObjectVariable<LocalVariableInt>("_DURABILITY_NB_REPAIRS") / 100);
        materiaCost /= item.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").Value == oid.LoginCreature.OriginalName ? 8 : 4;
        materiaCost *= learnableSkills.ContainsKey(CustomSkill.Repair) ? learnableSkills[CustomSkill.Repair].bonusReduction : 1;
        materiaCost *= learnableSkills.ContainsKey(CustomSkill.RepairExpert) ? learnableSkills[CustomSkill.RepairExpert].bonusReduction : 1;

        if (tool is not null)
        {
          for (int i = 0; i < tool.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
          {
            if (tool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
              continue;

            switch (tool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
            {
              case CustomInscription.MateriaProductionYieldMinor: materiaCost *= 1.02; break;
              case CustomInscription.MateriaProductionYield: materiaCost *= 1.04; break;
              case CustomInscription.MateriaProductionYieldMajor: materiaCost *= 1.06; break;
              case CustomInscription.MateriaProductionYieldSupreme: materiaCost *= 1.08; break;
            }
          }
        }

        return materiaCost;
      }
      public double GetItemRepairTime(NwItem item, double materiaCost, NwItem tool)
      {
        double timeCost = item.BaseItem.ItemType == BaseItemType.Armor ? GetArmorTimeCost(item.BaseACValue, materiaCost, tool) : GetWeaponTimeCost(item.BaseItem.ItemType, materiaCost, tool);
        timeCost *= 1 + (25 * item.GetObjectVariable<LocalVariableInt>("_DURABILITY_NB_REPAIRS") / 100);
        timeCost *= learnableSkills.ContainsKey(CustomSkill.Repair) ? learnableSkills[CustomSkill.Repair].bonusReduction : 1;
        timeCost *= learnableSkills.ContainsKey(CustomSkill.RepairFast) ? learnableSkills[CustomSkill.RepairFast].bonusReduction : 1;

        if (tool is not null)
        {
          for (int i = 0; i < tool.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
          {
            if (tool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
              continue;

            switch (tool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
            {
              case CustomInscription.MateriaProductionSpeedMinor: timeCost *= 1.02; break;
              case CustomInscription.MateriaProductionSpeed: timeCost *= 1.04; break;
              case CustomInscription.MateriaProductionSpeedMajor: timeCost *= 1.06; break;
              case CustomInscription.MateriaProductionSpeedSupreme: timeCost *= 1.08; break;
            }
          }
        }

        return timeCost;
      }
      public string GetUniqueTagForChar(string suffix)
      {
        return oid.CDKey + "_" + oid.BicFileName + "_" + suffix;
      }
      private void HandleGenericNuiEvents(ModuleEvents.OnNuiEvent nuiEvent)
      {
        string window = nuiEvent.Token.WindowId;
        nuiEvent.Player.LoginCreature.GetObjectVariable<DateTimeLocalVariable>("_LAST_ACTION_DATE").Value = DateTime.Now;

        switch (nuiEvent.ElementId)
        {
          case "geometry":

            NuiRect windowRectangle;

            try
            {
              windowRectangle = new NuiBind<NuiRect>("geometry").GetBindValue(nuiEvent.Player, nuiEvent.Token.Token);
            }
            catch(Exception e)
            {
              Utils.LogMessageToDMs($"{nuiEvent.Player.LoginCreature.Name}\n{nuiEvent.Token.WindowId}\n{e.Message}\n{e.StackTrace}");
              windowRectangle = new(10, 10, 600, 600);
            }

            if (windowRectangle.Width > 0 && windowRectangle.Height > 0)
            {
              if (!windowRectangles.ContainsKey(window))
                windowRectangles.Add(window, windowRectangle);
              else
              {
                if (windowRectangles[window].Width != windowRectangle.Width || windowRectangles[window].Height != windowRectangle.Height)
                {
                  windowRectangles[window] = windowRectangle;
                  windows[window].ResizeWidgets();
                }
                else
                  windowRectangles[window] = windowRectangle;
              }
            }

            if (pcState == PcState.Online)
              nuiEvent.Player.ExportCharacter();
            break;

          case "_window_":

            if (string.IsNullOrEmpty(window))
              return;

            switch (nuiEvent.EventType)
            {
              case NuiEventType.Open:
                windows[window].IsOpen = true;
                break;

              case NuiEventType.Close:
                windows[window].IsOpen = false;
                break;
            }

            break;
        }
      }
      public bool TryGetOpenedWindow(string windowId, out PlayerWindow openedWindow)
      {
        if (windows.TryGetValue(windowId, out openedWindow) && openedWindow.IsOpen)
          return true;

        return false;
      }
      public bool QueryAuthorized()
      {
        if (oid.LoginCreature.GetObjectVariable<DateTimeLocalVariable>("_LAST_QUERY_TIME").HasNothing || oid.LoginCreature.GetObjectVariable<DateTimeLocalVariable>("_LAST_QUERY_TIME").Value < DateTime.Now.AddSeconds(10))
        {
          oid.LoginCreature.GetObjectVariable<DateTimeLocalVariable>("_LAST_QUERY_TIME").Value = DateTime.Now;
          return true;
        }
        else
        {
          oid.SendServerMessage($"Votre dernière demande de persistance est trop récente, veuillez réessayez dans 10 secondes.", ColorConstants.Red);
          return false;
        }
      }
      public int GetMateriaYieldFromResource(int quantity, CraftResource resource)
      {
        /*double reprocessingSkill = learnableSkills.ContainsKey(resource.reprocessingLearnable) ? 1.00 + 3 * learnableSkills[resource.reprocessingLearnable].totalPoints / 100 : 1.00;
        double efficiencySkill = learnableSkills.ContainsKey(resource.reprocessingEfficiencyLearnable) ? 1.00 + 2 * learnableSkills[resource.reprocessingEfficiencyLearnable].totalPoints / 100 : 1.00;
        double reproGradeSkill = learnableSkills.ContainsKey(resource.reprocessingGradeLearnable) ? 1.00 + 2 * learnableSkills[resource.reprocessingGradeLearnable].totalPoints / 100 : 1.00;
        double connectionSkill = learnableSkills.ContainsKey(CustomSkill.ConnectionsPromenade) ? 0.95 + learnableSkills[CustomSkill.ConnectionsPromenade].totalPoints / 100 : 1.00;
        double expertSkill = learnableSkills.ContainsKey(resource.reprocessingExpertiseLearnable) ? 12 * learnableSkills[resource.reprocessingExpertiseLearnable].totalPoints / 100 : 0;
        */double total = 2 * quantity;
        //total -= quantity * 0.15 * expertSkill;
        ////total *= connectionSkill;

        return (int)total;
      }
      public bool IsDm()
      {
        if (oid.IsDM || oid.PlayerName == "Chim")
          return true;
        return false;
      }
      public async void RescheduleRepeatableJob(ResourceType type, double consumedTime, string jobIcon)
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0));
        craftJob = new CraftJob(this, type, consumedTime, jobIcon);
      }
      public NwItem GetItemBestBlueprint(NwItem item)
      {
        return oid.ControlledCreature.Inventory.Items
             .Where(i => i.Tag == "blueprint" && i.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value == (int)item.BaseItem.ItemType)
             .OrderByDescending(i => i.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value)
             .ThenByDescending(i => i.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_TIME_EFFICIENCY").Value).FirstOrDefault();
      }
      public double GetShieldInscriptionSkillScore()
      {
        double skillScore = 1;

        if (learnableSkills.ContainsKey(CustomSkill.CalligrapheBlindeur))
          skillScore -= learnableSkills[CustomSkill.CalligrapheBlindeur].totalPoints * 2 / 100;

        if (learnableSkills.ContainsKey(CustomSkill.CalligrapheBlindeurExpert))
          skillScore -= learnableSkills[CustomSkill.CalligrapheBlindeurExpert].totalPoints * 2 / 100;

        if (learnableSkills.ContainsKey(CustomSkill.CalligrapheBlindeurMaitre))
          skillScore -= learnableSkills[CustomSkill.CalligrapheBlindeurMaitre].totalPoints * 3 / 100;

        if (learnableSkills.ContainsKey(CustomSkill.CalligrapheBlindeurScience))
          skillScore -= learnableSkills[CustomSkill.CalligrapheBlindeurScience].totalPoints * 3 / 100;

        return skillScore;
      }
      public double GetArmorInscriptionSkillScore()
      {
        double skillScore = 1;

        if (learnableSkills.ContainsKey(CustomSkill.CalligrapheArmurier))
          skillScore -= learnableSkills[CustomSkill.CalligrapheArmurier].totalPoints * 2 / 100;

        if (learnableSkills.ContainsKey(CustomSkill.CalligrapheArmurierExpert))
          skillScore -= learnableSkills[CustomSkill.CalligrapheArmurierExpert].totalPoints * 2 / 100;

        if (learnableSkills.ContainsKey(CustomSkill.CalligrapheArmurierMaitre))
          skillScore -= learnableSkills[CustomSkill.CalligrapheArmurierMaitre].totalPoints * 3 / 100;

        if (learnableSkills.ContainsKey(CustomSkill.CalligrapheArmurierScience))
          skillScore -= learnableSkills[CustomSkill.CalligrapheArmurierScience].totalPoints * 3 / 100;

        return skillScore;
      }
      public double GetOrnamentInscriptionSkillScore()
      {
        double skillScore = 1;

        if (learnableSkills.ContainsKey(CustomSkill.CalligrapheCiseleur))
          skillScore -= learnableSkills[CustomSkill.CalligrapheCiseleur].totalPoints * 2 / 100;

        if (learnableSkills.ContainsKey(CustomSkill.CalligrapheCiseleurExpert))
          skillScore -= learnableSkills[CustomSkill.CalligrapheCiseleurExpert].totalPoints * 2 / 100;

        if (learnableSkills.ContainsKey(CustomSkill.CalligrapheCiseleurMaitre))
          skillScore -= learnableSkills[CustomSkill.CalligrapheCiseleurMaitre].totalPoints * 3 / 100;

        if (learnableSkills.ContainsKey(CustomSkill.CalligrapheCiseleurScience))
          skillScore -= learnableSkills[CustomSkill.CalligrapheCiseleurScience].totalPoints * 3 / 100;

        return skillScore;
      }
      public double GetWeaponInscriptionSkillScore()
      {
        double skillScore = 1;

        if (learnableSkills.ContainsKey(CustomSkill.CalligrapheFourbisseur))
          skillScore -= learnableSkills[CustomSkill.CalligrapheFourbisseur].totalPoints * 2 / 100;

        if (learnableSkills.ContainsKey(CustomSkill.CalligrapheFourbisseurExpert))
          skillScore -= learnableSkills[CustomSkill.CalligrapheFourbisseurExpert].totalPoints * 2 / 100;

        if (learnableSkills.ContainsKey(CustomSkill.CalligrapheFourbisseurMaitre))
          skillScore -= learnableSkills[CustomSkill.CalligrapheFourbisseurMaitre].totalPoints * 3 / 100;

        if (learnableSkills.ContainsKey(CustomSkill.CalligrapheFourbisseurScience))
          skillScore -= learnableSkills[CustomSkill.CalligrapheFourbisseurScience].totalPoints * 3 / 100;

        return skillScore;
      }
      /*public void SetMaxHP()
      {
        int improvedHealth = learnableSkills.ContainsKey(CustomSkill.ImprovedHealth) ? learnableSkills[CustomSkill.ImprovedHealth].currentLevel : 0;
        int toughness = learnableSkills.ContainsKey(CustomSkill.Toughness) ? learnableSkills[CustomSkill.Toughness].currentLevel : 0;
        int additionalHPFromItems = CheckForAdditionalHP();

        double totalMaxHP = oid.LoginCreature.ActiveEffects.Any(e => e.Tag == "_CORE_EFFECT") ?
            (byte)(endurance.maxHP + improvedHealth * (toughness + (oid.LoginCreature.GetAbilityScore(Ability.Constitution, true) - 10)) + additionalHPFromItems)
          : (byte)(endurance.maxHP + improvedHealth * (toughness + (oid.LoginCreature.GetAbilityScore(Ability.Constitution, true) - 10 / 2)) + additionalHPFromItems);

        foreach (var eff in oid.LoginCreature.ActiveEffects)
          if (eff.Tag == "CUSTOM_CONDITION_DEEPWOUND")
          { 
            totalMaxHP *= 0.80;
            break;
          }

        oid.LoginCreature.LevelInfo[0].HitDie = (byte)Math.Round(totalMaxHP, MidpointRounding.ToEven);
      }
      public int GetAdditionalMana()
      {
        return oid.LoginCreature.GetAbilityModifier(Ability.Intelligence) + oid.LoginCreature.GetAbilityModifier(Ability.Wisdom)
          + (oid.LoginCreature.GetAbilityModifier(Ability.Charisma) * 2)
          + ((oid.LoginCreature.GetAbilityScore(Ability.Intelligence, true) - 10) / 4 * GetAttributeLevel(SkillSystem.Attribut.EnergyStorage))
          + CheckForAdditionalMana();
      }
      public int CheckForAdditionalHP()
      {
        int additionnalHP = 0;

        foreach (InventorySlot slot in (InventorySlot[])Enum.GetValues(typeof(InventorySlot)))
          additionnalHP += CheckForAdditionalHPOnItem(oid?.LoginCreature?.GetItemInSlot(slot));

        return additionnalHP;
      }
      public int CheckForAdditionalMana()
      {
        int additionnalMana = 0;
        energyRegen = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand)?.BaseItem.ItemType == BaseItemType.MagicStaff ? 4 : 2;

        foreach (InventorySlot slot in (InventorySlot[])Enum.GetValues(typeof(InventorySlot)))
          additionnalMana += CheckForAdditionalManaOnItem(oid.LoginCreature.GetItemInSlot(slot));

        return additionnalMana;
      }
      public int CheckForAdditionalHPOnItem(NwItem item)
      {
        if (item is null || !item.IsValid)
          return 0;

        int additionalHP = 0;

        for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
        {
          if (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
            continue;

          switch (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
          {
            case CustomInscription.Courage:
            case CustomInscription.Vigueur:  additionalHP += 4; break;
            case CustomInscription.Dévotion:
            case CustomInscription.Piété:
              if (oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_POSITIVE_SPELL_")))
                additionalHP += 6;
              break;

            case CustomInscription.Endurance:
            case CustomInscription.Ténacité:
              if (oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("_STANCE_")))
                additionalHP += 6;
              break;

            case CustomInscription.Valeur:
            case CustomInscription.Détermination:
              if (oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_MALEFICE_")))
                additionalHP += 8;
              break;

            case CustomInscription.LaVieNestQueDouleur: additionalHP -= 3; break;
            case CustomInscription.Survivant: additionalHP += 1; break;
          }
        }

        return additionalHP;
      }
      public int CheckForAdditionalManaOnItem(NwItem item)
      {
        if (item is null || !item.IsValid)
          return 0;

        int additionalMana = 0;
        bool carpeDiem = false;
        bool zele = false;

        for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
        {
          if (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
            continue;

          switch (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
          {
            case CustomInscription.Rayonnant:
            case CustomInscription.Vision: additionalMana += 1; break;
            case CustomInscription.HeureuxLesSimplesdEsprits:
            case CustomInscription.QueDuMuscle: additionalMana -= 1; break;

            case CustomInscription.LaSécuritéAvantTout:
              if (oid.LoginCreature.HP > MaxHP / 2)
                additionalMana += 1;
              break;

            case CustomInscription.AucunRecours:
              if (oid.LoginCreature.HP < MaxHP / 2)
                additionalMana += 2;
              break;

            case CustomInscription.AyezFoi:
              if (oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_POSITIVE_SPELL_")))
                additionalMana += 1;
              break;

            case CustomInscription.MienneEstLaPeine:
            case CustomInscription.Détermination:
              if (oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_MALEFICE_")))
                additionalMana += 2;
              break;

            case CustomInscription.CarpeDiem: carpeDiem = true; break;
            case CustomInscription.Zèle: zele = true; break;
          }
        }

        if(carpeDiem)
        {
          additionalMana += 15;
          energyRegen -= 1;
        }

        if (zele)
          energyRegen -= 1;

        return additionalMana;
      }
      public bool IsVampireWeapon(NwItem item)
      {
        if(item is null || !item.IsValid) 
          return false;

        for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
        {
          if (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
            continue;

          if(item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value == CustomInscription.Vampirisme)
            return true;
        }

        return false;
      }
      protected void HandleItemPropertyChecksOnEffectApplied(OnEffectApply effectApplied)
      {
        SetMaxHP();
        endurance.additionnalMana = GetAdditionalMana();
      }
      protected void HandleItemPropertyChecksOnEffectRemoved(OnEffectRemove effectRemoved)
      {
        SetMaxHP();
        endurance.additionnalMana = GetAdditionalMana();
      }*/
    }
  }
}
