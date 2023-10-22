using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyWoodElfSpeed()
      {
        switch(oid.LoginCreature.Race.Id)
        {
          case CustomRace.WoodElf:
          case CustomRace.WoodHalfElf:

            if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.woodElfSpeed.Tag))
              oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.woodElfSpeed);

            return;

          default: return;
        }
      }
    }
  }
}
