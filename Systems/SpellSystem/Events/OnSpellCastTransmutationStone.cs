using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void OnSpellCastTransmutationStone(NwCreature caster, SpellEvents.OnSpellCast spellCast)
    {
      if (spellCast.Spell.SpellSchool != SpellSchool.Transmutation || !caster.KnowsFeat((Feat)CustomSkill.TransmutationStone)
        || spellCast.SpellLevel < 1 || caster.GetObjectVariable<LocalVariableInt>(Wizard.TransmutationStoneVariable).HasValue)
        return;

      caster.GetObjectVariable<LocalVariableInt>(Wizard.TransmutationStoneVariable).Value = 1;
    }
  }
}
