namespace DeliverySushi.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Worker")]
    public partial class Worker
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Worker()
        {
            Order = new HashSet<Order>();
        }

        public int id { get; set; }

        [StringLength(50)]
        public string login { get; set; }

        [StringLength(50)]
        public string password { get; set; }

        public long? phone { get; set; }

        [StringLength(50)]
        public string email { get; set; }

        [StringLength(50)]
        public string position { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Order { get; set; }
    }
}
