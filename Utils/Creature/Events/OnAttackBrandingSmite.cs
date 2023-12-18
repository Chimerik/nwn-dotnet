using Anvil.API;
using Anvil.API.Events;
using NWN.Core;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void OnAttackBrandingSmite(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

            NWScript.AssignCommand(onAttack.Attacker, () => target.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.Damage(NwRandom.Roll(Utils.random, 6, 2), DamageType.Divine), Effect.VisualEffect(VfxType.ImpDivineStrikeHoly))));
            NWScript.AssignCommand(onAttack.Attacker, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.brandingSmiteReveal, NwTimeSpan.FromRounds(Spells2da.spellTable[CustomSpell.BrandingSmite].duration)));

            target.OnStealthModeUpdate -= EffectSystem.OnBrandingSmiteReveal;
            target.OnStealthModeUpdate += EffectSystem.OnBrandingSmiteReveal;

            target.OnEffectApply -= EffectSystem.OnBrandingSmiteReveal;
            target.OnEffectApply += EffectSystem.OnBrandingSmiteReveal;

            foreach (var eff in target.ActiveEffects)
            {
              switch (eff.EffectType)
              {
                case EffectType.Invisibility:
                case EffectType.ImprovedInvisibility: target.RemoveEffect(eff); break;
              }
            }

            SpellUtils.DispelConcentrationEffects(onAttack.Attacker);

          break;
      }
    }
  }
}
