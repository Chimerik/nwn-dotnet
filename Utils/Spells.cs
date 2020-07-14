using System;
using System.Collections.Generic;
using NWN.Enums;
using NWN.Enums.Item.Property;
using NWN.Enums.VisualEffect;

namespace NWN
{
  public static class Spells
  {
    public static void LogException(Exception e)
    {
      Console.WriteLine(e.Message);
      NWScript.SendMessageToAllDMs(e.Message);
      NWScript.WriteTimestampedLogEntry(e.Message);
    }

    public static int MyResistSpell(uint oCaster, uint oTarget, float fDelay = 0.0f) // C'est une fonction de cde, j'ai aucun idée de pourquoi ça fait ce que ça fait de cette manière là. Mieux vaut nous en débarrasser
    {
      if (fDelay > 0.5)
      {
        fDelay = fDelay - 0.1f;
      }
      int nResist = NWScript.ResistSpell(oCaster, oTarget);
      Effect eSR = NWScript.EffectVisualEffect((VisualEffect)Impact.MagicResistanceUse);
      Effect eGlobe = NWScript.EffectVisualEffect((VisualEffect)Impact.GlobeUse);
      Effect eMantle = NWScript.EffectVisualEffect((VisualEffect)Impact.SpellMantleUse);
      if (nResist == 1) //Spell Resistance
      {
        //Ici oCaster a perdu le test de RM contre celle de oTarget
        //Le jet de oCaster est :1d20 + NDL + les dons
        //On va ajouter +1 par ecole sup/renf abjuration
        int nTargetSR = NWScript.GetSpellResistance(oTarget);
        int nCasterSRRoll = NWScript.d20(1) + oCaster.AsCreature().CasterLevel();
        if (NWScript.GetHasFeat((int)NWN.Enums.Feat.SpellFocus_Abjuration, oCaster)) { nCasterSRRoll = nCasterSRRoll + 2; }
        if (NWScript.GetHasFeat((int)NWN.Enums.Feat.GreaterSpellFocus_Abjuration, oCaster)) { nCasterSRRoll = nCasterSRRoll + 2; }
        if (NWScript.GetHasFeat((int)NWN.Enums.Feat.EpicSpellFocus_Abjuration, oCaster)) { nCasterSRRoll = nCasterSRRoll + 2; }


        if (nCasterSRRoll < nTargetSR) //oCaster passe pas la RM
          NWScript.DelayCommand((float)fDelay, () => NWScript.ApplyEffectToObject((int)DurationType.Instant, eSR, oTarget));
        else
          nResist = 0;
      }
      else if (nResist == 2) //Globe
      {
        NWScript.DelayCommand(fDelay, () => NWScript.ApplyEffectToObject((int)DurationType.Instant, eGlobe, oTarget));
      }
      else if (nResist == 3) //Spell Mantle
      {
        if (fDelay > 0.5)
        {
          fDelay = fDelay - 0.1f;
        }

        NWScript.DelayCommand(fDelay, () => NWScript.ApplyEffectToObject((int)DurationType.Instant, eMantle, oTarget));
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
      if (nMeta == (int)MetaMagic.Maximize)
      {
        nDamage = nDice * nNumberOfDice;
      }
      else if (nMeta == (int)MetaMagic.Empower)
      {
        nDamage = nDamage + nDamage / 2;
      }
      return nDamage + nBonus;
    }
    public static void RemoveAnySpellEffects(Spell spell, uint oTarget)
    {
      if (NWScript.GetHasSpellEffect(spell, oTarget))
      {
        foreach (Effect e in oTarget.AsObject().Effects)
        {
          if (NWScript.GetEffectSpellId(e) == (int)spell)
          {
            NWScript.RemoveEffect(oTarget, e);
          }
        }
      }
    }
    public static Boolean GetHasEffect(int effectType, uint oTarget)
    {
        foreach (Effect e in oTarget.AsObject().Effects)
        {
          if (NWScript.GetEffectType(e) == effectType)
            return true;
        }

        return false;
    }
    public static void RemoveEffectOfType(int effectType, uint oTarget)
    {
      foreach (Effect e in oTarget.AsObject().Effects)
      {
        if (NWScript.GetEffectType(e) == effectType)
          NWScript.RemoveEffect(oTarget, e);
      }
    }
  }
}
