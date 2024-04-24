using Anvil.API;

namespace NWN.Systems
{
  public partial class WizardUtils
  {
    public static void HandleMoissonDuFiel(NwGameObject oCaster, NwSpell spell, byte spellLevel)
    {
      if (spellLevel < 1 || oCaster is not NwCreature caster || !caster.KnowsFeat((Feat)CustomSkill.NecromancieMoissonDuFiel))
        return;

      int multiplier = spell.SpellSchool == SpellSchool.Necromancy ? 3 : 2;
      caster.ApplyEffect(EffectDuration.Instant, Effect.Heal(spellLevel * multiplier));

    }
  }
}
