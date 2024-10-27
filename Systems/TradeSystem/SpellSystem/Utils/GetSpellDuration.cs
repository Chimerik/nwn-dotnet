using System;
using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static TimeSpan GetSpellDuration(NwGameObject oCaster, SpellEntry spellEntry)
    {
      if (spellEntry.duration > 9 && oCaster.ActiveEffects.Any(e => e.Tag == EffectSystem.MetamagieEffectTag && e.IntParams[5] == CustomSkill.EnsoExtension))
      {
        EffectUtils.RemoveTaggedParamEffect(oCaster, CustomSkill.EnsoExtension, EffectSystem.MetamagieEffectTag);

        if (oCaster is NwCreature caster)
          StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Extension", StringUtils.gold, true, true);

        if (spellEntry.requiresConcentration)
          oCaster.ApplyEffect(EffectDuration.Permanent, EffectSystem.ConcentrationAdvantage);

        return NwTimeSpan.FromRounds(spellEntry.duration * 2);
      }
      return NwTimeSpan.FromRounds(spellEntry.duration);
    }
  }
}
