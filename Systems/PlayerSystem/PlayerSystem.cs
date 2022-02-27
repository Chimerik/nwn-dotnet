using System;
using System.Collections.Generic;
using System.Linq;
using NWN.Core;
using NWN.Core.NWNX;
using Anvil.API;
using Anvil.Services;
using Anvil.API.Events;
using NLog;
using System.Threading.Tasks;

namespace NWN.Systems
{
  [ServiceBinding(typeof(PlayerSystem))]
  public partial class PlayerSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public static EventService eventService { get; set; }
    public static FeedbackService feedbackService;
    public static ScriptHandleFactory scriptHandleFactory;
    public PlayerSystem(EventService eventServices, FeedbackService feedback, ScriptHandleFactory scriptFactory)
    {
      NwModule.Instance.OnClientEnter += HandlePlayerConnect;
      NwModule.Instance.OnClientDisconnect += HandlePlayerLeave;

      eventService = eventServices;
      feedbackService = feedback;
      scriptHandleFactory = scriptFactory;
    }

    public static Dictionary<uint, Player> Players = new Dictionary<uint, Player>();

    public static void OnCombatStarted(OnCombatStatusChange onCombatStatusChange)
    {
      if (onCombatStatusChange.CombatStatus == CombatStatus.ExitCombat)
        return;

      Effect effPC = onCombatStatusChange.Player.ControlledCreature.ActiveEffects.FirstOrDefault(e => e.EffectType == EffectType.CutsceneGhost);
      if (effPC != null)
        onCombatStatusChange.Player.ControlledCreature.RemoveEffect(effPC);
    }
    public static void OnCombatRoundStart(OnCombatRoundStart onStartCombatRound)
    {
      if (onStartCombatRound.Target is NwCreature { IsPlayerControlled: true } oTarget)
        oTarget.ControllingPlayer.SetPCReputation(false, onStartCombatRound.Creature.ControllingPlayer);
    }
    public static void HandleCombatModeOff(OnCombatModeToggle onCombatMode)
    {
      if(onCombatMode.NewMode == CombatMode.None && onCombatMode.Creature.GetObjectVariable<LocalVariableInt>("_ACTIVATED_TAUNT").HasValue) // Permet de conserver sa posture de combat après avoir utilisé taunt
      {
        onCombatMode.PreventToggle = true;
        onCombatMode.Creature.GetObjectVariable<LocalVariableInt>("_ACTIVATED_TAUNT").Delete();
      }
    }

