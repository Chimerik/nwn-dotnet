using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnOathFeat(PlayerSystem.Player player, int customSkillId)
    {
      NwCreature creature = player.oid.LoginCreature;

      if (!creature.KnowsFeat((Feat)customSkillId))
        creature.AddFeat((Feat)customSkillId);

      if(customSkillId == CustomSkill.SensDivin)
        creature.SetFeatRemainingUses((Feat)customSkillId, 2);
      else
        creature.SetFeatRemainingUses((Feat)customSkillId, creature.GetFeatRemainingUses((Feat)CustomSkill.SensDivin));

      return true;
    }
  }
}
