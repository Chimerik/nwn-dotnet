using System.Linq;
using NWN.API;

namespace NWN.Systems
{
  class ListenAll
  {
    public ListenAll(PlayerSystem.Player player)
    {
      if (player.listened.Count > 0)
      {
        player.oid.SendServerMessage("Ecoute globale désactivée.", ColorConstants.Cyan);
        player.listened.Clear();
      }
      else
      {
        foreach (NwPlayer oPC in NwModule.Instance.Players.Where(p => !p.IsDM))
          player.listened.Add(oPC);

        player.oid.SendServerMessage("Ecoute globale activée.", ColorConstants.Cyan);
      }
    }
  }
}
