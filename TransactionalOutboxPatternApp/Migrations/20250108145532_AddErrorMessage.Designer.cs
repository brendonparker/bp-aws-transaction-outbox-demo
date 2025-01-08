﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TransactionalOutboxPatternApp.Infrastructure;

#nullable disable

namespace TransactionalOutboxPatternApp.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250108145532_AddErrorMessage")]
    partial class AddErrorMessage
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TransactionalOutboxPatternApp.Domain.Order", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("status");

                    b.HasKey("Id")
                        .HasName("pk_order");

                    b.ToTable("order", (string)null);
                });

            modelBuilder.Entity("TransactionalOutboxPatternApp.Infrastructure.TransactionOutbox", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("AttemptCount")
                        .HasColumnType("integer")
                        .HasColumnName("attempt_count");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<long>("EntityId")
                        .HasColumnType("bigint")
                        .HasColumnName("entity_id");

                    b.Property<string>("EntityType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("entity_type");

                    b.Property<string>("ErrorMessage")
                        .HasColumnType("text")
                        .HasColumnName("error_message");

                    b.Property<string>("EventType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("event_type");

                    b.Property<bool>("IsProcessed")
                        .HasColumnType("boolean")
                        .HasColumnName("is_processed");

                    b.Property<string>("JsonContent")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("json_content");

                    b.Property<DateTime?>("ProcessedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("processed_at");

                    b.HasKey("Id")
                        .HasName("pk_transaction_outbox");

                    b.ToTable("transaction_outbox", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
