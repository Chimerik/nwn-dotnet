using NWN.Core;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDispelInviCommand(ChatSystem.Context ctx, Options.Result options)
    {
      SpellUtils.RemoveAnySpellEffects(NWScript.SPELL_IMPROVED_INVISIBILITY, ctx.oSender.ControlledCreature);
      SpellUtils.RemoveAnySpellEffects(NWScript.SPELL_INVISIBILITY, ctx.oSender.ControlledCreature);
      SpellUtils.RemoveAnySpellEffects(NWScript.SPELL_INVISIBILITY_SPHERE, ctx.oSender.ControlledCreature);
    }
  }
}
