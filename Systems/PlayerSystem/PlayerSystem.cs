using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using NWN.Enums;
using NWN.Enums.VisualEffect;
using NWN.NWNX.Enum;

namespace NWN.Systems
{
  public static partial class PlayerSystem
  {
    public const string ON_PC_KEYSTROKE_SCRIPT = "on_pc_keystroke";

    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
        {
            { "on_pc_perceived", HandlePlayerPerceived },
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
        };

    public static Dictionary<uint, Player> Players = new Dictionary<uint, Player>();

    private static int HandlePlayerConnect(uint oidSelf)
    {
      var oPC = NWScript.GetEnteringObject();

      //TODO : système de BANLIST

      NWNX.Events.AddObjectToDispatchList(NWNX.Events.ON_INPUT_KEYBOARD_BEFORE, ON_PC_KEYSTROKE_SCRIPT, oPC);
      NWNX.Events.AddObjectToDispatchList("NWNX_ON_USE_FEAT_BEFORE", "event_feat_used", oPC);
      NWNX.Events.AddObjectToDispatchList("NWNX_ON_ADD_ASSOCIATE_AFTER", "summon_add_after", oPC);
      NWNX.Events.AddObjectToDispatchList("NWNX_ON_REMOVE_ASSOCIATE_AFTER", "summon_remove_after", oPC);
      NWNX.Events.AddObjectToDispatchList("NWNX_ON_BROADCAST_CAST_SPELL_AFTER", "event_spellbroadcast_after", oPC);
      NWNX.Events.AddObjectToDispatchList("NWNX_ON_ITEM_EQUIP_BEFORE", "event_equip_items_before", oPC);
      NWNX.Events.AddObjectToDispatchList("NWNX_ON_ITEM_UNEQUIP_BEFORE", "event_unequip_items_before", oPC);
      NWNX.Events.AddObjectToDispatchList("NWNX_ON_COMBAT_MODE_OFF", "event_combatmode", oPC);
      NWNX.Events.AddObjectToDispatchList("NWNX_ON_USE_SKILL_BEFORE", "event_skillused", oPC);
      NWNX.Events.AddObjectToDispatchList("NWNX_ON_DO_LISTEN_DETECTION_AFTER", "event_detection_after", oPC);
      NWNX.Events.AddObjectToDispatchList("NWNX_ON_START_COMBAT_ROUND_AFTER", "event_start_combat_after", oPC);

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

      NWScript.SetEventScript(oPC, (int)EventScript.Creature_OnNotice, "on_perceived_pc");

      // TODO : Système de sauvegarde et de chargement de quickbar
      //NWNX.Events.AddObjectToDispatchList("NWNX_ON_QUICKBAR_SET_BUTTON_AFTER", "event_qbs", oPC);

      if (!player.IsDM)
      {
        if (player.IsNewPlayer)
        {
          // TODO : création des infos du nouveau joueur en BDD
          NWNX.Object.SetInt(player, "_PC_ID", 1, true); // TODO : enregistrer l'identifiant de BDD du pj sur le .bic du personnage au lieu du 1 par défaut des tests
          NWNX.Object.SetInt(player, "_BRP", 1, true);
        }
        else
        {
          // TODO : Initilisation de l'ancien joueur avec les infos en BDD

          if (NWNX.Object.GetInt(oPC, "_FROST_ATTACK") != 0)
          {
            NWNX.Events.AddObjectToDispatchList("NWNX_ON_INPUT_ATTACK_OBJECT_BEFORE", "event_auto_spell", oPC);
            NWNX.Events.AddObjectToDispatchList("NWNX_ON_INPUT_FORCE_MOVE_TO_OBJECT_BEFORE", "event_auto_spell", oPC);
            NWNX.Events.AddObjectToDispatchList("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "_onspellcast", oPC);
            NWNX.Events.AddObjectToDispatchList("NWNX_ON_INPUT_KEYBOARD_BEFORE", "event_auto_spell", oPC);
            NWNX.Events.AddObjectToDispatchList("NWNX_ON_INPUT_WALK_TO_WAYPOINT_BEFORE", "event_auto_spell", oPC);
          }

          if (oPC.AsCreature().GetPossessedItem("pj_lycan_curse").IsValid)
          {
            oPC.AsCreature().AddFeat(NWN.Enums.Feat.PlayerTool02);
            oPC.AsCreature().GetPossessedItem("pj_lycan_curse").Destroy();
          }

          // Initialisation de la faim (TODO : récupérer la faim en BDD)
          float fNourriture = 200.0f;

          if (fNourriture < 100.0f)
          {
            int nLoss = 100 - Convert.ToInt32(fNourriture);
            Effect eHunger = NWScript.EffectAbilityDecrease((int)Ability.Strength, NWScript.GetAbilityScore(oPC, Ability.Strength) * nLoss / 100);
            eHunger = NWScript.EffectLinkEffects(eHunger, NWScript.EffectAbilityDecrease((int)Ability.Dexterity, NWScript.GetAbilityScore(oPC, Ability.Dexterity) * nLoss / 100));
            eHunger = NWScript.EffectLinkEffects(eHunger, NWScript.EffectAbilityDecrease((int)Ability.Constitution, NWScript.GetAbilityScore(oPC, Ability.Constitution) * nLoss / 100));
            eHunger = NWScript.EffectLinkEffects(eHunger, NWScript.EffectAbilityDecrease((int)Ability.Charisma, NWScript.GetAbilityScore(oPC, Ability.Charisma) * nLoss / 100));
            eHunger = NWScript.SupernaturalEffect(eHunger);
            eHunger = NWScript.TagEffect(eHunger, "Effect_Hunger");
            NWScript.ApplyEffectToObject(DurationType.Permanent, eHunger, oPC);
          }

          if (NWNX.Object.GetString(player, "_LOCATION").Length > 0 && !player.IsDM)
          {
            NWScript.DelayCommand(1.0f, () => NWScript.AssignCommand(player, () => player.ClearAllActions(true)));
            NWScript.DelayCommand(1.1f, () => NWScript.AssignCommand(player, () => NWScript.JumpToLocation(Utils.StringToLocation(NWNX.Object.GetString(player, "_LOCATION")))));
          }

          player.CalculateAcquiredSkillPoints();
          NWNX.Object.SetString(player, "_DATE_LAST_SAVED", DateTime.Now.ToString(), true);
        }

        //Appliquer la distance de perception du chat en fonction de la compétence Listen du joueur
        NWNX.Chat.SetChatHearingDistance(NWNX.Chat.GetChatHearingDistance(oPC.AsObject(), NWNX.Enum.ChatChannel.PlayerTalk) + NWScript.GetSkillRank(Skill.Listen, oPC) / 5, oPC.AsObject(), NWNX.Enum.ChatChannel.PlayerTalk);
        NWNX.Chat.SetChatHearingDistance(NWNX.Chat.GetChatHearingDistance(oPC.AsObject(), NWNX.Enum.ChatChannel.PlayerWhisper) + NWScript.GetSkillRank(Skill.Listen, oPC) / 10, oPC.AsObject(), NWNX.Enum.ChatChannel.PlayerWhisper);
        player.isConnected = true;
        player.isAFK = true;
      }
      return Entrypoints.SCRIPT_HANDLED;
    }

    private static int HandlePlayerDisconnect(uint oidSelf)
    {
 /*     var oPC = NWScript.GetExitingObject();
      NWNX.Events.RemoveObjectFromDispatchList(NWNX.Events.ON_INPUT_KEYBOARD_BEFORE, ON_PC_KEYSTROKE_SCRIPT, oPC);
      Players.Remove(oPC);
*/
      return Entrypoints.SCRIPT_HANDLED;
    }

    private static int HandlePlayerKeystroke(uint oidSelf)
    {
      var key = NWNX.Events.GetEventData("KEY");
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        player.EmitKeydown(new Player.KeydownEventArgs(key));
      }

      return Entrypoints.SCRIPT_HANDLED;
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
            if (player.HasAnyEffect((int)EffectTypeEngine.Polymorph))
            {
              NWNX.Events.SkipEvent();
              return Entrypoints.SCRIPT_HANDLED;
            }
          }

