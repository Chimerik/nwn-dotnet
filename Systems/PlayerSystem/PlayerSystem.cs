﻿using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.Blueprint;
using static NWN.Systems.LootSystem;

namespace NWN.Systems
{
  public static partial class PlayerSystem
  {
    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
        {
            { "on_pc_perceived", HandlePlayerPerceived },
            { "event_before_area_exit", HandleBeforePlayerExitArea },
            { "event_after_area_enter", HandleAfterPlayerEnterArea },
            { "on_pc_target", HandlePlayerTargetSelection },
            { "on_pc_connect", HandlePlayerConnect },
            { "on_pc_disconnect", HandlePlayerDisconnect },
            { "player_exit_before", HandlePlayerBeforeDisconnect },
            { "event_player_save_before", HandleBeforePlayerSave },
            { "event_player_save_after", HandleAfterPlayerSave },
            { "event_dm_possess_before", HandleBeforeDMPossess },
            { "event_dm_spawn_object_after", HandleAfterDMSpawnObject },
            { "event_feat_used", HandleFeatUsed },
            { "event_auto_spell", HandleAutoSpell },
            { "_onspellcast_before", HandleBeforeSpellCast },
            { "_onspellcast_after", HandleAfterSpellCast },
            { "event_combatmode", HandleOnCombatMode },
            { "event_skillused", HandleOnSkillUsed },
            { "summon_add_after", HandleAfterAddSummon },
            { "summon_remove_after", HandleAfterRemoveSummon },
            { "event_detection_after", HandleAfterDetection },
            { "on_pc_death", HandlePlayerDeath },
            { "event_dm_jump_target_after", HandleAfterDMJumpTarget },
            { "event_start_combat_after", HandleAfterStartCombat },
            { "event_party_accept_after", HandleAfterPartyAccept },
            { "event_party_leave_after", HandleAfterPartyLeave },
            { "event_party_leave_before", HandleBeforePartyLeave },
            { "event_party_kick_after", HandleAfterPartyKick },
            { "event_examine_before", HandleBeforeExamine },
            { "event_examine_after", HandleAfterExamine },
            { "event_pccorpse_remove_item_after", HandleAfterItemRemovedFromPCCorpse },
            { "event_inventory_pccorpse_removed_after", HandleAfterPCCorpseRemovedFromInventory },
            { "pc_acquire_item", HandlePCAcquireItem },
            { "pc_unacquire_it", HandlePCUnacquireItem },
            { "event_on_journal_open", HandlePCJournalOpen },
            { "event_on_journal_close", HandlePCJournalClose },
        };
    
    public static Dictionary<uint, Player> Players = new Dictionary<uint, Player>();

    private static int HandlePlayerDisconnect(uint oidSelf)
    {
 /*     var oPC = NWScript.GetExitingObject();
      EventsPlugin.RemoveObjectFromDispatchList(EventsPlugin.ON_INPUT_KEYBOARD_BEFORE, ON_PC_KEYSTROKE_SCRIPT, oPC);
      Players.Remove(oPC);
*/
      return 0;
    }
    private static int HandlePlayerTargetSelection(uint oidSelf)
    {
      var oPC = NWScript.GetLastPlayerToSelectTarget();
      var oTarget = NWScript.GetTargetingModeSelectedObject();
      Vector3 vTarget = NWScript.GetTargetingModeSelectedPosition();

      Player player;
      if (Players.TryGetValue(oPC, out player))
      {
        player.DoActionOnTargetSelected(oTarget, vTarget);
      }

      return 0;
    }
   
    private static int HandleBeforeDMPossess(uint oidSelf)
    {
      var oPossessed = NWScript.StringToObject(EventsPlugin.GetEventData("TARGET"));
      Player oPC;
      if (Players.TryGetValue(oidSelf, out oPC))
      {
        if (NWScript.GetIsObjectValid(oPossessed) == 1)
        { // Ici, on prend possession
          if (NWScript.GetIsDMPossessed(oPC.oid) == 1)
          {
            NWScript.SetLocalObject(NWScript.GetLocalObject(oPC.oid, "_POSSESSER"), "_POSSESSING", oPossessed);
            NWScript.SetLocalObject(oPossessed, "_POSSESSER", NWScript.GetLocalObject(oPC.oid, "_POSSESSER"));
          }
          else
          {
            NWScript.SetLocalObject(oPC.oid, "_POSSESSING", oPossessed);
            NWScript.SetLocalObject(oPossessed, "_POSSESSER", oPC.oid);
          }
        }
        else
        {  // Ici, on cesse la possession
          if (NWScript.GetIsDMPossessed(oPC.oid) == 1)
          {
            NWScript.DeleteLocalObject(NWScript.GetLocalObject(oPC.oid, "_POSSESSER"), "_POSSESSING");
            NWScript.DeleteLocalObject(NWScript.GetLocalObject(oPC.oid, "_POSSESSER"), "_POSSESSER");
          }
          else
          {
            NWScript.DeleteLocalObject(NWScript.GetLocalObject(oPC.oid, "_POSSESSER"), "_POSSESSING");
            NWScript.DeleteLocalObject(oPC.oid, "_POSSESSER");
          }
        }
      }
      return 0;
    }

