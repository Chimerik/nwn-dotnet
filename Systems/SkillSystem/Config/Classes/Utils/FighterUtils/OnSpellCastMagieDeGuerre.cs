﻿using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class FighterUtils
  {
    public static void OnSpellCastMagieDeGuerre(OnSpellAction onSpellAction)
    {
      SpellEntry spellEntry = Spells2da.spellTable[onSpellAction.Spell.Id];
     
      if (spellEntry.isBonusAction)
        return;

      NwSpell spell = onSpellAction.Spell;
      NwCreature caster = onSpellAction.Caster;

      if ((spell.GetSpellLevelForClass((ClassType)CustomClass.EldritchKnight) < 1 
        || caster.GetClassInfo((ClassType)CustomClass.EldritchKnight).Level > 17)
        && caster.ActiveEffects.Any(e => e.Tag == EffectSystem.MagieDeGuerreEffectTag))
      {
        if (!SpellUtils.CanCastSpell(caster, onSpellAction.TargetObject, spell, spellEntry))
        {
          onSpellAction.PreventSpellCast = true;
          return;
        }

        if (!CreatureUtils.HandleBonusActionUse(caster))
          return;

        if (caster.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.MagieDeGuerreEffectTag).Creator is NwGameObject target)
          _ = caster.ActionAttackTarget(target);

        SpellUtils.CheckDispelConcentration(caster, spell, spellEntry);
        SpellUtils.HandlePhlegetos(caster, spellEntry);

        if (!caster.KnowsFeat((Feat)CustomSkill.WizardIllusionAmelioree) && spell.SpellType != (Spell)CustomSpell.IllusionMineure)
          EffectUtils.RemoveEffectType(caster, EffectType.Invisibility, EffectType.ImprovedInvisibility);

        LogUtils.LogMessage($"{caster.Name} - Mage de guerre - Sort {spell.Name.ToString()} ({spell.Id}) lancé en action bonus", LogUtils.LogType.Combat);
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - {spell.Name.ToString().ColorString(ColorConstants.Orange)} - Mage de guerre", StringUtils.gold, true, true);

        NwClass castingClass = onSpellAction.ClassIndex < 255 ? onSpellAction.Caster.Classes[onSpellAction.ClassIndex].Class : NwClass.FromClassId(CustomClass.Adventurer);

        SpellUtils.SpellSwitch(caster, onSpellAction.Spell, onSpellAction.Feat, spellEntry, onSpellAction.TargetObject, Location.Create(onSpellAction.Caster.Area, onSpellAction.TargetPosition, onSpellAction.Caster.Rotation), castingClass);
        
        onSpellAction.PreventSpellCast = true;
      }
    }
  }
}
