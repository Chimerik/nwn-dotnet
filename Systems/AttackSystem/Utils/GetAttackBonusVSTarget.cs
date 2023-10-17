

using Anvil.API;
using NWN.Native.API;
using System.Linq;
using System.Numerics;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetAttackBonusVSTarget(CNWSCreature attacker, CNWSCreature target, int rangedAttack)
    {
      int attackModifier = 0;
      attackModifier += CreatureUtils.OverrideSizeAttackAndACBonus(attacker); // On compense le bonus/malus de taille du jeu de base

      if (target is null)
        return attacker.m_pStats.GetAttackModifierVersus() + attackModifier;

      attackModifier += attacker.m_pStats.GetAttackModifierVersus(target); // Bonus d'attaque du jeu de base
      attackModifier -= target.GetInvisible(attacker) > 0 || target?.GetBlind() > 0 ? 2 : 0; // On compense le bonus de +2 des attaquants invisibles (ou cible aveuglée) du jeu de base
      attackModifier -= target.GetFlanked(attacker) > 0 ? 2 : 0; // On compense le bonus du jeu de base si la cible est flanquée
      attackModifier -= target.m_appliedEffects.Any(e => (EffectTrueType)e.m_nType == EffectTrueType.SetState && e.GetInteger(0) == 6) ? 2 : 0; // e.GetInteger(0) == 6 => Stunned state : on compense le bonus du jeu de base si la créature est stun
      attackModifier += (attacker.GetVisibleListElement(target.m_idSelf) is null // On compense le malus du jeu de base si l'attaquant ne voit pas sa cible
        || attacker.GetVisibleListElement(target.m_idSelf).m_bSeen < 1
        || attacker.GetVisibleListElement(target.m_idSelf).m_bInvisible > 0)
        ? 4 : 0;

      attackModifier += rangedAttack > 0 && target.m_appliedEffects.Any(e => (EffectTrueType)e.m_nType == EffectTrueType.Knockdown) ? 4 : -4; // On compense le bonus/malus du jeu de base si la cible est au sol

      if (rangedAttack > 0)
      {
        attackModifier += Core.NWNX.CreaturePlugin.GetMovementType(target.m_idSelf) > 0 ? 2 : 0; // On compense le malus du jeu de base si la cible est en mouvement;
        attackModifier += attacker.m_oidArea == target.m_oidArea && Vector3.DistanceSquared(attacker.m_vPosition.ToManagedVector(), target.m_vPosition.ToManagedVector()) < 5 ? 4 : 0; // on compense le malus du jeu de base si la cible d'une attaque à distance est en mêlée avec l'attaquant
      }

      //LogUtils.LogMessage($"hit distance: {attacker.m_pcPathfindInformation.dis.m_fHitDistance}", LogUtils.LogType.Combat);

      return attackModifier;
    }
  }
}
