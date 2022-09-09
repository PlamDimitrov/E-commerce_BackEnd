using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ecommerce_API.Migrations
{
    public partial class MenuDbUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_subSubMenuLinks_subMenus_SubMenuId",
                table: "subSubMenuLinks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_subSubMenuLinks",
                table: "subSubMenuLinks");

            migrationBuilder.RenameTable(
                name: "subSubMenuLinks",
                newName: "Links");

            migrationBuilder.RenameIndex(
                name: "IX_subSubMenuLinks_SubMenuId",
                table: "Links",
                newName: "IX_Links_SubMenuId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Links",
                table: "Links",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Links_subMenus_SubMenuId",
                table: "Links",
                column: "SubMenuId",
                principalTable: "subMenus",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Links_subMenus_SubMenuId",
                table: "Links");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Links",
                table: "Links");

            migrationBuilder.RenameTable(
                name: "Links",
                newName: "subSubMenuLinks");

            migrationBuilder.RenameIndex(
                name: "IX_Links_SubMenuId",
                table: "subSubMenuLinks",
                newName: "IX_subSubMenuLinks_SubMenuId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_subSubMenuLinks",
                table: "subSubMenuLinks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_subSubMenuLinks_subMenus_SubMenuId",
                table: "subSubMenuLinks",
                column: "SubMenuId",
                principalTable: "subMenus",
                principalColumn: "Id");
        }
    }
}
