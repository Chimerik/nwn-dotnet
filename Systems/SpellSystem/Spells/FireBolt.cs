using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void FireBolt(NwCreature caster, SpellEvents.OnSpellCast onSpellCast, SpellEntry spellEntry)
    {
      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, caster, onSpellCast.Spell.SpellType);

      if (onSpellCast.TargetObject is NwCreature target)
      {
        if (target.IsReactionTypeFriendly(caster))
        {
          var swapPosition = caster.Position;
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));

          caster.Position = target.Position;
          target.Position = swapPosition;
        }
        else
          caster.LoginPlayer?.SendServerMessage("Cible invalide", ColorConstants.Red);
      }
      else if(onSpellCast.TargetObject is null)
      {
        onSpellCast.TargetLocation.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));
        caster.Position = onSpellCast.TargetLocation.Position;
      }

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));
      caster.SetFeatRemainingUses((Feat)CustomSkill.InvocationPermutation, 0);
    }
  }
}
