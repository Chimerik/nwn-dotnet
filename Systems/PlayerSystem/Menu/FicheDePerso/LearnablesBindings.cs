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
          Learnable learnable = player.GetActiveLearnable();

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

          if (player.craftJob is not null)
          {
            jobIcon.SetBindValue(player.oid, nuiToken.Token, player.craftJob.icon);
            jobName.SetBindValue(player.oid, nuiToken.Token, player.craftJob.type.ToDescription());

            if (player.craftJob.progressLastCalculation.HasValue)
            {
              player.craftJob.remainingTime -= (DateTime.Now - player.craftJob.progressLastCalculation.Value).TotalSeconds;
              player.craftJob.progressLastCalculation = null;
            }

            if (player.oid.LoginCreature.Area == null || player.oid.LoginCreature.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value > 0)
              jobETA.SetBindValue(player.oid, nuiToken.Token, "En pause (Hors Cité)");
            else
              jobETA.SetBindValue(player.oid, nuiToken.Token, player.craftJob.GetReadableJobCompletionTime());
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
