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
    
    public partial class TehAgroTO
    {
        public int Id { get; set; }
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
        public Nullable<int> TehAgro_Id { get; set; }
    
        public virtual TehAgro TehAgro { get; set; }
    }
}