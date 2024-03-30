using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeStartingLearnables()
      {
        foreach(var feat in oid.LoginCreature.Feats)
          oid.LoginCreature.RemoveFeat(feat);

        if (!oid.LoginCreature.KnowsFeat((Feat)CustomSkill.Sprint))
          oid.LoginCreature.AddFeat((Feat)CustomSkill.Sprint);

        if (!oid.LoginCreature.KnowsFeat((Feat)CustomSkill.Disengage))
          oid.LoginCreature.AddFeat((Feat)CustomSkill.Disengage);
      }
    }
  }
}
