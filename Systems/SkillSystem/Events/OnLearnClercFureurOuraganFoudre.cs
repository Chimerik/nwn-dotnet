using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnClercFureurOuraganFoudre(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ClercFureurOuraganFoudre))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ClercFureurOuraganFoudre);

      int chaMod = player.oid.LoginCreature.GetAbilityModifier(Ability.Wisdom);
      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ClercFureurOuraganFoudre, (byte)(chaMod > 0 ? chaMod : 1));

      return true;
    }
  }
}