          // TODO : probablement faire pour chaque joueur tous les check faim / soif / jobs etc ici

          // AFK detection
          if(NWNX.Object.GetString(player, "_LOCATION") != Utils.LocationToString(player.Location))
          {
            NWNX.Object.SetString(player, "_LOCATION", Utils.LocationToString(player.Location), true);
            player.isAFK = false;
          }

          player.CalculateAcquiredSkillPoints();
          NWNX.Object.SetString(player, "_DATE_LAST_SAVED", DateTime.Now.ToString(), true);
          NWNX.Object.SetInt(player, "_CURRENT_HP", player.CurrentHP, true);
          player.isAFK = true;
        }

      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleBeforeDMPossess(uint oidSelf)
    {
      NWCreature oPossessed = NWNX.Object.StringToObject(NWNX.Events.GetEventData("TARGET")).AsCreature();
      Player oPC;
      if (Players.TryGetValue(oidSelf, out oPC))
      {
        if (oPossessed.IsValid)
        { // Ici, on prend possession
          if (oPC.IsDMPossessed)
          {
            NWScript.SetLocalObject(NWScript.GetLocalObject(oPC, "_POSSESSER"), "_POSSESSING", oPossessed);
            NWScript.SetLocalObject(oPossessed, "_POSSESSER", NWScript.GetLocalObject(oPC, "_POSSESSER"));
          }
          else
          {
            NWScript.SetLocalObject(oPC, "_POSSESSING", oPossessed);
            NWScript.SetLocalObject(oPossessed, "_POSSESSER", oPC);
          }
        }
        else
        {  // Ici, on cesse la possession
          if ((oPC.IsDMPossessed))
          {
            NWScript.DeleteLocalObject(NWScript.GetLocalObject(oPC, "_POSSESSER"), "_POSSESSING");
            NWScript.DeleteLocalObject(NWScript.GetLocalObject(oPC, "_POSSESSER"), "_POSSESSER");
          }
          else
          {
            NWScript.DeleteLocalObject(NWScript.GetLocalObject(oPC, "_POSSESSER"), "_POSSESSING");
            NWScript.DeleteLocalObject(oPC, "_POSSESSER");
          }
        }
      }
      return Entrypoints.SCRIPT_HANDLED;
    }

