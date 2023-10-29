using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public void OnBonusActionSpell(OnSpellAction onSpell)
    {
      SpellEntry spellEntry = Spells2da.spellTable[onSpell.Spell.Id];

      if (spellEntry.bonusAction == 1)
      {
        onSpell.PreventSpellCast = true;

        if (onSpell.Caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value > 0)
        {
          if (onSpell.Caster.ActiveEffects.Any(e => e.Tag == EffectSystem.ShieldArmorDisadvantageEffectTag))
            return;

          bool flameBladeRecast = false;

          if (onSpell.Spell.Id == CustomSpell.FlameBlade && onSpell.Caster.GetObjectVariable<LocalVariableInt>(EffectSystem.ConcentrationSpellIdString).Value == CustomSpell.FlameBlade)
          {
            flameBladeRecast = true;
            // TODO : Si on recast Flame Blade, alors on ne compte pas un nouvel emplacement de sort
          }

          if (!flameBladeRecast && spellEntry.requiresConcentration 
            && onSpell.Caster.ActiveEffects.Any(e => e.Tag == EffectSystem.ConcentrationEffectTag))
            SpellUtils.DispelConcentrationEffects(onSpell.Caster);

          switch (onSpell.Spell.Id)
          {
            case CustomSpell.FlameBlade: FlameBlade(onSpell.Caster, onSpell.Spell, spellEntry, flameBladeRecast); break;
            case CustomSpell.SearingSmite: SearingSmite(onSpell.Caster, onSpell.Spell, spellEntry); break;
            case CustomSpell.BrandingSmite: BrandingSmite(onSpell.Caster, onSpell.Spell, spellEntry); break;
          }

          onSpell.Caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value -= 1;
        }
        else
          onSpell.Caster?.ControllingPlayer.SendServerMessage("Vous avez déjà utilisé votre action bonus de ce round", ColorConstants.Red);
      }
    }
  }
}
