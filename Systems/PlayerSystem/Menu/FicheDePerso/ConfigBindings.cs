using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public partial class FicheDePersoWindow : PlayerWindow
      {
        private void ConfigBindings()
        {
          chatimentLevelSelection.SetBindWatch(player.oid, nuiToken.Token, false);
          chatimentLevelSelection.SetBindValue(player.oid, nuiToken.Token, false);
          chatimentLevelSelection.SetBindWatch(player.oid, nuiToken.Token, true);
          instantLearn.SetBindWatch(player.oid, nuiToken.Token, false);
          walk.SetBindWatch(player.oid, nuiToken.Token, false);
          touch.SetBindWatch(player.oid, nuiToken.Token, false);
          rollDM.SetBindWatch(player.oid, nuiToken.Token, false);
          selectedRollDistance.SetBindWatch(player.oid, nuiToken.Token, false);

          if (player.oid.ControlledCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.ModeToucherEffectTag))
          {
            touchLabel.SetBindValue(player.oid, nuiToken.Token, "Mode toucher (avec collisions)");
            touch.SetBindValue(player.oid, nuiToken.Token, true);
          }
          else
          {
            touchLabel.SetBindValue(player.oid, nuiToken.Token, "Mode toucher (sans collisions)");
            touch.SetBindValue(player.oid, nuiToken.Token, false);
          }

          if (player.oid.ControlledCreature.AlwaysWalk)
          {
            walkLabel.SetBindValue(player.oid, nuiToken.Token, "Mode marche");
            walk.SetBindValue(player.oid, nuiToken.Token, true);
          }
          else
          {
            walkLabel.SetBindValue(player.oid, nuiToken.Token, "Mode course");
            walk.SetBindValue(player.oid, nuiToken.Token, false);
          }

          if (player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_INSTANT_LEARN").HasValue)
          {
            instantLearnLabel.SetBindValue(player.oid, nuiToken.Token, "Désactiver l'apprentissage instantanné");
            instantLearn.SetBindValue(player.oid, nuiToken.Token, true);
          }
          else
          {
            instantLearnLabel.SetBindValue(player.oid, nuiToken.Token, "Activer l'apprentissage instantanné");
            instantLearn.SetBindValue(player.oid, nuiToken.Token, false);
          }

          if (player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_DICE_ROLL_DISTANCE").HasValue)
          {
            selectedRollDistance.SetBindValue(player.oid, nuiToken.Token, player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_DICE_ROLL_DISTANCE").Value);
          }
          else
          {
            selectedRollDistance.SetBindValue(player.oid, nuiToken.Token, 40);
          }

          if (player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_DICE_ROLL_DM").HasValue)
          {
            rollDM.SetBindValue(player.oid, nuiToken.Token, true);
          }
          else
          {
            rollDM.SetBindValue(player.oid, nuiToken.Token, false);
          }

          touch.SetBindWatch(player.oid, nuiToken.Token, true);
          walk.SetBindWatch(player.oid, nuiToken.Token, true);
          rollDM.SetBindWatch(player.oid, nuiToken.Token, true);
          selectedRollDistance.SetBindWatch(player.oid, nuiToken.Token, true);
        }
      }
    }
  }
}
