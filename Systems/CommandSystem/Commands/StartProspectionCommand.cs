using System;
using NWN.Enums;
using NWN.Enums.VisualEffect;
using NWN.NWNX;
using NWN.NWNX.Enum;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteStartProspectionCommand(ChatSystem.Context ctx, Options.Result options)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out player))
      {
          Action<uint, Vector> callback = (uint target, Vector position) =>
          {
            NWPlaceable oPlaceable = target.AsPlaceable();
            if (oPlaceable.Tag == "fissurerocheuse")
            {
              if (NWScript.GetDistanceBetween(player, oPlaceable) < 5.0f)
              {
                Action cancelCycle = () =>
                {
                  player.SendMessage("Cycle cancelled");
                  oPlaceable.RemoveTaggedEffect($"_{player.CDKey}_MINING_BEAM");
                  player.RemoveMiningCycleCallbacks();   // supprimer la callback de CompleteMiningCycle
                };

                Action completeCycle = () =>
                {
                  player.SendMessage("Entering Cycle completed callback");
                  oPlaceable.RemoveTaggedEffect($"_{player.CDKey}_MINING_BEAM");
                  player.RemoveMiningCycleCallbacks();   // supprimer la callback de Cancel MiningCycle

                  if (oPlaceable.IsValid && NWScript.GetDistanceBetween(player, oPlaceable) >= 5.0f)
                  {
                    NWPlaceable ressourcePoint = NWScript.GetNearestObjectByTag("ressourcepoint", oPlaceable, 1).AsPlaceable();
                    int i = 2;
                    int skillBonus = 0;
                    int value;
                    if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", NWNX.Creature.GetHighestLevelOfFeat(player, (int)Feat.Geology)), out value))
                    {
                      skillBonus += value;
                    }

                    if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", NWNX.Creature.GetHighestLevelOfFeat(player, (int)Feat.MiningProspection)), out value))
                    {
                      skillBonus += value;
                    }

                    int respawnChance = skillBonus * 5;

                    while (ressourcePoint.IsValid)
                    {
                      if (NWScript.GetDistanceBetween(oPlaceable, ressourcePoint) > 20.0f)
                        break;

                      string ressourceType = ressourcePoint.Locals.String.Get("_RESSOURCE_TYPE");

                      int iRandom = Utils.random.Next(1, 101);

                      if (iRandom < respawnChance)
                      {
                        NWPlaceable newRock = NWScript.CreateObject(ObjectType.Placeable, "mineable_rock", ressourcePoint.Location).AsPlaceable();
                        newRock.Name = ressourceType;
                        newRock.Locals.Int.Set("_ORE_AMOUNT", 200 * iRandom + 200 * iRandom * skillBonus / 100);
                        oPlaceable.Destroy();
                      }

                      ressourcePoint = NWScript.GetNearestObjectByTag("ressourcepoint", oPlaceable, i).AsPlaceable();
                      i++;
                    }

                    NWObject encounter = NWScript.GetNearestObject(oPlaceable, ObjectType.Encounter).AsObject();

                    if(encounter.IsValid)
                    {
                      for(int creatureIndex = 0; creatureIndex < NWNX.Encounter.GetNumberOfCreaturesInEncounterList(encounter); creatureIndex++)
                      {
                        NWScript.CreateObject(ObjectType.Creature, 
                          NWNX.Encounter.GetEncounterCreatureByIndex(encounter, creatureIndex).resref,
                          player.Location); // TODO : une fois la MAJ .net core effectuée, utiliser le spawn point de l'encounter exposé par NWNX
                      }
                    }
                  }
                  else
                  {
                    player.SendMessage("Vous êtes trop éloigné de la veine ciblée, ou alors celle-ci n'existe plus.");
                  }
                };

                player.StartMiningCycle(oPlaceable, cancelCycle, completeCycle);
              }
              else
                player.SendMessage($"{oPlaceable.Name} n'est pas à portée. Rapprochez-vous pour démarrer l'extraction.");
            }
            else
              player.SendMessage($"{oPlaceable.Name} n'est pas un filon de minerai. Impossible de démarrer l'extraction.");
          };

          player.SelectTarget(callback);
      }
    }
  }
}
