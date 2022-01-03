using System.ComponentModel;

namespace NWN.Systems
{
  public enum ResourceType
  {
    Invalid = -1,
    [Description("Minerai")]
    Ore = 1,
    [Description("Lingot")]
    Ingot = 2,
    [Description("Bûche")]
    Wood = 3,
    [Description("Planche")]
    Plank = 4,
    [Description("Peau")]
    Pelt = 5,
    [Description("Cuir")]
    Leather = 6,
    [Description("Plante")]
    Plant = 7,
    [Description("Poudre")]
    Powder = 8,
  }
  public class CraftResource
  {
    public readonly ResourceType type;
    public readonly string icon;
    public readonly byte grade;
    public string name { get => type.ToDescription(); }
    public int quantity { get; set; }

    public CraftResource(ResourceType type, string icon, byte grade, int quantity = 0)
    {
      this.type = type;
      this.icon = icon;
      this.grade = grade;
      this.quantity = quantity;
    }
  }
}
