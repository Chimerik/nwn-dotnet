using System;
using System.Numerics;
using NWN.API;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteVFXCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out PlayerSystem.Player player))
      {
          Action<uint, Vector3> callback = (uint oTarget, Vector3 position) =>
          {
            if (((string)options.positional[0]).Length != 0)
            {
              if (Int32.TryParse((string)options.positional[0], out int value))
              {
                oTarget.ToNwObject<NwGameObject>().ApplyEffect(EffectDuration.Temporary, API.Effect.VisualEffect((API.Constants.VfxType)value), TimeSpan.FromSeconds(5));
                return;
              }
            }
          };

          player.targetEvent = TargetEvent.LootSaverTarget;
          player.SelectTarget(callback);
      }
    }
  }
}
