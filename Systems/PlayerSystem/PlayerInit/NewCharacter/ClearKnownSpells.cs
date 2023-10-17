using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ClearKnownSpells()
      {
        for (byte spellLevel = 0; spellLevel < 10; spellLevel++)
          if (oid.LoginCreature.GetClassInfo((ClassType)43).KnownSpells.Count > 0)
            oid.LoginCreature.GetClassInfo((ClassType)43).KnownSpells[spellLevel].Clear();
      }
    }
  }
}
