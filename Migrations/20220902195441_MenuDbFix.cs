using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ecommerce_API.Migrations
{
    public partial class MenuDbFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Links_subMenus_SubMenuId",
                table: "Links");

            migrationBuilder.DropForeignKey(
                name: "FK_subMenus_Menu_MenuId",
                table: "subMenus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_subMenus",
                table: "subMenus");

            migrationBuilder.RenameTable(
                name: "subMenus",
                newName: "subMenu");

            migrationBuilder.RenameColumn(
                name: "SubMenuId",
                table: "Links",
                newName: "subMenuId");

            migrationBuilder.RenameIndex(
                name: "IX_Links_SubMenuId",
                table: "Links",
                newName: "IX_Links_subMenuId");

            migrationBuilder.RenameIndex(
                name: "IX_subMenus_MenuId",
                table: "subMenu",
                newName: "IX_subMenu_MenuId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_subMenu",
                table: "subMenu",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Links_subMenu_subMenuId",
                table: "Links",
                column: "subMenuId",
                principalTable: "subMenu",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_subMenu_Menu_MenuId",
                table: "subMenu",
                column: "MenuId",
                principalTable: "Menu",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Links_subMenu_subMenuId",
                table: "Links");

            migrationBuilder.DropForeignKey(
                name: "FK_subMenu_Menu_MenuId",
                table: "subMenu");

            migrationBuilder.DropPrimaryKey(
                name: "PK_subMenu",
                table: "subMenu");

            migrationBuilder.RenameTable(
                name: "subMenu",
                newName: "subMenus");

            migrationBuilder.RenameColumn(
                name: "subMenuId",
                table: "Links",
                newName: "SubMenuId");

            migrationBuilder.RenameIndex(
                name: "IX_Links_subMenuId",
                table: "Links",
                newName: "IX_Links_SubMenuId");

            migrationBuilder.RenameIndex(
                name: "IX_subMenu_MenuId",
                table: "subMenus",
                newName: "IX_subMenus_MenuId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_subMenus",
                table: "subMenus",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Links_subMenus_SubMenuId",
                table: "Links",
                column: "SubMenuId",
                principalTable: "subMenus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_subMenus_Menu_MenuId",
                table: "subMenus",
                column: "MenuId",
                principalTable: "Menu",
                principalColumn: "Id");
        }
    }
}
