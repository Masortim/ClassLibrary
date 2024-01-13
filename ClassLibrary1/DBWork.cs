using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data.EntityClient;
using System.Text;

namespace ClassLibrary1
{
    public class DBWork
    {
        /* добавить в StamatDBEntities после обновления базы данных
         public StamatDBEntities(string connectionString)
            : base(connectionString)
        {
        }
             */

        public static string ConnectionString
        {
            get
            {
                //
                var conStr = new EntityConnectionStringBuilder
                {
                    //AppDomain.CurrentDomain.BaseDirectory + "App_Data\\StamatDB.mdf;" + //
                    Provider = "System.Data.SqlClient",
                    ProviderConnectionString =
                            "data source=(LocalDB)\\MSSQLLocalDB;" +
                            "Initial Catalog=StamatDB.mdf;" +
                            "AttachDBFilename=" + AppDomain.CurrentDomain.BaseDirectory + "App_Data\\StamatDB.mdf;" +
                            //"AttachDBFilename=" + "D:\\StamatAut\\StamatAut\\StamatAut\\App_Data\\StamatDB.mdf;" +
                            "Integrated security=True;" +
                            "Connection Timeout=30;" +
                            "MultipleActiveResultSets=True;" +
                            "App=EntityFramework;",
                    Metadata = "res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl"
                };
                return conStr.ToString();
            }
        }

        static string Cyrillify(string s)
        {
            var sb = new StringBuilder(s);
            foreach (var kvp in Replacements)
                sb.Replace(kvp.Key, kvp.Value);
            return sb.ToString();
        }

        static Dictionary<char, char> Replacements = new Dictionary<char, char>()
        {
            ['a'] = 'а',
            ['b'] = 'б',
            ['c'] = 'с',
            ['e'] = 'е',
        };

        static Dictionary<string, int> RimskNumeric = new Dictionary<string, int>()
        {
            ["I"] = 1,
            ["II"] = 2,
            ["III"] = 3,
            ["IV"] = 4,
            ["V"] = 5,
            ["VI"] = 6,
            ["VII"] = 7,
            ["VIII"] = 8,
            ["IX"] = 9,
            ["X"] = 10,
            ["XI"] = 11,
            ["XII"] = 12,
            ["XIII"] = 13,
            ["XIV"] = 14,
            ["XV"] = 15,
            ["XVI"] = 16,
            ["XVII"] = 17,
            ["XVIII"] = 18,
            ["XIX"] = 19,
            ["XX"] = 20
        };

        static Dictionary<string, int> Alphavit = new Dictionary<string, int>()
        {
            ["а"] = 1,
            ["б"] = 2,
            ["в"] = 3,
            ["г"] = 4,
            ["д"] = 5,
            ["е"] = 6,
            ["ж"] = 7,
            ["з"] = 8,
            ["и"] = 9,
            ["к"] = 10
        };

