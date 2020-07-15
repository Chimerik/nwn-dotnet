namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private class Option
    {
      string name { get; }
      string alias { get; }
      string description { get; }
      string value { get; set; }

      Option (string description, string defaultValue = null, string name = null, string alias = null)
      {
        this.description = description;
        this.name = name;
        this.alias = alias;
        this.value = defaultValue;
      }
    }
  }
}
