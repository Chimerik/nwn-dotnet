using System;

using Anvil.API;

using NWN.Core.NWNX;
using NWN.Native.API;

namespace NWN.Systems
{
  public class CustomUnpackedEffect
  {
    public int type { get; set; }
    public int power { get; set; }
    public float duration { get; set; }
    public string tag { get; set; }
    public int param0 { get; set; }
    public int param1 { get; set; }
    public int param2 { get; set; }

    public void ApplyCustomUnPackedEffectToTarget(NwGameObject oTarget)
    {
      Effect eff = Effect.HitPointChangeWhenDying(1);
      GC.SuppressFinalize(eff);
      
      NWNX_EffectUnpacked unpackedEffect = EffectPlugin.UnpackEffect(eff);

      unpackedEffect.nParam0 = param0;
      unpackedEffect.nParam1 = param1;
      unpackedEffect.nParam2 = param2;
      unpackedEffect.nType = type;
      unpackedEffect.sTag = tag;

      if (power > 0)
        unpackedEffect = AddPowerModifier(unpackedEffect, power);
      
      oTarget.ApplyEffect(EffectDuration.Temporary, EffectPlugin.PackEffect(unpackedEffect), TimeSpan.FromSeconds(duration));
    }

    private NWNX_EffectUnpacked AddPowerModifier(NWNX_EffectUnpacked unpackedEffect, int power)
    {
      switch((EffectTrueType)unpackedEffect.nType)
      {
        default: return unpackedEffect;

        case EffectTrueType.TemporaryHitpoints:
          unpackedEffect.nParam0 *= power;
          return unpackedEffect;
      }
    }
  }
}
