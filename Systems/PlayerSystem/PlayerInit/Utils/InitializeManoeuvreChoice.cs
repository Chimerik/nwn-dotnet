using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public void InitializeManoeuvreChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_MANOEUVRE_CHOICE").HasValue)
        {
          int nbManoeuvre = oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_MANOEUVRE_CHOICE").Value;

          if (!windows.TryGetValue("manoeuvreChoice", out var value)) windows.Add("manoeuvreChoice", new ManoeuvreChoiceWindow(this, nbManoeuvre));
          else ((ManoeuvreChoiceWindow)value).CreateWindow(nbManoeuvre);
        }
      }
    }
  }
}
