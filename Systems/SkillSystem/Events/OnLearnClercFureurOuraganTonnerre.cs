using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnClercFureurOuraganTonnerre(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ClercFureurOuraganTonnerre))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ClercFureurOuraganTonnerre);

      int chaMod = player.oid.LoginCreature.GetAbilityModifier(Ability.Wisdom);
      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ClercFureurOuraganTonnerre, (byte)(chaMod > 0 ? chaMod : 1));

      return true;
    }
  }
}
