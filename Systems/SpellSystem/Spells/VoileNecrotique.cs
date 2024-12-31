using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void VoileNecrotique(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);      
      NWScript.AssignCommand(oCaster, () => oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.VoileNecrotique, NwTimeSpan.FromRounds(spellEntry.duration)));

      if (oCaster is NwCreature caster)
        caster.DecrementRemainingFeatUses((Feat)CustomSkill.RevelationCeleste);
    }
  }
}
