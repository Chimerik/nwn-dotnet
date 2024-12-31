using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleInspirationBardiqueUsed(CNWSCreature creature, int inspirationBonus, CGameEffect effect, string inspirationString, string attackerName)
    {
      creature.RemoveEffect(effect);
      LogUtils.LogMessage($"Activation {inspirationString}{inspirationBonus}", LogUtils.LogType.Combat);
      BroadcastNativeServerMessage($"{attackerName.ColorString(ColorConstants.Cyan)} - {inspirationString} : {StringUtils.ToWhitecolor(inspirationBonus)}".ColorString(StringUtils.gold), creature);
      return inspirationBonus;
    }
  }
}
