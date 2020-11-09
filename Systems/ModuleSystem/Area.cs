using System;
using System.Collections.Generic;
using System.Text;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  class Area
  {
    public readonly uint oid;
    public readonly string tag;
    public readonly string name;
    public readonly List<uint> lootChestList;
    public Area(uint nwobj)
    {
      this.oid = nwobj;
      this.tag = NWScript.GetTag(nwobj);
      this.name = NWScript.GetName(nwobj);
      this.lootChestList = new List<uint>();
      
      var firstObject = NWScript.GetFirstObjectInArea(nwobj);

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
      }
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
  }
}
