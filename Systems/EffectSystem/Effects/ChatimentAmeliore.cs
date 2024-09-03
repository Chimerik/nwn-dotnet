using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ChatimentAmelioreEffectTag = "_CHATIMENT_AMELIORE_EFFECT";
    public static Effect ChatimentAmeliore(NwCreature creature)
    {
      creature.OnItemEquip -= ItemSystem.OnEquipChatimentAmeliore;
      creature.OnItemEquip += ItemSystem.OnEquipChatimentAmeliore;

      Effect damage = Effect.DamageIncrease((int)DamageBonus.Plus1d8, DamageType.Divine);
      damage.ShowIcon = false;

      Effect eff = Effect.LinkEffects(Effect.Icon((EffectIcon)182), damage);
      eff.Tag = ChatimentAmelioreEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      return eff;
    }
  }
}
