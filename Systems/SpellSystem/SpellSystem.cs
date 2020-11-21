using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using System.Linq;
using static NWN.Systems.PlayerSystem;
using System.Numerics;

namespace NWN.Systems
{
  public static partial class SpellSystem
  {
    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
    {
            { "event_spellbroadcast_after", AfterSpellBroadcast },
            { "_onspellinterrupted_after", AfterSpellInterrupted },
            { "spellhook", HandleSpellHook },
            { "X0_S0_AcidSplash", CantripsScaler },
            { "NW_S0_Daze", CantripsScaler },
            { "X0_S0_ElecJolt", CantripsScaler },
            { "X0_S0_Flare", CantripsScaler },
            { "NW_S0_Light", CantripsScaler },
            { "NW_S0_RayFrost", CantripsScaler },
            { "NW_S0_Resis", CantripsScaler },
            { "NW_S0_Virtue", CantripsScaler },
            { "nw_s0_raisdead", HandleRaiseDeadCast },
            { "nw_s0_resserec", HandleRaiseDeadCast },
            //{ "NW_S0_DivPower", SpellTest },
            { "NW_S0_Fireball", SpellTest },
    };
    private static int CantripsScaler(uint oidSelf)
    {
      var oTarget = (NWScript.GetSpellTargetObject());
      var oCaster = oidSelf;
      int nCasterLevel = NWScript.GetCasterLevel(oCaster);
      NWScript.SignalEvent(oTarget, NWScript.EventSpellCastAt(oCaster, NWScript.GetSpellId()));
      int nMetaMagic = NWScript.GetMetaMagicFeat();
      Effect eVis = null;
      Effect eDur = null;
      Effect eLink = null;
      int nDuration = 0;

      switch (NWScript.GetSpellId())
      {
        case NWScript.SPELL_ACID_SPLASH:
          eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_ACID_S);

          //Make SR Check
          if (Spells.MyResistSpell(oCaster, oTarget) == 0)
          {
            //Set damage effect
            int iDamage = 3;
            int nDamage = Spells.MaximizeOrEmpower(iDamage, 1 + nCasterLevel / 6, nMetaMagic);
            NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectLinkEffects(eVis, NWScript.EffectDamage(nDamage, NWScript.DAMAGE_TYPE_ACID)), oTarget);
          }
          break;

        case NWScript.SPELL_DAZE:
          Effect eMind = NWScript.EffectVisualEffect(NWScript.VFX_DUR_MIND_AFFECTING_NEGATIVE);
          Effect eDaze = NWScript.EffectDazed();
          eDur = NWScript.EffectVisualEffect((NWScript.VFX_DUR_CESSATE_NEGATIVE));

          eLink = NWScript.EffectLinkEffects(eMind, eDaze);
          eLink = NWScript.EffectLinkEffects(eLink, eDur);
          
          eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_DAZED_S);

          nDuration = 2;
          //check meta magic for extend
          if (nMetaMagic == NWScript.METAMAGIC_EXTEND)
          {
            nDuration = 4;
          }
          
