using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlaceableSystem
  {
    public static void TestBouleDeFeu(PlaceableEvents.OnLeftClick onUsed)
    {
      _ = onUsed.Placeable.ActionCastSpellAt(Spell.BurningHands, onUsed.ClickedBy.LoginCreature);
    }
  }
}
