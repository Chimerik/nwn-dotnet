namespace NWN.Systems
{
  public static partial class SpellConfig
  {
    public const int BaseSpellDC = 8;
    public const string CurrentSpellVariable = "_CURRENT_SPELL";

    public class SavingThrowFeedback
    {
      public int proficiencyBonus { get; set; }
      public int saveRoll { get; set; }

      public SavingThrowFeedback()
      {

      }
    }
  }
}
