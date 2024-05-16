using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Item> Items { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Customize Identity models here
            builder.Entity<ApplicationUser>().HasKey(u => u.Id);
            builder.Entity<ApplicationRole>().HasKey(r => r.Id);
            builder.Entity<IdentityUserRole<string>>().HasKey(ur => new { ur.UserId, ur.RoleId });


            //SeedUsers(builder);
            SeedRoles(builder);
            //SeedUserRoles(builder);
            SeedCatagories(builder);
            SeedItems(builder);

            
        }


        private void SeedUsers(ModelBuilder builder)
        {
            PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();

            ApplicationUser adminUser = new ApplicationUser()
            {
                Id = new Guid("b74ddd14-6340-4840-95c2-db12554843e5"),
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                LockoutEnabled = false,
                PhoneNumber = "1234567890",
                Name = "Rakib",
                PasswordHash = passwordHasher.HashPassword(null , "Admin@123")
            };

                
            builder.Entity<ApplicationUser>().HasData(adminUser);

            ApplicationUser employeeUser = new ApplicationUser()
            {
                Id = new Guid("9D2B0228-4D0D-4C23-8B49-01A698857709"),
                UserName = "employee@gmail.com",
                Email = "employee@gmail.com",
                LockoutEnabled = false,
                PhoneNumber = "1234567890",
                Name = "Employee",
                PasswordHash = passwordHasher.HashPassword(null, "Test@123")
        };

            builder.Entity<ApplicationUser>().HasData(employeeUser);
        }

        private void SeedRoles(ModelBuilder builder)
        {

            builder.Entity<ApplicationRole>().HasData
                (
                new ApplicationRole() { Id = new Guid("b74ddd14-6340-4840-95c2-db12554843e5"), Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin", Description = "This is Admin Role" },
                new ApplicationRole() { Id = new Guid("9D2B0228-4D0D-4C23-8B49-01A698857709"), Name = "General", ConcurrencyStamp = "2", NormalizedName = "General", Description = "This is General Role" }
               );
        }

        private void SeedUserRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityUserRole<string>>().HasData
               (
                    new IdentityUserRole<string>() { RoleId = "b74ddd14-6340-4840-95c2-db12554843e5", UserId = "b74ddd14-6340-4840-95c2-db12554843e5" },
                    new IdentityUserRole<string>() { RoleId = "9D2B0228-4D0D-4C23-8B49-01A698857709", UserId = "9D2B0228-4D0D-4C23-8B49-01A698857709" }
               );
        }

        private void SeedCatagories(ModelBuilder builder)
        {
            builder.Entity<Category>().HasData
                (
                new Category {Id=1, Name = "Mobile" },
                new Category {Id=2, Name = "Laptop" },
                new Category {Id=3, Name = "PC" },
                new Category {Id=4, Name = "Monitor" },
                new Category {Id=5, Name = "TV" }
                );

        }

        private void SeedItems(ModelBuilder builder)
        {

            builder.Entity<Item>().HasData
                (
                 new Item {Id=1, Name = "Sumsung", Quantity = 10, Unit = 10, CategoryId = 1 },
                 new Item {Id=2, Name = "Lanovo", Quantity = 10, Unit = 10, CategoryId = 2 },
                 new Item {Id=3, Name = "Redmi Note", Quantity = 10, Unit = 10, CategoryId = 1 },
                 new Item {Id=4, Name = "iPhone 14", Quantity = 10, Unit = 10, CategoryId = 1 },
                 new Item {Id=5, Name = "LG", Quantity = 10, Unit = 10, CategoryId = 4 }
                );

        }
    }


}
