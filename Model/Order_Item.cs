namespace DeliverySushi.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Windows.Media.Imaging;

    public partial class Order_Item
    {
        public int id { get; set; }

        public int? FK_order_id { get; set; }

        public int? quantity { get; set; }

        public long? price { get; set; }

        public int? FK_product_id { get; set; }

        [StringLength(50)]
        public string product_type { get; set; }

        [NotMapped] // Указываем, что это свойство не будет сохраняться в базе данных
        public BitmapImage ImageSource { get; set; }
        [NotMapped] // Указываем, что это свойство не будет сохраняться в базе данных
        public string name { get; set; }
        public virtual Order Order { get; set; }

       
    }
}
