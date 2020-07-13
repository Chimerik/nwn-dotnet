﻿using System;
using System.Collections.Generic;
using NWN.Enums;
using NWN.Enums.Item;
using NWN.Enums.Item.Property;
using NWN.Enums.VisualEffect;
using System.Linq;

namespace NWN.Systems
{
  public static partial class SpellSystem
  {
    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
    {
            { "X0_S0_AcidSplash", CantripsScaler },
            { "NW_S0_Daze", CantripsScaler },
            { "X0_S0_ElecJolt", CantripsScaler },
            { "X0_S0_Flare", CantripsScaler },
            { "NW_S0_Light", CantripsScaler },
            { "NW_S0_RayFrost", CantripsScaler },
            { "NW_S0_Resis", CantripsScaler },
            { "NW_S0_Virtue", CantripsScaler },
    };
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
