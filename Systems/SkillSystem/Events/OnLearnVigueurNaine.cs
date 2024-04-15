using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnVigueurNaine(PlayerSystem.Player player, int customSkillId)
    {
      if(!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.VigueurNaine))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.VigueurNaine);

      byte rawConstitution = player.oid.LoginCreature.GetRawAbilityScore(Ability.Constitution);
      if (rawConstitution < 20)
        player.oid.LoginCreature.SetsRawAbilityScore(Ability.Constitution, (byte)(rawConstitution + 1));

      player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>(CreatureUtils.VigueurNaineHDVariable).Value = player.oid.LoginCreature.Level;

      return true;
    }
  }
}
