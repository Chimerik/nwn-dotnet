﻿using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class PaladinUtils
  {
    public static void OnSpellChampionAntique(OnSpellAction onSpellAction)
    {
      SpellEntry spellEntry = Spells2da.spellTable[onSpellAction.Spell.Id];

      if (spellEntry.isBonusAction || onSpellAction.Caster.Classes[onSpellAction.ClassIndex].Class.ClassType != ClassType.Paladin)
        return;

      NwSpell spell = onSpellAction.Spell;
      NwCreature caster = onSpellAction.Caster;

      if (!SpellUtils.CanCastSpell(caster, onSpellAction.TargetObject, spell, spellEntry))
      {
        onSpellAction.PreventSpellCast = true;
        return;
      }

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      SpellUtils.CheckDispelConcentration(caster, spell, spellEntry);
      SpellUtils.HandlePhlegetos(caster, spellEntry);

      if (!caster.KnowsFeat((Feat)CustomSkill.WizardIllusionAmelioree) && spell.SpellType != (Spell)CustomSpell.IllusionMineure)
        EffectUtils.RemoveEffectType(caster, EffectType.Invisibility, EffectType.ImprovedInvisibility);

      LogUtils.LogMessage($"{caster.Name} - Champion Antique - Sort {spell.Name.ToString()} ({spell.Id}) lancé en action bonus", LogUtils.LogType.Combat);
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - {spell.Name.ToString().ColorString(ColorConstants.Orange)} - Champion Antique", StringUtils.gold, true, true);

      NwClass castingClass = onSpellAction.ClassIndex < 255 ? onSpellAction.Caster.Classes[onSpellAction.ClassIndex].Class : NwClass.FromClassType(ClassType.Paladin);

      SpellUtils.SpellSwitch(caster, onSpellAction.Spell, onSpellAction.Feat, spellEntry, onSpellAction.TargetObject, Location.Create(onSpellAction.Caster.Area, onSpellAction.TargetPosition, onSpellAction.Caster.Rotation), castingClass);

      onSpellAction.PreventSpellCast = true;
    }
  }
}
