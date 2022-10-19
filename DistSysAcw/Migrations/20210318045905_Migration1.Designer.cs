﻿// <auto-generated />
using DistSysAcw.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DistSysAcw.Migrations
{
    [DbContext(typeof(UserContext))]
    [Migration("20210318045905_Migration1")]
    partial class Migration1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DistSysAcw.Models.User", b =>
                {
                    b.Property<string>("ApiKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Role")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ApiKey");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
