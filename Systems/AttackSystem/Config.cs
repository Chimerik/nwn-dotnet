
using NWN.API;

namespace NWN.Systems
{
  public static partial class Config
  {
    public enum AttackPosition
    {
      Low = 1,
      NormalOrRanged,
      High,
    }
    public static int GetIPSpecificAlignmentSubTypeAsInt(NwCreature oCreature)
    {
      return (oCreature.LawChaosAlignment + "_" + oCreature.GoodEvilAlignment) switch
      {
        "Lawful_Good" => 0,
        "Lawful_Neutral" => 1,
        "Lawful_Evil" => 2,
        "Neutral_Good" => 3,
        "Neutral_Neutral" => 4,
        "Neutral_Evil" => 5,
        "Chaotic_Good" => 6,
        "Chaotic_Neutral" => 7,
        "Chaotic_Evil" => 8,
        _ => 4,
      };
    }
    public static short RollDamage(int costValue)
    {
      switch(costValue)
      {
        case 1:
        case 2:
        case 3:
        case 4:
        case 5:
          return (short)costValue;
        case 6:
          return (short)NwRandom.Roll(Utils.random, 4);
        case 7:
          return (short)NwRandom.Roll(Utils.random, 6);
        case 8:
          return (short)NwRandom.Roll(Utils.random, 8);
        case 9:
          return (short)NwRandom.Roll(Utils.random, 10);
        case 10:
          return (short)NwRandom.Roll(Utils.random, 6, 2);
        case 11:
          return (short)NwRandom.Roll(Utils.random, 8, 2);
        case 12:
          return (short)NwRandom.Roll(Utils.random, 4, 2);
        case 13:
          return (short)NwRandom.Roll(Utils.random, 10, 2);
        case 14:
          return (short)NwRandom.Roll(Utils.random, 12, 1);
        case 15:
          return (short)NwRandom.Roll(Utils.random, 12, 2);
        case 16:
        case 17:
        case 18:
        case 19:
        case 20:
        case 21:
        case 22:
        case 23:
        case 24:
        case 25:
        case 26:
        case 27:
        case 28:
        case 29:
        case 30:
          return (short)(costValue - 10);
        default:
          return 0;
      }
    }
  }
}
