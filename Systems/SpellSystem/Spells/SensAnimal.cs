using System.Collections.Generic;
using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void SensAnimal(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      if (oCaster is not NwCreature caster || oTarget is not NwCreature target)
        return;

      if(caster.ActiveEffects.Any(e => e.Tag == EffectSystem.SensAnimalEffectTag))
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} ne se concentre plus sur Sens Animal", ColorConstants.Orange, true);   
      else if(target == caster || target.Race.RacialType != RacialType.Animal)
        caster.LoginPlayer?.SendServerMessage("Cible invalide", ColorConstants.Red);
      else
      {
        SpellUtils.SignalEventSpellCast(target, caster, spell.SpellType);

        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(spellEntry.damageVFX));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(spellEntry.damageVFX));
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.sensAnimalEffect, NwTimeSpan.FromRounds(spellEntry.duration));
        EffectSystem.ApplyConcentrationEffect(caster, spell.Id, new List<NwGameObject> { caster }, spellEntry.duration);
      }
    }
  }
}
