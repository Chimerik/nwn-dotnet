using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlaceableSystem
  {
    public static void OnTrapTriggered(DoorEvents.OnTrapTriggered onTrap)
    {
      if(onTrap.TriggeredBy is NwCreature target)
        HandleTrapTriggered(new List<NwCreature> { target }, onTrap.Door.TrapBaseType, onTrap.Door);
    }
    public static void OnTrapTriggered(PlaceableEvents.OnTrapTriggered onTrap)
    {
      if (onTrap.TriggeredBy is NwCreature target)
        HandleTrapTriggered(new List<NwCreature> { target }, onTrap.Placeable.TrapBaseType, onTrap.Placeable);
    }
    public static void OnTrapTriggered(TriggerEvents.OnTrapTriggered onTrap)
    {
      HandleTrapTriggered(onTrap.Trigger.GetObjectsInTrigger<NwCreature>().ToList(), onTrap.Trigger.TrapBaseType, onTrap.Trigger);
    }
  }
}
