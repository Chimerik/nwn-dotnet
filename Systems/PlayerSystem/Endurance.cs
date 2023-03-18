using System;

namespace NWN.Systems
{
  public class Endurance
  {
    public int maxHP { get; set; }
    public int maxMana { get; set; }
    public int regenerableHP { get; set; }
    public int regenerableMana { get; set; }
    public DateTime expirationDate { get; set; }

    public Endurance(int maxHP, int maxMana, int regenerableHP, int regenerableMana, DateTime expirationDate)
    {
      this.maxHP = maxHP;
      this.maxMana = maxMana;
      this.regenerableHP = regenerableHP;
      this.regenerableMana = regenerableMana;
      this.expirationDate = expirationDate;
    }
    public Endurance()
    {
      maxHP = 10;
      maxMana = 0;
      regenerableHP = 0;
      regenerableMana = 0;
      expirationDate = DateTime.Now;
    }
    public Endurance(SerializableEndurance serializedMail)
    {
      maxHP = serializedMail.maxHP;
      maxMana = serializedMail.maxMana;
      regenerableHP = serializedMail.regenerableHP;
      regenerableMana = serializedMail.regenerableMana;
      expirationDate = serializedMail.expirationDate;
    }

    public class SerializableEndurance
    {
      public int maxHP { get; set; }
      public int maxMana { get; set; }
      public int regenerableHP { get; set; }
      public int regenerableMana { get; set; }
      public DateTime expirationDate { get; set; }

      public SerializableEndurance()
      {

      }
      public SerializableEndurance(Endurance enduranceBase)
      {
        maxHP = enduranceBase.maxHP;
        maxMana = enduranceBase.maxMana;
        regenerableHP = enduranceBase.regenerableHP;
        regenerableMana = enduranceBase.regenerableMana;
        expirationDate = enduranceBase.expirationDate;
      }
    }
  }
}
