using System;
using Anvil.API;

namespace NWN.Systems
{
  class Sit
  {
    public Sit(PlayerSystem.Player player)
    {
      player.menu.Close();
      player.menu.isOpen = true;
      player.oid.LoginCreature.PlayAnimation(Animation.LoopingSitChair, 1, true, TimeSpan.FromSeconds(99999999));
      player.LoadMenuQuickbar(QuickbarType.Sit);
    }
  }
}
