using System;

namespace NWN.Systems
{
  public class Endurance
  {
    public int maxHP { get; set; }
    public int maxCoreMana { get; set; }
    public int additionnalMana { get; set; }
    public int maxMana { get => maxCoreMana + additionnalMana; }
    public double currentMana { get; set; }
    public int regenerableHP { get; set; }
    public double regenerableMana { get; set; }
    public DateTime expirationDate { get; set; }

    public Endurance(int maxHP, int maxCoreMana, double currentMana, int regenerableHP, double regenerableMana, DateTime expirationDate)
    {
      this.maxHP = maxHP;
      this.currentMana = currentMana;
      this.maxCoreMana = maxCoreMana;
      this.additionnalMana = 0;
      this.regenerableHP = regenerableHP;
      this.regenerableMana = regenerableMana;
      this.expirationDate = expirationDate;
    }
    public Endurance()
    {
      maxHP = 10;
      maxCoreMana = 0;
      additionnalMana = 0;
      regenerableHP = 0;
      regenerableMana = 0;
      expirationDate = DateTime.Now;
    }
    public Endurance(SerializableEndurance serializedEndurance)
    {
      maxHP = serializedEndurance.maxHP;
      maxCoreMana = serializedEndurance.maxMana;
      additionnalMana = 0;
      currentMana = serializedEndurance.currentMana;
      regenerableHP = serializedEndurance.regenerableHP;
      regenerableMana = serializedEndurance.regenerableMana;
      expirationDate = serializedEndurance.expirationDate;
    }

    public class SerializableEndurance
    {
      public int maxHP { get; set; }
      public int maxMana { get; set; }
      public double currentMana { get; set; }
      public int regenerableHP { get; set; }
      public double regenerableMana { get; set; }
      public DateTime expirationDate { get; set; }

      public SerializableEndurance()
      {

      }
      public SerializableEndurance(Endurance enduranceBase)
      {
        maxHP = enduranceBase.maxHP;
        maxMana = enduranceBase.maxMana;
        currentMana = enduranceBase.currentMana;
        regenerableHP = enduranceBase.regenerableHP;
        regenerableMana = enduranceBase.regenerableMana;
        expirationDate = enduranceBase.expirationDate;
      }
    }
  }
}
