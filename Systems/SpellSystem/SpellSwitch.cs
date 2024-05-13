using System;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static void SpellSwitch(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject target, Location targetLocation, NwClass castingClass)
    {
      switch (spell.SpellType)
      {
        case Spell.AcidSplash:
          SpellSystem.AcidSplash(oCaster, spell, spellEntry, target, targetLocation, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.ElectricJolt:
          SpellSystem.ElectricJolt(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Light:
          SpellSystem.Light(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.RayOfFrost:
          SpellSystem.RayOfFrost(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.TrueStrike:
          SpellSystem.TrueStrike(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Flare:
          SpellSystem.SacredFlame(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        /* case Spell.Virtue:
           HealingBreeze(onSpellCast, durationModifier);
           oPC.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
           break;*/

        /*case Spell.RaiseDead:
        case Spell.Resurrection:
          new RaiseDead(onSpellCast);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;*/

        case Spell.Invisibility:
          SpellSystem.Invisibility(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.ImprovedInvisibility:
          SpellSystem.ImprovedInvisibility(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        /*case Spell.FleshToStone:
          Petrify(onSpellCast);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;*/

        case Spell.Darkness:
          SpellSystem.Darkness(oCaster, spell, spellEntry, target, targetLocation);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Silence:
          if(oCaster is NwCreature caster && caster.GetObjectVariable<LocalVariableInt>("_CAST_FROM_SHADOW_MONK_FEAT").Value == CustomSkill.MonkSilence)
          {
            caster.IncrementRemainingFeatUses((Feat)CustomSkill.MonkSilence);
            FeatUtils.DecrementKi(caster, 2);
            caster.GetObjectVariable<LocalVariableInt>("_CAST_FROM_SHADOW_MONK_FEAT").Delete();
          }
          break;

        case Spell.Darkvision:
          if (oCaster is NwCreature castCreature && castCreature.GetObjectVariable<LocalVariableInt>("_CAST_FROM_SHADOW_MONK_FEAT").Value == CustomSkill.MonkDarkVision)
          {
            castCreature.IncrementRemainingFeatUses((Feat)CustomSkill.MonkDarkVision);
            FeatUtils.DecrementKi(castCreature, 2);
            castCreature.GetObjectVariable<LocalVariableInt>("_CAST_FROM_SHADOW_MONK_FEAT").Delete();
          }
          break;

        case Spell.BurningHands:
          SpellSystem.BurningHands(oCaster, spell, spellEntry, target, castingClass, targetLocation);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;
      }

      switch (spell.Id)
      {
        case CustomSpell.BladeWard:
          SpellSystem.BladeWard(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FireBolt:
          SpellSystem.FireBolt(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Friends:
          SpellSystem.Friends(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.BoneChill:
          SpellSystem.BoneChill(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.PoisonSpray:
          SpellSystem.PoisonSpray(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FaerieFire:
          SpellSystem.FaerieFire(oCaster, spell, spellEntry, target, targetLocation, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Enlarge:
          SpellSystem.Enlarge(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.SpeakAnimal:
          SpellSystem.SpeakAnimal(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ProduceFlame:
          SpellSystem.ProduceFlame(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.MageHand:
          SpellSystem.MageHand(oCaster, spell, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Thaumaturgy:
          SpellSystem.Thaumaturgy(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Serenity:
          SpellSystem.Serenity(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Sprint:
          SpellSystem.Sprint(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Stealth:
          SpellSystem.Stealth(oCaster);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Disengage:
          SpellSystem.Disengage(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Dodge:
          SpellSystem.Dodge(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.TirPerforant:
          SpellSystem.TirPerforant(oCaster, spell, spellEntry, target, targetLocation);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.PresenceIntimidante:
          SpellSystem.PresenceIntimidante(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FlameBlade:
          SpellSystem.FlameBlade(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.SearingSmite:
          SpellSystem.SearingSmite(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.BrandingSmite:
          SpellSystem.BrandingSmite(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.SensAnimal:
          SpellSystem.SensAnimal(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.HurlementGalvanisant:
          SpellSystem.HurlementGalvanisant(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.PassageSansTrace:
          SpellSystem.PassageSansTrace(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.RegardHypnotique:
          SpellSystem.RegardHypnotique(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.IllusionMineure:
          SpellSystem.IllusionMineure(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.InvocationPermutation:
          SpellSystem.InvocationPermutation(oCaster, spell, spellEntry, target, targetLocation);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;
      }

      if (oCaster is NwCreature castingCreature && castingClass is not null)
      {
        SpellSystem.OnSpellCastAbjurationWard(castingCreature, spell, spell.GetSpellLevelForClass(castingClass));
        SpellSystem.OnSpellCastDivinationExpert(castingCreature, spell, castingClass);
        SpellSystem.OnSpellCastInvocationPermutation(castingCreature, spell, spell.GetSpellLevelForClass(castingClass));
        SpellSystem.OnSpellCastTransmutationStone(castingCreature, spell, spell.GetSpellLevelForClass(castingClass));
      }

      oCaster.GetObjectVariable<LocalVariableInt>(SpellConfig.CurrentSpellVariable).Delete();
      oCaster.GetObjectVariable<DateTimeLocalVariable>("_LAST_ACTION_DATE").Value = DateTime.Now;
    }
  }
}
