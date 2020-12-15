using System;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Systems;

namespace NWN
{
  public static class Spells
  {
    public static int MyResistSpell(uint oCaster, uint oTarget, float fDelay = 0.0f) // TODO : check la fonction par rapport à celle de never de base
    {
      if (fDelay > 0.5)
      {
        fDelay = fDelay - 0.1f;
      }
      int nResist = NWScript.ResistSpell(oCaster, oTarget);
      Effect eSR = NWScript.EffectVisualEffect(NWScript.VFX_IMP_MAGIC_RESISTANCE_USE);
      Effect eGlobe = NWScript.EffectVisualEffect(NWScript.VFX_IMP_GLOBE_USE);
      Effect eMantle = NWScript.EffectVisualEffect(NWScript.VFX_IMP_SPELL_MANTLE_USE);
      if (nResist == 1) //Spell Resistance
      {
        //Ici oCaster a perdu le test de RM contre celle de oTarget
        //Le jet de oCaster est :1d20 + NDL + les dons
        //On va ajouter +1 par ecole sup/renf abjuration
        int nTargetSR = NWScript.GetSpellResistance(oTarget);
        int nCasterSRRoll = NWScript.d20(1) + NWScript.GetCasterLevel(oCaster);
        if (NWScript.GetHasFeat(NWScript.FEAT_SPELL_FOCUS_ABJURATION, oCaster) == 1) { nCasterSRRoll = nCasterSRRoll + 2; }
        if (NWScript.GetHasFeat(NWScript.FEAT_GREATER_SPELL_FOCUS_ABJURATION, oCaster) == 1) { nCasterSRRoll = nCasterSRRoll + 2; }
        if (NWScript.GetHasFeat(NWScript.FEAT_EPIC_SPELL_FOCUS_ABJURATION, oCaster) == 1) { nCasterSRRoll = nCasterSRRoll + 2; }
  
        if (nCasterSRRoll < nTargetSR) //oCaster passe pas la RM
          NWScript.DelayCommand((float)fDelay, () => NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, eSR, oTarget));
        else
          nResist = 0;
      }
      else if (nResist == 2) //Globe
      {
        NWScript.DelayCommand(fDelay, () => NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, eGlobe, oTarget));
      }
      else if (nResist == 3) //Spell Mantle
      {
        if (fDelay > 0.5)
        {
          fDelay = fDelay - 0.1f;
        }

        NWScript.DelayCommand(fDelay, () => NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, eMantle, oTarget));
      }
      return nResist;
    }

    public static int MaximizeOrEmpower(int nDice, int nNumberOfDice, int nMeta, int nBonus = 0)
    {
      int i = 0;
      int nDamage = 0;
      for (i = 1; i <= nNumberOfDice; i++)
      {
        nDamage = nDamage + NWScript.Random(nDice) + 1;
      }
      //Resolve metamagic
      if (nMeta == NWScript.METAMAGIC_MAXIMIZE)
      {
        nDamage = nDice * nNumberOfDice;
      }
      else if (nMeta == NWScript.METAMAGIC_EMPOWER)
      {
        nDamage = nDamage + nDamage / 2;
      }
      return nDamage + nBonus;
    }
    public static void RemoveAnySpellEffects(int spell, uint oTarget)
    {
      if (NWScript.GetHasSpellEffect(spell, oTarget) == 1)
      {
        var e = NWScript.GetFirstEffect(oTarget);
        while(NWScript.GetIsEffectValid(e) == 1)
        {
          if (NWScript.GetEffectSpellId(e) == spell)
          {
            NWScript.RemoveEffect(oTarget, e);
            break;
          }

          e = NWScript.GetNextEffect(oTarget);
        }
      }
    }
    public static Boolean GetHasEffect(int effectType, uint oTarget)
    {
      var eff = NWScript.GetFirstEffect(oTarget);
      while (NWScript.GetIsEffectValid(eff) == 1)
      {
        if (effectType == NWScript.GetEffectType(eff))
          return true;
        eff = NWScript.GetNextEffect(oTarget);
      }

      return false;
    }
    public static void RemoveEffectOfType(int effectType, uint oTarget)
    {
      var eff = NWScript.GetFirstEffect(oTarget);
      while (NWScript.GetIsEffectValid(eff) == 1)
      {
        if (NWScript.GetEffectType(eff) == effectType)
          NWScript.RemoveEffect(oTarget, eff);

        eff = NWScript.GetNextEffect(oTarget);
      }
    }
    public static int MySavingThrow(int nSavingThrow, uint oTarget, int nDC, int nSaveType = NWScript.SAVING_THROW_TYPE_NONE, uint oSaveVersus = 2130706432, float fDelay = 0.0f) // 2130706432 = OBJECT_SELF ?
    {
      // -------------------------------------------------------------------------
      // GZ: sanity checks to prevent wrapping around
      // -------------------------------------------------------------------------
      if (nDC < 1)
      {
        nDC = 1;
      }
      else if (nDC > 255)
      {
        nDC = 255;
      }

      Effect eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_FORTITUDE_SAVING_THROW_USE); 
      int bValid = 0;
      int nSpellID;
      if (nSavingThrow == NWScript.SAVING_THROW_FORT)
      {
        bValid = NWScript.FortitudeSave(oTarget, nDC, nSaveType, oSaveVersus);
        if (bValid == 1)
        {
          eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_FORTITUDE_SAVING_THROW_USE);
        }
      }
      else if (nSavingThrow == NWScript.SAVING_THROW_REFLEX)
      {
        bValid = NWScript.ReflexSave(oTarget, nDC, nSaveType, oSaveVersus);
        if (bValid == 1)
        {
          eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_REFLEX_SAVE_THROW_USE);
        }
      }
      else if (nSavingThrow == NWScript.SAVING_THROW_WILL)
      {
        bValid = NWScript.WillSave(oTarget, nDC, nSaveType, oSaveVersus);
        if (bValid == 1)
        {
          eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_WILL_SAVING_THROW_USE);
        }
      }

      nSpellID = NWScript.GetSpellId();

      /*
          return 0 = FAILED SAVE
          return 1 = SAVE SUCCESSFUL
          return 2 = IMMUNE TO WHAT WAS BEING SAVED AGAINST
      */
      if (bValid == 0)
      {
        if ((nSaveType == NWScript.SAVING_THROW_TYPE_DEATH
         || nSpellID == NWScript.SPELL_WEIRD
         || nSpellID == NWScript.SPELL_FINGER_OF_DEATH) &&
         nSpellID != NWScript.SPELL_HORRID_WILTING)
        {
          eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_DEATH);
          NWScript.DelayCommand(fDelay, () => NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, eVis, oTarget));
        }
      }
      //redundant comparison on bValid, let's move the eVis line down below
      /*    if(bValid == 2)
          {
              eVis = EffectVisualEffect(VFX_IMP_MAGIC_RESISTANCE_USE);
          }*/
      if (bValid == 1 || bValid == 2)
      {
        if (bValid == 2)
        {
          eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_MAGIC_RESISTANCE_USE);
          /*
          If the spell is save immune then the link must be applied in order to get the true immunity
          to be resisted.  That is the reason for returing false and not true.  True blocks the
          application of effects.
          */
          bValid = 0;
        }
        NWScript.DelayCommand(fDelay, () => NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, eVis, oTarget));
      }
      return bValid;
    }
  }
}
