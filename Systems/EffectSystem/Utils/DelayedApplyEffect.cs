using System;
using Anvil.API;
using NWN.Core;

namespace NWN
{
  public static partial class EffectUtils
  {
    public static async void DelayedApplyEffect(NwGameObject target, EffectDuration durationType, Effect eff, TimeSpan delay, TimeSpan remainingDuration = default)
    {
      await NwTask.Delay(delay);

      if (target is null || !target.IsValid)
        return;

      if (eff.Creator is not null)
        NWScript.AssignCommand(eff.Creator, () => target.ApplyEffect(durationType, eff, remainingDuration));
      else
        target.ApplyEffect(durationType, eff, remainingDuration);
    }
  }
}
