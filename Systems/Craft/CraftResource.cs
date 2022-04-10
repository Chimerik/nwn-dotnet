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
    public readonly int reprocessingLearnable;
    public readonly int reprocessingEfficiencyLearnable;
    public readonly int reprocessingGradeLearnable;
    public readonly int reprocessingExpertiseLearnable;

    public CraftResource(ResourceType type, string description, byte icon, byte grade, decimal weight)
    {
      this.type = type;
      this.description = description;
      this.icon = icon;
      this.grade = grade;
      this.weight = weight;

      switch (type)
      {
        case ResourceType.Ore:
          reprocessingLearnable = CustomSkill.ReprocessingOre;
          reprocessingEfficiencyLearnable = CustomSkill.ReprocessingOreEfficiency;
          reprocessingExpertiseLearnable = CustomSkill.ReprocessingOreExpertise;
          break;
        case ResourceType.Wood:
          reprocessingLearnable = CustomSkill.ReprocessingWood;
          reprocessingEfficiencyLearnable = CustomSkill.ReprocessingWoodEfficiency;
          reprocessingExpertiseLearnable = CustomSkill.ReprocessingWoodExpertise;
          break;
        case ResourceType.Pelt:
          reprocessingLearnable = CustomSkill.ReprocessingPelt;
          reprocessingEfficiencyLearnable = CustomSkill.ReprocessingPeltEfficiency;
          reprocessingExpertiseLearnable = CustomSkill.ReprocessingPeltExpertise;
          break;
      }

      switch(grade)
      {
        case 1:
          reprocessingGradeLearnable = CustomSkill.ReprocessingGrade1Expertise;
          break;
        case 2:
          reprocessingGradeLearnable = CustomSkill.ReprocessingGrade2Expertise;
          break;
        case 3:
          reprocessingGradeLearnable = CustomSkill.ReprocessingGrade3Expertise;
          break;
        case 4:
          reprocessingGradeLearnable = CustomSkill.ReprocessingGrade4Expertise;
          break;
        case 5:
          reprocessingGradeLearnable = CustomSkill.ReprocessingGrade5Expertise;
          break;
        case 6:
          reprocessingGradeLearnable = CustomSkill.ReprocessingGrade6Expertise;
          break;
        case 7:
          reprocessingGradeLearnable = CustomSkill.ReprocessingGrade7Expertise;
          break;
        case 8:
          reprocessingGradeLearnable = CustomSkill.ReprocessingGrade8Expertise;
          break;
      }
    }
    public CraftResource(CraftResource resourceBase, int quantity)
    {
      this.type = resourceBase.type;
      this.description = resourceBase.description;
      this.icon = resourceBase.icon;
      this.grade = resourceBase.grade;
      this.weight = resourceBase.weight;
      this.quantity = quantity;
      this.reprocessingLearnable = resourceBase.reprocessingLearnable;
      this.reprocessingEfficiencyLearnable = resourceBase.reprocessingEfficiencyLearnable;
      this.reprocessingGradeLearnable = resourceBase.reprocessingGradeLearnable;
    }
    public class SerializableCraftResource
    {
      public int type { get; set; }
      public byte grade { get; set; }
      public int quantity { get; set; }

      public SerializableCraftResource()
      {

      }
      public SerializableCraftResource(CraftResource resourceBase)
      {
        type = (int)resourceBase.type;
        grade = resourceBase.grade;
        quantity = resourceBase.quantity;
      }
    }
  }
}