    private static int HandleAfterDMSpawnObject(uint oidSelf)
    {
      Player oPC;
      if (Players.TryGetValue(oidSelf, out oPC))
      {
        if (int.Parse(NWNX.Events.GetEventData("OBJECT_TYPE")) == 9)
        {
          if (NWNX.Object.GetInt(oPC, "_SPAWN_PERSIST") != 0)
          {
            NWPlaceable oObject = NWNX.Object.StringToObject(NWNX.Events.GetEventData("OBJECT")).AsPlaceable();
            // TODO : Enregistrer l'objet créé en base de données. Ajouter à l'objet un script qui le supprime de la BDD OnDeath
            oPC.SendMessage($"Création persistante - Vous posez le placeable  {oObject.Name}");
          }
          else
            oPC.SendMessage("Création temporaire - Ce placeable sera effacé par le prochain reboot.");
        }
      }
      return Entrypoints.SCRIPT_HANDLED;
    }

    private static int HandlePlayerBeforeDisconnect(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        player.isConnected = false;
        //TODO : plutôt utiliser les fonctions sqlite de la prochaine MAJ ?
        NWNX.Object.SetInt(player, "_CURRENT_HP", player.CurrentHP, true);
        NWNX.Object.SetString(player, "_LOCATION", Utils.LocationToString(player.Location), true);
      }

