using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetPeauDecorceAC(CNWSCreature creature, int AC)
    {
      if (AC < 16 && creature.HasSpellEffectApplied((int)Spell.Barkskin).ToBool())
      {
        AC = 16;
        LogUtils.LogMessage("Peau d'écorce: CA fixée à 16", LogUtils.LogType.Combat);
      }

      return AC;
    }
  }
}
