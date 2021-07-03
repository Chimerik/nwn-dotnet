using System.Collections.Generic;
using System.Threading.Tasks;
using NWN.API;
using static NWN.Systems.PlayerSystem;
using System;
using NWN.Core;
using System.Linq;

namespace NWN.Systems
{
  class PrivateContractCreator
  {
    public class Contract
    {
      public int quantity { get; set; }
      public int unitPrice { get; set; }
      public Contract(int quantity, int price)
      {
        this.quantity = quantity;
        this.unitPrice = price;
      }
    }
    private Dictionary<string, Contract> materialContractDictionnary { get; set; }
    public PrivateContractCreator(Player player)
    {
      materialContractDictionnary = new Dictionary<string, Contract>();
      DrawMainContractPage(player);
    }
    private void DrawMainContractPage(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Que souhaitez-vous faire ?");

      int contractScienceLevel = 1;
      if(player.learntCustomFeats.ContainsKey(CustomFeats.ContractScience))
        contractScienceLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ContractScience, player.learntCustomFeats[CustomFeats.ContractScience]);

      var result = SqLiteUtils.SelectQuery("playerPrivateContracts",
          new List<string>() { { "count(rowid)" } },
          new List<string[]>() { new string[] { "characterId", player.characterId.ToString() } });

      if (result != null || result.FirstOrDefault().GetInt(0) <= contractScienceLevel)
      {
        player.menu.choices.Add((
        "Rédiger un nouveau contrat",
        () => WriteContractPage(player)
        ));
      }
      player.menu.choices.Add((
        "Consulter mes contrats en attente",
        () => DrawCurrentContractPage(player)
      ));
      player.menu.choices.Add((
        "Quitter",
        () => player.menu.Close()
      ));

      player.menu.Draw();
    }

