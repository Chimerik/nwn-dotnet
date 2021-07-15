using NLog;

using Anvil.API;
using NWN.Native.API;
using Anvil.Services;

namespace NWN.Systems
{
  [ServiceBinding(typeof(NativeAttackHook))]
  public unsafe class NativeAttackHook
  {
    private static readonly Logger Log = LogManager.GetCurrentClassLogger();

    private delegate int GetDamageRollHook(void* thisPtr, void* pTarget, int bOffHand, int bCritical, int bSneakAttack, int bDeathAttack, int bForceMax);
    private delegate void ResolveAttackRollHook(void* pCreature, void* pTarget);

    //private readonly FunctionHook<GetDamageRollHook> getDamageRollHook;
    private readonly FunctionHook<ResolveAttackRollHook> resolveAttackRollHook;

    public NativeAttackHook(HookService hookService)
    {
      //getDamageRollHook = hookService.RequestHook<GetDamageRollHook>(OnGetDamageRoll, FunctionsLinux._ZN17CNWSCreatureStats13GetDamageRollEP10CNWSObjectiiiii, HookOrder.Early);
      resolveAttackRollHook = hookService.RequestHook<ResolveAttackRollHook>(OnResolveAttackRoll, FunctionsLinux._ZN12CNWSCreature17ResolveAttackRollEP10CNWSObject, HookOrder.Early);
    }

    private void OnResolveAttackRoll(void* pCreature, void* pTarget)
    {
      CNWSCreature creature = CNWSCreature.FromPointer(pCreature);
      var targetObject = CNWSObject.FromPointer(pTarget);

      CNWSCombatRound combatRound = creature.m_pcCombatRound;
      CNWSCombatAttackData attackData = combatRound.GetAttack(combatRound.m_nCurrentAttack);

      if(attackData.m_nAttackResult == 0 || attackData.m_nAttackResult == 4)
        attackData.m_nAttackResult = 1;

      if (targetObject.m_nObjectType == (int)ObjectType.Creature)
      {
        CNWSCreature targetCreature = targetObject.AsNWSCreature();
        int skillBonusDodge = 0;

        if (PlayerSystem.Players.TryGetValue(targetObject.m_idSelf, out PlayerSystem.Player player) && player.learntCustomFeats.ContainsKey(CustomFeats.ImprovedDodge))
        {
          skillBonusDodge += 2 * SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ImprovedDodge, player.learntCustomFeats[CustomFeats.ImprovedDodge]);
        }

        if (targetCreature.m_pStats.HasFeat((ushort)10).ToBool())
          skillBonusDodge += 2;

        if (targetCreature.m_nCreatureSize < creature.m_nCreatureSize)
          skillBonusDodge += 5;

        int dodgeRoll = NwRandom.Roll(Utils.random, 100);
        if (dodgeRoll <= unchecked((sbyte)targetCreature.m_pStats.GetAbilityMod(1)) + skillBonusDodge - targetCreature.m_pStats.m_nArmorCheckPenalty - targetCreature.m_pStats.m_nShieldCheckPenalty)
          attackData.m_nAttackResult = 4;
      }
    }

    /*private int OnGetDamageRoll(void* thisPtr, void* pTarget, int bOffHand, int bCritical, int bSneakAttack, int bDeathAttack, int bForceMax)
    {
      var creatureStats = CNWSCreatureStats.FromPointer(thisPtr);
      var creature = CNWSCreature.FromPointer(creatureStats.m_pBaseCreature);
      var targetObject = CNWSObject.FromPointer(pTarget);
      var damageFlags = creatureStats.m_pBaseCreature.GetDamageFlags();

      var defense = 0f;
      var dmg = 0f;
      var attackAttribute = creatureStats.m_nStrengthBase < 10 ? 0 : creatureStats.m_nStrengthModifier;
      var damage = 9999;

      // Calculate attacker's DMG
      if (creature != null)
      {
        var weapon = bOffHand == 1
            ? creature.m_pInventory.GetItemInSlot((uint)EquipmentSlot.LeftHand)
            : creature.m_pInventory.GetItemInSlot((uint)EquipmentSlot.RightHand);

        // Nothing equipped - check gloves.
        if (weapon == null)
        {
          weapon = creature.m_pInventory.GetItemInSlot((uint)EquipmentSlot.Arms);
        }

        // Gloves not equipped. Check claws
        if (weapon == null)
        {
          weapon = bOffHand == 1
              ? creature.m_pInventory.GetItemInSlot((uint)EquipmentSlot.CreatureWeaponLeft)
              : creature.m_pInventory.GetItemInSlot((uint)EquipmentSlot.CreatureWeaponRight);
        }

        if (weapon != null)
        {
          // Iterate over properties and take the highest DMG rating.
          for (var index = 0; index < weapon.m_lstPassiveProperties.array_size; index++)
          {
            var ip = weapon.GetPassiveProperty(index);
            if (ip != null && ip.m_nPropertyName == (ushort)ItemPropertyType.DMG)
            {
              if (ip.m_nCostTableValue > dmg)
              {
                dmg = Combat.GetDMGValueFromItemPropertyCostTableValue(ip.m_nCostTableValue);
              }
            }
          }

          // Ranged weapons use Perception (NWN's DEX)
          // All others use Might (NWN's STR)
          if (weapon.m_nBaseItem == (uint)BaseItem.HeavyCrossbow ||
              weapon.m_nBaseItem == (uint)BaseItem.LightCrossbow ||
              weapon.m_nBaseItem == (uint)BaseItem.Shortbow ||
              weapon.m_nBaseItem == (uint)BaseItem.Longbow ||
              weapon.m_nBaseItem == (uint)BaseItem.Sling)
          {
            attackAttribute = creatureStats.m_nDexterityBase < 10 ? 0 : creatureStats.m_nDexterityModifier;
          }
        }
      }

      // Safety check - DMG minimum is 0.5
      if (dmg < 0.5f)
      {
        dmg = 0.5f;
      }

      // Calculate total defense on the target.
      if (targetObject != null && targetObject.m_nObjectType == (int)ObjectType.Creature)
      {
        var target = CNWSCreature.FromPointer(pTarget);
        var damagePower = creatureStats.m_pBaseCreature.CalculateDamagePower(target, bOffHand);
        float vitality = target.m_pStats.m_nConstitutionModifier;

        foreach (var slotItemId in target.m_pInventory.m_pEquipSlot)
        {
          if (slotItemId != NWNXLib.OBJECT_INVALID)
          {
            var item = NWNXLib.AppManager().m_pServerExoApp.GetItemByGameObjectID(slotItemId);
            for (var index = 0; index < item.m_lstPassiveProperties.array_size; index++)
            {
              var ip = item.GetPassiveProperty(index);
              if (ip != null && ip.m_nPropertyName == (ushort)ItemPropertyType.Defense)
              {
                defense += ip.m_nCostTableValue;
              }
            }
          }
        }

        damage = Combat.CalculateDamage(dmg, attackAttribute, defense, vitality, bCritical == 1);

        // Plot target - zero damage
        if (target.m_bPlotObject == 1)
        {
          damage = 0;
        }

        // Apply NWN mechanics to damage reduction
        damage = target.DoDamageImmunity(creature, damage, damageFlags, 0, 1);
        damage = target.DoDamageResistance(creature, damage, damageFlags, 0, 1, 1);
        damage = target.DoDamageReduction(creature, damage, damagePower, 0, 1);
        if (damage < 0)
          damage = 0;
      }

      return damage;
    }*/
  }
}
