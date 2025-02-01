using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool HandleReactionUse(NwCreature creature)
    {
      var reaction = creature.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.ReactionEffectTag);

      if (reaction is not null)
      {
        creature.RemoveEffect(reaction);
        return true;
      }

      creature.LoginPlayer?.SendServerMessage("Aucune réaction disponible", ColorConstants.Red);
      return false;
    }
  }
}
