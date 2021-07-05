﻿// <auto-generated />
using System;
using AnomalyService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AnomalyService.Migrations
{
    [DbContext(typeof(ApplicationDBContext))]
    [Migration("20210624062836_AddLatLonForAnomaly")]
    partial class AddLatLonForAnomaly
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.6");

            modelBuilder.Entity("AnomalyService.Models.Anomaly", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AnomalyType")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Latitude")
                        .HasColumnType("longtext");

                    b.Property<string>("Longitude")
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("Anomalys");
                });

            modelBuilder.Entity("AnomalyService.Models.AnomalyReport", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("AnomalyId")
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Kp")
                        .HasColumnType("longtext");

                    b.Property<string>("Latitude")
                        .HasColumnType("longtext");

                    b.Property<string>("Longitude")
                        .HasColumnType("longtext");

                    b.Property<string>("Road")
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("AnomalyId");

                    b.ToTable("AnomalyReports");
                });

            modelBuilder.Entity("AnomalyService.Models.AnomalyReportImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("AnomalyReportId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("ImageId")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("AnomalyReportId");

                    b.HasIndex("ImageId");

                    b.ToTable("AnomalyReportImages");
                });

            modelBuilder.Entity("AnomalyService.Models.Image", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Kp")
                        .HasColumnType("longtext");

                    b.Property<string>("Latitude")
                        .HasColumnType("longtext");

                    b.Property<string>("Longitude")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<string>("Road")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("TakenAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("AnomalyService.Models.AnomalyReport", b =>
                {
                    b.HasOne("AnomalyService.Models.Anomaly", null)
                        .WithMany("AnomalyReport")
                        .HasForeignKey("AnomalyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AnomalyService.Models.AnomalyReportImage", b =>
                {
                    b.HasOne("AnomalyService.Models.AnomalyReport", "AnomalyReport")
                        .WithMany("AnomelyReportImage")
                        .HasForeignKey("AnomalyReportId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AnomalyService.Models.Image", "Image")
                        .WithMany("AnomelyReportImage")
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AnomalyReport");

                    b.Navigation("Image");
                });

            modelBuilder.Entity("AnomalyService.Models.Anomaly", b =>
                {
                    b.Navigation("AnomalyReport");
                });

            modelBuilder.Entity("AnomalyService.Models.AnomalyReport", b =>
                {
                    b.Navigation("AnomelyReportImage");
                });

            modelBuilder.Entity("AnomalyService.Models.Image", b =>
                {
                    b.Navigation("AnomelyReportImage");
                });
#pragma warning restore 612, 618
        }
    }
}
