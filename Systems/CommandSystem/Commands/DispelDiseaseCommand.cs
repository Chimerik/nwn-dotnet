using NWN.API;
using NWN.API.Constants;
using System.Collections.Generic;
using System.Linq;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDispelDiseaseCommand(ChatSystem.Context ctx, Options.Result options)
    {
            NwPlayer oPC = ctx.oSender.ToNwObject<NwPlayer>();

            List<Effect> effectList = oPC.ActiveEffects.Where(e => e.EffectType == EffectType.Disease).ToList();

            if (effectList.Count == 0)
                oPC.ApplyEffect(EffectDuration.Permanent, Effect.CutsceneGhost());
            else
                foreach (Effect eff in effectList)
                    oPC.RemoveEffect(eff);
    }
  }
}
