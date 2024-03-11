using System;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static async void DelayMessage(string message, CNWSCreature attacker)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(0.8));
      LogUtils.LogMessage($"Attaque supplémentaire - {message.StripColors()}", LogUtils.LogType.Combat);
      BroadcastNativeServerMessage(message.ColorString(StringUtils.gold), attacker);
    }
  }
}
