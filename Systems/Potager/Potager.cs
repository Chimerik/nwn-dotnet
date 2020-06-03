using NWN.MySQL;
using System;
using System.Collections.Generic;
using System.Text;

namespace NWN.Systems.Potager
{
    public partial class Potager : NWPlaceable
    {
        public static Dictionary<string, Potager> Potagers = new Dictionary<string, Potager>();

        public Potager(uint nwobj) : base(nwobj)
        {
            Potagers[nwobj.AsPlaceable().uuid] = this;
        }

        static Potager()
        {

        }

        public static void Init()
        {
            var oPotager = NWScript.GetObjectByTag("potager").AsPlaceable();
            int i = 0;
            while (oPotager.IsValid)
            {
                var command = Client.CreateCommand($"SELECT * FROM sql_potager WHERE uuid=@uuid LIMIT 1;");
                command.Parameters.AddWithValue("@uuid", oPotager.uuid);
                var dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    var sPlanteType = dataReader["type"].ToString();
                    var iPlanteState = DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(dataReader.GetInt32("date")));

                    if(iPlanteState.CompareTo(TimeSpan.FromSeconds(324000)) > 0)
                    {
                        oPotager.Name = $"Plant de {sPlanteType} prêt pour la récolte";
                        oPotager.Locals.Int.Set("_PLANTE_STATE", 2);
                        oPotager.Appearance = 4340;
                        var fNextStep = TimeSpan.FromSeconds(486000).Subtract(TimeSpan.FromSeconds(new TimeSpan(iPlanteState.Ticks).TotalSeconds));
                        NWScript.DelayCommand((float)fNextStep.TotalSeconds, () => FanerPlante(oPotager, sPlanteType, (float)fNextStep.TotalSeconds));
                    }
                    else if (iPlanteState.CompareTo(TimeSpan.FromSeconds(0)) > 0)
                    {
                        oPotager.Name = $"Plant de {sPlanteType} en cours de pousse";
                        oPotager.Locals.Int.Set("_PLANTE_STATE", 1);
                        oPotager.Appearance = 8791;
                        var fNextStep = TimeSpan.FromSeconds(324000).Subtract(TimeSpan.FromSeconds(new TimeSpan(iPlanteState.Ticks).TotalSeconds));
                        NWScript.DelayCommand((float)fNextStep.TotalSeconds, () => PousserPlante(oPotager, sPlanteType, (float)fNextStep.TotalSeconds));
                    }

                    if (iPlanteState.CompareTo(TimeSpan.FromSeconds(486000)) > 0)
                    {
                        oPotager.Name = $"Plant de {sPlanteType} fâné";
                        oPotager.Locals.Int.Set("_PLANTE_STATE", 3);
                        oPotager.Appearance = 571;
                    }
                }
                dataReader.Close();
                i += 1;
                oPotager = NWScript.GetObjectByTag("potager", i).AsPlaceable();
            }
        }

        public static void UpdateForUUID()
        {
            var oPotager = NWScript.GetObjectByTag("potager").AsPlaceable();
            int i = 0;
            while (oPotager.IsValid)
            {
                if(oPotager.Locals.Int.Get("id") > 0)
                {
                    var command = Client.CreateCommand(
                    $"UPDATE sql_potager (uuid)" +
                    " VALUES (@uuid)" +
                    " WHERE id=@id;");
                    command.Parameters.AddWithValue("@uuid", oPotager.uuid);
                    command.Parameters.AddWithValue("@id", oPotager.Locals.Int.Get("id"));
                    command.ExecuteNonQuery();
                }
                else
                {
                    var command = Client.CreateCommand(
                    $"INSERT INTO sql_potager (uuid)" +  
                    " VALUES (@uuid)");
                    command.Parameters.AddWithValue("@uuid", oPotager.uuid);
                    command.ExecuteNonQuery();

                }
                
                i += 1;
                oPotager = NWScript.GetObjectByTag("potager", i).AsPlaceable();
            }
        }

        private static void FanerPlante(NWPlaceable oPotager, string sPlanteType, float sPlanteState)
        {
            if (oPotager.IsValid)
            {
                oPotager.Name = $"Plant de  {sPlanteType} fâné";
                oPotager.Appearance = 517;
                oPotager.Locals.Int.Set("_PLANTE_STATE", 3);
            }
        }

        private static void PousserPlante(NWPlaceable oPotager, string sPlanteType, float sPlanteState)
        {
            if (oPotager.IsValid)
            {
                oPotager.Name = "Plant de {sPlanteType} prêt pour la récolte";
                oPotager.Appearance = 4340;
                oPotager.Locals.Int.Set("_PLANTE_STATE", 2);
                NWScript.DelayCommand(162000.0f, () => FanerPlante(oPotager, sPlanteType, sPlanteState));
            }
        }
    }
}
