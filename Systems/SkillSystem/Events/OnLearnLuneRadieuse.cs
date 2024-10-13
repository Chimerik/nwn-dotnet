using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnLuneRadieuse(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.DruideLuneRadieuse))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.DruideLuneRadieuse);

      if (!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.PolymorphEffectTag))
        player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.DruideLuneRadieuse, 0);

      return true;
    }
  }
}
