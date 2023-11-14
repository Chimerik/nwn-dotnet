using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ResetSize()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableFloat>(CreatureUtils.OriginalSizeVariable).HasValue &&
          oid.LoginCreature.GetObjectVariable<PersistentVariableFloat>(CreatureUtils.OriginalSizeVariable).Value != oid.LoginCreature.VisualTransform.Scale
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.EnlargeEffectTag))
          oid.LoginCreature.VisualTransform.Scale = oid.LoginCreature.GetObjectVariable<PersistentVariableFloat>(CreatureUtils.OriginalSizeVariable).Value;
      }
    }
  }
}
