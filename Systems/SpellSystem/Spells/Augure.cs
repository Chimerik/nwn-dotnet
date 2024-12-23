
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Augure(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;

      if (caster.Gold < 25)
      {
        caster.LoginPlayer?.SendServerMessage("Vous devez être en possession de 25 po afin de faire usage de ce sort", ColorConstants.Red);
        return;
      }

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      caster.Gold -= 25;
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpMagicalVision));
    }
  }
}
