namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private class Option
    {
      string description { get; }
      string defaultValue { get; set; }

      public Option (string description, string defaultValue = null)
      {
        this.description = description;
        this.defaultValue = defaultValue;
      }
    }
  }
}
