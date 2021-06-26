using NWN.API;
using NWN.Core;
using NWN.API.Constants;
using System.Linq;
using System;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static class SpellUtils
  {
    public static int MyResistSpell(uint oCaster, uint oTarget, float fDelay = 0.0f) // TODO : check la fonction par rapport à celle de never de base
    {
      if (fDelay > 0.5)
      {
        fDelay = fDelay - 0.1f;
      }
      int nResist = NWScript.ResistSpell(oCaster, oTarget);
      Effect eSR = Effect.VisualEffect(VfxType.ImpMagicResistanceUse);
      Effect eGlobe = Effect.VisualEffect(VfxType.ImpGlobeUse);
      Effect eMantle = Effect.VisualEffect(VfxType.ImpSpellMantleUse);

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

    public static int MaximizeOrEmpower(int nDice, int nNumberOfDice, MetaMagic nMeta, int nBonus = 0)
    {
      int i = 0;
      int nDamage = 0;
      for (i = 1; i <= nNumberOfDice; i++)
      {
        nDamage = nDamage + Utils.random.Next(nDice) + 1;
      }
      //Resolve metamagic
      if (nMeta == MetaMagic.Maximize)
      {
        nDamage = nDice * nNumberOfDice;
      }
      else if (nMeta == MetaMagic.Empower)
      {
        nDamage = nDamage + nDamage / 2;
      }
      return nDamage + nBonus;
    }
    public static void RemoveAnySpellEffects(int spell, uint oTarget)
    {
      NwCreature oCreature = oTarget.ToNwObject<NwCreature>();
                
      if (oCreature.HasSpellEffect((Spell)spell))
      {
        foreach(API.Effect eff in oCreature.ActiveEffects)
          if (NWScript.GetEffectSpellId(eff) == spell)
            oCreature.RemoveEffect(eff);
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

      Effect eVis = Effect.VisualEffect(VfxType.ImpFortitudeSavingThrowUse); 
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
    public static Spell GetSpellIDFromScroll(uint oScroll)
    {
        NwItem scroll = oScroll.ToNwObject<NwItem>();
        API.ItemProperty ip = scroll.ItemProperties.Where(ip => ip.PropertyType == ItemPropertyType.CastSpell).FirstOrDefault();

        if (ip != null && int.TryParse(NWScript.Get2DAString("iprp_spells", "SpellIndex", ip.SubType), out int spellId))
            return (Spell)spellId;

      return (Spell)(-1);
    }
    public static byte GetSpellLevelFromScroll(uint oScroll)
    {
        NwItem scroll = oScroll.ToNwObject<NwItem>();
        API.ItemProperty ip = scroll.ItemProperties.Where(ip => ip.PropertyType == ItemPropertyType.CastSpell).FirstOrDefault();

        if (ip != null)
            return (byte)(float.Parse(NWScript.Get2DAString("iprp_spells", "InnateLvl", ip.SubType)));

      return 255;
    }
    public static int GetSpellSchoolFromString(string school)
    {
      return "GACDEVINT".IndexOf(school);
    }
    public static void ApplyCustomEffectToTarget(NwGameObject target, string effectTag, int iconId, int effectDuration = 0)
    {
      API.Effect eff = API.Effect.HitPointChangeWhenDying(1);
      eff.Tag = effectTag;
      eff.SubType = EffectSubType.Supernatural;

      target.GetLocalVariable<int>(effectTag).Value = iconId;

      if (effectDuration > 0)
        target.ApplyEffect(EffectDuration.Temporary, eff, TimeSpan.FromSeconds(effectDuration));
      else
        target.ApplyEffect(EffectDuration.Permanent, eff);
      
      ObjectPlugin.AddIconEffect(target, iconId, effectDuration);
    }
    public static async void RestoreSpell(NwCreature caster, Spell spell)
    {
      if (caster == null)
        return;

      await NwTask.Delay(TimeSpan.FromSeconds(0.2));

      foreach (MemorizedSpellSlot spellSlot in caster.GetClassInfo((ClassType)43).GetMemorizedSpellSlots(0).Where(s => s.Spell == spell && !s.IsReady))
        spellSlot.IsReady = true;
    }
    public static async void CancelCastOnMovement(NwCreature caster)
    {
      float posX = caster.Position.X;
      float posY = caster.Position.Y;
      await NwTask.WaitUntil(() => caster.Position.X != posX || caster.Position.Y != posY);

      caster.GetLocalVariable<int>("_AUTO_SPELL").Delete();
      caster.GetLocalVariable<NwObject>("_AUTO_SPELL_TARGET").Delete();
      caster.OnCombatRoundEnd -= PlayerSystem.HandleCombatRoundEndForAutoSpells;
    }
  }
}
