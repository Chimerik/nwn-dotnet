using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static async void Dodge(NwCreature caster, PlayerSystem.Player player)
    {
      await NwTask.NextFrame();
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.dodgeEffect, NwTimeSpan.FromRounds(2));
      
      caster.OnCreatureAttack -= CreatureUtils.OnAttackRemoveDodge;
      caster.OnCreatureAttack += CreatureUtils.OnAttackRemoveDodge;

      caster.OnSpellAction -= SpellSystem.OnSpellInputRemoveDodge;
      caster.OnSpellAction += SpellSystem.OnSpellInputRemoveDodge;

      if(player.learnableSkills.TryGetValue(CustomSkill.VigueurNaine, out LearnableSkill vigueur) && vigueur.currentLevel > 0)
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
