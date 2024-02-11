using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnTotemAspectLoup(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(customSkillId)))
        player.oid.LoginCreature.AddFeat(NwFeat.FromFeatId(customSkillId));

      if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.StealthProficiency)))
        player.oid.LoginCreature.AddFeat(NwFeat.FromFeatId(CustomSkill.StealthProficiency));

      if (!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.WolAspectAuraEffectTag))
        player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.wolfAspectAura);

      return true;
    }
  }
}
