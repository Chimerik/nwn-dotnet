using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void OnSpellCastTransmutationStone(NwCreature caster, NwSpell spell, int spellLevel)
    {
      if (spell.SpellSchool != SpellSchool.Transmutation || !caster.KnowsFeat((Feat)CustomSkill.TransmutationStone)
        || spellLevel < 1 || caster.GetObjectVariable<LocalVariableInt>(Wizard.TransmutationStoneVariable).HasValue)
        return;

      caster.GetObjectVariable<LocalVariableInt>(Wizard.TransmutationStoneVariable).Value = 1;
    }
  }
}
