﻿using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void OnSpellCastInvocationPermutation(NwCreature caster, NwSpell spell, int spellLevel)
    {
      if (spell.SpellSchool != SpellSchool.Conjuration || spell.Id != CustomSpell.InvocationPermutation
        || !caster.KnowsFeat((Feat)CustomSkill.InvocationPermutation)
        || spellLevel < 1 || spellLevel > 9 || caster.GetFeatRemainingUses((Feat)CustomSkill.InvocationPermutation) > 0)
        return;

      caster.SetFeatRemainingUses((Feat)CustomSkill.InvocationPermutation, 1);
    }
  }
}
