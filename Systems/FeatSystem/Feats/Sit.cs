using System;
using NWN.API;
using NWN.API.Constants;

namespace NWN.Systems
{
  class Sit
  {
    public Sit(PlayerSystem.Player player)
    {
      player.menu.Close();
      player.menu.isOpen = true;
      player.oid.PlayAnimation(Animation.LoopingSitChair, 1, true, TimeSpan.FromSeconds(99999999));
      player.LoadMenuQuickbar(QuickbarType.Sit);
    }
  }
}
