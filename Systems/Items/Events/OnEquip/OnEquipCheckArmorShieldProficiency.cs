using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipCheckArmorShieldProficiency(OnItemEquip onEquip)
    {
      NwCreature oPC = onEquip.EquippedBy;
      NwItem oItem = onEquip.Item;

      if (oPC is null || oItem is null)
        return;

      switch (oItem.BaseItem.ItemType)
      {
        case BaseItemType.SmallShield:
        case BaseItemType.LargeShield:
        case BaseItemType.TowerShield:
        case BaseItemType.Armor:

          List<Feat> proficenciesRequirements = ItemUtils.GetItemProficiencies(oItem.BaseItem.ItemType, oItem.BaseACValue);

          if (proficenciesRequirements.Count < 1)
            return;

          if (proficenciesRequirements.Contains(Feat.ArmorProficiencyHeavy) && oPC.GetAbilityScore(Ability.Strength) < 15)
          {
            oPC.ApplyEffect(EffectDuration.Permanent, slow);
            oPC.OnHeartbeat -= OnHeartbeatCheckHeavyArmorSlow;
            oPC.OnHeartbeat += OnHeartbeatCheckHeavyArmorSlow;
          }

          foreach (Feat requiredProficiency in proficenciesRequirements)
            if (oPC.KnowsFeat(requiredProficiency))
              return;

          ApplyShieldArmorDisadvantageToPlayer(oPC);
          oPC.LoginPlayer.SendServerMessage($"Vous ne maîtrisez pas le port de {StringUtils.ToWhitecolor(oItem.Name)}. Malus appliqué.", ColorConstants.Red);

          return;
      }
    }
    public static void ApplyShieldArmorDisadvantageToPlayer(NwCreature playerCreature)
    {
      playerCreature.OnSpellAction += SpellSystem.NoArmorShieldProficiencyOnSpellInput;
      playerCreature.ApplyEffect(EffectDuration.Permanent, shieldArmorDisadvantageEffect);
    }
  }
}
