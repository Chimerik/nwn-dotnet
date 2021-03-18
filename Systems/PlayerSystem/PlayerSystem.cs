using System;
using System.Collections.Generic;
using System.Linq;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.API;
using NWN.Services;
using NWN.API.Events;
using NWNX.Services;
using NWNX.API.Events;
using NWN.API.Constants;
using NLog;

namespace NWN.Systems
{
  [ServiceBinding(typeof(PlayerSystem))]
  public partial class PlayerSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public static CursorTargetService cursorTargetService { get; set; }
    public PlayerSystem(NWNXEventService nwnxEventService, CursorTargetService cursorTService)
    {
      NwModule.Instance.OnClientEnter += HandlePlayerConnect;
      NwModule.Instance.OnClientLeave += HandlePlayerLeave;
      NwModule.Instance.OnPlayerDeath += HandlePlayerDeath;
      NwModule.Instance.OnPlayerLevelUp += CancelPlayerLevelUp;
      nwnxEventService.Subscribe<ServerVaultEvents.OnServerCharacterSaveBefore>(HandleBeforePlayerSave);
      nwnxEventService.Subscribe<ServerVaultEvents.OnServerCharacterSaveAfter>(HandleAfterPlayerSave);
      cursorTargetService = cursorTService;
    }

    public static Dictionary<uint, Player> Players = new Dictionary<uint, Player>();

    [ScriptHandler("spacebar_down")]
    private void HandleSpacebarDown(CallInfo callInfo)
    {
      NWScript.PostString(callInfo.ObjectSelf, "", 40, 15, 0, 0.000001f, unchecked((int)0xFFFFFFFF), unchecked((int)0xFFFFFFFF), 9999, "fnt_my_gui");
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_INPUT_TOGGLE_PAUSE_BEFORE", "spacebar_down", callInfo.ObjectSelf);
    }
    [ScriptHandler("a_start_combat")]
    private void HandleAfterStartCombat(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwPlayer)
      {
        NwPlayer oPC = (NwPlayer)callInfo.ObjectSelf;
        API.Effect effPC = oPC.ActiveEffects.Where(e => e.EffectType == EffectType.CutsceneGhost).FirstOrDefault();
        if (effPC != null)
          oPC.RemoveEffect(effPC);
      }

      NwObject oTarget = NWScript.StringToObject(EventsPlugin.GetEventData("TARGET_OBJECT_ID")).ToNwObject();

      if (oTarget is NwPlayer)
      {
        NwPlayer target = (NwPlayer)oTarget;
        API.Effect effTarget = target.ActiveEffects.Where(e => e.EffectType == EffectType.CutsceneGhost).FirstOrDefault();
        if (effTarget != null)
          target.RemoveEffect(effTarget);
        
        NWScript.SetPCDislike(callInfo.ObjectSelf, oTarget);
      }
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
      if (!(callInfo.ObjectSelf is NwPlayer))
        return;

      NwPlayer oPC = (NwPlayer)callInfo.ObjectSelf;
      int skillID = int.Parse(EventsPlugin.GetEventData("SKILL_ID"));

