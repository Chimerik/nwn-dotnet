using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static bool HandleSpellTargetIncapacitated(NwCreature caster, NwCreature target, EffectType effectType, SpellEntry spellEntry)
    {
      if (spellEntry.savingThrowAbility != Ability.Dexterity && spellEntry.savingThrowAbility != Ability.Strength)
        return false;

      if (EffectUtils.IsIncapacitatingEffect(effectType))
      {
        NwSpell spell = NwSpell.FromSpellId(spellEntry.RowIndex);
        SendSaveAutoFailMessage(caster, target, spell.Name.ToString(), StringUtils.TranslateAttributeToFrench(spellEntry.savingThrowAbility));

        switch(NwSpell.FromSpellId(spellEntry.RowIndex).SpellType)
        {
          case Spell.Light: SpellSystem.ApplyLightEffect(target); return true;
        }

        DealSpellDamage(target, caster.LastSpellCasterLevel, spellEntry.damageDice, GetSpellDamageDiceNumber(caster, spell), DamageType.Acid, VfxType.ImpAcidS);

        return true;
      }

      return false;
    }
  }
}
