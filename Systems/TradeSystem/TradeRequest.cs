using System;
using System.Collections.Generic;

namespace NWN.Systems
{
  public class TradeRequest
  {
    public readonly int requesterId;
    public readonly string description;
    public readonly int buyoutPrice;
    public DateTime expirationDate;
    public List<TradeProposal> proposalList;

    public TradeRequest(int requesterId, string description, DateTime expirationDate, List<TradeProposal> proposalList)
    {
      this.requesterId = requesterId;
      this.description = description;
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
    public readonly int sellPrice;
    public readonly List<string> serializedItems;
    public bool cancelled;

    public TradeProposal(int characterId, int sellPrice, List<string> serializedItems)
    {
      this.characterId = characterId;
      this.sellPrice = sellPrice;
      this.serializedItems = serializedItems;
      cancelled = false;
    }
    public TradeProposal()
    {

    }
  }
}
