using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlaceableSystem
  {
    public static void OnTimeChangeHandleLights(OnCalendarTimeChange onTimechange)
    {
      //ModuleSystem.Log.Info($"{onTimechange.TimeChangeType} : {onTimechange.NewValue}");

      if (onTimechange.TimeChangeType == TimeChangeType.TimeOfDay)
      {
        List<NwArea> recomputeArea = new();

        //ModuleSystem.Log.Info($"{NwModule.Instance.IsDawn}");

        if (NwModule.Instance.IsDawn)
        {
          foreach (var obj in NwObject.FindObjectsWithTag("day_night_light"))
          {
            if (obj is NwPlaceable plc)
            {
              _ = plc.PlayAnimation(Animation.PlaceableDeactivate, 1);
              plc.Illumination = false;

              if(!recomputeArea.Contains(plc.Area))
                recomputeArea.Add(plc.Area);
            }
          }
        }
        else if(NwModule.Instance.IsDusk)
        {
          foreach (var obj in NwObject.FindObjectsWithTag("day_night_light"))
          {
            if (obj is NwPlaceable plc)
            {
              _ = plc.PlayAnimation(Animation.PlaceableActivate, 1);
              plc.Illumination = true;

              if (!recomputeArea.Contains(plc.Area))
                recomputeArea.Add(plc.Area);
            }
          }
        }

        foreach (NwArea area in recomputeArea)
          area.RecomputeStaticLighting();
      }
    }
  }
}
