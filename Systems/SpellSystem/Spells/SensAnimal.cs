using System.Collections.Generic;
using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> SensAnimal(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      List<NwGameObject> concentrationTargets = new();

      if (oCaster is NwCreature caster && oTarget is NwCreature target)
      {
        if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.SensAnimalEffectTag))
        {
          StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} ne se concentre plus sur Sens Animal", ColorConstants.Orange, true);
          SpellUtils.DispelConcentrationEffects(caster);
        }
        else if (target == caster || target.Race.RacialType != RacialType.Animal)
          caster.LoginPlayer?.SendServerMessage("Cible invalide", ColorConstants.Red);
        else
        {
          SpellUtils.SignalEventSpellCast(target, caster, spell.SpellType);

          caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(spellEntry.damageVFX));
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(spellEntry.damageVFX));
          caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.sensAnimalEffect, NwTimeSpan.FromRounds(spellEntry.duration));
          concentrationTargets.Add(caster);
        }
      }

      return concentrationTargets;
    }
  }
}
