using System;
using System.Collections.Generic;
using System.Linq;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.API;
using NWN.Services;
using NWN.API.Events;
using NWN.API.Constants;
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
      NWScript.PostString(callInfo.ObjectSelf, "", 40, 15, 0, 0.000001f, unchecked((int)0xFFFFFFFF), unchecked((int)0xFFFFFFFF), 9999, "fnt_my_gui");
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_INPUT_TOGGLE_PAUSE_BEFORE", "spacebar_down", callInfo.ObjectSelf);
    }
    public static void OnCombatStarted(OnCombatStatusChange onCombatStatusChange)
    {

      if (onCombatStatusChange.CombatStatus == CombatStatus.ExitCombat)
        return;

      API.Effect effPC = onCombatStatusChange.Player.ControlledCreature.ActiveEffects.FirstOrDefault(e => e.EffectType == EffectType.CutsceneGhost);
      if (effPC != null)
        onCombatStatusChange.Player.ControlledCreature.RemoveEffect(effPC);
    }
    public static void OnCombatRoundStart(OnCombatRoundStart onStartCombatRound)
    {
      NWScript.SetPCDislike(onStartCombatRound.Creature, onStartCombatRound.Target);
    }
    [ScriptHandler("event_combatmode")]
    private void HandleCombatModeOff(CallInfo callInfo)
    {
      if (NWScript.GetLocalInt(callInfo.ObjectSelf, "_ACTIVATED_TAUNT") != 0) // Permet de conserver sa posture de combat après avoir utilisé taunt
      {
        EventsPlugin.SkipEvent();
        NWScript.DeleteLocalInt(callInfo.ObjectSelf, "_ACTIVATED_TAUNT");
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
          oPC.GetLocalVariable<int>("_ACTIVATED_TAUNT").Value = 1;
          NWScript.DelayCommand(6.0f, () => oPC.GetLocalVariable<int>("_ACTIVATED_TAUNT").Delete());
          break;
        case Skill.PickPocket:
          EventsPlugin.SkipEvent();

          if (!(NWScript.StringToObject(EventsPlugin.GetEventData("TARGET_OBJECT_ID")).ToNwObject() is NwCreature { IsLoginPlayerCharacter: true } oTarget)
            || oTarget.ControllingPlayer.IsDM)
          {
            oPC.ControllingPlayer.FloatingTextString("Seuls d'autres joueurs peuvent être ciblés par cette compétence. Les tentatives de vol sur PNJ doivent être jouées en rp avec un dm.".ColorString(ColorConstants.Red), false);
            return;
          }

          if (!DateTime.TryParse(ObjectPlugin.GetString(oTarget, $"_PICKPOCKET_TIMER_{oPC.Name}"), out DateTime previousDate)
              || (DateTime.Now - previousDate).TotalHours < 24)
          {
            oPC.ControllingPlayer.FloatingTextString($"Vous ne serez autorisé à faire une nouvelle tentative de vol que dans : {(DateTime.Now - previousDate).TotalHours + 1}", false);
            return;
          }

          ObjectPlugin.SetString(oTarget, $"_PICKPOCKET_TIMER_{oPC.Name}", DateTime.Now.ToString(), 1);

          FeedbackPlugin.SetFeedbackMessageHidden(13, 1, oTarget); // 13 = COMBAT_TOUCH_ATTACK
          NWScript.DelayCommand(2.0f, () => FeedbackPlugin.SetFeedbackMessageHidden(13, 0, oTarget));

          int iSpot = oTarget.GetSkillRank(Skill.Spot);
          if (oTarget.DetectModeActive || oTarget.HasFeatEffect(API.Constants.Feat.KeenSense))
            iSpot += NwRandom.Roll(Utils.random, 20, 1);

          if (!oPC.DoSkillCheck(Skill.PickPocket, iSpot))
          {
            oTarget.ControllingPlayer.FloatingTextString($"{oPC} est en train d'essayer de faire les poches de {oTarget} !", true);
            oPC.ControllingPlayer.FloatingTextString($"{oPC} est en train d'essayer de faire les poches de {oTarget} !", true);
          }

          if (NWScript.TouchAttackMelee(oTarget) == 0)
          {
            oPC.ControllingPlayer.FloatingTextString($"Vous ne parvenez pas à atteindre les poches de {oTarget.Name} !", false);
            return;
          }

          int iStolenGold = (NwRandom.Roll(Utils.random, 20, 1) + oPC.GetSkillRank(Skill.PickPocket) - iSpot) * 10;

          if (oTarget.Gold >= iStolenGold)
          {
            CreaturePlugin.SetGold(oTarget, (int)oTarget.Gold - iStolenGold);
            oPC.GiveGold(iStolenGold);
            oPC.ControllingPlayer.FloatingTextString($"Vous venez de dérober {iStolenGold} pièces d'or des poches de {oTarget.Name} !", false);
          }
          else
          {
            oPC.ControllingPlayer.FloatingTextString($"Vous venez de vider les poches de {oTarget.Name} ! {oTarget.Gold} pièces d'or de plus pour vous.", false);
            oPC.GiveGold((int)oTarget.Gold);
            CreaturePlugin.SetGold(oTarget, 0);
          }

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
        int id = NWScript.GetLocalInt(player.oid.LoginCreature, "NW_TOTAL_MAP_PINS");
        player.mapPinDictionnary.Add(NWScript.GetLocalInt(player.oid.LoginCreature, "NW_TOTAL_MAP_PINS"), new MapPin(id, NWScript.GetTag(NWScript.GetArea(player.oid.ControlledCreature)), float.Parse(EventsPlugin.GetEventData("PIN_X")), float.Parse(EventsPlugin.GetEventData("PIN_Y")), EventsPlugin.GetEventData("PIN_NOTE")));
      }
    }

    [ScriptHandler("mappin_destroyed")]
    private void HandleDestroyMapPin(CallInfo callInfo)
    {
      if (Players.TryGetValue(callInfo.ObjectSelf, out Player player))
      {
        int pinId = Int32.Parse(EventsPlugin.GetEventData("PIN_ID"));
        player.mapPinDictionnary.Remove(pinId);

        var query = NWScript.SqlPrepareQueryCampaign(Systems.Config.database, "DELETE FROM playerMapPins where characterId = @characterId and mapPinId = @mapPinId");
        NWScript.SqlBindInt(query, "@characterId", player.characterId);
        NWScript.SqlBindInt(query, "@mapPinId", pinId);
        NWScript.SqlStep(query);
      }
    }
    [ScriptHandler("map_pin_changed")] 
    private void HandleChangeMapPin(CallInfo callInfo)
    {
      if (Players.TryGetValue(callInfo.ObjectSelf, out Player player))
      {
        MapPin updatedMapPin = player.mapPinDictionnary[Int32.Parse(EventsPlugin.GetEventData("PIN_ID"))];
        updatedMapPin.x = float.Parse(EventsPlugin.GetEventData("PIN_X"));
        updatedMapPin.y = float.Parse(EventsPlugin.GetEventData("PIN_Y"));
        updatedMapPin.note = EventsPlugin.GetEventData("PIN_NOTE");
      }
    }
    [ScriptHandler("pc_sheet_open")]
    private void HandleCharacterSheetOpened(CallInfo callInfo)
    {
      if (!(callInfo.ObjectSelf is NwCreature { IsLoginPlayerCharacter: true } player) || !player.ControllingPlayer.IsDM)
        return;

      if (!(NWScript.StringToObject(EventsPlugin.GetEventData("TARGET")).ToNwObject() is NwCreature { IsLoginPlayerCharacter: true } oTarget)
        || !Players.TryGetValue(oTarget, out Player target))
        return;

      foreach (KeyValuePair<Feat, int> feat in target.learntCustomFeats)
      {
        CustomFeat customFeat = SkillSystem.customFeatsDictionnary[feat.Key];

        if (int.TryParse(NWScript.Get2DAString("feat", "FEAT", (int)feat.Key), out int nameValue))
          PlayerPlugin.SetTlkOverride(callInfo.ObjectSelf, nameValue, $"{customFeat.name} - {SkillSystem.GetCustomFeatLevelFromSkillPoints(feat.Key, feat.Value)}");
        else
          Utils.LogMessageToDMs($"CUSTOM SKILL SYSTEM ERROR - Skill {customFeat.name} : no available custom name StrRef");

        if (int.TryParse(NWScript.Get2DAString("feat", "DESCRIPTION", (int)feat.Key), out int descriptionValue))
          PlayerPlugin.SetTlkOverride(callInfo.ObjectSelf, descriptionValue, customFeat.description);
        else
        {
          Utils.LogMessageToDMs($"CUSTOM SKILL SYSTEM ERROR - Skill {customFeat.name} : no available custom description StrRef");
        }
      }
    }
    [ScriptHandler("on_input_emote")]
    private void HandleInputEmote(CallInfo callInfo)
    {
      if (!Players.TryGetValue(callInfo.ObjectSelf, out Player player))
        return;

      int animation = Utils.TranslateEngineAnimation(Int32.Parse(EventsPlugin.GetEventData("ANIMATION")));

      switch (animation)
      {
        case NWScript.ANIMATION_LOOPING_MEDITATE:
        case NWScript.ANIMATION_LOOPING_CONJURE1:
        case NWScript.ANIMATION_LOOPING_CONJURE2:
        case NWScript.ANIMATION_LOOPING_GET_MID:
        case NWScript.ANIMATION_LOOPING_GET_LOW:
        case NWScript.ANIMATION_LOOPING_LISTEN:
        case NWScript.ANIMATION_LOOPING_LOOK_FAR:
        case NWScript.ANIMATION_LOOPING_PAUSE:
        case NWScript.ANIMATION_LOOPING_PAUSE_DRUNK:
        case NWScript.ANIMATION_LOOPING_PAUSE_TIRED:
        case NWScript.ANIMATION_LOOPING_PAUSE2:
        case NWScript.ANIMATION_LOOPING_SPASM:
        case NWScript.ANIMATION_LOOPING_TALK_FORCEFUL:
        case NWScript.ANIMATION_LOOPING_TALK_LAUGHING:
        case NWScript.ANIMATION_LOOPING_TALK_NORMAL:
        case NWScript.ANIMATION_LOOPING_TALK_PLEADING:
        case NWScript.ANIMATION_LOOPING_WORSHIP:
          EventsPlugin.SkipEvent();
          NWScript.PlayAnimation(animation, 1, 30000.0f);
          break;
        case NWScript.ANIMATION_LOOPING_DEAD_BACK:
        case NWScript.ANIMATION_LOOPING_DEAD_FRONT:
        case NWScript.ANIMATION_LOOPING_SIT_CHAIR:
        case NWScript.ANIMATION_LOOPING_SIT_CROSS:
          EventsPlugin.SkipEvent();
          NWScript.PlayAnimation(animation, 1, 30000.0f);

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
      var oScroll = NWScript.StringToObject(EventsPlugin.GetEventData("SCROLL"));
      int spellId = SpellUtils.GetSpellIDFromScroll(oScroll);
      int spellLevel = SpellUtils.GetSpellLevelFromScroll(oScroll);

      if (spellId < 0 || spellLevel < 0)
      {
        Utils.LogMessageToDMs($"LEARN SPELL FROM SCROLL - Player : {oPC.Name}, SpellId : {spellId}, SpellLevel : {spellLevel} - INVALID");
        oPC.ControllingPlayer.SendServerMessage("HRP - Ce parchemin ne semble pas correctement configuré, impossible d'en apprendre quoique ce soit. Le staff a été informé du problème.");
        return;
      }

      int knownSpellCount = CreaturePlugin.GetKnownSpellCount(oPC, 43, spellLevel);

      if (knownSpellCount > 0)
        for (int i = 0; i < knownSpellCount; i++)
          if (CreaturePlugin.GetKnownSpell(oPC, 43, spellLevel, i) == spellId)
          {
            oPC.ControllingPlayer.SendServerMessage("Ce sort est déjà inscrit dans votre grimoire.");
            return;
          }

      if (Players.TryGetValue(oPC, out Player player))
        if (player.learnableSpells.ContainsKey(spellId))
        {
          if (player.learnableSpells[spellId].nbScrollsUsed <= 5)
          {
            player.learnableSpells[spellId].acquiredPoints += player.learnableSpells[spellId].pointsToNextLevel / 20;
            player.learnableSpells[spellId].nbScrollsUsed += 1;
            oPC.ControllingPlayer.SendServerMessage("A l'aide de ce parchemin, vous affinez votre connaissance de ce sort. Votre apprentissage sera plus rapide.");
          }
          else
            oPC.ControllingPlayer.SendServerMessage("Vous avez déjà retiré tout ce que vous pouviez de ce parchemin.");
          return;
        }
        else
        {
          SkillSystem.LearnableSpell spell = new SkillSystem.LearnableSpell(spellId, 0, player);
          player.learnableSpells.Add(spellId, spell);
          oPC.ControllingPlayer.SendServerMessage($"Le sort {spell.name} a été ajouté à votre liste d'apprentissage et est désormais disponible pour étude.");
          NWScript.DestroyObject(oScroll);
        }
    }

    [ScriptHandler("collect_cancel")]
    private void HandleBeforeCollectCycleCancel(CallInfo callInfo)
    {
      callInfo.ObjectSelf.GetLocalVariable<int>("_COLLECT_CANCELLED").Value = 1;
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

      if (onPerception.PerceivedCreature.GetLocalVariable<int>($"_PERCEPTION_STATUS_{onPerception.Creature.ControllingPlayer.CDKey}").HasValue)
        return;

      onPerception.PerceivedCreature.GetLocalVariable<int>($"_PERCEPTION_STATUS_{onPerception.Creature.ControllingPlayer.CDKey}").Value = 1;

      API.Effect effectToRemove = onPerception.PerceivedCreature.ActiveEffects.FirstOrDefault(e => e.Tag == "_FREEZE_EFFECT");
      if (effectToRemove != null)
        onPerception.PerceivedCreature.RemoveEffect(effectToRemove);

      Task waitForAnimation = NwTask.Run(async () =>
      {
        onPerception.PerceivedCreature.PlayAnimation((Animation)Utils.random.Next(100, 116), 6);
        await NwTask.Delay(TimeSpan.FromSeconds(0.1));

        API.Effect eff = eff = API.Effect.VisualEffect(VfxType.DurFreezeAnimation);
        eff.Tag = "_FREEZE_EFFECT";
        eff.SubType = EffectSubType.Supernatural;
        onPerception.PerceivedCreature.ApplyEffect(EffectDuration.Permanent, eff);
      });
    }
    public static void HandleCombatRoundEndForAutoSpells(CreatureEvents.OnCombatRoundEnd onCombatRoundEnd)
    {
      if(onCombatRoundEnd.Creature.GetLocalVariable<int>("_AUTO_SPELL").HasNothing)
      {
        onCombatRoundEnd.Creature.OnCombatRoundEnd -= HandleCombatRoundEndForAutoSpells;
        return;
      }

      int spellId = onCombatRoundEnd.Creature.GetLocalVariable<int>("_AUTO_SPELL").Value;
      NwObject target = onCombatRoundEnd.Creature.GetLocalVariable<NwObject>("_AUTO_SPELL_TARGET").Value;

      if(target != null && target.IsValid)
        onCombatRoundEnd.Creature.ActionCastSpellAt((Spell)spellId, (NwGameObject)target);
      else
      {
        onCombatRoundEnd.Creature.GetLocalVariable<int>("_AUTO_SPELL").Delete();
        onCombatRoundEnd.Creature.GetLocalVariable<NwObject>("_AUTO_SPELL_TARGET").Delete();
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