        #region User
        /// <summary>
        /// возвращает '0', если такой пользователь не существует,
        /// и id_Farm, если пользователь существует
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="passwrd"></param>
        /// <returns></returns>
        public static int GetEntrance(string userName/*, string passwrd*/)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.User.Where(c => c.Name == userName/* && c.Password == passwrd*/).FirstOrDefault();
                if (str != null)
                {
                    var frm = dataContext.User.Where(c => c.Id == str.Id).FirstOrDefault().Farm.Id;
                    return frm;
                }
                else
                    return 0;
            }
        }

        public static int GetAccessLevel(string userName)
        {
            using (var dataContext = new StamatDBEntities( ConnectionString ))
            {
                var str = dataContext.User.Where(c => c.Name == userName).FirstOrDefault();
                if (str != null)
                {
                    var frm = dataContext.User.Where(c => c.Id == str.Id).FirstOrDefault().AccesLevel;
                    return frm;
                }
                else
                    return 0;
            }
        }

        /// <summary>
        /// возвращает'false', если уже сущствует пользователь с таким именем, или невозможно осуществить запись
        /// возвращает 'true', если пользователя не существует, и запись удалась
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="passwrd"></param>
        /// <param name="farmName"></param>
        /// <returns></returns>
        public static bool SetEntrance(string userName, string passwrd, string farmName)
        {
            using (var dataContext = new StamatDBEntities( ConnectionString ))
            {
                var str = dataContext.User.Where(c => c.Name == userName).FirstOrDefault();
                if (str == null)
                {
                    try
                    {
                        var f = dataContext.Farm.Where(c => c.Name == farmName).FirstOrDefault();
                        if (f == null)
                        {
                            var d1 = dataContext.Disease.Where(c => c.Name == "Пыльная головня" && c.Value == -1).First();
                            var d2 = dataContext.Stability.Where(c => c.Name == "Устойчивость к полеганию" && c.Value == 0).First();
                            var s1 = new Sort
                            {
                                Name = "Новосибирская 22",
                                Cultures = dataContext.Cultures.Where(c => c.Name == "Пшеница").First(),
                                M1000 = 34,
                                GrowingSeason_0 = 70,
                                GrowingSeason_1 = 82,
                                Productivity = 15,
                                ProductivityMax = 39,
                                Disease = new List<Disease> { d1 },
                                Stability = new List<Stability> { d2 },
                                TypeSort = dataContext.TypeSort.Where(c => c.Name == "Пшеница яровая (мягкая), ранние").First()
                            };
                            var sort = new List<Sort>
                                { s1 };
                            var fU = new Farm
                            {
                                Name = farmName,
                                User = new List<User>
                                {
                                    new User
                                    {
                                    Name = userName,
                                    Password = passwrd
                                    }
                                },
                                Sort = sort
                            };
                            dataContext.Farm.Add(fU);
                            dataContext.SaveChanges();
                            return true;
                        }
                        else
                        {
                            var u = new User
                            {
                                Name = userName,
                                Password = passwrd,
                                Farm_Id = f.Id,
                                Farm = f
                            };
                            dataContext.User.Add(u);
                            dataContext.SaveChanges();
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                else
                    return false;
            }
        }

        public static bool DeleteUserEntrance(string userName, string passwrd)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.User.Where(c => c.Name == userName).FirstOrDefault();
                if (str != null)
                    try
                    {
                        var f = dataContext.Farm.Where(c => c.User.Where(p => p.Id == str.Id).FirstOrDefault() != null).FirstOrDefault();
                        //dataContext.Farm.Remove(f);
                        dataContext.User.Remove(str);
                        if (f.User.Count() == 0)
                            dataContext.Farm.Remove(f);
                        dataContext.SaveChanges();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                else return false;
            }
        }

        public static bool DeleteFarmEntrance(string farmName)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var f = dataContext.Farm.Where(c => c.Name == farmName).FirstOrDefault();
                if (f != null)
                    try
                    {
                        var user = f.User;
                        while (user.Count != 0)
                            dataContext.User.Remove(user.First());
                        dataContext.Farm.Remove(f);
                        dataContext.SaveChanges();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                else return false;
            }
        }
        #endregion

        #region Intensification

        #region get
        public static List<string> GetIntensification()
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.Intensification.ToList();
                var res = new List<string>();
                foreach (var s in str)
                {
                    res.Add(s.Level_Int);
                }
                return res;
            }
        }
        #endregion

        #region set
        public static bool SetIntensification(string levelInt, List<int> idUch, int idFarm)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var lvl = dataContext.Intensification.Where(p => p.Level_Int == levelInt).FirstOrDefault();//интенсификация
                if (lvl != null)
                {
                    foreach (var s in idUch)
                    {
                        var id = dataContext.WorkAreas.FirstOrDefault(p => p.IdUch == s && p.Farm.Id==idFarm);//номер участка, площадь, название и др.
                        var tehK = dataContext.TechKart.Where(p => p.WorkAreas == id).FirstOrDefault();//есть ли для этого участка запись
                        if (tehK != null)
                        {
                        }
                        else //первое задание участка, но связь с предшественником есть
                        {

                        }
                        //создаем ТКР

                        /* var aP = new AreaPred
                         {
                             year = System.DateTime.Now.Year - 1,
                             WorkAreas = new WorkAreas
                             {
                                 IdUch = idUch,
                                 NumUch = "-",
                                 Square = 0,
                                 MaxShirZahv = 0,
                                 Farm = f
                             },
                             Predshest = pred
                         };
                         dataContext.AreaPred.Add(aP);
                         dataContext.SaveChanges();*/
                    }
                    return true;
                }
                else
                    return false;
            }
        }
        #endregion

        #endregion

        #region Predshest

        #region get
        public static List<string> GetPredshest()
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.Predshest.ToList();
                var res = new List<string>();
                foreach (var s in str)
                {
                    res.Add(s.Name);
                }
                return res;
            }
        }
        #endregion

        #endregion

        #region Climate

        #region get
        public static List<string> GetAllNameClimate()
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.Climate.ToList();
                var res = new List<string>();
                foreach (var s in str)
                {
                    res.Add(s.Name.ToString());
                }
                return res;
            }
        }

        public static List<Climat> GetAllClimate()
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.Climate.ToList();
                var res = new List<Climat>();
                foreach (var s in str)
                {
                    res.Add(new Climat(s.Id, s.Name, s.SummTempVegPer_5_10, s.SummTempVegPer_10_12, s.DataBVegPer, s.DataEVegPer, s.KolOsadS, s.KolOsadkovJune, s.KoefUvl));
                }
                return res;
            }
        }

        public static Climat GetClimateOnName(string name)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var s = dataContext.Climate.Where(p => p.Name == name).First();
                return new Climat(s.Id, s.Name, s.SummTempVegPer_5_10, s.SummTempVegPer_10_12, s.DataBVegPer, s.DataEVegPer, s.KolOsadS, s.KolOsadkovJune, s.KoefUvl);
            }
        }

        public static Climat GetClimateOnFarm(int idFarm)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var f = dataContext.Farm.Where(p => p.Id == idFarm).First();
                var s = dataContext.Climate.Where(p => p.Farm == f).First();
                return new Climat(s.Id, idFarm, s.Name, s.SummTempVegPer_5_10, s.SummTempVegPer_10_12, s.DataBVegPer, s.DataEVegPer, s.KolOsadS, s.KolOsadkovJune, s.KoefUvl);
            }
        }

        public static Climat GetClimateOnSoil(int idSoil)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var f = dataContext.Soil.Where(p => p.Id == idSoil).First();
                var s = dataContext.Climate.Where(p => p.Id == f.Climate_Id).First();
                return new Climat(s.Id, -1, s.Name, s.SummTempVegPer_5_10, s.SummTempVegPer_10_12, s.DataBVegPer, s.DataEVegPer, s.KolOsadS, s.KolOsadkovJune, s.KoefUvl);
            }
        }
        #endregion

        #region set
        public static bool SetClimate(int id, string Name, string STVPer_5_10, string STVPer_10_12, string KOsS, string KOsJune, string DataBeg, string DataEnd,
            string KoefUvl)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    if (id == -1)
                    {
                        if (dataContext.Climate.Where(p => p.Name == Name).FirstOrDefault() == null)
                            dataContext.Climate.Add(new Climate
                            {
                                Name = Name,
                                SummTempVegPer_5_10 = STVPer_5_10,
                                SummTempVegPer_10_12 = STVPer_10_12,
                                KolOsadS = KOsS,
                                KolOsadkovJune = KOsJune,
                                DataBVegPer = DataBeg,
                                DataEVegPer = DataEnd,
                                KoefUvl = KoefUvl
                            });
                        else
                            return false;
                    }
                    else
                    {
                        var str = dataContext.Climate.Where(p => p.Id == id).First();
                        str.Name = Name;
                        str.SummTempVegPer_5_10 = STVPer_5_10;
                        str.SummTempVegPer_10_12 = STVPer_10_12;
                        str.KolOsadS = KOsS;
                        str.KolOsadkovJune = KOsJune;
                        str.DataBVegPer = DataBeg;
                        str.DataEVegPer = DataEnd;
                        str.KoefUvl = KoefUvl;
                    }
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #region save
        public static bool SaveClimateToFarm(int idFarm, string climateName)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.Climate.Where(p => p.Name == climateName).First();
                var frm = dataContext.Farm.Where(p => p.Id == idFarm).First();
                frm.Climate = str;
                try
                {
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #region del
        public static bool DeleteClimate(int id)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    var str = dataContext.Climate.Where(p => p.Id == id).First();
                    str.Soil = null;
                    str.TechnologicalOperations = null;
                    str.Farm = null;
                    dataContext.Climate.Remove(str);
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #endregion

        #region Culture

        #region get
        public static List<Culture> GetCulture()
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.Cultures.ToList();
                var res = new List<Culture>();
                foreach (var s in str)
                {
                    res.Add(new Culture(s.Id, s.Name.ToString()));
                }
                return res;
            }
        }
        #endregion

        #region set
        public static bool SetCulture(int id, string Name)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    if (id == -1)
                    {
                        if (dataContext.Cultures.Where(p => p.Name == Name).FirstOrDefault() == null)
                            dataContext.Cultures.Add(new Cultures { Name = Name });
                        else
                            return false;
                    }
                    else
                    {
                        var str = dataContext.Cultures.Where(p => p.Id == id).First();
                        str.Name = Name;
                    }
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #region del
        public static bool DeleteCulture(int id)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    var str = dataContext.Cultures.Where(p => p.Id == id).First();
                    var tkr = dataContext.TKR.Where(c => c.Cultures.Id == id);
                    var ss = dataContext.Sort.Where(c => c.Cultures.Id == id);
                    foreach (var tk in tkr)
                        if (tk != null)
                        {
                            foreach (var wt in tk.WA_TKR)
                            {
                                wt.Pest = null;
                                wt.Disease = null;
                                wt.Sort = null;
                                wt.Stability = null;
                                wt.WorkAreas = null;
                                dataContext.WA_TKR.Remove(wt);
                            }
                            dataContext.TKR.Remove(tk);
                        }
                        else
                            return false;
                    foreach (var s in ss)
                        if (s != null)
                        {
                            s.Pest = null;
                            s.Disease = null;
                            s.Stability = null;
                            dataContext.Sort.Remove(s);
                        }
                        else
                            return false;
                    dataContext.Cultures.Remove(str);
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #endregion

        #region Disease

        #region get
        public static List<Diseas> GetDisease()
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.Disease.ToList();
                var res = new List<Diseas>();
                foreach (var s in str)
                {
                    res.Add(new Diseas(s.Id, s.Name, s.Value));
                }
                return res;
            }
        }
        #endregion

        #region set
        public static bool SetDisease(int id, string Name, int Value)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    if (id == -1)
                    {
                        if (dataContext.Disease.Where(p => p.Name == Name).FirstOrDefault() == null)
                            dataContext.Disease.Add(new Disease { Name = Name, Value = Value });
                        else
                            return false;
                    }
                    else
                    {
                        var str = dataContext.Disease.Where(p => p.Id == id).First();
                        str.Name = Name;
                        str.Value = Value;
                    }
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #region del
        public static bool DeleteDisease(int id)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    var str = dataContext.Disease.Where(p => p.Id == id).First();
                    var tkr = dataContext.WA_TKR.Where(c => c.Disease.Where(p => p.Id == id).FirstOrDefault() != null);
                    var ss = dataContext.Sort.Where(c => c.Disease.Where(p => p.Id == id).FirstOrDefault() != null);
                    foreach (var tk in tkr)
                        if (tk != null)
                        {
                            tk.WorkAreas = null;
                            var t = tk.TKR;
                            tk.Sort = null;
                            dataContext.TKR.Remove(t);
                            dataContext.WA_TKR.Remove(tk);
                        }
                        else
                            return false;
                    foreach (var s in ss)
                        if (s != null)
                        {
                            s.Disease = null;
                            //dataContext.Sort.Remove(s);
                        }
                        else
                            return false;
                    dataContext.Disease.Remove(str);
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #endregion

        #region Pest

        #region get
        public static List<Pests> GetPest()
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.Pest.ToList();
                var res = new List<Pests>();
                foreach (var s in str)
                {
                    res.Add(new Pests(s.Id, s.Name, s.Value));
                }
                return res;
            }
        }
        #endregion

        #region set
        public static bool SetPest(int id, string Name, int Value)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    if (id == -1)
                    {
                        if (dataContext.Pest.Where(p => p.Name == Name).FirstOrDefault() == null)
                            dataContext.Pest.Add(new Pest { Name = Name, Value = Value });
                        else
                            return false;
                    }
                    else
                    {
                        var str = dataContext.Pest.Where(p => p.Id == id).First();
                        str.Name = Name;
                        str.Value = Value;
                    }
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #region del
        public static bool DeletePest(int id)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    var str = dataContext.Pest.Where(p => p.Id == id).First();
                    var tkr = dataContext.WA_TKR.Where(c => c.Pest.Where(p => p.Id == id).FirstOrDefault() != null);
                    var ss = dataContext.Sort.Where(c => c.Pest.Where(p => p.Id == id).FirstOrDefault() != null);
                    foreach (var tk in tkr)
                        if (tk != null)
                        {
                            tk.WorkAreas = null;
                            var t = tk.TKR;
                            tk.Sort = null;
                            dataContext.TKR.Remove(t);
                            dataContext.WA_TKR.Remove(tk);
                        }
                        else
                            return false;
                    foreach (var s in ss)
                        if (s != null)
                        {
                            s.Disease = null;
                            //dataContext.Sort.Remove(s);
                        }
                        else
                            return false;
                    dataContext.Pest.Remove(str);
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #endregion

        #region Stability

        #region get
        public static List<Stabil> GetStability()
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.Stability.ToList();
                var res = new List<Stabil>();
                foreach (var s in str)
                {
                    res.Add(new Stabil(s.Id, s.Name, s.Value));
                }
                return res;
            }
        }
        #endregion

        #region set
        public static bool SetStability(int id, string Name, int Value)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    if (id == -1)
                    {
                        if (dataContext.Stability.Where(p => p.Name == Name).FirstOrDefault() == null)
                            dataContext.Stability.Add(new Stability { Name = Name, Value = Value });
                        else
                            return false;
                    }
                    else
                    {
                        var str = dataContext.Stability.Where(p => p.Id == id).First();
                        str.Name = Name;
                        str.Value = Value;
                    }
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #region del
        public static bool DeleteStability(int id)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    var str = dataContext.Stability.Where(p => p.Id == id).First();
                    var tkr = dataContext.WA_TKR.Where(c => c.Stability.Where(p => p.Id == id).FirstOrDefault() != null);
                    var ss = dataContext.Sort.Where(c => c.Stability.Where(p => p.Id == id).FirstOrDefault() != null);
                    foreach (var tk in tkr)
                        if (tk != null)
                        {
                            tk.WorkAreas = null;
                            var t = tk.TKR;
                            tk.Sort = null;
                            dataContext.TKR.Remove(t);
                            dataContext.WA_TKR.Remove(tk);
                        }
                        else
                            return false;
                    foreach (var s in ss)
                        if (s != null)
                        {
                            s.Disease = null;
                            //dataContext.Sort.Remove(s);
                        }
                        else
                            return false;
                    dataContext.Stability.Remove(str);
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #endregion

        #region Soil

        #region get
        public static List<Soile> GetSoil()
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.Soil.ToList();
                var res = new List<Soile>();
                foreach (var s in str)
                {
                    res.Add(new Soile(s.Id, s.Name));
                }
                return res;
            }
        }

        public static List<Soile> GetSoil(int idFarm)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var clim = dataContext.Climate.Where(p => p.Farm.Where(c => c.Id == idFarm).FirstOrDefault() != null).First();
                var str = clim.Soil.ToList();
                var res = new List<Soile>();
                foreach (var s in str)
                {
                    res.Add(new Soile(s.Id, s.Name));
                }
                return res;
            }
        }
        #endregion

        #region set
        public static bool SetSoil(int id, string Name, string nameClimat)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    var clim = dataContext.Climate.Where(p => p.Name == nameClimat).First();
                    if (id == -1)
                    {
                        if (dataContext.Soil.Where(p => p.Name == Name).FirstOrDefault() == null)
                            dataContext.Soil.Add(new Soil { Name = Name, Climate = clim });
                        else
                            return false;
                    }
                    else
                    {
                        var str = dataContext.Soil.Where(p => p.Id == id).First();
                        str.Name = Name;
                        str.Climate = clim;
                    }
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #region del
        public static bool DeleteSoil(int id)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    var str = dataContext.Soil.Where(p => p.Id == id).First();
                    str.WorkAreas = null;
                    str.Climate = null;
                    dataContext.Soil.Remove(str);
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #endregion

        #region Sort

        #region get
        public static List<Sorte> GetSort(int idFarm)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.Sort.Where(p => p.Farm.Id == idFarm).ToList();
                var res = new List<Sorte>();
                foreach (var s in str)
                {
                    res.Add(new Sorte(s.Id, s.Name.ToString(), s.SeedingRase, s.M1000, s.CountSeedOnGa, s.Productivity, s.ProductivityMax, s.GrowingSeason_0, s.GrowingSeason_1, s.Cultures.Name));
                }
                return res;
            }
        }

        public static List<Sorte> GetSort(int idFarm, string nameTypeSort)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var ss = dataContext.TypeSort.Where(p => p.Name == nameTypeSort).First();
                var str = dataContext.Sort.Where(p => p.Farm.Id == idFarm && p.TypeSort == ss).ToList();
                var res = new List<Sorte>();
                foreach (var s in str)
                {
                    res.Add(new Sorte(s.Id, s.Name.ToString(), s.SeedingRase, s.M1000, s.CountSeedOnGa, s.Productivity, s.ProductivityMax, s.GrowingSeason_0, s.GrowingSeason_1, s.Cultures.Name));
                }
                return res;
            }
        }

        public static List<Sorte> GetRecommendSorts(int idFarm, int idTKR)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var clt = dataContext.TKR.Where(p => p.Id == idTKR).First().Cultures.Name;
                var sort = dataContext.Sort.Where(p => p.Farm.Id == idFarm && p.Cultures.Name == clt).ToList();
                var clim = dataContext.Climate.Where(p => p.Farm.Where(c => c.Id == idFarm).First() != null).First();
                var res = new List<Sorte>();
                foreach (var s in sort)
                {
                    switch (s.TypeSort.Temp)
                    {
                        case 10:
                            {
                                var sumT = s.TypeSort.SummTemp;
                                var vegP = clim.SummTempVegPer_10_12.Split('-').ToArray();
                                int d1, d2;
                                int.TryParse(vegP[0], out d1);
                                int.TryParse(vegP[1], out d2);
                                if (sumT >= d1)
                                    res.Add(new Sorte(s.Id, s.Name.ToString(), s.SeedingRase, s.M1000, s.CountSeedOnGa, s.Productivity,
                                        s.ProductivityMax, s.GrowingSeason_0, s.GrowingSeason_1, s.Cultures.Name));
                            }
                            break;
                        case 5:
                            {
                                var sumT = s.TypeSort.SummTemp;
                                var vegP = clim.SummTempVegPer_5_10.Split('-').ToArray();
                                int d1, d2;
                                int.TryParse(vegP[0], out d1);
                                int.TryParse(vegP[1], out d2);
                                if (sumT >= d1)
                                    res.Add(new Sorte(s.Id, s.Name.ToString(), s.SeedingRase, s.M1000, s.CountSeedOnGa, s.Productivity,
                                        s.ProductivityMax, s.GrowingSeason_0, s.GrowingSeason_1, s.Cultures.Name));
                            }
                            break;
                    }
                }
                return res;
            }
        }
        #endregion

        #region set
        public static bool SetSort(int idFarm, int id, string Name, string SR, string M1000, string CSonGa, string Product, string ProductMax, string GS_0, string GS_1, string nameTypeSprt)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    float sr, m, cs, p, pM;
                    int g0, g1;
                    float.TryParse(SR, out sr);
                    float.TryParse(CSonGa, out cs);
                    float.TryParse(M1000, out m);
                    float.TryParse(Product, out p);
                    float.TryParse(ProductMax, out pM);
                    int.TryParse(GS_0, out g0);
                    int.TryParse(GS_1, out g1);
                    var tS = dataContext.TypeSort.Where(c => c.Name == nameTypeSprt).First();
                    if (id == -1)
                    {
                        var cult = nameTypeSprt.Split(' ')[0].Split(',')[0];
                        var culture = dataContext.Cultures.Where(c => c.Name == cult).First();
                        if (dataContext.Sort.Where(c => c.Name == Name && c.Farm.Id == idFarm).FirstOrDefault() == null)
                            dataContext.Sort.Add(new Sort
                            {
                                Name = Name,
                                SeedingRase = sr,
                                M1000 = m,
                                CountSeedOnGa = cs,
                                Productivity = p,
                                ProductivityMax = pM,
                                GrowingSeason_0 = g0,
                                GrowingSeason_1 = g1,
                                Cultures_Id = culture.Id,
                                Farm = dataContext.Farm.Where(c => c.Id == idFarm).First(),
                                TypeSort = tS
                            });

                        else
                            return false;
                    }
                    else
                    {
                        var str = dataContext.Sort.Where(c => c.Farm.Id == idFarm && c.Id == id).First();
                        str.Name = Name;
                        str.M1000 = m;
                        str.CountSeedOnGa = cs;
                        str.SeedingRase = sr;
                        str.TypeSort = tS;
                    }
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #region del
        public static bool DeleteSort(int idFarm, int id)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    var str = dataContext.Sort.Where(p => p.Farm.Id == idFarm && p.Id == id).First();
                    // var ts = dataContext.TypeSort.Where(p => p.Sort == str).First();
                    // ts.Sort = null;                    
                    // var tkr = dataContext.WA_TKR.Where(c => c.Sort.Id == id);
                    // foreach (var wt in tkr)
                    //   if (wt != null)
                    //   {
                    //       wt.WorkAreas = null;
                    //       wt.TKR = null;
                    //       wt.Pest = null;
                    //       wt.Disease = null;
                    //       wt.Stability = null;
                    //       dataContext.WA_TKR.Remove(wt);
                    //    }  
                    dataContext.Sort.Remove(str);
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #endregion

        #region TypeSort

        #region get
        public static TSort GetTypeSort(int idSort)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var t = dataContext.Sort.Where(c => c.Id == idSort).First();
                var s = dataContext.TypeSort.Where(p => p.Id == t.TypeSort.Id).First();
                return new TSort(s.Id, s.Name, s.SummTemp, s.Temp);
            }
        }

        public static TSort GetTypeSort(string nameSort)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var t = dataContext.Sort.Where(c => c.Name == nameSort).First();
                var s = dataContext.TypeSort.Where(p => p.Id == t.TypeSort.Id).First();
                return new TSort(s.Id, s.Name, s.SummTemp, s.Temp);
            }
        }

        public static List<string> GetAllTypeSort()
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var res = new List<string>();
                var s = dataContext.TypeSort.ToList();
                foreach (var v in s)
                    res.Add(v.Name);
                return res;
            }
        }

        public static List<string> GetTypeSort(int idFarm, string nameCult)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var cl = dataContext.Climate.Where(p => p.Farm.Where(c => c.Id == idFarm).FirstOrDefault() != null).First();
                var res = new List<string>();
                switch (nameCult)
                {
                    case "Пшеница":
                        {
                            var vegP = cl.SummTempVegPer_10_12.Split('-').ToArray();
                            int d1, d2;
                            int.TryParse(vegP[0], out d1);
                            int.TryParse(vegP[1], out d2);
                            var list = dataContext.TypeSort.Where(p => p.Name.Contains("пшеница") != false).ToList();
                            foreach (var v in list)
                            {
                                if (v.SummTemp >= d1)
                                    res.Add(v.Name);
                            }
                        }
                        break;
                    case "Ячмень":
                        {
                            var vegP = cl.SummTempVegPer_5_10.Split('-').ToArray();
                            int d1, d2;
                            int.TryParse(vegP[0], out d1);
                            int.TryParse(vegP[1], out d2);
                            var list = dataContext.TypeSort.Where(p => p.Name.Contains("Ячмень") != false).ToList();
                            foreach (var v in list)
                            {
                                if (v.SummTemp <= d2 && v.SummTemp >= d1)
                                    res.Add(v.Name);
                            }
                        }
                        break;
                    case "Рожь":
                        {
                            var vegP = cl.SummTempVegPer_5_10.Split('-').ToArray();
                            int d1, d2;
                            int.TryParse(vegP[0], out d1);
                            int.TryParse(vegP[1], out d2);
                            var list = dataContext.TypeSort.Where(p => p.Name.Contains("рожь") != false).ToList();
                            foreach (var v in list)
                            {
                                if (v.SummTemp <= d2 && v.SummTemp >= d1)
                                    res.Add(v.Name);
                            }
                        }
                        break;
                    case "Овес":
                        {
                            var vegP = cl.SummTempVegPer_5_10.Split('-').ToArray();
                            int d1, d2;
                            int.TryParse(vegP[0], out d1);
                            int.TryParse(vegP[1], out d2);
                            var list = dataContext.TypeSort.Where(p => p.Name.Contains("Овес") != false).ToList();
                            foreach (var v in list)
                            {
                                if (v.SummTemp <= d2 && v.SummTemp >= d1)
                                    res.Add(v.Name);
                            }
                        }
                        break;
                    default:
                        break;
                }
                return res;
            }
        }
        #endregion

        #endregion


        #region Him

        #region get
        public static List<Himiz> GetHim(int idFarm)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.Him.Where(c => c.Farm.Id == idFarm).ToList();
                var res = new List<Himiz>();
                foreach (var s in str)
                {
                    res.Add(new Himiz(s.Id, s.Name.ToString(), s.Unit.ToString()));
                }
                return res;
            }
        }
        #endregion

        #region set
        public static bool SetHim(int idFarm, int id, string Name, string Unit)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    if (id == -1)
                    {
                        if (dataContext.Him.Where(p => p.Name == Name && p.Farm.Id == idFarm).FirstOrDefault() == null)
                            dataContext.Him.Add(new Him { Name = Name, Unit = Unit, Farm = dataContext.Farm.Where(p => p.Id == idFarm).First() });
                        else
                            return false;
                    }
                    else
                    {
                        var str = dataContext.Him.Where(p => p.Farm.Id == idFarm && p.Id == id).First();
                        str.Name = Name;
                        str.Unit = Unit;
                    }
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #region del
        public static bool DeleteHim(int idFarm, int id)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    var str = dataContext.Him.Where(p => p.Farm.Id == idFarm && p.Id == id).First();
                    /*var tkr = dataContext.TKR.Where(c => c.Cultures.Id == id);
                    foreach (var tk in tkr)
                        if (tk != null)
                        {
                            tk.WorkAreas = null;
                            dataContext.TKR.Remove(tk);
                        }
                        else
                            return false;
                    dataContext.Cultures.Remove(str);
                    dataContext.SaveChanges();*/
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #endregion

        #region Fert

        #region get
        public static List<Fert> GetFert(int idFarm)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.Fertilizer.Where(p => p.Farm.Id == idFarm).ToList();
                var res = new List<Fert>();
                foreach (var s in str)
                {
                    res.Add(new Fert(s.Id, s.Name, s.Unit, s.ContentOfPrep, s.Reactant.ToString()));
                }
                return res;
            }
        }
        #endregion

        #region set
        public static bool SetFertilizer(int idFarm, int id, string Name, string Reactant, string Content, string Unit)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    int r;
                    int.TryParse(Reactant, out r);
                    if (id == -1)
                    {
                        if (dataContext.Fertilizer.Where(p => p.Name == Name && p.Farm.Id == idFarm).FirstOrDefault() == null)
                            dataContext.Fertilizer.Add(new Fertilizer { Name = Name, Reactant = r, ContentOfPrep = Content, Unit = Unit, Farm = dataContext.Farm.Where(p => p.Id == idFarm).First() });
                        else
                            return false;
                    }
                    else
                    {
                        var str = dataContext.Fertilizer.Where(p => p.Farm.Id == idFarm && p.Id == id).First();
                        str.Name = Name;
                        str.Reactant = r;
                        str.ContentOfPrep = Content;
                        str.Unit = Unit;
                    }
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #region del
        public static bool DeleteFertilizer(int idFarm, int id)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    var str = dataContext.Fertilizer.Where(p => p.Farm.Id == idFarm && p.Id == id).First();
                    /*var tkr = dataContext.TKR.Where(c => c.Cultures.Id == id);
                    foreach (var tk in tkr)
                        if (tk != null)
                        {
                            tk.WorkAreas = null;
                            dataContext.TKR.Remove(tk);
                        }
                        else
                            return false;
                    dataContext.Cultures.Remove(str);
                    dataContext.SaveChanges();*/
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #endregion

        #region Workareas

        #region get
        public static List<WArea> GetWorkAreas(int idFarm, int year)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var res = new List<WArea>();
                var str = dataContext.WorkAreas.Where(c => c.Farm.Id == idFarm).ToList();
                foreach (var s3 in str)
                {
                    try
                    {
                        var ss = dataContext.TKR.Where(p => p.WA_TKR.Where(c => c.WorkAreas_Id == s3.Id).FirstOrDefault() != null && p.YearCrop == year).Count();//c=>c.WorkAreas.IdUch == s3.IdUch
                        if (ss > 0)
                        {
                            if (s3.Soil != null)
                                res.Add(new WArea(s3.IdUch, s3.NumUch, s3.Square, s3.MaxShirZahv, s3.Lenght,
                                    s3.Wight, s3.Angle_gr, s3.KolokPercent, s3.Distanse, s3.Soil.Name, true));
                            else
                                res.Add(new WArea(s3.IdUch, s3.NumUch, s3.Square, s3.MaxShirZahv, s3.Lenght,
                                    s3.Wight, s3.Angle_gr, s3.KolokPercent, s3.Distanse, "", true));
                        }
                        else
                        {
                            if (s3.Soil != null)
                                res.Add(new WArea(s3.IdUch, s3.NumUch, s3.Square, s3.MaxShirZahv, s3.Lenght,
                                    s3.Wight, s3.Angle_gr, s3.KolokPercent, s3.Distanse, s3.Soil.Name, false));
                            else
                                res.Add(new WArea(s3.IdUch, s3.NumUch, s3.Square, s3.MaxShirZahv, s3.Lenght,
                                s3.Wight, s3.Angle_gr, s3.KolokPercent, s3.Distanse, "", false));
                        }
                    }
                    catch (Exception ex)
                    {
                        res.Add(new WArea(s3.IdUch, s3.NumUch, s3.Square, s3.MaxShirZahv, s3.Lenght,
                                s3.Wight, s3.Angle_gr, s3.KolokPercent, s3.Distanse, s3.Soil.Name, false));
                    }
                }
                return res;
            }
        }

        public static List<WArea> GetWorkAreas(int idFarm)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var res = new List<WArea>();
                var str = dataContext.WorkAreas.Where(c => c.Farm.Id == idFarm).ToList();
                foreach (var s3 in str)
                {
                    try
                    {
                        var ss = dataContext.TKR.Where(p => p.WA_TKR.Where(c => c.WorkAreas_Id == s3.Id).FirstOrDefault() != null).Count();//c=>c.WorkAreas.IdUch == s3.IdUch
                        if (ss > 0)
                        {
                            if (s3.Soil != null)
                                res.Add(new WArea(s3.IdUch, s3.NumUch, s3.Square, s3.MaxShirZahv, s3.Lenght,
                                    s3.Wight, s3.Angle_gr, s3.KolokPercent, s3.Distanse, s3.Soil.Name, true));
                            else
                                res.Add(new WArea(s3.IdUch, s3.NumUch, s3.Square, s3.MaxShirZahv, s3.Lenght,
                                    s3.Wight, s3.Angle_gr, s3.KolokPercent, s3.Distanse, "", true));
                        }
                        else
                        {
                            if (s3.Soil != null)
                                res.Add(new WArea(s3.IdUch, s3.NumUch, s3.Square, s3.MaxShirZahv, s3.Lenght,
                                    s3.Wight, s3.Angle_gr, s3.KolokPercent, s3.Distanse, s3.Soil.Name, false));
                            else
                                res.Add(new WArea(s3.IdUch, s3.NumUch, s3.Square, s3.MaxShirZahv, s3.Lenght,
                                s3.Wight, s3.Angle_gr, s3.KolokPercent, s3.Distanse, "", false));
                        }
                    }
                    catch (Exception ex)
                    {
                        res.Add(new WArea(s3.IdUch, s3.NumUch, s3.Square, s3.MaxShirZahv, s3.Lenght,
                                s3.Wight, s3.Angle_gr, s3.KolokPercent, s3.Distanse, s3.Soil.Name, false));
                    }
                }
                return res;
            }
        }
        #endregion

        #region add
        public static bool SetEmptyWorkAreas(int idFarm, int idUch)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.WorkAreas.Where(c => c.IdUch == idUch && c.Farm.Id == idFarm).FirstOrDefault();
                if (str == null)
                {
                    try
                    {
                        var f = dataContext.Farm.Where(c => c.Id == idFarm).FirstOrDefault();
                        var aP = new WorkAreas
                        {
                            IdUch = idUch,
                            NumUch = "-",
                            Square = 0,
                            MaxShirZahv = 0,
                            Farm = f
                        };
                        dataContext.WorkAreas.Add(aP);
                        dataContext.SaveChanges();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                else
                    return false;
            }
        }

        #endregion

        #region set
        public static bool SetWorkAreas(int idFarm, int idUch, string nameUch, double square, double maxShirZahv,
            double length, double width, double angle, double distance, double kolok, string soil)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var wA = dataContext.WorkAreas.Where(c => c.IdUch == idUch && c.Farm.Id == idFarm).FirstOrDefault();
                if (wA == null)
                {
                    try
                    {
                        var f = dataContext.Farm.Where(c => c.Id == idFarm).First();
                        float sq, mx;
                        float.TryParse(square.ToString(), out sq);
                        float.TryParse(maxShirZahv.ToString(), out mx);
                        var str = new WorkAreas
                        {
                            IdUch = idUch,
                            NumUch = nameUch,
                            Square = sq,
                            MaxShirZahv = mx,
                            Lenght = length,
                            Wight = width,
                            Angle_gr = angle,
                            Distanse = distance,
                            KolokPercent = kolok,
                            Soil = dataContext.Soil.Where(c => c.Name == soil).First(),
                            Farm = f
                        };
                        dataContext.WorkAreas.Add(str);
                        dataContext.SaveChanges();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                else
                {
                    try
                    {
                        var f = dataContext.Farm.Where(c => c.Id == idFarm).First();
                        float sq, mx;
                        float.TryParse(square.ToString(), out sq);
                        float.TryParse(maxShirZahv.ToString(), out mx);
                        var yearNow = DateTime.Now.Year;
                        wA.Square = sq;
                        wA.NumUch = nameUch;
                        wA.MaxShirZahv = mx;
                        wA.Lenght = length;
                        wA.Wight = width;
                        wA.Angle_gr = angle;
                        wA.Distanse = distance;
                        wA.KolokPercent = kolok;
                        wA.Soil = dataContext.Soil.Where(c => c.Name == soil).First();
                        dataContext.SaveChanges();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }
        }
        #endregion

        #region del
        public static bool DelWorkAreas(int idFarm, int idUch)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var wA = dataContext.WorkAreas.Where(c => c.IdUch == idUch && c.Farm.Id == idFarm).FirstOrDefault();
                if (wA != null)
                {
                    try
                    {
                        dataContext.WorkAreas.Remove(wA);
                        dataContext.SaveChanges();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                else
                    return false;
            }
        }
        #endregion

        #endregion

        #region TKR

        #region get
        public static List<int> GetYearsAll(int idFarm)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    var tkr = dataContext.TKR.Where(p => p.Farm.Id == idFarm);
                    if (tkr != null)
                    {
                        var list = new List<int> { DateTime.Now.Year };
                        foreach (var v in tkr)
                        {
                            if (!list.Contains(v.YearCrop))
                                list.Add(v.YearCrop);
                        }
                        return list.OrderByDescending(p => p).ToList();
                    }
                    else
                        return new List<int> { DateTime.Now.Year };
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public static bool ExistTKR(string culture, int year, string intensification, string pred)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var tkr = dataContext.TKR.Where(p => p.YearCrop == year && p.Intensification.Level_Int == intensification && p.Cultures.Name == culture && p.Predshest.Name == pred).FirstOrDefault();
                if (tkr != null)
                    return true;
                else
                    return false;
            }
        }

        public static List<TehKR> GetTKRonYear(int idFarm, int year)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var v = dataContext.TKR.Where(p => p.Farm.Id == idFarm && p.YearCrop == year);
                var list = new List<TehKR>();
                foreach (var tkr in v)
                {
                    var wa = new List<WorkAreas>();
                    foreach (var s in tkr.WA_TKR)
                    {
                        wa.Add(s.WorkAreas);
                    }
                    list.Add(new TehKR(tkr.Id, year, tkr.Cultures.Name, tkr.Intensification.Level_Int, tkr.Predshest.Name, wa, tkr.WA_TKR.ToList()));
                }
                return list;
            }
        }
        /*****************новое*****************/
        public static string GetTKRNameOnId (int idTKR)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                return dataContext.TKR.First(p => p.Id == idTKR).Name;               
            }
        }
        /**************************************/
        #endregion

        #region set
        public static bool SetTKR(int idFarm, int idTKR, string culture, string intensification, string predshest, int year, List<int> check)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    if (idTKR == -1)//не существует ли TKR?
                    {
                        if (dataContext.TKR.Where(p => p.Cultures.Name == culture && p.Farm.Id == idFarm && p.Intensification.Level_Int == intensification &&
                         p.Predshest.Name == predshest && p.YearCrop == year).FirstOrDefault() != null)
                        {
                            return false;
                        }
                        var F = dataContext.Farm.Where(p => p.Id == idFarm).FirstOrDefault();
                        var cult = dataContext.Cultures.Where(p => p.Name == culture).FirstOrDefault();
                        var intens = dataContext.Intensification.Where(p => p.Level_Int == intensification).FirstOrDefault();
                        var pred = dataContext.Predshest.Where(p => p.Name == predshest).FirstOrDefault();
                        var list = new List<WA_TKR>();//new List<WorkAreas>();
                        //удаляет привязку выбранных участков у всех ТКР заданного года
                        foreach (var v in check)
                        {
                            var wA = dataContext.WorkAreas.Where(p => p.IdUch == v && p.Farm.Id == idFarm).FirstOrDefault();
                            var TKwA = wA.WA_TKR.Where(p => p.TKR.YearCrop == year).FirstOrDefault();//var TKwA = wA.TKR.Where(p => p.YearCrop == year).FirstOrDefault();
                            if (TKwA != null)//есть связанная TKR
                            {
                                TKwA.WorkAreas = null;
                                TKwA.TKR = null;
                                dataContext.WA_TKR.Remove(TKwA);
                                //TKwA.WorkAreas.Remove(wA);
                            }
                            var nn = new WA_TKR { WorkAreas = wA };
                            dataContext.WA_TKR.Add(nn);
                            list.Add(nn);//list.Add(wA);                            
                        }
                        var tkr = new TKR()
                        {
                            Cultures = cult,
                            Intensification = intens,
                            YearCrop = year,
                            Predshest = pred,
                            Name = year + "/" + intensification + "/" + culture + "/" + predshest,
                            Farm = F,
                            //WorkAreas = list
                            WA_TKR = list
                        };
                        dataContext.TKR.Add(tkr);
                        dataContext.SaveChanges();
                    }
                    else
                    {
                        var tkr = dataContext.TKR.Where(p => p.Id == idTKR).FirstOrDefault();
                        if (tkr.Cultures.Name != culture || tkr.Intensification.Level_Int != intensification || tkr.Predshest.Name != predshest)
                        {
                            var cult = dataContext.Cultures.Where(p => p.Name == culture).FirstOrDefault();
                            var intens = dataContext.Intensification.Where(p => p.Level_Int == intensification).FirstOrDefault();
                            var pred = dataContext.Predshest.Where(p => p.Name == predshest).FirstOrDefault();
                            tkr.Cultures = cult;
                            tkr.Intensification = intens;
                            tkr.Predshest = pred;
                            tkr.Name = year + "/" + intensification + "/" + culture + "/" + predshest;
                        }
                        var list = new List<WA_TKR>();//new List<WorkAreas>();
                        var listWa = new List<WorkAreas>();
                        //удаляет привязку выбранных участков у всех ТКР заданного года
                        if (check != null)
                            foreach (var v in check)
                            {
                                var wA = dataContext.WorkAreas.Where(p => p.IdUch == v && p.Farm.Id == idFarm).FirstOrDefault();
                                var TKwA = wA.WA_TKR.Where(p => p.TKR.YearCrop == year).FirstOrDefault();//var TKwA = wA.TKR.Where(p => p.YearCrop == year).FirstOrDefault();
                                if (TKwA != null)//есть связанная TKR
                                {
                                    TKwA.WorkAreas = null;
                                    TKwA.TKR = null;
                                    dataContext.WA_TKR.Remove(TKwA);
                                    //TKwA.WorkAreas.Remove(wA);
                                }
                                var nn = new WA_TKR { WorkAreas = wA, TKR = tkr };
                                dataContext.WA_TKR.Add(nn);
                                list.Add(nn);//list.Add(wA);
                                listWa.Add(wA);
                            }
                        //удаялет привязку невыбранных участков заданной ТКР
                        var w = dataContext.WorkAreas.AsEnumerable();
                        var wa = w.Except(listWa);

                        foreach (var v in wa)
                        {
                            var TKwA = v.WA_TKR.Where(p => p.TKR.Id == idTKR && p.TKR.YearCrop == year).FirstOrDefault();//var TKwA = v.TKR.Where(p => p.Id == idTKR && p.YearCrop == year).FirstOrDefault();
                            if (TKwA != null)//есть связанная TKR
                            {
                                TKwA.WorkAreas = null;
                                TKwA.TKR = null;
                                dataContext.WA_TKR.Remove(TKwA);
                                //TKwA.WorkAreas.Remove(wA);
                            }
                        }
                        if (check == null)
                            tkr.WA_TKR = null;
                        else
                            tkr.WA_TKR = list;//.WorkAreas = list;
                        dataContext.SaveChanges();
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// добавляет записи по болезням, вредителям
        /// </summary>
        /// <param name="idFarm"></param>
        /// <param name="year"></param>
        /// <param name="idWa"></param>
        /// <param name="disease"></param>
        /// <param name="pest"></param>
        /// <returns></returns>
        public static string SetDopTKR(int idFarm, int year, int idWa, List<string> disease, List<string> pest)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var tkr = dataContext.TKR.Where(p => p.YearCrop == year && p.WA_TKR.Where(c => c.WorkAreas.IdUch == idWa).FirstOrDefault() != null && p.Farm.Id == idFarm).FirstOrDefault();
                if (tkr != null)
                {
                    List<Disease> disL = null;
                    if (disease != null)
                    {
                        disL = new List<Disease>();
                        foreach (var d in disease)
                        {
                            var dis = dataContext.Disease.Where(p => p.Name == d && p.Value == 1).First();
                            disL.Add(dis);
                        }
                    }

                    List<Pest> pesL = null;
                    if (pest != null)
                    {
                        pesL = new List<Pest>();
                        foreach (var d in pest)
                        {
                            var dis = dataContext.Pest.Where(p => p.Name == d && p.Value == 1).First();
                            pesL.Add(dis);
                        }
                    }
                    var wa = tkr.WA_TKR.Where(p => p.WorkAreas.IdUch == idWa).FirstOrDefault();
                    if (wa != null)
                    {
                        try
                        {
                            foreach (var v in wa.Disease)
                            {
                                var s = dataContext.Disease.Where(p => p.Id == v.Id).First();
                                s.WA_TKR = null;
                            }
                            foreach (var v in wa.Pest)
                            {
                                var s = dataContext.Pest.Where(p => p.Id == v.Id).First();
                                s.WA_TKR = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            return ex.ToString();
                        }
                        try
                        {
                            wa.Disease = disL;
                            wa.Pest = pesL;
                            dataContext.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            return ex.ToString();
                        }
                    }
                    else
                    {
                        try
                        {
                            var waT = new WA_TKR()
                            {
                                WorkAreas = dataContext.WorkAreas.Where(c => c.IdUch == idWa).First(),
                                TKR = tkr,
                                Disease = disL,
                                Pest = pesL
                            };
                            dataContext.WA_TKR.Add(waT);
                            dataContext.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            return ex.ToString();
                        }
                    }
                }
            }
            return "";
        }
        #endregion

        #region del
        public static bool DelTKR(int idFarm, int idTKR)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var tkr = dataContext.TKR.Where(c => c.Id == idTKR && c.Farm.Id == idFarm).FirstOrDefault();
                if (tkr != null)
                {
                    try
                    {
                        var lv = new List<WA_TKR>();
                        foreach (var v in tkr.WA_TKR)
                        {
                            foreach (var b in v.Disease)
                            {
                                var s = dataContext.Disease.Where(p => p.Id == b.Id).First();
                                s.WA_TKR = null;
                            }
                            foreach (var b in v.Pest)
                            {
                                var s = dataContext.Pest.Where(p => p.Id == b.Id).First();
                                s.WA_TKR = null;
                            }
                            foreach (var b in v.Stability)
                            {
                                var s = dataContext.Stability.Where(p => p.Id == b.Id).First();
                                s.WA_TKR = null;
                            }
                            v.Disease = null;
                            v.Pest = null;
                            v.Sort = null;
                            v.Stability = null;
                            v.WorkAreas = null;
                            lv.Add(v);
                        }
                        foreach (var v in lv)
                            dataContext.WA_TKR.Remove(v);
                        dataContext.TKR.Remove(tkr);
                        dataContext.SaveChanges();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                else
                    return false;
            }
        }
        #endregion

        #endregion

        #region TyagKlass
        public static List<ClassMashin> GetClassMashin()
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.ClassMachine.ToList();
                var res = new List<ClassMashin>();
                foreach (var s in str)
                {
                    res.Add(new ClassMashin(s.Id, s.Name.ToString(), s.TyagClass));
                }
                return res;
            }
        }

        #endregion

        #region Agregates

        #region get
        //GetBreakingList
        //Machinery with its Mechanizators
        /// <summary>
        /// Возвращает список машин в хозяйстве 
        /// </summary>
        /// <param name="idFarm"></param>
        /// <returns></returns>
        public static List<Machin> GetMachineryList(int idFarm)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.Machinery.Where(c => c.Farm.Id == idFarm);
                var res = new List<Machin>();
                if (str != null)
                {
                    foreach (var s in str)
                    {
                        res.Add(new Machin(s.Id, s.Name, s.Type, s.Price, s.Kind, s.SerialNumber, s.NormZagruz, s.PersentAmortOfTO, s.PercentAmort, s.ClassMachine.Name, idFarm));
                    }
                    return res;
                }
                else
                    return null;
            }
        }

        public static List<Machin> GetMachinery(int idFarm, string classMashine)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.Machinery.Where(c => c.Farm.Id == idFarm && c.ClassMachine.Name== classMashine);
                var res = new List<Machin>();
                if (str != null)
                {
                    foreach (var s in str)
                    {
                        res.Add(new Machin(s.Id, s.Name, s.Type, s.Price, s.Kind, s.SerialNumber, s.NormZagruz, s.PersentAmortOfTO, s.PercentAmort, s.ClassMachine.Name, idFarm));
                    }
                    return res;
                }
                else
                    return null;
            }
        }

        public static List<string> GetClassMachineNames(int idFarm)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.Machinery.Where(c => c.Farm.Id == idFarm);
                var cmn = new List<string>();
                if (str != null)
                {
                    foreach (var s in str)
                    {
                        cmn.Add(s.Name);
                    }
                    return cmn.Distinct().ToList();
                }
                else
                    return null;
            }
        }
        public static List<Trailing> GetTrailerList(int idFarm) // Скопировано с List<Machin> (выше) и переделано под Trailing
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.Trailers.Where(c => c.Farm.Id == idFarm); // Указание на таблицу БД Trailers
                var res = new List<Trailing>();
                if (str != null)
                {
                    foreach (var s in str)
                    {
                        res.Add(new Trailing(s.Id, s.Name, s.Price, s.NormZagruz, s.PersentAmortOfTO, s.PercentAmort, s.Count, idFarm));
                    }
                    return res;
                }
                else
                    return null;
            }
        }

        public static Trailing GetTrailer(int idFarm, string nameT) // Скопировано с List<Machin> (выше) и переделано под Trailing
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var s = dataContext.Trailers.FirstOrDefault(c => c.Farm.Id == idFarm && c.Name== nameT); // Указание на таблицу БД Trailers
                if (s != null)
                {
                    return new Trailing(s.Id, s.Name, s.Price, s.NormZagruz, s.PersentAmortOfTO, s.PercentAmort, s.Count, idFarm);
                }                                    
                else
                    return null;
            }
        }

        public static List<string> GetTrailerNames(int idFarm) // Скопировано с List<Machin> (выше) и переделано под Trailing
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.Trailers.Where(c => c.Farm.Id == idFarm); // Указание на таблицу БД Trailers
                var tn = new List<string>();
                if (str != null)
                {
                    foreach (var s in str)
                    {
                        tn.Add(s.Name);
                    }
                    return tn;
                }
                else
                    return null;
            }
        }
        public static List<Agreg> GetAgregateList(int idFarm) // Скопировано с List<Trailing> (выше) и переделано под Agregates
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.Agregates.Where(c => c.Farm.Id == idFarm); // Указание на таблицу БД Agregates
                var res = new List<Agreg>();
                if (str != null)
                {
                    foreach (var s in str)
                    {
                        var cmi = dataContext.ClassMachine.Where(c => c.Id == s.ClassMachine_Id); // Берём запись в таблице ClassMachine c данным Id
                        var kof = dataContext.KindOfFuel.Where(c => c.Id == s.KindOfFuel_Id); // // Берём запись в таблице KindOfFuel c данным Id
                        var tra = dataContext.Trailers.Where(c => c.Id == s.Trailers_Id); // // Берём запись в таблице Trailers c данным Id
                        foreach (var t in cmi)
                        {
                            foreach (var u in kof)
                            {
                                foreach (var v in tra)
                                { res.Add(new Agreg(s.Id, s.Name, s.Count, s.ShirZahvat, t.Name, u.Name, v.Name, idFarm)); } // Из записей в таблицах ClassMachine, KindOfFuel, Trailers извлекаем поля Name
                            }
                        }
                    }
                    return res;
                }
                else
                    return null;
            }
        }

        public static Agreg GetAgregate(int idagregate) // Получаем операции из таблицы OperationsOfAgregates
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var s = dataContext.Agregates.First(c => c.Id == idagregate); // Указание на таблицу БД Agregates                
                if (s != null)
                {                    
                   var res = new Agreg(s.Id, s.Name, s.Count, s.ShirZahvat, s.ClassMachine.Name, s.KindOfFuel.Name, s.Trailers.Name, s.Farm.Id);
                    
                    return res;
                }
                else
                    return null;
            }
        }

            public static List<AgregOper> GetAgregateOperations(int idagregate) // Получаем операции из таблицы OperationsOfAgregates
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.OperationsOfAgregate.Where(c => c.Agregates_Id == idagregate); // Указание на таблицу БД Agregates
                var res = new List<AgregOper>();
                if (str != null)
                {
                    foreach (var s in str)
                    {                        
                      res.Add(new AgregOper(s.TechnologicalOperations_Id.ToString(), s.GSMCharge.ToString(), s.SeedingRate.ToString()));  
                    }
                    return res;
                }
                else
                    return null;
            }
        }
        public static List<Operations> GetOperationLists(List<Operations> subgroups)
        //Получим в каждом элементе списка: Index группы, Index подгруппы, список имён операций подгруппы, список id операций подгруппы
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var res = new List<Operations>();
                foreach (var ss in subgroups)
                {
                    var str = dataContext.KindOfWork.FirstOrDefault(c => c.Index == ss.OperationGroupNameNumber).Id;
                    for (int i = 0; i < ss.OperationSubGroupNames.Count; i++)
                    {
                        var op = dataContext.TechnologicalOperations.Where(c => c.KindOfWork_Id == str);
                        var oplist = new List<string>();
                        var opnumberlist = new List<string>();
                        foreach (var o in op)
                        {
                            if (o.Name.StartsWith(ss.OperationSubGroupNames[i]))
                            {
                                oplist.Add(o.Name);
                                opnumberlist.Add(o.Id.ToString());
                            }
                        }
                        res.Add(new Operations(ss.OperationGroupNameNumber, ss.OperationSubGroupNumbers[i], oplist, opnumberlist));
                    }
                }
                return res;
            }
        }

        public static List<Operations> GetOperationGroups()
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.KindOfWork.Where(c => c.Index.Contains("_") == false).ToList();
                var res = new List<Operations>();
                var sgn = new List<string>();
                var sgf = new List<string>();
                if (str != null)
                {
                    int w = 0;
                    for (int i = 0; i < str.Count; i++)
                    {
                        var CI = str[i].Index + "_";
                        var sg = dataContext.KindOfWork.Where(c => c.Index.StartsWith(CI) == true).ToList();
                        for (int j = 0; j < sg.Count; j++)
                        {
                            sgn.Add(sg[j].Name);
                            sgf.Add(sg[j].Index);
                        }
                        res.Add(new Operations(str[i].Name, str[i].Index, sgn.GetRange(w, sg.Count), sgf.GetRange(w, sg.Count)));
                        w = w + sg.Count;
                    }
                    return res;
                }
                else
                    return null;
            }
        }
        #endregion

        #region StakesMech

        #region get
        public static List<MechStake> GetStakesMech(int idFarm)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.StakesOfMech.Where(p => p.Farm.Id == idFarm);
                var res = new List<MechStake>();
                if (str != null)
                {
                    foreach (var s in str)
                    {
                        res.Add(new MechStake(s.Id, s.Stake, s.Rank));
                    }
                    return res;
                }
                else
                    return null;
            }
        }
        #endregion

        #region set
        public static bool SetStakeMech(int idFarm, int id, string Stake, string Rank)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    float s;
                    float.TryParse(Stake, out s);
                    int r;
                    int.TryParse(Rank, out r);
                    if (id == -1)
                    {
                        var f = dataContext.Farm.FirstOrDefault(cc => cc.Id == idFarm);

                        if (dataContext.StakesOfMech.Where(p => p.Rank == r && p.Farm.Id == idFarm).FirstOrDefault() == null)
                            dataContext.StakesOfMech.Add(new StakesOfMech { Stake = s, Rank = r, Farm = f });
                        else
                            return false;
                    }
                    else
                    {
                        var str = dataContext.StakesOfMech.Where(p => p.Farm.Id == idFarm && p.Id == id).First();
                        str.Stake = s;
                        str.Rank = r;
                    }
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #region del
        public static bool DeleteStakeMech(int idFarm, int id)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    var str = dataContext.StakesOfMech.Where(p => p.Farm.Id == idFarm && p.Id == id).First();
                    dataContext.StakesOfMech.Remove(str);
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #endregion

        #region Mechanizator

        #region get
        public static List<Mechan> GetMechanizator(int idFarm)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.Mechanizator.Where(p => p.Farm.Id == idFarm);
                var res = new List<Mechan>();
                if (str != null)
                {
                    foreach (var s in str)
                    {
                        var st = dataContext.StakesOfMech.First(p => p.Id == s.StakesOfMech_Id);
                        res.Add(new Mechan(s.Id, s.Name, s.Class, st.Rank));
                    }
                    return res;
                }
                else
                    return null;
            }
        }
        #endregion

        #region set
        public static bool SetMechanizator(int idFarm, int id, string Name, string Rank, string Class)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    int r;
                    int.TryParse(Rank, out r);
                    var s = dataContext.StakesOfMech.FirstOrDefault(cp => cp.Rank == r);
                    if (id == -1)
                    {
                        var f = dataContext.Farm.FirstOrDefault(cc => cc.Id == idFarm);
                        if (dataContext.Mechanizator.Where(p => p.StakesOfMech_Id == s.Id && p.Farm.Id == idFarm).FirstOrDefault() == null)
                            dataContext.Mechanizator.Add(new Mechanizator { Name = Name, Class = Class, Farm = f, StakesOfMech_Id = s.Id });
                        else
                            return false;
                    }
                    else
                    {
                        var str = dataContext.Mechanizator.Where(p => p.Farm.Id == idFarm && p.Id == id).First();
                        str.Name = Name;
                        str.Class = Class;
                        str.StakesOfMech_Id = s.Id;
                    }
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #region del
        public static bool DeleteMechanizator(int idFarm, int id)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    var str = dataContext.Mechanizator.Where(p => p.Farm.Id == idFarm && p.Id == id).First();

                    var sl = dataContext.SickList.Where(p => p.Mechanizator_Id == id);
                    foreach (var t in sl)
                    {
                        t.Mechanizator_Id = null;
                        dataContext.SickList.Remove(t);
                    }
                    dataContext.Mechanizator.Remove(str);
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #endregion

        #region Breaking

        #region get
        public static List<Break> GetBreaking(int idFarm)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var mash = dataContext.Machinery.Where(p => p.Farm.Id == idFarm);
                var res = new List<Break>();
                if (mash != null)
                {
                    foreach (var s in mash)
                    {
                        var str = dataContext.Breaking.FirstOrDefault(p => p.Machinery_Id == s.Id);
                        if (str != null)
                        {
                            res.Add(new Break(str.Id, s.SerialNumber, str.DateBegin, str.DateEnd.Date, str.Reason));
                        }
                    }
                    return res;
                }
                else
                    return null;
            }
        }
        #endregion

        #region set
        public static bool SetBreaking(int id, string Serial, string BeginDate, string EndDate, string Reason)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var m = dataContext.Machinery.FirstOrDefault(c => c.SerialNumber == Serial);
                try
                {
                    DateTime b; DateTime.TryParse(BeginDate, out b); b = b.Date;
                    DateTime e; DateTime.TryParse(EndDate, out e); e = e.Date;
                    if (id == -1)
                    {
                        if (dataContext.Breaking.FirstOrDefault(p => p.Machinery_Id == m.Id) == null)
                            dataContext.Breaking.Add(new Breaking { DateBegin = b, DateEnd = e, Reason = Reason, Machinery_Id = m.Id });
                        else
                            return false;
                    }
                    else
                    {
                        var br = dataContext.Breaking.First(p => p.Id == id);
                        br.DateBegin = b;
                        br.DateEnd = e;
                        br.Reason = Reason;
                        br.Machinery_Id = m.Id;
                    }
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #region del
        public static bool DeleteBreaking(int id)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    var str = dataContext.Breaking.Where(p => p.Id == id).First();
                    dataContext.Breaking.Remove(str);
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #endregion

        #region Medik

        #region get
        public static List<Medik> GetMedic(int idFarm)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var mech = dataContext.Mechanizator.Where(p => p.Farm.Id == idFarm);
                var res = new List<Medik>();
                if (mech != null)
                {
                    foreach (var s in mech)
                    {
                        var str = dataContext.SickList.Where(p => p.Mechanizator_Id == s.Id);
                        if (str != null)
                            foreach (var t in str)
                            {
                                {
                                    res.Add(new Medik(t.Id, s.Name, t.DateBegin, t.DateEnd, t.Reason));
                                }
                            }
                    }
                    return res;
                }
                else
                    return null;
            }
        }
        #endregion

        #region set
        public static bool SetMedic(int id, string Name, string BeginDate, string EndDate, string Reason)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var m = dataContext.Mechanizator.FirstOrDefault(c => c.Name == Name);
                try
                {
                    DateTime b; DateTime.TryParse(BeginDate, out b); b = b.Date;
                    DateTime e; DateTime.TryParse(EndDate, out e); e = e.Date;
                    if (id == -1)
                    {
                        if (dataContext.SickList.FirstOrDefault(p => p.Mechanizator_Id == m.Id) == null)
                            dataContext.SickList.Add(new SickList { DateBegin = b, DateEnd = e, Reason = Reason, Mechanizator_Id = m.Id });
                        else
                            return false;
                    }
                    else
                    {
                        var br = dataContext.Breaking.First(p => p.Id == id);
                        br.DateBegin = b;
                        br.DateEnd = e;
                        br.Reason = Reason;
                        br.Machinery_Id = m.Id;
                    }
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #region del
        public static bool DeleteMedic(int id)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    var str = dataContext.SickList.Where(p => p.Id == id).First();
                    dataContext.SickList.Remove(str);
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #endregion

        #region              

        public static string[] GetIdM(string name)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var r1 = dataContext.ClassMachine.FirstOrDefault(c => c.Name == name);

                if (r1 != null)
                {
                    string[] res = { r1.Id.ToString(), r1.TyagClass.ToString() };
                    return res;
                }
                else return new string[] { "-1", "" };
            }
        }
        public static string GetTrailerQuantity(string name)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var r1 = dataContext.Trailers.FirstOrDefault(c => c.Name == name);

                if (r1 != null)
                {
                    string res = r1.Count.ToString();
                    return res;
                }
                else
                {
                    string res = "-1";
                    return res;
                }
            }
        }

        #endregion

        #region set
        public static bool SetMachinery(int id, string name, string type, float price, string kind, string serialNumber, int normZagruz,
            float persentAmort, float persentAmortOfTO, string classMachine, int farm_Id, int idm)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    if (idm == -1) // Если нет записи с именем classMachine в таблице classMachine
                    {
                        dataContext.ClassMachine.Add
                               (new ClassMachine
                               {
                                   Name = name,
                                   TyagClass = float.Parse(classMachine)
                               }
                               );
                        dataContext.SaveChanges();
                        idm = dataContext.ClassMachine.First(c => c.Name == name).Id;
                    }
                    if (id == -1)//добавляем машину
                    {
                        if (dataContext.Machinery.Where(p => p.SerialNumber == serialNumber).FirstOrDefault() == null)
                        {
                            dataContext.Machinery.Add
                                      (new Machinery
                                      {
                                          Name = name,
                                          Type = type,
                                          Price = price,
                                          Kind = kind,
                                          SerialNumber = serialNumber,
                                          NormZagruz = normZagruz,
                                          PercentAmort = persentAmort,
                                          PersentAmortOfTO = persentAmortOfTO,
                                          ClassMachine_Id = idm,
                                          Farm_Id = farm_Id
                                      }
                                      );
                        }
                        else
                            return false;
                    }
                    else //редактирование машины
                    {
                        var str = dataContext.Machinery.Where(p => p.Id == id).First();
                        str.Name = name;
                        str.Type = type;
                        str.Price = price;
                        str.Kind = kind;
                        str.SerialNumber = serialNumber;
                        str.NormZagruz = normZagruz;
                        str.PercentAmort = persentAmort;
                        str.PersentAmortOfTO = persentAmortOfTO;
                        str.ClassMachine_Id = idm;
                        str.Farm_Id = farm_Id;
                    }
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

        }

        public static bool SetTrailers(int id, string name, float price, float normZagruz,
            float persentAmort, float persentAmortOfTO, int quantity, int farm_Id)
        // Скопировано с SetMachinery (выше) и переделано под Trailers (таблица БД)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    if (id == -1)//добавляем машину
                    {
                        if (dataContext.Trailers.Where(p => p.Name == name).FirstOrDefault() == null)
                            dataContext.Trailers.Add(new Trailers
                            {
                                Name = name,
                                Price = price,
                                NormZagruz = normZagruz,
                                PercentAmort = persentAmort,
                                PersentAmortOfTO = persentAmortOfTO,
                                Count = quantity,
                                Farm_Id = farm_Id
                            });
                        else
                            return false;
                    }
                    else //редактирование машины
                    {
                        var str = dataContext.Trailers.Where(p => p.Id == id).First();
                        str.Name = name;
                        str.Price = price;
                        str.NormZagruz = normZagruz;
                        str.PercentAmort = persentAmort;
                        str.PersentAmortOfTO = persentAmortOfTO;
                        str.Count = quantity;
                        str.Farm_Id = farm_Id;
                    }
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

        }
        public static bool SetAgregates(int id, string name, int quantity, float shirina,
            string classname, string fuel, string trailer, int farm_Id, 
            List<string> agregateoperation, List<string> agregateoperationgsm, List<string> agregateoperationnorm)
        // Скопировано с SetTrailers (выше) и переделано под Agregates (таблица БД)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var cmi = dataContext.ClassMachine.FirstOrDefault(s => s.Name == classname);
                var kof = dataContext.KindOfFuel.FirstOrDefault(t => t.Name == fuel);
                var tra = dataContext.Trailers.FirstOrDefault(u => u.Name == trailer);
                try
                {
                    if (id == -1)//добавляем агрегат
                    {
                        if (dataContext.Agregates.FirstOrDefault(p => p.Name == name 
                             && p.Count == quantity && p.ShirZahvat == shirina && p.ClassMachine_Id == cmi.Id 
                             && p.KindOfFuel_Id == kof.Id && p.Trailers_Id == tra.Id && p.Farm_Id == farm_Id) == null)

                        {
                                       var AAA = dataContext.Agregates.Add(new Agregates
                                        {
                                            Name = name,
                                            Count = quantity,
                                            ShirZahvat = shirina,
                                            ClassMachine_Id = cmi.Id,
                                            KindOfFuel_Id = kof.Id,
                                            Trailers_Id = tra.Id,
                                            Farm_Id = farm_Id
                                        });
                                        dataContext.SaveChanges();
                            
                            for (int i = 0; i < agregateoperation.Count; i++) // добавляем операции агрегата
                            {                                
                                var oa = dataContext.OperationsOfAgregate.Add(new OperationsOfAgregate
                                {
                                    TechnologicalOperations_Id = Convert.ToInt32(agregateoperation[i]),
                                    SeedingRate = float.Parse(agregateoperationnorm[i]),
                                    GSMCharge = float.Parse(agregateoperationgsm[i]),
                                    Agregates_Id = AAA.Id
                                });
                            }
                        }
                                
                            
                        else
                            return false;
                    }
                    else //редактирование агрегата
                    {
                        var str = dataContext.Agregates.FirstOrDefault(p => p.Id == id); // Фиксируем запись агрегата                       
                                    str.Name = name;
                                    str.Count = quantity;
                                    str.ShirZahvat = shirina;
                                    str.ClassMachine_Id = cmi.Id;
                                    str.KindOfFuel_Id = kof.Id;
                                    str.Trailers_Id = tra.Id;
                                    str.Farm_Id = farm_Id;
                        var op = dataContext.OperationsOfAgregate.Where(p => p.Agregates_Id == id); // удаляем старые операции агрегата
                        foreach (var o in op)
                        {                           
                            o.TechnologicalOperations_Id = null;
                            o.Agregates_Id = null;
                            dataContext.OperationsOfAgregate.Remove(o);                            
                        }
                        agregateoperationgsm.RemoveAll(s => s == "");
                        agregateoperationnorm.RemoveAll(s => s == "");
                        for (int i=0; i< agregateoperation.Count; i++) // добавляем новые операции агрегата
                        {                          
                            var oa = dataContext.OperationsOfAgregate.Add(new OperationsOfAgregate
                            {
                                TechnologicalOperations_Id = Convert.ToInt32(agregateoperation[i]),
                                SeedingRate = float.Parse(agregateoperationnorm[i]),
                                GSMCharge = float.Parse(agregateoperationgsm[i]),
                                Agregates_Id = id
                            });
                        }
                    }
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

        }

     
        #endregion

        #region del
        public static bool DeleteMachinery(int idFarm, int id)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    var str = dataContext.Machinery.FirstOrDefault(p => p.Id == id);
                    var ml = dataContext.Machinery.Where(p => p.Name == str.Name).Count();
                    var cm = dataContext.ClassMachine.First(p => p.Name == str.Name);
                    if (ml == 1)
                    {
                        var atd = dataContext.Agregates.Where(p => p.ClassMachine_Id == cm.Id);
                        foreach (var t in atd)
                        {
                            t.ClassMachine_Id = null;
                            t.KindOfFuel_Id = null;
                            t.Trailers_Id = null;
                            dataContext.Agregates.Remove(t);
                        }
                    }
                    dataContext.Machinery.Remove(str);
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public static bool DeleteAgregate(int idFarm, int id)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    var str = dataContext.Agregates.First(p => p.Id == id);
                    var oa = dataContext.OperationsOfAgregate.Where(s => s.Agregates_Id == id);
                    foreach (var t in oa)
                    {
                        t.Agregates_Id = null;                       
                        dataContext.OperationsOfAgregate.Remove(t);
                    }
                    dataContext.Agregates.Remove(str);
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public static bool DeleteTrailer(int idFarm, int id)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    var str = dataContext.Trailers.FirstOrDefault(p => p.Id == id);
                    var att = dataContext.Agregates.Where(p => p.Trailers_Id == str.Id);
                    foreach (var t in att)
                    {
                        t.ClassMachine_Id = null;
                        t.KindOfFuel_Id = null;
                        t.Trailers_Id = null;
                        dataContext.Agregates.Remove(t);
                    }
                    dataContext.Trailers.Remove(str);
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }


        #endregion

        #endregion

        #region KindOfFuel

        #region get
        public static List<Fuels> GetKindOfFuel(int idFarm)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.KindOfFuel.Where(p => p.Farm.Id == idFarm);
                var res = new List<Fuels>();
                if (str != null)
                {
                    foreach (var s in str)
                    {
                        res.Add(new Fuels(s.Id, s.Name, s.Unit, s.Cost));
                    }
                    return res;
                }
                else
                    return null;
            }
        }

        public static Fuels GetKindOfFuel(int idFarm, int idAg)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var idFuel = dataContext.Agregates.First(p => p.Id == idAg).KindOfFuel_Id;
                var s = dataContext.KindOfFuel.FirstOrDefault(p => p.Farm.Id == idFarm && p.Id == idFuel);                
                if (s != null)  
                    return new Fuels(s.Id, s.Name, s.Unit, s.Cost);                
                else
                    return null;
            }
        }
        public static List<string> GetFuelNames(int idFarm)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.KindOfFuel.Where(c => c.Farm.Id == idFarm);
                var fn = new List<string>();
                if (str != null)
                {
                    foreach (var s in str)
                    {
                        fn.Add(s.Name);
                    }
                    return fn;
                }
                else
                    return null;
            }
        }
        #endregion        

        #region set
        public static bool SetKindOfFuel(int idFarm, int id, string Name, string Unit, string Cost)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    float c;
                    float.TryParse(Cost, out c);
                    if (id == -1)
                    {
                        var f = dataContext.Farm.FirstOrDefault(cc => cc.Id == idFarm);

                        if (dataContext.KindOfFuel.Where(p => p.Name == Name && p.Farm.Id == idFarm).FirstOrDefault() == null)
                            dataContext.KindOfFuel.Add(new KindOfFuel { Name = Name, Unit = Unit, Cost = c, Farm = f });
                        else
                            return false;
                    }
                    else
                    {
                        var str = dataContext.KindOfFuel.Where(p => p.Farm.Id == idFarm && p.Id == id).First();
                        str.Name = Name;
                        str.Unit = Unit;
                        str.Cost = c;
                    }
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #region del
        public static bool DeleteKindOfFuel(int idFarm, int id)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    var str = dataContext.KindOfFuel.Where(p => p.Farm.Id == idFarm && p.Id == id).First();
                    dataContext.KindOfFuel.Remove(str);
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #endregion

        #region KindOfWork

        #region get        
        #endregion

        #region set
        public static bool SetKindOfWork(int id, string Name, string Group)
        {            
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    if (id == -1)
                    {
                        //Group="" - добавление группы
                        //Group = I, II,... - добавление подгруппы
                        string IndexGroup = "";
                        if (dataContext.KindOfWork.FirstOrDefault(p => p.Name == Name) == null)
                        {
                            if (Group == "")
                            {
                                //Найдем все индексы групп в KindOfWork                                
                                var GroupOp = dataContext.KindOfWork.Where(p => p.Index.Contains("_") == false).ToList();
                                int i = 0;
                                bool f = false;
                                while (!f)
                                {
                                    if (GroupOp.FirstOrDefault(c => c.Index == RimskNumeric.ElementAt(i).Key) == null)
                                    {
                                        IndexGroup = RimskNumeric.ElementAt(i).Key;
                                        f = true;
                                    }
                                    i++;
                                }
                                dataContext.KindOfWork.Add(new KindOfWork { Name = Name, Index = IndexGroup });
                            }
                            else
                            {
                                string IndexSubGroup = "";
                                //Найдем все индексы групп в KindOfWork
                                var GroupOp = dataContext.KindOfWork.Where(p => p.Index.StartsWith(Group + "_")).ToList();
                                int i = 1;
                                bool f = false;
                                while (!f)
                                {
                                    if (GroupOp.FirstOrDefault(c => c.Index == Group + "_" + i.ToString()) == null || GroupOp.Count()==0)
                                    {
                                        IndexSubGroup = Group + "_" + i.ToString();
                                        f = true;
                                    }
                                }
                                dataContext.KindOfWork.Add(new KindOfWork { Name = Name, Index = IndexSubGroup });
                            }
                        }
                        else
                            return false;
                    }  
                    else
                    {
                        //Group содержит индекс группы или подгруппы
                        var str = dataContext.KindOfWork.Where(p => p.Id == id).First();
                        str.Name = Name;                        
                    }
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #region del
        #endregion

        #endregion

        #region TehnologicalOperations

        #region get        
        #endregion

        #region set 
      /*  public static bool SetTehOp(int id, string Name, string Annot, string Applic, string Unit, string Rang, string KofW)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    var kw = KofW.Split('_');
                    if (id == -1)
                    {
                        var KodOp = "";
                        var KindOfW = dataContext.KindOfWork.FirstOrDefault(p => p.Index == KofW);
                        int i = 1;
                        bool f = false;
                        while (!f)
                        {
                            if (KodOperation == Alphavit.ElementAt(i).Key)
                            {
                                IndexGroup = RimskNumeric.ElementAt(i).Key;
                                f = true;
                            }
                            i++;
                        }
                        if (dataContext.TechnologicalOperations.FirstOrDefault(p => p.Name == Name) == null)
                            dataContext.TechnologicalOperations.Add(new TechnologicalOperations
                            {
                                KodOperation = KodTehOp,
                                Name = Name,
                                Annotation=Annot,
                                Application=Applic,
                                Unit = Unit,
                                Rang = Convert.ToInt32(Rang),
                                KindOfWork_Id = dataContext.KindOfWork.FirstOrDefault(p => p.Index == kw[0]).Id
                            });
                        else
                            return false;
                    }
                    else
                    {
                        
                        var str = dataContext.TechnologicalOperations.First(p => p.Id == id);                        
                        str.Name = Name;
                        str.Annotation = Annot;
                        str.Application = Applic;
                        str.Unit = Unit;
                        str.Rang = Convert.ToInt32(Rang);
                        str.KindOfWork_Id = dataContext.KindOfWork.FirstOrDefault(p => p.Index == KofW).Id;
                    }
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }*/
        #endregion

        #region del        
        #endregion

        #endregion

        #region TechPacket
        #region get
        //GetBreakingList
        //Machinery with its Mechanizators
        /// <summary>
        /// Возвращает пакет технлогических операций
        /// </summary>
        /// <param name="idFarm"></param>
        /// <returns></returns>
        public static List<TechResult> GetTechPacket(List<int> idTKR)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                List<TechResult> result = new List<TechResult>();
                try
                {
                    foreach (var tkr in idTKR)
                    {
                        var tk = dataContext.TKR.Where(p => p.Id == tkr).First();
                        var resTK = new TechResult(tk.Id, tk.Name);// добавили имя техкарты
                        resTK.OnWaGroup = new List<WaResult>();
                        var soil = new List<string>();
                        var strWa = new List<string>();
                        #region
                        foreach (var t in tk.WA_TKR)
                        {
                            var ss = t.WorkAreas.Soil.Name;
                            if (soil.Contains(ss))
                            {
                                var i = soil.FindIndex(p => p == ss);
                                strWa[i] += "," + t.WorkAreas.NumUch;
                            }
                            else
                            {
                                soil.Add(ss);
                                strWa.Add(t.WorkAreas.NumUch);
                            }
                        }
                        #endregion
                        for (var j = 0; j < soil.Count(); j++)
                        {
                            var waRes = new WaResult(soil[j], strWa[j]);//для почвы и группы раб.уч.добавляем
                            waRes.ListTPacket = new List<ListTPacket>();
                            //список пакетов ТК
                            var lv = tk.Intensification.Level_Int;
                            var tN = tk.Predshest.Name;
                            var cN = tk.Cultures.Name;
                            var so = soil[j];
                            var teckP = dataContext.TechPacket.Where(p => p.Intensification.Level_Int == lv &&
                                                p.Predshest.Name == tN && p.Cultures.Name == cN && p.Soil.Name == so).ToList();

                            var lTP = ListTechResult(teckP, dataContext);
                            waRes.ListTPacket.Add(lTP);//добавляем для почвы и групп раб.уч.все полученные пакеты - ТК
                            resTK.OnWaGroup.Add(waRes);
                        }
                        result.Add(resTK);
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        //расшифровка техпакета
        public static ListTPacket ListTechResult(List<TechPacket> teckP, StamatDBEntities dataContext)
        {
            var lTP = new ListTPacket();
            lTP.TechP = new List<TP_TKR>();
            foreach (var v in teckP)
            {
                var tk_TP = new TP_TKR();
                tk_TP.IdTechP = v.Id;
                tk_TP.Name = v.Name;
                tk_TP.Operations = new List<TP>();
                #region  расщифровка
                v.Operations = Cyrillify(v.Operations);
                string[] s1 = v.Operations.Split(';');
                string[][] s2 = new string[s1.Count()][];
                List<List<string>> rlt = new List<List<string>>();
                for (int i = 0; i < s1.Count(); i++)
                {
                    List<string> miniRes = new List<string>();
                    s2[i] = s1[i].Split(':');
                    var s3 = s2[i][1].Split(',');
                    miniRes.Add(s2[i][0]);
                    foreach (var jj in s3)
                        miniRes.Add(jj);
                    rlt.Add(miniRes);
                }
                #endregion
                //идем по видам работ
                foreach (var vv in rlt)
                {
                    var id = vv[0];
                    var nm = dataContext.KindOfWork.Where(p => p.Index == id).First();
                    for (int k = 1; k < vv.Count; k++)
                    {
                        var tp = new TP();
                        id = vv[k];
                        var id_n = id.Split(new char[] { '(', ')' });
                        if (id_n.Count() > 1)
                        {
                            id = id_n[1];
                            tp.Name = "*";
                        }
                        else
                            id = id_n[0];
                        var op = nm.TechnologicalOperations.Where(p => p.KodOperation == id).First();
                        tp.IdOperation = op.Id;
                        tp.KodOperation = op.KodOperation;
                        tp.Name += op.Name;
                        tp.Application = op.Application;
                        tp.Technics = op.Parametrs;
                        tk_TP.Operations.Add(tp);//добавили отдельную операцию в ТК
                    }
                }
                lTP.TechP.Add(tk_TP);//добавляем полученный пакет - ТК
            }
            return lTP;
        }

        public static ListTPacket ListTechResultOnId(int idTeckP, StamatDBEntities dataContext)
        {
            var tehp = dataContext.TechPacket.First(c => c.Id == idTeckP);
            var lTP = new ListTPacket();
            lTP.TechP = new List<TP_TKR>();
            
                var tk_TP = new TP_TKR();
                tk_TP.IdTechP = tehp.Id;
                tk_TP.Name = tehp.Name;
                tk_TP.Operations = new List<TP>();
            #region  расщифровка
                tehp.Operations = Cyrillify(tehp.Operations);
                string[] s1 = tehp.Operations.Split(';');
                string[][] s2 = new string[s1.Count()][];
                List<List<string>> rlt = new List<List<string>>();
                for (int i = 0; i < s1.Count(); i++)
                {
                    List<string> miniRes = new List<string>();
                    s2[i] = s1[i].Split(':');
                    var s3 = s2[i][1].Split(',');
                    miniRes.Add(s2[i][0]);
                    foreach (var jj in s3)
                        miniRes.Add(jj);
                    rlt.Add(miniRes);
                }
                #endregion
                //идем по видам работ
                foreach (var vv in rlt)
                {
                    var id = vv[0];
                    var nm = dataContext.KindOfWork.Where(p => p.Index == id).First();
                    for (int k = 1; k < vv.Count; k++)
                    {
                        var tp = new TP();
                        id = vv[k];
                        var id_n = id.Split(new char[] { '(', ')' });
                        if (id_n.Count() > 1)
                        {
                            id = id_n[1];
                            tp.Name = "*";
                        }
                        else
                            id = id_n[0];
                        var op = nm.TechnologicalOperations.Where(p => p.KodOperation == id).First();
                        tp.IdOperation = op.Id;
                        tp.KodOperation = op.KodOperation;
                        tp.Name += op.Name;
                        tp.Application = op.Application;
                        tp.Technics = op.Parametrs;
                        tk_TP.Operations.Add(tp);//добавили отдельную операцию в ТК
                    }
                
                
            }
            lTP.TechP.Add(tk_TP);//добавляем полученный пакет - ТК
            return lTP;
        }

        public static List<TechResult> GetTechPacket(List<miniTK> TK, int idFarm)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                List<TechResult> result = new List<TechResult>();
                try
                {
                    List<int> idTKR = new List<int>();
                    foreach (var tkr in TK)
                    {
                        var tk = dataContext.TKR.FirstOrDefault(p => p.Name == tkr.nameTK && p.Farm_Id == idFarm);
                        idTKR.Add(tk.Id);
                    }
                    
                    var res = GetTechPacket(idTKR);
                    
                }
                catch (Exception ex)
                {
                    return null;
                }


                return result;

            }
        }
        //SickList
        //StakesMech
        //Mechanizator with its Machineries
        //Trailers
        //KindFuel
        //Agregaters
        #endregion
        #endregion

        #region MTPPacket

        #region SaveVarTechPaket
        public static bool SaveVarTechPaket(string[] techP)
        {
            //срока techP имеет вид: IdTKR-IdTechPacket по всем выбранным вариантам
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    for(int i=0;i< techP.Count();i++)
                    {
                        var tp = techP[i].Split('-');
                        var tkr= dataContext.TKR.FirstOrDefault(c => c.Id == Convert.ToInt32(tp[0]));
                        var techPacket = dataContext.TechPacket.FirstOrDefault(c => c.Id == Convert.ToInt32(tp[1]));
                    }
                    /*float c;
                    float.TryParse(Cost, out c);
                    if (id == -1)
                    {
                        var f = dataContext.Farm.FirstOrDefault(cc => cc.Id == idFarm);

                        if (dataContext.KindOfFuel.Where(p => p.Name == Name && p.Farm.Id == idFarm).FirstOrDefault() == null)
                            dataContext.KindOfFuel.Add(new KindOfFuel { Name = Name, Unit = Unit, Cost = c, Farm = f });
                        else
                            return false;
                    }
                    else
                    {
                        var str = dataContext.KindOfFuel.Where(p => p.Farm.Id == idFarm && p.Id == id).First();
                        str.Name = Name;
                        str.Unit = Unit;
                        str.Cost = c;
                    }
                    dataContext.SaveChanges();*/
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion
        #endregion 

        #region TechAgro

        #region setTechAgroTO
        //добавление информации о технологических операциях для конкретного варианта
        public static bool SetTechAgroTO(int id, int idTechAgro, List<TechAgroTo> idTechPaket)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    var TechAgroTo = dataContext.TehAgroTO.Where(p => p.TehAgro.Id == idTechAgro).DefaultIfEmpty(null);
                    if (TechAgroTo != null)// в таблице TehAgroTO есть записи для данного варианта технологий
                    {//удаляем их все
                        foreach(var to in TechAgroTo)                        
                            dataContext.TehAgroTO.Remove(to);
                        dataContext.SaveChanges();
                    }                    
                    foreach (var idTo in idTechPaket)
                    {//запсываем записи вновь
                        dataContext.TehAgroTO.Add(new TehAgroTO
                        {
                            IdTO = idTo.IdTO,
                            DateBegin = idTo.DateBegin,
                            DateEnd = idTo.DateEnd,
                            ChasSm = idTo.ChasSm,
                            Rast = idTo.Rast,
                            StoimPodv = idTo.StoimPodv,
                            StoimS = idTo.StoimS,
                            RankWorker = idTo.RankWorker,
                            KolWorker = idTo.KolWorker,
                            PovOPl = idTo.PovOPl,
                            ThisH = idTo.ThisH,
                            RashHim = idTo.RashHim,
                            StoimHim = idTo.StoimHim,
                            Koef = idTo.Koef,
                            ThisF = idTo.ThisF,
                            FlgUpak = idTo.FlgUpak,
                            URash = idTo.URash,
                            VUpak = idTo.VUpak,
                            StoimUpak = idTo.StoimUpak,
                            TehAgro = dataContext.TehAgro.First(p => p.Id == idTechAgro)
                        });
                    }
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #region setTechAgro
        public static bool SetTechAgro(int id, int idFarm, string nVar, int idTKR, string gWA, int idTechPaket)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    if (id == -1)//добавление нового варианта технологий
                    {
                        var ta = dataContext.TehAgro.FirstOrDefault(p => p.nameVar == nVar && p.IdFarm == idFarm && p.IdTKR==idTKR);
                        if (ta == null)
                        { 
                        dataContext.TehAgro.Add(new TehAgro
                        {
                            IdFarm = idFarm,
                            nameVar = nVar,
                            IdTKR = idTKR,
                            GroupWA = gWA,
                            IdTechPaket = idTechPaket
                        });
                    }
                    else
                        return false;                        
                    }
                    else
                    {
                        var str = dataContext.TehAgro.Where(p => p.Id == id).First();
                        str.nameVar = nVar;
                        str.IdTKR = idTKR;
                        str.GroupWA = gWA;
                        str.IdTechPaket= idTechPaket;
                    }
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #region setTehKart
        public static bool SetTechKart(int id, List<string> idTKR_gWA_idOP, List<string> dateB, List<string> dateE)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                try
                {
                    List<string[]> IdTKR_gWA_idOP = new List<string[]>();
                    foreach(var i in idTKR_gWA_idOP)
                    {
                        var str = i.Split('-').ToArray();                        
                        IdTKR_gWA_idOP.Add(new string[] { str[0], str[1], str[2] });                                                
                    }
                    if (id==-1)
                    {
                        for (int i =0; i< dateB.Count();i++)
                        {
                            int id0 = Convert.ToInt32(IdTKR_gWA_idOP.ElementAt(i).ElementAt(0));
                            var wa = IdTKR_gWA_idOP.ElementAt(i).ElementAt(1).Split(',').ToArray();
                            int id2 = Convert.ToInt32(IdTKR_gWA_idOP.ElementAt(i).ElementAt(2));
                            for (int w = 0; w < wa.Count() - 1; w++)
                            {
                                int id1= dataContext.WA_TKR.FirstOrDefault(c=>c.TKR.Farm.Id==id0 && c.WorkAreas.NumUch == wa.ElementAt(w)).WorkAreas.Id;                                
                                var ta = dataContext.TechKart.FirstOrDefault(p => p.TKR.Id == id0 && p.WorkAreas.Id==id1 && p.TechnologicalOperations.Id == id2);
                                if (ta == null)
                                {
                                    dataContext.TechKart.Add(new TechKart
                                    {
                                        TKR_Id =id0,
                                        WorkAreas_Id = id1,
                                        TechnologicalOperations_Id = id2,
                                        DateBegin = Convert.ToDateTime(dateB[i]),
                                        DateEnd = Convert.ToDateTime(dateE[i])
                                    });
                                }
                                else
                                    return false;
                            }
                        }

                    }
                    else
                    {
                       /* for (int i = 0; i < dateB.Count(); i++)
                        {
                            var str = dataContext.TechKart.Where(p => p.Id == id).First();
                            str.TKR_Id = IdTKR_gWA_idOP[i][0];
                            str.WorkAreas_Id = IdTKR_gWA_idOP[i][1];
                            str.TechnologicalOperations_Id = IdTKR_gWA_idOP[i][2];
                            str.DateBegin = Convert.ToDateTime(dateB[i]);
                            str.DateEnd = Convert.ToDateTime(dateE[i]);
                        }*/
                    }
                    dataContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #region getTechKart
        public static List<TehKart> GetTechKatr(int idFarm, int year)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                if (year == 0)//получаем список всех ткр
                {
                    var str = dataContext.TechKart.Where(p => p.TKR.Farm_Id == idFarm).ToList();
                    var res = new List<TehKart>();
                    if (str.Count() != 0)
                        foreach (var s in str)
                        {
                            res.Add(new TehKart(s.Id, s.TKR_Id, s.WorkAreas.NumUch, s.TechnologicalOperations.Name, 100f, s.DateBegin, s.DateEnd));
                        }
                    return res;
                }
                else
                {
                    var str = dataContext.TechKart.Where(p => p.TKR.Farm_Id == idFarm && p.TKR.YearCrop == year).ToList();
                    var res = new List<TehKart>();
                    if (str.Count() != 0)
                        foreach (var s in str)
                        {
                            res.Add(new TehKart(s.Id, s.TKR_Id, s.WorkAreas.NumUch, s.TechnologicalOperations.Name, 100f, s.DateBegin,s.DateEnd));
                        }
                    return res;
                }
            }



        }
        #endregion

        #region getTechAgro
        public static List<TechAgroVar> GetTechAgro(int idFarm)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var str = dataContext.TehAgro.Where(p => p.IdFarm == idFarm).ToList();
                var res = new List<TechAgroVar>();
                if (str.Count()!= 0)
                    foreach (var s in str)
                    {
                        var ListTO = ListTechResultOnId(s.IdTechPaket, dataContext);
                        res.Add(new TechAgroVar(s.Id, s.IdFarm, ListTO, s.GroupWA, s.IdTKR, s.nameVar, s.IdTechPaket));
                    }
                return res;
            }
                
            
        }

        public static List<TechAgroVar> GetTechAgro(int idFarm, List<string[]> listIdTehPac)
        {//сделать вариант возвращения не всех вариантов по хозяйству а выборочных
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                List<TehAgro> str = new List<TehAgro>();
                foreach(var idTP in listIdTehPac)
                {
                    int idTkr = Convert.ToInt32(idTP[0]);
                    int idTPaket = Convert.ToInt32(idTP[2]);
                    string groupWA = idTP[1];
                    str.Add(dataContext.TehAgro.First(p => p.IdFarm == idFarm && p.IdTechPaket== idTPaket && p.IdTKR == idTkr && p.GroupWA == groupWA));
                }                
                var res = new List<TechAgroVar>();
                foreach (var s in str)
                {
                    var ListTO = ListTechResultOnId(s.IdTechPaket, dataContext);
                    res.Add(new TechAgroVar(s.Id, s.IdFarm, ListTO, s.GroupWA, s.IdTKR, s.nameVar, s.IdTechPaket));
                }
                return res;
            }


        }

        public static List<TechnologicalOperations> GetTO()
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var res = dataContext.TechnologicalOperations.DefaultIfEmpty(null).ToList();                
                
                return res;
            }


        }
        #endregion

        #endregion

        #region Algorithm
        public static float StMeh(float tg) //Формирование расчётных групп        
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {

                var Meh = dataContext.Mechanizator.ToList();
                foreach (var v in Meh)
                {
                    if (v.Class == tg.ToString())
                    {
                        var StMeh = dataContext.StakesOfMech.First(p => p.Id == v.StakesOfMech.Id).Stake;
                        return StMeh;
                    }
                }
                
                return 0;
            }
        }

        public static List<List<RGroups>> GetRGroups(List<int> techkarts, int idFarm) //Формирование расчётных групп        
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                // Сначала получим выборку из таблицы TechKart                
                var reslist = new List<RGroups>();
                foreach (var ss in techkarts) 
                {
                    var str = dataContext.TechKart.Where(c => c.TKR_Id == ss);
                        foreach (var tt in str)
                        {
                        var pole = dataContext.WorkAreas.FirstOrDefault(c => c.Id == tt.WorkAreas_Id && c.Farm.Id==idFarm);
                        var oper = dataContext.TechnologicalOperations.FirstOrDefault(c => c.Id == tt.TechnologicalOperations_Id);
                        var agrs = dataContext.OperationsOfAgregate.Where(c =>c.Agregates.Farm.Id == idFarm && c.TechnologicalOperations_Id == oper.Id).ToList();
                        var AgList = new List<AgregateInfo>();                        
                        foreach (var aa in agrs)
                        {
                            var Machine = dataContext.Machinery.First(p => p.ClassMachine.Id == aa.Agregates.ClassMachine.Id);
                            var Trailer = dataContext.Trailers.First(p => p.Id == aa.Agregates.Trailers.Id);
                            var ZarPl = dataContext.StakesOfMech.First(p => p.Farm.Id == idFarm && p.Rank == tt.TechnologicalOperations.Rang).Stake;
                            float koef = 0, cost = 0, costGSMonGa = 0, PercentM = 0, PercentT = 0, ToM = 0, ToT = 0, PriceM = 0, PriceT = 0;
                            int NormZM = 0, NormZT = 0, idT, idM;
                            if (Machine.Name!= "-")
                            {
                                koef = Machine.Kind == "Отечественный" ? 1.1f :.25f;
                                PercentM = Machine.PercentAmort;
                                ToM = Machine.PersentAmortOfTO;
                                NormZM = Machine.NormZagruz;
                                PriceM = Machine.Price;
                            }
                            if (Trailer.Name != "-")
                            {                                
                                PercentT = Trailer.PercentAmort;
                                ToT = Trailer.PersentAmortOfTO;
                                NormZT = Convert.ToInt32(Trailer.NormZagruz);
                                PriceT = Trailer.Price;
                            }                             
                            cost = dataContext.KindOfFuel.First(p => p.Id == aa.Agregates.KindOfFuel.Id).Cost;
                            costGSMonGa = aa.GSMCharge * cost * koef;
                            
                            idT = Trailer.Id;
                            idM = Machine.Id;
                            var StM = StMeh(aa.Agregates.ClassMachine.TyagClass);  
                            
                            AgList.Add(new AgregateInfo(aa.Agregates.Id, aa.Agregates.Name, aa.SeedingRate, aa.GSMCharge, koef, cost, 
                                PercentM, PercentT, ToM, ToT, PriceM, PriceT, NormZM, NormZT, idM, idT, aa.Agregates.Count, StM, aa.Agregates.ShirZahvat, ZarPl));
                        }                           
                        reslist.Add (new RGroups(ss,tt.Id, tt.DateBegin, tt.DateEnd, tt.WorkAreas_Id, pole.Square, pole.MaxShirZahv, 
                                               tt.TechnologicalOperations_Id, oper.Name, AgList));                       
                        }
                }
                reslist.Sort(delegate (RGroups r1, RGroups r2) { return r1.BeginDate.CompareTo(r2.BeginDate); }); // Отсортируем reslist по датам начала
                // Теперь единую группу операций reslist разобъём на расчётные группы по непересекающимся периодам дат с занесением в список res
                // К базам данных больше не обращаемся, используем только список reslist
                for (int i=0; i < reslist.Count();)
                {
                    var r = reslist.Where(p => p.BeginDate == reslist[i].BeginDate).ToList();
                    if (r.Count() > 1)
                    {
                        r.Sort(delegate (RGroups r1, RGroups r2) { return r1.EndDate.CompareTo(r2.EndDate); });
                        for (int j = 0; j < r.Count(); j++)
                        {
                            reslist[i] = r[j];
                            i++;
                        }                        
                    }
                    else
                        i++;
                }
                var res = new List<List<RGroups>>();                                
                var GroupStart = reslist[0].BeginDate;
                var GroupFinish = reslist[0].BeginDate;
                while (GroupStart <= reslist[reslist.Count-1].BeginDate)
                {
                    var ActiveGroup = new List<RGroups>();
                    for (int i = 0; i < reslist.Count; i++)
                    {
                        if ((reslist[i].BeginDate <= GroupFinish) & (reslist[i].EndDate >= GroupStart))
                        {                            
                            ActiveGroup.Add(reslist[i]);
                            if (GroupStart > reslist[i].BeginDate)
                               { GroupStart = reslist[i].BeginDate; }
                            if (GroupFinish < reslist[i].EndDate)
                               { GroupFinish = reslist[i].EndDate; }                                                              
                        }                        
                    }
                    if (ActiveGroup.Count != 0) { res.Add(ActiveGroup);}
                    GroupStart = GroupFinish.AddDays(1);
                    GroupFinish = GroupStart;
                } 
                return res;
            }
        }


        /// <summary>
        /// возвращает '0', если в хозяйстве техники хватает
        /// '1' в хозяйстве не хватает сельхозмашин
        /// '2' в хозяйстве не хватает тракторов      
        /// '3' в хозяйстве не хватает как тракторов, так и сельхозмашин
        /// </summary>        
       /* public static List<NecessTechnics> CountAg(int idFarm, int idAg, int countAg)
        {
            using (var dataContext = new StamatDBEntities(ConnectionString))
            {
                var Agreg = dataContext.Agregates.First(p => p.Id == idAg);
                var Mashine = dataContext.Machinery.Where(p => p.Name == nameM && p.Farm.Id == idFarm).Count();
                var Trailer = dataContext.Trailers.First(p => p.Name == nameT && p.Farm.Id == idFarm).Count;
                if (Mashine < countAg)
                {
                    if (Trailer < countAg * Agreg.Count)
                        return 3;//в хозяйстве не хватает как тракторов, так и сельхозмашин
                    else
                        return 2;//в хозяйстве не хватает тракторов                
                }
                else
                    if (Trailer < countAg * Agreg.Count)
                        return 1;//в хозяйстве не хватает сельхозмашин
                return 0;                
            }
        }*/

        #endregion


    }


}









       