
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void FaveurDuMalin(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      EffectUtils.RemoveTaggedEffect(oCaster, EffectSystem.FaveurDuMalinEffectTag);

      NWScript.AssignCommand(oCaster, () => oCaster.ApplyEffect(EffectDuration.Permanent, EffectSystem.FaveurDuMalin(spell.Id)));
    }
  }
}
