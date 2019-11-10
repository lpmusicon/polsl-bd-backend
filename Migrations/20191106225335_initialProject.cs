using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace bd_backend.Migrations
{
    public partial class initialProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    DoctorId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Lastname = table.Column<string>(nullable: true),
                    PWZNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctors", x => x.DoctorId);
                });

            migrationBuilder.CreateTable(
                name: "ExaminationsDictionary",
                columns: table => new
                {
                    ExaminationDictionaryId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<char>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExaminationsDictionary", x => x.ExaminationDictionaryId);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryManagers",
                columns: table => new
                {
                    LaboratoryManagerId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Lastname = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryManagers", x => x.LaboratoryManagerId);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryWorkers",
                columns: table => new
                {
                    LaboratoryWorkerId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Lastname = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryWorkers", x => x.LaboratoryWorkerId);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    PatientId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Lastname = table.Column<string>(nullable: true),
                    PESEL = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.PatientId);
                });

            migrationBuilder.CreateTable(
                name: "Receptionists",
                columns: table => new
                {
                    ReceptionistId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Lastname = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receptionists", x => x.ReceptionistId);
                });

            migrationBuilder.CreateTable(
                name: "PhysicalExaminations",
                columns: table => new
                {
                    PhysicalExaminationId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Result = table.Column<string>(nullable: true),
                    ExaminationDictionaryId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhysicalExaminations", x => x.PhysicalExaminationId);
                    table.ForeignKey(
                        name: "FK_PhysicalExaminations_ExaminationsDictionary_ExaminationDictionaryId",
                        column: x => x.ExaminationDictionaryId,
                        principalTable: "ExaminationsDictionary",
                        principalColumn: "ExaminationDictionaryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientVisits",
                columns: table => new
                {
                    PatientVisitId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(nullable: true),
                    Diagnosis = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    RegisterDate = table.Column<DateTime>(nullable: false),
                    CloseDate = table.Column<DateTime>(nullable: false),
                    DoctorId = table.Column<int>(nullable: true),
                    PatientId = table.Column<int>(nullable: true),
                    ReceptionistId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientVisits", x => x.PatientVisitId);
                    table.ForeignKey(
                        name: "FK_PatientVisits_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "DoctorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientVisits_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientVisits_Receptionists_ReceptionistId",
                        column: x => x.ReceptionistId,
                        principalTable: "Receptionists",
                        principalColumn: "ReceptionistId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryExaminations",
                columns: table => new
                {
                    LaboratoryExaminationId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Result = table.Column<string>(nullable: true),
                    DoctorComment = table.Column<string>(nullable: true),
                    OrderDate = table.Column<DateTime>(nullable: false),
                    ExaminationDate = table.Column<DateTime>(nullable: false),
                    MenagerComment = table.Column<string>(nullable: true),
                    ApprovalRejectionDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    ExaminationDictionaryId = table.Column<int>(nullable: true),
                    LaboratoryManagerId = table.Column<int>(nullable: true),
                    LaboratoryWorkerId = table.Column<int>(nullable: true),
                    PatientVisitId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryExaminations", x => x.LaboratoryExaminationId);
                    table.ForeignKey(
                        name: "FK_LaboratoryExaminations_ExaminationsDictionary_ExaminationDictionaryId",
                        column: x => x.ExaminationDictionaryId,
                        principalTable: "ExaminationsDictionary",
                        principalColumn: "ExaminationDictionaryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboratoryExaminations_LaboratoryManagers_LaboratoryManagerId",
                        column: x => x.LaboratoryManagerId,
                        principalTable: "LaboratoryManagers",
                        principalColumn: "LaboratoryManagerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboratoryExaminations_LaboratoryWorkers_LaboratoryWorkerId",
                        column: x => x.LaboratoryWorkerId,
                        principalTable: "LaboratoryWorkers",
                        principalColumn: "LaboratoryWorkerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboratoryExaminations_PatientVisits_PatientVisitId",
                        column: x => x.PatientVisitId,
                        principalTable: "PatientVisits",
                        principalColumn: "PatientVisitId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryExaminations_ExaminationDictionaryId",
                table: "LaboratoryExaminations",
                column: "ExaminationDictionaryId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryExaminations_LaboratoryManagerId",
                table: "LaboratoryExaminations",
                column: "LaboratoryManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryExaminations_LaboratoryWorkerId",
                table: "LaboratoryExaminations",
                column: "LaboratoryWorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryExaminations_PatientVisitId",
                table: "LaboratoryExaminations",
                column: "PatientVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientVisits_DoctorId",
                table: "PatientVisits",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientVisits_PatientId",
                table: "PatientVisits",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientVisits_ReceptionistId",
                table: "PatientVisits",
                column: "ReceptionistId");

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalExaminations_ExaminationDictionaryId",
                table: "PhysicalExaminations",
                column: "ExaminationDictionaryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LaboratoryExaminations");

            migrationBuilder.DropTable(
                name: "PhysicalExaminations");

            migrationBuilder.DropTable(
                name: "LaboratoryManagers");

            migrationBuilder.DropTable(
                name: "LaboratoryWorkers");

            migrationBuilder.DropTable(
                name: "PatientVisits");

            migrationBuilder.DropTable(
                name: "ExaminationsDictionary");

            migrationBuilder.DropTable(
                name: "Doctors");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "Receptionists");
        }
    }
}
