using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initialize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "institutionseq",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "Analysis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Analysis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Institution",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StreetNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Institution", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Instrument",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instrument", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Keyword",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keyword", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Microorganism",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Microorganism", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    institution_id = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employee_Institution_institution_id",
                        column: x => x.institution_id,
                        principalTable: "Institution",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InstitutionAnalyses",
                columns: table => new
                {
                    analysis_id = table.Column<int>(type: "int", nullable: false),
                    institution_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstitutionAnalyses", x => new { x.analysis_id, x.institution_id });
                    table.ForeignKey(
                        name: "FK_InstitutionAnalyses_Analysis_analysis_id",
                        column: x => x.analysis_id,
                        principalTable: "Analysis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InstitutionAnalyses_Institution_institution_id",
                        column: x => x.institution_id,
                        principalTable: "Institution",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InstitutionInstrument",
                columns: table => new
                {
                    institution_id = table.Column<int>(type: "int", nullable: false),
                    instrument_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstitutionInstrument", x => new { x.institution_id, x.instrument_id });
                    table.ForeignKey(
                        name: "FK_InstitutionInstrument_Institution_institution_id",
                        column: x => x.institution_id,
                        principalTable: "Institution",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InstitutionInstrument_Instrument_instrument_id",
                        column: x => x.instrument_id,
                        principalTable: "Instrument",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InstitutionKeyword",
                columns: table => new
                {
                    institution_id = table.Column<int>(type: "int", nullable: false),
                    keyword_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstitutionKeyword", x => new { x.institution_id, x.keyword_id });
                    table.ForeignKey(
                        name: "FK_InstitutionKeyword_Institution_institution_id",
                        column: x => x.institution_id,
                        principalTable: "Institution",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InstitutionKeyword_Keyword_keyword_id",
                        column: x => x.keyword_id,
                        principalTable: "Keyword",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnalysisMicroorganism",
                columns: table => new
                {
                    analysis_id = table.Column<int>(type: "int", nullable: false),
                    microorganism_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisMicroorganism", x => new { x.analysis_id, x.microorganism_id });
                    table.ForeignKey(
                        name: "FK_AnalysisMicroorganism_Analysis_analysis_id",
                        column: x => x.analysis_id,
                        principalTable: "Analysis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnalysisMicroorganism_Microorganism_microorganism_id",
                        column: x => x.microorganism_id,
                        principalTable: "Microorganism",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InstitutionMicroorganism",
                columns: table => new
                {
                    institution_id = table.Column<int>(type: "int", nullable: false),
                    microorganism_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstitutionMicroorganism", x => new { x.institution_id, x.microorganism_id });
                    table.ForeignKey(
                        name: "FK_InstitutionMicroorganism_Institution_institution_id",
                        column: x => x.institution_id,
                        principalTable: "Institution",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InstitutionMicroorganism_Microorganism_microorganism_id",
                        column: x => x.microorganism_id,
                        principalTable: "Microorganism",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeKeyword",
                columns: table => new
                {
                    employee_id = table.Column<int>(type: "int", nullable: false),
                    keyword_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeKeyword", x => new { x.employee_id, x.keyword_id });
                    table.ForeignKey(
                        name: "FK_EmployeeKeyword_Employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeKeyword_Keyword_keyword_id",
                        column: x => x.keyword_id,
                        principalTable: "Keyword",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisMicroorganism_microorganism_id",
                table: "AnalysisMicroorganism",
                column: "microorganism_id");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_institution_id",
                table: "Employee",
                column: "institution_id");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeKeyword_keyword_id",
                table: "EmployeeKeyword",
                column: "keyword_id");

            migrationBuilder.CreateIndex(
                name: "IX_Institution_Name",
                table: "Institution",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InstitutionAnalyses_institution_id",
                table: "InstitutionAnalyses",
                column: "institution_id");

            migrationBuilder.CreateIndex(
                name: "IX_InstitutionInstrument_instrument_id",
                table: "InstitutionInstrument",
                column: "instrument_id");

            migrationBuilder.CreateIndex(
                name: "IX_InstitutionKeyword_keyword_id",
                table: "InstitutionKeyword",
                column: "keyword_id");

            migrationBuilder.CreateIndex(
                name: "IX_InstitutionMicroorganism_microorganism_id",
                table: "InstitutionMicroorganism",
                column: "microorganism_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalysisMicroorganism");

            migrationBuilder.DropTable(
                name: "EmployeeKeyword");

            migrationBuilder.DropTable(
                name: "InstitutionAnalyses");

            migrationBuilder.DropTable(
                name: "InstitutionInstrument");

            migrationBuilder.DropTable(
                name: "InstitutionKeyword");

            migrationBuilder.DropTable(
                name: "InstitutionMicroorganism");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "Analysis");

            migrationBuilder.DropTable(
                name: "Instrument");

            migrationBuilder.DropTable(
                name: "Keyword");

            migrationBuilder.DropTable(
                name: "Microorganism");

            migrationBuilder.DropTable(
                name: "Institution");

            migrationBuilder.DropSequence(
                name: "institutionseq");
        }
    }
}
