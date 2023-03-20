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
      public int tempCurrentSkillPoint { get; set; }
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
      public async void TeleportPlayerToSavedLocation()
      {
        await NwTask.WaitUntil(() => oid.LoginCreature.Location.Area != null);
        oid.LoginCreature.Location = location;
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
          case "activeLearnable":
            if (windows.ContainsKey(window))
              ((ActiveLearnableWindow)windows[window]).CreateWindow();
            else
              windows.Add(window, new ActiveLearnableWindow(this));
            break;
        }
      }
      /*public async void InitializePlayerLearnableJobs()
      {
        await NwTask.WaitUntil(() => oid.LoginCreature.Location.Area != null);

        if (learnableSkills.Any(l => l.Value.active))
          learnableSkills.First(l => l.Value.active).Value.AwaitPlayerStateChangeToCalculateSPGain(this);

        else if (learnableSpells.Any(l => l.Value.active))
          learnableSpells.First(l => l.Value.active).Value.AwaitPlayerStateChangeToCalculateSPGain(this);

        int improvedHealth = learnableSkills.ContainsKey(CustomSkill.ImprovedHealth) ? learnableSkills[CustomSkill.ImprovedHealth].currentLevel : 0;
        int toughness = learnableSkills.ContainsKey(CustomSkill.Toughness) ? learnableSkills[CustomSkill.Toughness].currentLevel : 0;
        
        oid.LoginCreature.LevelInfo[0].HitDie = (byte)(80
          + (1 + 5 * ((oid.LoginCreature.GetAbilityScore(Ability.Constitution, true) - 10) / 2)
          + toughness) * improvedHealth);

        Log.Info($"hit die : {oid.LoginCreature.LevelInfo[0].HitDie}");
        Log.Info($"{(oid.LoginCreature.GetAbilityScore(Ability.Constitution, true) - 10) / 2}");
        Log.Info($"{toughness}");
        Log.Info($"{improvedHealth}");
        Log.Info($"{(1 + 5 * ((oid.LoginCreature.GetAbilityScore(Ability.Constitution, true) - 10) / 2) + toughness) * improvedHealth}");

        if (oid.LoginCreature.HP <= 0)
          oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.Death());

        if (learnableSkills.ContainsKey(CustomSkill.ImprovedAttackBonus))
          oid.LoginCreature.BaseAttackBonus = (byte)(oid.LoginCreature.BaseAttackBonus + learnableSkills[CustomSkill.ImprovedAttackBonus].totalPoints);

        if (activeLearnable != null && activeLearnable.active && activeLearnable.spLastCalculation.HasValue)
          activeLearnable.acquiredPoints += (DateTime.Now - activeLearnable.spLastCalculation).Value.TotalSeconds * GetSkillPointsPerSecond(activeLearnable);

        pcState = PcState.Online;
        oid.LoginCreature.GetObjectVariable<DateTimeLocalVariable>("_LAST_ACTION_DATE").Value = DateTime.Now;
      }*/
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
          Log.Info($"{oid.LoginCreature.Name} was not connected. Applying 40 % malus.");
        }
        else if (pcState == PcState.AFK)
        {
          pointsPerSecond *= 0.8;
          //Log.Info($"{oid.LoginCreature.Name} was afk. Applying 20 % malus.");
        }

        if (oid.LoginCreature.KnowsFeat(Feat.QuickToMaster))
          pointsPerSecond *= 1.1;

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
      public double GetWeaponMasteryLevel(BaseItemType baseItem)
      {
        double masteryLevel = 0;

        switch (baseItem)
        {
          case BaseItemType.Shortsword:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ShortSwordProficiency) ? learnableSkills[CustomSkill.ShortSwordProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedShortSwordProficiency) ? learnableSkills[CustomSkill.ImprovedShortSwordProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ShortSwordScience) ? learnableSkills[CustomSkill.ShortSwordScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Battleaxe:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.BattleAxeProficiency) ? learnableSkills[CustomSkill.BattleAxeProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedBattleAxeProficiency) ? learnableSkills[CustomSkill.ImprovedBattleAxeProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.BattleAxeScience) ? learnableSkills[CustomSkill.BattleAxeScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Bastardsword:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.BastardSwordProficiency) ? learnableSkills[CustomSkill.BastardSwordProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.BastardSwordProficiency) ? learnableSkills[CustomSkill.ImprovedBastardSwordProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.BastardSwordScience) ? learnableSkills[CustomSkill.BastardSwordScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.LightFlail:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.LightFlailProficiency) ? learnableSkills[CustomSkill.LightFlailProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedLightFlailProficiency) ? learnableSkills[CustomSkill.ImprovedLightFlailProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.LightFlailScience) ? learnableSkills[CustomSkill.LightFlailScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Warhammer:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.WarHammerProficiency) ? learnableSkills[CustomSkill.WarHammerProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedWarHammerProficiency) ? learnableSkills[CustomSkill.ImprovedWarHammerProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.WarHammerScience) ? learnableSkills[CustomSkill.WarHammerScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.HeavyCrossbow:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.HeavyCrossbowProficiency) ? learnableSkills[CustomSkill.HeavyCrossbowProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedHeavyCrossbowProficiency) ? learnableSkills[CustomSkill.ImprovedHeavyCrossbowProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.HeavyCrossbowScience) ? learnableSkills[CustomSkill.HeavyCrossbowScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.LightCrossbow:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.LightCrossBowProficiency) ? learnableSkills[CustomSkill.LightCrossBowProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedLightCrossBowProficiency) ? learnableSkills[CustomSkill.ImprovedLightCrossBowProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.LightCrossBowScience) ? learnableSkills[CustomSkill.LightCrossBowScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Longbow:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.LongBowProficiency) ? learnableSkills[CustomSkill.LongBowProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedLongBowProficiency) ? learnableSkills[CustomSkill.ImprovedLongBowProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.LongBowScience) ? learnableSkills[CustomSkill.LongBowScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.LightMace:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.LightMaceProficiency) ? learnableSkills[CustomSkill.LightMaceProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedLightMaceProficiency) ? learnableSkills[CustomSkill.ImprovedLightMaceProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.LightMaceScience) ? learnableSkills[CustomSkill.LightMaceScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Halberd:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.HalberdProficiency) ? learnableSkills[CustomSkill.HalberdProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedHalberdProficiency) ? learnableSkills[CustomSkill.ImprovedHalberdProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.HalberdScience) ? learnableSkills[CustomSkill.HalberdScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.TwoBladedSword:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.TwoBladedSwordProficiency) ? learnableSkills[CustomSkill.TwoBladedSwordProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedTwoBladedSwordProficiency) ? learnableSkills[CustomSkill.ImprovedTwoBladedSwordProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.TwoBladedSwordScience) ? learnableSkills[CustomSkill.TwoBladedSwordScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Shortbow:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ShortBowProficiency) ? learnableSkills[CustomSkill.ShortBowProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedShortBowProficiency) ? learnableSkills[CustomSkill.ImprovedShortBowProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ShortBowScience) ? learnableSkills[CustomSkill.ShortBowScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Greatsword:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.GreatSwordProficiency) ? learnableSkills[CustomSkill.GreatSwordProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedGreatSwordProficiency) ? learnableSkills[CustomSkill.ImprovedGreatSwordProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.GreatSwordScience) ? learnableSkills[CustomSkill.GreatSwordScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Greataxe:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.GreatAxeProficiency) ? learnableSkills[CustomSkill.GreatAxeProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedGreatAxeProficiency) ? learnableSkills[CustomSkill.ImprovedGreatAxeProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.GreatAxeScience) ? learnableSkills[CustomSkill.GreatAxeScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Dagger:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.DaggerProficiency) ? learnableSkills[CustomSkill.DaggerProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedDaggerProficiency) ? learnableSkills[CustomSkill.ImprovedDaggerProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.DaggerScience) ? learnableSkills[CustomSkill.DaggerScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Club:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ClubProficiency) ? learnableSkills[CustomSkill.ClubProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedClubProficiency) ? learnableSkills[CustomSkill.ImprovedClubProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ClubScience) ? learnableSkills[CustomSkill.ClubScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Dart:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.DartProficiency) ? learnableSkills[CustomSkill.DartProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedDartProficiency) ? learnableSkills[CustomSkill.ImprovedDartProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.DartScience) ? learnableSkills[CustomSkill.DartScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.DireMace:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.DireMaceProficiency) ? learnableSkills[CustomSkill.DireMaceProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedDireMaceProficiency) ? learnableSkills[CustomSkill.ImprovedDireMaceProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.DireMaceScience) ? learnableSkills[CustomSkill.DireMaceScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Doubleaxe:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.DoubleAxeProficiency) ? learnableSkills[CustomSkill.DoubleAxeProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedDoubleAxeProficiency) ? learnableSkills[CustomSkill.ImprovedDoubleAxeProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.DoubleAxeScience) ? learnableSkills[CustomSkill.DoubleAxeScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.HeavyFlail:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.HeavyFlailProficiency) ? learnableSkills[CustomSkill.HeavyFlailProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedHeavyFlailProficiency) ? learnableSkills[CustomSkill.ImprovedHeavyFlailProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.HeavyFlailScience) ? learnableSkills[CustomSkill.HeavyFlailScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.LightHammer:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.LightHammerProficiency) ? learnableSkills[CustomSkill.LightHammerProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedLightHammerProficiency) ? learnableSkills[CustomSkill.ImprovedLightHammerProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.LightHammerScience) ? learnableSkills[CustomSkill.LightHammerScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Handaxe:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.HandAxeProficiency) ? learnableSkills[CustomSkill.HandAxeProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedHandAxeProficiency) ? learnableSkills[CustomSkill.ImprovedHandAxeProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.HandAxeScience) ? learnableSkills[CustomSkill.HandAxeScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Kama:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.KamaProficiency) ? learnableSkills[CustomSkill.KamaProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedKamaProficiency) ? learnableSkills[CustomSkill.ImprovedKamaProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.KamaScience) ? learnableSkills[CustomSkill.KamaScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Katana:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.KatanaProficiency) ? learnableSkills[CustomSkill.KatanaProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedKatanaProficiency) ? learnableSkills[CustomSkill.ImprovedKatanaProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.KatanaScience) ? learnableSkills[CustomSkill.KatanaScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Kukri:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.KukriProficiency) ? learnableSkills[CustomSkill.KukriProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedKukriProficiency) ? learnableSkills[CustomSkill.ImprovedKukriProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.KukriScience) ? learnableSkills[CustomSkill.KukriScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.MagicStaff:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.MagicStaffProficiency) ? learnableSkills[CustomSkill.MagicStaffProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedMagicStaffProficiency) ? learnableSkills[CustomSkill.ImprovedMagicStaffProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.MagicStaffScience) ? learnableSkills[CustomSkill.MagicStaffScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Morningstar:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.MorningStarProficiency) ? learnableSkills[CustomSkill.MorningStarProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedMorningStarProficiency) ? learnableSkills[CustomSkill.ImprovedMorningStarProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.MorningStarScience) ? learnableSkills[CustomSkill.MorningStarScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Quarterstaff:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.QuarterStaffProficiency) ? learnableSkills[CustomSkill.QuarterStaffProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedQuarterStaffProficiency) ? learnableSkills[CustomSkill.ImprovedQuarterStaffProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.QuarterStaffScience) ? learnableSkills[CustomSkill.QuarterStaffScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Rapier:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.RapierProficiency) ? learnableSkills[CustomSkill.RapierProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedRapierProficiency) ? learnableSkills[CustomSkill.ImprovedRapierProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.RapierScience) ? learnableSkills[CustomSkill.RapierScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Scimitar:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ScimitarProficiency) ? learnableSkills[CustomSkill.ScimitarProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedScimitarProficiency) ? learnableSkills[CustomSkill.ImprovedScimitarProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ScimitarScience) ? learnableSkills[CustomSkill.ScimitarScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Scythe:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ScytheProficiency) ? learnableSkills[CustomSkill.ScytheProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedScytheProficiency) ? learnableSkills[CustomSkill.ImprovedScytheProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ScytheScience) ? learnableSkills[CustomSkill.ScytheScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.ShortSpear:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ShortSpearProficiency) ? learnableSkills[CustomSkill.ShortSpearProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedShortSpearProficiency) ? learnableSkills[CustomSkill.ImprovedShortSpearProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ShortSpearScience) ? learnableSkills[CustomSkill.ShortSpearScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Shuriken:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ShurikenProficiency) ? learnableSkills[CustomSkill.ShurikenProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedShurikenProficiency) ? learnableSkills[CustomSkill.ImprovedShurikenProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ShurikenScience) ? learnableSkills[CustomSkill.ShurikenScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Sickle:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.SickleProficiency) ? learnableSkills[CustomSkill.SickleProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedSickleProficiency) ? learnableSkills[CustomSkill.ImprovedSickleProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.SickleScience) ? learnableSkills[CustomSkill.SickleScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Sling:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.SlingProficiency) ? learnableSkills[CustomSkill.SlingProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedSlingProficiency) ? learnableSkills[CustomSkill.ImprovedSlingProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.SlingScience) ? learnableSkills[CustomSkill.SlingScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.ThrowingAxe:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ThrowingAxeProficiency) ? learnableSkills[CustomSkill.ThrowingAxeProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedThrowingAxeProficiency) ? learnableSkills[CustomSkill.ImprovedThrowingAxeProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ThrowingAxeScience) ? learnableSkills[CustomSkill.ThrowingAxeScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Trident:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.TridentProficiency) ? learnableSkills[CustomSkill.TridentProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedTridentProficiency) ? learnableSkills[CustomSkill.ImprovedTridentProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.TridentScience) ? learnableSkills[CustomSkill.TridentScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.DwarvenWaraxe:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.DwarvenWarAxeProficiency) ? learnableSkills[CustomSkill.DwarvenWarAxeProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedDwarvenWarAxeProficiency) ? learnableSkills[CustomSkill.ImprovedDwarvenWarAxeProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.DwarvenWarAxeScience) ? learnableSkills[CustomSkill.DwarvenWarAxeScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Whip:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.WhipProficiency) ? learnableSkills[CustomSkill.WhipProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedWhipProficiency) ? learnableSkills[CustomSkill.ImprovedWhipProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.WhipScience) ? learnableSkills[CustomSkill.WhipScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Longsword:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.LongSwordProficiency) ? learnableSkills[CustomSkill.LongSwordProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedLongSwordProficiency) ? learnableSkills[CustomSkill.ImprovedLongSwordProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.LongSwordScience) ? learnableSkills[CustomSkill.LongSwordScience].totalPoints * 5 : 0;
            break;
          case BaseItemType.Gloves:
          case BaseItemType.Bracer:
          default:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.UnharmedProficiency) ? learnableSkills[CustomSkill.UnharmedProficiency].totalPoints * 10 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedUnharmedProficiency) ? learnableSkills[CustomSkill.ImprovedUnharmedProficiency].totalPoints * 5 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.UnharmedScience) ? learnableSkills[CustomSkill.UnharmedScience].totalPoints * 5 : 0;
            break;
        }

        return masteryLevel * 0.01;
      }
      public double GetWeaponDoubleStrikeChance(BaseItemType baseItem)
      {
        double masteryLevel = 0;

        switch (baseItem)
        {
          case BaseItemType.Shortsword:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ShortSwordProficiency) ? learnableSkills[CustomSkill.ShortSwordProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedShortSwordProficiency) ? learnableSkills[CustomSkill.ImprovedShortSwordProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ShortSwordScience) ? learnableSkills[CustomSkill.ShortSwordScience].totalPoints : 0;
            break;
          case BaseItemType.Dagger:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.DaggerProficiency) ? learnableSkills[CustomSkill.DaggerProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedDaggerProficiency) ? learnableSkills[CustomSkill.ImprovedDaggerProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.DaggerScience) ? learnableSkills[CustomSkill.DaggerScience].totalPoints : 0;
            break;
          case BaseItemType.Handaxe:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.HandAxeProficiency) ? learnableSkills[CustomSkill.HandAxeProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedHandAxeProficiency) ? learnableSkills[CustomSkill.ImprovedHandAxeProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.HandAxeScience) ? learnableSkills[CustomSkill.HandAxeScience].totalPoints : 0;
            break;
          case BaseItemType.Kama:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.KamaProficiency) ? learnableSkills[CustomSkill.KamaProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedKamaProficiency) ? learnableSkills[CustomSkill.ImprovedKamaProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.KamaScience) ? learnableSkills[CustomSkill.KamaScience].totalPoints : 0;
            break;
          case BaseItemType.Kukri:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.KukriProficiency) ? learnableSkills[CustomSkill.KukriProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedKukriProficiency) ? learnableSkills[CustomSkill.ImprovedKukriProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.KukriScience) ? learnableSkills[CustomSkill.KukriScience].totalPoints : 0;
            break;
          case BaseItemType.Quarterstaff:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.QuarterStaffProficiency) ? learnableSkills[CustomSkill.QuarterStaffProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedQuarterStaffProficiency) ? learnableSkills[CustomSkill.ImprovedQuarterStaffProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.QuarterStaffScience) ? learnableSkills[CustomSkill.QuarterStaffScience].totalPoints : 0;
            break;
          case BaseItemType.Rapier:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.RapierProficiency) ? learnableSkills[CustomSkill.RapierProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedRapierProficiency) ? learnableSkills[CustomSkill.ImprovedRapierProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.RapierScience) ? learnableSkills[CustomSkill.RapierScience].totalPoints : 0;
            break;
          case BaseItemType.Shuriken:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ShurikenProficiency) ? learnableSkills[CustomSkill.ShurikenProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedShurikenProficiency) ? learnableSkills[CustomSkill.ImprovedShurikenProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ShurikenScience) ? learnableSkills[CustomSkill.ShurikenScience].totalPoints : 0;
            break;
          case BaseItemType.Sickle:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.SickleProficiency) ? learnableSkills[CustomSkill.SickleProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedSickleProficiency) ? learnableSkills[CustomSkill.ImprovedSickleProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.SickleScience) ? learnableSkills[CustomSkill.SickleScience].totalPoints : 0;
            break;
          case BaseItemType.Gloves:
          case BaseItemType.Bracer:
          default:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.UnharmedProficiency) ? learnableSkills[CustomSkill.UnharmedProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedUnharmedProficiency) ? learnableSkills[CustomSkill.ImprovedUnharmedProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.UnharmedScience) ? learnableSkills[CustomSkill.UnharmedScience].totalPoints : 0;
            break;
        }

        return Math.Round(masteryLevel * 0.02, MidpointRounding.ToEven);
      }
      public int GetWeaponCritScienceLevel(BaseItemType baseItem)
      {
        int masteryLevel = 0;

        switch (baseItem)
        {
          case BaseItemType.Shortsword: 
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ShortSwordProficiency) ? learnableSkills[CustomSkill.ShortSwordProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedShortSwordProficiency) ? learnableSkills[CustomSkill.ImprovedShortSwordProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ShortSwordScience) ? learnableSkills[CustomSkill.ShortSwordScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Battleaxe:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.BattleAxeProficiency) ? learnableSkills[CustomSkill.BattleAxeProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedBattleAxeProficiency) ? learnableSkills[CustomSkill.ImprovedBattleAxeProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.BattleAxeScience) ? learnableSkills[CustomSkill.BattleAxeScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Bastardsword:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.BastardSwordProficiency) ? learnableSkills[CustomSkill.BastardSwordProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedBastardSwordProficiency) ? learnableSkills[CustomSkill.ImprovedBastardSwordProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.BastardSwordScience) ? learnableSkills[CustomSkill.BastardSwordScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.LightFlail:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.LightFlailProficiency) ? learnableSkills[CustomSkill.LightFlailProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedLightFlailProficiency) ? learnableSkills[CustomSkill.ImprovedLightFlailProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.LightFlailScience) ? learnableSkills[CustomSkill.LightFlailScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Warhammer:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.WarHammerProficiency) ? learnableSkills[CustomSkill.WarHammerProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedWarHammerProficiency) ? learnableSkills[CustomSkill.ImprovedWarHammerProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.WarHammerScience) ? learnableSkills[CustomSkill.WarHammerScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.HeavyCrossbow:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.HeavyCrossbowProficiency) ? learnableSkills[CustomSkill.HeavyCrossbowProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedHeavyCrossbowProficiency) ? learnableSkills[CustomSkill.ImprovedHeavyCrossbowProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.HeavyCrossbowScience) ? learnableSkills[CustomSkill.HeavyCrossbowScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.LightCrossbow:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.LightCrossBowProficiency) ? learnableSkills[CustomSkill.LightCrossBowProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedLightCrossBowProficiency) ? learnableSkills[CustomSkill.ImprovedLightCrossBowProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.LightCrossBowScience) ? learnableSkills[CustomSkill.LightCrossBowScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Longbow:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.LongBowProficiency) ? learnableSkills[CustomSkill.LongBowProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedLongBowProficiency) ? learnableSkills[CustomSkill.ImprovedLongBowProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.LongBowScience) ? learnableSkills[CustomSkill.LongBowScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.LightMace:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.LightMaceProficiency) ? learnableSkills[CustomSkill.LightMaceProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedLightMaceProficiency) ? learnableSkills[CustomSkill.ImprovedLightMaceProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.LightMaceScience) ? learnableSkills[CustomSkill.LightMaceScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Halberd:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.HalberdProficiency) ? learnableSkills[CustomSkill.HalberdProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedHalberdProficiency) ? learnableSkills[CustomSkill.ImprovedHalberdProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.HalberdScience) ? learnableSkills[CustomSkill.HalberdScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.TwoBladedSword:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.TwoBladedSwordProficiency) ? learnableSkills[CustomSkill.TwoBladedSwordProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedTwoBladedSwordProficiency) ? learnableSkills[CustomSkill.ImprovedTwoBladedSwordProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.TwoBladedSwordScience) ? learnableSkills[CustomSkill.TwoBladedSwordScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Shortbow:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ShortBowProficiency) ? learnableSkills[CustomSkill.ShortBowProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedShortBowProficiency) ? learnableSkills[CustomSkill.ImprovedShortBowProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ShortBowScience) ? learnableSkills[CustomSkill.ShortBowScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Greatsword:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.GreatSwordProficiency) ? learnableSkills[CustomSkill.GreatSwordProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedGreatSwordProficiency) ? learnableSkills[CustomSkill.ImprovedGreatSwordProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.GreatSwordScience) ? learnableSkills[CustomSkill.GreatSwordScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Greataxe:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.GreatAxeProficiency) ? learnableSkills[CustomSkill.GreatAxeProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedGreatAxeProficiency) ? learnableSkills[CustomSkill.ImprovedGreatAxeProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.GreatAxeScience) ? learnableSkills[CustomSkill.GreatAxeScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Dagger:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.DaggerProficiency) ? learnableSkills[CustomSkill.DaggerProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedDaggerProficiency) ? learnableSkills[CustomSkill.ImprovedDaggerProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.DaggerScience) ? learnableSkills[CustomSkill.DaggerScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Club:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ClubProficiency) ? learnableSkills[CustomSkill.ClubProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedClubProficiency) ? learnableSkills[CustomSkill.ImprovedClubProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ClubScience) ? learnableSkills[CustomSkill.ClubScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Dart:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.DartProficiency) ? learnableSkills[CustomSkill.DartProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedDartProficiency) ? learnableSkills[CustomSkill.ImprovedDartProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.DartScience) ? learnableSkills[CustomSkill.DartScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.DireMace:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.DireMaceProficiency) ? learnableSkills[CustomSkill.DireMaceProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedDireMaceProficiency) ? learnableSkills[CustomSkill.ImprovedDireMaceProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.DireMaceScience) ? learnableSkills[CustomSkill.DireMaceScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Doubleaxe:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.DoubleAxeProficiency) ? learnableSkills[CustomSkill.DoubleAxeProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedDoubleAxeProficiency) ? learnableSkills[CustomSkill.ImprovedDoubleAxeProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.DoubleAxeScience) ? learnableSkills[CustomSkill.DoubleAxeScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.HeavyFlail:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.HeavyFlailProficiency) ? learnableSkills[CustomSkill.HeavyFlailProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedHeavyFlailProficiency) ? learnableSkills[CustomSkill.ImprovedHeavyFlailProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.HeavyFlailScience) ? learnableSkills[CustomSkill.HeavyFlailScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.LightHammer:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.LightHammerProficiency) ? learnableSkills[CustomSkill.LightHammerProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedLightHammerProficiency) ? learnableSkills[CustomSkill.ImprovedLightHammerProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.LightHammerScience) ? learnableSkills[CustomSkill.LightHammerScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Handaxe:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.HandAxeProficiency) ? learnableSkills[CustomSkill.HandAxeProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedHandAxeProficiency) ? learnableSkills[CustomSkill.ImprovedHandAxeProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.HandAxeScience) ? learnableSkills[CustomSkill.HandAxeScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Kama:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.KamaProficiency) ? learnableSkills[CustomSkill.KamaProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedKamaProficiency) ? learnableSkills[CustomSkill.ImprovedKamaProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.KamaScience) ? learnableSkills[CustomSkill.KamaScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Katana:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.KatanaProficiency) ? learnableSkills[CustomSkill.KatanaProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedKatanaProficiency) ? learnableSkills[CustomSkill.ImprovedKatanaProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.KatanaScience) ? learnableSkills[CustomSkill.KatanaScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Kukri:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.KukriProficiency) ? learnableSkills[CustomSkill.KukriProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedKukriProficiency) ? learnableSkills[CustomSkill.ImprovedKukriProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.KukriScience) ? learnableSkills[CustomSkill.KukriScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.MagicStaff:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.MagicStaffProficiency) ? learnableSkills[CustomSkill.MagicStaffProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedMagicStaffProficiency) ? learnableSkills[CustomSkill.ImprovedMagicStaffProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.MagicStaffScience) ? learnableSkills[CustomSkill.MagicStaffScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Morningstar:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.MorningStarProficiency) ? learnableSkills[CustomSkill.MorningStarProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedMorningStarProficiency) ? learnableSkills[CustomSkill.ImprovedMorningStarProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.MorningStarScience) ? learnableSkills[CustomSkill.MorningStarScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Quarterstaff:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.QuarterStaffProficiency) ? learnableSkills[CustomSkill.QuarterStaffProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedQuarterStaffProficiency) ? learnableSkills[CustomSkill.ImprovedQuarterStaffProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.QuarterStaffScience) ? learnableSkills[CustomSkill.QuarterStaffScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Rapier:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.RapierProficiency) ? learnableSkills[CustomSkill.RapierProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedRapierProficiency) ? learnableSkills[CustomSkill.ImprovedRapierProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.RapierScience) ? learnableSkills[CustomSkill.RapierScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Scimitar:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ScimitarProficiency) ? learnableSkills[CustomSkill.ScimitarProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedScimitarProficiency) ? learnableSkills[CustomSkill.ImprovedScimitarProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ScimitarScience) ? learnableSkills[CustomSkill.ScimitarScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Scythe:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ScytheProficiency) ? learnableSkills[CustomSkill.ScytheProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedScytheProficiency) ? learnableSkills[CustomSkill.ImprovedScytheProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ScytheScience) ? learnableSkills[CustomSkill.ScytheScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.ShortSpear:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ShortSpearProficiency) ? learnableSkills[CustomSkill.ShortSpearProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedShortSpearProficiency) ? learnableSkills[CustomSkill.ImprovedShortSpearProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ShortSpearScience) ? learnableSkills[CustomSkill.ShortSpearScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Shuriken:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ShurikenProficiency) ? learnableSkills[CustomSkill.ShurikenProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedShurikenProficiency) ? learnableSkills[CustomSkill.ImprovedShurikenProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ShurikenScience) ? learnableSkills[CustomSkill.ShurikenScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Sickle:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.SickleProficiency) ? learnableSkills[CustomSkill.SickleProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedSickleProficiency) ? learnableSkills[CustomSkill.ImprovedSickleProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.SickleScience) ? learnableSkills[CustomSkill.SickleScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Sling:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.SlingProficiency) ? learnableSkills[CustomSkill.SlingProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedSlingProficiency) ? learnableSkills[CustomSkill.ImprovedSlingProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.SlingScience) ? learnableSkills[CustomSkill.SlingScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.ThrowingAxe:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ThrowingAxeProficiency) ? learnableSkills[CustomSkill.ThrowingAxeProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedThrowingAxeProficiency) ? learnableSkills[CustomSkill.ImprovedThrowingAxeProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ThrowingAxeScience) ? learnableSkills[CustomSkill.ThrowingAxeScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Trident:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.TridentProficiency) ? learnableSkills[CustomSkill.TridentProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedTridentProficiency) ? learnableSkills[CustomSkill.ImprovedTridentProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.TridentScience) ? learnableSkills[CustomSkill.TridentScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.DwarvenWaraxe:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.DwarvenWarAxeProficiency) ? learnableSkills[CustomSkill.DwarvenWarAxeProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedDwarvenWarAxeProficiency) ? learnableSkills[CustomSkill.ImprovedDwarvenWarAxeProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.DwarvenWarAxeScience) ? learnableSkills[CustomSkill.DwarvenWarAxeScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Whip:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.WhipProficiency) ? learnableSkills[CustomSkill.WhipProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedWhipProficiency) ? learnableSkills[CustomSkill.ImprovedWhipProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.WhipScience) ? learnableSkills[CustomSkill.WhipScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Longsword:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.LongSwordProficiency) ? learnableSkills[CustomSkill.LongSwordProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedLongSwordProficiency) ? learnableSkills[CustomSkill.ImprovedLongSwordProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.LongSwordScience) ? learnableSkills[CustomSkill.LongSwordScience].totalPoints : 0;
            return masteryLevel;
          case BaseItemType.Gloves:
          case BaseItemType.Bracer:
          default:
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.UnharmedProficiency) ? learnableSkills[CustomSkill.UnharmedProficiency].totalPoints * 2 : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.ImprovedUnharmedProficiency) ? learnableSkills[CustomSkill.ImprovedUnharmedProficiency].totalPoints : 0;
            masteryLevel += learnableSkills.ContainsKey(CustomSkill.UnharmedScience) ? learnableSkills[CustomSkill.UnharmedScience].totalPoints : 0;
            return masteryLevel;
        }
      }
      public int GetArmorProficiencyLevel(int baseACValue)
      {
        int skillPoints = 0;

        switch(baseACValue)
        {
          default:
            skillPoints += learnableSkills.ContainsKey(CustomSkill.ClothingArmorProficiency) ? learnableSkills[CustomSkill.ClothingArmorProficiency].totalPoints : 0;
            skillPoints += learnableSkills.ContainsKey(CustomSkill.ImprovedClothingArmorProficiency) ? learnableSkills[CustomSkill.ImprovedClothingArmorProficiency].totalPoints : 0;
            return skillPoints;
          case 1:
          case 2:
          case 3:
            skillPoints += learnableSkills.ContainsKey(CustomSkill.LightArmorProficiency) ? learnableSkills[CustomSkill.LightArmorProficiency].totalPoints : 0;
            skillPoints += learnableSkills.ContainsKey(CustomSkill.ImprovedLightArmorProficiency) ? learnableSkills[CustomSkill.ImprovedLightArmorProficiency].totalPoints : 0;
            return skillPoints;
          case 4:
          case 5:
            skillPoints += learnableSkills.ContainsKey(CustomSkill.MediumArmorProficiency) ? learnableSkills[CustomSkill.MediumArmorProficiency].totalPoints : 0;
            skillPoints += learnableSkills.ContainsKey(CustomSkill.ImprovedMediumArmorProficiency) ? learnableSkills[CustomSkill.ImprovedMediumArmorProficiency].totalPoints : 0;
            return skillPoints;
          case 6:
          case 7:
            skillPoints += learnableSkills.ContainsKey(CustomSkill.HeavyArmorProficiency) ? learnableSkills[CustomSkill.HeavyArmorProficiency].totalPoints : 0;
            skillPoints += learnableSkills.ContainsKey(CustomSkill.ImprovedHeavyArmorProficiency) ? learnableSkills[CustomSkill.ImprovedHeavyArmorProficiency].totalPoints : 0;
            return skillPoints;
          case 8:
            skillPoints += learnableSkills.ContainsKey(CustomSkill.FullPlateProficiency) ? learnableSkills[CustomSkill.FullPlateProficiency].totalPoints : 0;
            skillPoints += learnableSkills.ContainsKey(CustomSkill.ImprovedFullPlateProficiency) ? learnableSkills[CustomSkill.ImprovedFullPlateProficiency].totalPoints : 0;
            return skillPoints;
        }
      }
      public int GetShieldProficiencyLevel(BaseItemType baseItem)
      {
        int skillPoints = 0;

        switch(baseItem)
        {
          case BaseItemType.SmallShield:
            skillPoints += learnableSkills.ContainsKey(CustomSkill.ImprovedLightShieldProficiency) ? learnableSkills[CustomSkill.ImprovedLightShieldProficiency].totalPoints : 0;
            skillPoints += learnableSkills.ContainsKey(CustomSkill.LightShieldProficiency) ? learnableSkills[CustomSkill.LightShieldProficiency].totalPoints : 0;
            return skillPoints;
          case BaseItemType.LargeShield:
            skillPoints += learnableSkills.ContainsKey(CustomSkill.ImprovedMediumShieldProficiency) ? learnableSkills[CustomSkill.ImprovedMediumShieldProficiency].totalPoints : 0;
            skillPoints += learnableSkills.ContainsKey(CustomSkill.MediumShieldProficiency) ? learnableSkills[CustomSkill.MediumShieldProficiency].totalPoints : 0;
            return skillPoints;
          case BaseItemType.TowerShield:
            skillPoints += learnableSkills.ContainsKey(CustomSkill.ImprovedHeavyShieldProficiency) ? learnableSkills[CustomSkill.ImprovedHeavyShieldProficiency].totalPoints : 0;
            skillPoints += learnableSkills.ContainsKey(CustomSkill.HeavyShieldProficiency) ? learnableSkills[CustomSkill.HeavyShieldProficiency].totalPoints : 0;
            return skillPoints;
          default:
            skillPoints += learnableSkills.ContainsKey(CustomSkill.ImprovedDualWieldDefenseProficiency) ? learnableSkills[CustomSkill.ImprovedDualWieldDefenseProficiency].totalPoints : 0;
            skillPoints += learnableSkills.ContainsKey(CustomSkill.DualWieldDefenseProficiency) ? learnableSkills[CustomSkill.DualWieldDefenseProficiency].totalPoints : 0;
            return skillPoints;
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
        materiaCost *= tool is not null ? (1 - (tool.LocalVariables.Where(l => l.Name.StartsWith($"ENCHANTEMENT_CUSTOM_CRAFT_SPEED_") && !l.Name.Contains("_DURABILITY")).Sum(l => ((LocalVariableInt)l).Value) / 100)) : 1;

        return materiaCost;
      }
      public double GetWeaponTimeCost(BaseItemType baseItemType, double materiaCost, NwItem tool)
      {
        var entry = BaseItems2da.baseItemTable[(int)baseItemType];
        int jobFeat = GetJobLearnableFromWorkshop(entry.workshop);

        materiaCost *= learnableSkills.ContainsKey(entry.craftLearnable) ? learnableSkills[entry.craftLearnable].bonusReduction : 1;
        materiaCost *= learnableSkills.ContainsKey(jobFeat) ? learnableSkills[jobFeat].bonusReduction : 1;
        materiaCost *= tool is not null ? (1 - (tool.LocalVariables.Where(l => l.Name.StartsWith($"ENCHANTEMENT_CUSTOM_CRAFT_SPEED_") && !l.Name.Contains("_DURABILITY")).Sum(l => ((LocalVariableInt)l).Value) / 100)) : 1;

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
        materiaCost *= tool is not null ? (1 - (tool.LocalVariables.Where(l => l.Name.StartsWith($"ENCHANTEMENT_CUSTOM_CRAFT_YIELD_") && !l.Name.Contains("_DURABILITY")).Sum(l => ((LocalVariableInt)l).Value) / 100)) : 1;

        return materiaCost;
      }
      public double GetWeaponMateriaCost(BaseItemType baseItemType, NwItem tool)
      {
        var entry = BaseItems2da.baseItemTable[(int)baseItemType];
        double materiaCost = entry.cost * 1000;
        int jobFeat = GetJobLearnableFromWorkshop(entry.workshop);

        materiaCost *= learnableSkills.ContainsKey(entry.craftLearnable) ? learnableSkills[entry.craftLearnable].bonusReduction : 1;
        materiaCost *= learnableSkills.ContainsKey(jobFeat) ? learnableSkills[jobFeat].bonusReduction : 1;
        materiaCost *= tool is not null ? (1 - (tool.LocalVariables.Where(l => l.Name.StartsWith($"ENCHANTEMENT_CUSTOM_CRAFT_YIELD_") && !l.Name.Contains("_DURABILITY")).Sum(l => ((LocalVariableInt)l).Value) / 100)) : 1;

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
          "enchant" => CustomSkill.Enchanteur,
          "alchemy" => CustomSkill.Alchemist,
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

        if(tool is null || tool.Possessor != oid.LoginCreature || !tool.LocalVariables.Any(v => v.Name.Contains("ENCHANTEMENT_CUSTOM_CRAFT_")))
        {
          oid.SendServerMessage("L'outil que vous utilisez actuellement ne permet plus la manipulation de matéria raffinée. Veuillez en utiliser un autre.", ColorConstants.Red);
          return;
        }

        int grade = repairedItem.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").HasValue ? repairedItem.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").Value : 1;

        int materiaCost = (int)GetItemRepairMateriaCost(repairedItem, tool);
        CraftResource resource = craftResourceStock.FirstOrDefault(r => r.type == ItemUtils.GetResourceTypeFromItem(repairedItem) && r.grade == 1 && r.quantity >= materiaCost);
        int availableQuantity = resource != null ? resource.quantity : 0;

        if (availableQuantity < materiaCost)
        {
          oid.SendServerMessage($"Il vous manque {(materiaCost - availableQuantity).ToString().ColorString(ColorConstants.White)} unités de matéria pour pouvoir commencer ce travail artisanal.", ColorConstants.Red);
          return;
        }

        resource.quantity -= materiaCost;
        craftJob = new CraftJob(this, repairedItem, GetItemRepairTime(repairedItem, materiaCost, tool), JobType.Repair);

        ItemUtils.HandleCraftToolDurability(this, tool, "CRAFT", CustomSkill.ArtisanPrudent);

        if (!windows.ContainsKey("activeCraftJob")) windows.Add("activeCraftJob", new ActiveCraftJobWindow(this));
        else ((ActiveCraftJobWindow) windows["activeCraftJob"]).CreateWindow();

        return;
      }
      private bool HandleEnchantementItemChecks(NwItem item, NwSpell spell, int index)
      {
        if (craftJob != null)
        {
          oid.SendServerMessage("Veuillez annuler votre travail artisanal en cours avant d'en commencer un nouveau.", ColorConstants.Red);
          return false;
        }

        if (item == null || item.Possessor != oid.ControlledCreature)
        {
          oid.SendServerMessage($"{item.Name.ColorString(ColorConstants.White)} n'est plus en votre possession. Impossible de commencer le travail artisanal.", ColorConstants.Red);
          return false;
        }

        if (item.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").HasNothing)
        {
          oid.SendServerMessage($"{item.Name.ColorString(ColorConstants.White)} n'a plus d'emplacement de sort disponible.", ColorConstants.Red);
          return false;
        }

        craftJob = new CraftJob(this, item, spell, index, JobType.Enchantement);

        if (!windows.ContainsKey("activeCraftJob")) windows.Add("activeCraftJob", new ActiveCraftJobWindow(this));
        else ((ActiveCraftJobWindow)windows["activeCraftJob"]).CreateWindow();

        return true;
      }
      public void HandleCraftItemChecks(NwItem blueprint, NwItem tool, NwItem upgradedItem = null)
      {
        if (craftJob != null)
        {
          oid.SendServerMessage("Veuillez annuler votre travail artisanal en cours avant d'en commencer un nouveau.", ColorConstants.Red);
          return;
        }

        if (blueprint == null || blueprint.Possessor != oid.ControlledCreature)
        {
          oid.SendServerMessage($"{blueprint.Name.ColorString(ColorConstants.White)} n'est plus en votre possession. Impossible de commencer le travail artisanal.", ColorConstants.Red);
          return;
        }

        if (tool is null || tool.Possessor != oid.LoginCreature || !tool.LocalVariables.Any(v => v.Name.Contains("ENCHANTEMENT_CUSTOM_CRAFT_")))
        {
          oid.SendServerMessage("L'outil que vous utilisez actuellement ne permet plus la manipulation de matéria raffinée. Veuillez en utiliser un autre.", ColorConstants.Red);
          return;
        }

        int grade = 1;

        if (upgradedItem != null)
        {
          if (upgradedItem.Possessor != oid.ControlledCreature)
          {
            oid.SendServerMessage($"{upgradedItem.Name.ColorString(ColorConstants.White)} n'est plus en votre possession. Impossible de commencer le travail artisanal.", ColorConstants.Red);
            return;
          }

          grade = upgradedItem.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").Value + 1;
        }

        int materiaCost = (int)(GetItemMateriaCost(blueprint, tool, grade) * (1 - (blueprint.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value / 100)));
        CraftResource resource = craftResourceStock.FirstOrDefault(r => r.type == ItemUtils.GetResourceTypeFromBlueprint(blueprint) && r.grade == 1 && r.quantity >= materiaCost);
        int availableQuantity = resource != null ? resource.quantity : 0;

        if (availableQuantity < materiaCost)
        {
          oid.SendServerMessage($"Il vous manque {(materiaCost - availableQuantity).ToString().ColorString(ColorConstants.White)} unités de matéria pour pouvoir commencer ce travail artisanal.", ColorConstants.Red);
          return;
        }

        resource.quantity -= materiaCost;
        craftJob = grade < 2 ? new CraftJob(this, blueprint, GetItemCraftTime(blueprint, materiaCost, tool), tool) : new CraftJob(this, blueprint, GetItemCraftTime(blueprint, materiaCost, tool), upgradedItem, tool);
        
        ItemUtils.HandleCraftToolDurability(this, tool, "CRAFT", CustomSkill.ArtisanPrudent);

        if (!windows.ContainsKey("activeCraftJob")) windows.Add("activeCraftJob", new ActiveCraftJobWindow(this));
        else ((ActiveCraftJobWindow)windows["activeCraftJob"]).CreateWindow();
      }
      public void HandlePassiveJobChecks(string worshop)
      {
        if (craftJob != null)
        {
          oid.SendServerMessage("Veuillez annuler votre travail artisanal en cours avant d'en commencer un nouveau.", ColorConstants.Red);
          return;
        }

        craftJob = new CraftJob(this, ItemUtils.GetResourceFromWorkshopTag(worshop), 0, "beam");

        if (!windows.ContainsKey("activeCraftJob")) windows.Add("activeCraftJob", new ActiveCraftJobWindow(this));
        else ((ActiveCraftJobWindow)windows["activeCraftJob"]).CreateWindow();
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
        remainingTime *= (1 - (tool.LocalVariables.Where(l => l.Name.StartsWith($"ENCHANTEMENT_CUSTOM_CRAFT_SPEED_") && !l.Name.Contains("_DURABILITY")).Sum(l => ((LocalVariableInt)l).Value) / 100));

        return remainingTime;
      }
      public double GetItemRecycleGain(NwItem item)
      {
        double quantity = item.BaseItem.BaseCost * 125.0;
        quantity *= learnableSkills.ContainsKey(CustomSkill.Recycler) ? learnableSkills[CustomSkill.Recycler].bonusMultiplier : 1;
        quantity *= learnableSkills.ContainsKey(CustomSkill.RecyclerExpert) ? learnableSkills[CustomSkill.RecyclerExpert].bonusMultiplier : 1;

        return quantity;
      }
      public double GetItemRepairMateriaCost(NwItem item, NwItem tool)
      {
        double materiaCost = GetItemMateriaCost(item, tool, item.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").Value);
        materiaCost *= 1 + (5 * item.GetObjectVariable<LocalVariableInt>("_DURABILITY_NB_REPAIRS") / 100);
        materiaCost *= (1 - (tool.LocalVariables.Where(l => l.Name.StartsWith($"ENCHANTEMENT_CUSTOM_CRAFT_YIELD_") && !l.Name.Contains("_DURABILITY")).Sum(l => ((LocalVariableInt)l).Value) / 100));
        materiaCost /= item.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").Value == oid.LoginCreature.OriginalName ? 8 : 4;
        materiaCost *= learnableSkills.ContainsKey(CustomSkill.Repair) ? learnableSkills[CustomSkill.Repair].bonusReduction : 1;
        materiaCost *= learnableSkills.ContainsKey(CustomSkill.RepairExpert) ? learnableSkills[CustomSkill.RepairExpert].bonusReduction : 1;

        return materiaCost;
      }
      public double GetItemRepairTime(NwItem item, double materiaCost, NwItem tool)
      {
        double timeCost = item.BaseItem.ItemType == BaseItemType.Armor ? GetArmorTimeCost(item.BaseACValue, materiaCost, tool) : GetWeaponTimeCost(item.BaseItem.ItemType, materiaCost, tool);
        timeCost *= 1 + (25 * item.GetObjectVariable<LocalVariableInt>("_DURABILITY_NB_REPAIRS") / 100);
        timeCost *= learnableSkills.ContainsKey(CustomSkill.Repair) ? learnableSkills[CustomSkill.Repair].bonusReduction : 1;
        timeCost *= learnableSkills.ContainsKey(CustomSkill.RepairFast) ? learnableSkills[CustomSkill.RepairFast].bonusReduction : 1;
        timeCost *= (1 - (tool.LocalVariables.Where(l => l.Name.StartsWith($"ENCHANTEMENT_CUSTOM_CRAFT_SPEED_") && !l.Name.Contains("_DURABILITY")).Sum(l => ((LocalVariableInt)l).Value) / 100));

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
        double reprocessingSkill = learnableSkills.ContainsKey(resource.reprocessingLearnable) ? 1.00 + 3 * learnableSkills[resource.reprocessingLearnable].totalPoints / 100 : 1.00;
        double efficiencySkill = learnableSkills.ContainsKey(resource.reprocessingEfficiencyLearnable) ? 1.00 + 2 * learnableSkills[resource.reprocessingEfficiencyLearnable].totalPoints / 100 : 1.00;
        double reproGradeSkill = learnableSkills.ContainsKey(resource.reprocessingGradeLearnable) ? 1.00 + 2 * learnableSkills[resource.reprocessingGradeLearnable].totalPoints / 100 : 1.00;
        double connectionSkill = learnableSkills.ContainsKey(CustomSkill.ConnectionsPromenade) ? 0.95 + learnableSkills[CustomSkill.ConnectionsPromenade].totalPoints / 100 : 1.00;
        double expertSkill = learnableSkills.ContainsKey(resource.reprocessingExpertiseLearnable) ? 12 * learnableSkills[resource.reprocessingExpertiseLearnable].totalPoints / 100 : 0;
        double total = 2 * quantity;
        total -= quantity * resource.grade * 0.15 * expertSkill;
        total *= 0.3 * reprocessingSkill * efficiencySkill * reproGradeSkill;
        total *= connectionSkill;

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
    }
  }
}
