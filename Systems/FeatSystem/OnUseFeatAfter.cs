using Anvil.Services;
using Anvil.API.Events;

namespace NWN.Systems
{
  [ServiceBinding(typeof(FeatSystem))]
  public partial class FeatSystem
  {
    public static void OnUseFeatAfter(OnUseFeat onUseFeat)
    {
      switch (onUseFeat.Feat.Id)
      {
        case CustomSkill.Stealth: Stealth(onUseFeat.Creature); return;
      }
    }
  }
}
