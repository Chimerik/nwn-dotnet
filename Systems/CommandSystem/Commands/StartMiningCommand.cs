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
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out player))
      {
        if (NWScript.GetTag(NWScript.GetItemInSlot(NWScript.INVENTORY_SLOT_RIGHTHAND, player.oid)) == "extracteur")
        {
          Action<uint, Vector3> callback = (uint target, Vector3 position) =>
          {
            var oPlaceable = target;
            if (NWScript.GetTag(oPlaceable) == "mineable_rock")
            {
              if (NWScript.GetDistanceBetween(player.oid, oPlaceable) < 5.0f)
              {
                Action cancelCycle = () =>
                {
                  NWScript.SendMessageToPC(player.oid, "Cycle cancelled");
                  Utils.RemoveTaggedEffect(oPlaceable, $"_{NWScript.GetPCPublicCDKey(player.oid)}_MINING_BEAM");
                  CollectSystem.RemoveMiningCycleCallbacks(player);   // supprimer la callback de CompleteMiningCycle
              };

                Action completeCycle = () =>
                {
                  NWScript.SendMessageToPC(player.oid, "Entering Cycle completed callback");
                  Utils.RemoveTaggedEffect(oPlaceable, $"_{NWScript.GetPCPublicCDKey(player.oid)}_MINING_BEAM");
                  CollectSystem.RemoveMiningCycleCallbacks(player);   // supprimer la callback de Cancel MiningCycle

                  if (NWScript.GetIsObjectValid(oPlaceable) == 1 && NWScript.GetDistanceBetween(player.oid, oPlaceable) >= 5.0f)
                  {
                    var miningStriper = NWScript.GetItemInSlot(NWScript.INVENTORY_SLOT_RIGHTHAND, player.oid);
                    int miningYield = 0;

                    if (NWScript.GetIsObjectValid(miningStriper) == 1) // TODO : Idée pour plus tard, le strip miner le plus avancé pourra équipper un cristal de spécialisation pour extraire deux fois plus de minerai en un cycle sur son minerai de spécialité
                    {
                      miningYield = NWScript.GetLocalInt(miningStriper, "_ITEM_LEVEL") * 50;
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

                      NWScript.SendMessageToPC(player.oid, $"Mining yield = {miningYield}");
                      var ore = NWScript.CreateItemOnObject("ore", player.oid, miningYield);
                      NWScript.SetName(ore, NWScript.GetName(oPlaceable));
                      NWScript.SetLocalInt(ore, "DROPS_ON_DEATH", 1);

                      int stripperDurability = NWScript.GetLocalInt(miningStriper, "_DURABILITY");
                      if (stripperDurability <= 1)
                        NWScript.DestroyObject(miningStriper);
                      else
                        NWScript.SetLocalInt(miningStriper, "_DURABILITY", stripperDurability - 1);
                    }
                  }
                  else
                  {
                    NWScript.SendMessageToPC(player.oid, "Vous êtes trop éloigné du bloc ciblé, ou alors celui-ci n'existe plus.");
                  }
                };

                CollectSystem.StartMiningCycle(player, oPlaceable, cancelCycle, completeCycle);
              }
              else
                NWScript.SendMessageToPC(player.oid, $"{NWScript.GetName(oPlaceable)} n'est pas à portée. Rapprochez-vous pour démarrer l'extraction.");
            }
            else
              NWScript.SendMessageToPC(player.oid, $"{NWScript.GetName(oPlaceable)} n'est pas un filon de minerai. Impossible de démarrer l'extraction.");
          };

          player.SelectTarget(callback);
        }
        else
          NWScript.SendMessageToPC(player.oid, $"Il vous faut vous munir d'un extracteur pour démarrer l'extraction !");
      }
    }
  }
}
