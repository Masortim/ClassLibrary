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
    
    public partial class Mechanizator
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Mechanizator()
        {
            this.MehMach = new HashSet<MehMach>();
            this.SickList = new HashSet<SickList>();
            this.TKCosts = new HashSet<TKCosts>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public Nullable<int> Farm_Id { get; set; }
        public Nullable<int> StakesOfMech_Id { get; set; }
    
        public virtual Farm Farm { get; set; }
        public virtual StakesOfMech StakesOfMech { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MehMach> MehMach { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SickList> SickList { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TKCosts> TKCosts { get; set; }
    }
}