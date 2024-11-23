using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnEnnemiJure(PlayerSystem.Player player, int customSkillId)
    {
      NwCreature creature = player.oid.LoginCreature;

      if (!creature.KnowsFeat((Feat)customSkillId))
        creature.AddFeat((Feat)customSkillId);

      creature.SetFeatRemainingUses((Feat)customSkillId, 2);

      player.LearnAlwaysPreparedSpell(CustomSpell.MarqueDuChasseur, CustomClass.Ranger);

      return true;
    }
  }
}
