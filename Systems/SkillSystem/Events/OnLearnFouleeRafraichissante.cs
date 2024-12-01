using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFouleeRafraichissante(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.FouleeRafraichissante))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.FouleeRafraichissante);

      byte uses = (byte)(CreatureUtils.GetAbilityModifierMin1(player.oid.LoginCreature, Ability.Charisma)
        + player.oid.LoginCreature.GetClassInfo((ClassType)CustomClass.Occultiste).GetRemainingSpellSlots(2));

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.FouleeRafraichissante, uses);

      return true;
    }
  }
}
