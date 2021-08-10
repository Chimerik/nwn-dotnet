using System;

namespace NWN.Systems
{
  public class PlayerJournal
  {
    public DateTime? craftJobCountDown { get; set; }
    public PlayerJournal()
    {
      craftJobCountDown = null;
    }
  }
}
