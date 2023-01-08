using Anvil.Services;
using Anvil.API.Events;

namespace NWN.Systems
{
  [ServiceBinding(typeof(FeatSystem))]
  public class FeatSystem
  {
    public static void OnUseFeatBefore(OnUseFeat onUseFeat)
    {
      if (!PlayerSystem.Players.TryGetValue(onUseFeat.Creature.ControllingPlayer.LoginCreature, out PlayerSystem.Player player))
        return;

      switch (onUseFeat.Feat.FeatType)
      {
        case CustomFeats.CustomMenuUP:
        case CustomFeats.CustomMenuDOWN:
        case CustomFeats.CustomMenuSELECT:
        case CustomFeats.CustomMenuEXIT:

          onUseFeat.PreventFeatUse = true;
          player.EmitKeydown(new PlayerSystem.Player.MenuFeatEventArgs(onUseFeat.Feat.FeatType));
          break;
      }
    }
  }
}
