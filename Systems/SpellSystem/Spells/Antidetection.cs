using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Antidetection(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
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
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpMagicalVision));
      NWScript.AssignCommand(caster, () => oTarget.ApplyEffect(EffectDuration.Temporary, EffectSystem.Antidetection, SpellUtils.GetSpellDuration(oCaster, spellEntry)));
    }  
  }
}
