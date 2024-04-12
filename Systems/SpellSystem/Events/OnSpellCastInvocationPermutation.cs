using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void OnSpellCastInvocationPermutation(NwCreature caster, SpellEvents.OnSpellCast spellCast)
    {
      if (spellCast.Spell.SpellSchool != SpellSchool.Conjuration || !caster.KnowsFeat((Feat)CustomSkill.InvocationPermutation)
        || spellCast.SpellLevel < 1 || caster.GetFeatRemainingUses((Feat)CustomSkill.InvocationPermutation) > 0)
        return;

      caster.SetFeatRemainingUses((Feat)CustomSkill.InvocationPermutation, 1);
    }
  }
}
