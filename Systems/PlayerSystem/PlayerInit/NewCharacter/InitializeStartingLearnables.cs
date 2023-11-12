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

        if (!oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.Sprint)))
          oid.LoginCreature.AddFeat(NwFeat.FromFeatId(CustomSkill.Sprint));

        if (!oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.Disengage)))
          oid.LoginCreature.AddFeat(NwFeat.FromFeatId(CustomSkill.Disengage));
      }
    }
  }
}
