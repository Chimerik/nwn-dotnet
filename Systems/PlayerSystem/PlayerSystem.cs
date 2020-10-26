using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class PlayerSystem
  {
    public const string ON_PC_KEYSTROKE_SCRIPT = "on_pc_keystroke";

    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
        {
            { "on_pc_perceived", HandlePlayerPerceived },
            { "on_pc_target", HandlePlayerTargetSelection },
            { "on_pc_connect", HandlePlayerConnect },
            { "on_pc_disconnect", HandlePlayerDisconnect },
            { "player_exit_before", HandlePlayerBeforeDisconnect },
            { ON_PC_KEYSTROKE_SCRIPT, HandlePlayerKeystroke },
            { "event_player_save_before", HandleBeforePlayerSave },
            { "event_player_save_after", HandleAfterPlayerSave },
            { "event_dm_possess_before", HandleBeforeDMPossess },
            { "event_dm_spawn_object_after", HandleAfterDMSpawnObject },
            { "event_mv_plc", HandleMovePlaceable },
            { "event_feat_used", HandleFeatUsed },
            { "event_auto_spell", HandleAutoSpell },
            { "_onspellcast", HandleOnSpellCast },
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

    private static int HandlePlayerKeystroke(uint oidSelf)
    {
      var key = EventsPlugin.GetEventData("KEY");
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        player.EmitKeydown(new Player.KeydownEventArgs(key));
      }

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
        if (int.Parse(EventsPlugin.GetEventData("OBJECT_TYPE")) == 9)
        {
          if (ObjectPlugin.GetInt(oPC.oid, "_SPAWN_PERSIST") != 0)
          {
            var oObject = NWScript.StringToObject(EventsPlugin.GetEventData("OBJECT"));
            // TODO : Enregistrer l'objet créé en base de données. Ajouter à l'objet un script qui le supprime de la BDD OnDeath
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
        HandleBeforePartyLeave(oidSelf);
        HandleAfterPartyLeave(oidSelf);
      }

      return 0;
    }

    private static int HandleMovePlaceable(uint oidSelf)
    {
      string current_event = EventsPlugin.GetCurrentEvent();

      string sKey = EventsPlugin.GetEventData("KEY");
      var oMeuble = NWScript.GetLocalObject(oidSelf, "_MOVING_PLC");
      Vector3 vPos = NWScript.GetPosition(oMeuble);

      if (sKey == "W")
        ObjectPlugin.AddToArea(oMeuble, NWScript.GetArea(oMeuble), NWScript.Vector(vPos.X, vPos.Y + 0.1f, vPos.Z));
      else if (sKey == "S")
        ObjectPlugin.AddToArea(oMeuble, NWScript.GetArea(oMeuble), NWScript.Vector(vPos.X, vPos.Y - 0.1f, vPos.Z));
      else if (sKey == "D")
        ObjectPlugin.AddToArea(oMeuble, NWScript.GetArea(oMeuble), NWScript.Vector(vPos.X + 0.1f, vPos.Y, vPos.Z));
      else if (sKey == "A")
        ObjectPlugin.AddToArea(oMeuble, NWScript.GetArea(oMeuble), NWScript.Vector(vPos.X - 0.1f, vPos.Y, vPos.Z));
      else if (sKey == "Q")
        NWScript.AssignCommand(oMeuble, () => NWScript.SetFacing(NWScript.GetFacing(oMeuble) - 20.0f));
      //NWScript.AssignCommand(oMeuble, () => oMeuble.Facing (oMeuble.Facing - 20.0f));
      else if (sKey == "E")
        NWScript.AssignCommand(oMeuble, () => NWScript.SetFacing(NWScript.GetFacing(oMeuble) + 20.0f));
      //NWScript.AssignCommand(oMeuble, () => NWScript.SetFacing(oMeuble.Facing + 20.0f));

      return 0;
    }

    private static int HandleFeatUsed(uint oidSelf)
    {
      string current_event = EventsPlugin.GetCurrentEvent();
      var feat = int.Parse(EventsPlugin.GetEventData("FEAT_ID"));

      if (current_event == "NWNX_ON_USE_FEAT_BEFORE")
      {
        if (feat == (int)NWN.Feat.PlayerTool02)
        {
          EventsPlugin.SkipEvent();
          Player oPC;
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

          return 0;
        }
        else if (feat == (int)Feat.LanguageElf)
        {
          Player oPC;
          if (Players.TryGetValue(oidSelf, out oPC))
          {
            NWScript.SendMessageToPC(oidSelf, $"langue = {oPC.activeLanguage}");
            if (oPC.activeLanguage == Feat.LanguageElf)
            {
              oPC.activeLanguage = Feat.Invalid;
              NWScript.SendMessageToPC(oidSelf, "Vous cessez de parler elfe.");
              NWScript.SetTextureOverride("icon_elf", "", oidSelf);

              RefreshQBS(oidSelf, feat);
            }
            else
            {
              oPC.activeLanguage = Feat.LanguageElf; ;
              NWScript.SendMessageToPC(oidSelf, "Vous parlez désormais elfe.");
              NWScript.SetTextureOverride("icon_elf", "icon_elf_active", oidSelf);

              RefreshQBS(oidSelf, feat);
            }
          }
        
          return 0;
        }
        else if (feat == NWScript.FEAT_PLAYER_TOOL_01) // TODO : Refaire le système. Probablement mieux en changeant totalement l'apparence du PJ par l'objet, le laisser se positionner lui-même, puis sauvegarder l'emplacement
        {
          EventsPlugin.SkipEvent();
          var oTarget = NWScript.StringToObject(EventsPlugin.GetEventData("TARGET_OBJECT_ID"));
          Player myPlayer;
          if (Players.TryGetValue(oidSelf, out myPlayer))
          {
            if (NWScript.GetIsObjectValid(oTarget) == 1)
            {
              /*Utils.Meuble result;
              if (Enum.TryParse(oTarget.Tag, out result))
              {
                EventsPlugin.AddObjectToDispatchList("NWNX_ON_INPUT_KEYBOARD_AFTER", "event_mv_plc", oidSelf);
                oidSelf.AsObject().Locals.Object.Set("_MOVING_PLC", oTarget);
                oidSelf.AsPlayer().SendMessage($"Vous venez de sélectionner {NWScript.GetName(oTarget.oid)}, utilisez votre barre de raccourcis pour le déplacer. Pour enregistrer le nouvel emplacement et retrouver votre barre de raccourcis habituelle, activez le don sur un endroit vide (sans cible).");
                //remplacer la ligne précédente par un PostString().

                if (myPlayer.selectedObjectsList.Count == 0)
                {
                  myPlayer.BoulderBlock();
                }

                if (!myPlayer.selectedObjectsList.Contains(oTarget))
                  myPlayer.selectedObjectsList.Add(oTarget);
              }
              else
              {
                oidSelf.AsPlayer().SendMessage("Vous ne pouvez pas manier cet élément.");
              }*/
            }
            else
            {
              string sObjectSaved = "";

              foreach (uint selectedObject in myPlayer.selectedObjectsList)
              {
                var sql = $"UPDATE sql_meubles SET objectLocation = @loc WHERE objectUUID = @uuid";

                using (var connection = MySQL.GetConnection()) // TODO : à refaire, on ne va plus utiliser MySQL
                {
                  connection.Execute(sql, new { uuid = NWScript.GetObjectUUID(selectedObject), loc = Utils.LocationToString(NWScript.GetLocation(selectedObject)) }); // TODO : à refaire, il ne faut pas utiliser UUID entre différents reboot de serveur, mais plutôt un id incrémenté en BDD
                }

                sObjectSaved += NWScript.GetName(selectedObject) + "\n";
              }

              NWScript.SendMessageToPC(myPlayer.oid, $"Vous venez de sauvegarder le positionnement des meubles : \n{sObjectSaved}");
              myPlayer.BoulderUnblock();

              EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_INPUT_KEYBOARD_AFTER", "event_mv_plc", oidSelf);
              NWScript.DeleteLocalObject(oidSelf, "_MOVING_PLC");
              myPlayer.selectedObjectsList.Clear();
            }
          }
          return 0;
        }
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

    private static int HandleOnSpellCast(uint oidSelf)
    {
      Player oPC;

      if (Players.TryGetValue(oidSelf, out oPC))
      {
        var spellId = int.Parse(EventsPlugin.GetEventData("SPELL_ID"));

        if (spellId != NWScript.SPELL_RAY_OF_FROST)
          oPC.autoAttackTarget = NWScript.OBJECT_INVALID;
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
              if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, 1171)), out geologySkillLevel)) // TODO : feat enum geology
                NWScript.SetDescription(examineTarget, $"Minerai disponible : {Utils.random.Next(oreAmount * geologySkillLevel * 20 / 100, 2 * oreAmount - geologySkillLevel * 20 / 100)}");
              else
                NWScript.SetDescription(examineTarget, $"Minerai disponible estimé : {Utils.random.Next(0, 2 * oreAmount)}");
            }
            else
              NWScript.SetDescription(examineTarget, $"Minerai disponible : {oreAmount}");

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
        var query = NWScript.SqlPrepareQueryCampaign("AoaDatabase", $"COUNT (*) FROM playerDeathCorpses WHERE characterId = @characterId");
        NWScript.SqlBindInt(query, "@characterId", NWScript.GetLocalInt(oItem, "_PC_ID"));
        if (NWScript.SqlStep(query) < 1)
          NWScript.DestroyObject(oItem);
      }

      if (NWScript.GetMovementRate(oPC) != CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE)
        if (NWScript.GetWeight(oPC) > int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", NWScript.GetAbilityScore(oPC, NWScript.ABILITY_STRENGTH))))
          CreaturePlugin.SetMovementRate(oPC, CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE);

      return 0;
    }
  }
}
