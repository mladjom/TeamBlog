using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamBlog.Migrations
{
    public partial class Image : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Picture",
                table: "Article",
                newName: "FileName");

            migrationBuilder.AddColumn<byte[]>(
                name: "File",
                table: "Article",
                type: "varbinary(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "File",
                table: "Article");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "Article",
                newName: "Picture");
        }
    }
}
