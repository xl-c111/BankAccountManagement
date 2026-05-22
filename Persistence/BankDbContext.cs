using BankAccountManagement.Models;
using Microsoft.EntityFrameworkCore;


namespace BankAccountManagement.Persistence;

public partial class BankDbContext : DbContext
{
  public BankDbContext()
  {
  }

  public BankDbContext(DbContextOptions<BankDbContext> options)
        : base(options)
  {
  }

  public virtual DbSet<Customer> Customers { get; set; }
  public virtual DbSet<Person> Persons { get; set; }
  public virtual DbSet<Company> Companies { get; set; }
  public virtual DbSet<Account> Accounts { get; set; }
  public virtual DbSet<CheckingAccount> CheckingAccounts { get; set; }
  public virtual DbSet<SavingsAccount> SavingsAccounts { get; set; }


  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    if (!optionsBuilder.IsConfigured)
    {
      string? connectionString = Environment.GetEnvironmentVariable("BANK_DB_CONNECTION");
      if (string.IsNullOrWhiteSpace(connectionString))
      {
        throw new InvalidOperationException(
          "Missing BANK_DB_CONNECTION environment variable. Set it before running the application.");
      }

      optionsBuilder.UseMySQL(connectionString);
    }
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Customer>(entity =>
    {
      entity.HasKey(e => e.CustomerId);

      entity.ToTable("customers");

      entity.Property(e => e.CustomerId).HasColumnName("customer_id").ValueGeneratedNever();

      entity.Property(e => e.Name).IsRequired().HasMaxLength(100).HasColumnName("name");

      entity.Property(e => e.Address).IsRequired().HasMaxLength(200).HasColumnName("address");

      entity.Property(e => e.PhoneNumber).HasMaxLength(20).HasColumnName("phone_number");

      entity.Property(e => e.Email).HasMaxLength(100).HasColumnName("email");

      entity.Property(e => e.CreatedAt).HasColumnName("created_at");

      entity.Property(e => e.IsActive).HasColumnName("is_active");

      entity.HasDiscriminator<string>("customer_type").HasValue<Person>("person").HasValue<Company>("company");

    });

    modelBuilder.Entity<Person>(entity =>
    {
      entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");

      entity.Property(e => e.Occupation).HasMaxLength(200).HasColumnName("occupation");
    });

    modelBuilder.Entity<Company>(entity =>
    {
      entity.Property(e => e.ABN).HasMaxLength(20).HasColumnName("abn");

      entity.Property(e => e.ACN).HasMaxLength(100).HasColumnName("acn");

      entity.Property(e => e.Industry).HasMaxLength(100).HasColumnName("industry");
    });

    modelBuilder.Entity<Account>(entity =>
    {
      entity.HasKey(e => e.AccountId);

      entity.ToTable("accounts");

      entity.Property(e => e.AccountId).HasColumnName("account_id").ValueGeneratedNever();

      entity.Property(e => e.Balance).IsRequired().HasColumnType("decimal(18,2)").HasColumnName("balance");

      entity.Property(e => e.CreatedAt).HasColumnName("created_at");

      entity.Property(e => e.IsActive).HasColumnName("is_active");

      entity.HasDiscriminator<string>("account_type").HasValue<SavingsAccount>("savings").HasValue<CheckingAccount>("checking");

    });

    modelBuilder.Entity<SavingsAccount>(entity =>
        {
          entity.Property(e => e.InterestRate).HasColumnName("interest_rate");
        });

    modelBuilder.Entity<CheckingAccount>(entity =>
    {
      entity.Property(e => e.NextCheckNumber).HasColumnName("next_check_number");
    });

    OnModelCreatingPartial(modelBuilder);
  }
  partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
