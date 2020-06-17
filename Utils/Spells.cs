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

        public static int GetMyCasterLevel(uint oCreature)
        {
            int iMyCasterLevel = NWScript.GetCasterLevel(oCreature) + NWScript.GetLevelByClass(ClassType.Palemaster, oCreature)
                + (NWScript.GetLevelByClass(ClassType.DragonDisciple, oCreature) / 3) + (NWScript.GetLevelByClass(ClassType.DivineChampion, oCreature) / 2);

            //securite, min 1
            if (iMyCasterLevel < 1)
                iMyCasterLevel = 1;

            return iMyCasterLevel;

        }

        public static int MyResistSpell(uint oCaster, uint oTarget, float fDelay = 0.0f)
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
                int nCasterSRRoll = NWScript.d20(1) + GetMyCasterLevel(oCaster);
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

        // * Returns true if Target is a humanoid
        public static Boolean AmIAHumanoid(uint oTarget)
        {
            NWN.Enums.RacialType nRacial = NWScript.GetRacialType(oTarget);

            if ((nRacial == NWN.Enums.RacialType.Dwarf) ||
               (nRacial == NWN.Enums.RacialType.Halfelf) ||
               (nRacial == NWN.Enums.RacialType.Halforc) ||
               (nRacial == NWN.Enums.RacialType.Elf) ||
               (nRacial == NWN.Enums.RacialType.Gnome) ||
               (nRacial == NWN.Enums.RacialType.HumanoidGoblinoid) ||
               (nRacial == NWN.Enums.RacialType.Halfling) ||
               (nRacial == NWN.Enums.RacialType.Human) ||
               (nRacial == NWN.Enums.RacialType.HumanoidMonstrous) ||
               (nRacial == NWN.Enums.RacialType.HumanoidOrc) ||
               (nRacial == NWN.Enums.RacialType.HumanoidReptilian))
            {
                return true;
            }
            return false;
        }

        public static SaveReturn MySavingThrow(SavingThrow nSavingThrow, uint oTarget, int nDC, SavingThrowType nSaveType = SavingThrowType.All, uint oSaveVersus = NWObject.OBJECT_INVALID, float fDelay = 0.0f)
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

            if (!(oSaveVersus.AsObject()).IsValid)
                oSaveVersus = NWObject.OBJECT_SELF;

            Effect eVis = null;
            SaveReturn sReturn = SaveReturn.Failed;
            if (nSavingThrow == SavingThrow.Fortitude)
            {
                sReturn = NWScript.FortitudeSave(oTarget, nDC, nSaveType, oSaveVersus);
                if (sReturn == SaveReturn.Success)
                    eVis = NWScript.EffectVisualEffect((VisualEffect)Impact.FortitudeSavingThrowUse);
            }
            else if (nSavingThrow == SavingThrow.Reflex)
            {
                sReturn = NWScript.ReflexSave(oTarget, nDC, nSaveType, oSaveVersus);
                if (sReturn == SaveReturn.Success)
                {
                    eVis = NWScript.EffectVisualEffect((VisualEffect)Impact.ReflexSaveThrowUse);
                }
            }
            else if (nSavingThrow == SavingThrow.Will)
            {
                sReturn = NWScript.WillSave(oTarget, nDC, nSaveType, oSaveVersus);
                if (sReturn == SaveReturn.Success)
                {
                    eVis = NWScript.EffectVisualEffect((VisualEffect)Impact.WillSavingThrowUse);
                }
            }

            Spell nSpellID = (Spell)NWScript.GetSpellId();

            /*
                return 0 = FAILED SAVE
                return 1 = SAVE SUCCESSFUL
                return 2 = IMMUNE TO WHAT WAS BEING SAVED AGAINST
            */
            if (sReturn == SaveReturn.Failed)
            {
                if ((nSaveType == SavingThrowType.Death
                 || nSpellID == Spell.Weird
                 || nSpellID == Spell.FingerOfDeath) &&
                 nSpellID != Spell.HorridWilting)
                {
                    eVis = NWScript.EffectVisualEffect((VisualEffect)Impact.Death);
                    NWScript.DelayCommand(fDelay, () => NWScript.ApplyEffectToObject(DurationType.Instant, eVis, oTarget));
                }
            }

            if (sReturn == SaveReturn.Success || sReturn == SaveReturn.Immune)
            {
                if (sReturn == SaveReturn.Immune)
                {
                    eVis = NWScript.EffectVisualEffect((VisualEffect)Impact.MagicResistanceUse);
                    /*
                    If the spell is save immune then the link must be applied in order to get the true immunity
                    to be resisted.  That is the reason for returing false and not true.  True blocks the
                    application of effects.
                    */
                    //sReturn = SaveReturn.Failed;
                }
                NWScript.DelayCommand(fDelay, () => NWScript.ApplyEffectToObject(DurationType.Instant, eVis, oTarget));
            }
            return sReturn;
        }
    }
  }
}