      switch ((Skill)skillID)
      {
        case Skill.Taunt:
          oPC.GetLocalVariable<int>("_ACTIVATED_TAUNT").Value = 1;
          NWScript.DelayCommand(6.0f, () => oPC.GetLocalVariable<int>("_ACTIVATED_TAUNT").Delete());
          break;
        case Skill.PickPocket:
          EventsPlugin.SkipEvent();
          NwObject oObject = NWScript.StringToObject(EventsPlugin.GetEventData("TARGET_OBJECT_ID")).ToNwObject();

          if(!(oObject is NwPlayer))
          {
            oPC.FloatingTextString("Seuls d'autres joueurs peuvent être ciblés par cette compétence. Les tentatives de vol sur PNJ doivent être jouées en rp avec un dm.".ColorString(Color.RED), false);
            return;
          }

          NwPlayer oTarget = (NwPlayer)oObject;

          if (oTarget.IsDM || oTarget.IsDMPossessed || oTarget.IsPlayerDM)
          {
            oPC.FloatingTextString("Seuls d'autres joueurs peuvent être ciblés par cette compétence. Les tentatives de vol sur PNJ doivent être jouées en rp avec un dm.", false);
            return;
          }

          if (!DateTime.TryParse(ObjectPlugin.GetString(oTarget, $"_PICKPOCKET_TIMER_{oPC.Name}"), out DateTime previousDate)
              || (DateTime.Now - previousDate).TotalHours < 24)
          {
            oPC.FloatingTextString($"Vous ne serez autorisé à faire une nouvelle tentative de vol que dans : {(DateTime.Now - previousDate).TotalHours + 1}", false);
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
            oTarget.FloatingTextString($"{oPC} est en train d'essayer de faire les poches de {oTarget} !", true);
            oPC.FloatingTextString($"{oPC} est en train d'essayer de faire les poches de {oTarget} !", true);
          }

          if (NWScript.TouchAttackMelee(oTarget) == 0)
          {
            oPC.FloatingTextString($"Vous ne parvenez pas à atteindre les poches de {oTarget.Name} !", false);
            return;
          }

          int iStolenGold = (NwRandom.Roll(Utils.random, 20, 1) + oPC.GetSkillRank(Skill.PickPocket) - iSpot) * 10;

          if (oTarget.Gold >= iStolenGold)
          {
            CreaturePlugin.SetGold(oTarget, (int)oTarget.Gold - iStolenGold);
            oPC.GiveGold(iStolenGold);
            oPC.FloatingTextString($"Vous venez de dérober {iStolenGold} pièces d'or des poches de {oTarget.Name} !", false);
          }
          else
          {
            oPC.FloatingTextString($"Vous venez de vider les poches de {oTarget.Name} ! {oTarget.Gold} pièces d'or de plus pour vous.", false);
            oPC.GiveGold((int)oTarget.Gold);
            CreaturePlugin.SetGold(oTarget, 0);
          }

          break;
        case Skill.AnimalEmpathy:
          if (oPC.Area.Tag == "Promenadetest")
          {
            oPC.FloatingTextString("L'endroit est bien trop agité pour que vous puissiez vous permettre de nouer un lien avec l'animal.", false);
            EventsPlugin.SkipEvent();
          }
          break;
      }
    }
    [ScriptHandler("a_detection")]
    private void HandleAfterDetection(CallInfo callInfo)
    {
      if (!(callInfo.ObjectSelf is NwPlayer))
        return;
      NwPlayer oPC = (NwPlayer)callInfo.ObjectSelf;
      NwGameObject oTarget = NWScript.StringToObject(EventsPlugin.GetEventData("TARGET")).ToNwObject<NwGameObject>();

      if (!(oTarget is NwPlayer) && NWScript.GetIsPossessedFamiliar(oTarget) == 0)
        return;

      if (CreaturePlugin.GetMovementType(oTarget) == CreaturePlugin.NWNX_CREATURE_MOVEMENT_TYPE_STATIONARY)
        return;

      if (NWScript.GetIsDM(oTarget) == 1 || NWScript.GetIsDMPossessed(oTarget) == 1 || NWScript.GetIsPlayerDM(oTarget) == 1)
        return;

      if (NWScript.GetObjectSeen(oTarget, oPC) == 1 && oPC.Distance(oTarget) > 20.0f)
        return;

      if (int.Parse(EventsPlugin.GetEventData("TARGET_INVISIBLE")) != 1
          || (!oPC.DetectModeActive && oPC.HasFeatEffect(API.Constants.Feat.KeenSense)))
        return;

      if (!DateTime.TryParse(oPC.GetLocalVariable<string>($"_INVI_DETECT_TIMER_{oTarget.Name}"), out DateTime previousDate)
          || (DateTime.Now - previousDate).TotalSeconds > 6)
      {
        int iMoveSilentlyCheck = NWScript.GetSkillRank(NWScript.SKILL_MOVE_SILENTLY, oTarget) + NwRandom.Roll(NWN.Utils.random, 20) + (int)oPC.Distance(oTarget);
        if (NWScript.GetDetectMode(oPC) == 2)
          iMoveSilentlyCheck += 10;

        if (CreaturePlugin.GetMovementType(oTarget) == CreaturePlugin.NWNX_CREATURE_MOVEMENT_TYPE_RUN
            || oPC.DoSkillCheck(Skill.Listen, iMoveSilentlyCheck))
        {
          oPC.FloatingTextString("Vous entendez quelqu'un se faufiler dans les environs.", false);
          PlayerPlugin.ShowVisualEffect(oPC, NWScript.VFX_FNF_SMOKE_PUFF, oTarget.Position);
          oPC.GetLocalVariable<string>($"_INVI_DETECT_TIMER_{oTarget.Name}").Value = DateTime.Now.ToString();
          oPC.GetLocalVariable<string>($"_INVI_EFFECT_DETECT_TIMER_{oTarget.Name}").Value = DateTime.Now.ToString();
        }
        else
          oPC.GetLocalVariable<string>($"_INVI_DETECT_TIMER_{oTarget.Name}").Delete();
      }
      else if (DateTime.TryParse(oPC.GetLocalVariable<string>($"_INVI_EFFECT_DETECT_TIMER_{oTarget.Name}"), out DateTime previousEffectDate)
          || (DateTime.Now - previousEffectDate).TotalSeconds > 1)
      {
        PlayerPlugin.ShowVisualEffect(oPC, NWScript.VFX_FNF_SMOKE_PUFF, oTarget.Position);
        oPC.GetLocalVariable<string>($"_INVI_EFFECT_DETECT_TIMER_{oTarget.Name}").Value = DateTime.Now.ToString();
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
        int id = NWScript.GetLocalInt(player.oid, "NW_TOTAL_MAP_PINS");
        player.mapPinDictionnary.Add(NWScript.GetLocalInt(player.oid, "NW_TOTAL_MAP_PINS"), new MapPin(id, NWScript.GetTag(NWScript.GetArea(player.oid)), float.Parse(EventsPlugin.GetEventData("PIN_X")), float.Parse(EventsPlugin.GetEventData("PIN_Y")), EventsPlugin.GetEventData("PIN_NOTE")));
      }
    }

    [ScriptHandler("map_pin_changed")]
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
    [ScriptHandler("mappin_destroyed")]
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
    [ScriptHandler("on_input_emote")]
    private void HandleInputEmote(CallInfo callInfo)
    {
      int animation = NWN.Utils.TranslateEngineAnimation(Int32.Parse(EventsPlugin.GetEventData("ANIMATION")));

      switch (animation)
      {
        case NWScript.ANIMATION_LOOPING_MEDITATE:
        case NWScript.ANIMATION_LOOPING_CONJURE1:
        case NWScript.ANIMATION_LOOPING_CONJURE2:
        case NWScript.ANIMATION_LOOPING_DEAD_BACK:
        case NWScript.ANIMATION_LOOPING_GET_MID:
        case NWScript.ANIMATION_LOOPING_GET_LOW:
        case NWScript.ANIMATION_LOOPING_LISTEN:
        case NWScript.ANIMATION_LOOPING_DEAD_FRONT:
        case NWScript.ANIMATION_LOOPING_LOOK_FAR:
        case NWScript.ANIMATION_LOOPING_PAUSE:
        case NWScript.ANIMATION_LOOPING_PAUSE_DRUNK:
        case NWScript.ANIMATION_LOOPING_PAUSE_TIRED:
        case NWScript.ANIMATION_LOOPING_PAUSE2:
        case NWScript.ANIMATION_LOOPING_SIT_CHAIR:
        case NWScript.ANIMATION_LOOPING_SIT_CROSS:
        case NWScript.ANIMATION_LOOPING_SPASM:
        case NWScript.ANIMATION_LOOPING_TALK_FORCEFUL:
        case NWScript.ANIMATION_LOOPING_TALK_LAUGHING:
        case NWScript.ANIMATION_LOOPING_TALK_NORMAL:
        case NWScript.ANIMATION_LOOPING_TALK_PLEADING:
        case NWScript.ANIMATION_LOOPING_WORSHIP:
          EventsPlugin.SkipEvent();
          NWScript.PlayAnimation(animation, 1, 30000.0f);
          break;
      }
    }

    [ScriptHandler("b_learn_scroll")]
    private void HandleBeforeLearnScroll(CallInfo callInfo)
    {
      NwPlayer oPC = (NwPlayer)callInfo.ObjectSelf;

      EventsPlugin.SkipEvent();
      var oScroll = NWScript.StringToObject(EventsPlugin.GetEventData("SCROLL"));
      int spellId = SpellUtils.GetSpellIDFromScroll(oScroll);
      int spellLevel = SpellUtils.GetSpellLevelFromScroll(oScroll);

      if (spellId < 0 || spellLevel < 0)
      {
        Utils.LogMessageToDMs($"LEARN SPELL FROM SCROLL - Player : {oPC.Name}, SpellId : {spellId}, SpellLevel : {spellLevel} - INVALID");
        oPC.SendServerMessage("HRP - Ce parchemin ne semble pas correctement configuré, impossible d'en apprendre quoique ce soit. Le staff a été informé du problème.");
        return;
      }

      int knownSpellCount = CreaturePlugin.GetKnownSpellCount(oPC, 43, spellLevel);

      if (knownSpellCount > 0)
        for (int i = 0; i < knownSpellCount; i++)
          if (CreaturePlugin.GetKnownSpell(oPC, 43, spellLevel, i) == spellId)
          {
            oPC.SendServerMessage("Ce sort est déjà inscrit dans votre grimoire.");
            return;
          }

      if (Players.TryGetValue(oPC, out Player player))
        if (player.learnableSpells.ContainsKey(spellId))
        {
          oPC.SendServerMessage("Ce sort se trouve déjà dans votre liste d'apprentissage.");
          return;
        }
        else
        {
          SkillSystem.LearnableSpell spell = new SkillSystem.LearnableSpell(spellId, 0, player);
          player.learnableSpells.Add(spellId, spell);
          oPC.SendServerMessage($"Le sort {spell.name} a été ajouté à votre liste d'apprentissage et est désormais disponible pour étude.");
          NWScript.DestroyObject(oScroll);
        }
    }

    [ScriptHandler("collect_cancel")]
    private void HandleBeforeCollectCycleCancel(CallInfo callInfo)
    {
      if (Players.TryGetValue(callInfo.ObjectSelf, out Player player))
        player.CancelCollectCycle();
    }

    [ScriptHandler("collect_complete")]
    private void HandleAfterCollectCycleComplete(CallInfo callInfo)
    {
      if (PlayerSystem.Players.TryGetValue(callInfo.ObjectSelf, out Player player))
        player.CompleteCollectCycle();
    }

    private void CancelPlayerLevelUp(ModuleEvents.OnPlayerLevelUp onLevelUp)
    {
      onLevelUp.Player.Xp = 1;
      Utils.LogMessageToDMs($"{onLevelUp.Player.Name} vient d'essayer de level up.");
    }
    [ScriptHandler("on_client_levelup")]
    private void HandleOnClientLevelUp(CallInfo callInfo)
    {
      EventsPlugin.SkipEvent();
      Utils.LogMessageToDMs($"{callInfo.ObjectSelf.Name} vient d'essayer de level up.");
    }

    /*public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
    {
        { "a_detection", HandleAfterDetection },
        { "on_pc_death", HandlePlayerDeath },
    };
    private static void RefreshQBS(uint oidSelf, int feat)
    {
        string sQuickBar = CreaturePlugin.SerializeQuickbar(oidSelf);
        QuickBarSlot emptyQBS = NWN.Utils.CreateEmptyQBS();

        for (int i = 0; i < 36; i++)
        {
            QuickBarSlot qbs = PlayerPlugin.GetQuickBarSlot(oidSelf, i);

            if (qbs.nObjectType == 4 && qbs.nINTParam1 == feat)
            {
                PlayerPlugin.SetQuickBarSlot(oidSelf, i, emptyQBS);
            }
        }

        CreaturePlugin.DeserializeQuickbar(oidSelf, sQuickBar);
    }*/
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
