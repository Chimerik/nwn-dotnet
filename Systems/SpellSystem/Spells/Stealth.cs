using System;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static async void Stealth(NwCreature caster)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(0.4));
      caster.GetObjectVariable<LocalVariableInt>("_STEALTH_AUTHORIZED").Value = 1;
      caster.SetActionMode(ActionMode.Stealth, true);
    }
  }
}
