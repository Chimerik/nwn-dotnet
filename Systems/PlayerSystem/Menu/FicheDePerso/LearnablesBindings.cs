using System;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public partial class FicheDePersoWindow : PlayerWindow
      {
        public void LearnablesBindings()
        {
          Learnable learnable = targetPlayer.GetActiveLearnable();

          learnableDrawListRect.SetBindValue(player.oid, nuiToken.Token, new(0, windowHeight / 48, windowWidth / 2, windowHeight / 48));
          jobDrawListRect.SetBindValue(player.oid, nuiToken.Token, new(0, windowHeight / 48, windowWidth / 2, windowHeight / 48));

          if (learnable is not null)
          {
            learnableETA.SetBindValue(player.oid, nuiToken.Token, learnable.GetReadableTimeSpanToNextLevel(player));
            learnableIcon.SetBindValue(player.oid, nuiToken.Token, learnable.icon);
            learnableName.SetBindValue(player.oid, nuiToken.Token, learnable.name);
            learnableLevel.SetBindValue(player.oid, nuiToken.Token, $"{learnable.currentLevel}/{learnable.maxLevel}");
          }
          else
          {
            learnableETA.SetBindValue(player.oid, nuiToken.Token, "-");
            learnableIcon.SetBindValue(player.oid, nuiToken.Token, "sheet_learnables");
            learnableName.SetBindValue(player.oid, nuiToken.Token, "En attente de sélection");
            learnableLevel.SetBindValue(player.oid, nuiToken.Token, $"-/-");
          }

          if (targetPlayer.craftJob is not null)
          {
            jobIcon.SetBindValue(player.oid, nuiToken.Token, targetPlayer.craftJob.icon);
            jobName.SetBindValue(player.oid, nuiToken.Token, targetPlayer.craftJob.type.ToDescription());

            if (targetPlayer.craftJob.progressLastCalculation.HasValue)
            {
              targetPlayer.craftJob.remainingTime -= (DateTime.Now - targetPlayer.craftJob.progressLastCalculation.Value).TotalSeconds;
              targetPlayer.craftJob.progressLastCalculation = null;
            }

            if (targetPlayer.oid.LoginCreature.Area == null || targetPlayer.oid.LoginCreature.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value > 0)
              jobETA.SetBindValue(player.oid, nuiToken.Token, "En pause (Hors Cité)");
            else
              jobETA.SetBindValue(player.oid, nuiToken.Token, targetPlayer.craftJob.GetReadableJobCompletionTime());
          }
          else
          {
            jobETA.SetBindValue(player.oid, nuiToken.Token, "");
            jobIcon.SetBindValue(player.oid, nuiToken.Token, "ir_dis_trap");
            jobName.SetBindValue(player.oid, nuiToken.Token, "En attente de sélection");
          }
        }
      }
    }
  }
}
