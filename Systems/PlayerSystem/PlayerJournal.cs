using System;

namespace NWN.Systems
{
  public class PlayerJournal
  {
    public DateTime? skillJobCountDown { get; set; }
    public DateTime? craftJobCountDown { get; set; }
    public PlayerJournal()
    {
      this.skillJobCountDown = null;
      this.craftJobCountDown = null;
    }
  }
}
