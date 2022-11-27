using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Duende.IdentityServer.EntityFramework.Options;
using Application.Model;
using Application.Model.Org;
using Application.Model.ProductManagement;
using Application.Model.Procurement;
using Application.Model.Work;
using Microsoft.AspNetCore.Identity;

namespace Application.Server.Data;

public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
{
    public ApplicationDbContext(
        DbContextOptions options,
        IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(b =>
        {
            b.ToTable("ApplicationUser");
        });
        builder.Entity<IdentityUserClaim<string>>(b =>
        {
            b.ToTable("UserClaim");
        });
        builder.Entity<IdentityUserLogin<string>>(b =>
        {
            b.ToTable("UserLogin");
        });
        builder.Entity<IdentityUserToken<string>>(b =>
        {
            b.ToTable("Token");
        });

        builder.Entity<IdentityRole>(b =>
        {
            b.ToTable("Role");
        });
        
        builder.Entity<IdentityRoleClaim<string>>(b =>
        {
            b.ToTable("RoleClaim");
        });
        builder.Entity<IdentityUserRole<string>>(b =>
        {
            b.ToTable("UserRole");
        });

        builder.Entity<CompanySubscription>(entity =>
        {
            entity.HasOne(e => e.Subscription).WithMany().OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Company).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Member>(entity =>
        {
            entity.HasOne(e => e.Company).WithMany().OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.ApplicationUser).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Entity>(entity =>
        {
            entity.HasOne(e => e.Company).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Store>(entity =>
        {
            entity.HasOne(e => e.Entity).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<UserStore>(entity =>
        {
            entity.HasOne(e => e.Store).WithMany().OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.ApplicationUser).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<PurchaseHeader>(entity =>
        {
            entity.HasOne(e => e.Company).WithMany().OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Vendor).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<PurchaseLine>(entity =>
        {
            entity.HasOne(e => e.PurchaseHeader).WithMany().OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Item).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Vendor>(entity =>
        {
            entity.HasOne(e => e.Company).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Barcode>(entity =>
        {
            entity.HasOne(e => e.Company).WithMany().OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Item).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Item>(entity =>
        {
            entity.HasOne(e => e.Company).WithMany().OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Subgroup).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Division>(entity =>
        {
            entity.HasOne(e => e.Company).WithMany().OnDelete(DeleteBehavior.Restrict);
        });


        builder.Entity<Category>(entity =>
        {
            entity.HasOne(e => e.Division).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

         builder.Entity<Subcategory>(entity =>
        {
            entity.HasOne(e => e.Category).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

         builder.Entity<Group>(entity =>
        {
            entity.HasOne(e => e.Category).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Subgroup>(entity =>
        {
            entity.HasOne(e => e.Group).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<CustomWorkHeader>(entity =>
        {
            entity.HasOne(e => e.Company).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<CustomWorkLine>(entity =>
        {
            entity.HasOne(e => e.CustomWorkHeader).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<WorkHeader>(entity =>
        {
            entity.HasOne(e => e.Store).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<WorkLine>(entity =>
        {
            entity.HasOne(e => e.WorkHeader).WithMany().OnDelete(DeleteBehavior.Restrict);
        });
        
    }

    public DbSet<ApplicationUser> ApplicationUser { get; set; }
    public DbSet<Company> Company { get; set; }
    public DbSet<Member> Member { get; set; }
    public DbSet<Entity> Entity { get; set; }
    public DbSet<Store> Store { get; set; }
    public DbSet<UserStore> UserStore { get; set; }
    public DbSet<Division> Division { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<Subcategory> Subcategory { get; set; }
    public DbSet<Group> Group { get; set; }
    public DbSet<Subgroup> Subgroup { get; set; }
    public DbSet<Item> Item { get; set; }
    public DbSet<Barcode> Barcode { get; set; }
    public DbSet<Vendor> Vendor { get; set; }
    public DbSet<PurchaseHeader> PurchaseHeader { get; set; }
    public DbSet<PurchaseLine> PurchaseLine { get; set; }
    public DbSet<WorkHeader> WorkHeader { get; set; }
    public DbSet<WorkLine> WorkLine { get; set; }
    public DbSet<CustomWorkHeader> CustomWorkHeader { get; set; }
    public DbSet<CustomWorkLine> CustomWorkLine { get; set; }
}
