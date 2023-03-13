using System;

namespace NWN.Systems
{
  public class Endurance
  {
    public int maxHP { get; set; }
    public int regenerableHP { get; set; }
    public int regenerableMana { get; set; }
    public DateTime expirationDate { get; set; }

    public Endurance(int maxHP, int regenerableHP, int regenerableMana, DateTime expirationDate)
    {
      this.maxHP = maxHP;
      this.regenerableHP = regenerableHP;
      this.regenerableMana = regenerableMana;
      this.expirationDate = expirationDate;
    }
    public Endurance()
    {
      maxHP = 10;
      regenerableHP = 0;
      regenerableMana = 0;
      expirationDate = DateTime.Now;
    }
    public Endurance(SerializableEndurance serializedMail)
    {
      maxHP = serializedMail.maxHP;
      regenerableHP = serializedMail.regenerableHP;
      regenerableMana = serializedMail.regenerableMana;
      expirationDate = serializedMail.expirationDate;
    }

    public class SerializableEndurance
    {
      public int maxHP { get; set; }
      public int regenerableHP { get; set; }
      public int regenerableMana { get; set; }
      public DateTime expirationDate { get; set; }

      public SerializableEndurance()
      {

      }
      public SerializableEndurance(Endurance mailBase)
      {
        maxHP = mailBase.maxHP;
        regenerableHP = mailBase.regenerableHP;
        regenerableMana = mailBase.regenerableMana;
        expirationDate = mailBase.expirationDate;
      }
    }
  }
}
