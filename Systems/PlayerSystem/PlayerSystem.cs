using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.Blueprint;
using static NWN.Systems.LootSystem;
using static NWN.Systems.SkillSystem;
using Discord;

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
            { "_onspellcast_before", HandleBeforeSpellCast },
            { "_onspellcast_after", HandleAfterSpellCast },
            { "event_learn_scroll_before", HandleBeforeLearnScroll },
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
            { "pc_acquire_it", HandlePCAcquireItem },
            { "pc_unacquire_it", HandlePCUnacquireItem },
            { "event_on_journal_open", HandlePCJournalOpen },
            { "event_on_journal_close", HandlePCJournalClose },
            { "before_store_buy", HandleBeforeStoreBuy },
            { "before_store_sell", HandleBeforeStoreSell },
            { "event_spacebar_down", HandleSpacebarDown },
            { "map_pin_added", HandleAddMapPin },
            { "map_pin_changed", HandleChangeMapPin },
            { "map_pin_destroyed", HandleDestroyMapPin },
            //{ "event_has_feat", HandleAfterHasFeat },
           // { "before_reputation_change", HandleBeforeReputationChange },
        }; 

    public static Dictionary<uint, Player> Players = new Dictionary<uint, Player>();
    public static int[] plageCrittersAppearances = new int[] { 1984, 1985, 1986, 3160, 3155, 3156, 1956, 1957, 1958, 1959, 1960, 1961, 1962, 291, 292, 1964, 4310, 1428, 1430, 1980, 1981, 3261, 3262, 3263};
    public static int[] caveCrittersAppearances = new int[] { 3197, 3198, 3199, 3200, 3202, 3204, 3205, 3206, 3207, 3208, 3209, 3210, 3999, 6425, 6426, 6427, 6428, 6429, 6430, 6431, 6432, 6433, 6434, 6435, 6436, 3397, 3398, 3400, 3434};
    public static int[] cityCrittersAppearances = new int[] { 1983, 1390, 1391, 1392, 1393, 1394, 1395, 1396, 1397, 1398, 1399, 1400, 1401, 1402, 1403, 1404, 1405, 1406, 1407, 1408, 1409, 1410, 1411, 4385, 4408, 4112, 4113, 2505};
    public static int[] CivilianAppearances = new int[] { 220, 221, 222, 224, 225, 226, 227, 228, 229, 231, 4357, 4358, 238, 239, 240, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 278, 279, 280, 281, 282, 283, 284, 212, 4379 };
    public static int[] genericCrittersAppearances = new int[] { 3259, 1181, 1182, 1183, 1184, 4370, 4371, 1794, 1988, 3213, 3214, 3215, 3216, 3222, 3223, 1339, 3445, 3520, 4338, 1335, 1336, 1966, 1967, 1968, 1969, 1970, 1971, 1972, 4221, 1025, 1026, 1797, 3237, 3238, 3239 , 3240, 3241, 1328, 1941, 1330, 1438, 496, 509, 522, 535, 1784, 1785, 1787, 1788, 1789, 1791, 1855, 1856, 1857, 1858, 1859, 1860, 2589, 1334, 1973, 1974, 4309, 4310, 4320, 4321, 4322, 34, 142, 1796, 1340, 3192, 3193, 3194, 3195, 3196, 1341, 1005, 1006, 1007, 1008, 1009, 1010, 1011, 1013, 1014, 1015, 1016, 1017, 1019, 1020, 1255, 3152, 3153, 3157, 3158, 3159, 3161, 3162, 3163, 3164, 3165, 3166, 3167, 3168, 3154, 3148, 3149, 1975, 1976, 1977, 1978, 1979, 2506, 3043, 3044, 3045, 3046, 3047, 1275, 1947, 1949, 1950, 1951, 1952, 6365, 6408, 31, 145, 144, 3305, 4364, 1982, 1749, 1750, 1751, 1332, 1333, 1987, 1863, 1337, 1295, 1329, 3310, 3311, 1802, 1803, 1804, 1805, 8, 35, 37, 4115, 4116, 4117, 4118, 4119, 4120, 4121, 4122, 4123, 3138, 1338 };
   /* private static int HandleAfterHasFeat(uint oidSelf)
    {
      Console.WriteLine($"feat : {NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("feat", "FEAT", int.Parse(EventsPlugin.GetEventData("FEAT_ID")))))} : {EventsPlugin.GetEventData("HAS_FEAT")}"); 
      return 0;
    }*/
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

        player.UnloadMenuQuickbar();
        NWScript.SetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_X, 0.0f);
        NWScript.SetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_X, 0.0f);
        NWScript.SetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y, 0.0f);
        NWScript.SetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Z, 0.0f);
        player.setValue = 0;
        player.OnKeydown -= player.menu.HandleMenuFeatUsed;

        HandleBeforePartyLeave(oidSelf);
        HandleAfterPartyLeave(oidSelf);

        if(NWScript.GetTag(NWScript.GetArea(player.oid)) == $"entrepotpersonnel_{NWScript.GetName(player.oid)}")
        {
          uint storageToSave = NWScript.GetFirstObjectInArea(NWScript.GetArea(player.oid));
          if (NWScript.GetTag(storageToSave) != "ps_entrepot")
            storageToSave = NWScript.GetNearestObjectByTag("ps_entrepot", storageToSave);

          var saveStorage = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, 
            $"UPDATE playerCharacters set storage = @storage where rowid = @characterId");
          NWScript.SqlBindInt(saveStorage, "@characterId", player.characterId);
          NWScript.SqlBindObject(saveStorage, "@storage", storageToSave);
          NWScript.SqlStep(saveStorage);

          player.location = NWScript.GetLocation(NWScript.GetObjectByTag("portal_storage_in"));
        }
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
        case Feat.LanguageGnome:
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
        case Feat.CustomPositionRight:
        case Feat.CustomPositionLeft:
        case Feat.CustomPositionForward:
        case Feat.CustomPositionBackward:
        case Feat.CustomPositionRotateRight:
        case Feat.CustomPositionRotateLeft:

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

    private static int HandleBeforeSpellCast(uint oidSelf)
    {
      Player oPC;

      if (Players.TryGetValue(oidSelf, out oPC))
      {
        var spellId = int.Parse(EventsPlugin.GetEventData("SPELL_ID"));

        //NWScript.SendMessageToPC(oidSelf, "before spell cast");

        int classe;
        if(int.TryParse(NWScript.Get2DAString("spells", "Bard", spellId), out classe))
          classe = NWScript.CLASS_TYPE_BARD;

        if (int.TryParse(NWScript.Get2DAString("spells", "Ranger", spellId), out classe))
          classe = NWScript.CLASS_TYPE_RANGER;

        if (int.TryParse(NWScript.Get2DAString("spells", "Paladin", spellId), out classe))
          classe = NWScript.CLASS_TYPE_PALADIN;

        if (int.TryParse(NWScript.Get2DAString("spells", "Druid", spellId), out classe))
          classe = NWScript.CLASS_TYPE_DRUID;

        if (int.TryParse(NWScript.Get2DAString("spells", "Cleric", spellId), out classe))
          classe = NWScript.CLASS_TYPE_CLERIC;
 
        CreaturePlugin.SetClassByPosition(oidSelf, 0, classe);
      }

      return 0;
    }
    private static int HandleAfterSpellCast(uint oidSelf)
    {
      Player player;

      if (Players.TryGetValue(oidSelf, out player))
      {
        var spellId = int.Parse(EventsPlugin.GetEventData("SPELL_ID"));
        //NWScript.SendMessageToPC(oidSelf, "after spell cast");
        CreaturePlugin.SetClassByPosition(oidSelf, 0, 43); // 43 = aventurier

        if (NWScript.Get2DAString("spells", "School", spellId) == "D" && NWScript.GetTag(NWScript.GetItemInSlot(NWScript.INVENTORY_SLOT_NECK, player.oid)) == "amulettorillink")
        {
          (Bot._client.GetChannel(680072044364562532) as IMessageChannel).SendMessageAsync(
            $"{Bot._client.GetGuild(680072044364562528).EveryoneRole.Mention} {NWScript.GetName(player.oid)} " +
            $"vient de lancer un sort de divination ({NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("spells", "Name", spellId)))})" +
            $" en portant l'amulette de traçage. L'Amiral s'apprête à punir l'impudent !");
        }
      }

      return 0;
    }
    private static int HandleBeforeLearnScroll(uint oidSelf)
    {
      Player player;

      if (Players.TryGetValue(oidSelf, out player))
      {
        EventsPlugin.SkipEvent();

        var oScroll = NWScript.StringToObject(EventsPlugin.GetEventData("SCROLL"));
        int spellId = Spells.GetSpellIDFromScroll(oScroll);
        int spellLevel = Spells.GetSpellLevelFromScroll(oScroll);

        if(spellId < 0 || spellLevel < 0)
        {
          Utils.LogMessageToDMs($"LEARN SPELL FROM SCROLL - Player : {NWScript.GetName(player.oid)}, SpellId : {spellId}, SpellLevel : {spellLevel} - INVALID");
          NWScript.SendMessageToPC(player.oid, "HRP - Ce parchemin ne semble pas correctement configuré, impossible d'en apprendre quoique ce soit. Le staff a été informé du problème.");
          return 0;
        }

        int knownSpellCount = CreaturePlugin.GetKnownSpellCount(player.oid, 43, spellLevel);
        
        if (knownSpellCount > 0)
          for (int i = 0; i < knownSpellCount; i++)
            if (CreaturePlugin.GetKnownSpell(player.oid, 43, spellLevel, i) == spellId)
            {
              NWScript.SendMessageToPC(player.oid, "Ce sort est déjà inscrit dans votre grimoire.");
              return 0;
            }

        if (player.learnableSpells.ContainsKey(spellId))
        {
          NWScript.SendMessageToPC(player.oid, "Ce sort se trouve déjà dans votre liste d'apprentissage.");
          return 0;
        }
        else
        {
          LearnableSpell spell = new SkillSystem.LearnableSpell(spellId, 0, player);
          player.learnableSpells.Add(spellId, spell);
          NWScript.SendMessageToPC(player.oid, $"Le sort {spell.name} a été ajouté à votre liste d'apprentissage et est désormais disponible pour étude.");
          NWScript.DestroyObject(oScroll);
        }
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

            if (CollectSystem.blueprintDictionnary.ContainsKey(baseItemType))
              NWScript.SetDescription(examineTarget, CollectSystem.blueprintDictionnary[baseItemType].DisplayBlueprintInfo(player, examineTarget));
            else
            {
              NWScript.SendMessageToPC(oidSelf, "[ERREUR HRP] - Le patron utilisé n'est pas correctement initialisé. Le bug a été remonté au staff.");
              Utils.LogMessageToDMs($"Blueprint Invalid : {NWScript.GetName(examineTarget)} - Base Item Type : {baseItemType} - Examined by : {NWScript.GetName(oidSelf)}");
            }
              break;
          case "ore":
            string reprocessingData = $"{NWScript.GetName(examineTarget)} : Efficacité raffinage -30 % (base fonderie Amirauté)";

            int value;
            if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Reprocessing)), out value))
              reprocessingData += $"\n x1.{3 * value} (Raffinage)";

            if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.ReprocessingEfficiency)), out value))
              reprocessingData += $"\n x1.{2 * value} (Raffinage efficace)";

            CollectSystem.Ore processedOre;
            if (CollectSystem.oresDictionnary.TryGetValue(CollectSystem.GetOreTypeFromName(NWScript.GetName(examineTarget)), out processedOre))
              if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)processedOre.feat)), out value))
                reprocessingData += $"\n x1.{2 * value} (Raffinage {NWScript.GetName(examineTarget)})";

            float connectionsLevel;
            if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Connections)), out connectionsLevel))
              reprocessingData += $"\n x{1.00 - connectionsLevel / 100} (Raffinage {NWScript.GetName(examineTarget)})";

            NWScript.SetDescription(examineTarget, reprocessingData);
            break;
          case "refinery":
            string descriptionBrut = "Stock actuel de minerai brut : \n\n\n";
            foreach (KeyValuePair<string, int> stockEntry in player.materialStock)
              if(CollectSystem.GetOreTypeFromName(stockEntry.Key) != CollectSystem.OreType.Invalid)
                descriptionBrut += $"{stockEntry.Key} : {stockEntry.Value} unité(s).\n";
                
            NWScript.SetDescription(examineTarget, descriptionBrut);
            break;
          case "forge":
            string descriptionRefined = "Stock actuel de minerai raffiné : \n\n\n";
            foreach (KeyValuePair<string, int> stockEntry in player.materialStock)
              if (CollectSystem.GetOreTypeFromName(stockEntry.Key) != CollectSystem.OreType.Invalid)
                descriptionRefined += $"{stockEntry.Key} : {stockEntry.Value} unité(s).\n";

            NWScript.SetDescription(examineTarget, descriptionRefined);
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
          case "blueprint":
          case "ore":
          case "refinery":
          case "forge":
            NWScript.SetDescription(examineTarget, $"");
            break;
        }
      }
      return 0;
    }
    private static int HandlePCUnacquireItem(uint oidSelf)
    {
      uint oPC = NWScript.GetModuleItemLostBy();

      if (Convert.ToBoolean(NWScript.GetIsPC(oPC)))
      {
        var oItem = NWScript.GetModuleItemLost();
        var oGivenTo = NWScript.GetItemPossessor(oItem);

        if (Convert.ToBoolean(NWScript.GetIsObjectValid(oItem)))
        {
          if (NWScript.GetTag(oItem) == "item_pccorpse" && oGivenTo == NWScript.OBJECT_INVALID) // signifie que l'item a été drop au sol et pas donné à un autre PJ ou mis dans un placeable
          {
            uint oCorpse = ObjectPlugin.Deserialize(NWScript.GetLocalString(oItem, "_SERIALIZED_CORPSE"));
            ObjectPlugin.AddToArea(oCorpse, NWScript.GetArea(oItem), NWScript.GetPosition(oItem));
            Utils.DestroyInventory(oCorpse);
            ObjectPlugin.AcquireItem(oCorpse, oItem);
            VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, oCorpse, VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);
            SetupPCCorpse(oCorpse);
            //NWScript.DelayCommand(1.3f, () => ObjectPlugin.AcquireItem(NWScript.GetNearestObjectByTag("pccorpse_bodybag", oCorpse), oItem));

            var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"SELECT characterName from playerCharacters where rowid = @characterId");
            NWScript.SqlBindInt(query, "@characterId", NWScript.GetLocalInt(oItem, "_PC_ID"));
            NWScript.SqlStep(query);

            SavePlayerCorpseToDatabase(NWScript.GetLocalInt(oItem, "_PC_ID"), oCorpse, NWScript.GetTag(NWScript.GetArea(oCorpse)), NWScript.GetPosition(oCorpse));
          }
          /* En pause jusqu'à ce que le système de transport soit en place
          if (NWScript.GetMovementRate(oPC) == CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE)
            if (NWScript.GetWeight(oPC) <= int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", NWScript.GetAbilityScore(oPC, NWScript.ABILITY_STRENGTH))))
              CreaturePlugin.SetMovementRate(oPC, CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_PC);*/
        }
      }

      return 0;
    }
    private static int HandlePCAcquireItem(uint oidSelf)
    {
      var oPC = NWScript.GetModuleItemAcquiredBy();
      //Console.WriteLine(NWScript.GetName(oPC));
      //Console.WriteLine(NWScript.GetName(NWScript.GetArea(oPC)));

      if (Convert.ToBoolean(NWScript.GetIsPC(oPC)))
      {
        var oItem = NWScript.GetModuleItemAcquired();
        var oAcquiredFrom = NWScript.GetModuleItemAcquiredFrom();

        if (Convert.ToBoolean(NWScript.GetIsObjectValid(oItem)))
        {
          if (NWScript.GetTag(oItem) == "item_pccorpse" && NWScript.GetTag(oAcquiredFrom) == "pccorpse_bodybag")
          {
            DeletePlayerCorpseFromDatabase(NWScript.GetLocalInt(oItem, "_PC_ID"));

            var oCorpse = NWScript.GetObjectByTag("pccorpse");
            int corpseId = NWScript.GetLocalInt(oItem, "_PC_ID");
            int i = 1;
            while (Convert.ToBoolean(NWScript.GetIsObjectValid(oCorpse)))
            {
              if (corpseId == NWScript.GetLocalInt(oCorpse, "_PC_ID"))
              {
                NWScript.DestroyObject(oCorpse);
                NWScript.DestroyObject(oAcquiredFrom);
                break;
              }
              oCorpse = NWScript.GetObjectByTag("pccorpse", i++);
            }

          }
          /*En pause jusqu'à ce que le système de transport soit en place
          if (NWScript.GetMovementRate(oPC) != CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE)
            if (NWScript.GetWeight(oPC) > int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", NWScript.GetAbilityScore(oPC, NWScript.ABILITY_STRENGTH))))
              CreaturePlugin.SetMovementRate(oPC, CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE);*/
        }
      }
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
          if (NWScript.GetTag(firstObject) == "creature_spawn")
            HandleSpawnWaypoint(firstObject, 0);

          int i = 1;
          var spawnPoints = NWScript.GetNearestObjectByTag("creature_spawn", firstObject);

          while (Convert.ToBoolean(NWScript.GetIsObjectValid(spawnPoints)))
          {
            HandleSpawnWaypoint(spawnPoints, i);
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
          if (Convert.ToBoolean(NWScript.GetLocalInt(oArea, "_REST")))
          {
            NWScript.ExploreAreaForPlayer(oArea, player.oid, 1);

            if (player.craftJob.IsActive() && player.playerJournal.craftJobCountDown == null)
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

    private static void HandleSpawnWaypoint(uint spawnPoint, int count)
    {
      switch(NWScript.GetLocalString(spawnPoint, "_SPAWN_TYPE"))
      {
        case "npc":
          if (!Convert.ToBoolean(NWScript.GetIsObjectValid(NWScript.GetNearestObjectByTag($"{ NWScript.GetLocalString(spawnPoint, "_CREATURE_TEMPLATE")}_NB_{count}", spawnPoint))))
          {
            SpawnCreatureFromSpawnPoint(spawnPoint, count);
          }
          break;
        case "civilian":
          SetRandomAppearanceFrom2da(SpawnCreatureFromSpawnPoint(spawnPoint), CivilianAppearances);
          break;
        case "critter":
          SetRandomAppearanceAndNameFrom2da(SpawnCreatureFromSpawnPoint(spawnPoint), genericCrittersAppearances);
          break;
        case "critter_plage":
          SetRandomAppearanceAndNameFrom2da(SpawnCreatureFromSpawnPoint(spawnPoint), plageCrittersAppearances);
          break;
        case "critter_cave":
          SetRandomAppearanceAndNameFrom2da(SpawnCreatureFromSpawnPoint(spawnPoint), caveCrittersAppearances);
          break;
        case "critter_city":
          SetRandomAppearanceAndNameFrom2da(SpawnCreatureFromSpawnPoint(spawnPoint), cityCrittersAppearances);
          break;
        default:
          SpawnCreatureFromSpawnPoint(spawnPoint);
          break;
      }
    }
    private static uint SpawnCreatureFromSpawnPoint(uint spawnPoint, int count = 0)
    {
      uint creature;
      if(NWScript.GetLocalString(spawnPoint, "_SPAWN_TYPE") == "npc")
        creature  = NWScript.CreateObject(NWScript.OBJECT_TYPE_CREATURE, NWScript.GetLocalString(spawnPoint, "_CREATURE_TEMPLATE"), NWScript.GetLocation(spawnPoint), 0, $"{NWScript.GetLocalString(spawnPoint, "_CREATURE_TEMPLATE")}_NB_{count}");
      else
        creature = NWScript.CreateObject(NWScript.OBJECT_TYPE_CREATURE, NWScript.GetLocalString(spawnPoint, "_CREATURE_TEMPLATE"), NWScript.GetLocation(spawnPoint));
      
      string tag = NWScript.GetTag(creature);
      if (tag.Contains("_NB_"))
        tag = tag.Remove(tag.IndexOf("_NB_"));

      switch (tag)
      {
        case "civilian":
        case "neutralcritter":
        case "ratgarou":
        case "wereboar":
        case "sim_wraith":
        case "marten_arc":
          break;
        default:
          NWScript.SetAILevel(creature, 1);
          NWScript.SetEventScript(creature, NWScript.EVENT_SCRIPT_CREATURE_ON_DEATH, ON_LOOT_SCRIPT);
          break;
      }
       
      return creature;
    }
    private static void SetRandomAppearanceAndNameFrom2da(uint creature, int[] appearanceArray)
    {
      int appearance = appearanceArray[Utils.random.Next(0, appearanceArray.Length)];
      NWScript.SetCreatureAppearanceType(creature, appearance);

      int value;
      if (Int32.TryParse(NWScript.Get2DAString("appearance", "STRING_REF", appearance), out value))
        NWScript.SetName(creature, NWScript.GetStringByStrRef(value));
      else
        Utils.LogMessageToDMs($"Apparence {appearance} - Nom non défini.");

      NWScript.SetPortraitResRef(creature, NWScript.Get2DAString("appearance", "PORTRAIT", appearance));

      NWScript.AssignCommand(creature, () => NWScript.ActionRandomWalk());      
    }
    private static void SetRandomAppearanceFrom2da(uint creature, int[] appearanceArray)
    {
      int appearance = appearanceArray[Utils.random.Next(0, appearanceArray.Length)];
      NWScript.SetCreatureAppearanceType(creature, appearance);
      NWScript.SetPortraitResRef(creature, NWScript.Get2DAString("appearance", "PORTRAIT", appearance));

      NWScript.AssignCommand(creature, () => NWScript.ActionRandomWalk());
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
    private static int HandleBeforeStoreBuy(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        uint item = NWScript.StringToObject(EventsPlugin.GetEventData("ITEM"));
        int price = Int32.Parse(EventsPlugin.GetEventData("PRICE"));
        int pocketGold = NWScript.GetGold(oidSelf);

        if (pocketGold < price)
        {
          if (pocketGold + player.bankGold < price)
          {
            CreaturePlugin.SetGold(oidSelf, 0);
            int bankGold = 0;

            if (player.bankGold > 0)
              bankGold = player.bankGold;

            int debt = price - (pocketGold + bankGold);
            player.bankGold -= debt;

            ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, $"Très bien, je demanderai à la banque de vous faire un crédit sur {debt}. N'oubliez pas que les intérêts sont de 30 % par semaine.",
            NWScript.GetLocalObject(NWScript.StringToObject(EventsPlugin.GetEventData("STORE")), "_STORE_NPC"), oidSelf);
          }
          else
          {
            CreaturePlugin.SetGold(oidSelf, 0);
            player.bankGold -= price - pocketGold;

            ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, "Très bien, je demanderai à la banque de prélever l'or sur votre compte.",
            NWScript.GetLocalObject(NWScript.StringToObject(EventsPlugin.GetEventData("STORE")), "_STORE_NPC"), oidSelf);
          }
        }
        else
          NWScript.TakeGoldFromCreature(price, player.oid, 1);

        EventsPlugin.SkipEvent();
        NWScript.CopyItem(item, player.oid, 1);

        string tag = NWScript.GetTag(NWScript.GetLocalObject(NWScript.StringToObject(EventsPlugin.GetEventData("STORE")), "_STORE_NPC"));
        if (tag.Contains("_NB_"))
          tag = tag.Remove(tag.IndexOf("_NB_"));

        switch (tag)
        {
          case "blacksmith":
          case "tribunal_hotesse":
            break;
          default:
            NWScript.DestroyObject(item);
            break;
        }
      }

      return 0;
    }
    private static int HandleBeforeStoreSell(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        EventsPlugin.SkipEvent();
        ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, "Navré, je n'achète rien. J'arrive déjà tout juste à m'acquiter de ma dette.",
            NWScript.GetLocalObject(NWScript.StringToObject(EventsPlugin.GetEventData("STORE")), "_STORE_NPC"), oidSelf);
      }

      return 0;
    }
    
    private static int HandleSpacebarDown(uint oidSelf)
    {
      NWScript.PostString(oidSelf, "", 40, 15, 0, 0.000001f, unchecked((int)0xFFFFFFFF), unchecked((int)0xFFFFFFFF), 9999, "fnt_my_gui");
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_INPUT_TOGGLE_PAUSE_BEFORE", "event_spacebar_down", oidSelf);

      return 0;
    }
    private static int HandleAddMapPin(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        int id = NWScript.GetLocalInt(player.oid, "NW_TOTAL_MAP_PINS");
        player.mapPinDictionnary.Add(NWScript.GetLocalInt(player.oid, "NW_TOTAL_MAP_PINS"), new MapPin(id, NWScript.GetTag(NWScript.GetArea(player.oid)), float.Parse(EventsPlugin.GetEventData("PIN_X")), float.Parse(EventsPlugin.GetEventData("PIN_Y")), EventsPlugin.GetEventData("PIN_NOTE")));
      }
      return 0;
    }
    private static int HandleDestroyMapPin(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        int pinId = Int32.Parse(EventsPlugin.GetEventData("PIN_ID"));
        player.mapPinDictionnary.Remove(pinId);

        var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, "DELETE FROM playerMapPins where characterId = @characterId and mapPinId = @mapPinId");
        NWScript.SqlBindInt(query, "@characterId", player.characterId);
        NWScript.SqlBindInt(query, "@mapPinId", pinId);
        NWScript.SqlStep(query);
      }

      return 0;
    }
    private static int HandleChangeMapPin(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        MapPin updatedMapPin = player.mapPinDictionnary[Int32.Parse(EventsPlugin.GetEventData("PIN_ID"))];
        updatedMapPin.x = float.Parse(EventsPlugin.GetEventData("PIN_X"));
        updatedMapPin.y = float.Parse(EventsPlugin.GetEventData("PIN_Y"));
        updatedMapPin.note = EventsPlugin.GetEventData("PIN_NOTE");
      }

      return 0;
    }
    /*private static int HandleBeforeReputationChange(uint oidSelf)
    {
      EventsPlugin.SkipEvent();
      NWScript.SendMessageToPC(NWScript.GetFirstPC(), "Reputation change skipped ?");
      return 0;
    }*/
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
