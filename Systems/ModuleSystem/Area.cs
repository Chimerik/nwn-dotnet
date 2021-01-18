using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public class Area
  {
    public readonly uint oid;
    public readonly string uuid;
    public readonly string tag;
    public readonly string name;
    public int level { get; set; }
    public readonly List<uint> lootChestList;

    public Area(uint nwobj)
    {
      oid = nwobj;
      uuid = NWScript.GetObjectUUID(nwobj);
      tag = NWScript.GetTag(nwobj);
      name = NWScript.GetName(nwobj);
      level = 1;
      lootChestList = new List<uint>();
    }

    public void Clean()
    {
      if (AreaPlugin.GetNumberOfPlayersInArea(this.oid) == 0 && DateTime.TryParse(NWScript.GetLocalString(this.oid, "_LAST_ENTERED_ON"), out DateTime lastEnteredDate) && (DateTime.Now - lastEnteredDate).TotalMinutes >= 5)
      {
        var firstObject = NWScript.GetFirstObjectInArea(this.oid);
        int i = 1;
        var nearestObject = NWScript.GetNearestObject(NWScript.OBJECT_TYPE_CREATURE, firstObject);

        while (Convert.ToBoolean(NWScript.GetIsObjectValid(nearestObject)))
        {
          if (!Convert.ToBoolean(NWScript.GetIsPC(nearestObject)))
            Utils.DestroyInventory(nearestObject);

          if (NWScript.GetTag(nearestObject) != "Statuereptilienne")
          {
            if(NWScript.GetEventScript(nearestObject, NWScript.EVENT_SCRIPT_CREATURE_ON_DEATH) ==  "od_spawn_npc_wp")
              NWScript.SetLocalString(NWScript.CreateObject(NWScript.OBJECT_TYPE_WAYPOINT, NWScript.GetLocalString(nearestObject, "_WAYPOINT_TEMPLATE"), NWScript.GetLocation(nearestObject)), "_CREATURE_TEMPLATE", NWScript.GetResRef(nearestObject));

            NWScript.DestroyObject(nearestObject);
          }

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
        nearestObject = NWScript.GetNearestObject(NWScript.OBJECT_TYPE_STORE, firstObject);

        while (Convert.ToBoolean(NWScript.GetIsObjectValid(nearestObject)))
        {
          NWScript.DestroyObject(nearestObject);
          i++;
          nearestObject = NWScript.GetNearestObject(NWScript.OBJECT_TYPE_STORE, firstObject, i);
        }

        if (NWScript.GetObjectType(firstObject) == NWScript.OBJECT_TYPE_CREATURE || NWScript.GetObjectType(firstObject) == NWScript.OBJECT_TYPE_STORE
          || NWScript.GetTag(firstObject) == "BodyBag" || NWScript.GetObjectType(firstObject) == NWScript.OBJECT_TYPE_ITEM)
        {
          if (Convert.ToBoolean(NWScript.GetHasInventory(firstObject)))
            Utils.DestroyInventory(firstObject);
          NWScript.DestroyObject(firstObject);
        }
      }
    }

    public void DeferDestroy(float delay = 30.0f)
    {
      NWScript.DelayCommand(delay, () =>
      {
        if (AreaSystem.GetIsAnyPlayerInInvalidArea())
        {
          // Don't destroy Area because player can get stuck in loading screen
          DeferDestroy(delay);
          return;
        }

        var result = NWScript.DestroyArea(oid);

        if (result == -2) // Players in area
        {
          DeferDestroy(delay);
          return;
        }
      });
    }
  }
}
