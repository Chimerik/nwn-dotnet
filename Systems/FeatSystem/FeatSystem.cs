using Anvil.Services;
using Anvil.API.Events;
using static NWN.Systems.PlayerSystem;
using System;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  [ServiceBinding(typeof(FeatSystem))]
  public class FeatSystem
  {   
    public static void OnUseFeatBefore(OnUseFeat onUseFeat)
    {
      if (!Players.TryGetValue(onUseFeat.Creature.ControllingPlayer.LoginCreature, out PlayerSystem.Player player))
        return;

      switch (onUseFeat.Feat.FeatType)
      {
        case CustomFeats.CustomMenuUP:
        case CustomFeats.CustomMenuDOWN:
        case CustomFeats.CustomMenuSELECT:
        case CustomFeats.CustomMenuEXIT:
        case CustomFeats.CustomPositionRight:
        case CustomFeats.CustomPositionLeft:
        case CustomFeats.CustomPositionForward:
        case CustomFeats.CustomPositionBackward:
        case CustomFeats.CustomPositionRotateRight:
        case CustomFeats.CustomPositionRotateLeft:

          onUseFeat.PreventFeatUse = true;
          player.EmitKeydown(new MenuFeatEventArgs(onUseFeat.Feat.FeatType));
          break;

        case CustomFeats.Sit:

          onUseFeat.PreventFeatUse = true;
          _ = onUseFeat.Creature.PlayAnimation(Anvil.API.Animation.LoopingSitChair, 1, false, TimeSpan.FromDays(1));

          if (!player.windows.TryAdd("sitAnywhere", new SitAnywhereWindow(player)))
            ((SitAnywhereWindow)player.windows["sitAnywhere"]).CreateWindow();

          break;
      }
    }
  }
}
