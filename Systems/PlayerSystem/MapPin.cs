namespace NWN.Systems
{
  public class MapPin
  {
    public int id { get; }
    public string areaTag { get; set; }
    public float x { get; set; }
    public float y { get; set; }
    public string note { get; set; }

    public MapPin(int id, string areaTag, float x, float y, string note)
    {
      this.id = id;
      this.areaTag = areaTag;
      this.x = x;
      this.y = y;
      this.note = note;
    }
  }
}
