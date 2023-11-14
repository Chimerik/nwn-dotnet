using Anvil.API;
using Anvil.Services;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class PlaceableSystem
  {
    [ScriptHandler("on_set_trap")]
    public static void OnSetTrap(CallInfo callInfo)
    {
      var trap = NWScript.StringToObject(EventsPlugin.GetEventData("TRAP_OBJECT_ID")).ToNwObject<NwGameObject>();

      if (!trap.IsValid)
        return;

      if (trap is NwPlaceable plc)
        plc.OnTrapTriggered += OnTrapTriggered;
      else if(trap is NwDoor door)
        door.OnTrapTriggered += OnTrapTriggered;
      else if(trap is NwTrigger trigger)
        trigger.OnTrapTriggered += OnTrapTriggered;
    }
  }
}
