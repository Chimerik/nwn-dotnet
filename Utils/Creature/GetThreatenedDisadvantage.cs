using Anvil.API;
using NWN.Native.API;
using NWN.Systems;
using System.Linq;
using Feat = NWN.Native.API.Feat;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetThreatenedDisadvantage(CNWSCreature attacker, CNWSItem attackWeapon = null)
    {
      var isCrossbowAttack = false;

      if (attackWeapon is not null)
      {
        isCrossbowAttack = (BaseItemType)attackWeapon?.m_nBaseItem switch
        {
          BaseItemType.LightCrossbow or BaseItemType.HeavyCrossbow or BaseItemType.Shuriken => true,
          _ => false,
        };
      }

      if((isCrossbowAttack && attacker.m_pStats.HasFeat((ushort)Feat.PointBlankShot).ToBool())
        || !attacker.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.threatenedEffectExoTag).ToBool()))
      {
        return false;
      }
      else
      {
        LogUtils.LogMessage($"Désavantage - Attaque à distance en étant menacé en mêlée", LogUtils.LogType.Combat);
        return true;
      }


      /*foreach (var gameObject in attacker.GetArea().m_aGameObjects)
      {
        if (gameObject == attacker.m_idSelf)
          continue;

        CNWSCreature creature = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(gameObject);

        if (creature is null || creature.m_nCurrentHitPoints < 1 || creature.m_ScriptVars.GetInt(spellCastVariable) > 0
          || Vector3.DistanceSquared(attacker.m_vPosition.ToManagedVector(), creature.m_vPosition.ToManagedVector()) > 6)
          continue;

        CNWSItem creatureWeapon = creature.m_pInventory.GetItemInSlot((uint)Native.API.InventorySlot.RightHand);

        if (creatureWeapon is not null && NwBaseItem.FromItemId((int)creatureWeapon.m_nBaseItem).IsRangedWeapon)
          continue;

        if (creature.m_bPlayerCharacter > 0)
        {
          if (creature.GetPVPReputation(attacker.m_idSelf) > 49)
            continue;
        }
        else        
        {
          if(attacker.GetCreatureReputation(creature.m_idSelf, creature.m_nOriginalFactionId, 0) > 49)
          continue;
        }

        if (creature.CheckAttackClearLineToTarget(attacker.m_idSelf, attacker.m_vPosition) < 1)
          continue;

        if (creature.m_appliedEffects.Any(e => (EffectTrueType)e.m_nType == EffectTrueType.Knockdown
        || (EffectTrueType)e.m_nType == EffectTrueType.Petrify || (EffectTrueType)e.m_nType == EffectTrueType.Sanctuary
        || (EffectTrueType)e.m_nType == EffectTrueType.Timestop || (EffectTrueType)e.m_nType == EffectTrueType.Pacify 
        || ((EffectTrueType)e.m_nType == EffectTrueType.SetState && (e.GetInteger(0) == 6 || e.GetInteger(0) == 1 || e.GetInteger(0) == 2 || e.GetInteger(0) == 3 || e.GetInteger(0) == 7 || e.GetInteger(0) == 8 || e.GetInteger(0) == 9))))
          continue;

        return 1;
      }

      return 0;*/
    }
  }
}
