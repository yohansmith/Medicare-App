using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicareApp.Migrations
{
    /// <inheritdoc />
    public partial class AddFeedbackPatientRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PatientID",
                table: "Feedbacks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_PatientID",
                table: "Feedbacks",
                column: "PatientID");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Patients_PatientID",
                table: "Feedbacks",
                column: "PatientID",
                principalTable: "Patients",
                principalColumn: "PatientID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Patients_PatientID",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_PatientID",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "PatientID",
                table: "Feedbacks");
        }
    }
}
