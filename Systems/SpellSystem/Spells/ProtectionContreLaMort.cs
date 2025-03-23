using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ProtectionContreLaMort(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType, false);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);

      foreach (var target in targets)
      {
        if (target is NwCreature targetCreature)
        {
          EffectUtils.RemoveTaggedEffect(targetCreature, EffectSystem.ProtectionContreLaMortEffectTag);
          NWScript.AssignCommand(oCaster, () => targetCreature.ApplyEffect(EffectDuration.Temporary, EffectSystem.ProtectionContreLaMort, SpellUtils.GetSpellDuration(oCaster, spellEntry)));

          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDeathWard));
        }
      }
    }
  }
}
