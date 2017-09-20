﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using PSKcore.DbModel;
using System;

namespace PSKcore.Migrations
{
    [DbContext(typeof(APPDbContext))]
    partial class APPDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452");

            modelBuilder.Entity("PSKcore.DbModel.LocalSetting", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Key");

                    b.Property<string>("Value");

                    b.HasKey("ID");

                    b.ToTable("LocalSetting");
                });

            modelBuilder.Entity("PSKcore.DbModel.Recording", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("key");

                    b.Property<int>("uid");

                    b.Property<string>("value");

                    b.HasKey("ID");

                    b.ToTable("Recording");
                });

            modelBuilder.Entity("PSKcore.DbModel.StringSequenceObjA", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Data");

                    b.HasKey("ID");

                    b.ToTable("StringSequenceA");
                });

            modelBuilder.Entity("PSKcore.DbModel.StringSequenceObjB", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Data");

                    b.HasKey("ID");

                    b.ToTable("StringSequenceB");
                });

            modelBuilder.Entity("PSKcore.DbModel.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("pid");

                    b.Property<string>("pwd");

                    b.HasKey("ID");

                    b.ToTable("User");
                });
#pragma warning restore 612, 618
        }
    }
}
