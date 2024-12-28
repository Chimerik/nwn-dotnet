using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> ProtectionContreLeMalEtLeBien(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      List<NwGameObject> targets = new();

      if (oCaster is not NwCreature caster)
        return targets;

      if (caster.Gold < 25)
      {
        caster.LoginPlayer?.SendServerMessage("Vous devez être en possession de 25 po afin de faire usage de ce sort", ColorConstants.Red);
        return targets;
      }

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);

      foreach (var target in targets)
      {
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(spellEntry.damageVFX));
        target.ApplyEffect(EffectDuration.Temporary, EffectSystem.ProtectionContreLeMalEtLeBien, SpellUtils.GetSpellDuration(oCaster, spellEntry));
      }

      caster.Gold -= 25;
      return targets;
    }
  }
}
