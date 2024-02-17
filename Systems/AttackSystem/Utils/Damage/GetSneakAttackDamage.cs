using System;
using Anvil.API;
using Anvil.API.Events;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetSneakAttackDamage(CNWSCreature attacker, CNWSCreature target, CNWSItem attackWeapon, CNWSCombatAttackData attackData, CNWSCombatRound round)
    {
      if (target is null) // L'attaque sournoise ne peut toucher que des créatures (les objets sont exclus)
        return 0;

      int sneakLevel = (int)Math.Ceiling((double)attacker.m_pStats.GetNumLevelsOfClass((byte)Native.API.ClassType.Rogue) / 2);

      // Limitation à une sournoise par round
      if (sneakLevel < 1 || attacker.m_ScriptVars.GetInt(CreatureUtils.SneakAttackCooldownVariableExo).ToBool()) 
        return 0;

      // Nécessite une arme à distance ou une arme de finesse
      if(!attackData.m_bRangedAttack.ToBool() && !attackWeapon.m_ScriptVars.GetInt(ItemConfig.isFinesseWeaponCExoVariable).ToBool())
        return 0;

      int advantage = attacker.m_ScriptVars.GetInt($"_ADVANTAGE_ATTACK_{round.m_nCurrentAttack}".ToExoString());

      // Pas de sournoise si désavantage
      if (advantage < 0)
        return 0;

      // Si pas d'avantage, sournoise possible si un autre ennemi de la cible se trouve à portée de mêlée
      if (advantage == 0)
      {
        var distraction = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(target.GetNearestEnemy(3, attacker.m_idSelf, 1, 1));

        if (distraction is null || distraction.m_idSelf == 0x7F000000) // OBJECT_INVALID
          return 0;
      }

      int sneakRoll = NwRandom.Roll(Utils.random, 6, sneakLevel);

      attacker.m_ScriptVars.SetInt(CreatureUtils.SneakAttackCooldownVariableExo, 1);
      attacker.m_ScriptVars.DestroyInt($"_ADVANTAGE_ATTACK_{round.m_nCurrentAttack}".ToExoString());

      BroadcastNativeServerMessage($"Sournoise {sneakLevel}d{6} => {sneakRoll}", attacker);
      LogUtils.LogMessage($"Sournoise - {sneakLevel}d{6} => {sneakRoll} - Total : {sneakRoll}", LogUtils.LogType.Combat);

      return sneakRoll;
    }
  }
}
