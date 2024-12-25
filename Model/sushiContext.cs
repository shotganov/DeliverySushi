using DeliverySushi;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace DeliverySushi.Model
{
    public partial class sushiContext : DbContext
    {
        public sushiContext()
            : base("sushi")
        {
            //Configuration.LazyLoadingEnabled = false;
            //Database.SetInitializer(new CreateDatabaseIfNotExists<sushiContext>());
        }

     
        public virtual DbSet<Addon> Addon { get; set; }
        public virtual DbSet<Cart_Addon> Cart_Addon { get; set; }
        public virtual DbSet<User> User { get; set; }

        public virtual DbSet<Worker> Worker { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<Order_Item> Order_Item { get; set; }
        public virtual DbSet<Set> Set { get; set; }

        public virtual DbSet<Cart_Set> Cart_Set { get; set; }
       
        public virtual DbSet<Sushi> Sushi { get; set; }
        public virtual DbSet<Cart_Sushi> Cart_Sushi { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Addon>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<Addon>()
                .HasMany(e => e.Cart_Addon)
                .WithRequired(e => e.Addon)
                .HasForeignKey(e => e.FK_addon_id);

            modelBuilder.Entity<Worker>()
               .HasMany(e => e.Order)
               .WithRequired(e => e.Worker)
               .HasForeignKey(e => e.FK_worker_id);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Order)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.FK_customer_id);

            modelBuilder.Entity<User>()
               .HasMany(e => e.Cart_Addon)
               .WithRequired(e => e.User)
               .HasForeignKey(e => e.FK_customer_id);

            modelBuilder.Entity<User>()
              .HasMany(e => e.Cart_Set)
              .WithRequired(e => e.User)
              .HasForeignKey(e => e.FK_customer_id);

            modelBuilder.Entity<User>()
              .HasMany(e => e.Cart_Sushi)
              .WithRequired(e => e.User)
              .HasForeignKey(e => e.FK_customer_id);

            modelBuilder.Entity<Order>()
                .HasMany(e => e.Order_Item)
                .WithRequired(e => e.Order)
                .HasForeignKey(e => e.FK_order_id);

            //modelBuilder.Entity<Order_Item>()
            //    .HasOptional(oi => oi.Order)     // Один к одному
            //    .WithMany(o => o.Order_Item) // Связь с Order_Item
            //    .HasForeignKey(oi => oi.FK_order_id); // Внешний ключ

            modelBuilder.Entity<Set>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<Set>()
                .HasMany(e => e.Cart_Set)
                .WithRequired(e => e.Set)
                .HasForeignKey(e => e.FK_set_id);


            modelBuilder.Entity<Sushi>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<Sushi>()
                .HasMany(e => e.Cart_Sushi)
                .WithRequired(e => e.Sushi)
                .HasForeignKey(e => e.FK_sushi_id);

        }
    }
}

