using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static bool HandleSpellTargetIncapacitated(NwGameObject oCaster, NwCreature target, Effect effect, SpellEntry spellEntry, byte spellLevel)
    {
      if (EffectUtils.IsIncapacitatingEffect(effect))
      {
        NwSpell spell = NwSpell.FromSpellId(spellEntry.RowIndex);
        SendSaveAutoFailMessage(oCaster, target, spell.Name.ToString(), StringUtils.TranslateAttributeToFrench(spellEntry.savingThrowAbility));

        switch(NwSpell.FromSpellId(spellEntry.RowIndex).SpellType)
        {
          case Spell.Light: SpellSystem.ApplyLightEffect(oCaster, target); return true;
        }

        switch (NwSpell.FromSpellId(spellEntry.RowIndex).Id)
        {
          case CustomSpell.FaerieFire: SpellSystem.ApplyFaerieFireEffect(target, spellEntry); return true;
          case CustomSpell.TirPerforant: 

            if(oCaster is NwCreature caster)
              SpellSystem.ApplyTirPerforantDamage(caster, target);

            return true;
        }

        DealSpellDamage(target, oCaster.CasterLevel, spellEntry, GetSpellDamageDiceNumber(oCaster, spell), oCaster, spellLevel);

        return true;
      }

      return false;
    }
  }
}
