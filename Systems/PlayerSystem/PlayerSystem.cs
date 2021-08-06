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
          Learnable spell = new Learnable(LearnableType.Spell, (int)spellId, 0, player);
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
