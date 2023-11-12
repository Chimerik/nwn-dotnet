using System;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public static Location CreateIntroScene(NwPlayer player, AreaSystem areaSystem)
    {
      if (!NwModule.Instance.Areas.Any(a => a.Tag == "entry_scene"))
        return NwModule.Instance.StartingLocation;

      NwArea arrivalArea = NwArea.Create("intro_galere", $"entry_scene_{player.CDKey}", $"La galère de {player.LoginCreature.OriginalName} (Bienvenue !)");
      arrivalArea.OnExit += areaSystem.OnIntroAreaExit;

      arrivalArea.FindObjectsOfTypeInArea<NwCreature>().FirstOrDefault(c => c.Tag == "CapitaineMarco").OnConversation += OnConversationIntroCaptain;

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

      Location arrivalLocation = arrivalArea.FindObjectsOfTypeInArea<NwWaypoint>().FirstOrDefault(o => o.Tag == "ENTRY_POINT").Location;

      Task waitDefaultMapLoaded = NwTask.Run(async () =>
      {
        await NwTask.WaitUntilValueChanged(() => player.LoginCreature.Location.Area);
        player.LoginCreature.Location = arrivalLocation;
      });

      return arrivalLocation;
    }
    private static void OnConversationIntroCaptain(CreatureEvents.OnConversation onConv)
    {
      NwCreature pc = (NwCreature)onConv.LastSpeaker;
      bool go = true;

      if (pc.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_RACE").HasValue)
      {
        go = false;
        pc.LoginPlayer.SendServerMessage($"Avant de poursuivre votre voyage, veuillez choisir une {"race".ColorString(ColorConstants.White)} auprès du miroir !", ColorConstants.Orange);
      }

      if (pc.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_ORIGIN").HasValue)
      {
        go = false;
        pc.LoginPlayer.SendServerMessage($"Avant de poursuivre votre voyage, veuillez choisir une {"origine".ColorString(ColorConstants.White)} auprès du miroir !", ColorConstants.Orange);
      }

      if (pc.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_CLASS").HasValue)
      {
        go = false;
        pc.LoginPlayer.SendServerMessage($"Avant de poursuivre votre voyage, veuillez choisir une {"classe".ColorString(ColorConstants.White)} auprès du miroir !", ColorConstants.Orange);
      }

      if (pc.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_STATS").HasValue)
      {
        go = false;
        pc.LoginPlayer.SendServerMessage($"Avant de poursuivre votre voyage, veuillez finaliser vos {"stats".ColorString(ColorConstants.White)} auprès du miroir !", ColorConstants.Orange);
      }

      if (go)
        pc.Area.GetObjectVariable<LocalVariableInt>("_GO").Value = 1;

      //onConv.Creature.OnConversation -= OnConversationIntroCaptain;
      pc.LoginPlayer.ActionStartConversation(pc, onConv.Creature.DialogResRef);
    }
  }
}
