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

      Effect eff = Effect.Icon(CustomEffectIcon.ChatimentAmeliore);
      eff.Tag = ChatimentAmelioreEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      return eff;
    }
  }
}
