﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TecEnergy.Database;

#nullable disable

namespace TecEnergy.Database.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20231113094120_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TecEnergy.Database.DataModels.Building", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BuildingName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Buildings");
                });

            modelBuilder.Entity("TecEnergy.Database.DataModels.EnergyData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("EnergyMeterID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Value")
                        .HasColumnType("decimal(18, 2)");

                    b.HasKey("Id");

                    b.HasIndex("EnergyMeterID");

                    b.ToTable("EnergyData");
                });

            modelBuilder.Entity("TecEnergy.Database.DataModels.EnergyMeter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConnectionState")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("InstallmentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("MeasurementPointComment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MeasurementPointName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MeasurementType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReadingFrequency")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("RoomID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RoomID");

                    b.ToTable("EnergyMeters");
                });

            modelBuilder.Entity("TecEnergy.Database.DataModels.Room", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("BuildingID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("RoomComment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoomName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BuildingID");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("TecEnergy.Database.DataModels.EnergyData", b =>
                {
                    b.HasOne("TecEnergy.Database.DataModels.EnergyMeter", "EnergyMeter")
                        .WithMany("EnergyDatas")
                        .HasForeignKey("EnergyMeterID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EnergyMeter");
                });

            modelBuilder.Entity("TecEnergy.Database.DataModels.EnergyMeter", b =>
                {
                    b.HasOne("TecEnergy.Database.DataModels.Room", "Room")
                        .WithMany("EnergyMeters")
                        .HasForeignKey("RoomID");

                    b.Navigation("Room");
                });

            modelBuilder.Entity("TecEnergy.Database.DataModels.Room", b =>
                {
                    b.HasOne("TecEnergy.Database.DataModels.Building", "Building")
                        .WithMany("Rooms")
                        .HasForeignKey("BuildingID");

                    b.Navigation("Building");
                });

            modelBuilder.Entity("TecEnergy.Database.DataModels.Building", b =>
                {
                    b.Navigation("Rooms");
                });

            modelBuilder.Entity("TecEnergy.Database.DataModels.EnergyMeter", b =>
                {
                    b.Navigation("EnergyDatas");
                });

            modelBuilder.Entity("TecEnergy.Database.DataModels.Room", b =>
                {
                    b.Navigation("EnergyMeters");
                });
#pragma warning restore 612, 618
        }
    }
}
