namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private class Option
    {
      string alias { get; }
      string description { get; }
      string value { get; set; }

      Option (string description, string defaultValue = null, string alias = null)
      {
        this.description = description;
        this.alias = alias;
        this.value = defaultValue;
      }
    }
  }
}
