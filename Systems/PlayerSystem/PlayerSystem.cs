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
    public static CursorTargetService cursorTargetService { get; set; }
    public static EventService eventService { get; set; }
    public PlayerSystem(CursorTargetService cursorTService, EventService eventServices)
    {
      NwModule.Instance.OnClientEnter += HandlePlayerConnect;
      NwModule.Instance.OnClientDisconnect += HandlePlayerLeave;

      eventService = eventServices;
      cursorTargetService = cursorTService;
    }

    public static Dictionary<uint, Player> Players = new Dictionary<uint, Player>();

    [ScriptHandler("spacebar_down")]
    private void HandleSpacebarDown(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature { IsPlayerControlled: true } oCreature)
      {
        oCreature.ControllingPlayer.PostString("", 40, 15, 0, 0.000001f, ColorConstants.White, ColorConstants.White, 9999, "fnt_my_gui");
        EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_INPUT_TOGGLE_PAUSE_BEFORE", "spacebar_down", callInfo.ObjectSelf);
      }
    }
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
    [ScriptHandler("event_skillused")]
    private void HandleBeforeSkillUsed(CallInfo callInfo)
    {
      if (!(callInfo.ObjectSelf is NwCreature { IsLoginPlayerCharacter: true } oPC))
        return;

      int skillID = int.Parse(EventsPlugin.GetEventData("SKILL_ID"));

      switch ((Skill)skillID)
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
          EventsPlugin.SkipEvent();

          if (!(NWScript.StringToObject(EventsPlugin.GetEventData("TARGET_OBJECT_ID")).ToNwObject() is NwCreature { IsLoginPlayerCharacter: true } oTarget)
            || oTarget.ControllingPlayer.IsDM)
          {
            oPC.ControllingPlayer.FloatingTextString("Seuls d'autres joueurs peuvent être ciblés par cette compétence. Les tentatives de vol sur PNJ doivent être jouées en rp avec un dm.".ColorString(ColorConstants.Red), false);
            return;
          }

          if (!DateTime.TryParse(oTarget.GetObjectVariable<PersistentVariableString>($"_PICKPOCKET_TIMER_{oPC.Name}").Value, out DateTime previousDate)
              || (DateTime.Now - previousDate).TotalHours < 24)
          {
            oPC.ControllingPlayer.FloatingTextString($"Vous ne serez autorisé à faire une nouvelle tentative de vol que dans : {(DateTime.Now - previousDate).TotalHours + 1}", false);
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
      }
    }

    [ScriptHandler("mappin_destroyed")]
    private void HandleDestroyMapPin(CallInfo callInfo)
    {
      if (Players.TryGetValue(callInfo.ObjectSelf, out Player player))
      {
        int pinId = int.Parse(EventsPlugin.GetEventData("PIN_ID"));
        player.mapPinDictionnary.Remove(pinId);

        SqLiteUtils.DeletionQuery("playerMapPins",
         new Dictionary<string, string>() { { "characterId", player.characterId.ToString() }, { "mapPinId", pinId.ToString() } });
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
      }
    }
    [ScriptHandler("pc_sheet_open")]
    private void HandleCharacterSheetOpened(CallInfo callInfo)
    {
      if (!callInfo.ObjectSelf.IsLoginPlayerCharacter(out NwPlayer player) || !player.IsDM)
        return;

      if (!(NWScript.StringToObject(EventsPlugin.GetEventData("TARGET")).ToNwObject() is NwCreature { IsLoginPlayerCharacter: true } oTarget)
        || !Players.TryGetValue(oTarget, out Player target))
        return;

      foreach (KeyValuePair<Feat, int> feat in target.learntCustomFeats)
      {
        CustomFeat customFeat = SkillSystem.customFeatsDictionnary[feat.Key];
        FeatTable.Entry entry = Feat2da.featTable.GetFeatDataEntry(feat.Key);

        player.SetTlkOverride((int)entry.tlkName, $"{customFeat.name} - {SkillSystem.GetCustomFeatLevelFromSkillPoints(feat.Key, feat.Value)}");
        player.SetTlkOverride((int)entry.tlkDescription, customFeat.description);
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

    [ScriptHandler("b_learn_scroll")]
    private void HandleBeforeLearnScroll(CallInfo callInfo)
    {
      if (!(callInfo.ObjectSelf is NwCreature { IsLoginPlayerCharacter: true } oPC))
        return;

      EventsPlugin.SkipEvent();
      NwItem oScroll = NWScript.StringToObject(EventsPlugin.GetEventData("SCROLL")).ToNwObject<NwItem>();
      Spell spellId = SpellUtils.GetSpellIDFromScroll(oScroll);
      byte spellLevel = SpellUtils.GetSpellLevelFromScroll(oScroll);

      if (spellId < 0 || spellLevel > 10)
      {
        Utils.LogMessageToDMs($"LEARN SPELL FROM SCROLL - Player : {oPC.Name}, SpellId : {spellId}, SpellLevel : {spellLevel} - INVALID");
        oPC.ControllingPlayer.SendServerMessage("HRP - Ce parchemin ne semble pas correctement configuré, impossible d'en apprendre quoique ce soit. Le staff a été informé du problème.", ColorConstants.Red);
        return;
      }

      if (oPC.GetClassInfo((ClassType)43).GetKnownSpells(spellLevel).Any(s => s == spellId))
      {
        oPC.ControllingPlayer.SendServerMessage("Ce sort est déjà inscrit dans votre grimoire.");
        return;
      }

      if (Players.TryGetValue(oPC, out Player player))
        if (player.learnables.ContainsKey($"S{spellId}"))
        {
          Learnable learnable = player.learnables[$"S{spellId}"];

          if (learnable.nbScrollsUsed <= 5)
          {
            learnable.acquiredPoints += learnable.pointsToNextLevel / 20;
            learnable.nbScrollsUsed += 1;
            oPC.ControllingPlayer.SendServerMessage($"A l'aide de ce parchemin, vous affinez votre connaissance du sort de {learnable.name.ColorString(ColorConstants.White)}. Votre apprentissage sera plus rapide.", new Color(32, 255, 32));
          }
          else
            oPC.ControllingPlayer.SendServerMessage("Vous avez déjà retiré tout ce que vous pouviez de ce parchemin.", ColorConstants.Orange);
          return;
        }
        else
        {
          Learnable spell = new Learnable(LearnableType.Spell, (int)spellId, 0).InitializeLearnableLevel(player);
          player.learnables.Add($"S{(int)spellId}", spell);
          oPC.ControllingPlayer.SendServerMessage($"Le sort {spell.name} a été ajouté à votre liste d'apprentissage et est désormais disponible pour étude.");
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

    private static void HandlePlayerPerception(CreatureEvents.OnPerception onPerception)
    {
      if (onPerception.PerceptionEventType != PerceptionEventType.Seen || onPerception.PerceivedCreature.Tag != "Statuereptilienne"
        || !onPerception.Creature.IsLoginPlayerCharacter)
        return;

      if (onPerception.PerceivedCreature.GetObjectVariable<LocalVariableInt>($"_PERCEPTION_STATUS_{onPerception.Creature.ControllingPlayer.CDKey}").HasValue)
        return;

      onPerception.PerceivedCreature.GetObjectVariable<LocalVariableInt>($"_PERCEPTION_STATUS_{onPerception.Creature.ControllingPlayer.CDKey}").Value = 1;

      Effect effectToRemove = onPerception.PerceivedCreature.ActiveEffects.FirstOrDefault(e => e.Tag == "_FREEZE_EFFECT");
      if (effectToRemove != null)
        onPerception.PerceivedCreature.RemoveEffect(effectToRemove);

      Task waitForAnimation = NwTask.Run(async () =>
      {
        await onPerception.PerceivedCreature.PlayAnimation((Animation)Utils.random.Next(100, 116), 6);
        await NwTask.Delay(TimeSpan.FromSeconds(0.1));

        Effect eff = eff = Effect.VisualEffect(VfxType.DurFreezeAnimation);
        eff.Tag = "_FREEZE_EFFECT";
        eff.SubType = EffectSubType.Supernatural;
        onPerception.PerceivedCreature.ApplyEffect(EffectDuration.Permanent, eff);
      });
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

    [ScriptHandler("on_gui_event")]
    private void HandleGuiEvent(CallInfo callInfo)
    {
      NwCreature oPC = NWScript.GetLastGuiEventPlayer().ToNwObject<NwCreature>();

      switch(NWScript.GetLastGuiEventType())
      {
        case NWScript.GUIEVENT_CHATBAR_FOCUS:

          if (NWScript.GetLastGuiEventInteger() > 3)
            return;

          Effect visualMark = Effect.VisualEffect((VfxType)1248);
          visualMark.Tag = "VFX_SPEAKING_MARK";
          visualMark.SubType = EffectSubType.Supernatural;
          oPC.ApplyEffect(EffectDuration.Permanent, visualMark);

          break;

        case NWScript.GUIEVENT_CHATBAR_UNFOCUS:

          foreach (Effect eff in oPC.ActiveEffects.Where(e => e.Tag == "VFX_SPEAKING_MARK"))
            oPC.RemoveEffect(eff);

          break;

        case NWScript.GUIEVENT_EFFECTICON_CLICK:

          EffectType nIconEffectType = EffectIconToEffectType(NWScript.GetLastGuiEventInteger());

          if (nIconEffectType == EffectType.InvalidEffect)
            return;

          foreach(Effect eff in oPC.ActiveEffects.Where(e => e.EffectType == nIconEffectType))
          {
            SpellsTable.Entry entry = Spells2da.spellsTable.GetSpellDataEntry(eff.Spell);

            float percentageRemaining = eff.DurationRemaining / eff.TotalDuration;
            Color color = ColorConstants.White;

            if (percentageRemaining > 0.5)
              color = new Color(32, 255, 32);
            else if (percentageRemaining < 0.25)
              color = ColorConstants.Maroon;
            else
              color = ColorConstants.Lime;

            string durationRemaining = eff.DurationType == EffectDuration.Permanent ? "Permanent".ColorString(new Color(32, 255, 32)) : eff.DurationRemaining.ToString().ColorString(color);
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
            }


            oPC.ControllingPlayer.SendServerMessage($"{entry.name.ColorString(ColorConstants.White)}\n\n" +
              $"{durationRemaining}", ColorConstants.Orange);
          }

          break;
      }
    }
    string GetModifierType(EffectType nEffectType, EffectType nPlus, EffectType nMinus)
    {
      return nEffectType == nPlus ? "+" : nEffectType == nMinus ? "-" : "";
    }
    string ACTypeToString(int nACType)
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

    string SavingThrowToString(int nSavingThrow)
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
    string SavingThrowTypeToString(int nSavingThrowType)
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
    string GetVersusRacialTypeAndAlignment(int nRacialType, int nLawfulChaotic, int nGoodEvil)
    {
      string sRacialType = nRacialType == NWScript.RACIAL_TYPE_INVALID ? "" : NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("racialtypes", "NamePlural", nRacialType)));
      string sLawfulChaotic = nLawfulChaotic == NWScript.ALIGNMENT_LAWFUL ? "Lawful" : nLawfulChaotic == NWScript.ALIGNMENT_CHAOTIC ? "Chaotic" : "";
      string sGoodEvil = nGoodEvil == NWScript.ALIGNMENT_GOOD ? "Good" : nGoodEvil == NWScript.ALIGNMENT_EVIL ? "Evil" : "";
      string sAlignment = sLawfulChaotic + (sLawfulChaotic == "" ? sGoodEvil : (sGoodEvil == "" ? "" : " " + sGoodEvil));
      return (sRacialType != "" || sAlignment != "") ? (" vs. " + sAlignment + (sAlignment == "" ? sRacialType : (sRacialType == "" ? "" : " " + sRacialType))) : "";
    }
    EffectType EffectIconToEffectType(int nEffectIcon)
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
