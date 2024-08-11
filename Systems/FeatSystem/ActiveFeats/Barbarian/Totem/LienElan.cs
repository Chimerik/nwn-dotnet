using System.Linq;
using System.Numerics;
using Anvil.API;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void LienElan(NwCreature caster)
    {
      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.BarbarianRageEffectTag))
      {
        caster.LoginPlayer?.EnterTargetMode(SelectStampedeTarget, Config.LocationTargetMode(40, SpellTargetingShape.Rect, new Vector2() { X = 2, Y = 1 }, SpellTargetingFlags.HarmsEnemies | SpellTargetingFlags.HelpsAllies | SpellTargetingFlags.OriginOnSelf));
        caster.LoginPlayer?.SendServerMessage("Veuillez choisir une cible à plus de 9 m", ColorConstants.Orange);
      }
      else
      {
        caster.LoginPlayer?.SendServerMessage("Vous devez être sous l'effet de Rage pour utiliser cette attaque", ColorConstants.Red);
        caster.SetFeatRemainingUses((Feat)CustomSkill.TotemLienElan, 0);
      }
    }
    private static async void SelectStampedeTarget(Anvil.API.Events.ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled)
        return;

      NwCreature caster = selection.Player.LoginCreature;

      if(Vector3.DistanceSquared(selection.TargetPosition, caster.Position) < 80)
      {
        selection.Player.SendServerMessage("La cible sélectionnée doit se situer à plus de 9 m", ColorConstants.Red);
        return;
      }

      EffectUtils.RemoveTaggedEffect(caster, EffectSystem.SprintEffectTag);
      caster.SetFeatRemainingUses((Feat)CustomSkill.TotemLienElan, 0);

      Location target = Location.Create(selection.Player.LoginCreature.Area, selection.TargetPosition, selection.Player.LoginCreature.Rotation);

      _ = caster.ClearActionQueue();
      _ = caster.AddActionToQueue(() => _ = caster.ActionForceMoveTo(target, true));

      NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.totemLienElanAura, NwTimeSpan.FromRounds(1)));
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Lien de l'Elan", ColorConstants.Red, true, true);

      await NwTask.NextFrame();
      caster.Commandable = false;


      await NwTask.WaitUntil(() => caster is null || caster.CurrentAction != Action.MoveToPoint);
      
      if(caster is not null)
        EffectUtils.RemoveTaggedEffect(caster, EffectSystem.LienTotemElanAuraEffectTag);
    }
  }
}
