using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    private partial class Potion
    {
      public static void CureMini(NwPlayer oPC)
      {
        foreach (Effect arenaMalus in oPC.ControlledCreature.ActiveEffects.Where(f => f.Tag == "CUSTOM_EFFECT_MINI"))
          oPC.ControlledCreature.RemoveEffect(arenaMalus);

        oPC.ControlledCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpRemoveCondition));
      }
    }
  }
}
