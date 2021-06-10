
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
    public static string GetIPSpecificAlignmentSubType(NwCreature oCreature)
    {
      switch (oCreature.LawChaosAlignment + "_" + oCreature.GoodEvilAlignment)
      {
        case "Lawful_Good":
          return "0";
        case "Lawful_Neutral": //LN
          return "1";
        case "Lawful_Evil": //LM
          return "2";
        case "Neutral_Good":
          return "3";
        case "Neutral_Neutral":
          return "4";
        case "Neutral_Evil":
          return "5";
        case "Chaotic_Good":
          return "6";
        case "Chaotic_Neutral":
          return "7";
        case "Chaotic_Evil":
          return "8";
      }

      return "4";
    }
    public static int GetIPSpecificAlignmentSubTypeAsInt(NwCreature oCreature)
    {
      switch (oCreature.LawChaosAlignment + "_" + oCreature.GoodEvilAlignment)
      {
        case "Lawful_Good":
          return 0;
        case "Lawful_Neutral": //LN
          return  1;
        case "Lawful_Evil": //LM
          return 2;
        case "Neutral_Good":
          return 3;
        case "Neutral_Neutral":
          return 4;
        case "Neutral_Evil":
          return 5;
        case "Chaotic_Good":
          return 6;
        case "Chaotic_Neutral":
          return 7;
        case "Chaotic_Evil":
          return 8;
      }

      return 4;
    }
  }
}
