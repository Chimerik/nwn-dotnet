using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnAvatarDeBataille(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ClercGuerreAvatarDeBataille))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ClercGuerreAvatarDeBataille);

      if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AvatarDeBatailleEffectTag))
        NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AvatarDeBataille));

      return true;
    }
  }
}
