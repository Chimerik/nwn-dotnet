using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnConstitutionInfernale(PlayerSystem.Player player, int customSkillId)
    {
      byte rawConstitution = player.oid.LoginCreature.GetRawAbilityScore(Ability.Constitution);
      if (rawConstitution < 20)
        player.oid.LoginCreature.SetsRawAbilityScore(Ability.Constitution, (byte)(rawConstitution + 1));

      player.oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin).AddItemProperty(ItemProperty.DamageImmunity(IPDamageType.Cold, IPDamageImmunityType.Immunity50Pct), EffectDuration.Permanent);
      player.oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin).AddItemProperty(ItemProperty.DamageImmunity((IPDamageType)CustomItemPropertiesDamageType.Poison, IPDamageImmunityType.Immunity50Pct), EffectDuration.Permanent);

      return true;
    }
  }
}
