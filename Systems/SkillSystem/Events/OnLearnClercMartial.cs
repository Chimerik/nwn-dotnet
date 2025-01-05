using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    /*public static bool OnLearnClercMartial(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ClercMartial))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ClercMartial);

      int chaMod = player.oid.LoginCreature.GetAbilityModifier(Ability.Wisdom);
      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ClercMartial, (byte)(chaMod > 0 ? chaMod : 1));

      return true;
    }*/
  }
}
