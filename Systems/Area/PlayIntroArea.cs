using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using NWN.API.Constants;

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
      NwPlayer player = NWScript.GetLastSpeaker().ToNwObject<NwCreature>().ControllingPlayer;

      player.SetCameraFacing(180, 65, 20);

      ObjectPlugin.SetDialogResref(captain, "");
      captain.SpeakString("Des récifs ! Accrochez-vous, va falloir maneouvrer serré !");

      List<NwCreature> sailorList = captain.GetNearestCreatures(CreatureTypeFilter.PlayerChar(false)).Where(c => c.Tag == "intro_sailor").ToList();
      sailorList[0].ActionRandomWalk();
      sailorList[1].ActionRandomWalk();
      sailorList[0].SpeakString("Umberlie, épargne-nous !".ColorString(Color.ORANGE));
      sailorList[1].SpeakString("Oh non, non, non, faut faire quelque chose, vite !");
      sailorList[0].MovementRate = MovementRate.DM;
      sailorList[1].MovementRate = MovementRate.DM;

      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, NWScript.GetNearestObjectByTag("intro_brouillard", player.LoginCreature), VisibilityPlugin.NWNX_VISIBILITY_VISIBLE);
      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, NWScript.GetNearestObjectByTag("intro_brouillard", player.LoginCreature, 2), VisibilityPlugin.NWNX_VISIBILITY_VISIBLE);

      area.Weather = WeatherType.Rain;
      AreaPlugin.SetDayNightCycle(area, AreaPlugin.NWNX_AREA_DAYNIGHTCYCLE_ALWAYS_DARK);
      AreaPlugin.SetWeatherChance(area, AreaPlugin.NWNX_AREA_WEATHER_CHANCE_LIGHTNING, 100);
      NWScript.SetAreaWind(area, NWScript.Vector(1, 0, 0), 10.0f, 25.0f, 10.0f);

      List<NwPlaceable> rocks = player.LoginCreature.GetNearestObjectsByType<NwPlaceable>().Where(c => c.Tag == "intro_recif").ToList();
      NwPlaceable tourbillon = player.LoginCreature.GetNearestObjectsByType<NwPlaceable>().FirstOrDefault(c => c.Tag == "intro_tourbillon");

      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, rocks[0], VisibilityPlugin.NWNX_VISIBILITY_VISIBLE);
      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, rocks[1], VisibilityPlugin.NWNX_VISIBILITY_VISIBLE);
      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, rocks[2], VisibilityPlugin.NWNX_VISIBILITY_VISIBLE);
      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, tourbillon, VisibilityPlugin.NWNX_VISIBILITY_VISIBLE);

      NWScript.SetObjectVisualTransform(rocks[0], NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y, -72, 1, 30);
      NWScript.SetObjectVisualTransform(rocks[1], NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y, -80, 1, 24); 
      NWScript.SetObjectVisualTransform(rocks[2], NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y, -64, 1, 38);
      NWScript.SetObjectVisualTransform(tourbillon, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y, -15, 1, 40);

      Task waitIntroEvents = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0.3));
        player.LockCameraDirection(true);
        player.LockCameraDistance(true);
        player.LockCameraPitch(true);

        await NwTask.Delay(TimeSpan.FromSeconds(1.7));
        TriggerRandomLightnings(area, player.LoginCreature.Position, 25, player);

        await NwTask.Delay(TimeSpan.FromSeconds(23));
        StrikeSailor(sailorList[1], sailorList[0]);

        await NwTask.Delay(TimeSpan.FromSeconds(10));
        await captain.SpeakString("Qu'est ce que c'est que ce truc ? On ne peut pas éviter la collision, ABANDONNEZ LE NAVIRE !".ColorString(Color.RED));
        await NwTask.Delay(TimeSpan.FromSeconds(5));
        PlayTourbillonEffects(area, player);
      });

      /*Task waitTourbillon = NwTask.Run(async () =>
      {
        await NwTask.WaitUntil(() => NWScript.GetObjectVisualTransform(tourbillon, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y, 1) == -50);
        PlayTourbillonEffects(area, player);
      });
      test(player, tourbillon);*/
    }
    private static async void test(NwPlayer oPC, uint tourbillon)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(1));
      oPC.SendServerMessage($"tourbillon : {NWScript.GetObjectVisualTransform(tourbillon, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y, 0)}");
      test(oPC, tourbillon);
    }
    private static void PlayTourbillonEffects(NwArea area, NwPlayer oPC)
    {
      oPC.LoginCreature.Location.ApplyEffect(EffectDuration.Instant, API.Effect.VisualEffect(VfxType.FnfPwkill, false, 2, new Vector3(0, 0, 0), new Vector3(270, 90, 0)));
      oPC.LoginCreature.Location.ApplyEffect(EffectDuration.Instant, API.Effect.VisualEffect(VfxType.FnfFirestorm, false, 2, new Vector3(0, 0, 0), new Vector3(0, 0, 0)));
      oPC.LoginCreature.Location.ApplyEffect(EffectDuration.Instant, API.Effect.VisualEffect(VfxType.FnfFirestorm, false, 2, new Vector3(0, 0, 0), new Vector3(0, 90, 0)));

      Task waitTourbillonEvents = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(2));
        oPC.LoginCreature.Location.ApplyEffect(EffectDuration.Instant, API.Effect.VisualEffect(VfxType.FnfMysticalExplosion));

        await NwTask.Delay(TimeSpan.FromSeconds(1.6));
        oPC.LockCameraDirection(false);
        oPC.LockCameraDistance(false);
        oPC.LockCameraPitch(false);
        await oPC.LoginCreature.ClearActionQueue();
        Utils.DestroyInventory(oPC.LoginCreature);
        Utils.DestroyEquippedItems(oPC.LoginCreature);
        CreaturePlugin.RunEquip(oPC.LoginCreature, NwItem.Create("rags", oPC.LoginCreature), (int)InventorySlot.Chest);
        oPC.LoginCreature.Location = ((NwWaypoint)NwObject.FindObjectsWithTag("WP_START_NEW_CHAR").FirstOrDefault()).Location;

        await NwTask.WaitUntil(() => oPC.LoginCreature.Location.Area != null);
        await oPC.LoginCreature.PlayAnimation(Animation.LoopingDeadBack, 1, true, TimeSpan.FromSeconds(99999999));
        oPC.FloatingTextString("En dehors des épaves de navires éparpillées tout autour de vous, la plage sur laquelle vous avez atterri semble étrangement calme et agréable. Nulle trace de votre équipage ou des biens que vous aviez emportés. Devant vous se dressent les murailles d'une ville ancienne et délabrée. Qu'allez-vous faire maintenant ?".ColorString(Color.SILVER), false);
      });
    }
    private static void TriggerRandomLightnings(NwArea area, Vector3 center, int maxDistance, NwPlayer oPC)
    {
      int nbStrikes = Utils.random.Next(1, 5);

      for (int i = 0; i < nbStrikes; i++)
        NWScript.ApplyEffectAtLocation(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_IMP_LIGHTNING_M), NWScript.Location(area, NWScript.Vector(center.X + Utils.random.Next(-maxDistance / 4, maxDistance), center.Y + Utils.random.Next(-maxDistance / 3, maxDistance / 3), 0), 0));

      switch (Utils.random.Next(0, 6))
      {
        case 0:
          NWScript.AssignCommand(NWScript.GetNearestObjectByTag("intro_sailor", oPC.LoginCreature, Utils.random.Next(1, 3)), () => NWScript.SpeakString("Oh bordel, c'est pas passé loin !"));
          break;
        case 1:
          NWScript.AssignCommand(NWScript.GetNearestObjectByTag("intro_sailor", oPC.LoginCreature, Utils.random.Next(1, 3)), () => NWScript.SpeakString("Fichtre, encore un comme ça et est on foutu !"));
          break;
        case 2:
          NWScript.AssignCommand(NWScript.GetNearestObjectByTag("intro_sailor", oPC.LoginCreature, Utils.random.Next(1, 3)), () => NWScript.SpeakString("Talos, aie pitié de nous !"));
          break;
        case 3:
          NWScript.AssignCommand(NWScript.GetNearestObjectByTag("intro_sailor", oPC.LoginCreature, Utils.random.Next(1, 3)), () => NWScript.SpeakString("Si jamais ça passe un peu plus près, j'donne pas cher de notre peau !"));
          break;
        case 4:
          NWScript.AssignCommand(NWScript.GetNearestObjectByTag("intro_sailor", oPC.LoginCreature, Utils.random.Next(1, 3)), () => NWScript.SpeakString("J'crois que le moment est venu de paniquer !"));
          break;
        case 5:
          NWScript.AssignCommand(NWScript.GetNearestObjectByTag("intro_sailor", oPC.LoginCreature, Utils.random.Next(1, 3)), () => NWScript.SpeakString("C'est la fin, le ciel nous tombe sur la tête !"));
          break;
      }
      NWScript.DelayCommand(Utils.random.Next(5, 15), () => TriggerRandomLightnings(area, center, maxDistance, oPC));
    }

    private static void StrikeSailor(uint sailor2, uint sailor1)
    {
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_IMP_LIGHTNING_M), sailor2);
      NWScript.SetPlotFlag(sailor2, 0);
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectDamage(120, NWScript.DAMAGE_TYPE_ELECTRICAL, NWScript.DAMAGE_POWER_ENERGY), sailor2);
      NWScript.AssignCommand(sailor1, () => NWScript.SpeakString("NOOOOOON, OLAF, MON FRERE JUMEAU ! Quelle horreur !"));
    }
  }
}
