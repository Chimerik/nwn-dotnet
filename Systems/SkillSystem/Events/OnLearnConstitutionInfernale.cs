using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnConstitutionInfernale(PlayerSystem.Player player, int customSkillId)
    {
      if(player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ConstitutionInfernale))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ConstitutionInfernale);

      byte rawConstitution = player.oid.LoginCreature.GetRawAbilityScore(Ability.Constitution);
      if (rawConstitution < 20)
        player.oid.LoginCreature.SetsRawAbilityScore(Ability.Constitution, (byte)(rawConstitution + 1));

      NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ConstitutionInfernale));

      return true;
    }
  }
}
