using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFormeSauvage(PlayerSystem.Player player, int customSkillId)
    {
      NwCreature creature = player.oid.LoginCreature;

      if (!creature.KnowsFeat((Feat)customSkillId))
        creature.AddFeat((Feat)customSkillId);

      if(customSkillId == CustomSkill.FormeSauvageBlaireau)
        creature.SetFeatRemainingUses((Feat)customSkillId, 2);
      else
        creature.SetFeatRemainingUses((Feat)customSkillId, creature.GetFeatRemainingUses((Feat)CustomSkill.FormeSauvageBlaireau));

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.FormeSauvageBlaireau) < 2)
      {
        creature.SetFeatRemainingUses((Feat)CustomSkill.FormeSauvageAir, 0);
        creature.SetFeatRemainingUses((Feat)CustomSkill.FormeSauvageEau, 0);
        creature.SetFeatRemainingUses((Feat)CustomSkill.FormeSauvageFeu, 0);
        creature.SetFeatRemainingUses((Feat)CustomSkill.FormeSauvageTerre, 0);
      }

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkStunStrike) < 3)
      {
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkFrappeDombre, 0);
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPaumeVibratoire, 0);
      }

      return true;
    }
  }
}
