using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlaceableSystem
  {
    public static void TestBouleDeFeu(PlaceableEvents.OnLeftClick onUsed)
    {
      _ = onUsed.Placeable.ActionCastSpellAt(NwSpell.FromSpellType(Spell.Fireball), onUsed.ClickedBy.ControlledCreature, cheat:true);
    }
  }
}
