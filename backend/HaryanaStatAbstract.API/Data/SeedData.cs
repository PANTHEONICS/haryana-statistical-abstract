using HaryanaStatAbstract.API.Models;
using HaryanaStatAbstract.API.Models.AreaAndPopulation;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace HaryanaStatAbstract.API.Data
{
    /// <summary>
    /// Seed data for initial database population
    /// </summary>
    public static class SeedData
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Seed Roles if they don't exist
            await SeedRolesAsync(context);
            
            // Seed Admin User if it doesn't exist
            await SeedAdminUserAsync(context);
            
            // Check if census data already exists
            if (context.Table3_2CensusPopulations.Any())
            {
                return;
            }

            var censusData = new List<Table3_2CensusPopulation>
            {
                new Table3_2CensusPopulation
                {
                    Year = 1901,
                    TotalPopulation = 4623064,
                    VariationInPopulation = null,
                    DecennialPercentageIncrease = null,
                    MalePopulation = 2476390,
                    FemalePopulation = 2146674,
                    SexRatio = 867,
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow
                },
                new Table3_2CensusPopulation
                {
                    Year = 1911,
                    TotalPopulation = 4174677,
                    VariationInPopulation = -448387,
                    DecennialPercentageIncrease = -9.70m,
                    MalePopulation = 2274909,
                    FemalePopulation = 1899768,
                    SexRatio = 835,
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow
                },
                new Table3_2CensusPopulation
                {
                    Year = 1921,
                    TotalPopulation = 4255892,
                    VariationInPopulation = 81215,
                    DecennialPercentageIncrease = 1.95m,
                    MalePopulation = 2307985,
                    FemalePopulation = 1947907,
                    SexRatio = 844,
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow
                },
                new Table3_2CensusPopulation
                {
                    Year = 1931,
                    TotalPopulation = 4559917,
                    VariationInPopulation = 304025,
                    DecennialPercentageIncrease = 7.14m,
                    MalePopulation = 2473228,
                    FemalePopulation = 2086689,
                    SexRatio = 844,
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow
                },
                new Table3_2CensusPopulation
                {
                    Year = 1941,
                    TotalPopulation = 5272829,
                    VariationInPopulation = 712912,
                    DecennialPercentageIncrease = 15.63m,
                    MalePopulation = 2821783,
                    FemalePopulation = 2451046,
                    SexRatio = 869,
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow
                },
                new Table3_2CensusPopulation
                {
                    Year = 1951,
                    TotalPopulation = 5673597,
                    VariationInPopulation = 400768,
                    DecennialPercentageIncrease = 7.60m,
                    MalePopulation = 3031612,
                    FemalePopulation = 2641985,
                    SexRatio = 871,
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow
                },
                new Table3_2CensusPopulation
                {
                    Year = 1961,
                    TotalPopulation = 7590524,
                    VariationInPopulation = 1916927,
                    DecennialPercentageIncrease = 33.79m,
                    MalePopulation = 4062787,
                    FemalePopulation = 3527737,
                    SexRatio = 868,
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow
                },
                new Table3_2CensusPopulation
                {
                    Year = 1971,
                    TotalPopulation = 10036431,
                    VariationInPopulation = 2445907,
                    DecennialPercentageIncrease = 32.22m,
                    MalePopulation = 5377044,
                    FemalePopulation = 4659387,
                    SexRatio = 867,
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow
                },
                new Table3_2CensusPopulation
                {
                    Year = 1981,
                    TotalPopulation = 12922119,
                    VariationInPopulation = 2885688,
                    DecennialPercentageIncrease = 28.75m,
                    MalePopulation = 6909679,
                    FemalePopulation = 6012440,
                    SexRatio = 870,
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow
                },
                new Table3_2CensusPopulation
                {
                    Year = 1991,
                    TotalPopulation = 16463648,
                    VariationInPopulation = 3541529,
                    DecennialPercentageIncrease = 27.41m,
                    MalePopulation = 8827474,
                    FemalePopulation = 7636174,
                    SexRatio = 865,
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow
                },
                new Table3_2CensusPopulation
                {
                    Year = 2001,
                    TotalPopulation = 21144564,
                    VariationInPopulation = 4680916,
                    DecennialPercentageIncrease = 28.43m,
                    MalePopulation = 11363953,
                    FemalePopulation = 9780611,
                    SexRatio = 861,
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow
                },
                new Table3_2CensusPopulation
                {
                    Year = 2011,
                    TotalPopulation = 25351462,
                    VariationInPopulation = 4206898,
                    DecennialPercentageIncrease = 19.90m,
                    MalePopulation = 13494734,
                    FemalePopulation = 11856728,
                    SexRatio = 879,
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow
                }
            };

            await context.Table3_2CensusPopulations.AddRangeAsync(censusData);
            await context.SaveChangesAsync();
        }

        private static async Task SeedRolesAsync(ApplicationDbContext context)
        {
            // Ensure System Admin role exists
            if (!await context.MstRoles.AnyAsync(r => r.RoleName == "System Admin"))
            {
                context.MstRoles.Add(new MstRole { RoleName = "System Admin" });
                await context.SaveChangesAsync();
            }

            // Ensure DESA Head role exists
            if (!await context.MstRoles.AnyAsync(r => r.RoleName == "DESA Head"))
            {
                context.MstRoles.Add(new MstRole { RoleName = "DESA Head" });
                await context.SaveChangesAsync();
            }

            // Ensure Department Maker role exists
            if (!await context.MstRoles.AnyAsync(r => r.RoleName == "Department Maker"))
            {
                context.MstRoles.Add(new MstRole { RoleName = "Department Maker" });
                await context.SaveChangesAsync();
            }

            // Ensure Department Checker role exists
            if (!await context.MstRoles.AnyAsync(r => r.RoleName == "Department Checker"))
            {
                context.MstRoles.Add(new MstRole { RoleName = "Department Checker" });
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedAdminUserAsync(ApplicationDbContext context)
        {
            // Check if admin user already exists
            var adminUser = await context.MasterUsers
                .FirstOrDefaultAsync(u => u.LoginID == "admin");

            var systemAdminRole = await context.MstRoles
                .FirstOrDefaultAsync(r => r.RoleName == "System Admin");

            if (systemAdminRole == null)
            {
                // Create System Admin role if it doesn't exist
                systemAdminRole = new MstRole { RoleName = "System Admin" };
                context.MstRoles.Add(systemAdminRole);
                await context.SaveChangesAsync();
            }

            if (adminUser == null)
            {
                // Create admin user with correct BCrypt hash for "Admin@123"
                // This hash was generated using BCrypt.Net.BCrypt.HashPassword("Admin@123")
                var passwordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");

                adminUser = new MasterUser
                {
                    LoginID = "admin",
                    UserPassword = passwordHash,
                    UserMobileNo = "9876543210",
                    UserEmailID = "admin@haryanastatabstract.com",
                    FullName = "System Administrator",
                    RoleID = systemAdminRole.RoleID,
                    DepartmentID = null, // System Admin doesn't have a department
                    IsActive = true
                };

                context.MasterUsers.Add(adminUser);
                await context.SaveChangesAsync();
            }
            else
            {
                // Update admin user password if it's incorrect (verify it doesn't match)
                // This ensures the password is always correct even if database was manually modified
                try
                {
                    if (!BCrypt.Net.BCrypt.Verify("Admin@123", adminUser.UserPassword))
                    {
                        adminUser.UserPassword = BCrypt.Net.BCrypt.HashPassword("Admin@123");
                        adminUser.IsActive = true;
                        await context.SaveChangesAsync();
                    }
                }
                catch
                {
                    // If password verification fails (invalid hash format), update it
                    adminUser.UserPassword = BCrypt.Net.BCrypt.HashPassword("Admin@123");
                    adminUser.IsActive = true;
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}