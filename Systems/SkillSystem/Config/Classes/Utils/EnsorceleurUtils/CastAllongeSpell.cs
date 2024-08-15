using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class EnsoUtils
  {
    public static void CastAllongeSpell(OnSpellCast onCast)
    {
      if (onCast.Caster is NwCreature caster && caster.ActiveEffects.Any(e => e.Tag == EffectSystem.MetamagieEffectTag && e.IntParams[5] == CustomSkill.EnsoAllonge))
      {
        EffectUtils.RemoveTaggedParamEffect(caster, CustomSkill.EnsoAllonge, EffectSystem.MetamagieEffectTag);

        NwSpell spell = onCast.Spell;
        SpellEntry spellEntry = Spells2da.spellTable[spell.Id];

        if (SpellUtils.CanCastSpell(caster, caster, onCast.Spell, spellEntry))
        {
          SpellUtils.CheckDispelConcentration(caster, spell, spellEntry);
          SpellUtils.HandlePhlegetos(caster, spellEntry);

          if (!caster.KnowsFeat((Feat)CustomSkill.WizardIllusionAmelioree) && spell.SpellType != (Spell)CustomSpell.IllusionMineure)
            EffectUtils.RemoveEffectType(caster, EffectType.Invisibility, EffectType.ImprovedInvisibility);

          LogUtils.LogMessage($"{caster.Name} - Sort {spell.Name.ToString()} ({spell.Id}) lancé avec allonge", LogUtils.LogType.Combat);
          StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - {spell.Name.ToString().ColorString(ColorConstants.Orange)} - Allonge", StringUtils.gold, true, true);

          NwClass castingClass = onCast.ClassIndex < 255 ? caster.Classes[onCast.ClassIndex].Class : NwClass.FromClassId(CustomClass.Adventurer);
          SpellUtils.SpellSwitch(caster, spell, null, spellEntry, onCast.Caster.GetObjectVariable<LocalVariableObject<NwGameObject>>("_ENSO_ALLONGE_TARGET").Value, onCast.Caster.GetObjectVariable<LocalVariableLocation>("_ENSO_ALLONGE_TARGET").Value, castingClass);
        }
      }

      onCast.Caster.GetObjectVariable<LocalVariableLocation>("_ENSO_ALLONGE_TARGET").Delete();
      onCast.Caster.GetObjectVariable<LocalVariableObject<NwGameObject>>("_ENSO_ALLONGE_TARGET").Delete();
      ((NwGameObject)onCast.Caster).OnSpellCast -= CastAllongeSpell;
    }
  }
}
