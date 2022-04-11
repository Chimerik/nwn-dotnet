namespace NWN.Systems
{
  public class CustomDMVisualEffect
  {
    public int id { get; set; }
    public string name { get; set; }
    public int duration { get; set; }

    public CustomDMVisualEffect(int id, string name, int duration)
    {
      this.id = id;
      this.name = name;
      this.duration = duration;
    }
    public CustomDMVisualEffect()
    {

    }
  }
}
