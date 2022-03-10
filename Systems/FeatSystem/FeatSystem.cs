using Anvil.Services;
using Anvil.API.Events;

namespace NWN.Systems
{
  [ServiceBinding(typeof(FeatSystem))]
  public class FeatSystem
  {   
    public static void OnUseFeatBefore(OnUseFeat onUseFeat)
    {
      if (!PlayerSystem.Players.TryGetValue(onUseFeat.Creature, out PlayerSystem.Player player))
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
          player.EmitKeydown(new PlayerSystem.Player.MenuFeatEventArgs(onUseFeat.Feat.FeatType));
          break;
      }
    }
  }
}
