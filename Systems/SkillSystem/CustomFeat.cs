namespace NWN.Systems
{
  public class CustomFeat
  {
    public string name { get; }
    public string description { get; }
    public int maxLevel { get; }

    public CustomFeat(string name, string description, int maxLevel)
    {
      this.name = name;
      this.description = description;
      this.maxLevel = maxLevel;
    }
  }
}
