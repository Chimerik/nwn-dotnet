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
    public readonly byte icon;
    public readonly string description;
    public readonly byte grade;
    public readonly decimal weight;
    public string name { get => $"{type.ToDescription()} - Matéria {grade}"; }
    public string iconString { get => $"iit_midmisc_{icon.ToString().PadLeft(3, '0')}"; }
    public int quantity { get; set; }

    public CraftResource(ResourceType type, string description, byte icon, byte grade, decimal weight)
    {
      this.type = type;
      this.description = description;
      this.icon = icon;
      this.grade = grade;
      this.weight = weight;
    }
    public CraftResource(CraftResource resourceBase, int quantity)
    {
      this.type = resourceBase.type;
      this.description = resourceBase.description;
      this.icon = resourceBase.icon;
      this.grade = resourceBase.grade;
      this.weight = resourceBase.weight;
      this.quantity = quantity;
    }
    public class SerializableCraftResource
    {
      public readonly ResourceType type;
      public readonly byte grade;
      public readonly int quantity;

      public SerializableCraftResource()
      {

      }
      public SerializableCraftResource(CraftResource resourceBase)
      {
        type = resourceBase.type;
        grade = resourceBase.grade;
        quantity = resourceBase.quantity;
      }
    }
  }
}
