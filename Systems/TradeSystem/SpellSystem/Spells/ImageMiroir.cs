using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ImageMiroir(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      oCaster.GetObjectVariable<LocalVariableInt>(EffectSystem.ImageMiroirEffectTag).Value = 3;
      NWScript.AssignCommand(oCaster, () => oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.ImageMiroir, NwTimeSpan.FromRounds(spellEntry.duration)));
    }
  }
}
