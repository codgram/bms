﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Application.Model;
using Application.Model.Org;
using Application.Model.ProductManagement;
using Application.Model.Work;
using Application.Model.Procurement;

namespace Application.Model.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions options) : base(options)
    {
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
    public DbSet<Vendor> Vendor { get; set; }
    public DbSet<PurchaseHeader> PurchaseHeader { get; set; }
    public DbSet<PurchaseLine> PurchaseLine { get; set; }
    public DbSet<WorkHeader> WorkHeader { get; set; }
    public DbSet<WorkLine> WorkLine { get; set; }
    public DbSet<CustomWorkHeader> CustomWorkHeader { get; set; }
    public DbSet<CustomWorkLine> CustomWorkLine { get; set; }
}
