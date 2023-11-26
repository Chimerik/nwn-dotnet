using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnHBCheckMaitreArmureIntermediaire(CreatureEvents.OnHeartbeat onHB)
    {
      if(onHB.Creature.GetAbilityModifier(Ability.Dexterity) > 2)
      {
        if(!onHB.Creature.ActiveEffects.Any(e => e.Tag == EffectSystem.MaitreArmureIntermediaireEffectTag))
          onHB.Creature.ApplyEffect(EffectDuration.Permanent, EffectSystem.maitreArmureIntermediaire);
      }
      else 
        foreach(var eff in onHB.Creature.ActiveEffects)
          if(eff.Tag == EffectSystem.MaitreArmureIntermediaireEffectTag)
            onHB.Creature.RemoveEffect(eff);
    }
  }
}
