using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnUnEquipCheckArmorShieldProficiency(OnItemUnequip onUnequip)
    {
      NwCreature oPC = onUnequip.Creature;
      NwItem oItem = onUnequip.Item;

      if (oPC is null || oItem is null)
        return;

      // TODO : Il faudra penser à faire le check de suppression d'avantage si le perso apprend la maîtrise au cas où il a une armure ou bouclier d'équipé
      switch (oItem.BaseItem.ItemType) 
      {
        case BaseItemType.SmallShield:
        case BaseItemType.LargeShield:
        case BaseItemType.TowerShield:

          if (!oPC.KnowsFeat(Feat.ShieldProficiency))
            RemoveShieldArmorDisadvantageToPlayer(oPC);

          break;

        case BaseItemType.Armor:

          if (oItem.BaseACValue > 0 && oItem.BaseACValue < 3)
            if (!oPC.KnowsFeat(Feat.ArmorProficiencyLight))
              RemoveShieldArmorDisadvantageToPlayer(oPC);

          if (oItem.BaseACValue > 2 && oItem.BaseACValue < 6)
            if (!oPC.KnowsFeat(Feat.ArmorProficiencyMedium))
              RemoveShieldArmorDisadvantageToPlayer(oPC);

          if (oItem.BaseACValue > 6)
          {
            if (!oPC.KnowsFeat(Feat.ArmorProficiencyHeavy))
              RemoveShieldArmorDisadvantageToPlayer(oPC);

            if (oPC.GetAbilityScore(Ability.Strength) < 15)
            {
              foreach (var eff in oPC.ActiveEffects)
                if (eff.Tag == EffectSystem.heavyArmorSlowEffectTag)
                  oPC.RemoveEffect(eff);

              oPC.OnHeartbeat -= OnHeartbeatCheckHeavyArmorSlow;
            }
          }

          break;
      }
    }
    public static void RemoveShieldArmorDisadvantageToPlayer(NwCreature playerCreature)
    {
      if (!PlayerSystem.Players.TryGetValue(playerCreature, out PlayerSystem.Player player))
        return;

      playerCreature.OnSpellAction -= SpellSystem.NoArmorShieldProficiencyOnSpellInput;

      foreach(var eff in playerCreature.ActiveEffects)
        if(eff.Tag == EffectSystem.ShieldArmorDisadvantageEffectTag)
          playerCreature.RemoveEffect(eff);
    }
  }
}
