﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RaspSecure.Context;

namespace RaspSecure.Migrations
{
    [DbContext(typeof(RaspSecureDbContext))]
    [Migration("20201010195821_ReCreate")]
    partial class ReCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity("RaspSecure.Models.RefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<DateTime>("Expires");

                    b.Property<string>("Token");

                    b.Property<DateTimeOffset>("UpdatedAt");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("RaspSecure.Models.ResetToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<DateTime>("Expires");

                    b.Property<string>("Token");

                    b.Property<DateTimeOffset>("UpdatedAt");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ResetToken");
                });

            modelBuilder.Entity("RaspSecure.Models.SecurityCode", b =>
                {
                    b.Property<int>("SecurityCodeId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<string>("Description");

                    b.Property<DateTimeOffset>("ExpiredAt");

                    b.Property<bool>("IsActive");

                    b.Property<int?>("IssuerId");

                    b.HasKey("SecurityCodeId");

                    b.HasIndex("IssuerId");

                    b.ToTable("SecurityCodes");
                });

            modelBuilder.Entity("RaspSecure.Models.UserEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<string>("Email");

                    b.Property<string>("Password");

                    b.Property<int>("Role");

                    b.Property<string>("Salt");

                    b.Property<DateTimeOffset>("UpdatedAt");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("RaspSecure.Models.RefreshToken", b =>
                {
                    b.HasOne("RaspSecure.Models.UserEntity", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RaspSecure.Models.ResetToken", b =>
                {
                    b.HasOne("RaspSecure.Models.UserEntity", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RaspSecure.Models.SecurityCode", b =>
                {
                    b.HasOne("RaspSecure.Models.UserEntity", "Issuer")
                        .WithMany()
                        .HasForeignKey("IssuerId");
                });
#pragma warning restore 612, 618
        }
    }
}
