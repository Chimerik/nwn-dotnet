using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AttaqueMobileEffectTag = "_EFFECT_ATTAQUE_MOBILE";
    public static void ApplyAttaqueMobile(NwCreature caster)
    {
      if (caster.KnowsFeat((Feat)CustomSkill.ExpertiseAttaqueMobile))
      {
        var weapon = caster.GetItemInSlot(InventorySlot.RightHand);

        if (weapon is not null && ItemUtils.IsCreatureWeaponExpert(caster, weapon)
          && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Sling, BaseItemType.Dart, BaseItemType.ThrowingAxe, BaseItemType.Shuriken)
          && !caster.ActiveEffects.Any(e => e.Tag == AttaqueMobileEffectTag))
        {
          Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.AttaqueMobile), Effect.ModifyAttacks(1));
          eff.Tag = AttaqueMobileEffectTag;
          eff.SubType = EffectSubType.Supernatural;

          caster.ApplyEffect(EffectDuration.Temporary, eff, NwTimeSpan.FromRounds(1));
        }
      }
    }
  }
}
