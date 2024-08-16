using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFureurTempete(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.EnsoFureurTempete))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.EnsoFureurTempete);

      if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.FureurTempeteEffectTag))
        NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.FureurTempete(11)));

      player.oid.LoginCreature.OnDamaged -= EnsoUtils.OnDamagedFureurTempete;
      player.oid.LoginCreature.OnDamaged += EnsoUtils.OnDamagedFureurTempete;

      return true;
    }
  }
}
