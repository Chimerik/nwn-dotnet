
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ImpositionDesMainsMineure(NwGameObject oCaster, NwGameObject oTarget, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oTarget, spell.SpellType);

      int healAmount = 2 * caster.GetClassInfo(ClassType.Paladin).Level;

      oTarget.ApplyEffect(EffectDuration.Instant, Effect.Heal(healAmount));
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingM));
    }
  }
}
