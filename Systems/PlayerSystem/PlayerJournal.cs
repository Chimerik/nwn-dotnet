using System;

namespace NWN.Systems
{
  public class PlayerJournal
  {
    public DateTime? rebootCountDown { get; set; }
    public DateTime? skillJobCountDown { get; set; }
    public DateTime? craftJobCountDown { get; set; }
    public PlayerJournal()
    {
      this.rebootCountDown = null;
      this.skillJobCountDown = null;
      this.craftJobCountDown = null;
    }
  }
}
