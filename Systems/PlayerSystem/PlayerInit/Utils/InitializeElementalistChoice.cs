using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeElementalistChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_ELEMENTALIST_CHOICE_FEAT").HasValue)
        {
          if (!windows.TryGetValue("elementalistChoice", out var value)) windows.Add("elementalistChoice", new ElementalistChoiceWindow(this, oid.LoginCreature.Level));
          else ((ElementalistChoiceWindow)value).CreateWindow(oid.LoginCreature.Level);
        }
      }
    }
  }
}
