using System;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static async void Stealth(SpellEvents.OnSpellCast onSpellCast)
    {
      if (onSpellCast.Caster is not NwCreature caster)
        return;

      await NwTask.Delay(TimeSpan.FromSeconds(0.4));
      caster.GetObjectVariable<LocalVariableInt>("_STEALTH_AUTHORIZED").Value = 1;
      caster.SetActionMode(ActionMode.Stealth, true);
    }
  }
}
