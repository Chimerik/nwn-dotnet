using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class WizardUtils
  {
    public static void HandleMoissonDuFiel(NwCreature caster, NwSpell spell, byte spellLevel)
    {
      if (!caster.KnowsFeat((Feat)CustomSkill.NecromancieMoissonDuFiel) || spellLevel < 1)
        return;

      int multiplier = spell.SpellSchool == SpellSchool.Necromancy ? 3 : 2;
      caster.ApplyEffect(EffectDuration.Instant, Effect.Heal(spellLevel * multiplier));
    }
  }
}
