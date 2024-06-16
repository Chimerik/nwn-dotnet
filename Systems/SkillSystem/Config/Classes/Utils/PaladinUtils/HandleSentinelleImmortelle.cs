using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class PaladinUtils
  {
    public static void HandleSentinelleImmortelle(CreatureEvents.OnDamaged onDamage)
    {
      if (onDamage.Creature.HP < 1)
      {
        onDamage.Creature.HP = 1;

        EffectUtils.RemoveTaggedEffect(onDamage.Creature, EffectSystem.SentinelleImmortelleEffectTag);

        onDamage.Creature.GetObjectVariable<PersistentVariableInt>(EffectSystem.SentinelleImmortelleVariable).Delete();
        onDamage.Creature.OnDamaged -= HandleSentinelleImmortelle;

        StringUtils.DisplayStringToAllPlayersNearTarget(onDamage.Creature, "Sentinelle Immortelle", StringUtils.gold, true);
      }
    }
  }
}
