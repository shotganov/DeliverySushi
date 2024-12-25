namespace DeliverySushi.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order")]
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            Order_Item = new HashSet<Order_Item>();
        }

        public int id { get; set; }

        public int? FK_customer_id { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? order_date { get; set; }

        public int? total_price { get; set; }

        [StringLength(50)]
        public string status { get; set; }

        public int? FK_order_id { get; set; }

        public int? FK_worker_id { get; set; }

        [StringLength(50)]
        public string adress { get; set; }

        [StringLength(50)]
        public string name { get; set; }
        public long phone { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order_Item> Order_Item { get; set; }

        public virtual Worker Worker { get; set; }
    }
}
