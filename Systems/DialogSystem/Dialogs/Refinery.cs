using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.Craft.Collect.Config;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class Refinery
  {
    public Refinery(Player player)
    {
      this.DrawWelcomePage(player);
    }
    private void DrawWelcomePage(Player player)
    {
      player.setValue = 0;
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"Fonderie - Le minerai brut est acheminé de votre entrepôt.",
        "Efficacité : -35 %. Que souhaitez-vous fondre ?"
      };

      foreach (KeyValuePair<string, int> materialEntry in player.materialStock)
      {
        if(materialEntry.Value > 100 && Enum.TryParse(materialEntry.Key, out OreType myOreType) && myOreType != OreType.Invalid)
          player.menu.choices.Add(($"{materialEntry.Key} - {materialEntry.Value} unité(s).", () => HandleRefineOreQuantity(player, materialEntry.Key)));
      }

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleRefineOreQuantity(Player player, string oreName)
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string> {
        $"Quelle quantité de {oreName} souhaitez-vous fondre parmi vos {player.materialStock[oreName]} disponibles ?",
        "(Prononcez simplement la quantité à l'oral.)"
      };

      player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Value = 1;

      Task playerInput = NwTask.Run(async () =>
      {
        await NwTask.WaitUntil(() => player.oid.GetLocalVariable<int>("_PLAYER_INPUT").HasValue);
        if (player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Value != Config.invalidInput)
          HandleRefineOre(player, oreName);
        else
          player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Delete();
      });

      player.setValue = 0;
      player.menu.choices.Add(("Fondre tout le stock.", () => HandleRefineAll(player, oreName)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleRefineOre(Player player, string oreName)
    {
      player.menu.Clear();

      if (player.setValue < 100)
      {
        player.menu.titleLines = new List<string> {
          $"Les ouvriers chargés du transfert ne se dérangeant pas pour moins de 100 unités.",
          "Souhaitez-vous fondre tout votre stock ?"
        };
        player.menu.choices.Add(("Valider.", () => HandleRefineOre(player, oreName)));
        player.setValue = player.materialStock[oreName];
      }
      else
      {
        if (player.setValue > player.materialStock[oreName])
          player.setValue = player.materialStock[oreName];

        player.materialStock[oreName] -= player.setValue;

        float reprocessingEfficiency = 0.3f;

        float value;
        if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Reprocessing)), out value))
          reprocessingEfficiency += reprocessingEfficiency + 3 * value / 100;

        if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.ReprocessingEfficiency)), out value))
          reprocessingEfficiency += reprocessingEfficiency + 2 * value / 100;

        if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Connections)), out value))
          reprocessingEfficiency += reprocessingEfficiency + 1 * value / 100;

        if (Enum.TryParse(oreName, out OreType myOreType) && oresDictionnary.TryGetValue(myOreType, out Ore processedOre))
        {
          if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)processedOre.feat)), out value))
            reprocessingEfficiency += reprocessingEfficiency + 2 * value / 100;

          foreach (KeyValuePair<MineralType, float> mineralKeyValuePair in processedOre.mineralsDictionnary)
          {
            int refinedMinerals = Convert.ToInt32(player.setValue * mineralKeyValuePair.Value * reprocessingEfficiency);
            string mineralName = Enum.GetName(typeof(MineralType), mineralKeyValuePair.Key);

            if (player.materialStock.ContainsKey(mineralName))
              player.materialStock[mineralName] += refinedMinerals;
            else
              player.materialStock.Add(mineralName, refinedMinerals);

            player.oid.SendServerMessage($"Vous venez de raffiner {refinedMinerals} unités de {mineralName}. Les lingots sont en cours d'acheminage vers votre entrepôt.");
          }

          player.menu.titleLines.Add($"Voilà qui est fait !");
        }
        else
        {
          player.menu.titleLines.Add($"HRP - Erreur, votre minerai brut n'a pas correctement été reconnu. Le staff a été informé du problème.");
          NWN.Utils.LogMessageToDMs($"REFINERY - Could not recognize ore type : {oreName} - Used by : {player.oid.Name}");
        }
      }

      //player.menu.choices.Add(("Retour.", () => DrawWelcomePage(player)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleRefineAll(Player player, string oreName)
    {
      player.menu.Clear();

      float reprocessingEfficiency = 0.3f;

      float value;
      if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Reprocessing)), out value))
        reprocessingEfficiency += reprocessingEfficiency + 3 * value / 100;

      if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.ReprocessingEfficiency)), out value))
        reprocessingEfficiency += reprocessingEfficiency + 2 * value / 100;

      if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Connections)), out value))
        reprocessingEfficiency += reprocessingEfficiency + 1 * value / 100;

      if (Enum.TryParse(oreName, out OreType myOreType) && oresDictionnary.TryGetValue(myOreType, out Ore processedOre))
      {
        if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)processedOre.feat)), out value))
          reprocessingEfficiency += reprocessingEfficiency + 2 * value / 100;

        foreach (KeyValuePair<MineralType, float> mineralKeyValuePair in processedOre.mineralsDictionnary)
        {
          int refinedMinerals = Convert.ToInt32(player.materialStock[oreName] * mineralKeyValuePair.Value * reprocessingEfficiency);
          string mineralName = Enum.GetName(typeof(MineralType), mineralKeyValuePair.Key);

          if (player.materialStock.ContainsKey(mineralName))
            player.materialStock[mineralName] += refinedMinerals;
          else
            player.materialStock.Add(mineralName, refinedMinerals);

          player.oid.SendServerMessage($"Vous venez de raffiner {refinedMinerals} unités de {mineralName}. Les lingots sont en cours d'acheminage vers votre entrepôt.");
        }

        player.materialStock[oreName] = 0;

        player.menu.titleLines.Add($"Voilà qui est fait !");
      }
      else
      {
        player.menu.titleLines.Add($"HRP - Erreur, votre minerai brut n'a pas correctement été reconnu. Le staff a été informé du problème.");
        Utils.LogMessageToDMs($"REFINERY - Could not recognize ore type : {oreName} - Used by : {player.oid.Name}");
      }

      //player.menu.choices.Add(("Retour.", () => DrawWelcomePage(player)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
  }
}
