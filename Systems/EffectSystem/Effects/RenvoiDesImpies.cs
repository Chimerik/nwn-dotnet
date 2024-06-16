using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string RenvoiDesImpiesEffectTag = "_RENVOI_DES_IMPIES_EFFECT";

    public static Effect GetRenvoiDesImpiesEffect(NwCreature target)
    {
      Effect eff = Effect.LinkEffects(Effect.Turned(), Effect.VisualEffect(VfxType.DurMindAffectingFear));
      eff.Tag = RenvoiDesImpiesEffectTag;
      eff.SubType = EffectSubType.Supernatural;

      target.OnDamaged -= OnDamageRemoveRenvoi;
      target.OnDamaged += OnDamageRemoveRenvoi;
      return eff;
    }

    public static void OnDamageRemoveRenvoi(Anvil.API.Events.CreatureEvents.OnDamaged onDamaged)
    {
      EffectUtils.RemoveTaggedEffect(onDamaged.Creature, RenvoiDesImpiesEffectTag, RenvoiDesInfidelesEffectTag);
      onDamaged.Creature.OnDamaged -= OnDamageRemoveRenvoi;
    }
  }
}
