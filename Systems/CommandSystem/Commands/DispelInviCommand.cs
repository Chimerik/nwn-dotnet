using NWN.Enums;
using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDispelInviCommand(ChatSystem.Context chatContext)
    {
      Spells.RemoveAnySpellEffects(Spell.ImprovedInvisibilit, chatContext.oSender);
      Spells.RemoveAnySpellEffects(Spell.Invisibilit, chatContext.oSender);
      Spells.RemoveAnySpellEffects(Spell.InvisibilitySpher, chatContext.oSender);
    }
  }
}
