using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeMartialInitiateChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_MARTIAL_INITIATE_CHOICE_FEAT").HasValue)
        {
          bool gainedFromChampion = oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_MARTIAL_INITIATE_CHOICE_FEAT").Value == 2;

          if (!windows.TryGetValue("martialInitiateChoice", out var value)) windows.Add("martialInitiateChoice", new MartialInitiateChoiceWindow(this, oid.LoginCreature.Level, gainedFromChampion));
          else ((MartialInitiateChoiceWindow)value).CreateWindow(oid.LoginCreature.Level, gainedFromChampion);
        }
      }
    }
  }
}
