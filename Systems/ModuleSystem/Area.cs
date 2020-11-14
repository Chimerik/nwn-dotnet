using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  class Area
  {
    public readonly uint oid;
    public readonly string uuid;
    public readonly string tag;
    public readonly string name;
    public readonly List<uint> lootChestList;
    public Area(uint nwobj)
    {
      this.oid = nwobj;
      this.uuid = NWScript.GetObjectUUID(nwobj);
      this.tag = NWScript.GetTag(nwobj);
      this.name = NWScript.GetName(nwobj);
      this.lootChestList = new List<uint>();
      this.DoAreaSpecificInitialisation();

      /* var firstObject = NWScript.GetFirstObjectInArea(nwobj);

       if (NWScript.GetObjectType(firstObject) == NWScript.OBJECT_TYPE_PLACEABLE && Convert.ToBoolean(NWScript.GetHasInventory(firstObject))
         && LootSystem.lootablesDic.ContainsKey(NWScript.GetTag(firstObject)))
         lootChestList.Add(firstObject);

       var lootChest = NWScript.GetNearestObject(NWScript.OBJECT_TYPE_PLACEABLE, firstObject);
       int i = 1;

       while (Convert.ToBoolean(NWScript.GetIsObjectValid(lootChest)))
       {
         if(LootSystem.lootablesDic.ContainsKey(NWScript.GetTag(lootChest)))
           lootChestList.Add(lootChest);

         i++;
         lootChest = NWScript.GetNearestObject(NWScript.OBJECT_TYPE_PLACEABLE, firstObject, i);
       }*/
    }
    public void CleanArea()
    {
      DateTime lastEnteredDate;
      if (DateTime.TryParse(NWScript.GetLocalString(this.oid, "_LAST_ENTERED_ON"), out lastEnteredDate) && (DateTime.Now - lastEnteredDate).TotalMinutes >= 5)
      {
        var firstObject = NWScript.GetFirstObjectInArea(this.oid);
        int i = 1;
        var nearestObject = NWScript.GetNearestObject(NWScript.OBJECT_TYPE_CREATURE, firstObject);

        while (Convert.ToBoolean(NWScript.GetIsObjectValid(nearestObject)))
        {
          if (NWScript.GetIsPC(nearestObject) == 0)
            Utils.DestroyInventory(nearestObject);

          NWScript.DestroyObject(nearestObject);
          i++;
          nearestObject = NWScript.GetNearestObject(NWScript.OBJECT_TYPE_CREATURE, firstObject, i);
        }

        i = 1;
        nearestObject = NWScript.GetNearestObjectByTag("BodyBag", firstObject);

        while (Convert.ToBoolean(NWScript.GetIsObjectValid(nearestObject)))
        {
          Utils.DestroyInventory(nearestObject);
          NWScript.DestroyObject(nearestObject);
          i++;
          nearestObject = NWScript.GetNearestObjectByTag("BodyBag", firstObject, i);
        }

        i = 1;
        nearestObject = NWScript.GetNearestObject(NWScript.OBJECT_TYPE_ITEM, firstObject);

        while (Convert.ToBoolean(NWScript.GetIsObjectValid(nearestObject)))
        {
          NWScript.DestroyObject(nearestObject);
          i++;
          nearestObject = NWScript.GetNearestObject(NWScript.OBJECT_TYPE_ITEM, firstObject, i);
        }

        i = 1;
        nearestObject = NWScript.GetNearestObjectByTag("creature_spawn", firstObject);

        while (Convert.ToBoolean(NWScript.GetIsObjectValid(nearestObject)))
        {
          if (Convert.ToBoolean(NWScript.GetLocalInt(nearestObject, "_PNJ_SPAWN")))
            NWScript.DeleteLocalInt(nearestObject, "_SPAWN_BLOCKED");

          i++;
          nearestObject = NWScript.GetNearestObjectByTag("creature_spawn", firstObject, i);
        }

          if (NWScript.GetObjectType(firstObject) == NWScript.OBJECT_TYPE_CREATURE
          || NWScript.GetTag(firstObject) == "BodyBag" || NWScript.GetObjectType(firstObject) == NWScript.OBJECT_TYPE_ITEM)
        {
          if(Convert.ToBoolean(NWScript.GetHasInventory(firstObject)))
            Utils.DestroyInventory(firstObject);
          NWScript.DestroyObject(firstObject);
        }          
      }
    }
    public void DoAreaSpecificInitialisation()
    {
      switch (NWScript.GetTag(this.oid))
      {
        case "entry_scene":
          this.InitializeEntryArea();
          break;
      }
    }
    public void InitializeEntryArea()
    {
      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, NWScript.GetObjectByTag("intro_brouillard"), VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);
      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, NWScript.GetObjectByTag("intro_brouillard", 1), VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);
    }
    public void DoAreaSpecificBehavior(PlayerSystem.Player player )
    {
      if (NWScript.GetTag(this.oid) == $"entry_scene_{NWScript.GetPCPublicCDKey(player.oid)}")
      {
        NWScript.DelayCommand(5.0f, () => player.PlayIntroSong());
      }
    }
    public void StartEntryScene(PlayerSystem.Player player)
    {
      NWScript.AssignCommand(player.oid, () => NWScript.SetCameraFacing(180, 20, 65));
      
      NWScript.DelayCommand(0.2f, () => NWScript.LockCameraDirection(player.oid, 1));
      NWScript.DelayCommand(0.2f, () =>  NWScript.LockCameraDistance(player.oid, 1));
      NWScript.DelayCommand(0.2f, () =>  NWScript.LockCameraPitch(player.oid, 1));

      ObjectPlugin.SetDialogResref(NWScript.GetObjectByTag("intro_captain"), "");

      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, NWScript.GetObjectByTag("intro_brouillard"), VisibilityPlugin.NWNX_VISIBILITY_VISIBLE);
      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, NWScript.GetObjectByTag("intro_brouillard", 1), VisibilityPlugin.NWNX_VISIBILITY_VISIBLE);
     
      AreaPlugin.SetDayNightCycle(this.oid, AreaPlugin.NWNX_AREA_DAYNIGHTCYCLE_ALWAYS_DARK);
      AreaPlugin.SetWeatherChance(this.oid, AreaPlugin.NWNX_AREA_WEATHER_CHANCE_RAIN, 100);
      AreaPlugin.SetWeatherChance(this.oid, AreaPlugin.NWNX_AREA_WEATHER_CHANCE_LIGHTNING, 100);

      NWScript.SetAreaWind(this.oid, NWScript.Vector(1, 0, 0), 10.0f, 25.0f, 10.0f);
      NWScript.DelayCommand(2.0f, () => TriggerRandomLightnings(NWScript.GetPosition(player.oid), 25));

      uint rock1 = NWScript.GetObjectByTag("intro_recif_1");
      uint rock2 = NWScript.GetObjectByTag("intro_recif_2");
      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, rock1, VisibilityPlugin.NWNX_VISIBILITY_ALWAYS_VISIBLE);
      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, rock2, VisibilityPlugin.NWNX_VISIBILITY_ALWAYS_VISIBLE);
      MoveRock(rock1, rock2, player.oid);
    }
    private void MoveRock(uint rock1, uint rock2, uint oPC)
    {
      float position = NWScript.GetObjectVisualTransform(rock1, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y) - 0.25f;
      NWScript.SetObjectVisualTransform(rock1, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y, position);

      float position2 = NWScript.GetObjectVisualTransform(rock2, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y) - 0.25f;
      NWScript.SetObjectVisualTransform(rock2, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y, position2);

      //NWScript.SendMessageToPC(NWScript.GetFirstPC(), $"X = {position}");

      if (position < -75)
      {
        NWScript.DestroyObject(rock1);
        NWScript.DestroyObject(rock2);

        switch (NWScript.GetTag(rock1))
        {
          case "intro_recif_1":
            rock1 = NWScript.GetObjectByTag($"intro_recif_3");
            rock2 = NWScript.GetObjectByTag($"intro_recif_4");
            break;
          case "intro_recif_3":
            rock1 = NWScript.GetObjectByTag($"intro_tourbillon");
            rock2 = NWScript.OBJECT_INVALID;
            position = 0;
            break;
        }

        VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, rock1, VisibilityPlugin.NWNX_VISIBILITY_ALWAYS_VISIBLE);
        if(Convert.ToBoolean(NWScript.GetIsObjectValid(rock2)))
          VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, rock2, VisibilityPlugin.NWNX_VISIBILITY_ALWAYS_VISIBLE);
      }

      if (NWScript.GetTag(rock1) == "intro_tourbillon" && position < -45)
        this.PlayTourbillonEffects(oPC); 
      else if(Convert.ToBoolean(NWScript.GetIsObjectValid(rock1)))
        NWScript.DelayCommand(0.1f, () => MoveRock(rock1, rock2, oPC));
    }
    private void PlayTourbillonEffects(uint oPC)
    {
      NWScript.DelayCommand(10.0f, () => this.RemoveArea());

      NWScript.ApplyEffectAtLocation(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_FNF_PWKILL, 0, 2, new Vector3(0, 0, 0), new Vector3(270, 90, 0)), NWScript.GetLocation(oPC));
      NWScript.DelayCommand(2.0f, () => NWScript.ApplyEffectAtLocation(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_FNF_MYSTICAL_EXPLOSION), NWScript.GetLocation(oPC)));
      NWScript.ApplyEffectAtLocation(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_FNF_FIRESTORM, 0, 2, new Vector3(0, 0, 0), new Vector3(0, 0, 0)), NWScript.GetLocation(oPC));
      NWScript.ApplyEffectAtLocation(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_FNF_FIRESTORM, 0, 2, new Vector3(0, 0, 0), new Vector3(0, 90, 0)), NWScript.GetLocation(oPC));
      NWScript.DelayCommand(4.0f, () => NWScript.AssignCommand(oPC, () => NWScript.ClearAllActions()));
      NWScript.DelayCommand(4.1f, () => NWScript.AssignCommand(oPC, () => NWScript.JumpToObject(NWScript.GetWaypointByTag("WP_START_NEW_CHAR"))));

      NWScript.DelayCommand(4.0f, () => Utils.DestroyInventory(oPC));
      NWScript.DelayCommand(4.0f, () => Utils.DestroyEquippedItems(oPC));
      NWScript.DelayCommand(4.2f, () => NWScript.AssignCommand(oPC, () => NWScript.ActionEquipItem(NWScript.CreateItemOnObject("NW_CLOTH023", oPC), NWScript.INVENTORY_SLOT_CHEST)));

      NWScript.DelayCommand(0.2f, () => NWScript.LockCameraDirection(oPC, 0));
      NWScript.DelayCommand(0.2f, () => NWScript.LockCameraDistance(oPC, 0));
      NWScript.DelayCommand(0.2f, () => NWScript.LockCameraPitch(oPC, 0));
    }
    private void TriggerRandomLightnings(Vector3 center, int maxDistance)
    {
      int nbStrikes = Utils.random.Next(1, 5);

      for (int i = 0; i < nbStrikes; i++)
        NWScript.ApplyEffectAtLocation(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_IMP_LIGHTNING_M), NWScript.Location(this.oid, NWScript.Vector(center.X + Utils.random.Next(-maxDistance/4, maxDistance), center.Y + Utils.random.Next(-maxDistance/3, maxDistance/3), 0), 0));

      switch (Utils.random.Next(0, 6))
      {
        case 0:
          NWScript.AssignCommand(NWScript.GetObjectByTag("intro_sailor", Utils.random.Next(0, 2)), () => NWScript.SpeakString("Oh bordel, c'est pas passé loin !"));
          break;
        case 1:
          NWScript.AssignCommand(NWScript.GetObjectByTag("intro_sailor", Utils.random.Next(0, 2)), () => NWScript.SpeakString("Fichtre, encore un comme ça et est on foutu !"));
          break;
        case 2:
          NWScript.AssignCommand(NWScript.GetObjectByTag("intro_sailor", Utils.random.Next(0, 2)), () => NWScript.SpeakString("Talos, aie pitié de nous !"));
          break;
        case 3:
          NWScript.AssignCommand(NWScript.GetObjectByTag("intro_sailor", Utils.random.Next(0, 2)), () => NWScript.SpeakString("Si jamais ça passe un peu plus près, j'donne pas cher de notre peau !"));
          break;
        case 4:
          NWScript.AssignCommand(NWScript.GetObjectByTag("intro_sailor", Utils.random.Next(0, 2)), () => NWScript.SpeakString("J'crois que le moment est venu de paniquer !"));
          break;
        case 5:
          NWScript.AssignCommand(NWScript.GetObjectByTag("intro_sailor", Utils.random.Next(0, 2)), () => NWScript.SpeakString("C'est la fin, le ciel nous tombe sur la tête !"));
          break;
      }
      NWScript.DelayCommand(Utils.random.Next(5, 15), () => TriggerRandomLightnings(center, maxDistance));
    }
    private void RemoveArea()
    {
      NWScript.DestroyArea(this.oid);
      Module.areaDictionnary.Remove(this.uuid);
    }
  }
}
