using System.Collections.Generic;
using System.Data.Entity;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebsiteBanDoAnVaThucUong.Models.EF;

namespace WebsiteBanDoAnVaThucUong.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public bool Status { get; set; }
        public string FullName { get; set; }
        //public string Phone {  get; set; }
        public string Address { get; set; }
        public virtual ICollection<New> News { get; set; }
        public virtual ICollection<Wishlist> Wishlists { get; set; }
        public virtual ICollection<ReviewProduct> ReviewProducts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<FeedBackLetter> FeedBackLetters { get; set; }
        public virtual ICollection<Store> Stores { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //// Khóa ngoại Order
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Orders)
                .WithRequired(y => y.User)
                .HasForeignKey(y => y.CustomerId);

            // Khóa ngoại New
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.News)
                .WithRequired(y => y.User)
                .HasForeignKey(y => y.CreatedBy);


            // Khóa ngoại WhishList
            modelBuilder.Entity<ApplicationUser>()
               .HasMany(u => u.Wishlists)
               .WithRequired(y => y.User)
               .HasForeignKey(y => y.CustomerId);

            // Khóa ngoại Review
            modelBuilder.Entity<ApplicationUser>()
               .HasMany(u => u.ReviewProducts)
               .WithRequired(y => y.User)
               .HasForeignKey(y => y.CustomerId);

            // Khóa ngoại Voucher
            //modelBuilder.Entity<ApplicationUser>()
            //    .HasMany(u => u.Vouchers)
            //    .WithRequired(y => y.User)
            //    .HasForeignKey(y => y.CreatedBy);


            // Khóa ngoại FeedBackLetter
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.FeedBackLetters)
                .WithRequired(y => y.User)
                .HasForeignKey(y => y.CreateBy);

            //Khóa ngoại Stores
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Stores)
                .WithRequired(y => y.User)
                .HasForeignKey(y => y.IdManager);
        }

        public DbSet<OrderDetailPromotion> OrderDetailPromotion { get; set; }
        //public DbSet<ComboPromotionProduct> ComboPromotionProduct { get; set; }
        //public DbSet<ComboPromotion> ComboPromotion { get; set; }
        public DbSet<PromotionProduct> PromotionProducts { get; set; }
        //public DbSet<PromotionType> PromotionType { get; set; }
        //public DbSet<PromotionRule> PromotionRules { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<FeedBackLetter> FeedBackLetters { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<ImageSlider> ImageSlider { get; set; }
        public DbSet<Store> Stores { get; set; }
        //public DbSet<WishlistStore> WishlistStores { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<ReviewProduct> Reviews { get; set; }
        public DbSet<ThongKe> ThongKes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<New> News { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet <Subscribe> Subscribe { get; set; }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}