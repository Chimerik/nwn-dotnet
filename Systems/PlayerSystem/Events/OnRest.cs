using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public static async void OnRest(ModuleEvents.OnPlayerRest onRest)
    {
      await onRest.Player.ControlledCreature.ClearActionQueue();
    }
  }
}
