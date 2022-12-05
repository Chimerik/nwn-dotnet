using Anvil.API;
using NWN.Core;
using NWN.Core.NWNX;
using Anvil.Services;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AreaSystem))]
  partial class AreaSystem
  {
    [ScriptHandler("intro_start")]
    public void StartEntryScene(CallInfo callInfo)
    {
      NwCreature captain = (NwCreature)callInfo.ObjectSelf;
      NwArea area = captain.Area;

      area.GetObjectVariable<LocalVariableBool>("_STOP_INTRO_ROCK_SPAWN").Value = true;

      NwPlayer player = NWScript.GetLastSpeaker().ToNwObject<NwCreature>().ControllingPlayer;
      _ = player.SetCameraFacing(180, 65, 20);

      captain.DialogResRef = "";
      _ = captain.SpeakString("Des récifs ! Accrochez-vous, va falloir manoeuvrer serré !");

      List<NwCreature> sailorList = captain.GetNearestCreatures(CreatureTypeFilter.PlayerChar(false)).Where(c => c.Tag == "intro_sailor").ToList();
      _ = sailorList[0].ActionRandomWalk();
      _ = sailorList[1].ActionRandomWalk();
      _ = sailorList[0].SpeakString("Umberlie, épargne-nous !".ColorString(ColorConstants.Orange));
      _ = sailorList[1].SpeakString("Oh non, non, non, faut faire quelque chose, vite !");
      sailorList[0].MovementRate = MovementRate.DM;
      sailorList[1].MovementRate = MovementRate.DM;

      foreach (NwPlaceable fog in player.LoginCreature.GetNearestObjectsByType<NwPlaceable>().Where(t => t.Tag == "intro_brouillard"))
        fog.VisibilityOverride = VisibilityMode.Visible;

      area.Weather = WeatherType.Rain;
      AreaPlugin.SetDayNightCycle(area, AreaPlugin.NWNX_AREA_DAYNIGHTCYCLE_ALWAYS_DARK);
      AreaPlugin.SetWeatherChance(area, AreaPlugin.NWNX_AREA_WEATHER_CHANCE_LIGHTNING, 100);
      area.SetAreaWind(new Vector3(1, 0, 0), 8, 0, 0);

      List<NwPlaceable> rocks = player.LoginCreature.GetNearestObjectsByType<NwPlaceable>().Where(c => c.Tag == "intro_recif").ToList();
      NwPlaceable tourbillon = player.LoginCreature.GetNearestObjectsByType<NwPlaceable>().FirstOrDefault(c => c.Tag == "intro_tourbillon");
      tourbillon.ApplyEffect(EffectDuration.Permanent, Effect.VisualEffect((VfxType)915, false, 20));

      rocks[0].VisibilityOverride = VisibilityMode.AlwaysVisible;
      rocks[1].VisibilityOverride = VisibilityMode.AlwaysVisible;
      rocks[2].VisibilityOverride = VisibilityMode.AlwaysVisible;
      tourbillon.VisibilityOverride = VisibilityMode.AlwaysVisible;

      rocks[0].VisualTransform.Lerp(new VisualTransformLerpSettings { LerpType = VisualTransformLerpType.Linear, Duration = TimeSpan.FromSeconds(30), PauseWithGame = true }, transform => { transform.Translation = new Vector3(0, -72, 0); });
      rocks[1].VisualTransform.Lerp(new VisualTransformLerpSettings { LerpType = VisualTransformLerpType.Linear, Duration = TimeSpan.FromSeconds(24), PauseWithGame = true }, transform => { transform.Translation = new Vector3(0, -80, 0); });
      rocks[2].VisualTransform.Lerp(new VisualTransformLerpSettings { LerpType = VisualTransformLerpType.Linear, Duration = TimeSpan.FromSeconds(38), PauseWithGame = true }, transform => { transform.Translation = new Vector3(0, -64, 0); });
      tourbillon.VisualTransform.Lerp(new VisualTransformLerpSettings { LerpType = VisualTransformLerpType.Linear, Duration = TimeSpan.FromSeconds(40), PauseWithGame = true }, transform => { transform.Translation = new Vector3(0, -15, 0); });

      Task waitIntroEvents = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0.3));
        player.LockCameraDirection(true);
        player.LockCameraDistance(true);
        player.LockCameraPitch(true);

        await NwTask.Delay(TimeSpan.FromSeconds(1.7));
        TriggerRandomLightnings(area, player.LoginCreature.Position, 25, player.ControlledCreature);

        await NwTask.Delay(TimeSpan.FromSeconds(23));
        StrikeSailor(sailorList[1], sailorList[0]);

        await NwTask.Delay(TimeSpan.FromSeconds(10));
        await captain.SpeakString("Qu'est ce que c'est que ce truc ? On ne peut pas éviter la collision, ABANDONNEZ LE NAVIRE !".ColorString(ColorConstants.Red));

        //tourbillon.ApplyEffect(EffectDuration.Temporary, Effect.VisualEffect((VfxType)836, false, 3, new Vector3(0, 30, 3)), TimeSpan.FromSeconds(10));
        //tourbillon.ApplyEffect(EffectDuration.Temporary, Effect.VisualEffect((VfxType)836, false, 3, new Vector3(0, 15, 3)), TimeSpan.FromSeconds(10));
        //tourbillon.ApplyEffect(EffectDuration.Temporary, Effect.VisualEffect((VfxType)346, false, 3, new Vector3(-5, -20, 4)), TimeSpan.FromSeconds(10));
        //tourbillon.ApplyEffect(EffectDuration.Temporary, Effect.VisualEffect((VfxType)346, false, 3, new Vector3(5, -20, 4)), TimeSpan.FromSeconds(10));

        await NwTask.Delay(TimeSpan.FromSeconds(1));
        PlayTourbillonEffects(area, player);
      });
    }
    private static void PlayTourbillonEffects(NwArea area, NwPlayer oPC)
    {
      oPC.LoginCreature.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect((VfxType)135));
      //oPC.LoginCreature.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill, false, 2, new Vector3(0, 0, 0), new Vector3(270, 90, 0)));
      //oPC.LoginCreature.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfFirestorm, false, 2, new Vector3(0, 0, 0), new Vector3(0, 0, 0)));
      //oPC.LoginCreature.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfFirestorm, false, 2, new Vector3(0, 0, 0), new Vector3(0, 90, 0)));

      Task waitTourbillonEvents = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(2));
        oPC.LoginCreature.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfMysticalExplosion));

        await NwTask.Delay(TimeSpan.FromSeconds(1));
        oPC.LockCameraDirection(false);
        oPC.LockCameraDistance(false);
        oPC.LockCameraPitch(false);
        await oPC.LoginCreature.ClearActionQueue();
        Utils.DestroyInventory(oPC.LoginCreature);
        Utils.DestroyEquippedItems(oPC.LoginCreature);
        await NwItem.Create("learning_book", oPC.LoginCreature);
        NwItem rags = await NwItem.Create("rags", oPC.LoginCreature);
        //rags.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;
        oPC.LoginCreature.RunEquip(rags, InventorySlot.Chest);
        oPC.LoginCreature.Location = ((NwWaypoint)NwObject.FindObjectsWithTag("WP_START_NEW_CHAR").FirstOrDefault()).Location;

        await NwTask.WaitUntil(() => oPC.LoginCreature.Location.Area != null);
        await oPC.LoginCreature.PlayAnimation(Animation.LoopingDeadBack, 1, true, TimeSpan.FromSeconds(99999999));

        if (PlayerSystem.Players.TryGetValue(oPC.LoginCreature, out PlayerSystem.Player player))
        {
          if (!player.windows.ContainsKey("areaDescription")) player.windows.Add("areaDescription", new PlayerSystem.Player.AreaDescriptionWindow(player, NwModule.Instance.Areas.FirstOrDefault(a => a.Tag == "entry_scene")));
          else ((PlayerSystem.Player.AreaDescriptionWindow)player.windows["areaDescription"]).CreateWindow(NwModule.Instance.Areas.FirstOrDefault(a => a.Tag == "entry_scene"));
        }
        //oPC.FloatingTextString("En dehors des épaves de navires éparpillées tout autour de vous, la plage sur laquelle vous avez atterri semble étrangement calme et agréable. Nulle trace de votre équipage ou des biens que vous aviez emportés. Devant vous se dressent les murailles d'une ville ancienne et délabrée. Qu'allez-vous faire maintenant ?".ColorString(ColorConstants.Silver), false);
      });
    }
    private static async void TriggerRandomLightnings(NwArea area, Vector3 center, int maxDistance, NwCreature oPC)
    {
      oPC.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect((VfxType)286));

      int nbStrikes = Utils.random.Next(1, 5);

      for (int i = 0; i < nbStrikes; i++)
        Location.Create(area, new Vector3(center.X + Utils.random.Next(-maxDistance / 4, maxDistance), center.Y + Utils.random.Next(-maxDistance / 3, maxDistance / 3), 0), 0).ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpLightningM));

      foreach (NwCreature sailor in oPC.GetNearestCreatures().Where(c => c.Tag == "intro_sailor"))
      {
        switch (Utils.random.Next(0, 6))
        {
          case 0:
            await sailor.SpeakString("Oh bordel, c'est pas passé loin !");
            break;
          case 1:
            await sailor.SpeakString("Fichtre, encore un comme ça et est on foutu !");
            break;
          case 2:
            await sailor.SpeakString("Talos aie pitié de nous !");
            break;
          case 3:
            await sailor.SpeakString("Si jamais ça passe un peu plus près, j'donne pas cher de notre peau !");
            break;
          case 4:
            await sailor.SpeakString("J'crois que le moment est venu de paniquer !");
            break;
          case 5:
            await sailor.SpeakString("C'est la fin, le ciel nous tombe sur la tête !");
            break;
        }
      }

      await NwTask.Delay(TimeSpan.FromSeconds(Utils.random.Next(5, 10)));

      if (oPC.Area != area)
        return;

      TriggerRandomLightnings(area, center, maxDistance, oPC);
    }

    private static async void StrikeSailor(NwCreature sailor2, NwCreature sailor1)
    {
      await sailor2.SpeakString("Qu'est ce que ... ? Oooh, je me sens tout drôle ... A l'aide !".ColorString(ColorConstants.Lime));
      await sailor2.ClearActionQueue();
      await sailor2.PlayAnimation(Animation.LoopingSpasm, 3.0f, false, TimeSpan.FromHours(1));

      sailor2.VisualTransform.Lerp(new VisualTransformLerpSettings { LerpType = VisualTransformLerpType.SmootherStep, Duration = TimeSpan.FromSeconds(4), PauseWithGame = true }, transform => { transform.Translation = new Vector3(0, 0, 4); });

      await NwTask.Delay(TimeSpan.FromSeconds(2));

      sailor2.VisibilityOverride = VisibilityMode.Hidden;

      await NwTask.Delay(TimeSpan.FromSeconds(1));

      sailor2.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpLightningM));
      sailor2.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ComChunkRedLarge));
      
      //sailor2.PlotFlag = false;
     
      await sailor1.SpeakString("NOOOOOON, OLAF, MON FRERE JUMEAU ! Quelle horreur !".ColorString(ColorConstants.Red));
      //sailor2.ApplyEffect(EffectDuration.Instant, Effect.Damage(120, DamageType.Electrical));

    }
  }
}
