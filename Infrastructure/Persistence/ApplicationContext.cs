using Core.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public partial class ApplicationContext : DbContext
    {
        public ApplicationContext()
        {
        }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Address> Addresses { get; set; } = null!;
        public virtual DbSet<Agreement> Agreements { get; set; } = null!;
        public virtual DbSet<AgreementHistory> AgreementHistories { get; set; } = null!;
        public virtual DbSet<Bank> Banks { get; set; } = null!;
        public virtual DbSet<City> Cities { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<PaymentDetail> PaymentDetails { get; set; } = null!;
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; } = null!;
        public virtual DbSet<PaymentPlan> PaymentPlans { get; set; } = null!;
        public virtual DbSet<PaymentReceipt> PaymentReceipts { get; set; } = null!;
        public virtual DbSet<Person> People { get; set; } = null!;
        public virtual DbSet<Property> Properties { get; set; } = null!;
        public virtual DbSet<Rol> Rols { get; set; } = null!;
        public virtual DbSet<State> States { get; set; } = null!;
        public virtual DbSet<Token> Tokens { get; set; } = null!;
        public virtual DbSet<TokenType> TokenTypes { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(e => e.PersonId);

                entity.ToTable("Address");

                entity.Property(e => e.PersonId).ValueGeneratedNever();

                entity.Property(e => e.CityId).HasColumnName("CityID");

                entity.Property(e => e.Name)
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.HasOne(d => d.City)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.CityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Address_City");

                entity.HasOne(d => d.Person)
                    .WithOne(p => p.Address)
                    .HasForeignKey<Address>(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Address_Person");
            });

            modelBuilder.Entity<Agreement>(entity =>
            {
                entity.HasKey(e => e.PropertyId);

                entity.ToTable("Agreement");

                entity.Property(e => e.PropertyId)
                    .ValueGeneratedNever()
                    .HasColumnName("PropertyID");

                entity.Property(e => e.BalancePaid).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.BalancePayable).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.PaymentPlanId).HasColumnName("PaymentPlanID");

                entity.Property(e => e.PersonId).HasColumnName("PersonID");

                entity.Property(e => e.RegistrationDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.PaymentPlan)
                    .WithMany(p => p.Agreements)
                    .HasForeignKey(d => d.PaymentPlanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Agreement_PaymentPlan");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Agreements)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Agreement_Person");

                entity.HasOne(d => d.Property)
                    .WithOne(p => p.Agreement)
                    .HasForeignKey<Agreement>(d => d.PropertyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Agreement_Property");
            });

            modelBuilder.Entity<AgreementHistory>(entity =>
            {
                entity.HasKey(e => e.HistoryId);

                entity.ToTable("AgreementHistory");

                entity.Property(e => e.HistoryId)
                    .ValueGeneratedNever()
                    .HasColumnName("HistoryID");

                entity.Property(e => e.BalancePaid).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.BalancePayable).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.PaymentPlanId).HasColumnName("PaymentPlanID");

                entity.Property(e => e.PersonId).HasColumnName("PersonID");

                entity.Property(e => e.PropertyId).HasColumnName("PropertyID");

                entity.Property(e => e.RegistrationDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Property)
                    .WithMany(p => p.AgreementHistories)
                    .HasForeignKey(d => d.PropertyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AgreementHistory_Agreement");
            });

            modelBuilder.Entity<Bank>(entity =>
            {
                entity.ToTable("Bank");

                entity.Property(e => e.BankId)
                    .ValueGeneratedNever()
                    .HasColumnName("BankID");

                entity.Property(e => e.Name)
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.ToTable("City");

                entity.Property(e => e.CityId)
                    .ValueGeneratedNever()
                    .HasColumnName("CityID");

                entity.Property(e => e.Name)
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.StateId).HasColumnName("StateID");

                entity.HasOne(d => d.State)
                    .WithMany(p => p.Cities)
                    .HasForeignKey(d => d.StateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_City_State");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payment");

                entity.HasIndex(e => new { e.PropertyId, e.PaymentNumber }, "UK_Payment_PropertyId_PaymentNumber")
                    .IsUnique();

                entity.Property(e => e.PaymentId)
                    .ValueGeneratedNever()
                    .HasColumnName("PaymentID");

                entity.Property(e => e.Description)
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.NewBalancePaid).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.NewBalancePayable).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.PayerId).HasColumnName("PayerID");

                entity.Property(e => e.PreviousBalancePaid).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.PreviousBalancePayable).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.PropertyId).HasColumnName("PropertyID");

                entity.Property(e => e.ReceiverId).HasColumnName("ReceiverID");

                entity.Property(e => e.RegistrationDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Payer)
                    .WithMany(p => p.PaymentPayers)
                    .HasForeignKey(d => d.PayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Payment_Person1");

                entity.HasOne(d => d.Agreement)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.PropertyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Payment_Agreement");

                entity.HasOne(d => d.Receiver)
                    .WithMany(p => p.PaymentReceivers)
                    .HasForeignKey(d => d.ReceiverId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Payment_Person");
            });

            modelBuilder.Entity<PaymentDetail>(entity =>
            {
                entity.ToTable("PaymentDetail");

                entity.HasIndex(e => new { e.PaymentId, e.PaymentMethodId }, "UK_PaymentDetail_PaymentID_PaymentMethodID")
                    .IsUnique();

                entity.Property(e => e.PaymentDetailId).HasColumnName("PaymentDetailID");

                entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.BankId).HasColumnName("BankID");

                entity.Property(e => e.PaymentId).HasColumnName("PaymentID");

                entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");

                entity.Property(e => e.ReferenceNumber)
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.HasOne(d => d.Bank)
                    .WithMany(p => p.PaymentDetails)
                    .HasForeignKey(d => d.BankId)
                    .HasConstraintName("FK_PaymentDetail_Bank");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.PaymentDetails)
                    .HasForeignKey(d => d.PaymentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaymentDetail_Payment");

                entity.HasOne(d => d.PaymentMethod)
                    .WithMany(p => p.PaymentDetails)
                    .HasForeignKey(d => d.PaymentMethodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaymentDetail_PaymentMethod");
            });

            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.ToTable("PaymentMethod");

                entity.Property(e => e.PaymentMethodId)
                    .ValueGeneratedNever()
                    .HasColumnName("PaymentMethodID");

                entity.Property(e => e.Name)
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PaymentPlan>(entity =>
            {
                entity.ToTable("PaymentPlan");

                entity.Property(e => e.PaymentPlanId)
                    .ValueGeneratedNever()
                    .HasColumnName("PaymentPlanID");

                entity.Property(e => e.Interest).HasColumnType("decimal(4, 2)");

                entity.Property(e => e.Name)
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PaymentReceipt>(entity =>
            {
                entity.HasKey(e => e.PaymentId);

                entity.ToTable("PaymentReceipt");

                entity.Property(e => e.PaymentId)
                    .ValueGeneratedNever()
                    .HasColumnName("PaymentID");

                entity.HasOne(d => d.Payment)
                    .WithOne(p => p.PaymentReceipt)
                    .HasForeignKey<PaymentReceipt>(d => d.PaymentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaymentReceipt_Payment");
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable("Person");

                entity.HasIndex(e => e.NationalId, "UK_Person_NationalID")
                    .IsUnique();

                entity.Property(e => e.PersonId).HasColumnName("PersonID");

                entity.Property(e => e.Birthday).HasColumnType("date");

                entity.Property(e => e.EmailAddress)
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.NationalId)
                    .HasMaxLength(13)
                    .IsUnicode(false)
                    .HasColumnName("NationalID");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.RegistrationDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Property>(entity =>
            {
                entity.ToTable("Property");

                entity.HasIndex(e => e.Code, "UK_Property_Code")
                    .IsUnique();

                entity.Property(e => e.PropertyId)
                    .ValueGeneratedNever()
                    .HasColumnName("PropertyID");

                entity.Property(e => e.Code)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            });

            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("Rol");

                entity.Property(e => e.RolId)
                    .ValueGeneratedNever()
                    .HasColumnName("RolID");

                entity.Property(e => e.Name)
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.ToTable("State");

                entity.Property(e => e.StateId)
                    .ValueGeneratedNever()
                    .HasColumnName("StateID");

                entity.Property(e => e.Name)
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Token>(entity =>
            {
                entity.ToTable("Token");

                entity.Property(e => e.TokenId)
                    .ValueGeneratedNever()
                    .HasColumnName("TokenID");

                entity.Property(e => e.Data).IsUnicode(false);

                entity.Property(e => e.Expires).HasColumnType("datetime");

                entity.Property(e => e.PersonId).HasColumnName("PersonID");

                entity.Property(e => e.RegistrationDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TokenTypeId).HasColumnName("TokenTypeID");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Tokens)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Token_User");

                entity.HasOne(d => d.TokenType)
                    .WithMany(p => p.Tokens)
                    .HasForeignKey(d => d.TokenTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Token_TokenType");
            });

            modelBuilder.Entity<TokenType>(entity =>
            {
                entity.ToTable("TokenType");

                entity.Property(e => e.TokenTypeId)
                    .ValueGeneratedNever()
                    .HasColumnName("TokenTypeID");

                entity.Property(e => e.Name)
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.PersonId);

                entity.ToTable("User");

                entity.HasIndex(e => e.Username, "UK_User_Username")
                    .IsUnique();

                entity.Property(e => e.PersonId)
                    .ValueGeneratedNever()
                    .HasColumnName("PersonID");

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.RegistrationDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ResetPassword)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Username)
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.HasOne(d => d.Person)
                    .WithOne(p => p.User)
                    .HasForeignKey<User>(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Person");

                entity.HasMany(d => d.Rols)
                    .WithMany(p => p.People)
                    .UsingEntity<Dictionary<string, object>>(
                        "UserRol",
                        l => l.HasOne<Rol>().WithMany().HasForeignKey("RolId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_UserRol_Rol"),
                        r => r.HasOne<User>().WithMany().HasForeignKey("PersonId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_UserRol_User"),
                        j =>
                        {
                            j.HasKey("PersonId", "RolId");

                            j.ToTable("UserRol");

                            j.IndexerProperty<int>("PersonId").HasColumnName("PersonID");

                            j.IndexerProperty<int>("RolId").HasColumnName("RolID");
                        });
            });

            modelBuilder.HasSequence<int>("PaymentIdentifier").HasMin(1);

            modelBuilder.HasSequence<int>("TokenIdentifier").HasMin(1);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public async Task<int> GetTokenIdentifier()
        {
            SqlParameter tokenIdentifier = new(nameof(tokenIdentifier), System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            await Database.ExecuteSqlRawAsync($"SELECT @{tokenIdentifier} = (NEXT VALUE FOR TokenIdentifier)", tokenIdentifier);

            return (int)tokenIdentifier.Value;
        }

        public async Task<int> GetPaymentIdentifier()
        {
            SqlParameter paymentIdentifier = new(nameof(paymentIdentifier), System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            await Database.ExecuteSqlRawAsync($"SELECT @{paymentIdentifier} = (NEXT VALUE FOR PaymentIdentifier)", paymentIdentifier);

            return (int)paymentIdentifier.Value;
        }
    }
}
