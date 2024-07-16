using Anvil.API;

namespace NWN.Systems
{
  public partial class WizardUtils
  {
    public static async void HandleMoissonDuFiel(NwGameObject oCaster, NwGameObject oTarget, bool trigger, NwSpell spell, byte spellLevel)
    {
      await NwTask.NextFrame();

      if (!trigger || oTarget.HP > 0 || spellLevel < 1 || oCaster is not NwCreature caster || !caster.KnowsFeat((Feat)CustomSkill.NecromancieMoissonDuFiel))
        return;

      int multiplier = spell.SpellSchool == SpellSchool.Necromancy ? 3 : 2;
      caster.ApplyEffect(EffectDuration.Instant, Effect.Heal(spellLevel * multiplier));
    }
  }
}
