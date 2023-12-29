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
        NwArea arrivalArea = NwObject.FindObjectsWithTag<NwArea>("entry_scene").First().Clone();
        arrivalArea.Tag = $"entry_scene_{player.CDKey}";
        arrivalArea.Name = $"La galère de {player.LoginCreature.OriginalName} (Bienvenue !)";
        //NwArea arrivalArea = NwArea.Create("intro_galere", $"entry_scene_{player.CDKey}", $"La galère de {player.LoginCreature.OriginalName} (Bienvenue !)");
        arrivalArea.OnExit -= areaSystem.OnIntroAreaExit;
        arrivalArea.OnExit += areaSystem.OnIntroAreaExit;
        arrivalArea.OnEnter -= areaSystem.OnAreaEnter;
        arrivalArea.OnEnter += areaSystem.OnAreaEnter;
        arrivalArea.RestingAllowed = true;

        InitializeIntroAreaObjects(arrivalArea);

        Location arrivalLocation = arrivalArea.FindObjectsOfTypeInArea<NwWaypoint>().FirstOrDefault(o => o.Tag == "ENTRY_POINT").Location;

        Task waitDefaultMapLoaded = NwTask.Run(async () =>
        {
          await NwTask.WaitUntilValueChanged(() => player.LoginCreature.Location.Area);
          player.LoginCreature.Location = arrivalLocation;
        });

        return arrivalLocation;
      }
      catch(Exception e)
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

      NwPlaceable introMirror = arrivalArea.FindObjectsOfTypeInArea<NwPlaceable>().FirstOrDefault(o => o.Tag == "intro_mirror");
      introMirror.OnLeftClick += PlaceableSystem.StartIntroMirrorDialog;
    }
  }
}
