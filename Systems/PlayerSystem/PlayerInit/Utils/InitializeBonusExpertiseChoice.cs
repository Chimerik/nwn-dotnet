using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeBonusExpertiseChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_EXPERTISE_BONUS_CHOICE_FEAT").HasValue)
        {
          int source = oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_EXPERTISE_BONUS_CHOICE_FEAT").Value;
          if (!windows.TryGetValue("expertiseBonusChoice", out var value)) windows.Add("expertiseBonusChoice", new ExpertiseBonusChoiceWindow(this, oid.LoginCreature.Level, source));
          else ((ExpertiseBonusChoiceWindow)value).CreateWindow(oid.LoginCreature.Level, source);
        }
      }
    }
  }
}
