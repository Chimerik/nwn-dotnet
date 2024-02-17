using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public void InitializeExpertiseChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_EXPERTISE_CHOICE").HasValue)
        {
          if (!windows.TryGetValue("expertiseChoice", out var value)) windows.Add("expertiseChoice", new ExpertiseChoiceWindow(this));
          else ((ExpertiseChoiceWindow)value).CreateWindow();
        }
      }
    }
  }
}
