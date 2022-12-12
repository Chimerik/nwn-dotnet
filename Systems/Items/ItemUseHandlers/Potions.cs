using System;
using System.Linq;
using System.Text.Json;

using Anvil.API;

namespace NWN.Systems
{
  class Potion
  {
    public static void CureMini(NwPlayer oPC)
    {
      foreach (Effect arenaMalus in oPC.ControlledCreature.ActiveEffects.Where(f => f.Tag == "CUSTOM_EFFECT_MINI"))
        oPC.ControlledCreature.RemoveEffect(arenaMalus);

      oPC.ControlledCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpRemoveCondition));
    }
    public static void CureFrog(NwPlayer oPC)
    {
      foreach (Effect arenaMalus in oPC.ControlledCreature.ActiveEffects.Where(f => f.Tag == "CUSTOM_EFFECT_FROG"))
        oPC.ControlledCreature.RemoveEffect(arenaMalus);

      oPC.ControlledCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpRemoveCondition));
    }
    public static void AlchemyEffect(NwItem potion, NwPlayer oPC, NwGameObject target)
    {
      string[] jsonArray = potion.GetObjectVariable<LocalVariableString>("_SERIALIZED_PROPERTIES").Value.Split("|");

      foreach (string json in jsonArray)
      {
        CustomUnpackedEffect customUnpackedEffect = JsonSerializer.Deserialize<CustomUnpackedEffect>(json);

        if (target == null)
          customUnpackedEffect.ApplyCustomUnPackedEffectToTarget(oPC.ControlledCreature, potion);
        else
          customUnpackedEffect.ApplyCustomUnPackedEffectToTarget(target, potion);
      }
    }
  }
}
