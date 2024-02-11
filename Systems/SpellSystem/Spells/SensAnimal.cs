using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void SensAnimal(NwCreature caster, SpellEvents.OnSpellCast onSpellCast, SpellEntry spellEntry)
    {
      if (onSpellCast.TargetObject is not NwCreature target)
        return;

      if(caster.ActiveEffects.Any(e => e.Tag == EffectSystem.SensAnimalEffectTag))
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} ne se concentre plus sur Sens Animal", ColorConstants.Orange, true);   
      else if(target == caster || target.Race.RacialType != RacialType.Animal)
        caster.LoginPlayer?.SendServerMessage("Cible invalide", ColorConstants.Red);
      else
      {
        SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, caster, onSpellCast.Spell.SpellType);

        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(spellEntry.damageVFX));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(spellEntry.damageVFX));
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.sensAnimalEffect, NwTimeSpan.FromRounds(spellEntry.duration));
        EffectSystem.ApplyConcentrationEffect(caster, onSpellCast.Spell.Id, new List<NwGameObject> { caster }, spellEntry.duration);
      }
    }
  }
}
