using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class PlaceableSystem
  {
    private static int HandleBeforeItemAddedToRefinery(uint oidSelf)
    {
      var oItem = NWScript.StringToObject(EventsPlugin.GetEventData("ITEM"));

      if (NWScript.GetTag(oItem) != "ore" || NWScript.GetTag(oItem) != "mineral")
      {
        EventsPlugin.SkipEvent();
        NWScript.SpeakString("Seul le minerai peut être raffiné dans la fonderie.");
      }
      return 0;
    }
    private static int HandleItemAddedToRefinery(uint oidSelf)
    {
      if (NWScript.GetInventoryDisturbType() == NWScript.INVENTORY_DISTURB_TYPE_ADDED)
      {
        var item = NWScript.GetInventoryDisturbItem();

        if (NWScript.GetTag(item) == "ore")
        {
          PlayerSystem.Player player;
          if (PlayerSystem.Players.TryGetValue(NWScript.GetLastDisturbed(), out player))
          {
            string reprocessingData = $"{NWScript.GetName(item)} : Efficacité raffinage -30 % (base fonderie)";

            int value;
            if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Reprocessing)), out value))
              reprocessingData += $"\n x1.{3 * value} (Raffinage)";

            if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.ReprocessingEfficiency)), out value))
              reprocessingData += $"\n x1.{2 * value} (Raffinage efficace)";

            CollectSystem.Ore processedOre;
            if (CollectSystem.oresDictionnary.TryGetValue(CollectSystem.GetOreTypeFromName(NWScript.GetName(item)), out processedOre))
              if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)processedOre.feat)), out value))
                reprocessingData += $"\n x1.{2 * value} (Raffinage {NWScript.GetName(item)})";

            float connectionsLevel;
            if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Connections)), out connectionsLevel))
              reprocessingData += $"\n x{1.00 - connectionsLevel / 100} (Raffinage {NWScript.GetName(item)})";

            NWScript.SendMessageToPC(player.oid, reprocessingData);
          }
        }
      }

      return 0;
    }
    private static int HandleRefineryClose(uint oidSelf)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(NWScript.GetLastClosedBy(), out player))
      {
        var fonderie = oidSelf;
        float reprocessingEfficiency = 0.3f;

        float value;
        if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Reprocessing)), out value))
          reprocessingEfficiency += reprocessingEfficiency + 3 * value / 100;

        if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.ReprocessingEfficiency)), out value))
          reprocessingEfficiency += reprocessingEfficiency + 2 * value / 100;

        if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Connections)), out value))
          reprocessingEfficiency += reprocessingEfficiency + 1 * value / 100;

        var ore = NWScript.GetFirstItemInInventory(fonderie);
        while (NWScript.GetIsObjectValid(ore) == 1)
        {
          if (NWScript.GetTag(ore) == "ore")
          {
            if (NWScript.GetItemStackSize(ore) > 100)
            {
              CollectSystem.Ore processedOre;
              if (CollectSystem.oresDictionnary.TryGetValue(CollectSystem.GetOreTypeFromName(NWScript.GetName(ore)), out processedOre))
              {
                if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)processedOre.feat)), out value))
                  reprocessingEfficiency += reprocessingEfficiency + 2 * value / 100;

                foreach (KeyValuePair<CollectSystem.MineralType, float> mineralKeyValuePair in processedOre.mineralsDictionnary)
                {
                  var mineral = NWScript.CreateItemOnObject("mineral", player.oid, (int)((NWScript.GetItemStackSize(ore) * mineralKeyValuePair.Value * (int)reprocessingEfficiency)));
                  NWScript.SetName(mineral, CollectSystem.GetNameFromMineralType(mineralKeyValuePair.Key));
                  NWScript.SetLocalInt(mineral, "DROPS_ON_DEATH", 1);
                }

                NWScript.DestroyObject(ore);
              }
            }
            else
              NWScript.SendMessageToPC(player.oid, $"Ce lot de {NWScript.GetName(ore)} n'a pas pu être raffiné. Un minimum de 100 unités est nécessaire pour le bon fonctionnement de la fonderie.");
          }

          ore = NWScript.GetNextItemInInventory(fonderie);
        }
      }

      return 0;
    }
  }
}
