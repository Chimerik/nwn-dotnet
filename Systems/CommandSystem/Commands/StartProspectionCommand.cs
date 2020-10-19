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
            if (NWScript.GetTag(oPlaceable) == "fissurerocheuse")
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
                    var ressourcePoint = NWScript.GetNearestObjectByTag("ressourcepoint", oPlaceable, 1);
                    int i = 2;
                    int skillBonus = 0;
                    int value;
                    if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, 1171)), out value)) // feat geology // TODO : enum 
                    {
                      skillBonus += value;
                    }

                    if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, 1172)), out value)) // feat Mining prospection // TODO : enum 
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

                    var encounter = NWScript.GetNearestObject(NWScript.OBJECT_TYPE_ENCOUNTER, oPlaceable); // TODO : ne pas utiliser des encounters, mais des waypoints !

                    if(NWScript.GetIsObjectValid(encounter) == 1)
                    {
                      for(int creatureIndex = 0; creatureIndex < EncounterPlugin.GetNumberOfCreaturesInEncounterList(encounter); creatureIndex++)
                      {
                        NWScript.CreateObject(NWScript.OBJECT_TYPE_CREATURE, 
                          EncounterPlugin.GetEncounterCreatureByIndex(encounter, creatureIndex).resref,
                          EncounterPlugin.GetSpawnPointByIndex(encounter, 0));
                      }
                    }
                  }
                  else
                  {
                    NWScript.SendMessageToPC(player.oid, "Vous êtes trop éloigné de la veine ciblée, ou alors celle-ci n'existe plus.");
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
    }
  }
}
