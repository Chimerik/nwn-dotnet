using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class WizardUtils
  {
    public static void HandleEvocateurSurchargeSelfDamage(NwCreature creature, byte spellLevel)
    {
      int selfDamageDice = creature.GetObjectVariable<LocalVariableInt>(EffectSystem.EvocateurSurchargeVariable).Value;

      if (selfDamageDice > 0)
      {
        NWScript.AssignCommand(creature, () => creature.ApplyEffect(EffectDuration.Instant, Effect.Damage(
          NwRandom.Roll(Utils.random, 12, selfDamageDice * spellLevel), CustomDamageType.Necrotic)));

        creature.GetObjectVariable<LocalVariableInt>(EffectSystem.EvocateurSurchargeVariable).Value += 1;
      }
      else
        creature.GetObjectVariable<LocalVariableInt>(EffectSystem.EvocateurSurchargeVariable).Value = 2;
    }
  }
}
