using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void SpeakAnimal(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.speakAnimalEffect, SpellUtils.GetSpellDuration(oCaster, spellEntry));
    }
  }
}
