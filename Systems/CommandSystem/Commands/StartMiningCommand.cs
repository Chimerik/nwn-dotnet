using System;
using System.Numerics;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteStartMiningCommand(ChatSystem.Context ctx, Options.Result options)
    {
      PlayerSystem.Player player;

      // When player is invalid, do nothing
      if (!PlayerSystem.Players.TryGetValue(ctx.oSender, out player)) return;

      uint oExtractor = NWScript.GetItemInSlot(NWScript.INVENTORY_SLOT_RIGHTHAND, player.oid);

      if (NWScript.GetBaseItemType(oExtractor) != 115) // 115 = extracteur
      {
        NWScript.SendMessageToPC(player.oid, $"Il vous faut vous munir d'un extracteur pour démarrer l'extraction !");
        return;
      }

      Action<uint, Vector3> callback = (uint target, Vector3 position) =>
      {
        var oPlaceable = target;

        if (NWScript.GetTag(oPlaceable) != "mineable_rock")
        {
          NWScript.SendMessageToPC(player.oid, $"{NWScript.GetName(oPlaceable)} n'est pas un filon de minerai. Impossible de démarrer l'extraction.");
          return;
        }

        if (NWScript.GetDistanceBetween(player.oid, oPlaceable) > 5.0f)
        {
          NWScript.SendMessageToPC(player.oid, $"{NWScript.GetName(oPlaceable)} n'est pas à portée. Rapprochez-vous pour démarrer l'extraction.");
          return;
        }

        Action cancelCycle = () =>
        {
          Utils.RemoveTaggedEffect(oPlaceable, $"_{NWScript.GetPCPublicCDKey(player.oid)}_MINING_BEAM");
          CollectSystem.RemoveMiningCycleCallbacks(player);   // supprimer la callback de CompleteMiningCycle
        };

        Action completeCycle = () =>
        {
          NWScript.SendMessageToPC(NWScript.GetFirstPC(), "");
          Utils.RemoveTaggedEffect(oPlaceable, $"_{NWScript.GetPCPublicCDKey(player.oid)}_MINING_BEAM");
          CollectSystem.RemoveMiningCycleCallbacks(player);   // supprimer la callback de Cancel MiningCycle

          if (NWScript.GetIsObjectValid(oPlaceable) != 1 || NWScript.GetDistanceBetween(player.oid, oPlaceable) > 5.0f)
          {
            NWScript.SendMessageToPC(player.oid, "Vous êtes trop éloigné du bloc ciblé, ou alors celui-ci n'existe plus.");
            return;
          }

          int miningYield = 50;

          // TODO : Idée pour plus tard, le strip miner le plus avancé pourra équipper un cristal
          // de spécialisation pour extraire deux fois plus de minerai en un cycle sur son minerai de spécialité
          if (NWScript.GetIsObjectValid(oExtractor) != 1) return;
          
          miningYield += NWScript.GetLocalInt(oExtractor, "_ITEM_LEVEL") * 50;
          int bonusYield = 0;

          int value;
          if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Miner)), out value))
          {
            bonusYield += miningYield * value * 5 / 100;
          }

          if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Geology)), out value))
          {
            bonusYield += miningYield * value * 5 / 100;
          }

          miningYield += bonusYield;

          int remainingOre = NWScript.GetLocalInt(oPlaceable, "_ORE_AMOUNT") - miningYield;
          if (remainingOre <= 0)
          {
            miningYield = NWScript.GetLocalInt(oPlaceable, "_ORE_AMOUNT");
            NWScript.DestroyObject(oPlaceable);

            var newRessourcePoint = NWScript.CreateObject(NWScript.OBJECT_TYPE_WAYPOINT, "NW_WAYPOINT001", NWScript.GetLocation(oPlaceable));
            NWScript.SetLocalString(newRessourcePoint, "_RESSOURCE_TYPE", NWScript.GetName(oPlaceable));
            NWScript.SetTag(newRessourcePoint, "ressourcepoint");
          }
          else
          {
            NWScript.SetLocalInt(oPlaceable, "_ORE_AMOUNT", remainingOre);
          }

          var ore = NWScript.CreateItemOnObject("ore", player.oid, miningYield, NWScript.GetName(oPlaceable));
          NWScript.SetName(ore, NWScript.GetName(oPlaceable));

          ItemSystem.DecreaseItemDurability(oExtractor);
        };

        player.DoActionOnMiningCycleCancelled();
        CollectSystem.StartMiningCycle(player, oPlaceable, cancelCycle, completeCycle);
      };

      player.SelectTarget(callback);
    }
  }
}
