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
    
    public partial class TehAgro
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TehAgro()
        {
            this.TehAgroTO = new HashSet<TehAgroTO>();
        }
    
        public int Id { get; set; }
        public string nameVar { get; set; }
        public int IdTKR { get; set; }
        public string GroupWA { get; set; }
        public int IdTechPaket { get; set; }
        public int IdFarm { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TehAgroTO> TehAgroTO { get; set; }
    }
}