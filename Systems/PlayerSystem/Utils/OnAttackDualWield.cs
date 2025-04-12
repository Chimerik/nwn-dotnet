using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerUtils
  {
    public static void OnAttackDualWield(OnCreatureAttack onAttack)
    {
      if (onAttack.WeaponAttackType == WeaponAttackType.Offhand) // combat à deux armes
      {
        if (onAttack.Attacker.KnowsFeat((Feat)CustomSkill.FightingStyleDualWield))
        {
          onAttack.Attacker.GetObjectVariable<LocalVariableInt>("_ENTAILLE_BONUS_ATTACK").Delete();
        }
        else
        {
          var bonusAction = onAttack.Attacker.ActiveEffects.FirstOrDefault(e => e.Tag.ToString() == EffectSystem.BonusActionEffectTag);

          if (bonusAction is null) // L'attaque supplémentaire consomme l'action bonus du personnage
          {
            if (onAttack.Attacker.GetObjectVariable<LocalVariableInt>("_ENTAILLE_BONUS_ATTACK").HasNothing)
            {
              onAttack.AttackResult = AttackResult.MissChance; // Si pas d'action bonus dispo, auto miss

              onAttack.Attacker.LoginPlayer?.SendServerMessage("Main secondaire - Echec automatique - Pas d'action bonus disponible".ColorString(ColorConstants.Red));
              LogUtils.LogMessage("Main secondaire - Echec automatique - Pas d'action bonus disponible", LogUtils.LogType.Combat);
            }
            else
              onAttack.Attacker.GetObjectVariable<LocalVariableInt>("_ENTAILLE_BONUS_ATTACK").Delete();

            return;
          }
          else
          {
            onAttack.Attacker.RemoveEffect(bonusAction);
          }
        }
      }

    }
  }
}
