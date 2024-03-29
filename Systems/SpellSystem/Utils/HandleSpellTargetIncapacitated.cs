﻿using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static bool HandleSpellTargetIncapacitated(NwCreature caster, NwCreature target, Effect effect, SpellEntry spellEntry)
    {
      if (EffectUtils.IsIncapacitatingEffect(effect))
      {
        NwSpell spell = NwSpell.FromSpellId(spellEntry.RowIndex);
        SendSaveAutoFailMessage(caster, target, spell.Name.ToString(), StringUtils.TranslateAttributeToFrench(spellEntry.savingThrowAbility));

        switch(NwSpell.FromSpellId(spellEntry.RowIndex).SpellType)
        {
          case Spell.Light: SpellSystem.ApplyLightEffect(target); return true;
        }

        switch (NwSpell.FromSpellId(spellEntry.RowIndex).Id)
        {
          case CustomSpell.FaerieFire: SpellSystem.ApplyFaerieFireEffect(target, spellEntry); return true;
        }

        DealSpellDamage(target, caster.CasterLevel, spellEntry, GetSpellDamageDiceNumber(caster, spell), caster);

        return true;
      }

      return false;
    }
  }
}
