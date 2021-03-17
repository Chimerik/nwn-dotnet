using System;
using NWN.API;
using NWN.API.Constants;
using static NWN.Systems.Craft.Collect.Config;

namespace NWN.Systems
{
  class Renforcement
  {
    public Renforcement(NwPlayer oPC, NwGameObject oTarget)
    {
      if (!PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player))
        return;

      if (!(oTarget is NwItem))
      {
        oPC.SendServerMessage($"{oTarget.Name.ColorString(Color.WHITE)} n'est pas un objet et ne peut donc pas être renforcé.", Color.RED);
        return;
      }

      player.craftJob.Start(Craft.Job.JobType.Renforcement, null, player, null, (NwItem)oTarget);
    }
  }
}
