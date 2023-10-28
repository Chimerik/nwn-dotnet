using System;
using Anvil.API.Events;
using Anvil.Services;

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
