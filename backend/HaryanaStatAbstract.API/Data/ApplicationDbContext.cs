using HaryanaStatAbstract.API.Models;
using HaryanaStatAbstract.API.Models.AreaAndPopulation;
using HaryanaStatAbstract.API.Models.Education;
using HaryanaStatAbstract.API.Models.SocialSecurityAndSocialDefence;
using Microsoft.EntityFrameworkCore;

namespace HaryanaStatAbstract.API.Data
{
    /// <summary>
    /// Application Database Context for Entity Framework Core
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Area & Population Department
        public DbSet<Table3_2CensusPopulation> Table3_2CensusPopulations { get; set; }
        
        // Education Department
        public DbSet<Table6_1Institutions> Table6_1Institutions { get; set; }

        // Social Security and Social Defence Department
        public DbSet<Table7_1SanctionedStrengthPolice> Table7_1SanctionedStrengthPolice { get; set; }
        public DbSet<Table7_6NoOfPrisonersClasswise> Table7_6NoOfPrisonersClasswise { get; set; }
        public DbSet<Table7_7PrisonerMaintenanceExpenditure> Table7_7PrisonerMaintenanceExpenditure { get; set; }
        
        // Legacy (to be removed after migration)
        // public DbSet<CensusPopulation> CensusPopulations { get; set; }
        
        // User Management Module (Replaces old Users/Roles/UserRoles tables)
        public DbSet<MstRole> MstRoles { get; set; }
        public DbSet<MstDepartment> MstDepartments { get; set; }
        public DbSet<MstSecurityQuestion> MstSecurityQuestions { get; set; }
        public DbSet<MasterUser> MasterUsers { get; set; }
        
        // Menu Management Module
        public DbSet<MstMenu> MstMenus { get; set; }
        public DbSet<DepartmentMenuMapping> DepartmentMenuMappings { get; set; }
        public DbSet<RoleMenuMapping> RoleMenuMappings { get; set; }
        
        // Workflow Engine Module
        public DbSet<MstWorkflowStatus> MstWorkflowStatuses { get; set; }
        public DbSet<WorkflowAuditHistory> WorkflowAuditHistories { get; set; }
        public DbSet<ScreenWorkflow> ScreenWorkflows { get; set; }
        public DbSet<MstScreenRegistry> MstScreenRegistries { get; set; }
        