    private static int HandleAfterDMSpawnObject(uint oidSelf)
    {
      Player oPC;
      if (Players.TryGetValue(oidSelf, out oPC))
      {
        if (int.Parse(EventsPlugin.GetEventData("OBJECT_TYPE")) == NWScript.OBJECT_TYPE_PLACEABLE)
        {
          if (ObjectPlugin.GetInt(oPC.oid, "_SPAWN_PERSIST") != 0)
          {
            var oObject = NWScript.StringToObject(EventsPlugin.GetEventData("OBJECT"));
            NWScript.SetEventScript(oObject, NWScript.EVENT_SCRIPT_PLACEABLE_ON_DEATH, "ondeath_clean_dm_plc");

            var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, "INSERT INTO dm_persistant_placeable(accountID, serializedPlaceable, areaTag, position, facing)" +
              " VALUES(@accountId, @serializedPlaceable, @areaTag, @position, @facing)");
            NWScript.SqlBindInt(query, "@accountId", oPC.accountId);
            NWScript.SqlBindObject(query, "@serializedPlaceable", oObject);
            NWScript.SqlBindString(query, "@areaTag", NWScript.GetTag(NWScript.GetArea(oObject)));
            NWScript.SqlBindVector(query, "@position", NWScript.GetPosition(oObject));
            NWScript.SqlBindFloat(query, "@facing", NWScript.GetFacing(oObject));
            NWScript.SqlStep(query);

            query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"SELECT last_insert_rowid()");
            NWScript.SqlStep(query);
            NWScript.SetLocalInt(oObject, "_ID", NWScript.SqlGetInt(query, 0));

            NWScript.SendMessageToPC(oPC.oid, $"Création persistante - Vous posez le placeable  {NWScript.GetName(oObject)}");
          }
          else
            NWScript.SendMessageToPC(oPC.oid, "Création temporaire - Ce placeable sera effacé par le prochain reboot.");
        }
      }
      return 0;
    }

    private static int HandlePlayerBeforeDisconnect(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        player.isConnected = false;
        player.menu.Close();
        HandleBeforePartyLeave(oidSelf);
        HandleAfterPartyLeave(oidSelf);
      }

      return 0;
    }

    private static int HandleFeatUsed(uint oidSelf)
    {
      Feat feat = (Feat)int.Parse(EventsPlugin.GetEventData("FEAT_ID"));
      Player oPC;

      switch (feat)
      {
        case NWN.Feat.PlayerTool02:
          EventsPlugin.SkipEvent();
          
          if (Players.TryGetValue(oidSelf, out oPC))
          {
            if (Utils.HasTagEffect(oPC.oid, "lycan_curse"))
            {
              Utils.RemoveTaggedEffect(oPC.oid, "lycan_curse");
              oPC.RemoveLycanCurse();
            }
            else
            {
              if ((DateTime.Now - oPC.lycanCurseTimer).TotalSeconds > 10800)
              {
                oPC.ApplyLycanCurse();
                oPC.lycanCurseTimer = DateTime.Now;
              }
              else
                NWScript.SendMessageToPC(oPC.oid, "Vous ne vous sentez pas encore la force de changer de nouveau de forme.");
            }
          }
          break;

        case Feat.LanguageElf:
        case Feat.LanguageAbyssal:
        case Feat.LanguageCelestial:
        case Feat.LanguageDeep:
        case Feat.LanguageDraconic:
        case Feat.LanguageDruidic:
        case Feat.LanguageDwarf:
        case Feat.LanguageGiant:
        case Feat.LanguageGoblin:
        case Feat.LanguageHalfling:
        case Feat.LanguageInfernal:
        case Feat.LanguageOrc:
        case Feat.LanguagePrimodial:
        case Feat.LanguageSylvan:
        case Feat.LanguageThieves:
          if (Players.TryGetValue(oidSelf, out oPC))
          {
            if (oPC.activeLanguage == feat)
            {
              oPC.activeLanguage = Feat.Invalid;
              NWScript.SendMessageToPC(oidSelf, "Vous vous exprimez désormais en commun.");
              NWScript.SetTextureOverride("icon_elf", "", oidSelf); // TODO : chopper l'icône correspondate dynamiquement via feat.2da

              RefreshQBS(oidSelf, (int)feat);
            }
            else
            {
              oPC.activeLanguage = feat; ;
              NWScript.SendMessageToPC(oidSelf, $"Vous vous exprimez désormais en {Languages.GetLanguageName(oPC.activeLanguage)}.");
              NWScript.SetTextureOverride("icon_elf", "icon_elf_active", oidSelf);

              RefreshQBS(oidSelf, (int)feat);
            }
          }

          break;

        case Feat.BlueprintCopy:
        case Feat.BlueprintCopy2:
        case Feat.BlueprintCopy3:
        case Feat.BlueprintCopy4:
        case Feat.BlueprintCopy5:
        case Feat.Research:
        case Feat.Research2:
        case Feat.Research3:
        case Feat.Research4:
        case Feat.Research5:
        case Feat.Metallurgy:
        case Feat.Metallurgy2:
        case Feat.Metallurgy3:
        case Feat.Metallurgy4:
        case Feat.Metallurgy5:

          EventsPlugin.SkipEvent();
          var oTarget = NWScript.StringToObject(EventsPlugin.GetEventData("TARGET_OBJECT_ID"));
          BlueprintValidation(oidSelf, oTarget, feat);
          break;

        case Feat.CustomMenuUP:
        case Feat.CustomMenuDOWN:
        case Feat.CustomMenuSELECT:
        case Feat.CustomMenuEXIT:
          EventsPlugin.SkipEvent();

          if (Players.TryGetValue(oidSelf, out oPC))
            oPC.EmitKeydown(new Player.MenuFeatEventArgs(feat));
          break;
      }
          
      return 0;
    }
    private static void RefreshQBS(uint oidSelf, int feat)
    {
      string sQuickBar = CreaturePlugin.SerializeQuickbar(oidSelf);
      QuickBarSlot emptyQBS = Utils.CreateEmptyQBS();
       
      for (int i = 0; i < 36; i++)
      {
        QuickBarSlot qbs = PlayerPlugin.GetQuickBarSlot(oidSelf, i);
        
        if (qbs.nObjectType == 4 && qbs.nINTParam1 == feat)
        {
          PlayerPlugin.SetQuickBarSlot(oidSelf, i, emptyQBS);
        }
      }

      CreaturePlugin.DeserializeQuickbar(oidSelf, sQuickBar);
    }

    private static void RefreshQBS(uint oidSelf)
    {
      string sQuickBar = CreaturePlugin.SerializeQuickbar(oidSelf);
      QuickBarSlot emptyQBS = Utils.CreateEmptyQBS();

      for (int i = 0; i < 36; i++)
      {
        QuickBarSlot qbs = PlayerPlugin.GetQuickBarSlot(oidSelf, i);
        PlayerPlugin.SetQuickBarSlot(oidSelf, i, emptyQBS);
      }

      CreaturePlugin.DeserializeQuickbar(oidSelf, sQuickBar);
    }

    private static int HandleAutoSpell(uint oidSelf) //Je garde ça sous la main, mais je pense que le gérer différement serait mieux, notamment en créant un mode activable "autospell" en don gratuit pour les casters. Donc : A RETRAVAILLER 
    {
      Player oPC;

      if (Players.TryGetValue(oidSelf, out oPC))
      {
        var oTarget = NWScript.StringToObject(EventsPlugin.GetEventData("TARGET"));

        if (NWScript.GetIsObjectValid(oTarget) == 1)
        {
          NWScript.ClearAllActions();
          if (oPC.autoAttackTarget == NWScript.OBJECT_INVALID)
          {
            NWScript.AssignCommand(oidSelf, () => NWScript.ActionCastSpellAtObject(NWScript.SPELL_RAY_OF_FROST, oTarget));
            NWScript.DelayCommand(6.0f, () => oPC.OnFrostAutoAttackTimedEvent());
          }
        }

        oPC.autoAttackTarget = oTarget;
      }

      return 0;
    }

    private static int HandleBeforeSpellCast(uint oidSelf)
    {
      Player oPC;

      if (Players.TryGetValue(oidSelf, out oPC))
      {
        var spellId = int.Parse(EventsPlugin.GetEventData("SPELL_ID"));

        //if (spellId != NWScript.SPELL_RAY_OF_FROST)
          //oPC.autoAttackTarget = NWScript.OBJECT_INVALID;

        NWScript.SendMessageToPC(oidSelf, "before spell cast");
        //CreaturePlugin.SetClassByPosition(oidSelf, 0, NWScript.CLASS_TYPE_WIZARD);
      }

      return 0;
    }
    private static int HandleAfterSpellCast(uint oidSelf)
    {
      Player oPC;

      if (Players.TryGetValue(oidSelf, out oPC))
      {
        var spellId = int.Parse(EventsPlugin.GetEventData("SPELL_ID"));
        NWScript.SendMessageToPC(oidSelf, "after spell cast");
        //CreaturePlugin.SetClassByPosition(oidSelf, 0, 43); // 43 = aventurier
      }

      return 0;
    }

    private static void FrostAutoAttack(uint oClicker, uint oTarget)
    {
      if (NWScript.GetLocalInt(oClicker, "_FROST_ATTACK_CANCEL") == 0)
      {
        NWScript.AssignCommand(oClicker, () => NWScript.ActionAttack(oTarget));
      }
      else
      {
        NWScript.DeleteLocalInt(oClicker, "_FROST_ATTACK_CANCEL");
        NWScript.DeleteLocalObject(oClicker, "_FROST_ATTACK_TARGET");
      }
    }
    private static int HandlePlayerPerceived(uint oidSelf)
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

            Random d20 = Utils.random;
            int iRollAttack = iPCSenseSkill.Max() + d20.Next(21);
            int iRollDefense = iPerceivedHideSkill.Max() + d20.Next(21);

            /*  if (GetLocalInt(GetModule(), "_DEBUG_DISGUISE"))
              {
                NWNX_Chat_SendMessage(NWNX_CHAT_CHANNEL_PLAYER_DM, SEARTH + "Jet pour percer le déguisement : " + STOPAZE + IntToString(iPCSkill) + SEARTH + " + " + STOPAZE + IntToString(iRollAttack - iPCSkill) + SEARTH + " = " + SRED + IntToString(iRollAttack) + ".", oPC);
                NWNX_Chat_SendMessage(NWNX_CHAT_CHANNEL_PLAYER_DM, SEARTH + "Jet de déguisement : " + STOPAZE + IntToString(iPerceivedHideSkill) + SEARTH + " + " + STOPAZE + IntToString(iRollDefense - iPerceivedHideSkill) + SEARTH + " = " + SRED + IntToString(iRollDefense) + ".", oPerceived);
              }*/
            if (iRollAttack > iRollDefense)
            {
              NWScript.SendMessageToPC(oPC.oid, NWScript.GetName(oPerceived.oid) + " fait usage d'un déguisement ! Sous le masque, vous reconnaissez " + NWScript.GetName(oPerceived.oid, 1));
              //NWNX_Rename_ClearPCNameOverride(oPerceived, oPC);
            }
          }
        }
      }

      return 0;
    }
    private static int HandleAfterAddSummon(uint oidSelf)
    {
      //Pas méga utile dans l'immédiat, mais pourra être utilisé pour gérer les invocations de façon plus fine plus tard
      // TODO : Système de possession d'invocations, compagnons animaux, etc (mais attention, vérifier que si le PJ déco en possession, ça n'écrase pas son .bic. Si oui, sauvegarde le PJ avant possession et ne plus sauvegarder le PJ en mode possession)
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        var oSummon = (NWScript.StringToObject(EventsPlugin.GetEventData("ASSOCIATE_OBJECT_ID")));

        if (NWScript.GetIsObjectValid(oSummon) == 1)
        {
          player.summons.Add(oSummon, oSummon);
        }
      }

      return 0;
    }
    private static int HandleAfterRemoveSummon(uint oidSelf)
    {
      //Pas méga utile dans l'immédiat, mais pourra être utilisé pour gérer les invocations de façon plus fine plus tard
      // TODO : Système de possession d'invocations, compagnons animaux, etc (mais attention, vérifier que si le PJ déco en possession, ça n'écrase pas son .bic. Si oui, sauvegarde le PJ avant possession et ne plus sauvegarder le PJ en mode possession)
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        var oSummon = (NWScript.StringToObject(EventsPlugin.GetEventData("ASSOCIATE_OBJECT_ID")));

        if (NWScript.GetIsObjectValid(oSummon) == 1)
        {
          player.summons.Remove(oSummon);
        }
      }

      return 0;
    }
    private static int HandleOnCombatMode(uint oidSelf)
    { 
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
          if (NWScript.GetLocalInt(player.oid, "_ACTIVATED_TAUNT") != 0) // Permet de conserver sa posture de combat après avoir utilisé taunt
          {
            EventsPlugin.SkipEvent();
            NWScript.DeleteLocalInt(player.oid, "_ACTIVATED_TAUNT");
          }
      }

      return 0;
    }
    private static int HandleOnSkillUsed(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
          if (int.Parse(EventsPlugin.GetEventData("SKILL_ID")) == NWScript.SKILL_TAUNT)
          {
            NWScript.SetLocalInt(player.oid, "_ACTIVATED_TAUNT", 1);
            NWScript.DelayCommand(12.0f, () => NWScript.DeleteLocalInt(player.oid, "_ACTIVATED_TAUNT"));
          }
          else if (int.Parse(EventsPlugin.GetEventData("SKILL_ID")) == NWScript.SKILL_PICK_POCKET)
          {
            EventsPlugin.SkipEvent();
            var oObject = NWScript.StringToObject(EventsPlugin.GetEventData("TARGET_OBJECT_ID"));
            Player oTarget;
            if (Players.TryGetValue(oObject, out oTarget) && NWScript.GetIsDM(oTarget.oid) != 1 && NWScript.GetIsDMPossessed(oTarget.oid) != 1)
            {
              if (!oTarget.pickpocketDetectTimer.ContainsKey(player.oid) || (DateTime.Now - oTarget.pickpocketDetectTimer[player.oid]).TotalSeconds > 86400)
              {
                  oTarget.pickpocketDetectTimer.Add(player.oid, DateTime.Now);
              
                  FeedbackPlugin.SetFeedbackMessageHidden(13, 1, oTarget.oid); // 13 = COMBAT_TOUCH_ATTACK
                  NWScript.DelayCommand(2.0f, () => FeedbackPlugin.SetFeedbackMessageHidden(13, 0, oTarget.oid));

                  int iRandom = Utils.random.Next(21);
                  int iVol = NWScript.GetSkillRank(NWScript.SKILL_PICK_POCKET, player.oid);
                  int iSpot = Utils.random.Next(21) + NWScript.GetSkillRank(NWScript.SKILL_SPOT, player.oid);
                  if ((iRandom + iVol) > iSpot)
                  {
                    ChatPlugin.SendMessage((int)ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, $"Vous faites un jet de Vol à la tire, le résultat est de : {iRandom} + {iVol} = {iRandom + iVol}.", player.oid, player.oid);
                    if (NWScript.TouchAttackMelee(oTarget.oid) > 0)
                    {
                      int iStolenGold = (iRandom + iVol - iSpot) * 10;
                      int oTargetGold = NWScript.GetGold(oTarget.oid);
                      if (oTargetGold >= iStolenGold)
                      {
                        CreaturePlugin.SetGold(oTarget.oid, oTargetGold - iStolenGold);
                        CreaturePlugin.SetGold(player.oid, NWScript.GetGold(player.oid) + iStolenGold);
                        NWScript.FloatingTextStringOnCreature($"Vous venez de dérober {iStolenGold} pièces d'or des poches de {NWScript.GetName(oTarget.oid)} !", player.oid);
                      }
                      else
                      {
                        NWScript.FloatingTextStringOnCreature($"Vous venez de vider les poches de {NWScript.GetName(oTarget.oid)} ! {oTargetGold} pièces d'or de plus pour vous.", player.oid);
                        CreaturePlugin.SetGold(player.oid, NWScript.GetGold(player.oid) + oTargetGold);
                        CreaturePlugin.SetGold(oTarget.oid, 0);
                      }
                    }
                    else
                    {
                      NWScript.FloatingTextStringOnCreature($"Vous ne parvenez pas à atteindre les poches de {NWScript.GetName(oTarget.oid)} !", player.oid);
                    }
                  }
                  else
                NWScript.FloatingTextStringOnCreature($"{NWScript.GetName(player.oid)} est en train d'essayer de vous faire les poches !", oTarget.oid); 
              }
              else
                NWScript.FloatingTextStringOnCreature("Vous n'êtes pas autorisé à faire une nouvelle tentative pour le moment.", player.oid);
            }
            else
              NWScript.FloatingTextStringOnCreature("Seuls d'autres joueurs peuvent être ciblés par cette compétence. Les tentatives de vol sur PNJ doivent être jouées en rp avec un dm.", player.oid);
          }
      }

      return 0;
    }
    private static int HandleAfterDetection(uint oidSelf)
    {
      Player oPC;
      if (Players.TryGetValue(oidSelf, out oPC))
      {
        var oTarget = NWScript.StringToObject(EventsPlugin.GetEventData("TARGET"));

        if (NWScript.GetIsPC(oTarget) == 1 || NWScript.GetIsPossessedFamiliar(oTarget) == 1)
        {
          if (NWScript.GetObjectSeen(oTarget, oPC.oid) != 1 && NWScript.GetDistanceBetween(oTarget, oPC.oid) < 20.0f)
          {
            int iDetectMode = (int)NWScript.GetDetectMode(oPC.oid);
            if (int.Parse(EventsPlugin.GetEventData("TARGET_INVISIBLE")) == 1 && iDetectMode > 0)
            {
              switch (CreaturePlugin.GetMovementType(oTarget))
              {
                case CreaturePlugin.NWNX_CREATURE_MOVEMENT_TYPE_WALK_BACKWARDS:
                case CreaturePlugin.NWNX_CREATURE_MOVEMENT_TYPE_SIDESTEP:
                case CreaturePlugin.NWNX_CREATURE_MOVEMENT_TYPE_WALK:

                  if (!oPC.inviDetectTimer.ContainsKey(oTarget) || (DateTime.Now - oPC.inviDetectTimer[oTarget]).TotalSeconds > 6)
                  {
                    int iMoveSilentlyCheck = NWScript.GetSkillRank(NWScript.SKILL_MOVE_SILENTLY, oTarget) + Utils.random.Next(21) + (int)NWScript.GetDistanceBetween(oTarget, oPC.oid);
                    int iListenCheck = NWScript.GetSkillRank(NWScript.SKILL_LISTEN, oPC.oid) + Utils.random.Next(21);
                    if (iDetectMode == 2)
                      iListenCheck -= 10;

                    if (iListenCheck > iMoveSilentlyCheck)
                    {
                      NWScript.SendMessageToPC(oPC.oid, "Vous entendez quelqu'un se faufiler dans les environs.");
                      PlayerPlugin.ShowVisualEffect(oPC.oid, NWScript.VFX_FNF_SMOKE_PUFF, NWScript.GetPosition(oTarget));
                      oPC.inviDetectTimer.Add(oTarget, DateTime.Now);
                      oPC.inviEffectDetectTimer.Add(oTarget, DateTime.Now);
                    }
                    else
                      oPC.inviDetectTimer.Remove(oTarget);
                  }
                  else if ((DateTime.Now - oPC.inviEffectDetectTimer[oTarget]).TotalSeconds > 1)
                  {
                    PlayerPlugin.ShowVisualEffect(oPC.oid, NWScript.VFX_FNF_SMOKE_PUFF, NWScript.GetPosition(oTarget));
                    oPC.inviEffectDetectTimer.Add(oTarget, DateTime.Now);
                  }
                  break;
                case CreaturePlugin.NWNX_CREATURE_MOVEMENT_TYPE_RUN:

                  if (!oPC.inviDetectTimer.ContainsKey(oTarget) || (DateTime.Now - oPC.inviDetectTimer[oTarget]).TotalSeconds > 6)
                  {
                    NWScript.SendMessageToPC(oPC.oid, "Vous entendez quelqu'un courir peu discrètement dans les environs.");
                    PlayerPlugin.ShowVisualEffect(oPC.oid, NWScript.VFX_FNF_SMOKE_PUFF, NWScript.GetPosition(oTarget));
                    oPC.inviDetectTimer.Add(oTarget, DateTime.Now);
                    oPC.inviEffectDetectTimer.Add(oTarget, DateTime.Now);
                  }
                  else if ((DateTime.Now - oPC.inviEffectDetectTimer[oTarget]).TotalSeconds > 1)
                  {
                    PlayerPlugin.ShowVisualEffect(oPC.oid, NWScript.VFX_FNF_SMOKE_PUFF, NWScript.GetPosition(oTarget));
                    oPC.inviEffectDetectTimer.Add(oTarget, DateTime.Now);
                  }
                  break;
              }
            }
          }
        }
      }

      return 0;
    }

    private static int HandleAfterDMJumpTarget(uint oidSelf)
    {
      var oTarget = NWScript.StringToObject(EventsPlugin.GetEventData("TARGET_1"));

      Player player;
      if (Players.TryGetValue(oTarget, out player))
      {
        if(NWScript.GetTag(NWScript.GetArea(player.oid)) == "Labrume")
        {
          DestroyPlayerCorpse(player);
        }
      }

      return 0;
    }
    public static PlayerSystem.Player GetPCById(int PcId)
    {
      foreach (KeyValuePair<uint, PlayerSystem.Player> PlayerListEntry in PlayerSystem.Players)
      {
        if(PlayerListEntry.Value.characterId == PcId)
        {
          return PlayerListEntry.Value;
        }
      }

        return null;
    }
    private static int HandleAfterStartCombat(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        var oTarget = NWScript.StringToObject(EventsPlugin.GetEventData("TARGET_OBJECT_ID"));

        if (Spells.GetHasEffect(NWScript.GetEffectType(NWScript.EffectCutsceneGhost()), player.oid))
          Spells.RemoveEffectOfType(NWScript.GetEffectType(NWScript.EffectCutsceneGhost()), player.oid);          

        if (NWScript.GetIsPC(oTarget) == 1)
        {
          NWScript.SetPCDislike(player.oid, oTarget);
          if (Spells.GetHasEffect(NWScript.GetEffectType(NWScript.EffectCutsceneGhost()), oTarget))
            Spells.RemoveEffectOfType(NWScript.GetEffectType(NWScript.EffectCutsceneGhost()), oTarget);
        }
      }

      return 0;
    }
    private static int HandleAfterPartyAccept(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        Effect eParty = player.GetPartySizeEffect();

        // appliquer l'effet sur chaque membre du groupe
        var oPartyMember = NWScript.GetFirstFactionMember(oidSelf, 1);
        while (NWScript.GetIsObjectValid(oPartyMember) == 1)
        {
          Utils.RemoveTaggedEffect(oPartyMember, "PartyEffect");
          if (eParty != null)
            NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_PERMANENT, eParty, oPartyMember);

          oPartyMember = NWScript.GetNextFactionMember(oPartyMember, 1);
        }
      }   

      return 0;
    }
    private static int HandleAfterPartyLeave(uint oidSelf)
    {
      Utils.RemoveTaggedEffect(oidSelf, "PartyEffect");
      return 0;
    }
    private static int HandleAfterPartyKick(uint oidSelf)
    {
      Utils.RemoveTaggedEffect(NWScript.StringToObject(EventsPlugin.GetEventData("KICKED")), "PartyEffect");
      return 0;
    }
    private static int HandleBeforePartyLeave(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        Effect eParty = player.GetPartySizeEffect(-1);
        // appliquer l'effet sur chaque membre du groupe
        var oPartyMember = NWScript.GetFirstFactionMember(oidSelf, 1);
        while (NWScript.GetIsObjectValid(oPartyMember) == 1)
        {
          Utils.RemoveTaggedEffect(oPartyMember, "PartyEffect");
          if (eParty != null)
            NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_PERMANENT, eParty, oPartyMember);

          oPartyMember = NWScript.GetNextFactionMember(oPartyMember, 1);
        }
      }

      return 0;
    }
    private static int HandleBeforeExamine(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        var examineTarget =  NWScript.StringToObject(EventsPlugin.GetEventData("EXAMINEE_OBJECT_ID"));
      
        switch(NWScript.GetTag(examineTarget))
        {
          case "mineable_rock":
            int oreAmount = NWScript.GetLocalInt(examineTarget, "_ORE_AMOUNT");
            if (NWScript.GetIsDM(player.oid) != 1)
            {
              int geologySkillLevel;
              if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, 1171)), out geologySkillLevel))
                NWScript.SetDescription(examineTarget, $"Minerai disponible : {Utils.random.Next(oreAmount * geologySkillLevel * 20 / 100, 2 * oreAmount - geologySkillLevel * 20 / 100)}");
              else
                NWScript.SetDescription(examineTarget, $"Minerai disponible estimé : {Utils.random.Next(0, 2 * oreAmount)}");
            }
            else
              NWScript.SetDescription(examineTarget, $"Minerai disponible : {oreAmount}");

              break;
          case "blueprint":
            int baseItemType = NWScript.GetLocalInt(examineTarget, "_BASE_ITEM_TYPE");
            NWScript.SendMessageToPC(oidSelf, $"id : {baseItemType}");
            if (CollectSystem.blueprintDictionnary.ContainsKey(baseItemType))
              NWScript.SetDescription(examineTarget, CollectSystem.blueprintDictionnary[baseItemType].DisplayBlueprintInfo(player, examineTarget));
            else
            {
              NWScript.SendMessageToPC(oidSelf, "[ERREUR HRP] - Le patron utilisé n'est pas correctement initialisé. Le bug a été remonté au staff.");
              Utils.LogMessageToDMs($"Blueprint Invalid : {NWScript.GetName(examineTarget)} - Base Item Type : {baseItemType} - Examined by : {NWScript.GetName(oidSelf)}");
            }
              break;
        }
      }
      return 0;
    }
    private static int HandleAfterExamine(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        var examineTarget = NWScript.StringToObject(EventsPlugin.GetEventData("EXAMINEE_OBJECT_ID"));

        switch (NWScript.GetTag(examineTarget))
        {
          case "mineable_rock":
              NWScript.SetDescription(examineTarget, $"");
            break;
          case "blueprint":
            NWScript.SetDescription(examineTarget, $"");
            break;
        }
      }
      return 0;
    }
    private static int HandlePCUnacquireItem(uint oidSelf)
    {
      uint oPC = NWScript.GetModuleItemLostBy();
      
      if (NWScript.GetMovementRate(oPC) == CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE)
        if (NWScript.GetWeight(oPC) <= int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", NWScript.GetAbilityScore(oPC, NWScript.ABILITY_STRENGTH))))
          CreaturePlugin.SetMovementRate(oPC, CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_PC);

      return 0;
    }
    private static int HandlePCAcquireItem(uint oidSelf)
    {
      var oPC = NWScript.GetModuleItemAcquiredBy();
      var oItem = NWScript.GetModuleItemAcquired();

      if(NWScript.GetTag(oItem) == "item_pccorpse")
      {
        var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"COUNT (*) FROM playerDeathCorpses WHERE characterId = @characterId");
        NWScript.SqlBindInt(query, "@characterId", NWScript.GetLocalInt(oItem, "_PC_ID"));
        if (NWScript.SqlStep(query) < 1)
          NWScript.DestroyObject(oItem);
      }

      if (NWScript.GetMovementRate(oPC) != CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE)
        if (NWScript.GetWeight(oPC) > int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", NWScript.GetAbilityScore(oPC, NWScript.ABILITY_STRENGTH))))
          CreaturePlugin.SetMovementRate(oPC, CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE);

      return 0;
    }
    private static int HandleAfterPlayerEnterArea(uint oidSelf)
    {
      if(!Convert.ToBoolean(NWScript.GetIsDM(oidSelf)) && !Convert.ToBoolean(NWScript.GetIsDMPossessed(oidSelf)))
      {
        var oArea = NWScript.StringToObject(EventsPlugin.GetEventData("AREA"));
        NWScript.SetLocalString(oArea, "_LAST_ENTERED_ON", DateTime.Now.ToString());

        //GESTION DES SPAWNS & RENCONTRES
        DateTime previousSpawnDate;
        if (!DateTime.TryParse(NWScript.GetLocalString(oArea, "_DATE_LAST_SPAWNED"), out previousSpawnDate) || (DateTime.Now - previousSpawnDate).TotalMinutes > 10)
        {
          NWScript.SetLocalString(oArea, "_DATE_LAST_SPAWNED", DateTime.Now.ToString());

          var firstObject = NWScript.GetFirstObjectInArea(oArea);
          if(NWScript.GetTag(firstObject) == "creature_spawn")
            NWScript.CreateObject(NWScript.OBJECT_TYPE_CREATURE, NWScript.GetLocalString(firstObject, "_CREATURE_TEMPLATE"), NWScript.GetLocation(firstObject));

          int i = 1;
          var spawnPoints = NWScript.GetNearestObjectByTag("creature_spawn", firstObject);

          while (Convert.ToBoolean(NWScript.GetIsObjectValid(spawnPoints)))
          {
            if (Convert.ToBoolean(NWScript.GetLocalInt(spawnPoints, "_SPAWN_BLOCKED")))
              continue;

            if (Convert.ToBoolean(NWScript.GetLocalInt(spawnPoints, "_PNJ_SPAWN")))
              NWScript.SetLocalInt(spawnPoints, "_SPAWN_BLOCKED", 1);

              NWScript.SetEventScript(NWScript.CreateObject(
              NWScript.OBJECT_TYPE_CREATURE, NWScript.GetLocalString(spawnPoints, "_CREATURE_TEMPLATE"), NWScript.GetLocation(spawnPoints)),
              NWScript.EVENT_SCRIPT_CREATURE_ON_DEATH, ON_LOOT_SCRIPT);
            i++;
            spawnPoints = NWScript.GetNearestObjectByTag("creature_spawn", firstObject, i);
          }

          // Gestion des coffres lootables
          Area area;
          if (Module.areaDictionnary.TryGetValue(NWScript.GetObjectUUID(oArea), out area))
          {
            foreach (uint chest in area.lootChestList)
            {
              Utils.DestroyInventory(chest);

              Lootable.Config lootableConfig;
              var containerTag = NWScript.GetTag(chest);

              if (!lootablesDic.TryGetValue(containerTag, out lootableConfig))
              {
                Utils.LogMessageToDMs($"AREA - {NWScript.GetName(oArea)} - Unregistered container tag=\"{containerTag}\", name : {NWScript.GetName(chest)}");
                continue;
              }

              NWScript.AssignCommand(oArea, () => NWScript.DelayCommand(
                  0.1f,
                  () => lootableConfig.GenerateLoot(chest)
              ));
            }
          }
        }

        //EN FONCTION DE SI LA ZONE EST REST OU PAS, ON AFFICHE LA PROGRESSION DU JOURNAL DE CRAFT
        Player player;
        if (Players.TryGetValue(oidSelf, out player))
        {
          if (NWScript.GetLocalString(oArea, "_DATE_LAST_SPAWNED") == "_REST")
          {
            if (player.craftJob.isActive && player.playerJournal.craftJobCountDown == null)
              player.craftJob.CreateCraftJournalEntry();
          }
          else if (player.playerJournal.craftJobCountDown != null)
            player.craftJob.CancelCraftJournalEntry();

          player.previousArea = oArea;

          if(player.menu.isOpen)
            player.menu.Close();

          Area area;
          if (Module.areaDictionnary.TryGetValue(NWScript.GetObjectUUID(oArea), out area))
            area.DoAreaSpecificBehavior(player);
        }
      }

      return 0;
    }
    private static int HandleBeforePlayerExitArea(uint oidSelf)
    {
      if (!Convert.ToBoolean(NWScript.GetIsDM(oidSelf)) && !Convert.ToBoolean(NWScript.GetIsDMPossessed(oidSelf)))
      {
        Player player;
        if (Players.TryGetValue(oidSelf, out player))
        { 
          Area area;
          if (Module.areaDictionnary.TryGetValue(NWScript.GetObjectUUID(player.previousArea), out area))
          {
            if (AreaPlugin.GetNumberOfPlayersInArea(player.previousArea) == 0)
              NWScript.DelayCommand(1500.0f, () => area.CleanArea()); // 25 minutes
          }
        }
      }

      return 0;
    }
    private static int HandlePCJournalOpen(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        player.DoJournalUpdate = true;
        player.UpdateJournal();
      }

      return 0;
    }
    private static int HandlePCJournalClose(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        player.DoJournalUpdate = false;
      }

      return 0;
    }
    private static int HandleDialogStart(uint oidSelf)
    {
      /*Player player;
      if (Players.TryGetValue(NWScript.GetLastSpeaker(), out player))
      {
        DialogPlugin.SetCurrentNodeText("Hello, mon beau gloubi ! ");
      }*/

      switch (DialogPlugin.GetCurrentNodeType())
      {
        case DialogPlugin.NWNX_DIALOG_NODE_TYPE_STARTING_NODE: // INTRO DU DIALOGUE
          DialogPlugin.SetCurrentNodeText("Bienvenue chez TRUANT & TRUAN associés ! "); 
          break;
        case DialogPlugin.NWNX_DIALOG_NODE_TYPE_ENTRY_NODE: // PNJ QUI PARLE, HORS INTRO
          DialogPlugin.SetCurrentNodeText("Ceci est une entry node ! "); 
          break;
        case DialogPlugin.NWNX_DIALOG_NODE_TYPE_REPLY_NODE: // REPONSE PJ
          DialogPlugin.SetCurrentNodeText("Ceci est une REPLY node ! ");
          break;
      }

      /*string sMessage = "NWNX_Dialog debug:";
      int id = DialogPlugin.GetCurrentNodeID();
      sMessage = $"\nNode ID = {id}";

      int type = DialogPlugin.GetCurrentNodeType();
      sMessage += $"\nCurrent node type = {type} (";
      switch (type)
      {
        case DialogPlugin.NWNX_DIALOG_NODE_TYPE_INVALID: sMessage += "INVALID)"; break;
        case DialogPlugin.NWNX_DIALOG_NODE_TYPE_STARTING_NODE: sMessage += "STARTING_NODE)"; break;
        case DialogPlugin.NWNX_DIALOG_NODE_TYPE_ENTRY_NODE: sMessage += "ENTRY_NODE)"; break;
        case DialogPlugin.NWNX_DIALOG_NODE_TYPE_REPLY_NODE: sMessage += "REPLY_NODE)"; break;
      }

      int scripttype = DialogPlugin.GetCurrentScriptType();
      sMessage += $"\nScript type = {scripttype} (";
      switch (scripttype)
      {
        case DialogPlugin.NWNX_DIALOG_SCRIPT_TYPE_OTHER: sMessage += "OTHER)"; break;
        case DialogPlugin.NWNX_DIALOG_SCRIPT_TYPE_STARTING_CONDITIONAL: sMessage += "STARTING_CONDITIONAL)"; break;
        case DialogPlugin.NWNX_DIALOG_SCRIPT_TYPE_ACTION_TAKEN: sMessage += "ACTION_TAKEN)"; break;
      }

      int index = DialogPlugin.GetCurrentNodeIndex();
      sMessage += $"\nNode index = {index}";

      string text = DialogPlugin.GetCurrentNodeText();
      sMessage += $"\nText = '{text}'";

      DialogPlugin.SetCurrentNodeText(text + " [ADDED]");

      NWScript.SendMessageToPC(NWScript.GetFirstPC(), sMessage);
      */
      return 1;
    }
  }
}
