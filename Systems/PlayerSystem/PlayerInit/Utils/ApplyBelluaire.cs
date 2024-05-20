using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyBelluaire()
      {
        if (learnableSkills.ContainsKey(CustomSkill.RangerBelluaire))
        {
          if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.BelluaireFurieBestiale))
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.BelluaireFurieBestiale, 0);

          if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.BelluaireSprint))
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.BelluaireSprint, 0);

          if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.BelluaireDisengage))
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.BelluaireDisengage, 0);
        }
      }
    }
  }
}
