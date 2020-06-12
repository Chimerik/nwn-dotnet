using NWN.Enums;
using NWN.Enums.Item;
using NWN.Enums.Item.Property;
using NWN.Enums.VisualEffect;
using NWN.NWNX;
using NWN.Systems.PostString;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NWN.ScriptHandlers
{
  public static class Scripts
  {
    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
        {
            { "_onload", HandleModuleLoad },
            { "x2_mod_def_act", HandleActivateItem },
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
            { "event_mouse_clic", EventMouseClick },
        }.Concat(Systems.LootSystem.Register)
     .Concat(Systems.PlayerSystem.Register)
     .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

    private static int HandleModuleLoad(uint oidSelf)
    {
      Systems.LootSystem.InitChestArea();

      NWNX.Events.SubscribeEvent(NWNX.Events.ON_INPUT_KEYBOARD_BEFORE, Systems.PlayerSystem.ON_PC_KEYSTROKE_SCRIPT);
      NWNX.Events.ToggleDispatchListMode(NWNX.Events.ON_INPUT_KEYBOARD_BEFORE, Systems.PlayerSystem.ON_PC_KEYSTROKE_SCRIPT, 1);

      return Entrypoints.SCRIPT_NOT_HANDLED;
    }

    private static int HandleActivateItem(uint oidSelf)
    {
      var oItem = NWScript.GetItemActivated();
      var oActivator = NWScript.GetItemActivator();
      var tag = NWScript.GetTag(oItem);

      Func<uint, uint, int> handler;
      if (ActivateItemHandlers.Register.TryGetValue(tag, out handler))
      {
        try
        {
          return handler.Invoke(oItem, oActivator);
        }
        catch (Exception e)
        {
          Utils.LogException(e);
        }
      }

      return Entrypoints.SCRIPT_NOT_HANDLED;
    }

    private static int EventMouseClick(uint oidSelf)
    {
      NWObject oClicker = oidSelf.AsObject();
      if (!oClicker.IsPC)
        return Entrypoints.SCRIPT_NOT_HANDLED;

      if (NWScript.GetLocalInt(oClicker, "_FROST_ATTACK_ON") != 0 && (NWNX.Events.GetCurrentEvent() == "NWNX_ON_INPUT_ATTACK_OBJECT_BEFORE" || NWNX.Events.GetCurrentEvent() == "NWNX_ON_INPUT_FORCE_MOVE_TO_OBJECT_BEFORE" || NWNX.Events.GetCurrentEvent() == "NWNX_ON_INPUT_CAST_SPELL_BEFORE" || NWNX.Events.GetCurrentEvent() == "NWNX_ON_INPUT_KEYBOARD_BEFORE" || NWNX.Events.GetCurrentEvent() == "NWNX_ON_INPUT_WALK_TO_WAYPOINT_BEFORE"))
      {
        if (NWNX.Object.StringToObject(NWNX.Events.GetEventData("TARGET")) != NWScript.GetLocalObject(oClicker, "_FROST_ATTACK_TARGET") && NWScript.GetLocalObject(oClicker, "_FROST_ATTACK_TARGET") != NWObject.OBJECT_INVALID)
        {
          NWScript.SetLocalInt(oClicker, "_FROST_ATTACK_CANCEL", 1);
          NWScript.DeleteLocalInt(oClicker, "_FROST_ATTACK_ON");
          NWScript.DeleteLocalObject(oClicker, "_FROST_ATTACK_TARGET");
        }
      }

      if (!oClicker.IsPC || NWNX.Object.GetInt(oClicker, "_FROST_ATTACK") == 0 || NWNX.Events.GetCurrentEvent() != "NWNX_ON_INPUT_ATTACK_OBJECT_BEFORE")
        return Entrypoints.SCRIPT_NOT_HANDLED;

      uint oTarget = NWNX.Object.StringToObject(NWNX.Events.GetEventData("TARGET"));
      NWScript.AssignCommand(oClicker, () => NWScript.ClearAllActions());
      NWScript.AssignCommand(oClicker, () => NWScript.ActionCastSpellAtObject(Spell.RayOfFrost, oTarget));
      NWScript.DelayCommand(6.0f, () => FrostAutoAttack(oClicker, oTarget));

      NWScript.SetLocalInt(oClicker, "_FROST_ATTACK_ON", 1);
      NWScript.SetLocalObject(oClicker, "_FROST_ATTACK_TARGET", oTarget);

      return Entrypoints.SCRIPT_NOT_HANDLED;
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

    private static int ChatListener(uint oidSelf)
    {
      string sChatReceived = Chat.GetMessage();
      NWN.NWObject oChatSender = Chat.GetSender();
      NWN.NWObject oChatTarget = Chat.GetTarget();
      Enum iChannel = (ChatChannel)Chat.GetChannel();

      if (!oChatSender.IsPC)
        return Entrypoints.SCRIPT_NOT_HANDLED;

      if (sChatReceived.StartsWith("!frostattack"))
      {
        Chat.SkipMessage();
        if (NWScript.GetLevelByClass(ClassType.Wizard, oChatSender) > 0 || NWScript.GetLevelByClass(ClassType.Sorcerer, oChatSender) > 0)
        {
          if (NWNX.Object.GetInt(oChatSender, "_FROST_ATTACK") == 0)
          {
            NWNX.Object.SetInt(oChatSender, "_FROST_ATTACK", 1, true);
            NWScript.SendMessageToPC(oChatSender, "Vous activez le mode d'attaque par rayon de froid");
          }
          else
          {
            NWNX.Object.DeleteInt(oChatSender, "_FROST_ATTACK");
            NWScript.SendMessageToPC(oChatSender, "Vous désactivez le mode d'attaque par rayon de froid");
          }
        }
        else
          NWScript.SendMessageToPC(oChatSender, "Il vous faut pouvoir lancer le sort rayon de froid pour activer ce mode.");

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
          NWScript.SendMessageToPC(oChatSender, "Vous avez activé le mode marche.");
        }
        else
        {
          NWNX.Player.SetAlwaysWalk(oChatSender, false);
          NWNX.Object.DeleteInt(oChatSender, "_ALWAYS_WALK");
          NWScript.SendMessageToPC(oChatSender, "Vous avez désactivé le mode marche.");
        }
        return Entrypoints.SCRIPT_HANDLED;
      }

      return Entrypoints.SCRIPT_NOT_HANDLED;
    }
    private static int EventKeyboard(uint oidSelf)
    {
      string current_event = Events.GetCurrentEvent();

      NWScript.ApplyEffectToObject(DurationType.Permanent, NWScript.EffectCutsceneParalyze(), oidSelf);

      if (current_event == "NWNX_ON_INPUT_KEYBOARD_AFTER")
      {
        NWObject oPlayer = NWObject.OBJECT_SELF.AsObject();

        if (NWScript.GetLocalInt(oPlayer, "_MENU_ON") != 0)
        {
          NWScript.ClearAllActions();

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
            Player.PlaySound(oPlayer, "gui_picklockopen", NWObject.OBJECT_INVALID);

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
            Player.PlaySound(oPlayer, "gui_select", NWObject.OBJECT_INVALID);
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
      int nCasterLevel = Spells.GetMyCasterLevel(NWObject.OBJECT_SELF);
      NWScript.SignalEvent(oTarget, NWScript.EventSpellCastAt(NWObject.OBJECT_SELF, (Spell)NWScript.GetSpellId()));
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
          if (Spells.MyResistSpell(NWObject.OBJECT_SELF, oTarget) == 0)
          {
            //Set damage effect
            int iDamage = 3 * nCasterLevel / 6;
            if (iDamage < 3)
              iDamage = 3;
            int nDamage = Spells.MaximizeOrEmpower(iDamage, 1 + nCasterLevel / 6, NWScript.GetMetaMagicFeat());
            Effect eBad = NWScript.EffectDamage(nDamage, NWN.Enums.DamageType.Acid);
            //Apply the VFX impact and damage effect
            NWScript.ApplyEffectToObject(DurationType.Instant, eVis, oTarget);
            NWScript.ApplyEffectToObject(DurationType.Instant, eBad, oTarget);
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
          if (Spells.AmIAHumanoid(oTarget))
          {
            if (NWScript.GetHitDice(oTarget) <= 5 + nCasterLevel / 6)
            {
              //Make SR check
              if (Spells.MyResistSpell(NWObject.OBJECT_SELF, oTarget) == 0)
              {
                //Make Will Save to negate effect
                if (Spells.MySavingThrow(SavingThrow.Will, oTarget, NWScript.GetSpellSaveDC(), SavingThrowType.MindSpells) == SaveReturn.Failed)
                {
                  //Apply VFX Impact and daze effect
                  NWScript.ApplyEffectToObject(DurationType.Temporary, eLink, oTarget, NWScript.RoundsToSeconds(nDuration));
                  NWScript.ApplyEffectToObject(DurationType.Instant, eVis, oTarget);
                }
              }
            }
          }
          break;
        case (int)Spell.ElectricJolt:
          eVis = NWScript.EffectVisualEffect((VisualEffect)Impact.LightningBlast);
          //Make SR Check
          if (Spells.MyResistSpell(NWObject.OBJECT_SELF, oTarget) == 0)
          {
            //Set damage effect
            int iDamage = 3 * nCasterLevel / 6;
            if (iDamage < 3)
              iDamage = 3;
            Effect eBad = NWScript.EffectDamage(Spells.MaximizeOrEmpower(iDamage, 1 + nCasterLevel / 6, NWScript.GetMetaMagicFeat()), NWN.Enums.DamageType.Electrical);
            //Apply the VFX impact and damage effect
            NWScript.ApplyEffectToObject(DurationType.Instant, eVis, oTarget);
            NWScript.ApplyEffectToObject(DurationType.Instant, eBad, oTarget);
          }
          break;
        case (int)Spell.Flare:
          eVis = NWScript.EffectVisualEffect((VisualEffect)Impact.FlameSmall);

          // * Apply the hit effect so player knows something happened
          NWScript.ApplyEffectToObject(DurationType.Instant, eVis, oTarget);

          //Make SR Check
          if ((Spells.MyResistSpell(NWObject.OBJECT_SELF, oTarget)) == 0 && (Spells.MySavingThrow(SavingThrow.Fortitude, oTarget, NWScript.GetSpellSaveDC()) == SaveReturn.Failed))
          {
            //Set damage effect
            Effect eBad = NWScript.EffectAttackDecrease(1 + nCasterLevel / 6);
            //Apply the VFX impact and damage effect
            NWScript.ApplyEffectToObject(DurationType.Temporary, eBad, oTarget, NWScript.RoundsToSeconds(10 + 10 * nCasterLevel / 6));
          }
          break;
        case (int)Spell.Light:
          if (NWScript.GetObjectType(oTarget) == ObjectType.Item)
          {

            // Do not allow casting on not equippable items
            if (!((NWItem)oTarget).IsEquippable)
              NWScript.FloatingTextStrRefOnCreature(83326, NWObject.OBJECT_SELF);
            else
            {
              ItemProperty ip = NWScript.ItemPropertyLight(LightBrightness.LIGHTBRIGHTNESS_NORMAL, LightColor.WHITE);

              if (NWScript.GetItemHasItemProperty(oTarget, ItemPropertyType.Light) != 0)
              {
                NWScript.IPRemoveMatchingItemProperties(oTarget, (int)ItemPropertyType.Light, DurationType.Temporary);
              }

              nDuration = Spells.GetMyCasterLevel(NWObject.OBJECT_SELF);
              nMetaMagic = NWScript.GetMetaMagicFeat();
              //Enter Metamagic conditions
              if (nMetaMagic == (int)MetaMagic.Extend)
              {
                nDuration = nDuration * 2; //Duration is +100%
              }

              NWScript.AddItemProperty(DurationType.Temporary, ip, oTarget, NWScript.HoursToSeconds(nDuration));
            }
          }
          else
          {
            eVis = NWScript.EffectVisualEffect((VisualEffect)Temporary.LightWhite20);
            eDur = NWScript.EffectVisualEffect((VisualEffect)Temporary.CessatePositive);
            eLink = NWScript.EffectLinkEffects(eVis, eDur);

            nDuration = Spells.GetMyCasterLevel(NWObject.OBJECT_SELF);
            nMetaMagic = NWScript.GetMetaMagicFeat();
            //Enter Metamagic conditions
            if (nMetaMagic == (int)MetaMagic.Extend)
            {
              nDuration = nDuration * 2; //Duration is +100%
            }

            //Apply the VFX impact and effects
            NWScript.ApplyEffectToObject(DurationType.Temporary, eLink, oTarget, NWScript.HoursToSeconds(nDuration));
          }
          break;
        case (int)Spell.RayOfFrost:
          int nDam = NWScript.d4(1 + nCasterLevel / 6) + 1;
          Effect eDam;
          eVis = NWScript.EffectVisualEffect((VisualEffect)Impact.FrostSmall);
          Effect eRay = NWScript.EffectBeam(Beam.Cold, NWObject.OBJECT_SELF, 0);

          if ((NWScript.GetObjectType(oTarget) == ObjectType.Placeable) && (NWScript.GetResRef(oTarget) == "jbb_feupetit")) { NWScript.SetPlotFlag(oTarget, false); NWScript.DestroyObject(oTarget); }
          if ((NWScript.GetObjectType(oTarget) == ObjectType.Placeable) && (NWScript.GetResRef(oTarget) == "jbb_feumoyen")) { NWScript.SetPlotFlag(oTarget, false); NWScript.DestroyObject(oTarget); }
          if ((NWScript.GetObjectType(oTarget) == ObjectType.Placeable) && (NWScript.GetResRef(oTarget) == "jbb_feularge")) { NWScript.SetPlotFlag(oTarget, false); NWScript.DestroyObject(oTarget); }

          //Make SR Check
          if (Spells.MyResistSpell(NWObject.OBJECT_SELF, oTarget) == 0)
          {
            //Enter Metamagic conditions
            if (nMetaMagic == (int)MetaMagic.Maximize)
            {
              nDam = 5 + 5 * nCasterLevel / 6;//Damage is at max
            }
            else if (nMetaMagic == (int)MetaMagic.Empower)
            {
              nDam = nDam + nDam / 2; //Damage/Healing is +50%
            }
            //Set damage effect
            eDam = NWScript.EffectDamage(nDam, NWN.Enums.DamageType.Cold);
            //Apply the VFX impact and damage effect
            NWScript.ApplyEffectToObject(DurationType.Instant, eVis, oTarget);
            NWScript.ApplyEffectToObject(DurationType.Instant, eDam, oTarget);
          }

          NWScript.ApplyEffectToObject(DurationType.Temporary, eRay, oTarget, 1.7f);
          break;
        case (int)Spell.Resistance:
          Effect eSave;
          eVis = NWScript.EffectVisualEffect((VisualEffect)Impact.HeadHoly);
          eDur = NWScript.EffectVisualEffect((VisualEffect)Temporary.CessatePositive);

          int nBonus = 1 + nCasterLevel / 6; //Saving throw bonus to be applied
          nDuration = 2 + nCasterLevel / 6; // Turns

          //Check for metamagic extend
          if (nMetaMagic == (int)MetaMagic.Extend)
          {
            nDuration = nDuration * 2;
          }
          //Set the bonus save effect
          eSave = NWScript.EffectSavingThrowIncrease((int)SavingThrowType.All, nBonus);
          eLink = NWScript.EffectLinkEffects(eSave, eDur);

          //Apply the bonus effect and VFX impact
          NWScript.ApplyEffectToObject(DurationType.Temporary, eLink, oTarget, NWScript.TurnsToSeconds(nDuration));
          NWScript.ApplyEffectToObject(DurationType.Instant, eVis, oTarget);
          break;
        case (int)Spell.Virtue:
          nDuration = nCasterLevel;
          eVis = NWScript.EffectVisualEffect((VisualEffect)Impact.HolyAid);
          Effect eHP = NWScript.EffectTemporaryHitpoints(1);
          eDur = NWScript.EffectVisualEffect((VisualEffect)Temporary.CessatePositive);
          eLink = NWScript.EffectLinkEffects(eHP, eDur);

          //Enter Metamagic conditions
          if (nMetaMagic == (int)MetaMagic.Extend)
          {
            nDuration = nDuration * 2; //Duration is +100%
          }

          //Apply the VFX impact and effects
          NWScript.ApplyEffectToObject(DurationType.Instant, eVis, oTarget);
          NWScript.ApplyEffectToObject(DurationType.Temporary, eLink, oTarget, NWScript.TurnsToSeconds(nDuration));
          break;
      }

      NWNX.Creature.RestoreSpells(NWObject.OBJECT_SELF, 0);

      return Entrypoints.SCRIPT_HANDLED;
    }
  }
}
