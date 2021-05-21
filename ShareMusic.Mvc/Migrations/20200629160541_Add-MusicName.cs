using Microsoft.EntityFrameworkCore.Migrations;

namespace ShareMusic.Mvc.Migrations
{
    public partial class AddMusicName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MusicName",
                table: "Musics",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MusicName",
                table: "Musics");
        }
    }
}
