namespace NWN.Systems
{
  class ContractMenu
  {
    public ContractMenu(PlayerSystem.Player player)
    {
      new PrivateContractCreator(player);
    }
  }
}
