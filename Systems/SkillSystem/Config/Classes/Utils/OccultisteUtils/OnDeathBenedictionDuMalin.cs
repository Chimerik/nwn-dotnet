using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class OccultisteUtils
  {
    public static void OnDeathBenedictionDuMalin(CreatureEvents.OnDeath onDeath)
    {
      foreach(var eff in onDeath.KilledCreature.ActiveEffects.Where(e => e.Tag == EffectSystem.BenedictionDuMalinEffectTag))
      {
        if (eff.Creator is NwCreature caster)
        {
          int chaMod = caster.GetAbilityModifier(Ability.Charisma) > 1 ? caster.GetAbilityModifier(Ability.Charisma) : 1;
          chaMod += caster.GetClassInfo((ClassType)CustomClass.Occultiste).Level;

          caster.ApplyEffect(EffectDuration.Permanent, Effect.TemporaryHitpoints(chaMod));
          caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.DurMindAffectingPositive));

          EffectUtils.RemoveTaggedEffect(onDeath.KilledCreature, caster, EffectSystem.BenedictionDuMalinEffectTag);
        }
      }     
    }
  }
}
