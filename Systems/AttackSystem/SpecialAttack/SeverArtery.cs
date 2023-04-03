using Anvil.API;
using System;
using Anvil.Services;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AttackSystem))]
  public partial class AttackSystem
  {
    public static async void SeverArtery(Player player, NwGameObject targetObject)
    {
      if (targetObject is not NwCreature targetCreature || targetCreature.Race.RacialType == RacialType.Construct || targetCreature.Race.RacialType == RacialType.Undead)
      {
        player.oid.SendServerMessage($"La cible {StringUtils.ToWhitecolor(targetObject.Name)} ne peut pas être affectée par le saignement", ColorConstants.Orange);
        return;
      }

      foreach (var eff in targetCreature.ActiveEffects)
        if (eff.Tag == "CUSTOM_EFFECT_BLEEDING")
          targetCreature.RemoveEffect(eff);

      int duration = 5 + (int)(player.learnableSkills[CustomSkill.SeverArtery].totalPoints * 1.5);

      player.oid.LoginCreature.DecrementRemainingFeatUses(NwFeat.FromFeatId(CustomSkill.SeverArtery - 10000));
      player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>($"_ADRENALINE_{CustomSkill.SeverArtery - 10000}").Value = 0;
      StringUtils.UpdateQuickbarPostring(player, CustomSkill.SeverArtery - 10000, 0);

      foreach (var feat in player.oid.LoginCreature.Feats)
      {
        if (feat.MaxLevel > 0 && feat.MaxLevel < 255 && player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>($"_ADRENALINE_{feat.Id}").Value > 0)
        {
          player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>($"_ADRENALINE_{feat.Id}").Value -= 25;

          if (player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>($"_ADRENALINE_{feat.Id}").Value < 0)
            player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>($"_ADRENALINE_{feat.Id}").Value = 0;

          player.oid.LoginCreature.DecrementRemainingFeatUses(feat, 0);

          StringUtils.UpdateQuickbarPostring(player, feat.Id, player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>($"_ADRENALINE_{feat.Id}").Value / 25);
        }
      }

      await NwTask.NextFrame();
      targetCreature.ApplyEffect(EffectDuration.Temporary, bleeding, TimeSpan.FromSeconds(duration));

      // TODO : mise à jour de l'affichage poststring
    }
  }
}
