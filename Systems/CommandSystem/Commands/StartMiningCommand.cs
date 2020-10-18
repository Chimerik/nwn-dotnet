using System;
using NWN.Enums;
using NWN.Enums.VisualEffect;
using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteStartMiningCommand(ChatSystem.Context ctx, Options.Result options)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out player))
      {
        if (player.Equipped[InventorySlot.RightHand].Tag == "extracteur")
        {
          Action<uint, Vector> callback = (uint target, Vector position) =>
          {
            NWPlaceable oPlaceable = target.AsPlaceable();
            if (oPlaceable.Tag == "mineable_rock")
            {
              if (NWScript.GetDistanceBetween(player, oPlaceable) < 5.0f)
              {
                Action cancelCycle = () =>
                {
                  player.SendMessage("Cycle cancelled");
                  oPlaceable.RemoveTaggedEffect($"_{player.CDKey}_MINING_BEAM");
                  CollectSystem.RemoveMiningCycleCallbacks(player);   // supprimer la callback de CompleteMiningCycle
              };

                Action completeCycle = () =>
                {
                  player.SendMessage("Entering Cycle completed callback");
                  oPlaceable.RemoveTaggedEffect($"_{player.CDKey}_MINING_BEAM");
                  CollectSystem.RemoveMiningCycleCallbacks(player);   // supprimer la callback de Cancel MiningCycle

                  if (oPlaceable.IsValid && NWScript.GetDistanceBetween(player, oPlaceable) >= 5.0f)
                  {
                    NWItem miningStriper = player.Equipped[InventorySlot.RightHand];
                    int miningYield = 0;

                    if (miningStriper.IsValid) // TODO : Idée pour plus tard, le strip miner le plus avancé pourra équipper un cristal de spécialisation pour extraire deux fois plus de minerai en un cycle sur son minerai de spécialité
                    {
                      miningYield = miningStriper.Locals.Int.Get("_ITEM_LEVEL") * 50;
                      int bonusYield = 0;

                      int value;
                      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", NWNX.Creature.GetHighestLevelOfFeat(player, (int)Feat.Miner)), out value))
                      {
                        bonusYield += miningYield * value * 5 / 100;
                      }

                      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", NWNX.Creature.GetHighestLevelOfFeat(player, (int)Feat.Geology)), out value))
                      {
                        bonusYield += miningYield * value * 5 / 100;
                      }

                      miningYield += bonusYield;

                      int remainingOre = oPlaceable.Locals.Int.Get("_ORE_AMOUNT") - miningYield;
                      if (remainingOre <= 0)
                      {
                        miningYield = oPlaceable.Locals.Int.Get("_ORE_AMOUNT");
                        oPlaceable.Destroy();

                        NWObject newRessourcePoint = NWScript.CreateObject(ObjectType.Waypoint, "NW_WAYPOINT001", oPlaceable.Location).AsObject();
                        newRessourcePoint.Locals.String.Set("_RESSOURCE_TYPE", oPlaceable.Name);
                        newRessourcePoint.Tag = "ressourcepoint";
                      }
                      else
                      {
                        oPlaceable.Locals.Int.Set("_ORE_AMOUNT", remainingOre);
                      }

                      player.SendMessage($"Mining yield = {miningYield}");
                      NWItem ore = NWScript.CreateItemOnObject("ore", player, miningYield).AsItem();
                      ore.Name = oPlaceable.Name;

                      int stripperDurability = miningStriper.Locals.Int.Get("_DURABILITY");
                      if (stripperDurability <= 1)
                        miningStriper.Destroy();
                      else
                        miningStriper.Locals.Int.Set("_DURABILITY", stripperDurability - 1);
                    }
                  }
                  else
                  {
                    player.SendMessage("Vous êtes trop éloigné du bloc ciblé, ou alors celui-ci n'existe plus.");
                  }
                };

                CollectSystem.StartMiningCycle(player, oPlaceable, cancelCycle, completeCycle);
              }
              else
                player.SendMessage($"{oPlaceable.Name} n'est pas à portée. Rapprochez-vous pour démarrer l'extraction.");
            }
            else
              player.SendMessage($"{oPlaceable.Name} n'est pas un filon de minerai. Impossible de démarrer l'extraction.");
          };

          player.SelectTarget(callback);
        }
        else
          player.SendMessage($"Il vous faut vous munir d'un extracteur pour démarrer l'extraction !");
      }
    }
  }
}
