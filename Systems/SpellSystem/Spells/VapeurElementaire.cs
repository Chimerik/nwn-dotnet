
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void VapeurElementaire(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      foreach (NwCreature target in oCaster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
      {
        if (caster.IsReactionTypeHostile(target))
        {
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPoisonS));
          SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell), oCaster, 3);
        }
        else
        {
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingM));
          target.ApplyEffect(EffectDuration.Instant, Effect.Heal(Utils.Roll(8, 2)));
        }
      }

      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPulseWater));
    }
  }
}