    private void WriteContractPage(Player player)
    {
      //TODO : ne permettre la rédaction d'un nouveau contrat qu'en fonction du skill de rédaction de contrat + supprimer contrats expirés du calcul
      player.menu.Clear();
      player.menu.titleLines.Add("Quelle ressource souhaitez-vous faire figurer dans ce contrat ?");

      foreach (var entry in player.materialStock)
        if (materialContractDictionnary.ContainsKey(entry.Key))
          player.menu.choices.Add(($"* {entry.Key} : {entry.Value - materialContractDictionnary[entry.Key].quantity}", () => HandleValidateMaterialSelection(player, entry.Key)));
        else
          player.menu.choices.Add(($"* {entry.Key} : {entry.Value}", () => HandleValidateMaterialSelection(player, entry.Key)));

      player.menu.choices.Add((
        "Retour",
        () => DrawMainContractPage(player)
      ));

      player.menu.Draw();
    }
    private async void HandleValidateMaterialSelection(Player player, string material)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
          $"Quelle quantité de {material} souhaitez-vous faire figurer dans ce contrat ?",
          "(Indiquez simplement la valeur à l'oral)"
        };

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        HandleSetupPriceContract(player, material);
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
    private async void HandleSetupPriceContract(Player player, string material)
    {
      player.menu.Clear();
      int input = int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT"));

      if (input <= 0)
      {
        player.menu.titleLines.Add($"La quantité indiquée n'est pas valide, veuillez ré-essayer.");
        player.menu.choices.Add(($"Entrer une nouvelle valeur.", () => HandleValidateMaterialSelection(player, material)));
        player.menu.Draw();
      }
      else
      {
        if (input >= player.materialStock[material])
          input = player.materialStock[material];
        else
        {
          player.menu.titleLines = new List<string> {
          $"{input} de {material}. A quel prix unitaire ?",
          $"(Indiquez à l'oral le prix que vous souhaitez pour chaque unité de {material}"
          };

          player.menu.Draw();

          bool awaitedValue = await player.WaitForPlayerInputInt();

          if (awaitedValue)
          {
            HandleRegisterMaterialToContract(player, material, input);
            player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
          }
        }
      }
    }
    private void HandleRegisterMaterialToContract(Player player, string material, int quantity)
    {
      player.menu.Clear();
      int input = int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT"));

      if (input < 0)
      {
        player.menu.titleLines.Add($"Le prix indiqué n'est pas valide, veuillez ré-essayer.");
        player.menu.choices.Add(($"Entrer une nouvelle valeur.", () => WriteContractPage(player)));
      }
      else
      {
        if (materialContractDictionnary.ContainsKey(material))
        {
          materialContractDictionnary[material].quantity += quantity;
          materialContractDictionnary[material].unitPrice = input;
        }
        else
          materialContractDictionnary.Add(material, new Contract(quantity, input));

        player.menu.titleLines = new List<string>();
        int grandTotal = 0;

        foreach (KeyValuePair<string, Contract> entry in materialContractDictionnary)
        {
          player.menu.titleLines.Add($"Contrat : {entry.Value.quantity} de {entry.Key} à {entry.Value.unitPrice} l'unité. Total : {entry.Value.quantity * entry.Value.unitPrice} pièces d'or.");
          grandTotal += entry.Value.quantity * entry.Value.unitPrice;
        }

        player.menu.titleLines.Add($"Total final : {grandTotal} pièce(s) d'or.");
        player.menu.choices.Add(($"Ajouter une autre ressource au contrat.", () => WriteContractPage(player)));
        player.menu.choices.Add(($"Finaliser la rédaction du contrat.", () => HandleRegisterContractExpirationDate(player)));
      }

      player.menu.Draw();
    }
    private async void HandleRegisterContractExpirationDate(Player player)
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string> {
          $"Veuillez indiquer la date d'expiration de ce contrat.",
          $"(Indiquez à l'oral le nombre de jours, compris entre 1 et 30, pendant lequel ce contrat sera valide)"
      };

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        CreateContractPage(player);
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
    private async void CreateContractPage(Player player)
    {
      int expirationDate = 30;

      int input = int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT"));

      if (input > 0 || input < 31)
        expirationDate = input;

      if (materialContractDictionnary.Count < 1)
      {
        player.oid.SendServerMessage("Le contrat est vide. Veuillez y faire figurer des ressources avant de le valider.", ColorConstants.Orange);
        player.menu.Close();
        return;
      }

      NwItem contract = await NwItem.Create("skillbookgeneriq", player.oid.LoginCreature, 1, "private_contract");

      int grandTotal = 0;
      string serializedContract = "";
      contract.Description = "";

      foreach (KeyValuePair<string, Contract> entry in materialContractDictionnary)
      {
        contract.Description += $"Contrat : {entry.Value.quantity} de {entry.Key} à {entry.Value.unitPrice} l'unité. Total : {entry.Value.quantity * entry.Value.unitPrice} pièces d'or.\n";
        grandTotal += entry.Value.quantity * entry.Value.unitPrice;
        serializedContract += $"{entry.Key}${entry.Value.quantity}${entry.Value.unitPrice}|";

        player.materialStock[entry.Key] -= entry.Value.quantity;
      }

      contract.Description += $"\n\nTotal final : {grandTotal} pièce(s) d'or.\n\n Expiration le : {DateTime.Now.AddDays(expirationDate)}";
      contract.GetLocalVariable<int>("_CONTRACT_CREATOR_ID").Value = player.characterId;
      contract.GetLocalVariable<int>("_CONTRACT_TOTAL_GOLD_PRICE").Value = grandTotal;
      contract.GetLocalVariable<string>("_CONTRACT_EXPIRATION_DATE").Value = DateTime.Now.AddDays(expirationDate).ToString();
      contract.GetLocalVariable<string>("_SERIALIZED_CONTRACT_DATA").Value = serializedContract;

      SqLiteUtils.InsertQuery("playerPrivateContracts",
          new List<string[]>() {
            new string[] { "characterId", player.oid.PlayerName },
            new string[] { "expirationDate", DateTime.Now.AddDays(expirationDate).ToString() },
            new string[] { "serializedContract", serializedContract },
            new string[] { "serializedContract", grandTotal.ToString() } });

      var query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, $"SELECT last_insert_rowid()");
      query.Execute();

      contract.GetLocalVariable<int>("_CONTRACT_ID").Value = query.Result.GetInt(0);
      contract.Name = $"Contrat {query.Result.GetInt(0)} de {player.oid.LoginCreature.Name}";

      player.oid.SendServerMessage("Votre contrat d'échange privé de ressources a bien été créé.", ColorConstants.Pink);
      player.oid.LoginCreature.AcquireItem(contract, true);
      player.menu.Close();
    }
    private void DrawCurrentContractPage(Player player)
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string> {
          $"Voici la liste de vos contrats privés actifs.",
          $"Lequel souhaitez-vous consulter ?"
      };

      var result = SqLiteUtils.SelectQuery("playerPrivateContracts",
          new List<string>() { { "expirationDate" }, { "totalValue" }, { "rowid" } },
          new List<string[]>() { new string[] { "characterId", player.characterId.ToString() } });

      if(result != null)
      foreach(var contract in result)
      {
        int contractId = contract.GetInt(2);

        TimeSpan remainingTime = (DateTime.Parse(contract.GetString(0)) - DateTime.Now);
        if(remainingTime.TotalMinutes > 0)
          player.menu.choices.Add(($"{contractId} - Expire dans : {remainingTime.Days}:{remainingTime.Hours}:{remainingTime.Minutes}:{remainingTime.Seconds} - Valeur totale : {contract.GetInt(1)}", () => HandleContractSelection(player, contractId)));
        else
        {
          Task contractExpiration = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.2));
            DeleteExpiredContract(player, contractId);
          });
        }
      }

      player.menu.choices.Add((
        "Retour",
        () => DrawMainContractPage(player)
      ));

      player.menu.Draw();
    }
    private void HandleContractSelection(Player player, int contractId)
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string> {
          $"Contenu du contrat {contractId} - Souhaitez-vous l'annuler ce contrat ?"
      };

      player.menu.choices.Add(($"Oui, le contrat {contractId} est nul et non avenu.", () => HandleContractCancellation(player, contractId)));
      player.menu.choices.Add(($"Non, retour à la liste des contrats.", () => DrawCurrentContractPage(player)));

      var result = SqLiteUtils.SelectQuery("playerPrivateContracts",
        new List<string>() { { "serializedContract" }, { "expirationDate" }, { "totalValue" } },
        new List<string[]>() { new string[] { "rowid", contractId.ToString() } });

      if (result != null && result.Count() > 0)
      {
        foreach (string materialString in result.FirstOrDefault().GetString(0).Split("|"))
        {
          string[] descriptionString = materialString.Split("$");
          if (descriptionString.Length == 3)
            player.menu.titleLines.Add($"{descriptionString[0]} : {descriptionString[1]} * {descriptionString[2]} = {Int32.Parse(descriptionString[1]) * Int32.Parse(descriptionString[2])}");
        }

        player.menu.titleLines.Add($"Valeur totale : {result.FirstOrDefault().GetString(2)}");
        player.menu.titleLines.Add($"Date d'expiration : {result.FirstOrDefault().GetString(1)}");
      }

      player.menu.Draw();
    }
    private void HandleContractCancellation(Player player, int contractId)
    {
      DeleteExpiredContract(player, contractId);

      player.oid.SendServerMessage($"Le contrat {contractId} a été annulé.", ColorConstants.Magenta);
      DrawCurrentContractPage(player);
    }
    private void DeleteExpiredContract(Player player, int contractId)
    {
      var result = SqLiteUtils.SelectQuery("playerPrivateContracts",
        new List<string>() { { "serializedContract" } },
        new List<string[]>() { new string[] { "rowid", contractId.ToString() } });

      if (result != null && result.Count() > 0)
      {
        foreach (string materialString in  result.FirstOrDefault().GetString(0).Split("|"))
        {
          string[] descriptionString = materialString.Split("$");
          if (descriptionString.Length == 3)
            if (player.materialStock.ContainsKey(descriptionString[0]))
              player.materialStock[descriptionString[0]] += Int32.Parse(descriptionString[1]);
            else
              player.materialStock.Add(descriptionString[0], Int32.Parse(descriptionString[1]));
        }
      
      SqLiteUtils.DeletionQuery("playerPrivateContracts",
         new Dictionary<string, string>() { { "rowid", contractId.ToString() } });
      }
    }
  }
}
