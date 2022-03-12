namespace NWN.Systems
{
  public class CharacterDescription
  {
    public string name { get; set; }
    public string description { get; set; }

    public CharacterDescription(string name, string description)
    {
      this.name = name;
      this.description = description;
    }
    public CharacterDescription()
    {

    }
  }
}
