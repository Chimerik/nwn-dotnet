using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnTotemAspectElan(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(customSkillId)))
        player.oid.LoginCreature.AddFeat(NwFeat.FromFeatId(customSkillId));

      if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.ElkAspectAuraEffectTag))
        player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.elkAspectAura);

      return true;
    }
  }
}
