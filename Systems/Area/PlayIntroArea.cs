using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
      NwPlayer player = NWScript.GetLastSpeaker().ToNwObject<NwPlayer>();

      player.SetCameraFacing(180, 65, 20);

      Task task3 = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0.3));
        player.LockCameraDirection(true);
        player.LockCameraDistance(true);
        player.LockCameraPitch(true);
        return true;
      });

      ObjectPlugin.SetDialogResref(captain, "");
      captain.SpeakString("Des récifs ! Accrochez-vous, va falloir maneouvrer serré !");

      uint sailor1 = NWScript.GetNearestObjectByTag("intro_sailor", player);
      uint sailor2 = NWScript.GetNearestObjectByTag("intro_sailor", player, 2);
      NWScript.AssignCommand(sailor1, () => NWScript.ActionRandomWalk());
      NWScript.AssignCommand(sailor1, () => NWScript.SpeakString("Umberlie, épargne-nous !"));
      NWScript.AssignCommand(sailor2, () => NWScript.ActionRandomWalk());
      NWScript.AssignCommand(sailor2, () => NWScript.SpeakString("Oh non, non, non, faut faire quelque chose, vite !"));
      CreaturePlugin.SetMovementRate(sailor1, CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_DM_FAST);
      CreaturePlugin.SetMovementRate(sailor2, CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_DM_FAST);

      NWScript.DelayCommand(25.0f, () => StrikeSailor(sailor2, sailor1));
      NWScript.DelayCommand(45.0f, () => NWScript.AssignCommand(captain, () => NWScript.SpeakString("Qu'est ce que c'est que ce truc ? On ne peut pas éviter la collision, ABANDONNEZ LE NAVIRE !")));

      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, NWScript.GetNearestObjectByTag("intro_brouillard", player), VisibilityPlugin.NWNX_VISIBILITY_VISIBLE);
      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, NWScript.GetNearestObjectByTag("intro_brouillard", player, 2), VisibilityPlugin.NWNX_VISIBILITY_VISIBLE);

      AreaPlugin.SetDayNightCycle(area, AreaPlugin.NWNX_AREA_DAYNIGHTCYCLE_ALWAYS_DARK);
      AreaPlugin.SetWeatherChance(area, AreaPlugin.NWNX_AREA_WEATHER_CHANCE_RAIN, 100);
      AreaPlugin.SetWeatherChance(area, AreaPlugin.NWNX_AREA_WEATHER_CHANCE_LIGHTNING, 100);

      NWScript.SetAreaWind(area, NWScript.Vector(1, 0, 0), 10.0f, 25.0f, 10.0f);
      NWScript.DelayCommand(2.0f, () => TriggerRandomLightnings(area, NWScript.GetPosition(player), 25, player));

      uint rock1 = NWScript.GetNearestObjectByTag("intro_recif", player);
      uint rock2 = NWScript.GetNearestObjectByTag("intro_recif", player, 2);
      uint rock3 = NWScript.GetNearestObjectByTag("intro_recif", player, 3);
      uint tourbillon = NWScript.GetNearestObjectByTag("intro_tourbillon", player);

      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, rock1, VisibilityPlugin.NWNX_VISIBILITY_ALWAYS_VISIBLE);
      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, rock2, VisibilityPlugin.NWNX_VISIBILITY_ALWAYS_VISIBLE);
      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, rock3, VisibilityPlugin.NWNX_VISIBILITY_ALWAYS_VISIBLE);
      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, tourbillon, VisibilityPlugin.NWNX_VISIBILITY_ALWAYS_VISIBLE);

      MoveRock(area, rock1, rock2, rock3, tourbillon, player);
    }
    private static void MoveRock(NwArea area, uint rock1, uint rock2, uint rock3, uint tourbillon, NwPlayer oPC)
    {
      NWScript.SetObjectVisualTransform(rock1, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y, NWScript.GetObjectVisualTransform(rock1, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y) - 0.25f);
      NWScript.SetObjectVisualTransform(rock2, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y, NWScript.GetObjectVisualTransform(rock2, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y) - 0.25f);
      NWScript.SetObjectVisualTransform(rock3, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y, NWScript.GetObjectVisualTransform(rock3, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y) - 0.25f);
      float position = NWScript.SetObjectVisualTransform(tourbillon, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y, NWScript.GetObjectVisualTransform(tourbillon, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y) - 0.25f);

      if (position < -45)
        PlayTourbillonEffects(area, oPC);
      else
        NWScript.DelayCommand(0.1f, () => MoveRock(area, rock1, rock2, rock3, tourbillon, oPC));
    }
    private static void PlayTourbillonEffects(NwArea area, NwPlayer oPC)
    {
      NWScript.ApplyEffectAtLocation(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_FNF_PWKILL, 0, 2, new Vector3(0, 0, 0), new Vector3(270, 90, 0)), NWScript.GetLocation(oPC));
      NWScript.DelayCommand(2.0f, () => NWScript.ApplyEffectAtLocation(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_FNF_MYSTICAL_EXPLOSION), NWScript.GetLocation(oPC)));
      NWScript.ApplyEffectAtLocation(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_FNF_FIRESTORM, 0, 2, new Vector3(0, 0, 0), new Vector3(0, 0, 0)), NWScript.GetLocation(oPC));
      NWScript.ApplyEffectAtLocation(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_FNF_FIRESTORM, 0, 2, new Vector3(0, 0, 0), new Vector3(0, 90, 0)), NWScript.GetLocation(oPC));
      NWScript.DelayCommand(3.6f, () => NWScript.AssignCommand(oPC, () => NWScript.ClearAllActions()));
      NWScript.DelayCommand(3.7f, () => NWScript.AssignCommand(oPC, () => NWScript.JumpToObject(NWScript.GetWaypointByTag("WP_START_NEW_CHAR"))));

      NWN.Utils.DestroyInventory(oPC);
      NWN.Utils.DestroyEquippedItems(oPC);

      NWScript.DelayCommand(3.4f, () => NWScript.AssignCommand(oPC, () => NWScript.ActionEquipItem(NWScript.CreateItemOnObject("rags", oPC), NWScript.INVENTORY_SLOT_CHEST)));
      NWScript.DelayCommand(5.0f, () => NWScript.AssignCommand(oPC, () => NWScript.PlayAnimation(NWScript.ANIMATION_LOOPING_DEAD_BACK, 1, 999999.99f)));
      NWScript.DelayCommand(8.0f, () => NWScript.FloatingTextStringOnCreature("En dehors des épaves de navires éparpillées toutes autour de vous, la plage sur laquelle vous avez atterri semble étrangement calme et agréable. Nulle trace de votre équipage ou des biens que vous aviez emportés. Devant vous se dressent les murailles d'une ville ancienne et délabrée. Qu'allez-vous faire maintenant ?", oPC, 0));

      NWScript.DelayCommand(0.2f, () => NWScript.LockCameraDirection(oPC, 0));
      NWScript.DelayCommand(0.2f, () => NWScript.LockCameraDistance(oPC, 0));
      NWScript.DelayCommand(0.2f, () => NWScript.LockCameraPitch(oPC, 0));
    }
    private static void TriggerRandomLightnings(NwArea area, Vector3 center, int maxDistance, NwPlayer oPC)
    {
      int nbStrikes = NWN.Utils.random.Next(1, 5);

      for (int i = 0; i < nbStrikes; i++)
        NWScript.ApplyEffectAtLocation(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_IMP_LIGHTNING_M), NWScript.Location(area, NWScript.Vector(center.X + NWN.Utils.random.Next(-maxDistance / 4, maxDistance), center.Y + NWN.Utils.random.Next(-maxDistance / 3, maxDistance / 3), 0), 0));

      switch (NWN.Utils.random.Next(0, 6))
      {
        case 0:
          NWScript.AssignCommand(NWScript.GetNearestObjectByTag("intro_sailor", oPC, NWN.Utils.random.Next(1, 3)), () => NWScript.SpeakString("Oh bordel, c'est pas passé loin !"));
          break;
        case 1:
          NWScript.AssignCommand(NWScript.GetNearestObjectByTag("intro_sailor", oPC, NWN.Utils.random.Next(1, 3)), () => NWScript.SpeakString("Fichtre, encore un comme ça et est on foutu !"));
          break;
        case 2:
          NWScript.AssignCommand(NWScript.GetNearestObjectByTag("intro_sailor", oPC, NWN.Utils.random.Next(1, 3)), () => NWScript.SpeakString("Talos, aie pitié de nous !"));
          break;
        case 3:
          NWScript.AssignCommand(NWScript.GetNearestObjectByTag("intro_sailor", oPC, NWN.Utils.random.Next(1, 3)), () => NWScript.SpeakString("Si jamais ça passe un peu plus près, j'donne pas cher de notre peau !"));
          break;
        case 4:
          NWScript.AssignCommand(NWScript.GetNearestObjectByTag("intro_sailor", oPC, NWN.Utils.random.Next(1, 3)), () => NWScript.SpeakString("J'crois que le moment est venu de paniquer !"));
          break;
        case 5:
          NWScript.AssignCommand(NWScript.GetNearestObjectByTag("intro_sailor", oPC, NWN.Utils.random.Next(1, 3)), () => NWScript.SpeakString("C'est la fin, le ciel nous tombe sur la tête !"));
          break;
      }
      NWScript.DelayCommand(NWN.Utils.random.Next(5, 15), () => TriggerRandomLightnings(area, center, maxDistance, oPC));
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
