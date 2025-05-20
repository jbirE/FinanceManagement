using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.DbSql
{

    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
            
            new IdentityRole
            {
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole
            {
                Name = "DepartementManager",
                NormalizedName = "DepartementManager"
            },
            new IdentityRole
            {
                Name = "Financier",
                NormalizedName = "Financier"
            },

            new IdentityRole
            {
                Name = "Employe",
                NormalizedName = "Employe"
            }
            );
        }
    }

}