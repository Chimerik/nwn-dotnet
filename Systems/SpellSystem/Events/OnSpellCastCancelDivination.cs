using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void OnSpellCastCancelDivination(OnSpellCast onSpellCast)
    {
      // Retiré temporairement pour permettre le test des capacités de devin. A remettre par la suite

      /*if (onSpellCast.Caster.IsLoginPlayerCharacter(out NwPlayer player) && onSpellCast.Spell.SpellSchool == SpellSchool.Divination)
      {
        if (onSpellCast.TargetObject is not null)
          LogUtils.LogMessage($"----- {onSpellCast.Caster.Name} lance {onSpellCast.Spell.Name.ToString()} (id {onSpellCast.Spell.Id}) sur {onSpellCast.TargetObject.Name} -----", LogUtils.LogType.Combat);
        else
          LogUtils.LogMessage($"----- {onSpellCast.Caster.Name} lance {onSpellCast.Spell.Name.ToString()} (id {onSpellCast.Spell.Id}) en mode AoE -----", LogUtils.LogType.Combat);

        onSpellCast.PreventSpellCast = true;
        player.ControlledCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));
        player.SendServerMessage("La Loi même vous empêche de faire appel à ce sort. Un cliquetis lointain de chaînes résonne dans votre esprit. Quelque chose vient de se mettre en mouvement ...", ColorConstants.Red);
        LogUtils.LogMessage($"{player.ControlledCreature.Name} ({player.PlayerName}) vient de lancer un sort de divination. Faire intervenir les apôtres pour jugement.", LogUtils.LogType.ModuleAdministration);
      }*/
    }
  }
}
