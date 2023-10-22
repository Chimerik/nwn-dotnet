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

          List<int> proficenciesRequirements = ItemUtils.GetItemProficiencies(oItem.BaseItem.ItemType, oItem.BaseACValue);

          if (proficenciesRequirements.Count < 1)
            return;

          if (oPC.Race.Id != CustomRace.Duergar && oPC.Race.Id != CustomRace.Dwarf && oPC.Race.Id != CustomRace.ShieldDwarf && oPC.Race.Id != CustomRace.GoldDwarf
            && proficenciesRequirements.Contains((int)Feat.ArmorProficiencyHeavy) && oPC.GetAbilityScore(Ability.Strength) < 15)
          {
            oPC.ApplyEffect(EffectDuration.Permanent, EffectSystem.heavyArmorSlow);
            oPC.OnHeartbeat -= OnHeartbeatCheckHeavyArmorSlow;
            oPC.OnHeartbeat += OnHeartbeatCheckHeavyArmorSlow;
          }

          foreach (int requiredProficiency in proficenciesRequirements)
            if (oPC.KnowsFeat(NwFeat.FromFeatId(requiredProficiency)))
              return;

          ApplyShieldArmorDisadvantageToPlayer(oPC);
          oPC.LoginPlayer.SendServerMessage($"Vous ne maîtrisez pas le port de {StringUtils.ToWhitecolor(oItem.Name)}. Malus appliqué.", ColorConstants.Red);

          return;
      }
    }
    public static void ApplyShieldArmorDisadvantageToPlayer(NwCreature playerCreature)
    {
      playerCreature.OnSpellAction += SpellSystem.NoArmorShieldProficiencyOnSpellInput;
      playerCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.shieldArmorDisadvantage);
    }
  }
}
