using System.Collections.Generic;
using System.Threading.Tasks;
using NWN.API;
using static NWN.Systems.PlayerSystem;
using System;
using NWN.Core;

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

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT count(rowid) from playerPrivateContracts where characterId = @characterId");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);

      if (NWScript.SqlStep(query) == 0 || NWScript.SqlGetInt(query, 0) <= contractScienceLevel)
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
    private void HandleValidateMaterialSelection(Player player, string material)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
          $"Quelle quantité de {material} souhaitez-vous faire figurer dans ce contrat ?",
          "(Indiquez simplement la valeur à l'oral)"
        };

      Task playerInput = NwTask.Run(async () =>
      {
        player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Value = 1;
        player.setValue = Config.invalidInput;
        await NwTask.WaitUntil(() => player.setValue != Config.invalidInput);
        HandleSetupPriceContract(player, material);
        player.setValue = Config.invalidInput;
      });

      player.setValue = Config.invalidInput;
      player.menu.Draw();
    }
    private void HandleSetupPriceContract(Player player, string material)
    {
      player.menu.Clear();

      if (player.setValue <= 0)
      {
        player.menu.titleLines.Add($"La quantité indiquée n'est pas valide, veuillez ré-essayer.");
        player.menu.choices.Add(($"Entrer une nouvelle valeur.", () => HandleValidateMaterialSelection(player, material)));
      }
      else
      {
        if (player.setValue >= player.materialStock[material])
          player.setValue = player.materialStock[material];
        else
        {
          player.menu.titleLines = new List<string> {
          $"{player.setValue} de {material}. A quel prix unitaire ?",
          $"(Indiquez à l'oral le prix que vous souhaitez pour chaque unité de {material}"
          };

          Task playerInput = NwTask.Run(async () =>
          {
            int quantity = player.setValue;
            player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Value = 1;
            player.setValue = Config.invalidInput;
            await NwTask.WaitUntil(() => player.setValue != Config.invalidInput);
            HandleRegisterMaterialToContract(player, material, quantity);
            player.setValue = Config.invalidInput;
          });
        }
      }

      player.setValue = Config.invalidInput;
      player.menu.Draw();
    }
    private void HandleRegisterMaterialToContract(Player player, string material, int quantity)
    {
      player.menu.Clear();

      if (player.setValue < 0)
      {
        player.menu.titleLines.Add($"Le prix indiqué n'est pas valide, veuillez ré-essayer.");
        player.menu.choices.Add(($"Entrer une nouvelle valeur.", () => WriteContractPage(player)));
      }
      else
      {
        if (materialContractDictionnary.ContainsKey(material))
        {
          materialContractDictionnary[material].quantity += quantity;
          materialContractDictionnary[material].unitPrice = player.setValue;
        }
        else
          materialContractDictionnary.Add(material, new Contract(quantity, player.setValue));

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

      player.setValue = Config.invalidInput;
      player.menu.Draw();
    }
    private void HandleRegisterContractExpirationDate(Player player)
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string> {
          $"Veuillez indiquer la date d'expiration de ce contrat.",
          $"(Indiquez à l'oral le nombre de jours, compris entre 1 et 30, pendant lequel ce contrat sera valide)"
      };

      Task playerInput = NwTask.Run(async () =>
      {
        player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Value = 1;
        player.setValue = Config.invalidInput;
        await NwTask.WaitUntil(() => player.setValue != Config.invalidInput);
        CreateContractPage(player);
        player.setValue = Config.invalidInput;
      });

      player.setValue = Config.invalidInput;
      player.menu.Draw();
    }
    private void CreateContractPage(Player player)
    {
      int expirationDate = 30;

      if (player.setValue > 0 || player.setValue < 31)
        expirationDate = player.setValue;

      if (materialContractDictionnary.Count < 1)
      {
        player.oid.SendServerMessage("Le contrat est vide. Veuillez y faire figurer des ressources avant de le valider.", Color.ORANGE);
        player.menu.Close();
        return;
      }

      NwItem contract = NwItem.Create("skillbookgeneriq", player.oid, 1, "private_contract");

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

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO playerPrivateContracts (characterId, expirationDate, serializedContract, totalValue) VALUES (@characterId, @expirationDate, @serializedContract, @totalValue)");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);
      NWScript.SqlBindString(query, "@expirationDate", DateTime.Now.AddDays(expirationDate).ToString());
      NWScript.SqlBindString(query, "@serializedContract", serializedContract);
      NWScript.SqlBindInt(query, "@totalValue", grandTotal);
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT last_insert_rowid()");
      NWScript.SqlStep(query);

      contract.GetLocalVariable<int>("_CONTRACT_ID").Value = NWScript.SqlGetInt(query, 0);
      contract.Name = $"Contrat {NWScript.SqlGetInt(query, 0)} de {player.oid.Name}";

      player.oid.SendServerMessage("Votre contrat d'échange privé de ressources a bien été créé.", Color.OLIVE);
      player.oid.AcquireItem(contract, true);
      player.setValue = Config.invalidInput;
      player.menu.Close();
    }
    private void DrawCurrentContractPage(Player player)
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string> {
          $"Voici la liste de vos contrats privés actifs.",
          $"Lequel souhaitez-vous consulter ?"
      };

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT expirationDate, totalValue, rowid from playerPrivateContracts where characterId = @characterId");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);

      while(NWScript.SqlStep(query) > 0)
      {
        int contractId = NWScript.SqlGetInt(query, 2);

        TimeSpan remainingTime = (DateTime.Parse(NWScript.SqlGetString(query, 0)) - DateTime.Now);
        if(remainingTime.TotalMinutes > 0)
          player.menu.choices.Add(($"{contractId} - Expire dans : {remainingTime.Days}:{remainingTime.Hours}:{remainingTime.Minutes}:{remainingTime.Seconds} - Valeur totale : {NWScript.SqlGetInt(query, 1)}", () => HandleContractSelection(player, contractId)));
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

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT serializedContract, expirationDate, totalValue from playerPrivateContracts where rowid = @rowid");
      NWScript.SqlBindInt(query, "@rowid", contractId);
      NWScript.SqlStep(query);

      foreach (string materialString in NWScript.SqlGetString(query, 0).Split("|"))
      {
        string[] descriptionString = materialString.Split("$");
        if(descriptionString.Length == 3)
          player.menu.titleLines.Add($"{descriptionString[0]} : {descriptionString[1]} * {descriptionString[2]} = {Int32.Parse(descriptionString[1]) * Int32.Parse(descriptionString[2])}");
      }

      player.menu.titleLines.Add($"Valeur totale : {NWScript.SqlGetString(query, 2)}");
      player.menu.titleLines.Add($"Date d'expiration : {NWScript.SqlGetString(query, 1)}");

      player.menu.Draw();
    }
    private void HandleContractCancellation(Player player, int contractId)
    {
      DeleteExpiredContract(player, contractId);

      player.oid.SendServerMessage($"Le contrat {contractId} a été annulé.", Color.MAGENTA);
      DrawCurrentContractPage(player);
    }
    private void DeleteExpiredContract(Player player, int contractId)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT serializedContract from playerPrivateContracts where rowid = @rowid");
      NWScript.SqlBindInt(query, "@rowid", contractId);
      NWScript.SqlStep(query);

      foreach (string materialString in NWScript.SqlGetString(query, 0).Split("|"))
      {
        string[] descriptionString = materialString.Split("$");
        if (descriptionString.Length == 3)
          if (player.materialStock.ContainsKey(descriptionString[0]))
            player.materialStock[descriptionString[0]] += Int32.Parse(descriptionString[1]);
          else
            player.materialStock.Add(descriptionString[0], Int32.Parse(descriptionString[1]));
      }

      var deletionQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"DELETE from playerPrivateContracts where rowid = @rowid");
      NWScript.SqlBindInt(deletionQuery, "@rowid", contractId);
      NWScript.SqlStep(deletionQuery);
    }
  }
}
