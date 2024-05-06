using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyInspirationSuperieure()
      {
        if (oid.LoginCreature.Classes.Any(c => c.Class.ClassType == ClassType.Bard && c.Level > 19))
        {
          oid.OnCombatStatusChange -= BardUtils.OnCombatBardRecoverInspiration;
          oid.OnCombatStatusChange += BardUtils.OnCombatBardRecoverInspiration;
        }
      }
    }
  }
}
