using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class Garden
  {
    public int id { get; set; }
    public uint oid { get; set; }
    public DateTime? datePlantage { get; set; }
    public string? type { get; set; }
    public string? tag { get; set; }

    public static Dictionary<int, Garden> Potagers = new Dictionary<int, Garden>();

    public Garden(uint nwobj, DateTime dateStored, string type, string tag)
    {
      this.oid = nwobj;
      this.id = NWScript.GetLocalInt(nwobj, "id");
      this.datePlantage = dateStored;
      this.type = type;
      this.tag = tag;
      Potagers[this.id] = this;
    }

    static Garden()
    {

    }

    public static void Init()
    {
      var oPotager = NWScript.GetObjectByTag("potager");
      int i = 0;
      while (NWScript.GetIsObjectValid(oPotager) == 1)
      {
        var sql = $"SELECT * FROM sql_potager WHERE id=@id LIMIT 1;";

        /*using (var connection = MySQL.GetConnection())
        {
          var potager = connection.QueryFirstOrDefault<Potager.Models.PotagerSql>(sql, new { id = NWScript.GetLocalInt(oPotager, "id") });
          if (potager != null)
          {
            var sPlanteType = potager.type;
            var iPlanteState = (DateTime.Now - potager.datePlantage).TotalSeconds;

            if (iPlanteState > 486000)
            {
              NWScript.SetName(oPotager, $"Plant de {sPlanteType} fâné");
              NWScript.SetLocalInt(oPotager, "_PLANTE_STATE", 3);
              ObjectPlugin.SetAppearance(oPotager, 571);
            }
            else if (iPlanteState > 324000)
            {
              NWScript.SetName(oPotager, $"Plant de {sPlanteType} prêt pour la récolte");
              NWScript.SetLocalInt(oPotager, "_PLANTE_STATE", 2);
              ObjectPlugin.SetAppearance(oPotager, 4340);
              NWScript.DelayCommand((float)(486000 - iPlanteState), () => FanerPlante(oPotager, sPlanteType));
            }
            else if (iPlanteState > 0)
            {
              NWScript.SetName(oPotager, $"Plant de {sPlanteType} en cours de pousse");
              NWScript.SetLocalInt(oPotager, "_PLANTE_STATE", 1);
              ObjectPlugin.SetAppearance(oPotager, 8791);
              NWScript.DelayCommand((float)(324000 - iPlanteState), () => PousserPlante(oPotager, sPlanteType));
            }

            Garden myPotager = new Garden(oPotager, potager.datePlantage, potager.type, potager.tag);
          }
          else
          {
            Garden myPotager = new Garden(oPotager, DateTime.MinValue, "", "");
          }
        }*/
        i += 1;
        oPotager = NWScript.GetObjectByTag("potager", i);
      }
    }

    private static void FanerPlante(uint oPotager, string sPlanteType)
    {
      if (NWScript.GetIsObjectValid(oPotager) == 1)
      {
        NWScript.SetName(oPotager, $"Plant de  {sPlanteType} fâné");
        ObjectPlugin.SetAppearance(oPotager, 517);
        NWScript.SetLocalInt(oPotager, "_PLANTE_STATE", 3);
      }
    }

    private static void PousserPlante(uint oPotager, string sPlanteType)
    {
      if (NWScript.GetIsObjectValid(oPotager) == 1)
      {
        NWScript.SetName(oPotager, $"Plant de {sPlanteType} prêt pour la récolte");
        ObjectPlugin.SetAppearance(oPotager, 4340);
        NWScript.SetLocalInt(oPotager, "_PLANTE_STATE", 2);
        NWScript.DelayCommand(162000.0f, () => FanerPlante(oPotager, sPlanteType));
      }
    }

    public void PlanterFruit(string FruitName, string FruitTag)
    {
      /*NWScript.SetName(this.oid, $"Plant de {FruitName} en cours de pousse");
      NWScript.SetLocalInt(this.oid, "_PLANTE_STATE", 1);
      ObjectPlugin.SetAppearance(this.oid, 8791);
      NWScript.DelayCommand(324000.0f, () => PousserPlante(this.oid, FruitName));

      var sql = $"REPLACE INTO sql_potager (id, type, datePlantage, tag) VALUES (@id, @type, @datePlantage, @tag);";

      using (var connection = MySQL.GetConnection())
      {
        connection.Execute(sql, new { id = this.id, datePlantage = DateTime.Now, type = FruitName, tag = FruitTag });
      }*/
    }
  }
}
