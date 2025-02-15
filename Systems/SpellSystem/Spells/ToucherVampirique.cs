using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> ToucherVampirique(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass castingClass, NwGameObject oTarget)
    {
      if (oCaster is NwCreature caster)
      {
        SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
        int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, castingClass.SpellCastingAbility);
        int nbDice = SpellUtils.GetSpellDamageDiceNumber(caster, spell);
        
        oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadEvil));

        DelayEffect(oCaster, EffectSystem.ToucherVampirique, SpellUtils.GetSpellDuration(oCaster, spellEntry));

        if (oTarget is NwCreature target)
        {
          var result = SpellUtils.GetSpellAttackRoll(target, caster, spell, castingClass.SpellCastingAbility, 0);
          if (result == TouchAttackResult.CriticalHit)
            nbDice = SpellUtils.GetCriticalSpellDamageDiceNumber(caster, spellEntry, nbDice);

          switch (result)
          {
            case TouchAttackResult.CriticalHit:
            case TouchAttackResult.Hit:

              int damage = SpellUtils.DealSpellDamage(target, caster.CasterLevel, spellEntry, nbDice, caster, 3);
              NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Instant, Effect.Heal(damage/2)));
              caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingM));
              oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpNegativeEnergy));

              break;
          }
        }
      }

      return new List<NwGameObject>() { oCaster };
    }

    public static void ToucherVampiriqueRecast(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass castingClass, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, castingClass.SpellCastingAbility);

      int nbDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, spell);

      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadEvil));

      DelayEffect(oCaster, EffectSystem.ToucherVampirique, SpellUtils.GetSpellDuration(oCaster, spellEntry));

      if (oTarget is NwCreature target)
      {
        var result = SpellUtils.GetSpellAttackRoll(target, oCaster, spell, castingClass.SpellCastingAbility, 0);
        if (result == TouchAttackResult.CriticalHit)
          nbDice = SpellUtils.GetCriticalSpellDamageDiceNumber(oCaster, spellEntry, nbDice);

        switch (result)
        {
          case TouchAttackResult.CriticalHit:
          case TouchAttackResult.Hit:

            int damage = SpellUtils.DealSpellDamage(target, 3, spellEntry, nbDice, oCaster, 3);
            NWScript.AssignCommand(oCaster, () => oCaster.ApplyEffect(EffectDuration.Instant, Effect.Heal(damage / 2)));
            oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingM));
            oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpNegativeEnergy));

            break;
        }
      }
    }
  }
}
