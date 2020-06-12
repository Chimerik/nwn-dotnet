using Dapper;
using NWN.Enums;
using NWN.Enums.Item;
using NWN.Enums.Item.Property;
using NWN.Enums.VisualEffect;
using NWN.NWNX;
using NWN.Systems;
using NWN.Systems.PostString;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NWN
{
  class ScriptHandlers
  {
    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
        {
            { "_onload", OnModuleLoad },
            { "cs_chatlistener", ChatListener },
            { "event_keyboard", EventKeyboard },
            { "X0_S0_AcidSplash", CantripsScaler },
            { "NW_S0_Daze", CantripsScaler },
            { "X0_S0_ElecJolt", CantripsScaler },
            { "X0_S0_Flare", CantripsScaler },
            { "NW_S0_Light", CantripsScaler },
            { "NW_S0_RayFrost", CantripsScaler },
            { "NW_S0_Resis", CantripsScaler },
            { "NW_S0_Virtue", CantripsScaler },
          //  { "event_mouse_clic", EventMouseClick },
            { "event_auto_spell", EventAutoSpell },
            { "_onspellcast", EventOnSpellCast },
            { "event_dm_actions", EventDMActions },
            { "event_mv_plc", EventMovePlaceable },
            { "event_feat_used", EventFeatUsed },
            { "connexion", EventPlayerConnexion },
            { "_onenter", OnEnter },
            { "event_potager", EventPotager },
            { "_on_activate", EventItemActivated },
            { "_event_effects", EventEffects },
        }.Concat(Systems.Loot.Register)
     .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

    private static int OnModuleLoad(uint oidSelf)
    {
      //Systems.Loot.InitChestArea();

      NWNX.Events.SubscribeEvent("NWNX_ON_INPUT_KEYBOARD_AFTER", "event_mv_plc");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_INPUT_KEYBOARD_AFTER", "event_mv_plc", 1);

      NWNX.Events.SubscribeEvent("NWNX_ON_INPUT_ATTACK_OBJECT_BEFORE", "event_auto_spell");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_INPUT_ATTACK_OBJECT_BEFORE", "event_auto_spell", 1);
      NWNX.Events.SubscribeEvent("NWNX_ON_INPUT_FORCE_MOVE_TO_OBJECT_BEFORE", "event_auto_spell");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_INPUT_FORCE_MOVE_TO_OBJECT_BEFORE", "event_auto_spell", 1);
      NWNX.Events.SubscribeEvent("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "_onspellcast");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "_onspellcast", 1);
      NWNX.Events.SubscribeEvent("NWNX_ON_INPUT_KEYBOARD_BEFORE", "event_auto_spell");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_INPUT_KEYBOARD_BEFORE", "event_auto_spell", 1);
      NWNX.Events.SubscribeEvent("NWNX_ON_INPUT_WALK_TO_WAYPOINT_BEFORE", "event_auto_spell");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_INPUT_WALK_TO_WAYPOINT_BEFORE", "event_auto_spell", 1);

      NWNX.Events.SubscribeEvent("NWNX_ON_USE_FEAT_BEFORE", "event_feat_used");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_USE_FEAT_BEFORE", "event_feat_used", 1);

      NWNX.Events.SubscribeEvent("NWNX_ON_EFFECT_REMOVED_AFTER", "event_effects");
      NWNX.Events.ToggleDispatchListMode("NWNX_ON_EFFECT_REMOVED_AFTER", "event_effects", 1);

      NWNX.Events.SubscribeEvent("CDE_POTAGER", "event_potager");

      Garden.Init();

      return Entrypoints.SCRIPT_NOT_HANDLED;
    }

    private static int EventAutoSpell(uint oidSelf)
    {
      var oClicker = oidSelf.AsPlayer();
      Systems.Player oPC;

      if (Systems.Player.Players.TryGetValue(oClicker.uuid, out oPC))
      {
        var oTarget = NWNX.Object.StringToObject(NWNX.Events.GetEventData("TARGET"));

        if (oTarget.AsObject().IsValid)
        {
          oPC.ClearAllActions();
          if (oPC.AutoAttackTarget == NWObject.OBJECT_INVALID)
          {
            oPC.CastSpellAtObject(Spell.RayOfFrost, oTarget);
            NWScript.DelayCommand(6.0f, () => oPC.OnFrostAutoAttackTimedEvent());
          }
        }

        oPC.AutoAttackTarget = oTarget;
      }

      return Entrypoints.SCRIPT_HANDLED;
    }

    private static int EventOnSpellCast(uint oidSelf)
    {
      var oClicker = oidSelf.AsPlayer();
      Systems.Player oPC;

      if (Systems.Player.Players.TryGetValue(oClicker.uuid, out oPC))
      {
        var spellId = int.Parse(NWNX.Events.GetEventData("SPELL_ID"));

        if (spellId != (int)Spell.RayOfFrost)
          oPC.AutoAttackTarget = NWObject.OBJECT_INVALID;
      }

      return Entrypoints.SCRIPT_HANDLED;
    }

    private static int EventItemActivated(uint oidSelf)
    {
      var oItem = NWScript.GetItemActivated().AsItem();
      var oActivator = NWScript.GetItemActivator().AsPlayer();
      var oTarget = NWScript.GetItemActivatedTarget();

      if (oItem.Tag == "test_block")
      {
        var oPC = Systems.Player.Players.GetValueOrDefault(oActivator.uuid);
        oPC.BlockPlayer();
      }

      return Entrypoints.SCRIPT_NOT_HANDLED;
    }

    private static int EventEffects(uint oidSelf)
    {
      string current_event = NWNX.Events.GetCurrentEvent();

      if (current_event == "NWNX_ON_EFFECT_REMOVED_AFTER")
      {
        if (NWNX.Events.GetEventData("CUSTOM_TAG") == "lycan_curse")
        {
          Systems.Player myPlayer = Systems.Player.Players.GetValueOrDefault(oidSelf.AsPlayer().uuid);
          myPlayer.RemoveLycanCurse();
        }
      }

      return Entrypoints.SCRIPT_HANDLED;
    }

    private static int EventDMActions(uint oidSelf)
    {
      NWPlayer oPC = oidSelf.AsPlayer();
      string current_event = NWNX.Events.GetCurrentEvent();

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

    private static int EventPlayerConnexion(uint oidSelf)
    {
      string current_event = NWNX.Events.GetCurrentEvent();

      if (current_event == "NWNX_ON_CLIENT_DISCONNECT_BEFORE")
      {
        Systems.Player.Players.Remove(oidSelf.AsObject().uuid);
        oidSelf.AsObject().Locals.Int.Set("_IS_DISCONNECTING", 1);
      }
      return Entrypoints.SCRIPT_NOT_HANDLED;
    }

    private static int OnEnter(uint oidSelf)
    {
      var oPC = NWScript.GetEnteringObject();
      Systems.Player myPlayer = new Systems.Player(oPC);
      myPlayer.AddFeat(NWN.Enums.Feat.PlayerTool01);

      myPlayer.AddToDispatchList("NWNX_ON_USE_FEAT_BEFORE", "event_feat_used");

      if (NWNX.Object.GetInt(oPC, "_FROST_ATTACK") != 0)
      {
        NWNX.Events.AddObjectToDispatchList("NWNX_ON_INPUT_ATTACK_OBJECT_BEFORE", "event_auto_spell", myPlayer);
        NWNX.Events.AddObjectToDispatchList("NWNX_ON_INPUT_FORCE_MOVE_TO_OBJECT_BEFORE", "event_auto_spell", myPlayer);
        NWNX.Events.AddObjectToDispatchList("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "_onspellcast", myPlayer);
        NWNX.Events.AddObjectToDispatchList("NWNX_ON_INPUT_KEYBOARD_BEFORE", "event_auto_spell", myPlayer);
        NWNX.Events.AddObjectToDispatchList("NWNX_ON_INPUT_WALK_TO_WAYPOINT_BEFORE", "event_auto_spell", myPlayer);
      }

      if (myPlayer.GetPossessedItem("pj_lycan_curse").IsValid)
      {
        myPlayer.AddFeat(NWN.Enums.Feat.PlayerTool02);
        myPlayer.GetPossessedItem("pj_lycan_curse").Destroy();
      }

      return Entrypoints.SCRIPT_NOT_HANDLED;
    }

    private static int EventPotager(uint oidSelf)
    {
      Garden oGarden;
      if (Garden.Potagers.TryGetValue(oidSelf.AsPlaceable().Locals.Int.Get("id"), out oGarden))
      {
        oGarden.PlanterFruit(NWNX.Events.GetEventData("FRUIT_NAME"), NWNX.Events.GetEventData("FRUIT_TAG"));
      }

      return Entrypoints.SCRIPT_HANDLED;
    }

    private static int EventMovePlaceable(uint oidSelf)
    {
      string current_event = NWNX.Events.GetCurrentEvent();

      string sKey = Events.GetEventData("KEY");
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

    private static int EventFeatUsed(uint oidSelf)
    {
      string current_event = NWNX.Events.GetCurrentEvent();
      var feat = int.Parse(NWNX.Events.GetEventData("FEAT_ID"));

      if (current_event == "NWNX_ON_USE_FEAT_BEFORE")
      {
        if (feat == (int)NWN.Enums.Feat.PlayerTool02)
        {
          NWNX.Events.SkipEvent();
          var oPC = Systems.Player.Players.GetValueOrDefault(oidSelf.AsObject().uuid);

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

          return Entrypoints.SCRIPT_HANDLED;
        }
        else if (feat == (int)NWN.Enums.Feat.PlayerTool01)
        {
          NWNX.Events.SkipEvent();
          NWPlaceable oTarget = NWNX.Object.StringToObject(NWNX.Events.GetEventData("TARGET_OBJECT_ID")).AsPlaceable();
          Systems.Player myPlayer = Systems.Player.Players.GetValueOrDefault(oidSelf.AsPlayer().uuid);

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
                myPlayer.BlockPlayer();
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
                connection.Execute(sql, new { uuid = selectedObject.AsObject().uuid, loc = APSLocationToString(selectedObject.AsObject().Location) });
              }

              sObjectSaved += selectedObject.AsObject().Name + "\n";
            }

            myPlayer.SendMessage($"Vous venez de sauvegarder le positionnement des meubles : \n{sObjectSaved}");
            myPlayer.UnblockPlayer();

            NWNX.Events.RemoveObjectFromDispatchList("NWNX_ON_INPUT_KEYBOARD_AFTER", "event_mv_plc", oidSelf);
            oidSelf.AsObject().Locals.Object.Delete("_MOVING_PLC");
            myPlayer.SelectedObjectsList.Clear();
          }
          return Entrypoints.SCRIPT_HANDLED;
        }
      }
      return Entrypoints.SCRIPT_HANDLED;
    }

    private static string APSLocationToString(Location lLocation)
    {
      uint oArea = NWScript.GetAreaFromLocation(lLocation);
      Vector vPosition = NWScript.GetPositionFromLocation(lLocation);
      float fOrientation = NWScript.GetFacingFromLocation(lLocation);
      string sReturnValue = null;

      if (NWScript.GetIsObjectValid(oArea))
        sReturnValue =
            "#AREA#" + NWScript.GetTag(oArea) + "#POSITION_X#" + (vPosition.x).ToString() +
            "#POSITION_Y#" + (vPosition.y).ToString() + "#POSITION_Z#" +
            (vPosition.z).ToString() + "#ORIENTATION#" + (fOrientation).ToString() + "#END#";

      return sReturnValue;
    }

    private static int ChatListener(uint oidSelf)
    {
      string sChatReceived = Chat.GetMessage();
      NWPlayer oChatSender = ((uint)Chat.GetSender()).AsPlayer();
      NWObject oChatTarget = Chat.GetTarget();
      Enum iChannel = (ChatChannel)Chat.GetChannel();

      if (!oChatSender.IsPC)
        return Entrypoints.SCRIPT_NOT_HANDLED;

      Systems.Player oPC = Systems.Player.Players.GetValueOrDefault(oChatSender.uuid);

      if (sChatReceived.StartsWith("!frostattack"))
      {
        Chat.SkipMessage();

        if (oChatSender.HasSpell(Spell.RayOfFrost))
        {
          if (NWNX.Object.GetInt(oChatSender, "_FROST_ATTACK") == 0)
          {
            NWNX.Object.SetInt(oChatSender, "_FROST_ATTACK", 1, true);
            NWNX.Events.AddObjectToDispatchList("NWNX_ON_INPUT_ATTACK_OBJECT_BEFORE", "event_auto_spell", oidSelf);
            NWNX.Events.AddObjectToDispatchList("NWNX_ON_INPUT_FORCE_MOVE_TO_OBJECT_BEFORE", "event_auto_spell", oidSelf);
            NWNX.Events.AddObjectToDispatchList("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "_onspellcast", oidSelf);
            NWNX.Events.AddObjectToDispatchList("NWNX_ON_INPUT_KEYBOARD_BEFORE", "event_auto_spell", oidSelf);
            NWNX.Events.AddObjectToDispatchList("NWNX_ON_INPUT_WALK_TO_WAYPOINT_BEFORE", "event_auto_spell", oidSelf);
            oPC.SendMessage("Vous activez le mode d'attaque par rayon de froid");
          }
          else
          {
            NWNX.Object.DeleteInt(oChatSender, "_FROST_ATTACK");
            NWNX.Events.RemoveObjectFromDispatchList("NWNX_ON_INPUT_ATTACK_OBJECT_BEFORE", "event_auto_spell", oidSelf);
            NWNX.Events.RemoveObjectFromDispatchList("NWNX_ON_INPUT_FORCE_MOVE_TO_OBJECT_BEFORE", "event_auto_spell", oidSelf);
            NWNX.Events.RemoveObjectFromDispatchList("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "_onspellcast", oidSelf);
            NWNX.Events.RemoveObjectFromDispatchList("NWNX_ON_INPUT_KEYBOARD_BEFORE", "event_auto_spell", oidSelf);
            NWNX.Events.RemoveObjectFromDispatchList("NWNX_ON_INPUT_WALK_TO_WAYPOINT_BEFORE", "event_auto_spell", oidSelf);
            oPC.SendMessage("Vous désactivez le mode d'attaque par rayon de froid");
          }
        }
        else
          oPC.SendMessage("Il vous faut pouvoir lancer le sort rayon de froid pour activer ce mode.");

        return Entrypoints.SCRIPT_HANDLED;
      }
      else if (sChatReceived.StartsWith("!testmenu"))
      {
        Chat.SkipMessage();
        NWScript.SetLocalInt(oChatSender, "_MENU_ON", 1);
        PostString.Menu_UpdateGUI(oChatSender);
        PostString.Menu_DrawStaticGUI(oChatSender);
        return Entrypoints.SCRIPT_HANDLED;
      }
      else if (sChatReceived.StartsWith("!walk"))
      {
        Chat.SkipMessage();
        if (NWNX.Object.GetInt(oChatSender, "_ALWAYS_WALK") == 0)
        {
          NWNX.Player.SetAlwaysWalk(oChatSender, true);
          NWNX.Object.SetInt(oChatSender, "_ALWAYS_WALK", 1, true);
          oChatSender.SendMessage("Vous avez activé le mode marche.");
        }
        else
        {
          NWNX.Player.SetAlwaysWalk(oChatSender, false);
          NWNX.Object.DeleteInt(oChatSender, "_ALWAYS_WALK");
          oChatSender.SendMessage("Vous avez désactivé le mode marche.");
        }
        return Entrypoints.SCRIPT_HANDLED;
      }
      else if (sChatReceived.StartsWith("!testblockdotnet"))
      {
        Chat.SkipMessage();
        oPC.BlockPlayer();
        //Garden.Init();
        return Entrypoints.SCRIPT_HANDLED;
      }

      return Entrypoints.SCRIPT_NOT_HANDLED;
    }
    private static int EventKeyboard(uint oidSelf)
    {
      string current_event = Events.GetCurrentEvent();

      if (current_event == "NWNX_ON_INPUT_KEYBOARD_AFTER")
      {
        NWObject oPlayer = NWObject.OBJECT_SELF.AsObject();

        if (NWScript.GetLocalInt(oPlayer, "_MENU_ON") != 0)
        {
          string sKey = Events.GetEventData("KEY");
          int nCurrentGUISelection = NWScript.GetLocalInt(oPlayer, "CurrentGUISelection");
          bool bRedraw = false;

          if (sKey == "W")
          {
            if (nCurrentGUISelection > 0)
            {
              nCurrentGUISelection--;
              bRedraw = true;
            }
          }
          else
          if (sKey == "S")
          {
            if (nCurrentGUISelection < 2)
            {
              nCurrentGUISelection++;
              bRedraw = true;
            }
          }
          else
          if (sKey == "E")
          {
            NWNX.Player.PlaySound(oPlayer, "gui_picklockopen", NWObject.OBJECT_INVALID);

            switch (nCurrentGUISelection)
            {
              case 0:
                {
                  NWScript.FloatingTextStringOnCreature("Start!", oPlayer, false);
                  break;
                }

              case 1:
                {
                  NWScript.FloatingTextStringOnCreature("Stop!", oPlayer, false);
                  break;
                }
              case 2:
                {
                  NWScript.FloatingTextStringOnCreature("Exit!", oPlayer, false);
                  break;
                }
            }
          }

          NWScript.SetLocalInt(oPlayer, "CurrentGUISelection", nCurrentGUISelection);

          if (bRedraw)
          {
            NWNX.Player.PlaySound(oPlayer, "gui_select", NWObject.OBJECT_INVALID);
            PostString.Menu_UpdateGUI(oPlayer);
            PostString.Menu_DrawStaticGUI(oPlayer);
          }
        }
      }

      return Entrypoints.SCRIPT_NOT_HANDLED;
    }

    private static int CantripsScaler(uint oidSelf)
    {
      NWObject oTarget = (NWScript.GetSpellTargetObject()).AsObject();
      NWCreature oCaster = oidSelf.AsCreature();
      int nCasterLevel = oCaster.CasterLevel();
      NWScript.SignalEvent(oTarget, NWScript.EventSpellCastAt(oCaster, (Spell)NWScript.GetSpellId()));
      int nMetaMagic = NWScript.GetMetaMagicFeat();
      Effect eVis = null;
      Effect eDur = null;
      Effect eLink = null;
      int nDuration = 0;

      switch (NWScript.GetSpellId())
      {
        case (int)Spell.AcidSplash:
          eVis = NWScript.EffectVisualEffect((VisualEffect)Impact.AcidSmall);

          //Make SR Check
          if (Spells.MyResistSpell(oCaster, oTarget) == 0)
          {
            //Set damage effect
            int iDamage = 3;
            int nDamage = Spells.MaximizeOrEmpower(iDamage, 1 + nCasterLevel / 6, nMetaMagic);
            oTarget.ApplyEffect(DurationType.Instant, NWScript.EffectLinkEffects(eVis, NWScript.EffectDamage(nDamage, NWN.Enums.DamageType.Acid)));
          }
          break;

        case (int)Spell.Daze:
          Effect eMind = NWScript.EffectVisualEffect((VisualEffect)Temporary.MindAffectingNegative);
          Effect eDaze = NWScript.EffectDazed();
          eDur = NWScript.EffectVisualEffect((VisualEffect)Temporary.CessateNegative);

          eLink = NWScript.EffectLinkEffects(eMind, eDaze);
          eLink = NWScript.EffectLinkEffects(eLink, eDur);

          eVis = NWScript.EffectVisualEffect((VisualEffect)Impact.Dazed);

          nDuration = 2;
          //check meta magic for extend
          if (nMetaMagic == (int)MetaMagic.Extend)
          {
            nDuration = 4;
          }

          //Make sure the target is a humanoid
          if (oTarget.IsHumanoid)
          {
            if (((NWCreature)oTarget).HitDice <= 5 + nCasterLevel / 6)
            {
              //Make SR check
              if (Spells.MyResistSpell(oCaster, oTarget) == 0)
              {
                //Make Will Save to negate effect
                if (((NWCreature)oTarget).MySavingThrow(SavingThrow.Will, NWScript.GetSpellSaveDC(), SavingThrowType.MindSpells) == SaveReturn.Failed)
                {
                  //Apply VFX Impact and daze effect
                  oTarget.ApplyEffect(DurationType.Temporary, eLink, NWScript.RoundsToSeconds(nDuration));
                  oTarget.ApplyEffect(DurationType.Instant, eVis);
                }
              }
            }
          }
          break;
        case (int)Spell.ElectricJolt:
          eVis = NWScript.EffectVisualEffect((VisualEffect)Impact.LightningBlast);
          //Make SR Check
          if (Spells.MyResistSpell(oCaster, oTarget) == 0)
          {
            //Set damage effect
            int iDamage = 3;
            Effect eBad = NWScript.EffectDamage(Spells.MaximizeOrEmpower(iDamage, 1 + nCasterLevel / 6, nMetaMagic), NWN.Enums.DamageType.Electrical);
            //Apply the VFX impact and damage effect
            oTarget.ApplyEffect(DurationType.Instant, eVis, oTarget);
            oTarget.ApplyEffect(DurationType.Instant, eBad, oTarget);
          }
          break;
        case (int)Spell.Flare:
          eVis = NWScript.EffectVisualEffect((VisualEffect)Impact.FlameSmall);

          // * Apply the hit effect so player knows something happened
          oTarget.ApplyEffect(DurationType.Instant, eVis);

          //Make SR Check
          if ((Spells.MyResistSpell(oCaster, oTarget)) == 0 && (((NWCreature)oTarget).MySavingThrow(SavingThrow.Fortitude, NWScript.GetSpellSaveDC()) == SaveReturn.Failed))
          {
            //Set damage effect
            Effect eBad = NWScript.EffectAttackDecrease(1 + nCasterLevel / 6);
            //Apply the VFX impact and damage effect
            oTarget.ApplyEffect(DurationType.Temporary, eBad, NWScript.RoundsToSeconds(10 + 10 * nCasterLevel / 6));
          }
          break;
        case (int)Spell.Light:
          if (oTarget.ObjectType == ObjectType.Item)
          {
            // Do not allow casting on not equippable items
            if (!((NWItem)oTarget).IsEquippable)
              NWScript.FloatingTextStrRefOnCreature(83326, oCaster);
            else
            {
              ItemProperty ip = NWScript.ItemPropertyLight(LightBrightness.LIGHTBRIGHTNESS_NORMAL, LightColor.WHITE);

              if (((NWItem)oTarget).ItemProperties.Contains(ip))
                ((NWItem)oTarget).RemoveMatchingItemProperties(ItemPropertyType.Light, DurationType.Temporary);

              nDuration = oCaster.CasterLevel();
              //Enter Metamagic conditions
              if (nMetaMagic == (int)MetaMagic.Extend)
                nDuration = nDuration * 2; //Duration is +100%

              ((NWItem)oTarget).AddItemProperty(DurationType.Temporary, ip, NWScript.HoursToSeconds(nDuration));
            }
          }
          else
          {
            eVis = NWScript.EffectVisualEffect((VisualEffect)Temporary.LightWhite20);
            eDur = NWScript.EffectVisualEffect((VisualEffect)Temporary.CessatePositive);
            eLink = NWScript.EffectLinkEffects(eVis, eDur);

            nDuration = oCaster.CasterLevel();
            //Enter Metamagic conditions
            if (nMetaMagic == (int)MetaMagic.Extend)
              nDuration = nDuration * 2; //Duration is +100%

            //Apply the VFX impact and effects
            oTarget.ApplyEffect(DurationType.Temporary, eLink, NWScript.HoursToSeconds(nDuration));
          }
          break;
        case (int)Spell.RayOfFrost:
          Effect eDam;
          eVis = NWScript.EffectVisualEffect((VisualEffect)Impact.FrostSmall);
          Effect eRay = NWScript.EffectBeam(Beam.Cold, oCaster, 0);

          if (oTarget.ObjectType == ObjectType.Placeable && oTarget.ResRef == "jbb_feupetit") { oTarget.IsPlot = false; oTarget.Destroy(); }
          if (oTarget.ObjectType == ObjectType.Placeable && oTarget.ResRef == "jbb_feumoyen") { oTarget.IsPlot = false; oTarget.Destroy(); }
          if (oTarget.ObjectType == ObjectType.Placeable && oTarget.ResRef == "jbb_feularge") { oTarget.IsPlot = false; oTarget.Destroy(); }

          //Make SR Check
          if (Spells.MyResistSpell(oCaster, oTarget) == 0)
          {
            int nDamage = Spells.MaximizeOrEmpower(4, 1 + nCasterLevel / 6, nMetaMagic);
            //Set damage effect
            eDam = NWScript.EffectDamage(nDamage, NWN.Enums.DamageType.Cold);
            //Apply the VFX impact and damage effect
            oTarget.ApplyEffect(DurationType.Instant, eVis);
            oTarget.ApplyEffect(DurationType.Instant, eDam);
          }

          oTarget.ApplyEffect(DurationType.Temporary, eRay, 1.7f);
          break;
        case (int)Spell.Resistance:
          Effect eSave;
          eVis = NWScript.EffectVisualEffect((VisualEffect)Impact.HeadHoly);
          eDur = NWScript.EffectVisualEffect((VisualEffect)Temporary.CessatePositive);

          int nBonus = 1 + nCasterLevel / 6; //Saving throw bonus to be applied
          nDuration = 2 + nCasterLevel / 6; // Turns

          //Check for metamagic extend
          if (nMetaMagic == (int)MetaMagic.Extend)
            nDuration = nDuration * 2;
          //Set the bonus save effect
          eSave = NWScript.EffectSavingThrowIncrease((int)SavingThrowType.All, nBonus);
          eLink = NWScript.EffectLinkEffects(eSave, eDur);

          //Apply the bonus effect and VFX impact
          oTarget.ApplyEffect(DurationType.Temporary, eLink, NWScript.TurnsToSeconds(nDuration));
          oTarget.ApplyEffect(DurationType.Instant, eVis);
          break;
        case (int)Spell.Virtue:
          nDuration = nCasterLevel;
          eVis = NWScript.EffectVisualEffect((VisualEffect)Impact.HolyAid);
          Effect eHP = NWScript.EffectTemporaryHitpoints(1);
          eDur = NWScript.EffectVisualEffect((VisualEffect)Temporary.CessatePositive);
          eLink = NWScript.EffectLinkEffects(eHP, eDur);

          //Enter Metamagic conditions
          if (nMetaMagic == (int)MetaMagic.Extend)
            nDuration = nDuration * 2; //Duration is +100%

          //Apply the VFX impact and effects
          oTarget.ApplyEffect(DurationType.Instant, eVis);
          oTarget.ApplyEffect(DurationType.Temporary, eLink, NWScript.TurnsToSeconds(nDuration));
          break;
      }

      oCaster.RestoreSpells(0);

      return Entrypoints.SCRIPT_HANDLED;
    }
  }
}
