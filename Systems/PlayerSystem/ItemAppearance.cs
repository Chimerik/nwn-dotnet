namespace NWN.Systems
{
  public class ItemAppearance
  {
    public string name { get; set; }
    public string serializedAppearance { get; set; }
    public int baseItem { get; set; }
    public int ACValue { get; set; }

    public ItemAppearance(string name, string serializedAppearance, int baseItem, int ACValue)
    {
      this.name = name;
      this.serializedAppearance = serializedAppearance;
      this.baseItem = baseItem;
      this.ACValue = ACValue;
    }
    public ItemAppearance()
    {

    }
  }
}
