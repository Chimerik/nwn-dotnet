using System;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeJob()
      {
        if (craftJob is not null)
        {
          if ((craftJob.type == JobType.Mining || craftJob.type == JobType.WoodCutting 
            || craftJob.type == JobType.Pelting) && craftJob.progressLastCalculation.HasValue)
          {
            craftJob.remainingTime -= (DateTime.Now - craftJob.progressLastCalculation.Value).TotalSeconds;
            craftJob.progressLastCalculation = null;
          }
        }
      }
    }
  }
}
