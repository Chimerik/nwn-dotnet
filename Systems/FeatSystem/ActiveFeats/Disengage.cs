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

      Effect disengage = Effect.Icon(NwGameTables.EffectIconTable.GetRow(143));
      disengage.Tag = "_EFFECT_DISENGAGE";
      creature.ApplyEffect(EffectDuration.Temporary, disengage, NwTimeSpan.FromRounds(1));
      StringUtils.DisplayStringToAllPlayersNearTarget(creature, "Désengagement", StringUtils.gold, true);
    }
  }
}
