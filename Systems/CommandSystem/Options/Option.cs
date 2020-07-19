namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private class Option
    {
      public string description { get; }
      public string defaultValue { get; }
      public bool isRequired { get; }

      public Option (string description, bool isRequired = false, string defaultValue = null)
      {
        this.description = description;
        this.isRequired = isRequired;
        this.defaultValue = defaultValue;
      }
    }
  }
}
