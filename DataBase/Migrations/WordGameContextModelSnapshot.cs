﻿// <auto-generated />
using System;
using DataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataBase.Migrations
{
    [DbContext(typeof(WordGameContext))]
    partial class WordGameContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DataBase.Models.Antonimo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)");

                    b.Property<int>("WordId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("WordId");

                    b.ToTable("Antonimo");
                });

            modelBuilder.Entity("DataBase.Models.Definicion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("WordId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("WordId")
                        .IsUnique();

                    b.ToTable("Definicion");
                });

            modelBuilder.Entity("DataBase.Models.Modo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)");

                    b.HasKey("Id");

                    b.ToTable("Modo");
                });

            modelBuilder.Entity("DataBase.Models.PalabraEn", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)");

                    b.Property<int>("WordId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("WordId")
                        .IsUnique();

                    b.ToTable("PalabraEn");
                });

            modelBuilder.Entity("DataBase.Models.Pista", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("WordId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("WordId")
                        .IsUnique();

                    b.ToTable("Pista");
                });

            modelBuilder.Entity("DataBase.Models.Sinonimo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)");

                    b.Property<int>("WordId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("WordId");

                    b.ToTable("Sinonimo");
                });

            modelBuilder.Entity("DataBase.Models.Uso", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("WordId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("WordId")
                        .IsUnique();

                    b.ToTable("Uso");
                });

            modelBuilder.Entity("DataBase.Models.Word", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)");

                    b.Property<int?>("ModoId")
                        .HasColumnType("integer");

                    b.Property<bool>("Usada")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("Descripcion")
                        .IsUnique();

                    b.HasIndex("ModoId");

                    b.ToTable("Word");
                });

            modelBuilder.Entity("DataBase.Models.WordMode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Fecha")
                        .HasColumnType("datetime2(0)");

                    b.Property<int>("ModoId")
                        .HasColumnType("integer");

                    b.Property<int>("WordId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ModoId");

                    b.HasIndex("WordId");

                    b.ToTable("WordMode");
                });

            modelBuilder.Entity("DataBase.Models.Antonimo", b =>
                {
                    b.HasOne("DataBase.Models.Word", null)
                        .WithMany("Antonimo")
                        .HasForeignKey("WordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataBase.Models.Definicion", b =>
                {
                    b.HasOne("DataBase.Models.Word", null)
                        .WithOne("Definicion")
                        .HasForeignKey("DataBase.Models.Definicion", "WordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataBase.Models.PalabraEn", b =>
                {
                    b.HasOne("DataBase.Models.Word", null)
                        .WithOne("PalabraEn")
                        .HasForeignKey("DataBase.Models.PalabraEn", "WordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataBase.Models.Pista", b =>
                {
                    b.HasOne("DataBase.Models.Word", null)
                        .WithOne("Pista")
                        .HasForeignKey("DataBase.Models.Pista", "WordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataBase.Models.Sinonimo", b =>
                {
                    b.HasOne("DataBase.Models.Word", null)
                        .WithMany("Sinonimo")
                        .HasForeignKey("WordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataBase.Models.Uso", b =>
                {
                    b.HasOne("DataBase.Models.Word", null)
                        .WithOne("Uso")
                        .HasForeignKey("DataBase.Models.Uso", "WordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataBase.Models.Word", b =>
                {
                    b.HasOne("DataBase.Models.Word", null)
                        .WithMany()
                        .HasForeignKey("ModoId");
                });

            modelBuilder.Entity("DataBase.Models.WordMode", b =>
                {
                    b.HasOne("DataBase.Models.Modo", null)
                        .WithMany()
                        .HasForeignKey("ModoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataBase.Models.Word", null)
                        .WithMany()
                        .HasForeignKey("WordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataBase.Models.Word", b =>
                {
                    b.Navigation("Antonimo");

                    b.Navigation("Definicion")
                        .IsRequired();

                    b.Navigation("PalabraEn")
                        .IsRequired();

                    b.Navigation("Pista")
                        .IsRequired();

                    b.Navigation("Sinonimo");

                    b.Navigation("Uso")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
