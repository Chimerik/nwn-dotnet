
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Bouclier(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      oCaster.ApplyEffect(EffectDuration.Temporary, Effect.VisualEffect(VfxType.ImpGlobeUse));

      EffectSystem.ApplyBouclier(caster);
    }
  }
}
