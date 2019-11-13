using Microsoft.EntityFrameworkCore.Migrations;

namespace bd_backend.Migrations
{
    public partial class DBClinicv2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MenagerComment",
                table: "LaboratoryExaminations",
                newName: "ManagerComment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MenagerComment",
                table: "LaboratoryExaminations",
                newName: "ManagerComment");
        }
    }
}
