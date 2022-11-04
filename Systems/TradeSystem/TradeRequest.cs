using System;
using System.Collections.Generic;

namespace NWN.Systems.TradeSystem
{
  public class TradeRequest
  {
    public readonly int requesterId;
    public readonly string description;
    public readonly int buyoutPrice;
    public readonly DateTime expirationDate;
    public List<TradeProposal> proposalList;

    public TradeRequest(int requesterId, string description, int buyoutPrice, DateTime expirationDate, List<TradeProposal> proposalList)
    {
      this.requesterId = requesterId;
      this.description = description;
      this.buyoutPrice = buyoutPrice;
      this.expirationDate = expirationDate;
      this.proposalList = proposalList;
    }
    public TradeRequest()
    {

    }
  }

  public class TradeProposal
  {
    public readonly int characterId;
    public readonly List<string> serializedItems;

    public TradeProposal(int characterId, List<string> serializedItems)
    {
      this.characterId = characterId;
      this.serializedItems = serializedItems;
    }
    public TradeProposal()
    {

    }
  }
}
