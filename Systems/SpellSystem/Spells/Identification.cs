
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Identification(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      if (oCaster is not NwCreature caster)
        return;

      if (caster.Gold < 100)
      {
        caster.LoginPlayer?.SendServerMessage("Vous devez être en possession de 100 po afin de faire usage de ce sort", ColorConstants.Red);
        return;
      }

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      caster.Gold -= 100;
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpMagicalVision));

      if (oTarget is not NwItem target)
        return;
      
      target.Identified = true;
      caster.LoginPlayer?.SendServerMessage($"Vous connaissez désormais les propriétés magiques de {target.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
    }  
  }
}
