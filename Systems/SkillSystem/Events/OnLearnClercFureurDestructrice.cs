using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnClercFureurDestructrice(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ClercFureurDestructrice))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ClercFureurDestructrice);

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ClercFureurDestructrice, 1);

      return true;
    }
  }
}
