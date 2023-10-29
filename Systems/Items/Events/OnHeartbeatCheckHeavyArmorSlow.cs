using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    private static void OnHeartbeatCheckHeavyArmorSlow(CreatureEvents.OnHeartbeat onHB)
    {
      if(onHB.Creature.GetAbilityScore(Ability.Strength) > 14)
      {
        foreach(var eff in onHB.Creature.ActiveEffects)
          if(eff.Tag == EffectSystem.heavyArmorSlowEffectTag)
            onHB.Creature.RemoveEffect(eff);
      }
      else if(!onHB.Creature.ActiveEffects.Any(e => e.Tag == EffectSystem.heavyArmorSlow.Tag))
        onHB.Creature.ApplyEffect(EffectDuration.Permanent, EffectSystem.heavyArmorSlow);
    }
  }
}
