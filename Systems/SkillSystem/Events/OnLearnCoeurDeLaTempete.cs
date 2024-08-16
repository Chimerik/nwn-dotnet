using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnCoeurDeLaTempete(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.EnsoCoeurDeLaTempete))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.EnsoCoeurDeLaTempete);

      if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CoeurDeLaTempeteEffectTag))
        NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.CoeurDeLaTempete));

      return true;
    }
  }
}
