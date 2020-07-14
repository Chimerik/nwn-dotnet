using System;
using System.Collections.Generic;
using Dapper;
using NWN.Enums;
using NWN.NWNX.Enum;

namespace NWN.Systems
{
  public static partial class PlayerSystem
  {
    public const string ON_PC_KEYSTROKE_SCRIPT = "on_pc_keystroke";

    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
        {
            { "on_pc_connect", HandlePlayerConnect },
            { "on_pc_disconnect", HandlePlayerDisconnect },
            { "connexion", HandlePlayerBeforeDisconnect },
            { ON_PC_KEYSTROKE_SCRIPT, HandlePlayerKeystroke },
            { "event_dm_actions", HandleDMActions },
            { "event_mv_plc", HandleMovePlaceable },
            { "event_feat_used", HandleFeatUsed },
            { "event_auto_spell", HandleAutoSpell },
            { "_onspellcast", HandleOnSpellCast },
        };

    public static Dictionary<uint, Player> Players = new Dictionary<uint, Player>();

    private static int HandlePlayerConnect(uint oidSelf)
    {
      var oPC = NWScript.GetEnteringObject();
      NWNX.Events.AddObjectToDispatchList(NWNX.Events.ON_INPUT_KEYBOARD_BEFORE, ON_PC_KEYSTROKE_SCRIPT, oPC);
      NWNX.Events.AddObjectToDispatchList("NWNX_ON_USE_FEAT_BEFORE", "event_feat_used", oPC);
      NWNX.Events.AddObjectToDispatchList("NWNX_ON_ADD_ASSOCIATE_AFTER", "summon", oPC);
      NWNX.Events.AddObjectToDispatchList("NWNX_ON_REMOVE_ASSOCIATE_AFTER", "summon", oPC);

      oPC.AsCreature().AddFeat(NWN.Enums.Feat.PlayerTool01);

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

      var player = new Player(oPC);
      Players.Add(oPC, player);

      if(player.IsNewPlayer)
      {
        // TODO : création des infos du nouveau joueur en BDD
      }
      else
      {
        // TODO : Initilisation de l'ancien joueur avec les infos en BDD

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
      }

      //Appliquer la distance de perception du chat en fonction de la compétence Listen du joueur
      NWNX.Chat.SetChatHearingDistance(NWNX.Chat.GetChatHearingDistance(oPC.AsObject(), NWNX.Enum.ChatChannel.PlayerTalk) + NWScript.GetSkillRank(Skill.Listen, oPC) / 5, oPC.AsObject(), NWNX.Enum.ChatChannel.PlayerTalk);
      NWNX.Chat.SetChatHearingDistance(NWNX.Chat.GetChatHearingDistance(oPC.AsObject(), NWNX.Enum.ChatChannel.PlayerWhisper) + NWScript.GetSkillRank(Skill.Listen, oPC) / 10, oPC.AsObject(), NWNX.Enum.ChatChannel.PlayerWhisper);

      return Entrypoints.SCRIPT_HANDLED;
    }

    private static int HandlePlayerDisconnect(uint oidSelf)
    {
      var oPC = NWScript.GetExitingObject();
      NWNX.Events.RemoveObjectFromDispatchList(NWNX.Events.ON_INPUT_KEYBOARD_BEFORE, ON_PC_KEYSTROKE_SCRIPT, oPC);
      Players.Remove(oPC);

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

    private static int HandleDMActions(uint oidSelf)
    {
      NWPlayer oPC = oidSelf.AsPlayer();
      string current_event = NWNX.Events.GetCurrentEvent();

      /* Fix polymorph bug : Lorsqu'un PJ métamorphosé est sauvegardé, toutes ses buffs sont supprimées afin que les stats de 
       * la nouvelle forme ne remplace pas celles du PJ dans son fichier .bic. Après sauvegarde, les stats de la métamorphose 
       * sont réappliquées. 
       * Bug 1 : les PV temporaires de la forme se cumulent avec chaque sauvegarde, ce qui permet d'avoir PV infinis
       * BUG 2 : Les buffs ne faisant pas partie de la métamorphose (appliquées par sort par exemple), ne sont pas réappliquées
       * Ici, la correction consiste à ne pas sauvegarder le PJ s'il est métamorphosé, sauf s'il s'agit d'une déconnexion.
       * Mais il se peut que dans ce cas, ses buffs soient perdues à la reco. A vérifier. Si c'est le cas, une meilleure
       * correction pourrait être de parcourir tous ses buffs et de les réappliquer dans l'event AFTER de la sauvegarde*/
      if (current_event == "NWNX_ON_CLIENT_EXPORT_CHARACTER_BEFORE" || current_event == "NWNX_ON_SERVER_CHARACTER_SAVE_BEFORE")
      {
        if (oPC.Locals.Int.Get("_IS_DISCONNECTING") == 0)
        {
          if (oPC.HasAnyEffect((int)EffectTypeEngine.Polymorph))
          {
            NWNX.Events.SkipEvent();
            return Entrypoints.SCRIPT_HANDLED;
          }
        }
        else
          oPC.Locals.Int.Delete("_IS_DISCONNECTING");
      }
      return Entrypoints.SCRIPT_NOT_HANDLED;
    }

    private static int HandlePlayerBeforeDisconnect(uint oidSelf)
    {
      string current_event = NWNX.Events.GetCurrentEvent();

      if (current_event == "NWNX_ON_CLIENT_DISCONNECT_BEFORE")
      {
        oidSelf.AsObject().Locals.Int.Set("_IS_DISCONNECTING", 1);
        NWNX.Object.SetInt(oidSelf, "_CURRENT_HP", NWScript.GetCurrentHitPoints(oidSelf), true);
        NWNX.Object.SetString(oidSelf, "_LOCATION", Utils.LocationToString(NWScript.GetLocation(oidSelf)), true);
      }
      return Entrypoints.SCRIPT_NOT_HANDLED;
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
              Utils.Meuble result;
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
              }
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
  }
}
