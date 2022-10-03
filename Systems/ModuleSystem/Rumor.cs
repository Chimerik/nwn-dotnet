namespace NWN.Systems
{
  public class Rumor
  {
    public int id { get; set; }
    public string title { get; set; }
    public string content { get; set; }
    public bool dmCreated { get; set; }
    public int characterId { get; set; }
    public string characterName { get; set; }

    public Rumor(int id, string title, string content, bool dmCreated, int characterId, string characterName)
    {
      this.id = id;
      this.title = title;
      this.content = content;
      this.dmCreated = dmCreated;
      this.characterId = characterId;
      this.characterName = characterName;
    }
    public Rumor()
    {

    }
  }
}
