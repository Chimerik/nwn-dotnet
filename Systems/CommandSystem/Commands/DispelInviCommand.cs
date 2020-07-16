using NWN.Enums;
using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDispelInviCommand(ChatSystem.ChatEventArgs e, Options.Result options)
    {
      Spells.RemoveAnySpellEffects(Spell.ImprovedInvisibilit, e.oSender);
      Spells.RemoveAnySpellEffects(Spell.Invisibilit, e.oSender);
      Spells.RemoveAnySpellEffects(Spell.InvisibilitySpher, e.oSender);
    }
  }
}