    private static void HandleBeforeSkillUsed(OnUseSkill onUseSkill)
    {
      NwCreature oPC = onUseSkill.Creature;
      
      switch (onUseSkill.Skill.SkillType)
      {
        case Skill.Taunt:
          oPC.GetObjectVariable<LocalVariableInt>("_ACTIVATED_TAUNT").Value = 1;

          Task waitForCooldown= NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(6));
            oPC.GetObjectVariable<LocalVariableInt>("_ACTIVATED_TAUNT").Delete();
          });

          break;
        case Skill.PickPocket:
          onUseSkill.PreventSkillUse = true;

          if (!(onUseSkill.Target is NwCreature { IsLoginPlayerCharacter: true } oTarget) || oTarget.ControllingPlayer.IsDM)
          {
            oPC.ControllingPlayer.FloatingTextString("Seuls d'autres joueurs peuvent être ciblés par cette compétence. Les tentatives de vol sur PNJ doivent être jouées en rp avec un dm.".ColorString(ColorConstants.Red), false);
            return;
          }

          if (!DateTime.TryParse(oTarget.GetObjectVariable<PersistentVariableString>($"_PICKPOCKET_TIMER_{oPC.Name}").Value, out DateTime previousDate)
              || (DateTime.Now - previousDate).TotalHours < 24)
          {
            oPC.ControllingPlayer.FloatingTextString($"Vous ne serez autorisé à faire une nouvelle tentative de vol que dans : {(DateTime.Now - previousDate).TotalHours + 1} heure(s)", false);
            return;
          }

          oTarget.GetObjectVariable<PersistentVariableString>($"_PICKPOCKET_TIMER_{oPC.Name}").Value = DateTime.Now.ToString();

          FeedbackPlugin.SetFeedbackMessageHidden(13, 1, oTarget); // 13 = COMBAT_TOUCH_ATTACK

          Task waitForMessage = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(2));
            FeedbackPlugin.SetFeedbackMessageHidden(13, 0, oTarget);
          });

          int iSpot = oTarget.GetSkillRank(Skill.Spot);
          if (oTarget.DetectModeActive || oTarget.HasFeatEffect(Feat.KeenSense))
            iSpot += NwRandom.Roll(Utils.random, 20, 1);

          if (!oPC.DoSkillCheck(Skill.PickPocket, iSpot))
          {
            oTarget.ControllingPlayer.FloatingTextString($"{oPC} est en train d'essayer de faire les poches de {oTarget} !", true);
            oPC.ControllingPlayer.FloatingTextString($"{oPC} est en train d'essayer de faire les poches de {oTarget} !", true);
          }

          Task waitForTouch = NwTask.Run(async () =>
          {
            TouchAttackResult touch = await oPC.TouchAttackMelee(oTarget);
            if(touch == TouchAttackResult.Miss)
            {
              oPC.ControllingPlayer.FloatingTextString($"Vous ne parvenez pas à atteindre les poches de {oTarget.Name} !", false);
              return;
            }

            int iStolenGold = (NwRandom.Roll(Utils.random, 20, 1) + oPC.GetSkillRank(Skill.PickPocket) - iSpot) * 10;

            if (oTarget.Gold >= iStolenGold)
            {
              oTarget.Gold = (uint)(oTarget.Gold - iStolenGold);
              oPC.GiveGold(iStolenGold);
              oPC.ControllingPlayer.FloatingTextString($"Vous venez de dérober {iStolenGold} pièces d'or des poches de {oTarget.Name} !", false);
            }
            else
            {
              oPC.ControllingPlayer.FloatingTextString($"Vous venez de vider les poches de {oTarget.Name} ! {oTarget.Gold} pièces d'or de plus pour vous.", false);
              oPC.GiveGold((int)oTarget.Gold);
              oTarget.Gold = 0;
            }
          });

          break;
        case Skill.AnimalEmpathy:
          if (oPC.Area.Tag == "Promenadetest")
          {
            oPC.ControllingPlayer.FloatingTextString("L'endroit est bien trop agité pour que vous puissiez vous permettre de nouer un lien avec l'animal.", false);
            EventsPlugin.SkipEvent();
          }
          break;
      }
    }

    [ScriptHandler("on_journal_open")]
    private void HandlePCJournalOpen(CallInfo callInfo)
    {
      if (Players.TryGetValue(callInfo.ObjectSelf, out Player player))
      {
        player.DoJournalUpdate = true;
        player.UpdateJournal();
      }
    }
    [ScriptHandler("on_journal_close")]
    private void HandlePCJournalClose(CallInfo callInfo)
    {
      if (Players.TryGetValue(callInfo.ObjectSelf, out Player player))
      {
        player.DoJournalUpdate = false;
      }
    }

    [ScriptHandler("map_pin_added")]
    private void HandleAddMapPin(CallInfo callInfo)
    {
      if (Players.TryGetValue(callInfo.ObjectSelf, out Player player))
      {
        int id = player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("NW_TOTAL_MAP_PINS").Value;
        player.mapPinDictionnary.Add(id, new MapPin(id, player.oid.ControlledCreature.Area.Tag, float.Parse(EventsPlugin.GetEventData("PIN_X")), float.Parse(EventsPlugin.GetEventData("PIN_Y")), EventsPlugin.GetEventData("PIN_NOTE")));
        player.SaveMapPinsToDatabase();
      }
    }

    [ScriptHandler("mappin_destroyed")]
    private void HandleDestroyMapPin(CallInfo callInfo)
    {
      if (Players.TryGetValue(callInfo.ObjectSelf, out Player player))
      {
        int pinId = int.Parse(EventsPlugin.GetEventData("PIN_ID"));
        player.mapPinDictionnary.Remove(pinId);
        player.SaveMapPinsToDatabase();
      }
    }
    [ScriptHandler("map_pin_changed")] 
    private void HandleChangeMapPin(CallInfo callInfo)
    {
      if (Players.TryGetValue(callInfo.ObjectSelf, out Player player))
      {
        MapPin updatedMapPin = player.mapPinDictionnary[int.Parse(EventsPlugin.GetEventData("PIN_ID"))];
        updatedMapPin.x = float.Parse(EventsPlugin.GetEventData("PIN_X"));
        updatedMapPin.y = float.Parse(EventsPlugin.GetEventData("PIN_Y"));
        updatedMapPin.note = EventsPlugin.GetEventData("PIN_NOTE");

        player.SaveMapPinsToDatabase();
      }
    }
    [ScriptHandler("on_input_emote")]
    private async void HandleInputEmote(CallInfo callInfo)
    {
      if (!Players.TryGetValue(callInfo.ObjectSelf, out Player player))
        return;

      Animation animation = Utils.TranslateEngineAnimation(int.Parse(EventsPlugin.GetEventData("ANIMATION")));

      switch (animation)
      {
        case Animation.LoopingMeditate:
        case Animation.LoopingConjure1:
        case Animation.LoopingConjure2:
        case Animation.LoopingGetMid:
        case Animation.LoopingGetLow:
        case Animation.LoopingListen:
        case Animation.LoopingLookFar:
        case Animation.LoopingPause:
        case Animation.LoopingPauseDrunk:
        case Animation.LoopingPauseTired:
        case Animation.LoopingPause2:
        case Animation.LoopingSpasm:
        case Animation.LoopingTalkForceful:
        case Animation.LoopingTalkLaughing:
        case Animation.LoopingTalkNormal:
        case Animation.LoopingTalkPleading:
        case Animation.LoopingWorship:
          EventsPlugin.SkipEvent();
          await player.oid.ControlledCreature.PlayAnimation(animation, 1, false, TimeSpan.FromDays(1));
          break;
        case Animation.LoopingDeadBack:
        case Animation.LoopingDeadFront:
        case Animation.LoopingSitChair:
        case Animation.LoopingSitCross:
          EventsPlugin.SkipEvent();
          await player.oid.ControlledCreature.PlayAnimation(animation, 1, false, TimeSpan.FromDays(1));

          player.menu.Close();
          player.menu.isOpen = true;
          player.LoadMenuQuickbar(QuickbarType.Sit);
          break;
      }
    }

    public static void HandleBeforeScrollLearn(OnItemScrollLearn onScrollLearn)
    {
      NwCreature oPC = onScrollLearn.Creature;

      if (!Players.TryGetValue(onScrollLearn.Creature, out Player player))
        return;

      onScrollLearn.PreventLearnScroll = true;
      NwItem oScroll = onScrollLearn.Scroll;
      int spellId = SpellUtils.GetSpellIDFromScroll(oScroll);
      byte spellLevel = SpellUtils.GetSpellLevelFromScroll(oScroll);

      if (spellId < 0 || spellLevel > 10)
      {
        Utils.LogMessageToDMs($"LEARN SPELL FROM SCROLL - Player : {oPC.Name}, SpellId : {spellId}, SpellLevel : {spellLevel} - INVALID");
        oPC.ControllingPlayer.SendServerMessage("HRP - Ce parchemin ne semble pas correctement configuré, impossible d'en apprendre quoique ce soit. Le staff a été informé du problème.", ColorConstants.Red);
        return;
      }

      if (oPC.GetClassInfo((ClassType)43).GetKnownSpells(spellLevel).Any(s => s.SpellType == (Spell)spellId))
      {
        oPC.ControllingPlayer.SendServerMessage("Ce sort est déjà inscrit dans votre grimoire.");
        return;
      }

      if (player.learnableSpells.ContainsKey(spellId))
      {
        LearnableSpell learnable = player.learnableSpells[spellId];

        if (learnable.nbScrollUsed <= 5)
        {
          learnable.acquiredPoints += learnable.GetPointsToNextLevel() / 20;
          learnable.nbScrollUsed += 1;
          oPC.ControllingPlayer.SendServerMessage($"Les informations supplémentaires contenues dans ce parchemin vous permettent d'affiner votre connaissance du sort {learnable.name.ColorString(ColorConstants.White)}. Votre apprentissage sera plus rapide.", new Color(32, 255, 32));
        }
        else
          oPC.ControllingPlayer.SendServerMessage("Vous avez déjà retiré tout ce que vous pouviez de ce parchemin.", ColorConstants.Orange);
        return;
      }
      else
      {
        player.learnableSpells.Add(spellId, new LearnableSpell((LearnableSpell)SkillSystem.learnableDictionary[spellId]));
        oPC.ControllingPlayer.SendServerMessage($"Le sort a été ajouté à votre liste d'apprentissage et est désormais disponible pour étude.");
        oScroll.Destroy();
      }
    }

    [ScriptHandler("collect_cancel")]
    private void HandleBeforeCollectCycleCancel(CallInfo callInfo)
    {
      callInfo.ObjectSelf.GetObjectVariable<LocalVariableInt>("_COLLECT_CANCELLED").Value = 1;
    }
    public static void HandleOnClientLevelUp(OnClientLevelUpBegin onLevelUp)
    {
      onLevelUp.PreventLevelUp = true;
      Utils.LogMessageToDMs($"{onLevelUp.Player.LoginCreature.Name} vient d'essayer de level up.");
      onLevelUp.Player.LoginCreature.Xp = 1;
    }
    public static void HandleCombatRoundEndForAutoSpells(CreatureEvents.OnCombatRoundEnd onCombatRoundEnd)
    {
      if(onCombatRoundEnd.Creature.GetObjectVariable<LocalVariableInt>("_AUTO_SPELL").HasNothing)
      {
        onCombatRoundEnd.Creature.OnCombatRoundEnd -= HandleCombatRoundEndForAutoSpells;
        return;
      }

      int spellId = onCombatRoundEnd.Creature.GetObjectVariable<LocalVariableInt>("_AUTO_SPELL").Value;
      NwGameObject target = onCombatRoundEnd.Creature.GetObjectVariable<LocalVariableObject<NwGameObject>>("_AUTO_SPELL_TARGET").Value;

      if(target != null && target.IsValid)
        _ = onCombatRoundEnd.Creature.ActionCastSpellAt((Spell)spellId, target);
      else
      {
        onCombatRoundEnd.Creature.GetObjectVariable<LocalVariableInt>("_AUTO_SPELL").Delete();
        onCombatRoundEnd.Creature.GetObjectVariable<LocalVariableObject<NwGameObject>>("_AUTO_SPELL_TARGET").Delete();
        onCombatRoundEnd.Creature.OnCombatRoundEnd -= HandleCombatRoundEndForAutoSpells;
      }
    }

    private static void HandlePortraitDemoEvents(ModuleEvents.OnNuiEvent nuiEvent)
    {
      if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != "portrait_demo")
        return;

      switch (nuiEvent.ElementId)
      {
        case "btnnext":

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              NuiBind<string> portraitId = new NuiBind<string>("po_id");
              NuiBind<string> portraitResRef = new NuiBind<string>("po_resref");
              NuiBind<int> portraitCategory = new NuiBind<int>("po_category");
              int min = 0;
              int max = 0;

              switch (portraitCategory.GetBindValue(nuiEvent.Player, nuiEvent.WindowToken))
              {
                case 0:
                  min = 164;
                  max = 167;
                  break;
              }

              int po_id = int.Parse(portraitId.GetBindValue(nuiEvent.Player, nuiEvent.WindowToken)) + 1;
              string resRef = "po_" + Portraits2da.portraitsTable.GetDataEntry(po_id).resRef + "h";

              if (po_id > max) po_id = min;
              if (po_id < min) po_id = max;

              portraitId.SetBindValue(nuiEvent.Player, nuiEvent.WindowToken, po_id.ToString());
              portraitResRef.SetBindValue(nuiEvent.Player, nuiEvent.WindowToken, resRef);

              break;
          }

          break;
      }
    }
    
    private static void HandleGenericNuiEvents(ModuleEvents.OnNuiEvent nuiEvent)
    {
      int windowToken = NWScript.NuiGetEventWindow();
      string window = nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken);
      string eventElement = nuiEvent.ElementId;
      NuiEventType eventType = nuiEvent.EventType;

      Log.Info($"nui window id : {window}");
      Log.Info($"nui element : {eventElement}");
      Log.Info($"nui event : {eventType}");
      Log.Info($"nwscript nuit event : {NWScript.NuiGetEventType()}");

      if (!Players.TryGetValue(nuiEvent.Player.LoginCreature, out Player player))
        return;

      switch (eventElement)
      {
        case "geometry":

          NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");
          NuiRect windowRectangle = geometry.GetBindValue(nuiEvent.Player, windowToken);

          if (player.windowRectangles.ContainsKey(window))
            player.windowRectangles[window] = windowRectangle;
          else
            player.windowRectangles.Add(window, windowRectangle);

          if (player.pcState == Player.PcState.Online)
            nuiEvent.Player.ExportCharacter();
          break;

        case "_window_":

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Open:
              if (!player.openedWindows.ContainsKey(window))
              {
                player.openedWindows.Add(window, windowToken);

                if (player.pcState == Player.PcState.Online)
                  nuiEvent.Player.ExportCharacter();
              }
              break;

            case NuiEventType.Close:
              if (player.openedWindows.ContainsKey(window))
              {
                player.openedWindows.Remove(window);

                if (player.pcState == Player.PcState.Online)
                  nuiEvent.Player.ExportCharacter();
              }
              break;
          }

          break;
      }
    }

    private static void HandleGuiEvents(ModuleEvents.OnPlayerGuiEvent guiEvent)
    {
      NwPlayer oPC = guiEvent.Player;
      oPC.LoginCreature.GetObjectVariable<DateTimeLocalVariable>("_LAST_ACTION_DATE").Value = DateTime.Now;
      
      switch (guiEvent.EventType)
      {
        case GuiEventType.DisabledPanelAttemptOpen:

          switch(guiEvent.OpenedPanel)
          {
            case GUIPanel.ExamineItem:

              if (!Players.TryGetValue(oPC.LoginCreature, out Player player))
                return;

              if (player.windows.ContainsKey("itemExamine"))
                ((Player.ItemExamineWindow)player.windows["itemExamine"]).CreateWindow((NwItem)guiEvent.EventObject);
              else
                player.windows.Add("itemExamine", new Player.ItemExamineWindow(player, (NwItem)guiEvent.EventObject));

              return;
          }
          
          break;

        case GuiEventType.ExamineObject:

          if(guiEvent.EventObject is NwCreature examineCreature && examineCreature == oPC.LoginCreature)
          {
            if (!Players.TryGetValue(oPC.LoginCreature, out Player player))
              return;

            if (player.newCraftJob != null && !player.openedWindows.ContainsKey("activeCraftJob"))
            {
              if (player.windows.ContainsKey("activeCraftJob"))
                ((Player.ActiveCraftJobWindow)player.windows["activeCraftJob"]).CreateWindow();
              else
                player.windows.Add("activeCraftJob", new Player.ActiveCraftJobWindow(player));
            }

            if (!player.openedWindows.ContainsKey("learnables"))
            {
              if (player.windows.ContainsKey("learnables"))
                ((Player.ActiveCraftJobWindow)player.windows["learnables"]).CreateWindow();
              else
                player.windows.Add("learnables", new Player.ActiveCraftJobWindow(player));
            }
          }

          break;

        case GuiEventType.PartyBarPortraitClick:
          oPC.SendServerMessage("portrait click");
          break;

        case GuiEventType.PlayerListPlayerClick:
          oPC.SendServerMessage("player list click");
          break;

        case GuiEventType.ChatBarFocus:

          if(guiEvent.ChatBarChannel != ChatBarChannel.Talk && guiEvent.ChatBarChannel != ChatBarChannel.Whisper)
            return;

          Effect visualMark = Effect.VisualEffect((VfxType)1248);
          visualMark.Tag = "VFX_SPEAKING_MARK";
          visualMark.SubType = EffectSubType.Supernatural;
          oPC.ControlledCreature.ApplyEffect(EffectDuration.Permanent, visualMark);

          break;

        case GuiEventType.ChatBarUnFocus:

          if (guiEvent.ChatBarChannel != ChatBarChannel.Talk && guiEvent.ChatBarChannel != ChatBarChannel.Whisper)
            return;

          foreach (Effect eff in oPC.ControlledCreature.ActiveEffects.Where(e => e.Tag == "VFX_SPEAKING_MARK"))
            oPC.ControlledCreature.RemoveEffect(eff);

          break;

        case GuiEventType.EffectIconClick:

          int nEffectIcon = (int)guiEvent.EffectIcon;
          EffectType nIconEffectType = EffectIconToEffectType(nEffectIcon);

          if (nIconEffectType == EffectType.InvalidEffect)
            return;

          bool bSkipDisplay = false, bIsSpellLevelAbsorptionPretendingToBeSpellImmunity = false;

          foreach (Effect eff in oPC.ControlledCreature.ActiveEffects.Where(e => e.EffectType == nIconEffectType))
          {
            string name = "Echec des sorts 50 %";
            int nAmount, nRemaining;

            if (eff.Spell.SpellType != Spell.AllSpells)
              name = eff.Spell.Name;

            float percentageRemaining = eff.DurationRemaining / eff.TotalDuration;
            Color color = ColorConstants.White;

            if (percentageRemaining > 0.5)
              color = new Color(32, 255, 32);
            else if (percentageRemaining < 0.25)
              color = ColorConstants.Maroon;
            else
              color = ColorConstants.Lime;

            string durationRemaining = eff.DurationType == EffectDuration.Permanent ? "Permanent".ColorString(new Color(32, 255, 32)) : $"Temps restant : {Utils.StripTimeSpanMilliseconds(DateTime.Now.AddSeconds(eff.DurationRemaining) - DateTime.Now)}".ColorString(color);
            string sStats = "";
            string sRacialTypeAlignment = "";

            string sModifier = "";

            switch (eff.EffectType)
            {
              case EffectType.AcIncrease:
              case EffectType.AcDecrease:

                sModifier = GetModifierType(eff.EffectType, EffectType.AcIncrease, EffectType.AcDecrease);
                sStats = $"{sModifier} {eff.IntParams.ElementAt(1)} {ACTypeToString(eff.IntParams.ElementAt(0))} CA";
                sRacialTypeAlignment = GetVersusRacialTypeAndAlignment(eff.IntParams.ElementAt(2), eff.IntParams.ElementAt(3), eff.IntParams.ElementAt(4));

                break;

              case EffectType.AttackIncrease:
              case EffectType.AttackDecrease:

                sModifier = GetModifierType(eff.EffectType, EffectType.AttackIncrease, EffectType.AttackDecrease);
                sStats = sModifier + eff.IntParams.ElementAt(0) + " BA";
                sRacialTypeAlignment = GetVersusRacialTypeAndAlignment(eff.IntParams.ElementAt(2), eff.IntParams.ElementAt(3), eff.IntParams.ElementAt(4));

                break;

              case EffectType.SavingThrowIncrease:
              case EffectType.SavingThrowDecrease:

                sModifier = GetModifierType(eff.EffectType, EffectType.SavingThrowIncrease, EffectType.SavingThrowDecrease);
                string sSavingThrow = SavingThrowToString(eff.IntParams.ElementAt(1));
                string sSavingThrowType = SavingThrowTypeToString(eff.IntParams.ElementAt(2));
                sStats = sModifier + eff.IntParams.ElementAt(0) + " " + sSavingThrow + (sSavingThrowType == "" ? "" : " (vs. " + sSavingThrowType + ")");
                sRacialTypeAlignment = GetVersusRacialTypeAndAlignment(eff.IntParams.ElementAt(3), eff.IntParams.ElementAt(4), eff.IntParams.ElementAt(5));

                break;

              case EffectType.AbilityIncrease:
              case EffectType.AbilityDecrease:
                {
                  Ability nAbility = AbilityTypeFromEffectIconAbility(nEffectIcon);

                  if ((int)nAbility != eff.IntParams.ElementAt(0))
                    bSkipDisplay = true;
                  else
                  {
                    sModifier = GetModifierType(eff.EffectType, EffectType.AbilityIncrease, EffectType.AbilityDecrease);
                    sStats = $"{sModifier}{eff.IntParams.ElementAt(1)}  {StringUtils.TranslateAttributeToFrench(nAbility)}";
                  }
                  break;
                }

              case EffectType.DamageIncrease:
              case EffectType.DamageDecrease:
                {
                  sModifier = GetModifierType(eff.EffectType, EffectType.DamageIncrease, EffectType.DamageDecrease);

                  sStats = sModifier + NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("iprp_damagecost", "Name", eff.IntParams.ElementAt(0)))) + " (" + DamageTypeToString(eff.IntParams.ElementAt(1)) + ")";
                  sRacialTypeAlignment = GetVersusRacialTypeAndAlignment(eff.IntParams.ElementAt(2), eff.IntParams.ElementAt(3), eff.IntParams.ElementAt(4));
                  break;
                }

              case EffectType.SkillIncrease:
              case EffectType.SkillDecrease:
                {
                  int nSkill = eff.IntParams.ElementAt(0);
                  string sSkill = nSkill == 255 ? "Toute compétence" : NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("skills", "Name", nSkill)));
                  sModifier = GetModifierType(eff.EffectType, EffectType.SkillIncrease, EffectType.SkillDecrease);
                  sStats = sModifier + eff.IntParams.ElementAt(1) + " " + sSkill;
                  sRacialTypeAlignment = GetVersusRacialTypeAndAlignment(eff.IntParams.ElementAt(2), eff.IntParams.ElementAt(3), eff.IntParams.ElementAt(4));
                  break;
                }

              case EffectType.TemporaryHitpoints:
                {
                  sStats = "+" + eff.IntParams.ElementAt(0) + " Points de vie";
                  break;
                }

              case EffectType.DamageReduction:
                {
                  nAmount = eff.IntParams.ElementAt(0);
                  int nDamagePower = eff.IntParams.ElementAt(1);
                  nDamagePower = nDamagePower > 6 ? --nDamagePower : nDamagePower;
                  nRemaining = eff.IntParams.ElementAt(2);
                  sStats = nAmount + "/+" + nDamagePower + " (" + (nRemaining == 0 ? "Illimité" : nRemaining + " Dégâts Restants") + ")";
                  break;
                }

              case EffectType.DamageResistance:
                {
                  nAmount = eff.IntParams.ElementAt(1);
                  nRemaining = eff.IntParams.ElementAt(2);
                  sStats = nAmount + "/- " + DamageTypeToString(eff.IntParams.ElementAt(0)) + " Résistance (" + (nRemaining == 0 ? "Illimité" : nRemaining + " Dégâts Restants") + ")";
                  break;
                }

              case EffectType.Immunity:
                {
                  ImmunityType nImmunity = ImmunityTypeFromEffectIconImmunity(nEffectIcon);

                  if ((int)nImmunity != eff.IntParams.ElementAt(0))
                    bSkipDisplay = true;
                  else
                  {
                    sStats = NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("effecticons", "StrRef", nEffectIcon)));
                    sRacialTypeAlignment = GetVersusRacialTypeAndAlignment(eff.IntParams.ElementAt(1), eff.IntParams.ElementAt(2), eff.IntParams.ElementAt(3));
                  }
                  break;
                }

              case EffectType.DamageImmunityIncrease:
              case EffectType.DamageImmunityDecrease:
                {
                  int nDamageType = eff.IntParams.ElementAt(0);
                  DamageType nDamageTypeFromIcon = DamageTypeFromEffectIconDamageImmunity(nEffectIcon);

                  if (nDamageTypeFromIcon != DamageType.BaseWeapon && nDamageType != (int)nDamageTypeFromIcon)
                    bSkipDisplay = true;

                  sModifier = GetModifierType(eff.EffectType, EffectType.DamageImmunityIncrease, EffectType.DamageImmunityDecrease);
                  sStats = sModifier + eff.IntParams.ElementAt(1) + "% " + DamageTypeToString(nDamageType) + " Damage Immunity";
                  break;
                }

              case EffectType.SpellImmunity:
                {
                  sStats = "Immunité au sort : " + NwSpell.FromSpellId(eff.IntParams.ElementAt(0)).Name;
                  break;
                }

              case EffectType.SpellLevelAbsorption:
                {
                  int nMaxSpellLevelAbsorbed = eff.IntParams.ElementAt(0);
                  bool bUnlimited = eff.IntParams.ElementAt(3).ToBool();
                  string sSpellLevel;
                  switch (nMaxSpellLevelAbsorbed)
                  {
                    case 0: sSpellLevel = "Tours de magie"; break;
                    default: sSpellLevel = "Niveau " + nMaxSpellLevelAbsorbed; break;
                  }
                  sSpellLevel += " Level" + (nMaxSpellLevelAbsorbed == 0 ? "" : " and Below");
                  string sSpellSchool = SpellSchoolToString(eff.IntParams.ElementAt(2));
                  string sRemainingSpellLevels = bUnlimited ? "" : "(" + eff.IntParams.ElementAt(1) + " Niveaux de sorts restants)";
                  sStats = sSpellLevel + " " + sSpellSchool + " Spell Immunity " + sRemainingSpellLevels;

                  if (bIsSpellLevelAbsorptionPretendingToBeSpellImmunity)
                    nIconEffectType = EffectType.SpellImmunity;
                  else if (bUnlimited && !bIsSpellLevelAbsorptionPretendingToBeSpellImmunity)
                    bSkipDisplay = true;

                  break;
                }

              case EffectType.Regenerate:
                {
                  sStats = "+" + eff.IntParams.ElementAt(0) + " HP / " + (eff.IntParams.ElementAt(1) / 1000.0f).ToString("0.00") + "s";
                  break;
                }

              case EffectType.Poison:
                {
                  sStats = "Poison: " + NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("poison", "Name", eff.IntParams.ElementAt(0))));
                  break;
                }

              case EffectType.Disease:
                {
                  sStats = "Disease: " + NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("disease", "Name", eff.IntParams.ElementAt(0))));
                  break;
                }

              case EffectType.Curse:
                {
                  int nAbility;
                  string sAbilityDecrease = "";

                  for (nAbility = 0; nAbility < 6; nAbility++)
                  {
                    int nAbilityMod = eff.IntParams.ElementAt(nAbility);
                    if (nAbilityMod > 0)
                    {
                      string sAbility = StringUtils.TranslateAttributeToFrench((Ability)nAbility).Substring(0, 3);
                      sAbilityDecrease += "-" + nAbilityMod + " " + sAbility + ", ";
                    }
                  }

                  sAbilityDecrease = sAbilityDecrease.Substring(0, sAbilityDecrease.Length - 2);
                  sStats = sAbilityDecrease;
                  break;
                }

              case EffectType.MovementSpeedIncrease:
              case EffectType.MovementSpeedDecrease:
                {
                  sModifier = GetModifierType(eff.EffectType, EffectType.MovementSpeedIncrease, EffectType.MovementSpeedDecrease);
                  sStats = sModifier + eff.IntParams.ElementAt(0) + "% Movement Speed";
                  break;
                }

              case EffectType.ElementalShield:
                {
                  sStats = eff.IntParams.ElementAt(0) + " + " + NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("iprp_damagecost", "Name", eff.IntParams.ElementAt(1)) + " (" + DamageTypeToString(eff.IntParams.ElementAt(2)))) + ")";
                  break;
                }

              case EffectType.NegativeLevel:
                {
                  sStats = "-" + eff.IntParams.ElementAt(0) + " Niveaux";
                  break;
                }

              case EffectType.Concealment:
                {
                  string sMissChance = MissChanceToString(eff.IntParams.ElementAt(4) - 1);
                  sStats = eff.IntParams.ElementAt(0) + "% Concealment" + (sMissChance == "" ? "" : " (" + sMissChance + ")");
                  sRacialTypeAlignment = GetVersusRacialTypeAndAlignment(eff.IntParams.ElementAt(1), eff.IntParams.ElementAt(2), eff.IntParams.ElementAt(3));
                  break;
                }

              case EffectType.SpellResistanceIncrease:
              case EffectType.SpellResistanceDecrease:
                {
                  sModifier = GetModifierType(eff.EffectType, EffectType.SpellResistanceIncrease, EffectType.SpellResistanceDecrease);
                  sStats = sModifier + eff.IntParams.ElementAt(0) + " Résistance aux sorts";
                  break;
                }

              case EffectType.SpellFailure:
                {
                  sStats = eff.IntParams.ElementAt(0) + "% Echec des sorts (Ecole : " + SpellSchoolToString(eff.IntParams.ElementAt(1)) + ")";
                  break;
                }

              case EffectType.Invisibility:
                {
                  int nInvisibilityType = eff.IntParams.ElementAt(0);
                  if (nEffectIcon == NWScript.EFFECT_ICON_INVISIBILITY)
                    bSkipDisplay = nInvisibilityType != NWScript.INVISIBILITY_TYPE_NORMAL;
                  else if (nEffectIcon == NWScript.EFFECT_ICON_IMPROVEDINVISIBILITY)
                    bSkipDisplay = nInvisibilityType != NWScript.INVISIBILITY_TYPE_IMPROVED;
                  if (!bSkipDisplay)
                  {
                    sStats = "Invisibilité" + (nInvisibilityType == NWScript.INVISIBILITY_TYPE_IMPROVED ? "Suprème " : "");
                    sRacialTypeAlignment = GetVersusRacialTypeAndAlignment(eff.IntParams.ElementAt(1), eff.IntParams.ElementAt(2), eff.IntParams.ElementAt(3));
                  }
                  break;
                }
            }

            if (!bSkipDisplay)
            {
              string creator = !eff.Creator.IsValid ? "Inconnue" : eff.Creator.Name;
              string message = $"\n{name.ColorString(ColorConstants.Red)}\n";
              message += $"{durationRemaining}\n";
              message += sStats == "" ? "" : " -> " + sStats + sRacialTypeAlignment + "\n";
              message += $"Source : {creator}";


              oPC.SendServerMessage(message, ColorConstants.Orange);
            }
          }

          break;
      }
    }
    private static string GetModifierType(EffectType nEffectType, EffectType nPlus, EffectType nMinus)
    {
      return nEffectType == nPlus ? "+" : nEffectType == nMinus ? "-" : "";
    }
    private static string ACTypeToString(int nACType)
    {
      
      switch ((ACBonus)nACType)
      {
        case ACBonus.Dodge: return "Esquive";
        case ACBonus.Natural: return "Naturelle";
        case ACBonus.ArmourEnchantment: return "Armure";
        case ACBonus.ShieldEnchantment: return "Bouclier";
        case ACBonus.Deflection: return "Parade";
      }

      return "";
    }
    private static string SpellSchoolToString(int nSpellSchool)
    {
      switch (nSpellSchool)
      {
        case NWScript.SPELL_SCHOOL_GENERAL: return "Généraliste";
        case NWScript.SPELL_SCHOOL_ABJURATION: return "Abjuration";
        case NWScript.SPELL_SCHOOL_CONJURATION: return "Invocation";
        case NWScript.SPELL_SCHOOL_DIVINATION: return "Divination";
        case NWScript.SPELL_SCHOOL_ENCHANTMENT: return "Enchantement";
        case NWScript.SPELL_SCHOOL_EVOCATION: return "Evocation";
        case NWScript.SPELL_SCHOOL_ILLUSION: return "Illusion";
        case NWScript.SPELL_SCHOOL_NECROMANCY: return "Nécromancie";
        case NWScript.SPELL_SCHOOL_TRANSMUTATION: return "Transmutation";
      }

      return "";
    }

    private static string MissChanceToString(int nMissChance)
    {
      switch ((MissChanceType)nMissChance)
      {
        case MissChanceType.VsRanged: return "vs. Distance";
        case MissChanceType.VsMelee: return "vs. Mêlée";
      }

      return "";
    }
    private static DamageType DamageTypeFromEffectIconDamageImmunity(int nEffectIcon)
    {
      switch (nEffectIcon)
      {
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_MAGIC:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_MAGIC_DECREASE:
          return DamageType.Magical;
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_ACID:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_ACID_DECREASE:
          return DamageType.Acid;
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_COLD:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_COLD_DECREASE:
          return DamageType.Cold;
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_DIVINE:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_DIVINE_DECREASE:
          return DamageType.Divine;
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_ELECTRICAL:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_ELECTRICAL_DECREASE:
          return DamageType.Electrical;
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_FIRE:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_FIRE_DECREASE:
          return DamageType.Fire;
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_NEGATIVE:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_NEGATIVE_DECREASE:
          return DamageType.Negative;
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_POSITIVE:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_POSITIVE_DECREASE:
          return DamageType.Positive;
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_SONIC:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_SONIC_DECREASE:
          return DamageType.Sonic;
      }

      return DamageType.BaseWeapon;
    }
    private static ImmunityType ImmunityTypeFromEffectIconImmunity(int nEffectIcon)
    {
      switch (nEffectIcon)
      {
        case NWScript.EFFECT_ICON_IMMUNITY_MIND: return ImmunityType.MindSpells;
        case NWScript.EFFECT_ICON_IMMUNITY_POISON: return ImmunityType.Poison;
        case NWScript.EFFECT_ICON_IMMUNITY_DISEASE: return ImmunityType.Disease;
        case NWScript.EFFECT_ICON_IMMUNITY_FEAR: return ImmunityType.Fear;
        case NWScript.EFFECT_ICON_IMMUNITY_TRAP: return ImmunityType.Trap;
        case NWScript.EFFECT_ICON_IMMUNITY_PARALYSIS: return ImmunityType.Paralysis;
        case NWScript.EFFECT_ICON_IMMUNITY_BLINDNESS: return ImmunityType.Blindness;
        case NWScript.EFFECT_ICON_IMMUNITY_DEAFNESS: return ImmunityType.Deafness;
        case NWScript.EFFECT_ICON_IMMUNITY_SLOW: return ImmunityType.Slow;
        case NWScript.EFFECT_ICON_IMMUNITY_ENTANGLE: return ImmunityType.Entangle;
        case NWScript.EFFECT_ICON_IMMUNITY_SILENCE: return ImmunityType.Silence;
        case NWScript.EFFECT_ICON_IMMUNITY_STUN: return ImmunityType.Stun;
        case NWScript.EFFECT_ICON_IMMUNITY_SLEEP: return ImmunityType.Sleep;
        case NWScript.EFFECT_ICON_IMMUNITY_CHARM: return ImmunityType.Charm;
        case NWScript.EFFECT_ICON_IMMUNITY_DOMINATE: return ImmunityType.Dominate;
        case NWScript.EFFECT_ICON_IMMUNITY_CONFUSE: return ImmunityType.Confused;
        case NWScript.EFFECT_ICON_IMMUNITY_CURSE: return ImmunityType.Cursed;
        case NWScript.EFFECT_ICON_IMMUNITY_DAZED: return ImmunityType.Dazed;
        case NWScript.EFFECT_ICON_IMMUNITY_ABILITY_DECREASE: return ImmunityType.AbilityDecrease;
        case NWScript.EFFECT_ICON_IMMUNITY_ATTACK_DECREASE: return ImmunityType.AttackDecrease;
        case NWScript.EFFECT_ICON_IMMUNITY_DAMAGE_DECREASE: return ImmunityType.DamageDecrease;
        case NWScript.EFFECT_ICON_IMMUNITY_DAMAGE_IMMUNITY_DECREASE: return ImmunityType.DamageImmunityDecrease;
        case NWScript.EFFECT_ICON_IMMUNITY_AC_DECREASE: return ImmunityType.AcDecrease;
        case NWScript.EFFECT_ICON_IMMUNITY_MOVEMENT_SPEED_DECREASE: return ImmunityType.MovementSpeedDecrease;
        case NWScript.EFFECT_ICON_IMMUNITY_SAVING_THROW_DECREASE: return ImmunityType.SavingThrowDecrease;
        case NWScript.EFFECT_ICON_IMMUNITY_SPELL_RESISTANCE_DECREASE: return ImmunityType.SpellResistanceDecrease;
        case NWScript.EFFECT_ICON_IMMUNITY_SKILL_DECREASE: return ImmunityType.SkillDecrease;
        case NWScript.EFFECT_ICON_IMMUNITY_KNOCKDOWN: return ImmunityType.Knockdown;
        case NWScript.EFFECT_ICON_IMMUNITY_NEGATIVE_LEVEL: return ImmunityType.NegativeLevel;
        case NWScript.EFFECT_ICON_IMMUNITY_SNEAK_ATTACK: return ImmunityType.SneakAttack;
        case NWScript.EFFECT_ICON_IMMUNITY_CRITICAL_HIT: return ImmunityType.CriticalHit;
        case NWScript.EFFECT_ICON_IMMUNITY_DEATH_MAGIC: return ImmunityType.Death;
      }

      return ImmunityType.None;
    }
    private static string DamageTypeToString(int nDamageType)
    {
      switch ((DamageType)nDamageType)
      {
        case DamageType.Bludgeoning: return "Contondant";
        case DamageType.Piercing: return "Perçant";
        case DamageType.Slashing: return "Tranchant";
        case DamageType.Magical: return "Magique";
        case DamageType.Acid: return "Acide";
        case DamageType.Cold: return "Froid";
        case DamageType.Divine: return "Divin";
        case DamageType.Electrical: return "Electrique";
        case DamageType.Fire: return "Feu";
        case DamageType.Negative: return "Negatif";
        case DamageType.Positive: return "Positif";
        case DamageType.Sonic: return "Sonique";
        case DamageType.BaseWeapon: return "Base";
      }

      return "";
    }
    private static Ability AbilityTypeFromEffectIconAbility(int nEffectIcon)
    {
      switch (nEffectIcon)
      {
        case NWScript.EFFECT_ICON_ABILITY_INCREASE_STR:
        case NWScript.EFFECT_ICON_ABILITY_DECREASE_STR:
          return Ability.Strength;
        case NWScript.EFFECT_ICON_ABILITY_INCREASE_DEX:
        case NWScript.EFFECT_ICON_ABILITY_DECREASE_DEX:
          return Ability.Dexterity;
        case NWScript.EFFECT_ICON_ABILITY_INCREASE_CON:
        case NWScript.EFFECT_ICON_ABILITY_DECREASE_CON:
          return Ability.Constitution;
        case NWScript.EFFECT_ICON_ABILITY_INCREASE_INT:
        case NWScript.EFFECT_ICON_ABILITY_DECREASE_INT:
          return Ability.Intelligence;
        case NWScript.EFFECT_ICON_ABILITY_INCREASE_WIS:
        case NWScript.EFFECT_ICON_ABILITY_DECREASE_WIS:
          return Ability.Wisdom;
        case NWScript.EFFECT_ICON_ABILITY_INCREASE_CHA:
        case NWScript.EFFECT_ICON_ABILITY_DECREASE_CHA:
          return Ability.Charisma;
      }

      return Ability.Strength;
    }

    private static string SavingThrowToString(int nSavingThrow)
    {
      switch (nSavingThrow)
      {
        case NWScript.SAVING_THROW_ALL: return "Universel";
        case NWScript.SAVING_THROW_FORT: return "Vigueur";
        case NWScript.SAVING_THROW_REFLEX: return "Réflexes";
        case NWScript.SAVING_THROW_WILL: return "Volonté";
      }

      return "";
    }
    private static string SavingThrowTypeToString(int nSavingThrowType)
    {
      switch ((SavingThrowType)nSavingThrowType)
      {
        case SavingThrowType.MindSpells: return "Sorts affectant l'esprit";
        case SavingThrowType.Poison: return "Poison";
        case SavingThrowType.Disease: return "Maladie";
        case SavingThrowType.Fear: return "Peur";
        case SavingThrowType.Sonic: return "Sonique";
        case SavingThrowType.Acid: return "Acide";
        case SavingThrowType.Fire: return "Feu";
        case SavingThrowType.Electricity: return "Electricité";
        case SavingThrowType.Positive: return "Positif";
        case SavingThrowType.Negative: return "Négatif";
        case SavingThrowType.Death: return "Mort";
        case SavingThrowType.Cold: return "Froid";
        case SavingThrowType.Divine: return "Divin";
        case SavingThrowType.Trap: return "Pièges";
        case SavingThrowType.Spell: return "Sorts";
        case SavingThrowType.Good: return "Bon";
        case SavingThrowType.Evil: return "Mauvais";
        case SavingThrowType.Law: return "Loi";
        case SavingThrowType.Chaos: return "Chaos";
      }

      return "";
    }
    private static string GetVersusRacialTypeAndAlignment(int nRacialType, int nLawfulChaotic, int nGoodEvil)
    {
      string sRacialType = nRacialType == NWScript.RACIAL_TYPE_INVALID ? "" : NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("racialtypes", "NamePlural", nRacialType)));
      string sLawfulChaotic = nLawfulChaotic == NWScript.ALIGNMENT_LAWFUL ? "Lawful" : nLawfulChaotic == NWScript.ALIGNMENT_CHAOTIC ? "Chaotic" : "";
      string sGoodEvil = nGoodEvil == NWScript.ALIGNMENT_GOOD ? "Good" : nGoodEvil == NWScript.ALIGNMENT_EVIL ? "Evil" : "";
      string sAlignment = sLawfulChaotic + (sLawfulChaotic == "" ? sGoodEvil : (sGoodEvil == "" ? "" : " " + sGoodEvil));
      return (sRacialType != "" || sAlignment != "") ? (" vs. " + sAlignment + (sAlignment == "" ? sRacialType : (sRacialType == "" ? "" : " " + sRacialType))) : "";
    }
    private static EffectType EffectIconToEffectType(int nEffectIcon)
    {
      switch (nEffectIcon)
      {
        case NWScript.EFFECT_ICON_INVALID: return EffectType.InvalidEffect;

        // *** No Extra Stats
        case NWScript.EFFECT_ICON_BLIND: return EffectType.Blindness;
        case NWScript.EFFECT_ICON_CHARMED: return EffectType.Charmed;
        case NWScript.EFFECT_ICON_CONFUSED: return EffectType.Confused;
        case NWScript.EFFECT_ICON_FRIGHTENED: return EffectType.Frightened;
        case NWScript.EFFECT_ICON_DOMINATED: return EffectType.Dominated;
        case NWScript.EFFECT_ICON_PARALYZE: return EffectType.Paralyze;
        case NWScript.EFFECT_ICON_DAZED: return EffectType.Dazed;
        case NWScript.EFFECT_ICON_STUNNED: return EffectType.Stunned;
        case NWScript.EFFECT_ICON_SLEEP: return EffectType.Sleep;
        case NWScript.EFFECT_ICON_SILENCE: return EffectType.Silence;
        case NWScript.EFFECT_ICON_TURNED: return EffectType.Turned;
        case NWScript.EFFECT_ICON_HASTE: return EffectType.Haste;
        case NWScript.EFFECT_ICON_SLOW: return EffectType.Slow;
        case NWScript.EFFECT_ICON_ENTANGLE: return EffectType.Entangle;
        case NWScript.EFFECT_ICON_DEAF: return EffectType.Deaf;
        case NWScript.EFFECT_ICON_DARKNESS: return EffectType.Darkness;
        case NWScript.EFFECT_ICON_POLYMORPH: return EffectType.Polymorph;
        case NWScript.EFFECT_ICON_SANCTUARY: return EffectType.Sanctuary;
        case NWScript.EFFECT_ICON_TRUESEEING: return EffectType.TrueSeeing;
        case NWScript.EFFECT_ICON_SEEINVISIBILITY: return EffectType.SeeInvisible;
        case NWScript.EFFECT_ICON_ETHEREALNESS: return EffectType.Ethereal;
        case NWScript.EFFECT_ICON_PETRIFIED: return EffectType.Petrify;
        // ***
        
        case NWScript.EFFECT_ICON_DAMAGE_RESISTANCE: return EffectType.DamageResistance;
        case NWScript.EFFECT_ICON_REGENERATE: return EffectType.Regenerate;
        case NWScript.EFFECT_ICON_DAMAGE_REDUCTION: return EffectType.DamageReduction;
        case NWScript.EFFECT_ICON_TEMPORARY_HITPOINTS: return EffectType.TemporaryHitpoints;
        case NWScript.EFFECT_ICON_IMMUNITY: return EffectType.Immunity;
        case NWScript.EFFECT_ICON_POISON: return EffectType.Poison;
        case NWScript.EFFECT_ICON_DISEASE: return EffectType.Disease;
        case NWScript.EFFECT_ICON_CURSE: return EffectType.Curse;
        case NWScript.EFFECT_ICON_ATTACK_INCREASE: return EffectType.AttackIncrease;
        case NWScript.EFFECT_ICON_ATTACK_DECREASE: return EffectType.AttackDecrease;
        case NWScript.EFFECT_ICON_DAMAGE_INCREASE: return EffectType.DamageIncrease;
        case NWScript.EFFECT_ICON_DAMAGE_DECREASE: return EffectType.DamageDecrease;
        case NWScript.EFFECT_ICON_AC_INCREASE: return EffectType.AcIncrease;
        case NWScript.EFFECT_ICON_AC_DECREASE: return EffectType.AcDecrease;
        case NWScript.EFFECT_ICON_MOVEMENT_SPEED_INCREASE: return EffectType.MovementSpeedIncrease;
        case NWScript.EFFECT_ICON_MOVEMENT_SPEED_DECREASE: return EffectType.MovementSpeedDecrease;
        case NWScript.EFFECT_ICON_SAVING_THROW_DECREASE: return EffectType.SavingThrowDecrease;
        case NWScript.EFFECT_ICON_SPELL_RESISTANCE_INCREASE: return EffectType.SpellResistanceIncrease;
        case NWScript.EFFECT_ICON_SPELL_RESISTANCE_DECREASE: return EffectType.SpellResistanceDecrease;
        case NWScript.EFFECT_ICON_SKILL_INCREASE: return EffectType.SkillIncrease;
        case NWScript.EFFECT_ICON_SKILL_DECREASE: return EffectType.SkillDecrease;
        case NWScript.EFFECT_ICON_ELEMENTALSHIELD: return EffectType.ElementalShield;
        case NWScript.EFFECT_ICON_LEVELDRAIN: return EffectType.NegativeLevel;
        case NWScript.EFFECT_ICON_SPELLLEVELABSORPTION: return EffectType.SpellLevelAbsorption;
        case NWScript.EFFECT_ICON_SPELLIMMUNITY: return EffectType.SpellImmunity;
        case NWScript.EFFECT_ICON_CONCEALMENT: return EffectType.Concealment;
        case NWScript.EFFECT_ICON_EFFECT_SPELL_FAILURE: return EffectType.SpellFailure;

        case NWScript.EFFECT_ICON_INVISIBILITY:
        case NWScript.EFFECT_ICON_IMPROVEDINVISIBILITY: return EffectType.Invisibility;

        case NWScript.EFFECT_ICON_ABILITY_INCREASE_STR:
        case NWScript.EFFECT_ICON_ABILITY_INCREASE_DEX:
        case NWScript.EFFECT_ICON_ABILITY_INCREASE_CON:
        case NWScript.EFFECT_ICON_ABILITY_INCREASE_INT:
        case NWScript.EFFECT_ICON_ABILITY_INCREASE_WIS:
        case NWScript.EFFECT_ICON_ABILITY_INCREASE_CHA: return EffectType.AbilityIncrease;

        case NWScript.EFFECT_ICON_ABILITY_DECREASE_STR:
        case NWScript.EFFECT_ICON_ABILITY_DECREASE_CHA:
        case NWScript.EFFECT_ICON_ABILITY_DECREASE_DEX:
        case NWScript.EFFECT_ICON_ABILITY_DECREASE_CON:
        case NWScript.EFFECT_ICON_ABILITY_DECREASE_INT:
        case NWScript.EFFECT_ICON_ABILITY_DECREASE_WIS: return EffectType.AbilityDecrease;

        case NWScript.EFFECT_ICON_IMMUNITY_ALL:
        case NWScript.EFFECT_ICON_IMMUNITY_MIND:
        case NWScript.EFFECT_ICON_IMMUNITY_POISON:
        case NWScript.EFFECT_ICON_IMMUNITY_DISEASE:
        case NWScript.EFFECT_ICON_IMMUNITY_FEAR:
        case NWScript.EFFECT_ICON_IMMUNITY_TRAP:
        case NWScript.EFFECT_ICON_IMMUNITY_PARALYSIS:
        case NWScript.EFFECT_ICON_IMMUNITY_BLINDNESS:
        case NWScript.EFFECT_ICON_IMMUNITY_DEAFNESS:
        case NWScript.EFFECT_ICON_IMMUNITY_SLOW:
        case NWScript.EFFECT_ICON_IMMUNITY_ENTANGLE:
        case NWScript.EFFECT_ICON_IMMUNITY_SILENCE:
        case NWScript.EFFECT_ICON_IMMUNITY_STUN:
        case NWScript.EFFECT_ICON_IMMUNITY_SLEEP:
        case NWScript.EFFECT_ICON_IMMUNITY_CHARM:
        case NWScript.EFFECT_ICON_IMMUNITY_DOMINATE:
        case NWScript.EFFECT_ICON_IMMUNITY_CONFUSE:
        case NWScript.EFFECT_ICON_IMMUNITY_CURSE:
        case NWScript.EFFECT_ICON_IMMUNITY_DAZED:
        case NWScript.EFFECT_ICON_IMMUNITY_ABILITY_DECREASE:
        case NWScript.EFFECT_ICON_IMMUNITY_ATTACK_DECREASE:
        case NWScript.EFFECT_ICON_IMMUNITY_DAMAGE_DECREASE:
        case NWScript.EFFECT_ICON_IMMUNITY_DAMAGE_IMMUNITY_DECREASE:
        case NWScript.EFFECT_ICON_IMMUNITY_AC_DECREASE:
        case NWScript.EFFECT_ICON_IMMUNITY_MOVEMENT_SPEED_DECREASE:
        case NWScript.EFFECT_ICON_IMMUNITY_SAVING_THROW_DECREASE:
        case NWScript.EFFECT_ICON_IMMUNITY_SPELL_RESISTANCE_DECREASE:
        case NWScript.EFFECT_ICON_IMMUNITY_SKILL_DECREASE:
        case NWScript.EFFECT_ICON_IMMUNITY_KNOCKDOWN:
        case NWScript.EFFECT_ICON_IMMUNITY_NEGATIVE_LEVEL:
        case NWScript.EFFECT_ICON_IMMUNITY_SNEAK_ATTACK:
        case NWScript.EFFECT_ICON_IMMUNITY_CRITICAL_HIT:
        case NWScript.EFFECT_ICON_IMMUNITY_DEATH_MAGIC: return EffectType.Immunity;

        case NWScript.EFFECT_ICON_SAVING_THROW_INCREASE:
        case NWScript.EFFECT_ICON_REFLEX_SAVE_INCREASED:
        case NWScript.EFFECT_ICON_FORT_SAVE_INCREASED:
        case NWScript.EFFECT_ICON_WILL_SAVE_INCREASED: return EffectType.SavingThrowIncrease;

        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_INCREASE:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_MAGIC:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_ACID:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_COLD:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_DIVINE:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_ELECTRICAL:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_FIRE:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_NEGATIVE:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_POSITIVE:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_SONIC: return EffectType.DamageImmunityIncrease;

        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_DECREASE:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_MAGIC_DECREASE:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_ACID_DECREASE:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_COLD_DECREASE:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_DIVINE_DECREASE:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_ELECTRICAL_DECREASE:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_FIRE_DECREASE:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_NEGATIVE_DECREASE:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_POSITIVE_DECREASE:
        case NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_SONIC_DECREASE: return EffectType.DamageImmunityDecrease;

          case NWScript.EFFECT_ICON_INVULNERABLE: return EffectType.Invulnerable;
          case NWScript.EFFECT_ICON_WOUNDING: return EffectType.InvalidEffect;
          case NWScript.EFFECT_ICON_TAUNTED: return EffectType.InvalidEffect;
          case NWScript.EFFECT_ICON_TIMESTOP: return EffectType.TimeStop;
          case NWScript.EFFECT_ICON_BLINDNESS: return EffectType.Blindness;
          case NWScript.EFFECT_ICON_DISPELMAGICBEST: return EffectType.InvalidEffect;
          case NWScript.EFFECT_ICON_DISPELMAGICALL: return EffectType.InvalidEffect;
          case NWScript.EFFECT_ICON_ENEMY_ATTACK_BONUS: return EffectType.InvalidEffect;
          case NWScript.EFFECT_ICON_FATIGUE: return EffectType.InvalidEffect;
      }

      return EffectType.InvalidEffect;
    }

    // TODO : Probablement refaire le système de déguisement plus proprement
    /*private static int HandlePlayerPerceived(CallInfo callInfo)
    {
        Player oPC;

        if (Players.TryGetValue(oidSelf, out oPC))
        {
            Player oPerceived;
            if (Players.TryGetValue(NWScript.GetLastPerceived(), out oPerceived))
            {
                if (NWScript.GetIsPC(oPerceived.oid) != 1 || NWScript.GetIsDM(oPerceived.oid) == 1 || NWScript.GetIsDMPossessed(oPerceived.oid) == 1 || oPerceived.disguiseName.Length == 0)
                    return 0;

                if (!oPC.disguiseDetectTimer.ContainsKey(oPC.oid) || (DateTime.Now - oPC.disguiseDetectTimer[oPerceived.oid]).TotalSeconds > 1800)
                {
                    oPC.disguiseDetectTimer[oPerceived.oid] = DateTime.Now;

                    int[] iPCSenseSkill = { NWScript.GetSkillRank(NWScript.SKILL_LISTEN, oPC.oid), NWScript.GetSkillRank(NWScript.SKILL_SEARCH, oPC.oid), NWScript.GetSkillRank(NWScript.SKILL_SPOT, oPC.oid),
        NWScript.GetSkillRank(NWScript.SKILL_BLUFF, oPC.oid) };

                    int[] iPerceivedHideSkill = { NWScript.GetSkillRank(NWScript.SKILL_BLUFF, oPerceived.oid), NWScript.GetSkillRank(NWScript.SKILL_HIDE, oPerceived.oid),
        NWScript.GetSkillRank(NWScript.SKILL_PERFORM, oPerceived.oid), NWScript.GetSkillRank(NWScript.SKILL_PERSUADE, oPerceived.oid) };

                    Random d20 = NWN.Utils.random;
                    int iRollAttack = iPCSenseSkill.Max() + d20.Next(21);
                    int iRollDefense = iPerceivedHideSkill.Max() + d20.Next(21);

                    if (iRollAttack > iRollDefense)
                    {
                        NWScript.SendMessageToPC(oPC.oid, NWScript.GetName(oPerceived.oid) + " fait usage d'un déguisement ! Sous le masque, vous reconnaissez " + NWScript.GetName(oPerceived.oid, 1));
                        //NWNX_Rename_ClearPCNameOverride(oPerceived, oPC);
                    }
                }
            }
        }

        return 0;
    }*/
  }
}
