using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public string GetTlkOverride(StrRef tlk)
      {
        return tlkOverrides.TryGetValue(tlk, out var newTlk) ? newTlk.ToString() : tlk.ToString();
      }

      public string GetIconOverride(string icon)
      {
        return iconOverrides.TryGetValue(icon, out var newIcon) ? newIcon : icon;
      }
    }
  }
}
