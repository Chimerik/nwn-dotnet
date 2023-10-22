using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyHalfOrcEndurance()
      {
        ModuleSystem.Log.Info($"persistent : {oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_HALFORC_ENDURANCE").HasValue}");
        ModuleSystem.Log.Info($"endurance effect : {!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.enduranceImplacable.Tag)}");

        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_HALFORC_ENDURANCE").HasValue
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.enduranceImplacable.Tag))
            oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.enduranceImplacable);
      }
    }
  }
}
