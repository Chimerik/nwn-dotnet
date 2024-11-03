using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> AppelDeLaFoudre(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass castingClass, Location targetLocation)
    {
      if (oCaster is NwCreature caster)
      {
        SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
        int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, castingClass.SpellCastingAbility);

        targetLocation.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpLightningM));
        oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadElectricity));

        if (caster.GetObjectVariable<LocalVariableInt>(EffectSystem.ConcentrationSpellIdString).Value != (int)Spell.CallLightning)
          caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.AppelDeLaFoudre, SpellUtils.GetSpellDuration(oCaster, spellEntry));
        else
          caster.GetObjectVariable<LocalVariableInt>("_FREE_SPELL").Value = 1;

        foreach (NwCreature target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
        {
          SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell), oCaster, spell.GetSpellLevelForClass(castingClass), CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, spellDC));

          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ComHitElectrical));
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfScreenBump));
        }
      }

      return new List<NwGameObject>() { oCaster };
    }
  }
}