          if (NWScript.GetHitDice(oTarget) <= 5 + nCasterLevel / 6)
          {
            //Make SR check
            if (Spells.MyResistSpell(oCaster, oTarget) == 0)
            {
              //Make Will Save to negate effect
              if (Spells.MySavingThrow(NWScript.SAVING_THROW_WILL, oTarget, NWScript.GetSpellSaveDC(), NWScript.SAVING_THROW_TYPE_MIND_SPELLS) == 0) // 0 = SAVE FAILED
              {
                //Apply VFX Impact and daze effect
                NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eLink, oTarget, NWScript.RoundsToSeconds(nDuration));
                NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, eVis, oTarget);
              }
            }
          }
          break;
        case NWScript.SPELL_ELECTRIC_JOLT:
          eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_LIGHTNING_S);
          //Make SR Check
          if (Spells.MyResistSpell(oCaster, oTarget) == 0)
          {
            //Set damage effect
            int iDamage = 3;
            Effect eBad = NWScript.EffectDamage(Spells.MaximizeOrEmpower(iDamage, 1 + nCasterLevel / 6, nMetaMagic), NWScript.DAMAGE_TYPE_ELECTRICAL);
            //Apply the VFX impact and damage effect
            NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, eVis, oTarget);
            NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, eBad, oTarget);
          }
          break;
        case NWScript.SPELL_FLARE:
          eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_FLAME_S);

          // * Apply the hit effect so player knows something happened
          NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, eVis, oTarget);

          //Make SR Check
          if ((Spells.MyResistSpell(oCaster, oTarget)) == 0 && Spells.MySavingThrow(NWScript.SAVING_THROW_FORT, oTarget, NWScript.GetSpellSaveDC()) == 0) // 0 = failed
          {
            //Set damage effect
            Effect eBad = NWScript.EffectAttackDecrease(1 + nCasterLevel / 6);
            //Apply the VFX impact and damage effect
            NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eBad, oTarget, NWScript.RoundsToSeconds(10 + 10 * nCasterLevel / 6));
          }
          break;
        case NWScript.SPELL_LIGHT:
          if (NWScript.GetObjectType(oTarget) == NWScript.OBJECT_TYPE_ITEM)
          {
            // Do not allow casting on not equippable items
            if (!ItemSystem.GetIsItemEquipable(oTarget))
              NWScript.FloatingTextStrRefOnCreature(83326, oCaster);
            else
            {
              ItemProperty ip = NWScript.ItemPropertyLight(NWScript.IP_CONST_LIGHTBRIGHTNESS_NORMAL, NWScript.IP_CONST_LIGHTCOLOR_WHITE);
              
              if (NWScript.GetItemHasItemProperty(oTarget, NWScript.ITEM_PROPERTY_LIGHT) == 1) 
                ItemSystem.RemoveMatchingItemProperties(oTarget, NWScript.ITEM_PROPERTY_LIGHT, NWScript.DURATION_TYPE_TEMPORARY);

              nDuration = NWScript.GetCasterLevel(oCaster);
              //Enter Metamagic conditions
              if (nMetaMagic == NWScript.METAMAGIC_EXTEND)
                nDuration = nDuration * 2; //Duration is +100%

              NWScript.AddItemProperty(NWScript.DURATION_TYPE_TEMPORARY, ip, oTarget, NWScript.HoursToSeconds(nDuration));
            }
          }
          else
          {
            eVis = NWScript.EffectVisualEffect(NWScript.VFX_DUR_LIGHT_WHITE_20);
            eDur = NWScript.EffectVisualEffect(NWScript.VFX_DUR_CESSATE_POSITIVE);
            eLink = NWScript.EffectLinkEffects(eVis, eDur);

            nDuration = NWScript.GetCasterLevel(oCaster);
            //Enter Metamagic conditions
            if (nMetaMagic == NWScript.METAMAGIC_EXTEND)
              nDuration = nDuration * 2; //Duration is +100%

            //Apply the VFX impact and effects
            NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eLink, oTarget, NWScript.HoursToSeconds(nDuration));
          }
          break;
        case NWScript.SPELL_RAY_OF_FROST:
          Effect eDam;
          eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_FROST_S);
          Effect eRay = NWScript.EffectBeam(NWScript.VFX_BEAM_COLD, oCaster, 0);

          //Make SR Check
          if (Spells.MyResistSpell(oCaster, oTarget) == 0)
          {
            int nDamage = Spells.MaximizeOrEmpower(4, 1 + nCasterLevel / 6, nMetaMagic);
            //Set damage effect
            eDam = NWScript.EffectDamage(nDamage, NWScript.DAMAGE_TYPE_COLD);
            //Apply the VFX impact and damage effect
            NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, eVis, oTarget);
            NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, eDam, oTarget);
          }

          NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eRay, oTarget, 1.7f);
          break;
        case NWScript.SPELL_RESISTANCE:
          Effect eSave;
          eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_HEAD_HOLY);
          eDur = NWScript.EffectVisualEffect(NWScript.VFX_DUR_CESSATE_POSITIVE);

          int nBonus = 1 + nCasterLevel / 6; //Saving throw bonus to be applied
          nDuration = 2 + nCasterLevel / 6; // Turns

          //Check for metamagic extend
          if (nMetaMagic == NWScript.METAMAGIC_EXTEND)
            nDuration = nDuration * 2;
          //Set the bonus save effect
          eSave = NWScript.EffectSavingThrowIncrease(NWScript.SAVING_THROW_ALL, nBonus);
          eLink = NWScript.EffectLinkEffects(eSave, eDur);

          //Apply the bonus effect and VFX impact
          NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eLink, oTarget, NWScript.TurnsToSeconds(nDuration));
          NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, eVis, oTarget);
          break;
        case NWScript.SPELL_VIRTUE:
          nDuration = nCasterLevel;
          eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_HOLY_AID);
          Effect eHP = NWScript.EffectTemporaryHitpoints(1);
          eDur = NWScript.EffectVisualEffect(NWScript.VFX_DUR_CESSATE_POSITIVE);
          eLink = NWScript.EffectLinkEffects(eHP, eDur);

          //Enter Metamagic conditions
          if (nMetaMagic == NWScript.METAMAGIC_EXTEND)
            nDuration = nDuration * 2; //Duration is +100%

          //Apply the VFX impact and effects
          NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, eVis, oTarget);
          NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eLink, oTarget, NWScript.TurnsToSeconds(nDuration));
          break;
      }

      CreaturePlugin.RestoreSpells(oCaster, 0);

      return 0;
    }
    private static int AfterSpellBroadcast(uint oidSelf)
    {
      Player oPC;
      if (Players.TryGetValue(oidSelf, out oPC))
      {
        if (NWScript.GetIsDM(oPC.oid) != 1)
        {
          int iCount = 1;
            
          if (NWScript.GetHasSpellEffect(NWScript.SPELL_IMPROVED_INVISIBILITY, oPC.oid) == 1 || NWScript.GetHasSpellEffect(NWScript.SPELL_INVISIBILITY, oPC.oid) == 1 || NWScript.GetHasSpellEffect(NWScript.SPELL_INVISIBILITY_PURGE, oPC.oid) == 1)
            if (int.Parse(EventsPlugin.GetEventData("META_TYPE")) != NWScript.METAMAGIC_SILENT)
            {
              var oSpotter = NWScript.GetNearestCreature(1, 1, oPC.oid, iCount);
              while (NWScript.GetIsObjectValid(oSpotter) == 1)
              {
                if (NWScript.GetDistanceBetween(oSpotter, oPC.oid) > 20.0f)
                  break;

                if (NWScript.GetObjectSeen(oPC.oid, oSpotter) != 1)
                {
                  NWScript.SendMessageToPC(oSpotter, "Quelqu'un d'invisible est en train de lancer un sort à proximité !");
                  PlayerPlugin.ShowVisualEffect(oSpotter, 191, NWScript.GetPosition(oPC.oid));
                }

              iCount++;
                oSpotter = NWScript.GetNearestCreature(1, 1, oPC.oid, iCount);
              }
            }
        }
      }

      return 0;
    }
    private static int HandleRaiseDeadCast(uint oidSelf)
    {
      var oTarget = NWScript.GetSpellTargetObject();
      if(NWScript.GetTag(oTarget) == "pccorpse")
      {
        int PcId = NWScript.GetLocalInt(oTarget, "_PC_ID");
        PlayerSystem.Player oPC = GetPCById(PcId);
        
        if (oPC != null && oPC.isConnected)
        {
          NWScript.AssignCommand(oPC.oid, () => NWScript.JumpToLocation(NWScript.GetLocation(oTarget)));

          if (NWScript.GetSpellId() == (NWScript.SPELL_RAISE_DEAD))
            NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectResurrection(), oTarget);
        }
        else
        {
          NWScript.SendMessageToPC(oidSelf, "Vous sentez une forme de résistance : cette âme met du temps à regagner son enveloppe corporelle. Votre sort a bien eu l'effet escompté, mais il faudra un certain temps avant de voir le corps s'animer.");

          var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"SELECT areaTag, position from playerDeathCorpses where characterId = @characterId");
          NWScript.SqlBindInt(query, "@characterId", PcId);
          NWScript.SqlStep(query);

          string areaTag = NWScript.SqlGetString(query, 0);
          Vector3 position = NWScript.SqlGetVector(query, 1);

          //TODO : vérifier ce qu'il se passe quand on ramasse et dépose le cadavre
          query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"UPDATE playerCharacters SET areaTag = @areaTag, position = @position WHERE characterId = @characterId");
          NWScript.SqlBindInt(query, "@characterId", PcId);
          NWScript.SqlBindString(query, "@areaTag", NWScript.GetTag(NWScript.GetArea(oTarget)));
          NWScript.SqlBindVector(query, "@position", NWScript.GetPosition(oTarget));
          NWScript.SqlStep(query);
        }

        uint oItemCorpse = NWScript.GetFirstItemInInventory(oTarget);

        while(NWScript.GetIsObjectValid(oItemCorpse) == 1)
        {
          if(NWScript.GetTag(oItemCorpse) == "item_pccorpse")
          {
            NWScript.DestroyObject(oItemCorpse);
            break;
          }
          oItemCorpse = NWScript.GetNextItemInInventory(oTarget);
        }

        NWScript.DestroyObject(oTarget);
        DeletePlayerCorpseFromDatabase(PcId);

        NWScript.SignalEvent(oTarget, NWScript.EventSpellCastAt(oidSelf, NWScript.SPELL_RAISE_DEAD, 0));
        NWScript.ApplyEffectAtLocation(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_IMP_RAISE_DEAD), NWScript.GetLocation(oTarget));
        return 0;
      }

      return 0;
    }
    private static int SpellTest(uint oidSelf)
    {
      Player oPC;
      if (Players.TryGetValue(oidSelf, out oPC))
      {
        var oTarget = NWScript.GetSpellTargetObject();

        int nCasterLevel = NWScript.GetCasterLevel(oidSelf);
        int nTotalCharacterLevel = NWScript.GetHitDice(oidSelf);

        NWScript.SendMessageToPC(oPC.oid, $"Entering spell script");
        NWScript.SendMessageToPC(oPC.oid, $"CL self = {nCasterLevel}");
        NWScript.SendMessageToPC(oPC.oid, $"CL target = {NWScript.GetCasterLevel(oTarget)}");
        NWScript.SendMessageToPC(oPC.oid, $"CL player = {NWScript.GetCasterLevel(oPC.oid)}");
        NWScript.SendMessageToPC(oPC.oid, $"Item used = {NWScript.GetName(NWScript.GetSpellCastItem())}");
      }

      return -1;
    }
    private static int HandleSpellHook(uint oidSelf)
    {
      Player oPC;
      if (Players.TryGetValue(oidSelf, out oPC))
      {
        NWScript.SetLocalInt(oidSelf, "_DELAYED_SPELLHOOK_REFLEX", CreaturePlugin.GetBaseSavingThrow(oidSelf, NWScript.SAVING_THROW_REFLEX));
        NWScript.SetLocalInt(oidSelf, "_DELAYED_SPELLHOOK_WILL", CreaturePlugin.GetBaseSavingThrow(oidSelf, NWScript.SAVING_THROW_WILL));
        NWScript.SetLocalInt(oidSelf, "_DELAYED_SPELLHOOK_FORT", CreaturePlugin.GetBaseSavingThrow(oidSelf, NWScript.SAVING_THROW_FORT));
        NWScript.SendMessageToPC(oidSelf, "entering spellhook");
        int casterLevel;
        if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(oidSelf, (int)Feat.ImprovedCasterLevel)), out casterLevel))
          CreaturePlugin.SetLevelByPosition(oidSelf, 0, casterLevel + 1);

        NWScript.DelayCommand(0.0f, () => DelayedSpellHook(oidSelf));
        //CreaturePlugin.SetClassByPosition(oidSelf, 0, 43);
      }

      return 0;
    }
    private static void DelayedSpellHook(uint oidSelf)
    {
      NWScript.SendMessageToPC(oidSelf, "delayed spellhook");
      CreaturePlugin.SetLevelByPosition(oidSelf, 0, 1);
      CreaturePlugin.SetBaseSavingThrow(oidSelf, NWScript.SAVING_THROW_REFLEX, NWScript.GetLocalInt(oidSelf, "_DELAYED_SPELLHOOK_REFLEX"));
      CreaturePlugin.SetBaseSavingThrow(oidSelf, NWScript.SAVING_THROW_WILL, NWScript.GetLocalInt(oidSelf, "_DELAYED_SPELLHOOK_WILL"));
      CreaturePlugin.SetBaseSavingThrow(oidSelf, NWScript.SAVING_THROW_FORT, NWScript.GetLocalInt(oidSelf, "_DELAYED_SPELLHOOK_FORT"));
    }
    private static int AfterSpellInterrupted(uint oidSelf)
    {
      /*Player oPC;
      if (Players.TryGetValue(oidSelf, out oPC))
      {
        NWScript.SendMessageToPC(oPC.oid, $"spell interrupted caster level : {NWScript.GetCasterLevel(oidSelf)}");
        CreaturePlugin.SetLevelByPosition(oidSelf, 0, 1);
        //CreaturePlugin.SetClassByPosition(oidSelf, 0, 43);
      }*/

      return 0;
    }
  }
}
