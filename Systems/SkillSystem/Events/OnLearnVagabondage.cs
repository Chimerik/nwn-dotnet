using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnVagabondage(PlayerSystem.Player player, int customSkillId)
    {
      if(!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.RangerVagabondage))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.RangerVagabondage);

      EffectSystem.ApplyVagabondage(player.oid.LoginCreature);

      return true;
    }
  }
}
