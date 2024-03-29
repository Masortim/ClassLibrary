//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClassLibrary1
{
    using System;
    using System.Collections.Generic;
    
    public partial class TKCosts
    {
        public int Id { get; set; }
        public System.DateTime DateBegin { get; set; }
        public System.DateTime DateEnd { get; set; }
        public float AmmountOfWorks { get; set; }
        public float PercentOfWorksOnThis { get; set; }
        public float CountOfNormSm { get; set; }
        public float CostLaborOnWork { get; set; }
        public float CostGSMOnWork { get; set; }
        public float CostOfTransport { get; set; }
        public float CostOfSeeds { get; set; }
        public float CostOfChem { get; set; }
        public float CostOfFertiliz { get; set; }
        public float GeneralCosts { get; set; }
        public int ChasSm { get; set; }
        public int ClassOfMeh { get; set; }
        public int CountOfMeh { get; set; }
        public int ClassOfWorker { get; set; }
        public int CountOfWorker { get; set; }
        public float TOAndRepairCosts { get; set; }
        public float AmmortOfMachineryOnV { get; set; }
        public float AmmortOfTrailerOnV { get; set; }
        public float AllAmmort { get; set; }
        public float workersC_LabourMeh_days { get; set; }
        public float workersC_LabourWrks_days { get; set; }
        public float workersC_LabourAll_days { get; set; }
        public float workersC_CostMoneyMeh { get; set; }
        public float workersC_CostMoneyWrks { get; set; }
        public float workersC_CostMoneyAll { get; set; }
        public float workersC_Payment_CostMoney { get; set; }
        public float workersC_TaxAndPayment { get; set; }
        public float workersC_TariffRateM { get; set; }
        public float workersC_TariffRateW { get; set; }
        public float transportC_DistanceKm { get; set; }
        public float transportC_Seeds_TKm { get; set; }
        public float transportC_SupplyUdoWater_TKm { get; set; }
        public float transportC_Unit { get; set; }
        public float transportC_CostMoney { get; set; }
        public float himC_CostOnUnit { get; set; }
        public float himC_CostOnVWork { get; set; }
        public float himC_CostsPrep { get; set; }
        public float himC_CoeffApply { get; set; }
        public float himC_Price { get; set; }
        public float himC_CostMoney { get; set; }
        public float gsmC_CostFuel { get; set; }
        public float gsmC_CostMoney { get; set; }
        public int fertC_Count { get; set; }
        public float fertC_ExpensisOn1V { get; set; }
        public float fertC_ExpensisOn1W { get; set; }
        public float fertC_PriceUnit { get; set; }
        public float fertC_CostsMoney { get; set; }
        public float fertC_VUpak { get; set; }
        public int fertC_FlgUpak { get; set; }
        public float seedC_CountOfSeeds { get; set; }
        public float seedC_Price { get; set; }
        public float seedC_CostAll { get; set; }
        public Nullable<int> Agregates_Id { get; set; }
        public Nullable<int> Fertilizer_Id { get; set; }
        public Nullable<int> Him_Id { get; set; }
        public Nullable<int> Machinery_Id { get; set; }
        public Nullable<int> Mechanizator_Id { get; set; }
        public Nullable<int> StakesOfWorkers_Id { get; set; }
        public Nullable<int> TechKart_Id { get; set; }
    
        public virtual Agregates Agregates { get; set; }
        public virtual Fertilizer Fertilizer { get; set; }
        public virtual Him Him { get; set; }
        public virtual Machinery Machinery { get; set; }
        public virtual Mechanizator Mechanizator { get; set; }
        public virtual StakesOfWorkers StakesOfWorkers { get; set; }
        public virtual TechKart TechKart { get; set; }
    }
}
