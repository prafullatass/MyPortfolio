using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyPortfolio.Migrations
{
    public partial class UserIdAsString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_AspNetUsers_UserId",
                table: "AspNetUsers");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "cb046eec-ac93-4101-aa19-9f9424909b73");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserAgencies",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "StreetAddress", "TwoFactorEnabled", "UserName" },
                values: new object[] { "139ed871-45ed-4aa6-9a5b-6a4b79861ae0", 0, "8bc311cf-d2bf-48f0-81b9-407101118f95", "admin@admin.com", true, "Admina", "Straytor", false, null, "ADMIN@ADMIN.COM", "ADMIN@ADMIN.COM", "AQAAAAEAACcQAAAAEHl0aeHDKyAgSzvRPFr5ezsW7UqqNp/GCj1GqCF15emmox/874F5R+1U8IObkGBMBw==", null, false, "cbd149c1-d250-44c7-ab56-c30fcb015ee7", "123 Infinity Way", false, "admin@admin.com" });

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "TransactionId",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2019, 6, 11, 11, 13, 55, 477, DateTimeKind.Local).AddTicks(9304));

            migrationBuilder.UpdateData(
                table: "UserAgencies",
                keyColumn: "UserAgencyId",
                keyValue: 1,
                column: "UserId",
                value: "139ed871-45ed-4aa6-9a5b-6a4b79861ae0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "139ed871-45ed-4aa6-9a5b-6a4b79861ae0");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserAgencies",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_AspNetUsers_UserId",
                table: "AspNetUsers",
                column: "UserId");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "StreetAddress", "TwoFactorEnabled", "UserId", "UserName" },
                values: new object[] { "cb046eec-ac93-4101-aa19-9f9424909b73", 0, "af8c8064-b936-4ac6-b22e-1ae513dffe6e", "admin@admin.com", true, "Admina", "Straytor", false, null, "ADMIN@ADMIN.COM", "ADMIN@ADMIN.COM", "AQAAAAEAACcQAAAAEJYWQ3SZC/wWtp5BlgYATJruyu3OiwSSbPNbGvBE+rSRfmiHFYIMYnJIJ3as+42QmQ==", null, false, "478b5ed4-9881-4c0d-a483-3007385c4f1f", "123 Infinity Way", false, 0, "admin@admin.com" });

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "TransactionId",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2019, 6, 10, 13, 35, 53, 347, DateTimeKind.Local).AddTicks(3714));

            migrationBuilder.UpdateData(
                table: "UserAgencies",
                keyColumn: "UserAgencyId",
                keyValue: 1,
                column: "UserId",
                value: 0);
        }
    }
}
