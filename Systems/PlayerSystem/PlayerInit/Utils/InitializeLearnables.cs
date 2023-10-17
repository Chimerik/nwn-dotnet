﻿using System;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeLearnables()
      {
        if (activeLearnable is not null && activeLearnable.active && activeLearnable.spLastCalculation.HasValue)
        {
          activeLearnable.acquiredPoints += (DateTime.Now - activeLearnable.spLastCalculation).Value.TotalSeconds * GetSkillPointsPerSecond(activeLearnable);
          if (!windows.ContainsKey("activeLearnable")) windows.Add("activeLearnable", new ActiveLearnableWindow(this));
          else ((ActiveLearnableWindow)windows["activeLearnable"]).CreateWindow();
        }
      }
    }
  }
}
