using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  class ResetPosition
  {
    public ResetPosition(NwPlayer oPC)
    {
      Utils.ResetVisualTransform(oPC.ControlledCreature);
    }
  }
}
