using System;
using System.Linq;
using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class PrivateContract
  {
    public PrivateContract(NwPlayer oPC, NwItem contract)
    {
      if (!(Players.TryGetValue(oPC, out Player player)))
        return;

      if (!DateTime.TryParse(contract.GetLocalVariable<string>("_CONTRACT_EXPIRATION_DATE").Value, out DateTime expirationDate)
        || contract.GetLocalVariable<int>("_CONTRACT_ID").HasNothing || contract.GetLocalVariable<string>("_SERIALIZED_CONTRACT_DATA").HasNothing
        || contract.GetLocalVariable<int>("_CONTRACT_TOTAL_GOLD_PRICE").HasNothing || contract.GetLocalVariable<int>("_CONTRACT_CREATOR_ID").HasNothing)
      {
        oPC.SendServerMessage("Certaines mentions légales sont absentes de ce contrat qui ne semble donc pas valide.", Color.PURPLE);
        contract.Destroy();
        return;
      }

      int creatorId = contract.GetLocalVariable<int>("_CONTRACT_CREATOR_ID").Value;

      if (creatorId == player.characterId)
      {
        oPC.SendServerMessage("Vous ne pouvez pas accepter votre propre contrat.");
        return;
      }

      if ((expirationDate - DateTime.Now).TotalSeconds < 0)
      {
        oPC.SendServerMessage("Ce contrat est expiré et n'est donc plus valide.", Color.BLUE);
        contract.Destroy();
        return;
      }

      int contractId = contract.GetLocalVariable<int>("_CONTRACT_ID").Value;

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT characterId from playerPrivateContracts where rowid = @rowid");
      NWScript.SqlBindInt(query, "@rowid", contractId);
      
      if(NWScript.SqlStep(query) <= 0)
      {
        oPC.SendServerMessage("Ce contrat a été annulé par son rédacteur et n'est donc plus valide.", Color.BLUE);
        contract.Destroy();
        return;
      }

      int totalPrice = contract.GetLocalVariable<int>("_CONTRACT_TOTAL_GOLD_PRICE").Value;

      if (totalPrice > player.bankGold + (int)oPC.Gold)
      {
        oPC.SendServerMessage("Vous ne disposez pas de l'or nécessaire pour accepter ce contrat.", Color.ORANGE);
        return;
      }

      if ((int)oPC.Gold < totalPrice)
      {
        player.bankGold -= totalPrice - (int)oPC.Gold;
        oPC.SendServerMessage($"{totalPrice - (int)oPC.Gold} ont été prélevés directement à la banque.", Color.ROSE);
        oPC.Gold = 0;
      }
      else
        oPC.Gold -= (uint)totalPrice;

      NwPlayer oCreator = NwModule.Instance.Players.FirstOrDefault(p => ObjectPlugin.GetInt(p, "characterId") == creatorId);

      if (oCreator != null)
      {
        if (Players.TryGetValue(oCreator, out Player creator))
          creator.bankGold += totalPrice;

        oCreator.SendServerMessage($"Votre contrat {contractId} a été accepté par {oPC.Name}. La somme de {totalPrice} pièce(s) d'or a été versée sur votre compte.", Color.NAVY);
      }
      else
      {
        query = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE playerCharacters SET bankGold = bankGold + @bankGold where rowid = @characterId");
        NWScript.SqlBindInt(query, "@characterId", creatorId);
        NWScript.SqlBindInt(query, "@bankGold", totalPrice);
        NWScript.SqlStep(query);

        //TODO : si le joueur n'est pas connecté, lui envoyer une lettre via le système de courrier
      }


      foreach (string materialString in contract.GetLocalVariable<string>("_SERIALIZED_CONTRACT_DATA").Value.Split("|"))
      {
        string[] descriptionString = materialString.Split("$");
        if (descriptionString.Length == 3)
        {
          int quantity = Int32.Parse(descriptionString[1]);
          string material = descriptionString[0];

          if (player.materialStock.ContainsKey(material))
            player.materialStock[material] += quantity;
          else
            player.materialStock.Add(material, quantity);

          oPC.SendServerMessage($"{quantity} unité(s) de {material} ont été transférés vers votre entrepôt.", Color.MAGENTA);
        }
      }

      var deletionQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"DELETE from playerPrivateContracts where rowid = @rowid");
      NWScript.SqlBindInt(deletionQuery, "@rowid", contractId);
      NWScript.SqlStep(deletionQuery);

      contract.Destroy();
    }
  }
}