        // Error Logging Module
        public DbSet<ErrorLog> ErrorLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Table3_2CensusPopulation entity (Area & Population Department)
            modelBuilder.Entity<Table3_2CensusPopulation>(entity =>
            {
                entity.HasKey(e => e.CensusID);
                entity.HasIndex(e => e.Year).IsUnique();
                
                entity.Property(e => e.TotalPopulation)
                    .IsRequired();
                
                entity.Property(e => e.MalePopulation)
                    .IsRequired();
                
                entity.Property(e => e.FemalePopulation)
                    .IsRequired();
                
                entity.Property(e => e.SexRatio)
                    .IsRequired();
                
                entity.Property(e => e.DecennialPercentageIncrease)
                    .HasPrecision(5, 2);

                // Data validation: Total population should equal male + female
                entity.ToTable(t =>
                    t.HasCheckConstraint(
                        "CK_AP_Table_3_2_CensusPopulation_TotalEqualsSum",
                        "[total_population] = [male_population] + [female_population]"));

                // Configure foreign keys for audit columns
                entity.HasOne(e => e.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.ModifiedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.ModifiedBy)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Configure Table6_1Institutions entity (Education Department)
            modelBuilder.Entity<Table6_1Institutions>(entity =>
            {
                entity.HasKey(e => e.InstitutionID);
                entity.HasIndex(e => e.InstitutionType).IsUnique();
                
                entity.Property(e => e.InstitutionType)
                    .IsRequired()
                    .HasMaxLength(200);

                // Configure foreign keys for audit columns
                entity.HasOne(e => e.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.ModifiedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.ModifiedBy)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Table7_1SanctionedStrengthPolice>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Year).IsUnique();
                entity.HasOne(e => e.CreatedByUser).WithMany().HasForeignKey(e => e.CreatedBy).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.ModifiedByUser).WithMany().HasForeignKey(e => e.ModifiedBy).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Table7_6NoOfPrisonersClasswise>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Year).IsUnique();
                entity.HasOne(e => e.CreatedByUser).WithMany().HasForeignKey(e => e.CreatedBy).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.ModifiedByUser).WithMany().HasForeignKey(e => e.ModifiedBy).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Table7_7PrisonerMaintenanceExpenditure>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Year).IsUnique();
                entity.HasOne(e => e.CreatedByUser).WithMany().HasForeignKey(e => e.CreatedBy).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.ModifiedByUser).WithMany().HasForeignKey(e => e.ModifiedBy).OnDelete(DeleteBehavior.NoAction);
            });

            // Old User/Role/UserRole/RefreshToken entities removed
            // Now using User Management Module: MstRole, MasterUser, etc.

            // Configure MstRole entity
            modelBuilder.Entity<MstRole>(entity =>
            {
                entity.HasKey(e => e.RoleID);
                entity.HasIndex(e => e.RoleName).IsUnique();

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            // Configure MstDepartment entity
            modelBuilder.Entity<MstDepartment>(entity =>
            {
                entity.HasKey(e => e.DepartmentID);

                entity.Property(e => e.DepartmentName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.DepartmentCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasIndex(e => e.DepartmentCode).IsUnique();
                entity.HasIndex(e => e.DepartmentName);
            });

            // Configure MstSecurityQuestion entity
            modelBuilder.Entity<MstSecurityQuestion>(entity =>
            {
                entity.HasKey(e => e.SecurityQuestionID);

                entity.Property(e => e.QuestionText)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasIndex(e => e.QuestionText).IsUnique();
            });

            // Configure MasterUser entity
            modelBuilder.Entity<MasterUser>(entity =>
            {
                entity.HasKey(e => e.UserID);

                entity.Property(e => e.LoginID)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UserPassword)
                    .IsRequired()
                    .HasColumnType("NVARCHAR(MAX)");

                entity.Property(e => e.UserMobileNo)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.UserEmailID)
                    .HasMaxLength(100);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.SecurityQuestionAnswer)
                    .HasMaxLength(100);

                entity.Property(e => e.LoggedInSessionID)
                    .HasMaxLength(100);

                entity.Property(e => e.CurrentLoginDateTime)
                    .HasColumnType("DATETIME2");

                entity.Property(e => e.LastLoginDateTime)
                    .HasColumnType("DATETIME2");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                // Foreign Keys
                entity.HasOne(e => e.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(e => e.RoleID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Department)
                    .WithMany(d => d.Users)
                    .HasForeignKey(e => e.DepartmentID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.SecurityQuestion)
                    .WithMany(sq => sq.Users)
                    .HasForeignKey(e => e.SecurityQuestionID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Indexes
                entity.HasIndex(e => e.LoginID).IsUnique();
                entity.HasIndex(e => e.RoleID);
                entity.HasIndex(e => e.DepartmentID);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.LoggedInSessionID);

                // Check constraint for mobile number length
                entity.ToTable(t =>
                    t.HasCheckConstraint(
                        "CK_Master_User_UserMobileNo",
                        "LEN([UserMobileNo]) = 10"));

                // Note: Filtered unique index for Department Checker will be created via migration/SQL
                // This ensures only one active Department Checker per Department
            });

            // Configure MstMenu entity
            modelBuilder.Entity<MstMenu>(entity =>
            {
                entity.HasKey(e => e.MenuID);
                entity.HasIndex(e => e.MenuPath).IsUnique();
                entity.HasIndex(e => e.ParentMenuID);
                entity.HasIndex(e => e.DisplayOrder);
                entity.HasIndex(e => e.IsActive);

                entity.Property(e => e.MenuName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.MenuPath)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.MenuIcon)
                    .HasMaxLength(50);

                entity.Property(e => e.MenuDescription)
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasColumnType("DATETIME2");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("DATETIME2");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(e => e.IsAdminOnly)
                    .IsRequired()
                    .HasDefaultValue(false);

                // Self-referencing relationship for parent menus
                entity.HasOne(e => e.ParentMenu)
                    .WithMany(m => m.ChildMenus)
                    .HasForeignKey(e => e.ParentMenuID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure DepartmentMenuMapping entity
            modelBuilder.Entity<DepartmentMenuMapping>(entity =>
            {
                entity.ToTable("Mst_Department_Menu_Mapping");
                entity.HasKey(e => e.MappingID);
                entity.HasIndex(e => new { e.DepartmentID, e.MenuID }).IsUnique();
                entity.HasIndex(e => e.DepartmentID);
                entity.HasIndex(e => e.MenuID);
                entity.HasIndex(e => e.IsActive);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasColumnType("DATETIME2");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("DATETIME2");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.HasOne(e => e.Department)
                    .WithMany()
                    .HasForeignKey(e => e.DepartmentID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Menu)
                    .WithMany(m => m.DepartmentMenuMappings)
                    .HasForeignKey(e => e.MenuID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure RoleMenuMapping entity
            modelBuilder.Entity<RoleMenuMapping>(entity =>
            {
                entity.ToTable("Mst_Role_Menu_Mapping");
                entity.HasKey(e => e.MappingID);
                entity.HasIndex(e => new { e.RoleID, e.MenuID }).IsUnique();
                entity.HasIndex(e => e.RoleID);
                entity.HasIndex(e => e.MenuID);
                entity.HasIndex(e => e.IsActive);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasColumnType("DATETIME2");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("DATETIME2");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.HasOne(e => e.Role)
                    .WithMany()
                    .HasForeignKey(e => e.RoleID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Menu)
                    .WithMany(m => m.RoleMenuMappings)
                    .HasForeignKey(e => e.MenuID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure MstWorkflowStatus entity
            modelBuilder.Entity<MstWorkflowStatus>(entity =>
            {
                entity.HasKey(e => e.StatusID);
                entity.HasIndex(e => e.StatusCode).IsUnique();
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.DisplayOrder);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasColumnType("DATETIME2");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("DATETIME2");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);
            });

            // Configure WorkflowAuditHistory entity
            modelBuilder.Entity<WorkflowAuditHistory>(entity =>
            {
                entity.HasKey(e => e.AuditID);
                entity.HasIndex(e => new { e.TargetTableName, e.TargetRecordID });
                entity.HasIndex(e => e.ActionByUserID);
                entity.HasIndex(e => e.ActionDate);
                entity.HasIndex(e => e.FromStatusID);
                entity.HasIndex(e => e.ToStatusID);

                entity.Property(e => e.ActionDate)
                    .IsRequired()
                    .HasColumnType("DATETIME2");

                entity.Property(e => e.Remarks)
                    .HasColumnType("NVARCHAR(MAX)");

                entity.HasOne(e => e.FromStatus)
                    .WithMany(s => s.FromStatusAudits)
                    .HasForeignKey(e => e.FromStatusID)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.ToStatus)
                    .WithMany(s => s.ToStatusAudits)
                    .HasForeignKey(e => e.ToStatusID)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.ActionByUser)
                    .WithMany()
                    .HasForeignKey(e => e.ActionByUserID)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => e.ScreenWorkflowID);

                entity.HasOne(e => e.ScreenWorkflow)
                    .WithMany()
                    .HasForeignKey(e => e.ScreenWorkflowID)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Configure ScreenWorkflow entity
            modelBuilder.Entity<ScreenWorkflow>(entity =>
            {
                entity.HasKey(e => e.ScreenWorkflowID);
                entity.HasIndex(e => e.ScreenCode).IsUnique();
                entity.HasIndex(e => e.TableName);
                entity.HasIndex(e => e.CurrentStatusID);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasColumnType("DATETIME2");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("DATETIME2");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(e => e.CurrentStatusID)
                    .IsRequired()
                    .HasDefaultValue(1);

                entity.HasOne(e => e.WorkflowStatus)
                    .WithMany()
                    .HasForeignKey(e => e.CurrentStatusID)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedByUserID)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Configure ErrorLog entity
            modelBuilder.Entity<ErrorLog>(entity =>
            {
                entity.HasKey(e => e.ErrorLogID);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.ErrorLevel);
                entity.HasIndex(e => e.IsResolved);
                entity.HasIndex(e => e.UserID);
                entity.HasIndex(e => e.Source);
                entity.HasIndex(e => e.RequestPath);

                entity.Property(e => e.ErrorLevel)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ErrorMessage)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasColumnType("DATETIME2");

                entity.Property(e => e.ResolvedAt)
                    .HasColumnType("DATETIME2");

                entity.Property(e => e.IsResolved)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserID)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.ResolvedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.ResolvedBy)
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}