using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public void ApplySentinelleImmortelle()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>(EffectSystem.SentinelleImmortelleVariable).HasValue
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.SentinelleImmortelleEffectTag))
        {
          oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.SentinelleImmortelle);
        }
      }
    }
  }
}
