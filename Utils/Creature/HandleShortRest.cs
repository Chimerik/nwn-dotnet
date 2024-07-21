using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void HandleShortRest(Player player)
    {
      player.oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.Heal(player.oid.LoginCreature.MaxHP / 2));
      player.oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingL));
      player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>(MeneurExaltantVariable).Delete();
      player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_ILLUSION_SEE_INVI_COOLDOWN").Delete();
      player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>(CreatureUtils.AnimalCompanionVariable).Delete();
      FighterUtils.RestoreManoeuvres(player.oid.LoginCreature);
      FighterUtils.RestoreTirArcanique(player.oid.LoginCreature);
      MonkUtils.RestoreKi(player.oid.LoginCreature);
      WizardUtils.RestaurationArcanique(player.oid.LoginCreature);
      WizardUtils.AbjurationSuperieure(player.oid.LoginCreature);
      FighterUtils.RestoreEldritchKnight(player.oid.LoginCreature);

      if (player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.SourceDinspiration))
        BardUtils.RestoreInspirationBardique(player.oid.LoginCreature);

      foreach (var skill in player.learnableSkills.Values.Where(l => l.restoreOnShortRest && l.currentLevel > 0))
      {
        byte nbCharge = 1;

        switch (skill.id)
        {
          case CustomSkill.VigueurNaine:
            player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>(VigueurNaineHDVariable).Value = player.oid.LoginCreature.Level;
            return;

          case CustomSkill.FighterSurge:
            if (player.oid.LoginCreature.Classes.Any(c => Utils.In(c.Class.ClassType, ClassType.Fighter, (ClassType)CustomClass.EldritchKnight) && c.Level > 16))
              nbCharge++;
            break;
        }

        player.oid.LoginCreature.SetFeatRemainingUses((Feat)skill.id, nbCharge);
      }

      BarbarianUtils.RestoreImplacableRage(player.oid.LoginCreature);
      player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_RAGE_IMPLACABLE_DD").Value = 10;
      player.oid.LoginCreature.GetObjectVariable<LocalVariableString>(CharmeInstinctifVariable).Delete();
    }
  }
}
