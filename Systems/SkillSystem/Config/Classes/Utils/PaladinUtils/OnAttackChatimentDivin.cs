using Anvil.API.Events;
using Anvil.API;
using NWN.Core;
using System.Linq;

namespace NWN.Systems
{
  public static partial class PaladinUtils
  {
    public static async void OnAttackChatimentDivin(OnCreatureAttack onAttack)
    {
      switch(onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          if (onAttack.Target is not NwCreature targetCreature || onAttack.Attacker.IsRangedWeaponEquipped)
            return;

          var chatiment = onAttack.Attacker.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.ChatimentDivinEffectTag);

          if (chatiment is not null)
          {
            int nbDice = 1 + chatiment.CasterLevel;

            if (Utils.In(targetCreature.Race.RacialType, RacialType.Undead, RacialType.Outsider))
            {
              LogUtils.LogMessage($"Châtiment Divin - Cible mort-vivant ou extérieur : +1d8", LogUtils.LogType.Combat);
              nbDice += 1;
            }

            nbDice *= onAttack.AttackResult == AttackResult.CriticalHit ? 2 : 1;

            string logString = "";
            int damage = 0;

            for(int i = 0; i < nbDice; i++)
            {
              int roll = NwRandom.Roll(Utils.random, 8);
              logString += $"{roll} + ";
              damage += roll;
            }

            LogUtils.LogMessage($"Châtiment Divin - {nbDice}d8 : {logString.Remove(logString.Length - 2)} = {damage}", LogUtils.LogType.Combat);

            EffectUtils.RemoveTaggedEffect(onAttack.Attacker, EffectSystem.ChatimentDivinEffectTag);

            StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, $"{onAttack.Attacker.Name.ColorString(ColorConstants.Cyan)} " +
              $"Châtiment Divin {targetCreature.Name.ColorString(ColorConstants.Cyan)}", StringUtils.gold, true, true);

            NWScript.AssignCommand(onAttack.Attacker, () => targetCreature.ApplyEffect(EffectDuration.Instant, Effect.Damage(damage, DamageType.Divine)));
          }

          await NwTask.NextFrame();
          onAttack.Attacker.OnCreatureAttack -= OnAttackChatimentDivin;

          break;
      }
    }
  }
}
