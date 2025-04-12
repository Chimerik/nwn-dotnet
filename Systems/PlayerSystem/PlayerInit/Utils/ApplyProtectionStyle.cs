using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyProtectionStyle()
      {
        if (learnableSkills.TryGetValue(CustomSkill.FightingStyleProtection, out var protection) && protection.currentLevel > 0)
        {
          oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipApplyProtectionStyle;
          oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipRemoveProtectionStyle;
          oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipApplyProtectionStyle;
          oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipRemoveProtectionStyle;

          switch (oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand)?.BaseItem.ItemType)
          {
            case BaseItemType.SmallShield:
            case BaseItemType.LargeShield:
            case BaseItemType.TowerShield:
              if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.ProtectionStyleEffectTag))
                NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.protectionStyleAura));
              break;
          }
        }
      }
    }
  }
}
