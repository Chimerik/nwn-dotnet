using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    [ScriptHandler("on_disengage")]
    private void OnDisengage(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is not NwCreature creature)
        return;

      creature.ApplyEffect(EffectDuration.Temporary, EffectSystem.disengageEffect, NwTimeSpan.FromRounds(1));
      StringUtils.DisplayStringToAllPlayersNearTarget(creature, "Désengagement", StringUtils.gold, true);
    }
  }
}
