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
    public TradeRequest(SerializableTradeRequest serializedTradeRequest)
    {
      requesterId = serializedTradeRequest.requesterId;
      description = serializedTradeRequest.description;
      expirationDate = serializedTradeRequest.expirationDate;
      proposalList = new();

      foreach (var proposal in serializedTradeRequest.proposalList)
        proposalList.Add(new TradeProposal(proposal));
    }

    public class SerializableTradeRequest
    {
      public int requesterId { get; set; }
      public string description { get; set; }
      public List<TradeProposal.SerializableTradeProposal> proposalList { get; set; }
      public DateTime expirationDate { get; set; }

      public SerializableTradeRequest()
      {

      }
      public SerializableTradeRequest(TradeRequest tradeRequestBase)
      {
        requesterId = tradeRequestBase.requesterId;
        description = tradeRequestBase.description;
        expirationDate = tradeRequestBase.expirationDate;
        proposalList = new();

        foreach (var proposal in tradeRequestBase.proposalList)
          proposalList.Add(new TradeProposal.SerializableTradeProposal(proposal));
      }
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
    public TradeProposal(SerializableTradeProposal serializedTradeProposal)
    {
      characterId = serializedTradeProposal.characterId;
      sellPrice = serializedTradeProposal.sellPrice;
      serializedItems = serializedTradeProposal.serializedItems;
    }

    public class SerializableTradeProposal
    {
      public int characterId { get; set; }
      public int sellPrice { get; set; }
      public List<string> serializedItems { get; set; }

      public SerializableTradeProposal()
      {

      }
      public SerializableTradeProposal(TradeProposal TradeProposalBase)
      {
        characterId = TradeProposalBase.characterId;
        sellPrice = TradeProposalBase.sellPrice;
        serializedItems = TradeProposalBase.serializedItems;
      }
    }
  }
}
