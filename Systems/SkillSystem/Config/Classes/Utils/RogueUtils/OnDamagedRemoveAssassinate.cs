using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class RogueUtils
  {
    public static async void OnDamagedRemoveAssassinate(CreatureEvents.OnDamaged onDamage)
    {
      EffectUtils.RemoveTaggedEffect(onDamage.Creature, EffectSystem.AssassinateEffectTag);

      await NwTask.NextFrame();
      onDamage.Creature.OnDamaged -= OnDamagedRemoveAssassinate;
    }
  }
}
