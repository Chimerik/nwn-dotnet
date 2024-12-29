using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool HandleReactionUse(NwCreature creature)
    {
      if (creature.GetObjectVariable<LocalVariableInt>(ReactionVariable).Value > 0 || creature.IsDMPossessed || creature.IsDMAvatar)
      {
        creature.GetObjectVariable<LocalVariableInt>(ReactionVariable).Value -= 1;
        return true;
      }

      creature.LoginPlayer?.SendServerMessage("Aucune réaction disponible", ColorConstants.Red);
      return false;
    }
  }
}
