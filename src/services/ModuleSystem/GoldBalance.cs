namespace ModuleService
{
  public class GoldBalance
  {
    public int nbTimesLooted { get; set; }
    public int cumulatedGold { get; set; }
    public GoldBalance(int gold)
    {
      nbTimesLooted = 1;
      cumulatedGold = gold;
    }
  }
}
