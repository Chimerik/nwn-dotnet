using System;
using System.Collections.Generic;
using System.Linq;
using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class PrivateContract
  {
    public PrivateContract(NwCreature oPC, NwItem contract)
    {
      if (!(Players.TryGetValue(oPC, out Player player)))
        return;

      if (!DateTime.TryParse(contract.GetLocalVariable<string>("_CONTRACT_EXPIRATION_DATE").Value, out DateTime expirationDate)
        || contract.GetLocalVariable<int>("_CONTRACT_ID").HasNothing || contract.GetLocalVariable<string>("_SERIALIZED_CONTRACT_DATA").HasNothing
        || contract.GetLocalVariable<int>("_CONTRACT_TOTAL_GOLD_PRICE").HasNothing || contract.GetLocalVariable<int>("_CONTRACT_CREATOR_ID").HasNothing)
      {
        player.oid.SendServerMessage("Certaines mentions légales sont absentes de ce contrat qui ne semble donc pas valide.", ColorConstants.Purple);
        contract.Destroy();
        return;
      }

      int creatorId = contract.GetLocalVariable<int>("_CONTRACT_CREATOR_ID").Value;

      if (creatorId == player.characterId)
      {
        player.oid.SendServerMessage("Vous ne pouvez pas accepter votre propre contrat.");
        return;
      }

      if ((expirationDate - DateTime.Now).TotalSeconds < 0)
      {
        player.oid.SendServerMessage("Ce contrat est expiré et n'est donc plus valide.", ColorConstants.Blue);
        contract.Destroy();
        return;
      }

      int contractId = contract.GetLocalVariable<int>("_CONTRACT_ID").Value;

      var result = SqLiteUtils.SelectQuery("playerPrivateContracts",
        new List<string>() { { "characterId" } },
        new List<string[]>() { new string[] { "rowid", contractId.ToString() } });

      if(result.Result == null)
      {
        player.oid.SendServerMessage("Ce contrat a été annulé par son rédacteur et n'est donc plus valide.", ColorConstants.Blue);
        contract.Destroy();
        return;
      }

      int totalPrice = contract.GetLocalVariable<int>("_CONTRACT_TOTAL_GOLD_PRICE").Value;

      if (totalPrice > player.bankGold + (int)oPC.Gold)
      {
        player.oid.SendServerMessage("Vous ne disposez pas de l'or nécessaire pour accepter ce contrat.", ColorConstants.Orange);
        return;
      }

      if ((int)oPC.Gold < totalPrice)
      {
        player.bankGold -= totalPrice - (int)oPC.Gold;
        player.oid.SendServerMessage($"{totalPrice - (int)oPC.Gold} ont été prélevés directement à la banque.", ColorConstants.Rose);
        oPC.Gold = 0;
      }
      else
        oPC.Gold -= (uint)totalPrice;

      NwPlayer oCreator = NwModule.Instance.Players.FirstOrDefault(p => ObjectPlugin.GetInt(p.LoginCreature, "characterId") == creatorId);

      if (oCreator != null)
      {
        if (Players.TryGetValue(oCreator.LoginCreature, out Player creator))
          creator.bankGold += totalPrice;

        oCreator.SendServerMessage($"Votre contrat {contractId} a été accepté par {oPC.Name}. La somme de {totalPrice} pièce(s) d'or a été versée sur votre compte.", ColorConstants.Navy);
      }
      else
      {
        if (SqLiteUtils.UpdateQuery("playerCharacters",
          new List<string[]>() { new string[] { "bankGold+", totalPrice.ToString() } },
          new List<string[]>() { new string[] { "rowid", creatorId.ToString() } }))
          Utils.SendMailToPC(creatorId, "Hôtel des ventes de Similisse", "Contrat accepté", $"Votre contrat {contractId} a été accepté par {oPC.Name}. La somme de {totalPrice} pièce(s) d'or a été versée sur votre compte.");
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

          player.oid.SendServerMessage($"{quantity} unité(s) de {material} ont été transférés vers votre entrepôt.", ColorConstants.Magenta);
        }
      }

      SqLiteUtils.DeletionQuery("playerPrivateContracts",
         new Dictionary<string, string>() { { "rowid", contractId.ToString() } });

      contract.Destroy();
    }
  }
}
