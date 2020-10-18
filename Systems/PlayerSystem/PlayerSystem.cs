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
            { "pc_acquire_item", HandlePCAcquireItem },
            { "pc_unacquire_it", HandlePCUnacquireItem },
        };
    
    public static Dictionary<uint, Player> Players = new Dictionary<uint, Player>();

    private static int HandlePlayerConnect(uint oidSelf)
    {
      var oPC = NWScript.GetEnteringObject();

      //TODO : système de BANLIST

      EventsPlugin.AddObjectToDispatchList("NWNX_ON_INPUT_KEYBOARD_BEFORE", ON_PC_KEYSTROKE_SCRIPT, oPC);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_USE_FEAT_BEFORE", "event_feat_used", oPC);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_ADD_ASSOCIATE_AFTER", "summon_add_after", oPC);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_REMOVE_ASSOCIATE_AFTER", "summon_remove_after", oPC);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_BROADCAST_CAST_SPELL_AFTER", "event_spellbroadcast_after", oPC);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_ITEM_EQUIP_BEFORE", "event_equip_items_before", oPC);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_ITEM_UNEQUIP_BEFORE", "event_unequip_items_before", oPC);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_VALIDATE_ITEM_EQUIP_BEFORE", "event_validate_equip_items_before", oPC);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_VALIDATE_USE_ITEM_BEFORE", "event_validate_equip_items_before", oPC);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_COMBAT_MODE_OFF", "event_combatmode", oPC);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_USE_SKILL_BEFORE", "event_skillused", oPC);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_DO_LISTEN_DETECTION_AFTER", "event_detection_after", oPC);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_START_COMBAT_ROUND_AFTER", "event_start_combat_after", oPC);

      //oPC.AsCreature().AddFeat(NWN.Enums.Feat.PlayerTool01);

      //if (NWScript.GetLocalInt(oidSelf, "_LANGUE_ACTIVE") != 0)
      //{
      /*      NWScript.SendMessageToPC(oPC, $"langue = {NWScript.GetLocalInt(oPC, "_LANGUE_ACTIVE")}");
              NWScript.DeleteLocalInt(oPC, "_LANGUE_ACTIVE");
            NWScript.SendMessageToPC(oPC, $"langue = {NWScript.GetLocalInt(oPC, "_LANGUE_ACTIVE")}");

            NWScript.DelayCommand(4.5f, () => NWScript.SetTextureOverride("icon_elf", "", oPC));
              NWScript.DelayCommand(5.0f, () => RefreshQBS(oPC));
       */     //}
              // else
              //{
              //NWScript.SetTextureOverride("icon_elf", "", oidSelf);

      //RefreshQBS(oidSelf, 0);
      // }

      Player player;
      if (!Players.ContainsKey(oPC))
      {
        player = new Player(oPC);
        Players.Add(oPC, player);
      }
      else
        player = Players[oPC];

      NWScript.SetEventScript(oPC, NWScript.EVENT_SCRIPT_CREATURE_ON_NOTICE, "on_perceived_pc");

      // TODO : Système de sauvegarde et de chargement de quickbar
      //EventsPlugin.AddObjectToDispatchList("NWNX_ON_QUICKBAR_SET_BUTTON_AFTER", "event_qbs", oPC);

      if (NWScript.GetIsDM(player.oid) != 1)
      {
        if (player.isNewPlayer)
        {
          // TODO : création des infos du nouveau joueur en BDD
          ObjectPlugin.SetInt(player.oid, "_PC_ID", 1, 1); // TODO : enregistrer l'identifiant de BDD du pj sur le .bic du personnage au lieu du 1 par défaut des tests
          ObjectPlugin.SetInt(player.oid, "_BRP", 1, 1);
        }
        else
        {
          // TODO : Initilisation de l'ancien joueur avec les infos en BDD

          if (ObjectPlugin.GetInt(oPC, "_FROST_ATTACK") != 0)
          {
            EventsPlugin.AddObjectToDispatchList("NWNX_ON_INPUT_ATTACK_OBJECT_BEFORE", "event_auto_spell", oPC);
            EventsPlugin.AddObjectToDispatchList("NWNX_ON_INPUT_FORCE_MOVE_TO_OBJECT_BEFORE", "event_auto_spell", oPC);
            EventsPlugin.AddObjectToDispatchList("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "_onspellcast", oPC);
            EventsPlugin.AddObjectToDispatchList("NWNX_ON_INPUT_KEYBOARD_BEFORE", "event_auto_spell", oPC);
            EventsPlugin.AddObjectToDispatchList("NWNX_ON_INPUT_WALK_TO_WAYPOINT_BEFORE", "event_auto_spell", oPC);
          }

          if (NWScript.GetIsObjectValid(NWScript.GetItemPossessedBy(oPC, "pj_lycan_curse")) == 1)
          {
            CreaturePlugin.AddFeat(oPC, NWScript.FEAT_PLAYER_TOOL_02);
            NWScript.DestroyObject(NWScript.GetItemPossessedBy(oPC, "pj_lycan_curse"));
          }

          // Initialisation de la faim (TODO : récupérer la faim en BDD)
          float fNourriture = 200.0f;

          if (fNourriture < 100.0f)
          {
            int nLoss = 100 - Convert.ToInt32(fNourriture);
            Effect eHunger = NWScript.EffectAbilityDecrease(NWScript.ABILITY_STRENGTH, NWScript.GetAbilityScore(oPC, NWScript.ABILITY_STRENGTH) * nLoss / 100);
            eHunger = NWScript.EffectLinkEffects(eHunger, NWScript.EffectAbilityDecrease(NWScript.ABILITY_DEXTERITY, NWScript.GetAbilityScore(oPC, NWScript.ABILITY_DEXTERITY) * nLoss / 100));
            eHunger = NWScript.EffectLinkEffects(eHunger, NWScript.EffectAbilityDecrease(NWScript.ABILITY_CONSTITUTION, NWScript.GetAbilityScore(oPC, NWScript.ABILITY_CONSTITUTION) * nLoss / 100));
            eHunger = NWScript.EffectLinkEffects(eHunger, NWScript.EffectAbilityDecrease(NWScript.ABILITY_CHARISMA, NWScript.GetAbilityScore(oPC, NWScript.ABILITY_CHARISMA) * nLoss / 100));
            eHunger = NWScript.SupernaturalEffect(eHunger);
            eHunger = NWScript.TagEffect(eHunger, "Effect_Hunger");
            NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_PERMANENT, eHunger, oPC);
          }

          if (ObjectPlugin.GetString(player.oid, "_LOCATION").Length > 0 && NWScript.GetIsDM(player.oid) != 1)
          {
            NWScript.DelayCommand(1.0f, () => NWScript.AssignCommand(player.oid, () => NWScript.ClearAllActions()));
            NWScript.DelayCommand(1.1f, () => NWScript.AssignCommand(player.oid, () => NWScript.JumpToLocation(Utils.StringToLocation(ObjectPlugin.GetString(player.oid, "_LOCATION")))));
          }

          if (ObjectPlugin.GetInt(player.oid, "_CURRENT_JOB") != 0) // probablement plutôt initialiser ça à partir de la BDD
          {
            player.learnableSkills[ObjectPlugin.GetInt(player.oid, "_CURRENT_JOB")].currentJob = true;
            player.AcquireSkillPoints();
          }
          else
            NWScript.DelayCommand(10.0f, () => player.PlayNoCurrentTrainingEffects());

          ObjectPlugin.SetString(player.oid, "_DATE_LAST_SAVED", DateTime.Now.ToString(), 1);
        }

        //Appliquer la distance de perception du chat en fonction de la compétence Listen du joueur
        ChatPlugin.SetChatHearingDistance(ChatPlugin.GetChatHearingDistance(oPC, ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK) + NWScript.GetSkillRank(NWScript.SKILL_LISTEN, oPC) / 5, oPC, ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK);
        ChatPlugin.SetChatHearingDistance(ChatPlugin.GetChatHearingDistance(oPC, ChatPlugin.NWNX_CHAT_CHANNEL_DM_WHISPER) + NWScript.GetSkillRank(NWScript.SKILL_LISTEN, oPC) / 10, oPC, ChatPlugin.NWNX_CHAT_CHANNEL_DM_WHISPER);
        player.isConnected = true;
        player.isAFK = true;
        
        EventsPlugin.AddObjectToDispatchList("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "player_exit_before", oPC);
      }
      return 1;
    }

    private static int HandlePlayerDisconnect(uint oidSelf)
    {
 /*     var oPC = NWScript.GetExitingObject();
      EventsPlugin.RemoveObjectFromDispatchList(EventsPlugin.ON_INPUT_KEYBOARD_BEFORE, ON_PC_KEYSTROKE_SCRIPT, oPC);
      Players.Remove(oPC);
*/
      return 1;
    }

    private static int HandlePlayerKeystroke(uint oidSelf)
    {
      var key = EventsPlugin.GetEventData("KEY");
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        player.EmitKeydown(new Player.KeydownEventArgs(key));
      }

      return 1;
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

      return 1;
    }
    private static int HandleBeforePlayerSave(uint oidSelf)
    {
      /* Fix polymorph bug : Lorsqu'un PJ métamorphosé est sauvegardé, toutes ses buffs sont supprimées afin que les stats de 
       * la nouvelle forme ne remplace pas celles du PJ dans son fichier .bic. Après sauvegarde, les stats de la métamorphose 
       * sont réappliquées. 
       * Bug 1 : les PV temporaires de la forme se cumulent avec chaque sauvegarde, ce qui permet d'avoir PV infinis
       * BUG 2 : Les buffs ne faisant pas partie de la métamorphose (appliquées par sort par exemple), ne sont pas réappliquées
       * Ici, la correction consiste à ne pas sauvegarder le PJ s'il est métamorphosé, sauf s'il s'agit d'une déconnexion.
       * Mais il se peut que dans ce cas, ses buffs soient perdues à la reco. A vérifier. Si c'est le cas, une meilleure
       * correction pourrait être de parcourir tous ses buffs et de les réappliquer dans l'event AFTER de la sauvegarde*/

        Player player;
        if (Players.TryGetValue(oidSelf, out player))
        {
          if (player.isConnected)
          {
            if (Utils.HasAnyEffect(player.oid, NWScript.EFFECT_TYPE_POLYMORPH))
            {
              EventsPlugin.SkipEvent();
              return 1;
            }
          }

          // TODO : probablement faire pour chaque joueur tous les check faim / soif / jobs etc ici

          // AFK detection
          if(ObjectPlugin.GetString(player.oid, "_LOCATION") != Utils.LocationToString(NWScript.GetLocation(player.oid)))
          {
            ObjectPlugin.SetString(player.oid, "_LOCATION", Utils.LocationToString(NWScript.GetLocation(player.oid)), 1);
            player.isAFK = false;
          }

          if(NWScript.GetLocalInt(player.oid, "REST") != 0)
            player.CraftJobProgression();

          player.AcquireSkillPoints();
          ObjectPlugin.SetString(player.oid, "_DATE_LAST_SAVED", DateTime.Now.ToString(), 1);
          ObjectPlugin.SetInt(player.oid, "_CURRENT_HP", NWScript.GetCurrentHitPoints(player.oid), 1);
          player.isAFK = true;
        }

      return 1;
    }
    private static int HandleBeforeDMPossess(uint oidSelf)
    {
      var oPossessed = ObjectPlugin.StringToObject(EventsPlugin.GetEventData("TARGET"));
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
      return 1;
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
            var oObject = ObjectPlugin.StringToObject(EventsPlugin.GetEventData("OBJECT"));
            // TODO : Enregistrer l'objet créé en base de données. Ajouter à l'objet un script qui le supprime de la BDD OnDeath
            NWScript.SendMessageToPC(oPC.oid, $"Création persistante - Vous posez le placeable  {NWScript.GetName(oObject)}");
          }
          else
            NWScript.SendMessageToPC(oPC.oid, "Création temporaire - Ce placeable sera effacé par le prochain reboot.");
        }
      }
      return 1;
    }

    private static int HandlePlayerBeforeDisconnect(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        player.isConnected = false;
        //TODO : plutôt utiliser les fonctions sqlite de la prochaine MAJ ?
        ObjectPlugin.SetInt(player.oid, "_CURRENT_HP", NWScript.GetCurrentHitPoints(player.oid), 1);
        ObjectPlugin.SetString(player.oid, "_LOCATION", Utils.LocationToString(NWScript.GetLocation(player.oid)), 1); 

        HandleBeforePartyLeave(oidSelf);
        HandleAfterPartyLeave(oidSelf);
      }

      return 1;
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

      return 1;
    }

    private static int HandleFeatUsed(uint oidSelf)
    {
      string current_event = EventsPlugin.GetCurrentEvent();
      var feat = int.Parse(EventsPlugin.GetEventData("FEAT_ID"));

      if (current_event == "NWNX_ON_USE_FEAT_BEFORE")
      {
        if (feat == NWScript.FEAT_PLAYER_TOOL_02)
        {
          EventsPlugin.SkipEvent();
          PlayerSystem.Player oPC;
          if (PlayerSystem.Players.TryGetValue(oidSelf, out oPC))
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

          return 1;
        }
        else if (feat == 1116) // TODO : enum LanguageElf
        {
          NWScript.SendMessageToPC(oidSelf, $"langue = {NWScript.GetLocalInt(oidSelf, "_LANGUE_ACTIVE")}");
          if (NWScript.GetLocalInt(oidSelf, "_LANGUE_ACTIVE") == feat)
          {
            NWScript.DeleteLocalInt(oidSelf, "_LANGUE_ACTIVE");
            NWScript.SendMessageToPC(oidSelf, "Vous cessez de parler elfe.");
            NWScript.SetTextureOverride("icon_elf", "", oidSelf);

            RefreshQBS(oidSelf, feat);
          }
          else
          {
            NWScript.SetLocalInt(oidSelf, "_LANGUE_ACTIVE", feat);
            NWScript.SendMessageToPC(oidSelf, "Vous parlez désormais elfe.");
            NWScript.SetTextureOverride("icon_elf", "icon_elf_active", oidSelf);

            RefreshQBS(oidSelf, feat);
          }
        
          return 1;
        }
        else if (feat == NWScript.FEAT_PLAYER_TOOL_01)
        {
          EventsPlugin.SkipEvent();
          var oTarget = ObjectPlugin.StringToObject(EventsPlugin.GetEventData("TARGET_OBJECT_ID"));
          PlayerSystem.Player myPlayer;
          if (PlayerSystem.Players.TryGetValue(oidSelf, out myPlayer))
          {
            if (NWScript.GetIsObjectValid(oTarget) == 1)
            {
              /*Utils.Meuble result;
              if (Enum.TryParse(oTarget.Tag, out result))
              {
                EventsPlugin.AddObjectToDispatchList("NWNX_ON_INPUT_KEYBOARD_AFTER", "event_mv_plc", oidSelf);
                oidSelf.AsObject().Locals.Object.Set("_MOVING_PLC", oTarget);
                oidSelf.AsPlayer().SendMessage($"Vous venez de sélectionner {oTarget.Name}, utilisez votre barre de raccourcis pour le déplacer. Pour enregistrer le nouvel emplacement et retrouver votre barre de raccourcis habituelle, activez le don sur un endroit vide (sans cible).");
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
          return 1;
        }
      }
      return 1;
    }

    private static void RefreshQBS(uint oidSelf, int feat)
    {
      string sQuickBar = CreaturePlugin.SerializeQuickbar(oidSelf);
      QuickBarSlot emptyQBS = QuickBarSlot.CreateEmptyQBS();
      QuickBarSlot 
      for (int i = 0; i < 36; i++)
      {
        QuickBarSlot qbs = NWNX.Player.GetQuickBarSlot(oidSelf, i);

        if (qbs.ObjectType == QuickBarSlotType.Feat && qbs.INTParam1 == feat)
        {
          NWNX.Player.SetQuickBarSlot(oidSelf, i, emptyQBS);
        }
      }

      CreaturePlugin.DeserializeQuickbar(oidSelf, sQuickBar);
    }

    private static void RefreshQBS(uint oidSelf)
    {
      string sQuickBar = CreaturePlugin.SerializeQuickbar(oidSelf);
      QuickBarSlot emptyQBS = QuickBarSlot.CreateEmptyQBS();

      for (int i = 0; i < 36; i++)
      {
        QuickBarSlot qbs = NWNX.Player.GetQuickBarSlot(oidSelf, i);
        NWNX.Player.SetQuickBarSlot(oidSelf, i, emptyQBS);
      }

      CreaturePlugin.DeserializeQuickbar(oidSelf, sQuickBar);
    }

    private static int HandleAutoSpell(uint oidSelf) //Je garde ça sous la main, mais je pense que le gérer différement serait mieux, notamment en créant un mode activable "autospell" en don gratuit pour les casters. Donc : A RETRAVAILLER 
    {
      Player oPC;

      if (Players.TryGetValue(oidSelf, out oPC))
      {
        var oTarget = ObjectPlugin.StringToObject(EventsPlugin.GetEventData("TARGET"));

        if (oTarget.AsObject().IsValid)
        {
          NWScript.ClearAllActions();
          if (oPC.autoAttackTarget == NWObject.OBJECT_INVALID)
          {
            oidSelf.AsPlayer().CastSpellAtObject(Spell.RayOfFrost, oTarget);
            NWScript.DelayCommand(6.0f, () => oPC.OnFrostAutoAttackTimedEvent());
          }
        }

        oPC.autoAttackTarget = oTarget;
      }

      return 1;
    }

    private static int HandleOnSpellCast(uint oidSelf)
    {
      Player oPC;

      if (Players.TryGetValue(oidSelf, out oPC))
      {
        var spellId = int.Parse(EventsPlugin.GetEventData("SPELL_ID"));

        if (spellId != (int)Spell.RayOfFrost)
          oPC.autoAttackTarget = NWObject.OBJECT_INVALID;
      }

      return 1;
    }

    private static void FrostAutoAttack(NWObject oClicker, uint oTarget)
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
          if (!oPerceived.IsPC || oPerceived.IsDM || oPerceived.IsDMPossessed || oPerceived.disguiseName.Length == 0)
            return 1;

          if (!oPC.disguiseDetectTimer.ContainsKey(oPC) || (DateTime.Now - oPC.disguiseDetectTimer[oPerceived]).TotalSeconds > 1800)
          {
            oPC.disguiseDetectTimer[oPerceived] = DateTime.Now;

            int[] iPCSenseSkill = { oPC.GetSkillRank(NWScript.SKILL_LISTEN), oPC.GetSkillRank(Skill.Search), oPC.GetSkillRank(Skill.Spot),
            oPC.GetSkillRank(Skill.Bluff) };

            int[] iPerceivedHideSkill = { oPerceived.GetSkillRank(Skill.Bluff), oPerceived.GetSkillRank(Skill.Hide),
            oPerceived.GetSkillRank(Skill.Perform), oPerceived.GetSkillRank(Skill.Persuade) };

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
              NWScript.SendMessageToPC(oPC.oid, oPerceived.Name + " fait usage d'un déguisement ! Sous le masque, vous reconnaissez " + NWScript.GetName(oPerceived, true));
              //NWNX_Rename_ClearPCNameOverride(oPerceived, oPC);
            }
          }
        }
      }

      return 1;
    }
    private static int HandleAfterAddSummon(uint oidSelf)
    {
      //Pas méga utile dans l'immédiat, mais pourra être utilisé pour gérer les invocations de façon plus fine plus tard
      // TODO : Système de possession d'invocations, compagnons animaux, etc (mais attention, vérifier que si le PJ déco en possession, ça n'écrase pas son .bic. Si oui, sauvegarde le PJ avant possession et ne plus sauvegarder le PJ en mode possession)
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        NWCreature oSummon = (ObjectPlugin.StringToObject(EventsPlugin.GetEventData("ASSOCIATE_OBJECT_ID"))).AsCreature();

        if (oSummon.IsValid)
        {
          player.summons.Add(oSummon, oSummon);
        }
      }

      return 1;
    }
    private static int HandleAfterRemoveSummon(uint oidSelf)
    {
      //Pas méga utile dans l'immédiat, mais pourra être utilisé pour gérer les invocations de façon plus fine plus tard
      // TODO : Système de possession d'invocations, compagnons animaux, etc (mais attention, vérifier que si le PJ déco en possession, ça n'écrase pas son .bic. Si oui, sauvegarde le PJ avant possession et ne plus sauvegarder le PJ en mode possession)
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        NWCreature oSummon = (ObjectPlugin.StringToObject(EventsPlugin.GetEventData("ASSOCIATE_OBJECT_ID"))).AsCreature();

        if (oSummon.IsValid)
        {
          player.summons.Remove(oSummon);
        }
      }

      return 1;
    }
    private static int HandleOnCombatMode(uint oidSelf)
    { 
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
          if (NWScript.GetLocalInt(player, "_ACTIVATED_TAUNT") != 0) // Permet de conserver sa posture de combat après avoir utilisé taunt
          {
            EventsPlugin.SkipEvent();
            NWScript.DeleteLocalInt(player, "_ACTIVATED_TAUNT");
          }
      }

      return 1;
    }
    private static int HandleOnSkillUsed(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
          if (int.Parse(EventsPlugin.GetEventData("SKILL_ID")) == (int)Skill.Taunt)
          {
            NWScript.SetLocalInt(player, "_ACTIVATED_TAUNT", 1);
            NWScript.DelayCommand(12.0f, () => NWScript.DeleteLocalInt(player, "_ACTIVATED_TAUNT"));
          }
          else if (int.Parse(EventsPlugin.GetEventData("SKILL_ID")) == (int)Skill.PickPocket)
          {
            EventsPlugin.SkipEvent();
            NWPlayer oObject = ObjectPlugin.StringToObject(EventsPlugin.GetEventData("TARGET_OBJECT_ID")).AsPlayer();
            Player oTarget;
            if (Players.TryGetValue(oObject, out oTarget) && !oTarget.IsDM && !oTarget.IsDMPossessed)
            {
              if (!oTarget.pickpocketDetectTimer.ContainsKey(player) || (DateTime.Now - oTarget.pickpocketDetectTimer[player]).TotalSeconds > 86400)
              {
                  oTarget.pickpocketDetectTimer.Add(player, DateTime.Now);

                  NWNX.Feedback.SetFeedbackMessageHidden(FeedbackMessageTypes.CombatTouchAttack, 1, oTarget);
                  NWScript.DelayCommand(2.0f, () => NWNX.Feedback.SetFeedbackMessageHidden(FeedbackMessageTypes.CombatTouchAttack, 0, oTarget));

                  int iRandom = Utils.random.Next(21);
                  int iVol = NWScript.GetSkillRank(Skill.PickPocket, player);
                  int iSpot = Utils.random.Next(21) + NWScript.GetSkillRank(Skill.Spot, player);
                  if ((iRandom + iVol) > iSpot)
                  {
                    ChatPlugin.SendMessage((int)ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, $"Vous faites un jet de Vol à la tire, le résultat est de : {iRandom} + {iVol} = {iRandom + iVol}.", player, player);
                    if (NWScript.TouchAttackMelee(oTarget) > 0)
                    {
                      int iStolenGold = (iRandom + iVol - iSpot) * 10;
                      if (oTarget.Gold >= iStolenGold)
                      {
                        oTarget.Gold -= iStolenGold;
                        player.Gold += iStolenGold;
                        player.FloatingText($"Vous venez de dérober {iStolenGold} pièces d'or des poches de {oTarget.Name} !");
                      }
                      else
                      {
                        player.FloatingText($"Vous venez de vider les poches de {oTarget.Name} ! {oTarget.Gold} pièces d'or de plus pour vous.");
                        player.Gold += oTarget.Gold;
                        oTarget.Gold = 0;
                      }
                    }
                    else
                    {
                      player.FloatingText($"Vous ne parvenez pas à atteindre les poches de {oTarget.Name} !");
                    }
                  }
                  else
                    oTarget.FloatingText($"{player.Name} est en train d'essayer de vous faire les poches !"); 
              }
              else
                player.FloatingText("Vous n'êtes pas autorisé à faire une nouvelle tentative pour le moment.");
            }
            else
              player.FloatingText("Seuls d'autres joueurs peuvent être ciblés par cette compétence. Les tentatives de vol sur PNJ doivent être jouées en rp avec un dm.");
          }
      }

      return 1;
    }
    private static int HandleAfterDetection(uint oidSelf)
    {
      Player oPC;
      if (Players.TryGetValue(oidSelf, out oPC))
      {
        var oTarget = ObjectPlugin.StringToObject(EventsPlugin.GetEventData("TARGET")).AsCreature();

        if (oTarget.IsPC || oTarget.IsPossessedFamiliar)
        {
          if (!oTarget.GetObjectSeen(oPC) && NWScript.GetDistanceBetween(oTarget, oPC) < 20.0f)
          {
            int iDetectMode = (int)NWScript.GetDetectMode(oPC);
            if (int.Parse(EventsPlugin.GetEventData("TARGET_INVISIBLE")) == 1 && iDetectMode > 0)
            {
              switch (CreaturePlugin.GetMovementType(oTarget))
              {
                case MovementType.WalkBackwards:
                case MovementType.Sidestep:
                case MovementType.Walk:

                  if (!oPC.inviDetectTimer.ContainsKey(oTarget) || (DateTime.Now - oPC.inviDetectTimer[oTarget]).TotalSeconds > 6)
                  {
                    int iMoveSilentlyCheck = NWScript.GetSkillRank(Skill.MoveSilently, oTarget) + Utils.random.Next(21) + (int)NWScript.GetDistanceBetween(oTarget, oPC);
                    int iListenCheck = NWScript.GetSkillRank(NWScript.SKILL_LISTEN, oPC) + Utils.random.Next(21);
                    if (iDetectMode == 2)
                      iListenCheck -= 10;

                    if (iListenCheck > iMoveSilentlyCheck)
                    {
                      NWScript.SendMessageToPC(oPC.oid, "Vous entendez quelqu'un se faufiler dans les environs.");
                      NWNX.Player.ShowVisualEffect(oPC, (int)Flashier.Vfx_Fnf_Smoke_Puff, oTarget.Position);
                      oPC.inviDetectTimer.Add(oTarget, DateTime.Now);
                      oPC.inviEffectDetectTimer.Add(oTarget, DateTime.Now);
                    }
                    else
                      oPC.inviDetectTimer.Remove(oTarget);
                  }
                  else if ((DateTime.Now - oPC.inviEffectDetectTimer[oTarget]).TotalSeconds > 1)
                  {
                    NWNX.Player.ShowVisualEffect(oPC, (int)Flashier.Vfx_Fnf_Smoke_Puff, oTarget.Position);
                    oPC.inviEffectDetectTimer.Add(oTarget, DateTime.Now);
                  }
                  break;
                case MovementType.Run:

                  if (!oPC.inviDetectTimer.ContainsKey(oTarget) || (DateTime.Now - oPC.inviDetectTimer[oTarget]).TotalSeconds > 6)
                  {
                    NWScript.SendMessageToPC(oPC.oid, "Vous entendez quelqu'un courir peu discrètement dans les environs.");
                    NWNX.Player.ShowVisualEffect(oPC, (int)Flashier.Vfx_Fnf_Smoke_Puff, oTarget.Position);
                    oPC.inviDetectTimer.Add(oTarget, DateTime.Now);
                    oPC.inviEffectDetectTimer.Add(oTarget, DateTime.Now);
                  }
                  else if ((DateTime.Now - oPC.inviEffectDetectTimer[oTarget]).TotalSeconds > 1)
                  {
                    NWNX.Player.ShowVisualEffect(oPC, (int)Flashier.Vfx_Fnf_Smoke_Puff, oTarget.Position);
                    oPC.inviEffectDetectTimer.Add(oTarget, DateTime.Now);
                  }
                  break;
              }
            }
          }
        }
      }

      return 1;
    }
    private static int HandlePlayerDeath(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(NWScript.GetLastPlayerDied(), out player))
      {
        player.SendMessage("Tout se brouille autour de vous. Avant de perdre connaissance, vous sentez comme un étrange maëlstrom vous aspirer.");

        NWPlaceable oPCCorpse = NWScript.CreateObject(ObjectType.Placeable, "pccorpse", NWScript.GetLocation(player.oid)).AsPlaceable();
        EventsPlugin.AddObjectToDispatchList("NWNX_ON_INVENTORY_REMOVE_ITEM_AFTER", "event_pccorpse_remove_item_after", oPCCorpse);
        EventsPlugin.AddObjectToDispatchList("NWNX_ON_INVENTORY_ADD_ITEM_AFTER", "event_pccorpse_add_item_after", oPCCorpse);

        int PlayerId = ObjectPlugin.GetInt(player, "_PC_ID");
        oPCCorpse.Name = $"Cadavre de {player.Name}";
        oPCCorpse.Description = $"Cadavre de {player.Name}";
        oPCCorpse.Locals.Int.Set("_PC_ID", PlayerId);

        NWScript.SetLocalInt(NWScript.CreateItemOnObject("item_pccorpse", oPCCorpse), "PC_ID", PlayerId);

        if (player.Gold > 0)
        {
          do
          {
            NWScript.CreateItemOnObject("nw_it_gold001", oPCCorpse, player.Gold);
            player.Gold -= 50000;
          } while (player.Gold > 50000);
        }

        if (player.Gold < 0)
          player.Gold = 0;

        // TODO : Dropper toutes les ressources craft de l'inventaire du défunt

        // TODO : Enregistrer l'objet serialized cadavre en BDD pour restauration après reboot + IdPJ + Location

        NWScript.DelayCommand(5.0f, () => player.SendToLimbo());
      }

      return 1;
    }

    private static int HandleAfterDMJumpTarget(uint oidSelf)
    {
      var oTarget = ObjectPlugin.StringToObject(EventsPlugin.GetEventData("TARGET_1"));

      Player player;
      if (Players.TryGetValue(oTarget, out player))
      {
        if(NWScript.GetTag(NWScript.GetArea(player.oid)) == "Labrume")
        {
          player.DestroyCorpses();
        }
      }

      return 1;
    }
    public static PlayerSystem.Player GetPCById(int PcId)
    {
      foreach (KeyValuePair<uint, PlayerSystem.Player> PlayerListEntry in PlayerSystem.Players)
      {
        if(ObjectPlugin.GetInt(PlayerListEntry.Key, "_PC_ID") == PcId)
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
        NWCreature oTarget = ObjectPlugin.StringToObject(EventsPlugin.GetEventData("TARGET_OBJECT_ID")).AsCreature();

        if (Spells.GetHasEffect(NWScript.GetEffectType(NWScript.EffectCutsceneGhost()), player))
          Spells.RemoveEffectOfType(NWScript.GetEffectType(NWScript.EffectCutsceneGhost()), player);          

        if (oTarget.IsPC)
        {
          NWScript.SetPCDislike(player, oTarget);
          if (Spells.GetHasEffect(NWScript.GetEffectType(NWScript.EffectCutsceneGhost()), oTarget))
            Spells.RemoveEffectOfType(NWScript.GetEffectType(NWScript.EffectCutsceneGhost()), oTarget);
        }
      }

      return 1;
    }
    private static int HandleAfterPartyAccept(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        Effect eParty = player.GetPartySizeEffect();

        // appliquer l'effet sur chaque membre du groupe
        NWPlayer oPartyMember = NWScript.GetFirstFactionMember(oidSelf, true).AsPlayer();
        while (oPartyMember.IsValid)
        {
          oPartyMember.RemoveTaggedEffect("PartyEffect");
          if (eParty != null)
            oPartyMember.ApplyEffect(NWScript.DURATION_TYPE_PERMANENT, eParty);

          oPartyMember = NWScript.GetNextFactionMember(oPartyMember, true).AsPlayer();
        }
      }   

      return 1;
    }
    private static int HandleAfterPartyLeave(uint oidSelf)
    {
      oidSelf.AsPlayer().RemoveTaggedEffect("PartyEffect");
      return 1;
    }
    private static int HandleAfterPartyKick(uint oidSelf)
    {
      ObjectPlugin.StringToObject(EventsPlugin.GetEventData("KICKED")).AsPlayer().RemoveTaggedEffect("PartyEffect");
      return 1;
    }
    private static int HandleBeforePartyLeave(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        Effect eParty = player.GetPartySizeEffect(-1);
        // appliquer l'effet sur chaque membre du groupe
        NWPlayer oPartyMember = NWScript.GetFirstFactionMember(oidSelf, true).AsPlayer();
        while (oPartyMember.IsValid)
        {
          oPartyMember.RemoveTaggedEffect("PartyEffect");
          if (eParty != null)
            oPartyMember.ApplyEffect(NWScript.DURATION_TYPE_PERMANENT, eParty);

          oPartyMember = NWScript.GetNextFactionMember(oPartyMember, true).AsPlayer();
        }
      }

      return 1;
    }
    private static int HandleBeforeExamine(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        NWObject examineTarget =  ObjectPlugin.StringToObject(EventsPlugin.GetEventData("EXAMINEE_OBJECT_ID")).AsObject();
      
        switch(examineTarget.Tag)
        {
          case "mineable_rock":
            int oreAmount = examineTarget.Locals.Int.Get("_ORE_AMOUNT");
            if (!player.IsDM)
            {
              int geologySkillLevel;
              if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player, (int)Feat.Geology)), out geologySkillLevel))
                examineTarget.Description = $"Minerai disponible : {Utils.random.Next(oreAmount * geologySkillLevel * 20 / 100, 2 * oreAmount - geologySkillLevel * 20 / 100)}";
              else
                examineTarget.Description = $"Minerai disponible estimé : {Utils.random.Next(0, 2 * oreAmount)}";
            }
            else
              examineTarget.Description = $"Minerai disponible : {oreAmount}";

              break;
        }
      }
      return 1;
    }
    private static int HandleAfterExamine(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        NWObject examineTarget = ObjectPlugin.StringToObject(EventsPlugin.GetEventData("EXAMINEE_OBJECT_ID")).AsObject();

        switch (examineTarget.Tag)
        {
          case "mineable_rock":
              examineTarget.Description = $"";
            break;
        }
      }
      return 1;
    }
    private static int HandlePCUnacquireItem(uint oidSelf)
    {
      uint oPC = NWScript.GetModuleItemLostBy();

      if (NWScript.GetMovementRate(oPC) == (int)MovementRate.Immobile)
        if (NWScript.GetWeight(oPC) <= int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", oPC.AsCreature().Ability[Ability.Strength].Total)))
          CreaturePlugin.SetMovementRate(oPC, MovementRate.PC);

      return 1;
    }
    private static int HandlePCAcquireItem(uint oidSelf)
    {
      uint oPC = NWScript.GetModuleItemAcquiredBy();

      if (NWScript.GetMovementRate(oPC) != (int)MovementRate.Immobile)
        if (NWScript.GetWeight(oPC) > int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", oPC.AsCreature().Ability[Ability.Strength].Total)))
          CreaturePlugin.SetMovementRate(oPC, MovementRate.Immobile);

      return 1;
    }
  }
}
