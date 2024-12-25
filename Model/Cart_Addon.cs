namespace DeliverySushi.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Cart_Addon
    {
        public int id { get; set; }

        public int? FK_customer_id { get; set; }

        public int? FK_addon_id { get; set; }

        public int quantity { get; set; }

        public virtual Addon Addon { get; set; }

        public virtual User User { get; set; }
    }
}
