using System;
using System.Numerics;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  class Swing
  {
    private readonly NwPlaceable swing;
    private readonly NwCreature oPC;
    private readonly PlaceableSystem placeableSystem;
    private IDisposable scheduleMotion { get; set; }

    public Swing(NwPlaceable swing, NwCreature oPC, PlaceableSystem placeableSystem)
    {
      this.placeableSystem = placeableSystem;
      this.swing = swing;
      this.oPC = oPC;

      this.swing.OnUsed -= placeableSystem.OnUsedBalancoire;
      this.swing.OnLeftClick += OnClickSwingBalancoire;

      HandleSwingReset();
    }
    private async void HandleSwingReset()
    {
      await NwTask.Delay(TimeSpan.FromSeconds(1));
      await NwTask.WaitUntilValueChanged(() => oPC.Position);

      swing.GetObjectVariable<LocalVariableInt>("_IS_SWINGING").Delete();
      
      scheduleMotion?.Dispose();

      VisualTransformLerpSettings vtLerpSettings = new VisualTransformLerpSettings
      {
        LerpType = VisualTransformLerpType.SmootherStep,
        Duration = TimeSpan.FromSeconds(2),
        PauseWithGame = true,
        ReturnDestinationTransform = true
      };

      swing.VisualTransform.Lerp(vtLerpSettings, transform =>
      {
        transform.Translation = new Vector3(0, 0, 0);
        transform.Rotation = new Vector3(0, 0, 0);
      });

      oPC.VisualTransform.Lerp(vtLerpSettings, transform =>
      {
        transform.Translation = new Vector3(0, 0, 0);
        transform.Rotation = new Vector3(0, 0, 0);
      });

      swing.OnUsed += placeableSystem.OnUsedBalancoire;
      swing.OnLeftClick -= OnClickSwingBalancoire;
    }

    private async void OnClickSwingBalancoire(PlaceableEvents.OnLeftClick onClick)
    {
      if (swing.GetObjectVariable<LocalVariableInt>("_IS_SWINGING").HasValue)
      {
        swing.GetObjectVariable<LocalVariableInt>("_IS_SWINGING").Delete();
        scheduleMotion.Dispose();

        await NwTask.Delay(TimeSpan.FromSeconds(2));

        VisualTransformLerpSettings vtLerpSettings = new VisualTransformLerpSettings
        {
          LerpType = VisualTransformLerpType.SmootherStep,
          Duration = TimeSpan.FromSeconds(1),
          PauseWithGame = true,
          ReturnDestinationTransform = true
        };

        swing.VisualTransform.Lerp(vtLerpSettings, transform =>
        {
          transform.Translation = new Vector3(0, 0, 0);
          transform.Rotation = new Vector3(0, 0, 0);
        });

        oPC.VisualTransform.Lerp(vtLerpSettings, transform =>
        {
          transform.Translation = new Vector3(0, 0, 0);
          transform.Rotation = new Vector3(0, 0, 0);
        });
      }
      else
      {
        swing.GetObjectVariable<LocalVariableInt>("_IS_SWINGING").Value = 1;

        VisualTransformLerpSettings vtLerpSettings = new VisualTransformLerpSettings
        {
          LerpType = VisualTransformLerpType.SmootherStep,
          Duration = TimeSpan.FromSeconds(2),
          PauseWithGame = true,
          ReturnDestinationTransform = true
        };

        oPC.VisualTransform.Lerp(vtLerpSettings, transform =>
        {
          transform.Translation = new Vector3(-0.75f, 0, 0);
          transform.Rotation = new Vector3(0, 0, 15);
        });

        swing.VisualTransform.Lerp(vtLerpSettings, transform =>
        {
          transform.Translation = new Vector3(0.75f, 0, 0);
          transform.Rotation = new Vector3(0, 0, -15);
        });

        scheduleMotion = placeableSystem.scheduler.ScheduleRepeating(HandleSwing, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(0));
      }
    }
    private void HandleSwing()
    {
      VisualTransformLerpSettings vtLerpSettings = new VisualTransformLerpSettings
      {
        LerpType = VisualTransformLerpType.SmootherStep,
        Duration = TimeSpan.FromSeconds(2),
        PauseWithGame = true,
        ReturnDestinationTransform = true
      };

      oPC.VisualTransform.Lerp(vtLerpSettings, transform =>
      {
        transform.Translation = new Vector3(-oPC.VisualTransform.Translation.X, 0, 0);
        transform.Rotation = new Vector3(0, 0, -oPC.VisualTransform.Rotation.Z);
      });

      swing.VisualTransform.Lerp(vtLerpSettings, transform =>
      {
        transform.Translation = new Vector3(-swing.VisualTransform.Translation.X, 0, 0);
        transform.Rotation = new Vector3(0, 0, -swing.VisualTransform.Rotation.Z);
      });
    }
  }
}
