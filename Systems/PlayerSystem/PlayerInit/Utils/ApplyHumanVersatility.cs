using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyHumanVersatility()
      {
        oid.LoginCreature.OnAcquireItem -= ItemSystem.OnAcquireCheckHumanVersatility;
        oid.LoginCreature.OnUnacquireItem -= ItemSystem.OnUnAcquireCheckHumanVersatility;

        if (oid.LoginCreature.Race.RacialType == RacialType.Human 
          || oid.LoginCreature.KnowsFeat((Feat)CustomSkill.TotemAspectOurs))
        {
          oid.LoginCreature.OnAcquireItem += ItemSystem.OnAcquireCheckHumanVersatility;
          oid.LoginCreature.OnUnacquireItem += ItemSystem.OnUnAcquireCheckHumanVersatility;
        }
      }
    }
  }
}
