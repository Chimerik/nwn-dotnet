using System;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN
{
  public static class Spells
  {
    public static void LogException(Exception e)
    {
      Console.WriteLine(e.Message);
      NWScript.SendMessageToAllDMs(e.Message);
      NWScript.WriteTimestampedLogEntry(e.Message);
      WebhookPlugin.SendWebHookHTTPS("discordapp.com", "/api/webhooks/737378235402289264/3-nDoj7dEw-edzjM-DDyjWFCZbs6LXACoJ9vFnOWXc8Pn2nArFEt3HiVIhHyu_lYiNUt/slack", e.Message, "AOA Errors");
    }

    public static int MyResistSpell(uint oCaster, uint oTarget, float fDelay = 0.0f) // C'est une fonction de cde, j'ai aucun idée de pourquoi ça fait ce que ça fait de cette manière là. Mieux vaut nous en débarrasser
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
  }
}
