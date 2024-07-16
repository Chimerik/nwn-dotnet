using Anvil.API.Events;
using Anvil.API;
using NWN.Core;

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

          if (onAttack.Target is not NwCreature targetCreature)
            return;

          EffectUtils.RemoveTaggedEffect(onAttack.Attacker, EffectSystem.ChatimentDivinEffectTag);

          StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, $"{onAttack.Attacker.Name.ColorString(ColorConstants.Cyan)} " +
            $"utilise Châtiment Divin sur {targetCreature.Name.ColorString(ColorConstants.Cyan)}", StringUtils.gold, true, true);

          if (Utils.In(targetCreature.Race.RacialType, RacialType.Undead, RacialType.Outsider))
          {
            LogUtils.LogMessage($"Châtiment Divin - Cible mort-vivant ou extérieur : +1d8", LogUtils.LogType.Combat);
            NWScript.AssignCommand(onAttack.Attacker, () => targetCreature.ApplyEffect(EffectDuration.Instant, Effect.Damage(NwRandom.Roll(Utils.random, 8), DamageType.Divine)));
          }

          await NwTask.NextFrame();
          onAttack.Attacker.OnCreatureAttack -= OnAttackChatimentDivin;

          break;
      }
    }
  }
}
