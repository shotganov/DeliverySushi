namespace DeliverySushi.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Windows.Media.Imaging;

    [Table("Addon")]
    public partial class Addon
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Addon()
        {
            Cart_Addon = new HashSet<Cart_Addon>();
        }

        public int id { get; set; }

        [StringLength(50)]
        public string name { get; set; }

        public int price { get; set; }

        [Column(TypeName = "text")]
        public string description { get; set; }

        [Column(TypeName = "image")]
        public byte[] image { get; set; }

        public int quantity { get; set; }

        [NotMapped] // Указываем, что это свойство не будет сохраняться в базе данных
        public BitmapImage ImageSource { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cart_Addon> Cart_Addon { get; set; }
    }
}
