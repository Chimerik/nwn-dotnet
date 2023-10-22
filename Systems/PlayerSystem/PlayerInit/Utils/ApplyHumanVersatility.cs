using System.Linq;
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

        if (learnableSkills.TryGetValue(CustomSkill.HumanVersatility, out LearnableSkill versatility) && versatility.currentLevel > 0)
        {
          oid.LoginCreature.OnAcquireItem += ItemSystem.OnAcquireCheckHumanVersatility;
          oid.LoginCreature.OnUnacquireItem += ItemSystem.OnUnAcquireCheckHumanVersatility;
        }
      }
    }
  }
}