      return Entrypoints.SCRIPT_HANDLED;
    }

    private static int HandleMovePlaceable(uint oidSelf)
    {
      string current_event = NWNX.Events.GetCurrentEvent();

      string sKey = NWNX.Events.GetEventData("KEY");
      NWPlaceable oMeuble = NWScript.GetLocalObject(oidSelf, "_MOVING_PLC").AsPlaceable();
      Vector vPos = oMeuble.Position;

      if (sKey == "W")
        oMeuble.AddToArea(oMeuble.Area, NWScript.Vector(vPos.x, vPos.y + 0.1f, vPos.z));
      else if (sKey == "S")
        oMeuble.AddToArea(oMeuble.Area, NWScript.Vector(vPos.x, vPos.y - 0.1f, vPos.z));
      else if (sKey == "D")
        oMeuble.AddToArea(oMeuble.Area, NWScript.Vector(vPos.x + 0.1f, vPos.y, vPos.z));
      else if (sKey == "A")
        oMeuble.AddToArea(oMeuble.Area, NWScript.Vector(vPos.x - 0.1f, vPos.y, vPos.z));
      else if (sKey == "Q")
        oMeuble.Facing = oMeuble.Facing - 20.0f;
      //NWScript.AssignCommand(oMeuble, () => oMeuble.Facing (oMeuble.Facing - 20.0f));
      else if (sKey == "E")
        oMeuble.Facing = oMeuble.Facing + 20.0f;
      //NWScript.AssignCommand(oMeuble, () => NWScript.SetFacing(oMeuble.Facing + 20.0f));

      return Entrypoints.SCRIPT_HANDLED;
    }

    private static int HandleFeatUsed(uint oidSelf)
    {
      string current_event = NWNX.Events.GetCurrentEvent();
      var feat = int.Parse(NWNX.Events.GetEventData("FEAT_ID"));

      if (current_event == "NWNX_ON_USE_FEAT_BEFORE")
      {
        if (feat == (int)NWN.Enums.Feat.PlayerTool02)
        {
          NWNX.Events.SkipEvent();
          PlayerSystem.Player oPC;
          if (PlayerSystem.Players.TryGetValue(oidSelf, out oPC))
          {
            if (oPC.HasTagEffect("lycan_curse"))
            {
              oPC.RemoveTaggedEffect("lycan_curse");
              oPC.RemoveLycanCurse();
            }
            else
            {
              if ((DateTime.Now - oPC.LycanCurseTimer).TotalSeconds > 10800)
              {
                oPC.ApplyLycanCurse();
                oPC.LycanCurseTimer = DateTime.Now;
              }
              else
                oPC.SendMessage("Vous ne vous sentez pas encore la force de changer de nouveau de forme.");
            }
          }

          return Entrypoints.SCRIPT_HANDLED;
        }
        else if (feat == (int)NWN.Enums.Feat.LanguageElf)
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
        
          return Entrypoints.SCRIPT_HANDLED;
        }
        else if (feat == (int)NWN.Enums.Feat.PlayerTool01)
        {
          NWNX.Events.SkipEvent();
          NWPlaceable oTarget = NWNX.Object.StringToObject(NWNX.Events.GetEventData("TARGET_OBJECT_ID")).AsPlaceable();
          PlayerSystem.Player myPlayer;
          if (PlayerSystem.Players.TryGetValue(oidSelf, out myPlayer))
          {
            if (oTarget.IsValid)
            {
              /*Utils.Meuble result;
              if (Enum.TryParse(oTarget.Tag, out result))
              {
                NWNX.Events.AddObjectToDispatchList("NWNX_ON_INPUT_KEYBOARD_AFTER", "event_mv_plc", oidSelf);
                oidSelf.AsObject().Locals.Object.Set("_MOVING_PLC", oTarget);
                oidSelf.AsPlayer().SendMessage($"Vous venez de sélectionner {oTarget.Name}, utilisez votre barre de raccourcis pour le déplacer. Pour enregistrer le nouvel emplacement et retrouver votre barre de raccourcis habituelle, activez le don sur un endroit vide (sans cible).");
                //remplacer la ligne précédente par un PostString().

                if (myPlayer.SelectedObjectsList.Count == 0)
                {
                  myPlayer.BoulderBlock();
                }

                if (!myPlayer.SelectedObjectsList.Contains(oTarget))
                  myPlayer.SelectedObjectsList.Add(oTarget);
              }
              else
              {
                oidSelf.AsPlayer().SendMessage("Vous ne pouvez pas manier cet élément.");
              }*/
            }
            else
            {
              string sObjectSaved = "";

              foreach (uint selectedObject in myPlayer.SelectedObjectsList)
              {
                var sql = $"UPDATE sql_meubles SET objectLocation = @loc WHERE objectUUID = @uuid";

                using (var connection = MySQL.GetConnection())
                {
                  connection.Execute(sql, new { uuid = selectedObject.AsObject().uuid, loc = Utils.LocationToString(selectedObject.AsObject().Location) });
                }

                sObjectSaved += selectedObject.AsObject().Name + "\n";
              }

              myPlayer.SendMessage($"Vous venez de sauvegarder le positionnement des meubles : \n{sObjectSaved}");
              myPlayer.BoulderUnblock();

              NWNX.Events.RemoveObjectFromDispatchList("NWNX_ON_INPUT_KEYBOARD_AFTER", "event_mv_plc", oidSelf);
              oidSelf.AsObject().Locals.Object.Delete("_MOVING_PLC");
              myPlayer.SelectedObjectsList.Clear();
            }
          }
          return Entrypoints.SCRIPT_HANDLED;
        }
      }
      return Entrypoints.SCRIPT_HANDLED;
    }

    private static void RefreshQBS(uint oidSelf, int feat)
    {
      string sQuickBar = NWNX.Creature.SerializeQuickbar(oidSelf);
      QuickBarSlot emptyQBS = QuickBarSlot.CreateEmptyQBS();

      for (int i = 0; i < 36; i++)
      {
        QuickBarSlot qbs = NWNX.Player.GetQuickBarSlot(oidSelf, i);

        if (qbs.ObjectType == QuickBarSlotType.Feat && qbs.INTParam1 == feat)
        {
          NWNX.Player.SetQuickBarSlot(oidSelf, i, emptyQBS);
        }
      }

      NWNX.Creature.DeserializeQuickbar(oidSelf, sQuickBar);
    }

    private static void RefreshQBS(uint oidSelf)
    {
      string sQuickBar = NWNX.Creature.SerializeQuickbar(oidSelf);
      QuickBarSlot emptyQBS = QuickBarSlot.CreateEmptyQBS();

      for (int i = 0; i < 36; i++)
      {
        QuickBarSlot qbs = NWNX.Player.GetQuickBarSlot(oidSelf, i);
        NWNX.Player.SetQuickBarSlot(oidSelf, i, emptyQBS);
      }

      NWNX.Creature.DeserializeQuickbar(oidSelf, sQuickBar);
    }

    private static int HandleAutoSpell(uint oidSelf) //Je garde ça sous la main, mais je pense que le gérer différement serait mieux, notamment en créant un mode activable "autospell" en don gratuit pour les casters. Donc : A RETRAVAILLER 
    {
      Player oPC;

      if (Players.TryGetValue(oidSelf, out oPC))
      {
        var oTarget = NWNX.Object.StringToObject(NWNX.Events.GetEventData("TARGET"));

        if (oTarget.AsObject().IsValid)
        {
          NWScript.ClearAllActions();
          if (oPC.AutoAttackTarget == NWObject.OBJECT_INVALID)
          {
            oidSelf.AsPlayer().CastSpellAtObject(Spell.RayOfFrost, oTarget);
            NWScript.DelayCommand(6.0f, () => oPC.OnFrostAutoAttackTimedEvent());
          }
        }

        oPC.AutoAttackTarget = oTarget;
      }

      return Entrypoints.SCRIPT_HANDLED;
    }

    private static int HandleOnSpellCast(uint oidSelf)
    {
      Player oPC;

      if (Players.TryGetValue(oidSelf, out oPC))
      {
        var spellId = int.Parse(NWNX.Events.GetEventData("SPELL_ID"));

        if (spellId != (int)Spell.RayOfFrost)
          oPC.AutoAttackTarget = NWObject.OBJECT_INVALID;
      }

      return Entrypoints.SCRIPT_HANDLED;
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
          if (!oPerceived.IsPC || oPerceived.IsDM || oPerceived.IsDMPossessed || oPerceived.DisguiseName.Length == 0)
            return Entrypoints.SCRIPT_HANDLED;

          if (!oPC.DisguiseDetectTimer.ContainsKey(oPC) || (DateTime.Now - oPC.DisguiseDetectTimer[oPerceived]).TotalSeconds > 1800)
          {
            oPC.DisguiseDetectTimer[oPerceived] = DateTime.Now;

            int[] iPCSenseSkill = { oPC.GetSkillRank(Skill.Listen), oPC.GetSkillRank(Skill.Search), oPC.GetSkillRank(Skill.Spot),
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
              oPC.SendMessage(oPerceived.Name + " fait usage d'un déguisement ! Sous le masque, vous reconnaissez " + NWScript.GetName(oPerceived, true));
              //NWNX_Rename_ClearPCNameOverride(oPerceived, oPC);
            }
          }
        }
      }

      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleAfterAddSummon(uint oidSelf)
    {
      //Pas méga utile dans l'immédiat, mais pourra être utilisé pour gérer les invocations de façon plus fine plus tard
      // TODO : Système de possession d'invocations, compagnons animaux, etc (mais attention, vérifier que si le PJ déco en possession, ça n'écrase pas son .bic. Si oui, sauvegarde le PJ avant possession et ne plus sauvegarder le PJ en mode possession)
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        NWCreature oSummon = (NWNX.Object.StringToObject(NWNX.Events.GetEventData("ASSOCIATE_OBJECT_ID"))).AsCreature();

        if (oSummon.IsValid)
        {
          player.Summons.Add(oSummon, oSummon);
        }
      }

      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleAfterRemoveSummon(uint oidSelf)
    {
      //Pas méga utile dans l'immédiat, mais pourra être utilisé pour gérer les invocations de façon plus fine plus tard
      // TODO : Système de possession d'invocations, compagnons animaux, etc (mais attention, vérifier que si le PJ déco en possession, ça n'écrase pas son .bic. Si oui, sauvegarde le PJ avant possession et ne plus sauvegarder le PJ en mode possession)
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        NWCreature oSummon = (NWNX.Object.StringToObject(NWNX.Events.GetEventData("ASSOCIATE_OBJECT_ID"))).AsCreature();

        if (oSummon.IsValid)
        {
          player.Summons.Remove(oSummon);
        }
      }

      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleOnCombatMode(uint oidSelf)
    { 
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
          if (NWScript.GetLocalInt(player, "_ACTIVATED_TAUNT") != 0) // Permet de conserver sa posture de combat après avoir utilisé taunt
          {
            NWNX.Events.SkipEvent();
            NWScript.DeleteLocalInt(player, "_ACTIVATED_TAUNT");
          }
      }

      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleOnSkillUsed(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
          if (int.Parse(NWNX.Events.GetEventData("SKILL_ID")) == (int)Skill.Taunt)
          {
            NWScript.SetLocalInt(player, "_ACTIVATED_TAUNT", 1);
            NWScript.DelayCommand(12.0f, () => NWScript.DeleteLocalInt(player, "_ACTIVATED_TAUNT"));
          }
          else if (int.Parse(NWNX.Events.GetEventData("SKILL_ID")) == (int)Skill.PickPocket)
          {
            NWNX.Events.SkipEvent();
            NWPlayer oObject = NWNX.Object.StringToObject(NWNX.Events.GetEventData("TARGET_OBJECT_ID")).AsPlayer();
            Player oTarget;
            if (Players.TryGetValue(oObject, out oTarget) && !oTarget.IsDM && !oTarget.IsDMPossessed)
            {
              if (!oTarget.PickpocketDetectTimer.ContainsKey(player) || (DateTime.Now - oTarget.PickpocketDetectTimer[player]).TotalSeconds > 86400)
              {
                  oTarget.PickpocketDetectTimer.Add(player, DateTime.Now);

                  NWNX.Feedback.SetFeedbackMessageHidden(FeedbackMessageTypes.CombatTouchAttack, 1, oTarget);
                  NWScript.DelayCommand(2.0f, () => NWNX.Feedback.SetFeedbackMessageHidden(FeedbackMessageTypes.CombatTouchAttack, 0, oTarget));

                  int iRandom = Utils.random.Next(21);
                  int iVol = NWScript.GetSkillRank(Skill.PickPocket, player);
                  int iSpot = Utils.random.Next(21) + NWScript.GetSkillRank(Skill.Spot, player);
                  if ((iRandom + iVol) > iSpot)
                  {
                    NWNX.Chat.SendMessage((int)NWNX.Enum.ChatChannel.PlayerTalk, $"Vous faites un jet de Vol à la tire, le résultat est de : {iRandom} + {iVol} = {iRandom + iVol}.", player, player);
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

      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleAfterDetection(uint oidSelf)
    {
      Player oPC;
      if (Players.TryGetValue(oidSelf, out oPC))
      {
        var oTarget = NWNX.Object.StringToObject(NWNX.Events.GetEventData("TARGET")).AsCreature();

        if (oTarget.IsPC || oTarget.IsPossessedFamiliar)
        {
          if (!oTarget.GetObjectSeen(oPC) && NWScript.GetDistanceBetween(oTarget, oPC) < 20.0f)
          {
            int iDetectMode = (int)NWScript.GetDetectMode(oPC);
            if (int.Parse(NWNX.Events.GetEventData("TARGET_INVISIBLE")) == 1 && iDetectMode > 0)
            {
              switch (NWNX.Creature.GetMovementType(oTarget))
              {
                case MovementType.WalkBackwards:
                case MovementType.Sidestep:
                case MovementType.Walk:

                  if (!oPC.InviDetectTimer.ContainsKey(oTarget) || (DateTime.Now - oPC.InviDetectTimer[oTarget]).TotalSeconds > 6)
                  {
                    int iMoveSilentlyCheck = NWScript.GetSkillRank(Skill.MoveSilently, oTarget) + Utils.random.Next(21) + (int)NWScript.GetDistanceBetween(oTarget, oPC);
                    int iListenCheck = NWScript.GetSkillRank(Skill.Listen, oPC) + Utils.random.Next(21);
                    if (iDetectMode == 2)
                      iListenCheck -= 10;

                    if (iListenCheck > iMoveSilentlyCheck)
                    {
                      oPC.SendMessage("Vous entendez quelqu'un se faufiler dans les environs.");
                      NWNX.Player.ShowVisualEffect(oPC, (int)Flashier.Vfx_Fnf_Smoke_Puff, oTarget.Position);
                      oPC.InviDetectTimer.Add(oTarget, DateTime.Now);
                      oPC.InviEffectDetectTimer.Add(oTarget, DateTime.Now);
                    }
                    else
                      oPC.InviDetectTimer.Remove(oTarget);
                  }
                  else if ((DateTime.Now - oPC.InviEffectDetectTimer[oTarget]).TotalSeconds > 1)
                  {
                    NWNX.Player.ShowVisualEffect(oPC, (int)Flashier.Vfx_Fnf_Smoke_Puff, oTarget.Position);
                    oPC.InviEffectDetectTimer.Add(oTarget, DateTime.Now);
                  }
                  break;
                case MovementType.Run:

                  if (!oPC.InviDetectTimer.ContainsKey(oTarget) || (DateTime.Now - oPC.InviDetectTimer[oTarget]).TotalSeconds > 6)
                  {
                    oPC.SendMessage("Vous entendez quelqu'un courir peu discrètement dans les environs.");
                    NWNX.Player.ShowVisualEffect(oPC, (int)Flashier.Vfx_Fnf_Smoke_Puff, oTarget.Position);
                    oPC.InviDetectTimer.Add(oTarget, DateTime.Now);
                    oPC.InviEffectDetectTimer.Add(oTarget, DateTime.Now);
                  }
                  else if ((DateTime.Now - oPC.InviEffectDetectTimer[oTarget]).TotalSeconds > 1)
                  {
                    NWNX.Player.ShowVisualEffect(oPC, (int)Flashier.Vfx_Fnf_Smoke_Puff, oTarget.Position);
                    oPC.InviEffectDetectTimer.Add(oTarget, DateTime.Now);
                  }
                  break;
              }
            }
          }
        }
      }

      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandlePlayerDeath(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(NWScript.GetLastPlayerDied(), out player))
      {
        player.SendMessage("Tout se brouille autour de vous. Avant de perdre connaissance, vous sentez comme un étrange maëlstrom vous aspirer.");

        NWPlaceable oPCCorpse = NWScript.CreateObject(ObjectType.Placeable, "pccorpse", player.Location).AsPlaceable();
        NWNX.Events.AddObjectToDispatchList("NWNX_ON_INVENTORY_REMOVE_ITEM_AFTER", "event_inventory_remove_item_after", oPCCorpse);
        NWNX.Events.AddObjectToDispatchList("NWNX_ON_INVENTORY_ADD_ITEM_AFTER", "event_inventory_add_item_after", oPCCorpse);

        int PlayerId = NWNX.Object.GetInt(player, "_PC_ID");
        oPCCorpse.Name = $"Cadavre de {player.Name}";
        oPCCorpse.Description = $"Cadavre de {player.Name}";
        oPCCorpse.Locals.Int.Set("_PC_ID", PlayerId);

        NWScript.SetLocalInt(NWScript.CreateItemOnObject("item_pccorpse", oPCCorpse), "PC_ID", PlayerId);

        if (player.Gold > 0)
          NWScript.CreateItemOnObject("nw_it_gold001", oPCCorpse, player.Gold); // TODO : penser à modifier la valeur row 76 of baseitems.2da afin de permettre à l'or de stack à plus de 50K unités

        // TODO : Dropper toutes les ressources craft de l'inventaire du défunt

        // TODO : Enregistrer l'objet serialized cadavre en BDD pour restauration après reboot + IdPJ + Location

        NWScript.DelayCommand(5.0f, () => player.SendToLimbo());
      }

      return Entrypoints.SCRIPT_HANDLED;
    }

    private static int HandleAfterDMJumpTarget(uint oidSelf)
    {
      var oTarget = NWNX.Object.StringToObject(NWNX.Events.GetEventData("TARGET_1"));

      Player player;
      if (Players.TryGetValue(oTarget, out player))
      {
        if(player.Area.Tag == "Labrume")
        {
          player.DestroyCorpses();
        }
      }

      return Entrypoints.SCRIPT_HANDLED;
    }
    public static PlayerSystem.Player GetPCById(int PcId)
    {
      foreach (KeyValuePair<uint, PlayerSystem.Player> PlayerListEntry in PlayerSystem.Players)
      {
        if(NWNX.Object.GetInt(PlayerListEntry.Key, "_PC_ID") == PcId)
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
        NWCreature oTarget = NWNX.Object.StringToObject(NWNX.Events.GetEventData("TARGET_OBJECT_ID")).AsCreature();

        if (Spells.GetHasEffect(NWScript.GetEffectType(NWScript.EffectCutsceneGhost()), player))
          Spells.RemoveEffectOfType(NWScript.GetEffectType(NWScript.EffectCutsceneGhost()), player);          

        if (oTarget.IsPC)
        {
          NWScript.SetPCDislike(player, oTarget);
          if (Spells.GetHasEffect(NWScript.GetEffectType(NWScript.EffectCutsceneGhost()), oTarget))
            Spells.RemoveEffectOfType(NWScript.GetEffectType(NWScript.EffectCutsceneGhost()), oTarget);
        }
      }

      return Entrypoints.SCRIPT_HANDLED;
    }
  }
}
