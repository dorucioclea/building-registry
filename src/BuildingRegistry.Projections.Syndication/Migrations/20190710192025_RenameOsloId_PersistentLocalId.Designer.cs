﻿// <auto-generated />
using System;
using BuildingRegistry.Projections.Syndication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BuildingRegistry.Projections.Syndication.Migrations
{
    [DbContext(typeof(SyndicationContext))]
    [Migration("20190710192025_RenameOsloId_PersistentLocalId")]
    partial class RenameOsloId_PersistentLocalId
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.ProjectionStates.ProjectionStateItem", b =>
                {
                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("Position");

                    b.HasKey("Name")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("ProjectionStates","BuildingRegistrySyndication");
                });

            modelBuilder.Entity("BuildingRegistry.Projections.Syndication.Address.AddressPersistentLocalIdItem", b =>
                {
                    b.Property<Guid>("AddressId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsComplete");

                    b.Property<bool>("IsRemoved");

                    b.Property<string>("PersistentLocalId");

                    b.Property<long>("Position");

                    b.Property<DateTimeOffset?>("Version");

                    b.HasKey("AddressId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("IsComplete", "IsRemoved");

                    b.ToTable("AddressPersistentLocalIdSyndication","BuildingRegistrySyndication");
                });

            modelBuilder.Entity("BuildingRegistry.Projections.Syndication.Parcel.BuildingParcelLatestItem", b =>
                {
                    b.Property<Guid>("ParcelId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CaPaKey");

                    b.Property<bool>("IsComplete");

                    b.Property<bool>("IsRemoved");

                    b.Property<long>("Position");

                    b.Property<int?>("Status");

                    b.Property<DateTimeOffset?>("Version");

                    b.HasKey("ParcelId")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("CaPaKey");

                    b.ToTable("BuildingParcelLatestItems","BuildingRegistryLegacy");
                });
#pragma warning restore 612, 618
        }
    }
}
