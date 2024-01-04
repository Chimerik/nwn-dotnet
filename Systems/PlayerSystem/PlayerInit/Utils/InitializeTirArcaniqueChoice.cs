using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public void InitializeTirArcaniqueChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_TIR_ARCANIQUE_CHOICE").HasValue)
        {
          int nbManoeuvre = oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_TIR_ARCANIQUE_CHOICE").Value;

          if (!windows.TryGetValue("tirArcaniqueChoice", out var value)) windows.Add("tirArcaniqueChoice", new TirArcaniqueChoiceWindow(this, nbManoeuvre));
          else ((TirArcaniqueChoiceWindow)value).CreateWindow(nbManoeuvre);
        }
      }
    }
  }
}
