using System.ComponentModel;

namespace NWN.Systems
{
  public enum ResourceType
  {
    Invalid = -1,
    [Description("Influx_brut")]
    InfluxBrut = 1,
    [Description("Influx_raffiné")]
    InfluxRaffine = 2,
    /*[Description("Lingot")]
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
    Powder = 8,*/
  }
  public class CraftResource
  {
    public readonly ResourceType type;
    public readonly byte icon;
    public readonly string description;
    public readonly decimal weight;
    public string name { get => $"{type.ToDescription()}"; }
    public string iconString { get => $"iit_midmisc_{icon.ToString().PadLeft(3, '0')}"; }
    public int quantity { get; set; }

    public CraftResource(ResourceType type, string description, byte icon, decimal weight)
    {
      this.type = type;
      this.description = description;
      this.icon = icon;
      this.weight = weight;
    }
    public CraftResource(CraftResource resourceBase, int quantity)
    {
      this.type = resourceBase.type;
      this.description = resourceBase.description;
      this.icon = resourceBase.icon;
      this.weight = resourceBase.weight;
      this.quantity = quantity;
    }
    public class SerializableCraftResource
    {
      public int type { get; set; }
      public int quantity { get; set; }

      public SerializableCraftResource()
      {

      }
      public SerializableCraftResource(CraftResource resourceBase)
      {
        type = (int)resourceBase.type;
        quantity = resourceBase.quantity;
      }
    }
  }
}
