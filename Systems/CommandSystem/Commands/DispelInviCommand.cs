using NWN.Enums;
using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDispelInviCommand(ChatSystem.Context ctx, Options.Result options)
    {
      Spells.RemoveAnySpellEffects(Spell.ImprovedInvisibilit, ctx.oSender);
      Spells.RemoveAnySpellEffects(Spell.Invisibilit, ctx.oSender);
      Spells.RemoveAnySpellEffects(Spell.InvisibilitySpher, ctx.oSender);
    }
  }
}
