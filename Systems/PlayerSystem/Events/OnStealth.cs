using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public static void OnStealth(OnStealthModeUpdate onStealth)
    {
      if(onStealth.EventType == ToggleModeEventType.Enter)
      {
        if (onStealth.Creature.GetObjectVariable<LocalVariableInt>("_STEALTH_AUTHORIZED").HasNothing)
          onStealth.EnterOverride = StealthModeOverride.PreventEnter;
        else
        {
          onStealth.EnterOverride = StealthModeOverride.None;
          onStealth.Creature.GetObjectVariable<LocalVariableInt>("_STEALTH_AUTHORIZED").Delete();
        }
      }
    }
  }
}
