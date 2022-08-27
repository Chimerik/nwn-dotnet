namespace NWN.Systems
{
  public class PaletteCreatureEntry
  {
    public string name { get; set; }
    public string creator { get; set; }
    public string serializedCreature { get; set; }
    public string lastModified { get; set; }
    public string comment { get; set; }
    public PaletteCreatureEntry(string name, string creator, string serializedCreature, string lastModified, string comment)
    {
      this.name = name;
      this.creator = creator;
      this.serializedCreature = serializedCreature;
      this.lastModified = lastModified;
      this.comment = comment;
    }
    public PaletteCreatureEntry()
    {

    }
  }
}
