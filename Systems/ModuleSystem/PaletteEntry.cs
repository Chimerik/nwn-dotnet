namespace NWN.Systems
{
  public class PaletteEntry
  {
    public string name { get; set; }
    public string creator { get; set; }
    public string serializedObject { get; set; }
    public string lastModified { get; set; }
    public string comment { get; set; }
    public PaletteEntry(string name, string creator, string serializedObject, string lastModified, string comment)
    {
      this.name = name;
      this.creator = creator;
      this.serializedObject = serializedObject;
      this.lastModified = lastModified;
      this.comment = comment;
    }
    public PaletteEntry()
    {

    }
  }
}
