using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public void ApplyHalfOrcEndurance()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>(EffectSystem.EnduranceImplacableVariable).HasValue
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.EnduranceImplacableEffectTag))      
        {
          oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.enduranceImplacable);
        }
      }
    }
  }
}
