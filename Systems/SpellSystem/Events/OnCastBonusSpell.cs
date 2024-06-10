using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void OnCastBonusSpell(OnSpellAction onSpellAction)
    {
      SpellEntry spellEntry = Spells2da.spellTable[onSpellAction.Spell.Id];
      NwSpell spell = onSpellAction.Spell;
      NwCreature caster = onSpellAction.Caster;

      if (spellEntry.isBonusAction || (spell.Id == CustomSpell.MageHand && caster.KnowsFeat((Feat)CustomSkill.ArcaneTricksterPolyvalent)))
      {
        if (!CreatureUtils.HandleBonusActionUse(caster))
          return;

        if (!SpellUtils.CanCastSpell(caster, onSpellAction.TargetObject, spell, spellEntry))
        {
          onSpellAction.PreventSpellCast = true;
          return;
        }

        SpellUtils.CheckDispelConcentration(caster, spell, spellEntry);
        SpellUtils.HandlePhlegetos(caster, spellEntry);
        EffectUtils.RemoveEffectType(caster, EffectType.Invisibility, EffectType.ImprovedInvisibility);

        LogUtils.LogMessage($"{caster.Name} - Sort {spell.Name.ToString()} ({spell.Id}) lancé en action bonus", LogUtils.LogType.Combat);
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - {spell.Name.ToString().ColorString(ColorConstants.Orange)} - Action Bonus", StringUtils.gold, true, true);

        SpellUtils.SpellSwitch(caster, onSpellAction.Spell, onSpellAction.Feat, spellEntry, onSpellAction.TargetObject, Location.Create(onSpellAction.Caster.Area, onSpellAction.Caster.Position, onSpellAction.Caster.Rotation), onSpellAction.Caster.Classes[onSpellAction.ClassIndex].Class);

        onSpellAction.PreventSpellCast = true;
      }
    }
  }
}
