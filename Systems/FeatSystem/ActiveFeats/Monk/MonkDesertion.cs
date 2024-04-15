using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MonkDesertion(NwCreature caster)
    {
      if (caster.GetFeatRemainingUses((Feat)CustomSkill.MonkDesertion) < 4)
      {
        caster.LoginPlayer?.SendServerMessage("Nécessite 4 charges de Ki", ColorConstants.Red);
        return;
      }

      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.Desertion, NwTimeSpan.FromRounds(10));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Désertion de l'âme", StringUtils.gold, true);

      FeatUtils.DecrementKi(caster, 4);
    }
  }
}
