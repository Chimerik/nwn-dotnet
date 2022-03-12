namespace NWN.Systems
{
  public class Quickbar
  {
    public string name { get; set; }
    public string serializedQuickbar { get; set; }

    public Quickbar(string name, string serializedQuickbar)
    {
      this.name = name;
      this.serializedQuickbar = serializedQuickbar;
    }
    public Quickbar()
    {

    }
  }
}
