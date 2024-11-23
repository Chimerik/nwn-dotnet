using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Dodge(NwGameObject oCaster, NwSpell spell)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.dodgeEffect, NwTimeSpan.FromRounds(1));

      caster.OnCreatureAttack -= CreatureUtils.OnAttackRemoveDodge;
      caster.OnCreatureAttack += CreatureUtils.OnAttackRemoveDodge;

      caster.OnSpellAction -= OnSpellInputRemoveDodge;
      caster.OnSpellAction += OnSpellInputRemoveDodge;

      if (caster.KnowsFeat((Feat)CustomSkill.VigueurNaine))
      {
        int HD = caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.VigueurNaineHDVariable).Value;

        if (HD > 0)
        {
          caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingS));
          caster.ApplyEffect(EffectDuration.Instant, Effect.Heal(NwRandom.Roll(Utils.random, caster.LevelInfo[Utils.random.Next(HD)].HitDie)));
          caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.VigueurNaineHDVariable).Value -= 1;
        }
        else
          caster.LoginPlayer?.SendServerMessage("Vigueur Naine à court de charges", ColorConstants.Red);
      }

      if (caster.KnowsFeat((Feat)CustomSkill.BelluaireEntrainementExceptionnel))
      {
        var companion = caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).Value;

        if (companion is not null)
        {
          companion.ApplyEffect(EffectDuration.Temporary, EffectSystem.dodgeEffect, NwTimeSpan.FromRounds(1));

          companion.OnCreatureAttack -= CreatureUtils.OnAttackRemoveDodge;
          companion.OnCreatureAttack += CreatureUtils.OnAttackRemoveDodge;

          companion.OnSpellAction -= OnSpellInputRemoveDodge;
          companion.OnSpellAction += OnSpellInputRemoveDodge;
        }
      }
    }
  }
}
