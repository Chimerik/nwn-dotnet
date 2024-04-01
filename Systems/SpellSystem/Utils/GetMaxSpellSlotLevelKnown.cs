using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static byte GetMaxSpellSlotLevelKnown(NwCreature caster, ClassType spellClass)
    {
      var spellGain = NwClass.FromClassType(spellClass).SpellGainTable[caster.GetClassInfo(spellClass).Level - 1];
      for (byte i = 1; i < 10; i++)
        if (spellGain[i] < 1)
          return i < 1 ? (byte)0 : (byte)(i - 1);

      return 9;
    }
  }
}
