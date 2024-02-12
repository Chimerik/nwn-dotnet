using System.Linq;
using System.Numerics;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Recuperation(NwCreature caster, NwGameObject target)
    {
      if(target is not NwCreature creature)
      {
        caster.LoginPlayer?.SendServerMessage("Cible invalide", ColorConstants.Red);
        return;
      }

      caster.LoginPlayer?.SendServerMessage("La récupération des emplacements de sorts n'est pas encore en place pour le moment", ColorConstants.Red);
    }
  }
}
