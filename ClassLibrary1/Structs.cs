using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Checked
    {
        public bool Check;
        public int Id;

        public Checked(bool check, int id)
        {
            Check = check;
            Id = id;
        }
    }

    public class Soile
    {
        public int Id;
        public string Name;
        public bool Editable;

        public Soile(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class WArea
    {
        public int Id;
        public string Name;
        public double Square;
        public double MaxShirZ;
        public double Lenght;
        public double Wight;
        public double Angle_gr;
        public double KolokPercent;
        public string SoilType;
        public double Distanse;
        public bool YorN;

        public WArea(int id, string name, double square, double maxShZ,
            double lenght, double wight, double angle, double kolok, double distanse, string soil, bool yorn)
        {
            Id = id;
            Name = name;
            Square = square;
            MaxShirZ = maxShZ;
            Lenght = lenght;
            Wight = wight;
            Angle_gr = angle;
            KolokPercent = kolok;
            Distanse = distanse;
            SoilType = soil;
            YorN = yorn;
        }
    }

    public class TK_dop
    {
        public int Id_Wa;
        public string Sort;
        public List<Diseas> Dis;
        public List<Pests> Pes;
        public List<Stabil> Stab;

        public TK_dop(int id, string sort, List<Diseas> dis, List<Pests> pes, List<Stabil> stab)
        {
            Id_Wa = id;
            Sort = sort;
            Dis = dis;
            Pes = pes;
            Stab = stab;
        }
    }

    public class TehKR
    {
        public string Culture;
        public string Intensification;
        public int Id;
        public List<WArea> WA;
        public List<TK_dop> TK_dop;
        public int Year;
        public string Name;
        public string Pred;

        public TehKR(int id, int year, string cult, string intens, string pred, List<WorkAreas> WorkA, List<WA_TKR> tk_dop)
        {
            Id = id;
            Year = year;
            Culture = cult;
            Intensification = intens;
            Pred = pred;
            var wA = new List<WArea>();
            Name = year + "/" + intens + "/" + cult + "/" + pred;
            foreach (var v in WorkA)
            {
                wA.Add(new WArea(v.IdUch, v.NumUch, v.Square, v.MaxShirZahv, v.Lenght, v.Wight, v.Angle_gr, v.KolokPercent, v.Distanse, v.Soil.Name, true));
            }
            WA = wA;
            var tkD = new List<TK_dop>();
            foreach (var v in tk_dop)
            {
                var lD = new List<Diseas>();
                foreach (var d in v.Disease)
                {
                    lD.Add(new Diseas(d.Id, d.Name, d.Value));
                }
                var lP = new List<Pests>();
                foreach (var d in v.Pest)
                {
                    lP.Add(new Pests(d.Id, d.Name, d.Value));
                }
                var lS = new List<Stabil>();
                foreach (var d in v.Stability)
                {
                    lS.Add(new Stabil(d.Id, d.Name, d.Value));
                }
                if (v.Sort != null)
                    tkD.Add(new TK_dop(v.WorkAreas.IdUch, v.Sort.Name, lD, lP, lS));
                else
                    tkD.Add(new TK_dop(v.WorkAreas.IdUch, "", lD, lP, lS));
            }
            TK_dop = tkD;
        }
    }

    public class Climat
    {
        public int Id;
        public string Name;
        public int IdFarm;
        public string SummTempVegPer_5_10;
        public string SummTempVegPer_10_12;
        public string KolOsS;
        public string KolOsadkovJune;
        public string DataBeg;
        public string DataEnd;
        public string KoefUvl;
        public bool Editable;

        public Climat(int id, string name, string summTempVegPer_5_10, string summTempVegPer_10_12, string dataB, string dataE, string kolOsadS, string kolOsadkovJune, string koefUvl)
        {
            Id = id;
            Name = name;
            SummTempVegPer_5_10 = summTempVegPer_5_10;
            SummTempVegPer_10_12 = summTempVegPer_10_12;
            KolOsS = kolOsadS;
            DataBeg = dataB;
            DataEnd = dataE;
            KoefUvl = koefUvl;
            KolOsadkovJune = kolOsadkovJune;
        }

        public Climat(int id, int idFarm, string name, string summTempVegPer_5_10, string summTempVegPer_10_12, string dataB, string dataE, string kolOsadS, string kolOsadkovJune, string koefUvl)
        {
            Id = id;
            IdFarm = idFarm;
            Name = name;
            SummTempVegPer_5_10 = summTempVegPer_5_10;
            SummTempVegPer_10_12 = summTempVegPer_10_12;
            KolOsS = kolOsadS;
            DataBeg = dataB;
            DataEnd = dataE;
            KoefUvl = koefUvl;
            KolOsadkovJune = kolOsadkovJune;
        }
    }

    public class Culture
    {
        public int Id;
        public string Name;
        public bool Editable;

        public Culture(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class ClassMashin
    {
        public int Id;
        public string Name;
        public float TyagClass;

        public ClassMashin(int id, string name, float tyaga)
        {
            Id = id;
            Name = name;
            TyagClass = tyaga;
        }
    }

    public class Sorte
    {
        public int Id;
        public string Name;
        public float SeedingRase;
        public float M1000;
        public float CountSeedOnGa;
        public bool Editable;
        public double Productivity;
        public double ProductivityMax;
        public int GrowingSeason_0;
        public int GrowingSeason_1;
        public string nameCult;

        public Sorte(int id, string name, float seedingRase, float m1000, float countSeedOnGa, double productivity, double productivityMax,
            int growingSeason_0, int growingSeason_1, string nameCult)
        {
            Id = id;
            Name = name;
            SeedingRase = seedingRase;
            M1000 = m1000;
            CountSeedOnGa = countSeedOnGa;
            Productivity = productivity;
            ProductivityMax = productivityMax;
            GrowingSeason_0 = growingSeason_0;
            GrowingSeason_1 = growingSeason_1;
            this.nameCult = nameCult;
        }
    }

    public class TSort
    {
        public int Id;
        public string Name;
        public int SumTemp;
        public int Temp;

        public TSort(int id, string name, int sTemp, int temp)
        {
            Id = id;
            Name = name;
            SumTemp = sTemp;
            Temp = temp;
        }
    }

    public class Diseas
    {
        public int Id;
        public string Name;
        public int Value;
        public bool Editable;

        public Diseas(int id, string name, int value)
        {
            Id = id;
            Name = name;
            Value = value;
        }
    }

    public class Pests
    {
        public int Id;
        public string Name;
        public int Value;
        public bool Editable;

        public Pests(int id, string name, int value)
        {
            Id = id;
            Name = name;
            Value = value;
        }
    }

    public class Stabil
    {
        public int Id;
        public string Name;
        public int Value;
        public bool Editable;

        public Stabil(int id, string name, int value)
        {
            Id = id;
            Name = name;
            Value = value;
        }
    }

    public class Himiz
    {
        public int Id;
        public string Name;
        public string Unit;
        public bool Editable;

        public Himiz(int id, string name, string unit)
        {
            Id = id;
            Name = name;
            Unit = unit;
        }
    }

    public class Machin
    {
        public int Id;
        public string Name;
        public string Type;
        public float Price;
        public string Kind;
        public string SerialNumber;
        public int NormZagruz;
        public float PersentAmortOfTO;
        public float PersentAmort;
        public string ClassMachine;
        public int Farm_Id;
        public bool Editable;

        public Machin(int id, string name, string type, float price, string kind, string serialNumber, int normZagruz,
            float persentAmortOfTO, float persentAmort, string classMachine, int farm_Id)
        {
            Id = id;
            Name = name;
            Type = type;
            Price = price;
            Kind = kind;
            SerialNumber = serialNumber;
            NormZagruz = normZagruz;
            PersentAmortOfTO = persentAmortOfTO;
            PersentAmort = persentAmort;
            ClassMachine = classMachine;
            Farm_Id = farm_Id;
        }
    }

    public class Trailing // Скопировано с Machine (выше) и переделано под Trailers
    {
        public int Id;
        public string Name;
        public float Price;
        public float NormZagruz; // В таблице БД тип данных REAL
        public float PersentAmortOfTO;
        public float PersentAmort;
        public int Quantity;
        public int Farm_Id;
        public bool Editable;

        public Trailing(int id, string name, float price, float normZagruz,
            float persentAmortOfTO, float persentAmort, int quantity, int farm_Id)
        {
            Id = id;
            Name = name;
            Price = price;
            NormZagruz = normZagruz;
            PersentAmortOfTO = persentAmortOfTO;
            PersentAmort = persentAmort;
            Quantity = quantity;
            Farm_Id = farm_Id;
        }
    }
    public class Agreg // Скопировано с Trailing (выше) и переделано под Agregates
    {
        public int Id;
        public string Name;
        public int Quantity;
        public float Shirina;
        public string ClassMachine; // здесь string, а в БД это числовой айди
        public string Fuel; // здесь string, а в БД это числовой айди
        public string Trailer;  // здесь string, а в БД это числовой айди
        public int Farm_Id;
        public bool Editable;

        public Agreg(int id, string name, int quantity, float shirina,
            string classmachine, string fuel, string trailer, int farm_Id)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Shirina = shirina;
            ClassMachine = classmachine;
            Fuel = fuel;
            Quantity = quantity;
            Trailer = trailer;
            Farm_Id = farm_Id;
        }
    }
    public class AgregOper
    {
        public string IdOperation;
        public string GSM;
        public string SR;

        public AgregOper(string idoperation, string gsm, string sr)
        {
            IdOperation = idoperation;
            GSM = gsm;
            SR = sr;
        }
    }

    public class Operations
    {
        public string OperationGroupName;
        public string OperationGroupNameNumber;
        public bool EditableGroup;
        public List<string> OperationSubGroupNames;
        public List<string> OperationSubGroupNumbers;        
        public List<bool> EditableSubGroup;
        public bool EditableTO;

        public Operations(string operationgroupname, string operationgroupnamenumber, List<string> operationsubgroupnames,
            List<string> operationsubgroupnumbers)
        {
            OperationGroupName = operationgroupname;
            OperationGroupNameNumber = operationgroupnamenumber;
            OperationSubGroupNames = operationsubgroupnames;
            OperationSubGroupNumbers = operationsubgroupnumbers;
        }
    }
    public class Fuels
    {
        public int Id;
        public string Name;
        public string Unit;
        public float Cost;
        public bool Editable;

        public Fuels(int id, string name, string unit, float cost)
        {
            Id = id;
            Name = name;
            Unit = unit;
            Cost = cost;
        }
    }
    public class Fert
    {
        public string Name;
        public int Id;
        public string Unit;
        public string ContentOfPrep;
        public string Reactant;
        public bool Editable;

        public Fert(int id, string name, string unit, string content, string reactant)
        {
            Id = id;
            Name = name;
            Unit = unit;
            ContentOfPrep = content;
            Reactant = reactant;
        }
    }
    public class Break // Скопировано с Trailing (выше) и переделано под Breaking
    {
        public int Id;
        public string Serial;
        public DateTime BeginDate;
        public DateTime EndDate;
        public string Reason;
        public bool Editable;

        public Break(int id, string serial, DateTime begindate, DateTime enddate, string reason)
        {
            Id = id;
            Serial = serial;
            BeginDate = begindate;
            EndDate = enddate;
            Reason = reason;
        }
    }
    public class MechStake // Скопировано с Trailing (выше) и переделано под Breaking
    {
        public int Id;
        public float Stake;
        public int Rank;
        public bool Editable;

        public MechStake(int id, float stake, int rank)
        {
            Id = id;
            Stake = stake;
            Rank = rank;
        }
    }
    public class Mechan // Скопировано с Trailing (выше) и переделано под Breaking
    {
        public int Id;
        public string Name;
        public string Class;
        public int Rank;
        public bool Editable;

        public Mechan(int id, string name, string classs, int rank)
        {
            Id = id;
            Name = name;
            Class = classs;
            Rank = rank;
        }
    }
    public class Medik // Скопировано с Break (выше) и переделано под Medik
    {
        public int Id;
        public string Name;
        public DateTime BeginDate;
        public DateTime EndDate;
        public string Reason;
        public bool Editable;

        public Medik(int id, string name, DateTime begindate, DateTime enddate, string reason)
        {
            Id = id;
            Name = name;
            BeginDate = begindate;
            EndDate = enddate;
            Reason = reason;
        }
    }

    public class RGroups // Разбивка совокупности отобранных TechKart на расчётные группы
    {
        public int IdTKR;
        public int IdTechKart;        
        public DateTime BeginDate;
        public DateTime EndDate;
        public int PoleId; // Workarea
        public float PoleSquare; // площадь поля
        public float MaxShirZahvat; // максимальная ширина захвата поля
        public int OperationId; // TechnologicalOperations
        public string OperationName; // Название операции
        public List<AgregateInfo> Agregate; // Список агрегатов, способных выполнять данную операцию
       

        public RGroups(int idTKR, int id, DateTime begindate, DateTime enddate, int poleid, float polesquare, float maxshirzahvat,
                       int operationid, string operationname, List<AgregateInfo> agregatelist )
        {
            IdTKR = idTKR;
            IdTechKart = id;           
            BeginDate = begindate;
            EndDate = enddate;
            PoleId = poleid;
            PoleSquare = polesquare;
            MaxShirZahvat = maxshirzahvat;
            OperationId = operationid;
            OperationName = operationname;
            Agregate = agregatelist;
            
        }
    }

    public class AgregateInfo
    {
        public int Id;
        public string Name;
        public float SeedingRates;
        public float GSM;
        public float Koef;
        public float CostGSM;
        public float PercentM;
        public float PercentT;
        public float ToM;
        public float ToT;
        public float PriceM;
        public float PriceT;
        public float ZarPl;
        public int IdTrailer;
        public int IdMachin;        
        public int NormZM;
        public int NormZT;
        public int CountT;
        public float StMeh;
        public float ShirZahv;

        public AgregateInfo(int id, string name, float sr, float gsm, float koef, float costgsm, float percentM, float percentT, float toM, float toT,
            float priceM, float priceT, int normZM, int normZT, int idMachin, int idTrailer, int countT, float stMeh, float shirZahv, float zarPl)
        {
            Id = id;
            Name = name;
            SeedingRates = sr;
            GSM = gsm;
            Koef = koef;
            CostGSM = costgsm;
            PercentM = percentM;
            PercentT = percentT;
            ToM = toM;
            ToT = toT;
            PriceM = priceM;
            PriceT = priceT;
            ZarPl = zarPl;
            IdTrailer = idTrailer;
            IdMachin = idMachin;
            NormZM = normZM;
            NormZT = normZT;
            CountT = countT;
            StMeh = stMeh;
            ShirZahv = shirZahv;
        }
    }

   



    public class TechResult
    {
        public int Id;
        public string NameTKR;
        public List<WaResult> OnWaGroup;

        public TechResult(int id,string name)
        {
            Id = id;
            NameTKR = name;
            //OnWaGroup = onWaGroup;
        }
    }

    public class WaResult
    {
        public string NameWaGroup;
        public string Soil;
        public List<ListTPacket> ListTPacket;
        public WaResult(string soil, string nameWaGropu)
        {
            Soil = soil;
            NameWaGroup = nameWaGropu;
        }
    }

    public class ListTPacket
    {
        public List<TP_TKR> TechP { get; set; }
        public ListTPacket()
        {
        }
    }    

    public class TP
    {
        public int IdOperation { get; set; }
        public string KodOperation { get; set; }
        public string Name { get; set; }
        public string Application { get; set; }
        public string Technics { get; set; }

    }
    //--------------------------------
   


    public class TP_TKR
    {
        public int  IdTechP { get; set; }
        public string Name { get; set; }
        public List<TP> Operations { get; set; }

    }
    
    public class miniTK
    {
        public string nameTK { get; set; }
        public string i { get; set; }
        public string k { get; set; }

    }

    public class TechAgroTo
    {
        public int IdTO { get; set; }
        public System.DateTime DateBegin { get; set; }
        public System.DateTime DateEnd { get; set; }
        public int ChasSm { get; set; }
        public float Rast { get; set; }
        public float StoimPodv { get; set; }
        public float StoimS { get; set; }
        public int RankWorker { get; set; }
        public int KolWorker { get; set; }
        public string PovOPl { get; set; }
        public int ThisH { get; set; }
        public float RashHim { get; set; }
        public float StoimHim { get; set; }
        public float Koef { get; set; }
        public int ThisF { get; set; }
        public string FlgUpak { get; set; }
        public float URash { get; set; }
        public float VUpak { get; set; }
        public float StoimUpak { get; set; }
    }

    public class TechAgroVar
    {
        public int Id { get; set; }
        public string NameVar { get; set; }
        public int IdTKR { get; set; }
        public string GroupWA { get; set; }
        public int IdTechPaket { get; set; }
        public int IdFarm { get; set; }
        public ListTPacket TechPaket { get; set; }

        public TechAgroVar(int id, int idFarm, ListTPacket techPaket, string groupWA, int idTKR, string nameVar, int idTechPacet)
        {
            Id = id;
            IdFarm = idFarm;
            TechPaket = techPaket;
            GroupWA = groupWA;
            IdTKR = idTKR;
            NameVar = nameVar;
            IdTechPaket = idTechPacet;
        }
    }

    public class TehKart
    {
        public int Id { get; set; }        
        public int IdTKR { get; set; }
        public string GroupWA { get; set; }
        public string TechOp { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateEnd { get; set; }
        public float Persent { get; set; }
        

        public TehKart(int id, int idTKR, string groupWA, string idTehOp, float persent, DateTime db, DateTime de)
        {
            Id = id;            
            GroupWA = groupWA;
            IdTKR = idTKR;
            TechOp = idTehOp;
            Persent = persent;
            DateBegin = db;
            DateEnd = de;
        }
    }

}
