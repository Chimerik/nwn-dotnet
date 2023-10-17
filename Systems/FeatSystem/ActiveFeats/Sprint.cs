using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    [ScriptHandler("on_sprint_use")]
    private void OnSprintUse(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is not NwCreature creature)
        return;

      Effect sprint = Effect.LinkEffects(Effect.MovementSpeedIncrease(99), Effect.Icon(NwGameTables.EffectIconTable.GetRow(142)));
      creature.ApplyEffect(EffectDuration.Temporary, sprint, NwTimeSpan.FromRounds(1));

      StringUtils.DisplayStringToAllPlayersNearTarget(creature, "Sprint", StringUtils.gold, true);
    }
  }
}
