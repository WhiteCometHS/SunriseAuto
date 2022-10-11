using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using SunriseAuto.Areas.Identity.Data;
using System.Text;

#nullable disable

namespace SunriseAuto.Migrations
{
    public partial class AddAdminAccount : Migration
    {
        const string ADMIN_USER_GUID = "6c26733d-5eab-48db-838c-70bc8e875579";
        const string ADMIN_ROLE_GUID = "02491275-e394-41b3-a269-92c6e2c912bf";
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var hasher = new PasswordHasher<User>();
            var passwordHash = hasher.HashPassword(null, "admin");

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("INSERT INTO AspNetUSers(Id, UserName,NormalizedUserName,Email,EmailConfirmed,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnabled,AccessFailedCount,NormalizedEmail,PasswordHash,SecurityStamp,FirstName,LastName,Address)");
            sb.AppendLine("VALUES(");
            sb.AppendLine($"'{ADMIN_USER_GUID}'");
            sb.AppendLine(",'admin@gmail.com'");
            sb.AppendLine(",'ADMIN@GMAIL.COM'");
            sb.AppendLine(",'admin@gmail.com'");
            sb.AppendLine(",0");
            sb.AppendLine(",0");
            sb.AppendLine(",0");
            sb.AppendLine(",0");
            sb.AppendLine(",0");
            sb.AppendLine(",'ADMIN@GMAIL.COM'");
            sb.AppendLine($",'{passwordHash}'");
            sb.AppendLine(", ''");
            sb.AppendLine(",'Kiril'");
            sb.AppendLine(",'Milosh'");
            sb.AppendLine(",'Siedlce, 3-Maja, 49'");
            sb.AppendLine(")");

            migrationBuilder.Sql(sb.ToString());
            migrationBuilder.Sql($"INSERT INTO AspNetRoles (Id, Name, NormalizedName) VALUES ('{ADMIN_ROLE_GUID}','Admin','ADMIN')");
            migrationBuilder.Sql($"INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES ('{ADMIN_USER_GUID}','{ADMIN_ROLE_GUID}')");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DELETE FROM AspNetUserRoles WHERE UserId = '{ADMIN_USER_GUID}' AND RoleId = '{ADMIN_ROLE_GUID}'");
            migrationBuilder.Sql($"DELETE FROM AspNetUsers WHERE Id = '{ADMIN_USER_GUID}'");
            migrationBuilder.Sql($"DELETE FROM AspNetRoles WHERE Id = '{ADMIN_ROLE_GUID}'");

        }
    }
}
