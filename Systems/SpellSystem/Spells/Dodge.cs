﻿using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Dodge(SpellEvents.OnSpellCast onSpellCast, PlayerSystem.Player player)
    {
      if (onSpellCast.Caster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, caster, onSpellCast.Spell.SpellType);

      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.dodgeEffect, NwTimeSpan.FromRounds(1));

      caster.OnCreatureAttack -= CreatureUtils.OnAttackRemoveDodge;
      caster.OnCreatureAttack += CreatureUtils.OnAttackRemoveDodge;

      caster.OnSpellAction -= OnSpellInputRemoveDodge;
      caster.OnSpellAction += OnSpellInputRemoveDodge;

      if (player.learnableSkills.TryGetValue(CustomSkill.VigueurNaine, out LearnableSkill vigueur) && vigueur.currentLevel > 0)
      {
        int HD = caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.VigueurNaineHDVariable).Value;

        if (HD > 0)
        {
          caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingS));
          caster.ApplyEffect(EffectDuration.Instant, Effect.Heal(NwRandom.Roll(Utils.random, caster.LevelInfo[Utils.random.Next(HD)].HitDie)));
          caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.VigueurNaineHDVariable).Value -= 1;
        }
        else
          player.oid.SendServerMessage("Vigueur Naine à court de charges", ColorConstants.Red);
      }
    }
  }
}
