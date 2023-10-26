using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellConfig
  {
    public const int BaseSpellDC = 8;

    public class SavingThrowFeedback
    {
      public int dexProficiencyBonus { get; set; }
      public int saveRoll { get; set; }

      public SavingThrowFeedback()
      {

      }
    }
  }
}
