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
    
    public partial class Agregates
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Agregates()
        {
            this.TKCosts = new HashSet<TKCosts>();
            this.OperationsOfAgregate = new HashSet<OperationsOfAgregate>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public float ShirZahvat { get; set; }
        public Nullable<int> ClassMachine_Id { get; set; }
        public Nullable<int> Farm_Id { get; set; }
        public Nullable<int> KindOfFuel_Id { get; set; }
        public Nullable<int> Trailers_Id { get; set; }
    
        public virtual ClassMachine ClassMachine { get; set; }
        public virtual Farm Farm { get; set; }
        public virtual KindOfFuel KindOfFuel { get; set; }
        public virtual Trailers Trailers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TKCosts> TKCosts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OperationsOfAgregate> OperationsOfAgregate { get; set; }
    }
}
