﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RaspSecure.Context;

namespace RaspSecure.Migrations.LogsDb
{
    [DbContext(typeof(LogsDbContext))]
    partial class LogsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity("RaspSecure.Models.LogEntity", b =>
                {
                    b.Property<int>("LogEntityId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("AccessTime");

                    b.Property<string>("Code");

                    b.Property<bool>("IsSucceed");

                    b.Property<int>("SecurityCodeId");

                    b.HasKey("LogEntityId");

                    b.ToTable("LogEntitys");
                });
#pragma warning restore 612, 618
        }
    }
}
