using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public static Location CreateIntroScene(NwPlayer player, AreaSystem areaSystem)
    {
      try
      {
        player.OnClientDisconnect += OnLeaveDestroyIntroScene;

        NwArea arrivalAreaIn = NwObject.FindObjectsWithTag<NwArea>("entry_scene_in").First().Clone();
        arrivalAreaIn.Tag = $"entry_scene_in_{player.CDKey}";
        arrivalAreaIn.Name = $"La galère de {player.LoginCreature.OriginalName} (Bienvenue !)";
        arrivalAreaIn.OnEnter -= areaSystem.OnAreaEnter;
        arrivalAreaIn.OnEnter += areaSystem.OnAreaEnter;
        arrivalAreaIn.RestingAllowed = false;
        arrivalAreaIn.FindObjectsOfTypeInArea<NwPlaceable>().FirstOrDefault(o => o.Tag == "intro_mirror").OnLeftClick += PlaceableSystem.StartIntroMirrorDialog;

        NwArea arrivalAreaOut = NwObject.FindObjectsWithTag<NwArea>("entry_scene_out").First().Clone();
        arrivalAreaOut.Tag = $"entry_scene_out_{player.CDKey}";
        arrivalAreaOut.Name = $"La galère de {player.LoginCreature.OriginalName} (Bienvenue !)";
        arrivalAreaOut.OnEnter -= areaSystem.OnAreaEnter;
        arrivalAreaOut.OnEnter += areaSystem.OnAreaEnter;
        arrivalAreaOut.RestingAllowed = false;
        arrivalAreaOut.FindObjectsOfTypeInArea<NwPlaceable>().FirstOrDefault(o => o.Tag == "intro_mirror").OnLeftClick += PlaceableSystem.StartIntroMirrorDialog;

        InitializeIntroAreaObjects(arrivalAreaOut);

        Location arrivalLocation = arrivalAreaIn.FindObjectsOfTypeInArea<NwWaypoint>().FirstOrDefault(o => o.Tag == "ENTRY_POINT").Location;

        Task waitDefaultMapLoaded = NwTask.Run(async () =>
        {
          await NwTask.WaitUntilValueChanged(() => player.LoginCreature.Location.Area);
          player.LoginCreature.Location = arrivalLocation;
        });

        return arrivalLocation;
      }
      catch (Exception e)
      {
        ModuleSystem.Log.Info($"{e.Message}\n{e.StackTrace}");
        return NwModule.Instance.StartingLocation;
      }
    }
    private static void InitializeIntroAreaObjects(NwArea arrivalArea)
    {
      //AreaSystem.ScheduleRockSpawn(arrivalArea, 0); // TODO : en soit, est-ce que je ferais pas mieux de tout bêtement le mettre dans le heartbeat de la zone ?
      //AreaSystem.ScheduleRockSpawn(arrivalArea, 1);

      arrivalArea.SetAreaWind(new Vector3(1, 0, 0), 4, 0, 0);

      foreach (NwPlaceable recif in arrivalArea.FindObjectsOfTypeInArea<NwPlaceable>().Where(o => o.Tag == "intro_recif"))
        recif.VisibilityOverride = VisibilityMode.Hidden;

      NwPlaceable tourbillon = arrivalArea.FindObjectsOfTypeInArea<NwPlaceable>().FirstOrDefault(c => c.Tag == "intro_tourbillon");
      tourbillon.VisibilityOverride = VisibilityMode.Hidden;
      tourbillon.VisualTransform.Translation = new Vector3(tourbillon.VisualTransform.Translation.X, 115, tourbillon.VisualTransform.Translation.Z);  
    }
    public static void OnLeaveDestroyIntroScene(OnClientDisconnect onPCDisconnect)
    {
      if (onPCDisconnect.Player is null || onPCDisconnect.Player.LoginCreature is null)
        return;

      AreaSystem.AreaDestroyer(NwObject.FindObjectsWithTag<NwArea>($"entry_scene_in_{onPCDisconnect.Player.CDKey}").FirstOrDefault());
      AreaSystem.AreaDestroyer(NwObject.FindObjectsWithTag<NwArea>($"entry_scene_out_{onPCDisconnect.Player.CDKey}").FirstOrDefault());
    }
  }
}
