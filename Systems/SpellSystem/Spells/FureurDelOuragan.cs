using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void FureurDelOuragan(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadElectricity));

      EffectUtils.RemoveTaggedEffect(oCaster, EffectSystem.FureurDelOuraganEffectTag);

      NWScript.AssignCommand(oCaster, () => oCaster.ApplyEffect(EffectDuration.Permanent, EffectSystem.FureurDeLOuragan(spell)));
    }
  }
}
