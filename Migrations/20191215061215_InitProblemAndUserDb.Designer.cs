﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Semicolon.OnlineJudge.Data;

namespace Semicolon.OnlineJudge.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20191215061215_InitProblemAndUserDb")]
    partial class InitProblemAndUserDb
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0");

            modelBuilder.Entity("Semicolon.OnlineJudge.Models.Problemset.Problem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AuthorId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("ExampleData")
                        .HasColumnType("TEXT");

                    b.Property<string>("JudgeProfile")
                        .HasColumnType("TEXT");

                    b.Property<string>("PassRate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("PublishTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Problems");
                });

            modelBuilder.Entity("Semicolon.OnlineJudge.Models.User.OJUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<string>("NickName")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProblemsPassedId")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("OJUsers");
                });
#pragma warning restore 612, 618
        }
    }
}
