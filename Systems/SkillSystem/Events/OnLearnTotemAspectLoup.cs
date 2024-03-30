using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnTotemAspectLoup(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.StealthProficiency))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.StealthProficiency);

      if (!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.WolAspectAuraEffectTag))
        player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.wolfAspectAura);

      return true;
    }
  }
}
