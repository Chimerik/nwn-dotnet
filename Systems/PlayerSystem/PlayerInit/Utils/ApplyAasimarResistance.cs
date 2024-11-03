using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAasimarResistance()
      {
        if (oid.LoginCreature.Race.Id == CustomRace.Aasimar)
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AasimarResistance));
      }
    }
  }
}
