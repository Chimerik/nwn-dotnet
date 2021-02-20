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
  class Tannerie
  {
    public Tannerie(Player player)
    {
      this.DrawWelcomePage(player);
    }
    private void DrawWelcomePage(Player player)
    {
      player.setValue = 0;
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"Tannerie - Les peaux brutes sont acheminées de votre entrepôt.",
        "Efficacité : -35 %. Que souhaitez-vous transformer en cuir ?",
        "(Utilisez la commande !set X avant de valider votre choix)"
      };

      foreach (KeyValuePair<string, int> materialEntry in player.materialStock)
      {
        if (materialEntry.Value > 100 && Enum.TryParse(materialEntry.Key, out PeltType myOreType) && myOreType != PeltType.Invalid)
          player.menu.choices.Add(($"{myOreType.ToDescription()} - {materialEntry.Value} unité(s).", () => HandleRefineOreQuantity(player, materialEntry.Key)));
      }

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleRefineOreQuantity(Player player, string oreName)
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string> {
        $"Quelle quantité de {oreName} souhaitez-vous traiter parmi vos {player.materialStock[oreName]} disponibles ?",
        "(Prononcez simplement la quantité à l'oral. Dites 0 si vous souhaitez tout traiter.)"
      };

      player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Value = 1;

      Task playerInput = NwTask.Run(async () =>
      {
        await NwTask.WaitUntilValueChanged(() => player.oid.GetLocalVariable<int>("_PLAYER_INPUT").HasValue);
        if (player.oid.GetLocalVariable<int>("_PLAYER_INPUT_CANCELLED").HasNothing)
          HandleRefineOre(player, oreName);
        else
          player.oid.GetLocalVariable<int>("_PLAYER_INPUT_CANCELLED").Delete();
      });

      player.menu.choices.Add(("Tout tanner.", () => HandleRefineAll(player, oreName)));
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
          "Souhaitez-vous utiliser tout votre stock ?"
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
        if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.PeltReprocessing)), out value))
          reprocessingEfficiency += reprocessingEfficiency + 3 * value / 100;

        if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.PeltReprocessingEfficiency)), out value))
          reprocessingEfficiency += reprocessingEfficiency + 2 * value / 100;

        if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Connections)), out value))
          reprocessingEfficiency += reprocessingEfficiency + 1 * value / 100;

        if (Enum.TryParse(oreName, out PeltType myOreType) && peltDictionnary.TryGetValue(myOreType, out Pelt processedOre))
        {
          if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)processedOre.feat)), out value))
            reprocessingEfficiency += reprocessingEfficiency + 2 * value / 100;

          int refinedMinerals = Convert.ToInt32(player.setValue * processedOre.leathers * reprocessingEfficiency);
          string mineralName = Enum.GetName(typeof(LeatherType), processedOre.refinedType) ?? "";

          if (player.materialStock.ContainsKey(mineralName))
            player.materialStock[mineralName] += refinedMinerals;
          else
            player.materialStock.Add(mineralName, refinedMinerals);

          player.oid.SendServerMessage($"Vous venez de tanner {refinedMinerals} peaux de {mineralName}. Les cuirs sont en cours d'acheminage vers votre entrepôt.");

          player.menu.titleLines.Add($"Voilà qui est fait !");
        }
        else
        {
          player.menu.titleLines.Add($"HRP - Erreur, votre peau brut n'a pas correctement été reconnue. Le staff a été informé du problème.");
          NWN.Utils.LogMessageToDMs($"TANNERIE - Could not recognize pelt type : {oreName} - Used by : {player.oid.Name}");
        }
      }

      player.menu.choices.Add(("Retour.", () => DrawWelcomePage(player)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleRefineAll(Player player, string oreName)
    {
      player.menu.Clear();

        float reprocessingEfficiency = 0.3f;

        float value;
        if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.PeltReprocessing)), out value))
          reprocessingEfficiency += reprocessingEfficiency + 3 * value / 100;

        if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.PeltReprocessingEfficiency)), out value))
          reprocessingEfficiency += reprocessingEfficiency + 2 * value / 100;

        if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Connections)), out value))
          reprocessingEfficiency += reprocessingEfficiency + 1 * value / 100;

      if (Enum.TryParse(oreName, out PeltType myOreType) && peltDictionnary.TryGetValue(myOreType, out Pelt processedOre))
      {
        if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)processedOre.feat)), out value))
          reprocessingEfficiency += reprocessingEfficiency + 2 * value / 100;

        int refinedMinerals = Convert.ToInt32(player.materialStock[oreName] * processedOre.leathers * reprocessingEfficiency);
        string mineralName = Enum.GetName(typeof(LeatherType), processedOre.refinedType) ?? "";

        if (player.materialStock.ContainsKey(mineralName))
          player.materialStock[mineralName] += refinedMinerals;
        else
          player.materialStock.Add(mineralName, refinedMinerals);

        player.oid.SendServerMessage($"Vous venez de tanner {refinedMinerals} peaux de {mineralName}. Les cuirs sont en cours d'acheminage vers votre entrepôt.");
        player.materialStock[oreName] = 0;
        player.menu.titleLines.Add($"Voilà qui est fait !");
      }
      else
      {
        player.menu.titleLines.Add($"HRP - Erreur, votre peau brut n'a pas correctement été reconnue. Le staff a été informé du problème.");
        NWN.Utils.LogMessageToDMs($"TANNERIE - Could not recognize pelt type : {oreName} - Used by : {player.oid.Name}");
      }

      player.menu.choices.Add(("Retour.", () => DrawWelcomePage(player)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
  }
}
