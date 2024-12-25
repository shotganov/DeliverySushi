namespace DeliverySushi.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Cart_Sushi
    {
        public int id { get; set; }

        public int? FK_customer_id { get; set; }

        public int? FK_sushi_id { get; set; }

        public int quantity { get; set; }

        public virtual User User { get; set; }

        public virtual Sushi Sushi { get; set; }
    }
}
