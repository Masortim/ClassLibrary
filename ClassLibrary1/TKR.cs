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
    
    public partial class TKR
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TKR()
        {
            this.TechKart = new HashSet<TechKart>();
            this.WA_TKR = new HashSet<WA_TKR>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public int YearCrop { get; set; }
        public string Others { get; set; }
        public Nullable<int> Cultures_Id { get; set; }
        public Nullable<int> Farm_Id { get; set; }
        public Nullable<int> Intensification_Id { get; set; }
        public Nullable<int> Predshest_Id { get; set; }
    
        public virtual Cultures Cultures { get; set; }
        public virtual Farm Farm { get; set; }
        public virtual Intensification Intensification { get; set; }
        public virtual Predshest Predshest { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TechKart> TechKart { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WA_TKR> WA_TKR { get; set; }
    }
}