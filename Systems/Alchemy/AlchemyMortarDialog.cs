using System;
using System.Collections.Generic;

using Anvil.API;

using static NWN.Systems.Craft.Collect.Config;

namespace NWN.Systems.Alchemy
{
  class AlchemyMortarDialog
  {
    readonly PlayerSystem.Player player;

    public AlchemyMortarDialog(PlayerSystem.Player player)
    {
      this.player = player;
      this.DrawWelcomePage();
    }
    private void DrawWelcomePage()
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"Voici votre stock d'ingrédients d'alchimie.",
        "Souhaitez-vous en réduire certains en poudre ?"
      };

      /*foreach(KeyValuePair<string, int> materialEntry in player.materialStock)
      {
        if (Enum.TryParse(materialEntry.Key, out PlantType myOreType) && myOreType != PlantType.Invalid)
          player.menu.choices.Add(($"{materialEntry.Key} - {materialEntry.Value} unité(s).", () => HandleRefineOreQuantity(materialEntry.Key)));
      }*/

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    /*private async void HandleRefineOreQuantity(string oreName)
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string> {
        $"Quelle quantité de {oreName} souhaitez-vous réduire en poudre parmi vos {player.materialStock[oreName]} disponibles ?",
        "(Prononcez simplement la quantité à l'oral.)"
      };


      player.menu.choices.Add(("Tout réduire en poudre.", () => HandleRefineAll(oreName)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        HandleRefineOre(oreName);
        player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Delete();
      }
    }*/
    private void HandleRefineOre(string oreName)
    {
      player.menu.Clear();
      int input = int.Parse(player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT"));

      if (input < 1)
      {
        player.menu.titleLines = new List<string> {
          $"La valeur saisie est invalide.",
          "Souhaitez-vous utiliser tout votre stock ?"
        };

        player.menu.choices.Add(("Valider.", () => HandleRefineAll(oreName)));
      }
      else
      {
        /*if (input > player.materialStock[oreName])
          input = player.materialStock[oreName];

        player.materialStock[oreName] -= input;

        int powderQuantity = 0;*/

        /*if (player.learntCustomFeats.ContainsKey(CustomFeats.AlchemistEfficiency))
          powderQuantity += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.AlchemistEfficiency, player.learntCustomFeats[CustomFeats.AlchemistEfficiency]);
        else
        {
          player.menu.Close();
          player.oid.SendServerMessage($"Vos connaissances en alchimie sont insuffisantes pour produire une poudre utilisable à partir de cet ingrédient. Essayez d'apprendre le talent {"Alchimiste économe".ColorString(ColorConstants.White)}.", ColorConstants.Red);
          return;
        }*/

        /*if (Enum.TryParse(oreName, out PlantType myPlantType) && plantDictionnary.TryGetValue(myPlantType, out Plant processedPlant))
        {
          if (player.materialStock.ContainsKey("PoudreDe" + myPlantType.ToString()))
            player.materialStock["PoudreDe" + myPlantType.ToString()] += powderQuantity;
          else
            player.materialStock.Add("PoudreDe" + myPlantType.ToString(), powderQuantity);

          player.oid.SendServerMessage($"Vous parvenez à produire {powderQuantity.ToString().ColorString(ColorConstants.White)} unité(s) de poudre de {processedPlant.name.ColorString(ColorConstants.White)}. Les poudres sont en cours d'acheminement vers votre entrepôt.", new Color(32, 255, 32));
          player.menu.titleLines.Add($"Voilà qui est fait !");
        }
        else
        {
          player.menu.titleLines.Add($"HRP - Erreur, votre ingrédient d'alchimie n'a pas correctement été reconnu. Le staff a été informé du problème.");
          Utils.LogMessageToDMs($"ALCHEMY MORTAR - Could not recognize plant type : {oreName} - Used by : {player.oid.LoginCreature.Name}");
        }*/
      }

      //player.menu.choices.Add(("Retour.", () => DrawWelcomePage(player)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleRefineAll(string oreName)
    {
      //player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Value = player.materialStock[oreName].ToString();
      HandleRefineOre(oreName);
    }
  }
}
