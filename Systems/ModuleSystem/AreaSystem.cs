using System;
using System.Collections.Generic;
using System.Numerics;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static class AreaSystem
  {
    public static Dictionary<string, Area> areaDictionnary = new Dictionary<string, Area>();

    public static void CreateArea(uint nwobj)
    {
      var area = new Area(nwobj);
      areaDictionnary.Add(area.uuid, area);
      DoAreaSpecificInitialisation(area);

      var firstObject = NWScript.GetFirstObjectInArea(nwobj);

      if (NWScript.GetTag(firstObject) == "loot_chest")
      {
        NWScript.SetTag(firstObject, NWScript.GetLocalString(firstObject, "_LOOT_REFERENCE"));

        if (LootSystem.lootablesDic.ContainsKey(NWScript.GetTag(firstObject)))
          area.lootChestList.Add(firstObject);
        else
          Utils.LogMessageToDMs($"LOOT SYSTEM - Area {NWScript.GetName(nwobj)} - Chest {NWScript.GetName(firstObject)} not found in loot table.");
      }

      var lootChest = NWScript.GetNearestObjectByTag("loot_chest", firstObject);
      int i = 1;

      while (Convert.ToBoolean(NWScript.GetIsObjectValid(lootChest)))
      {
        NWScript.SetTag(lootChest, NWScript.GetLocalString(lootChest, "_LOOT_REFERENCE"));

        if (LootSystem.lootablesDic.ContainsKey(NWScript.GetLocalString(lootChest, "_LOOT_REFERENCE")))
          area.lootChestList.Add(lootChest);
        else
          Utils.LogMessageToDMs($"LOOT SYSTEM - Area {NWScript.GetName(nwobj)} - Chest {NWScript.GetName(lootChest)} not found in loot table.");

        i++;
        lootChest = NWScript.GetNearestObjectByTag("loot_chest", firstObject, i);
      }
    }

    public static void RemoveArea(Area area)
    {
      area.DeferDestroy();
      areaDictionnary.Remove(area.uuid);
    }

    public static void DoAreaSpecificInitialisation(Area area)
    {
      switch (NWScript.GetTag(area.oid))
      {
        case "entry_scene":
          InitializeEntryArea();
          area.level = 0;
          break;
        case "SimilisseThetreSalledeSpectacle":
          uint scene = NWScript.GetObjectByTag("theater_scene");
          NWScript.SetEventScript(scene, NWScript.EVENT_SCRIPT_TRIGGER_ON_OBJECT_ENTER, "onent_theater_sc");
          NWScript.SetEventScript(scene, NWScript.EVENT_SCRIPT_TRIGGER_ON_OBJECT_EXIT, "onex_theater_sc");
          area.level = 0;
          break;
        case "SIMILISCITYGATE":
        case "Similiscityentrepot":
        case "Similisse":
        case "Promenadetest":
        case "SimilisseQuartierdelaPromenadeAt":
        case "entrepotpersonnel":
        case "Forge":
        case "SimilisseQuartierdelaPromenadeTa":
        case "similisseslums":
        case "SIMILISSE_BIBLIOTHEQUE":
        case "Dispensaire":
        case "couronnedecuivre":
        case "similissetempledistrict":
        case "SIMILISSE_THERMES":
        case "Governmenttest":
        case "ChateauRepoduction":
        case "PalaceGardenTest":
        case "SimilisseTribunalBureaudesAvocat":
        case "SimilisseTribunal":
        case "SimilisseTribunalPrison":
        case "SimilisseSalleDesDelibrations":
        case "Sawmill":
          area.level = 0;
          break;
        case "lepontdaruthen":
        case "Fermesnord":
        case "fermes_ouest":
        case "terres_de_fryar":
        case "vallee":
        case "cave_flooded":
        case "cave_uw_ruins_entry":
          area.level = 2;
          break;
        case "chemin_interdit":
        case "collines_mugissantes":
        case "basse_montagne":
        case "haute_montagne":
        case "GoblinTunnels":
        case "caverne_kobolts":
          area.level = 3;
          break;
        case "epine_seeksa":
        case "OrcEncampment":
        case "vallee_caverne":
        case "cave_kuotoa":
          area.level = 4;
          break;
        case "SaltMines":
          area.level = 5;
          break;
        case "ant_nest":
          area.level = 6;
          break;
      }
    }
    public static void InitializeEntryArea()
    {
      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, NWScript.GetObjectByTag("intro_brouillard"), VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);
      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, NWScript.GetObjectByTag("intro_brouillard", 1), VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);
    }
    public static void DoAreaSpecificBehavior(Area area, PlayerSystem.Player player)
    {

    }
    public static void StartEntryScene(Area area, PlayerSystem.Player player)
    {
      NWScript.AssignCommand(player.oid, () => NWScript.SetCameraFacing(180, 20, 65));

      NWScript.DelayCommand(0.2f, () => NWScript.LockCameraDirection(player.oid, 1));
      NWScript.DelayCommand(0.2f, () => NWScript.LockCameraDistance(player.oid, 1));
      NWScript.DelayCommand(0.2f, () => NWScript.LockCameraPitch(player.oid, 1));

      uint oCaptain = NWScript.GetNearestObjectByTag("intro_captain", player.oid);
      ObjectPlugin.SetDialogResref(oCaptain, "");
      NWScript.AssignCommand(oCaptain, () => NWScript.SpeakString("Des récifs ! Accrochez-vous, va falloir maneouvrer serré !"));

      uint sailor1 = NWScript.GetNearestObjectByTag("intro_sailor", player.oid);
      uint sailor2 = NWScript.GetNearestObjectByTag("intro_sailor", player.oid, 2);
      NWScript.AssignCommand(sailor1, () => NWScript.ActionRandomWalk());
      NWScript.AssignCommand(sailor1, () => NWScript.SpeakString("Umberlie, épargne-nous !"));
      NWScript.AssignCommand(sailor2, () => NWScript.ActionRandomWalk());
      NWScript.AssignCommand(sailor2, () => NWScript.SpeakString("Oh non, non, non, faut faire quelque chose, vite !"));
      CreaturePlugin.SetMovementRate(sailor1, CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_DM_FAST);
      CreaturePlugin.SetMovementRate(sailor2, CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_DM_FAST);

      NWScript.DelayCommand(25.0f, () => StrikeSailor(sailor2, sailor1));
      NWScript.DelayCommand(45.0f, () => NWScript.AssignCommand(oCaptain, () => NWScript.SpeakString("Qu'est ce que c'est que ce truc ? On ne peut pas éviter la collision, ABANDONNEZ LE NAVIRE !")));

      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, NWScript.GetNearestObjectByTag("intro_brouillard", player.oid), VisibilityPlugin.NWNX_VISIBILITY_VISIBLE);
      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, NWScript.GetNearestObjectByTag("intro_brouillard", player.oid, 2), VisibilityPlugin.NWNX_VISIBILITY_VISIBLE);

      AreaPlugin.SetDayNightCycle(area.oid, AreaPlugin.NWNX_AREA_DAYNIGHTCYCLE_ALWAYS_DARK);
      AreaPlugin.SetWeatherChance(area.oid, AreaPlugin.NWNX_AREA_WEATHER_CHANCE_RAIN, 100);
      AreaPlugin.SetWeatherChance(area.oid, AreaPlugin.NWNX_AREA_WEATHER_CHANCE_LIGHTNING, 100);

      NWScript.SetAreaWind(area.oid, NWScript.Vector(1, 0, 0), 10.0f, 25.0f, 10.0f);
      NWScript.DelayCommand(2.0f, () => TriggerRandomLightnings(area, NWScript.GetPosition(player.oid), 25, player.oid));

      uint rock1 = NWScript.GetNearestObjectByTag("intro_recif", player.oid);
      uint rock2 = NWScript.GetNearestObjectByTag("intro_recif", player.oid, 2);
      uint rock3 = NWScript.GetNearestObjectByTag("intro_recif", player.oid, 3);
      uint tourbillon = NWScript.GetNearestObjectByTag("intro_tourbillon", player.oid);

      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, rock1, VisibilityPlugin.NWNX_VISIBILITY_ALWAYS_VISIBLE);
      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, rock2, VisibilityPlugin.NWNX_VISIBILITY_ALWAYS_VISIBLE);
      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, rock3, VisibilityPlugin.NWNX_VISIBILITY_ALWAYS_VISIBLE);
      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, tourbillon, VisibilityPlugin.NWNX_VISIBILITY_ALWAYS_VISIBLE);

      MoveRock(area, rock1, rock2, rock3, tourbillon, player.oid);
    }
    private static void MoveRock(Area area, uint rock1, uint rock2, uint rock3, uint tourbillon, uint oPC)
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
    private static void PlayTourbillonEffects(Area area, uint oPC)
    {
      NWScript.DelayCommand(10.0f, () => RemoveArea(area));

      NWScript.ApplyEffectAtLocation(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_FNF_PWKILL, 0, 2, new Vector3(0, 0, 0), new Vector3(270, 90, 0)), NWScript.GetLocation(oPC));
      NWScript.DelayCommand(2.0f, () => NWScript.ApplyEffectAtLocation(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_FNF_MYSTICAL_EXPLOSION), NWScript.GetLocation(oPC)));
      NWScript.ApplyEffectAtLocation(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_FNF_FIRESTORM, 0, 2, new Vector3(0, 0, 0), new Vector3(0, 0, 0)), NWScript.GetLocation(oPC));
      NWScript.ApplyEffectAtLocation(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_FNF_FIRESTORM, 0, 2, new Vector3(0, 0, 0), new Vector3(0, 90, 0)), NWScript.GetLocation(oPC));
      NWScript.DelayCommand(3.6f, () => NWScript.AssignCommand(oPC, () => NWScript.ClearAllActions()));
      NWScript.DelayCommand(3.7f, () => NWScript.AssignCommand(oPC, () => NWScript.JumpToObject(NWScript.GetWaypointByTag("WP_START_NEW_CHAR"))));

      Utils.DestroyInventory(oPC);
      Utils.DestroyEquippedItems(oPC);
      NWScript.DelayCommand(3.4f, () => NWScript.AssignCommand(oPC, () => NWScript.ActionEquipItem(NWScript.CreateItemOnObject("NW_CLOTH023", oPC), NWScript.INVENTORY_SLOT_CHEST)));
      NWScript.DelayCommand(5.0f, () => NWScript.AssignCommand(oPC, () => NWScript.PlayAnimation(NWScript.ANIMATION_LOOPING_DEAD_BACK, 1, 999999.99f)));
      NWScript.DelayCommand(8.0f, () => NWScript.FloatingTextStringOnCreature("En dehors des épaves de navires éparpillées toutes autour de vous, la plage sur laquelle vous avez atterri semble étrangement calme et agréable. Nulle trace de votre équipage ou des biens que vous aviez emportés. Devant vous se dressent les murailles d'une ville ancienne et délabrée. Qu'allez-vous faire maintenant ?", oPC, 0));

      NWScript.DelayCommand(0.2f, () => NWScript.LockCameraDirection(oPC, 0));
      NWScript.DelayCommand(0.2f, () => NWScript.LockCameraDistance(oPC, 0));
      NWScript.DelayCommand(0.2f, () => NWScript.LockCameraPitch(oPC, 0));
    }
    private static void TriggerRandomLightnings(Area area, Vector3 center, int maxDistance, uint oPC)
    {
      int nbStrikes = Utils.random.Next(1, 5);

      for (int i = 0; i < nbStrikes; i++)
        NWScript.ApplyEffectAtLocation(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_IMP_LIGHTNING_M), NWScript.Location(area.oid, NWScript.Vector(center.X + Utils.random.Next(-maxDistance / 4, maxDistance), center.Y + Utils.random.Next(-maxDistance / 3, maxDistance / 3), 0), 0));

      switch (Utils.random.Next(0, 6))
      {
        case 0:
          NWScript.AssignCommand(NWScript.GetNearestObjectByTag("intro_sailor", oPC, Utils.random.Next(1, 3)), () => NWScript.SpeakString("Oh bordel, c'est pas passé loin !"));
          break;
        case 1:
          NWScript.AssignCommand(NWScript.GetNearestObjectByTag("intro_sailor", oPC, Utils.random.Next(1, 3)), () => NWScript.SpeakString("Fichtre, encore un comme ça et est on foutu !"));
          break;
        case 2:
          NWScript.AssignCommand(NWScript.GetNearestObjectByTag("intro_sailor", oPC, Utils.random.Next(1, 3)), () => NWScript.SpeakString("Talos, aie pitié de nous !"));
          break;
        case 3:
          NWScript.AssignCommand(NWScript.GetNearestObjectByTag("intro_sailor", oPC, Utils.random.Next(1, 3)), () => NWScript.SpeakString("Si jamais ça passe un peu plus près, j'donne pas cher de notre peau !"));
          break;
        case 4:
          NWScript.AssignCommand(NWScript.GetNearestObjectByTag("intro_sailor", oPC, Utils.random.Next(1, 3)), () => NWScript.SpeakString("J'crois que le moment est venu de paniquer !"));
          break;
        case 5:
          NWScript.AssignCommand(NWScript.GetNearestObjectByTag("intro_sailor", oPC, Utils.random.Next(1, 3)), () => NWScript.SpeakString("C'est la fin, le ciel nous tombe sur la tête !"));
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

    public static bool GetIsAnyPlayerInInvalidArea()
    {
      bool isAnyPlayerInInvalidArea = false;

      var oPC = NWScript.GetFirstPC();

      while (NWScript.GetIsObjectValid(oPC) == 1 && isAnyPlayerInInvalidArea == false)
      {
        if (NWScript.GetIsObjectValid(NWScript.GetArea(oPC)) != 1) isAnyPlayerInInvalidArea = true;
        oPC = NWScript.GetNextPC();
      }

      return isAnyPlayerInInvalidArea;
    }
  }
}
