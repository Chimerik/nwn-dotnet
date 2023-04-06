namespace NWN.Systems
{
  public class CooldownPosition
  {
    public int xPos { get; set; }
    public int spacing { get; set; }


    public CooldownPosition(int xPos, int spacing)
    {
      this.xPos = xPos;
      this.spacing = spacing;
    }
    public CooldownPosition()
    {
      
    }
  }
}
