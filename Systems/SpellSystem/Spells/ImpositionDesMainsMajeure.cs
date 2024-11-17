
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ImpositionDesMainsMajeure(NwGameObject oCaster, NwGameObject oTarget, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;

      if (caster.GetFeatRemainingUses((Feat)CustomSkill.ImpositionDesMains) < 2)
      {
        caster.LoginPlayer?.SendServerMessage("2 charges requises", ColorConstants.Red);
        return;
      }

      SpellUtils.SignalEventSpellCast(oCaster, oTarget, spell.SpellType);

      int healAmount = 4 * caster.GetClassInfo(ClassType.Paladin).Level;

      oTarget.ApplyEffect(EffectDuration.Instant, Effect.Heal(healAmount));
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingL));

      caster.DecrementRemainingFeatUses((Feat)CustomSkill.ImpositionDesMains);
    }
  }
}
