using NWN.Core;
using NWN.API.Constants;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDispelInviCommand(ChatSystem.Context ctx, Options.Result options)
    {
      SpellUtils.RemoveAnySpellEffects(Spell.ImprovedInvisibility, ctx.oSender.ControlledCreature);
      SpellUtils.RemoveAnySpellEffects(Spell.Invisibility, ctx.oSender.ControlledCreature);
      SpellUtils.RemoveAnySpellEffects(Spell.InvisibilitySphere, ctx.oSender.ControlledCreature);
    }
  }
}
