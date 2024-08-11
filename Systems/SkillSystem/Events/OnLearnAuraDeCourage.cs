using System.Linq;
using Anvil.API;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnAuraDeCourage(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.AuraDeCourage))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.AuraDeCourage);

      if (!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AuraDeCourageEffectTag && e.Creator == player.oid.LoginCreature))
      { 
        NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AuraDeCourage(player.oid.LoginCreature, 10)));
        UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>().SetRadius(3);
      }

      return true;
    }
  }
}
