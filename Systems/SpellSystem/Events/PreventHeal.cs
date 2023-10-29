using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void PreventHeal(OnHeal onHeal)
    {
      onHeal.HealAmount = 0;
    }
  }
}
