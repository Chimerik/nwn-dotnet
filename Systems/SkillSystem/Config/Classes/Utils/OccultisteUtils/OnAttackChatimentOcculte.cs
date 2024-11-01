using Anvil.API.Events;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class OccultisteUtils
  {
    public static async void OnAttackChatimentOcculte(OnCreatureAttack onAttack)
    {
      NwCreature caster = onAttack.Attacker;

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          if (onAttack.Target is not NwCreature targetCreature)
            return;

          NwItem mainWeapon = caster.GetItemInSlot(InventorySlot.RightHand);
          NwItem secondaryWeapon = caster.GetItemInSlot(InventorySlot.LeftHand);

          if ((mainWeapon is not null && mainWeapon.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.PacteDeLaLameVariable).Value == caster)
            || (secondaryWeapon is not null && secondaryWeapon.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.PacteDeLaLameVariable).Value == caster))
          {
            var occultisteClass = caster.GetClassInfo((ClassType)CustomClass.Occultiste);
            byte remainingSlots = occultisteClass.GetRemainingSpellSlots(1);
            byte consumedSlots = (byte)(remainingSlots - 1);

            if (remainingSlots > 0)
            {
              for (byte i = 1; i < 10; i++)
                occultisteClass.SetRemainingSpellSlots(i, consumedSlots);

              caster.SetFeatRemainingUses((Feat)CustomSkill.ChatimentOcculte, consumedSlots);

              int nbDice = 1 + SpellUtils.GetMaxSpellSlotLevelKnown(caster, (ClassType)CustomClass.Occultiste);

              if (onAttack.AttackResult == AttackResult.CriticalHit)
                nbDice *= 2;

              string logString = "";
              int damage = 0;

              for (int i = 0; i < nbDice; i++)
              {
                int roll = NwRandom.Roll(Utils.random, 8);
                logString += $"{roll} + ";
                damage += roll;
              }

              LogUtils.LogMessage($"Châtiment Occulte - {nbDice}d8 : {logString.Remove(logString.Length - 2)} = {damage}", LogUtils.LogType.Combat);

              EffectUtils.RemoveTaggedEffect(onAttack.Attacker, EffectSystem.ChatimentOcculteEffectTag);

              StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, $"{onAttack.Attacker.Name.ColorString(ColorConstants.Cyan)} " +
                $"Châtiment Occulte {targetCreature.Name.ColorString(ColorConstants.Cyan)}", StringUtils.brightPurple, true, true);

              NWScript.AssignCommand(onAttack.Attacker, () => targetCreature.ApplyEffect(EffectDuration.Instant, Effect.Damage(damage, DamageType.Magical)));
              EffectSystem.ApplyKnockdown(targetCreature, CreatureSize.Huge, 2);
            }
          }

          await NwTask.NextFrame();
          onAttack.Attacker.OnCreatureAttack -= OnAttackChatimentOcculte;

          break;
      }
    }
  }
}
