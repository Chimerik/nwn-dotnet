using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ArmuredAgathysEffectTag = "_ARMURE_DAGATHYS_EFFECT";
    public static Effect ArmuredAgathys(NwCreature oCaster)
    {
      oCaster.OnDamaged -= OnDamagedArmuredAgathys;
      oCaster.OnDamaged += OnDamagedArmuredAgathys;

      Effect eff = Effect.LinkEffects(Effect.TemporaryHitpoints(5), Effect.DamageShield(5, 0, DamageType.Cold));
      eff.Tag = ArmuredAgathysEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    private static void OnDamagedArmuredAgathys(Anvil.API.Events.CreatureEvents.OnDamaged onDamaged)
    {
      if(!onDamaged.Creature.ActiveEffects.Any(e => e.EffectType == EffectType.TemporaryHitpoints))
      {
        onDamaged.Creature.OnDamaged -= OnDamagedArmuredAgathys;
        EffectUtils.RemoveTaggedEffect(onDamaged.Creature, ArmuredAgathysEffectTag);
      }
    }
  }
}
