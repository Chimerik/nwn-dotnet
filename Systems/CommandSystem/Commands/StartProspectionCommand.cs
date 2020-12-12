using System;
using System.Numerics;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteStartProspectionCommand(ChatSystem.Context ctx, Options.Result options)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out player))
      {
          Action<uint, Vector3> callback = (uint target, Vector3 position) =>
          {
            var oPlaceable = target;
            uint oExtractor = NWScript.GetItemInSlot(NWScript.INVENTORY_SLOT_RIGHTHAND, player.oid);
            if (NWScript.GetBaseItemType(oExtractor) == 115) // 115 = extracteur
            {
              if (NWScript.GetTag(oPlaceable) == "fissurerocheuse")
              {
                if (NWScript.GetDistanceBetween(player.oid, oPlaceable) < 5.0f)
                {
                  Action cancelCycle = () =>
                  {
                    NWScript.SendMessageToPC(NWScript.GetFirstPC(), "");
                    Utils.RemoveTaggedEffect(oPlaceable, $"_{NWScript.GetPCPublicCDKey(player.oid)}_MINING_BEAM");
                    CollectSystem.RemoveMiningCycleCallbacks(player);   // supprimer la callback de CompleteMiningCycle
                };

                  Action completeCycle = () =>
                  {
                    NWScript.SendMessageToPC(NWScript.GetFirstPC(), "");
                    Utils.RemoveTaggedEffect(oPlaceable, $"_{NWScript.GetPCPublicCDKey(player.oid)}_MINING_BEAM");
                    CollectSystem.RemoveMiningCycleCallbacks(player);   // supprimer la callback de Cancel MiningCycle

                    if (Convert.ToBoolean(NWScript.GetIsObjectValid(oPlaceable)) && NWScript.GetDistanceBetween(player.oid, oPlaceable) >= 5.0f)
                    {
                      var ressourcePoint = NWScript.GetNearestObjectByTag("ressourcepoint", oPlaceable, 1);
                      int i = 2;
                      int skillBonus = 0;
                      int value;
                      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Geology)), out value))
                      {
                        skillBonus += value;
                      }

                      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Prospection)), out value))
                      {
                        skillBonus += value;
                      }

                      int respawnChance = skillBonus * 5;

                      while (NWScript.GetIsObjectValid(ressourcePoint) == 1)
                      {
                        if (NWScript.GetDistanceBetween(oPlaceable, ressourcePoint) > 20.0f)
                          break;

                        string ressourceType = NWScript.GetLocalString(ressourcePoint, "_RESSOURCE_TYPE");

                        int iRandom = Utils.random.Next(1, 101);

                        if (iRandom < respawnChance)
                        {
                          var newRock = NWScript.CreateObject(NWScript.OBJECT_TYPE_PLACEABLE, "mineable_rock", NWScript.GetLocation(ressourcePoint));
                          NWScript.SetName(newRock, ressourceType);
                          NWScript.SetLocalInt(newRock, "_ORE_AMOUNT", 200 * iRandom + 200 * iRandom * skillBonus / 100);
                          NWScript.DestroyObject(oPlaceable);
                        }

                        ressourcePoint = NWScript.GetNearestObjectByTag("ressourcepoint", oPlaceable, i);
                        i++;
                      }
                    }
                    else
                    {
                      NWScript.SendMessageToPC(player.oid, "Vous êtes trop éloigné de la veine ciblée, ou alors celle-ci n'existe plus.");
                    }
                  };

                  int i = 1;
                  uint creatureSpawn = NWScript.GetNearestObjectByTag("disturbed_creature_spawn", oPlaceable);

                  while (Convert.ToBoolean(NWScript.GetIsObjectValid(creatureSpawn)) && NWScript.GetDistanceBetween(oPlaceable, creatureSpawn) < 50.0f)
                  {
                    NWScript.CreateObject(NWScript.OBJECT_TYPE_CREATURE, NWScript.GetLocalString(creatureSpawn, "_CREATURE_TEMPLATE"), NWScript.GetLocation(creatureSpawn));
                    i++;
                    creatureSpawn = NWScript.GetNearestObjectByTag("disturbed_creature_spawn", oPlaceable, i);
                  }

                  ItemSystem.DecreaseItemDurability(oExtractor);

                  player.DoActionOnMiningCycleCancelled();
                  CollectSystem.StartMiningCycle(player, oPlaceable, cancelCycle, completeCycle);
                }
                else
                  NWScript.SendMessageToPC(player.oid, $"{NWScript.GetName(oPlaceable)} n'est pas à portée. Rapprochez-vous pour démarrer la prospection.");
              }
              else
                NWScript.SendMessageToPC(player.oid, $"{NWScript.GetName(oPlaceable)} n'est pas une veine de minerai. Impossible de démarrer la prospection.");
            }
            else
              NWScript.SendMessageToPC(player.oid, $"Il vous faut vous munir d'un extracteur pour démarrer la prospection !");
          };

          player.SelectTarget(callback);
      }
    }
  }
}
