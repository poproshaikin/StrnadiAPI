using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using StrnadiAPI.Data.Models.Database;

namespace StrnadiAPI.Data;

public partial class StrnadiDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    
    public StrnadiDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public virtual DbSet<Bird> Birds { get; set; }

    public virtual DbSet<DetectedDialect> DetectedDialects { get; set; }

    public virtual DbSet<Dialect> Dialects { get; set; }

    public virtual DbSet<FiltredSubrecording> FiltredSubrecordings { get; set; }

    public virtual DbSet<Photo> Photos { get; set; }

    public virtual DbSet<Recording> Recordings { get; set; }

    public virtual DbSet<RecordingPart> RecordingParts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserObject> UserObjectsRepositories { get; set; }

    public virtual DbSet<UserPart> UserParts { get; set; }

    private string _connectionString => _configuration.GetConnectionString("Default")!;
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(_connectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("\"FiltredSubrecordingState\"", ["1", "2", "3", "4", "5", "6"])
            .HasPostgresEnum("\"UserRole\"", ["user", "admin"]);
        
        modelBuilder.Entity<Bird>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Birds_pkey");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Mfcc).HasMaxLength(45);
            entity.Property(e => e.Uid).HasMaxLength(100);
        });

        modelBuilder.Entity<DetectedDialect>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("DetectedDialects_pkey");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Dialects).WithMany(p => p.DetectedDialects)
                .HasForeignKey(d => d.DialectsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("DetectedDialects_DialectsId_fkey");

            entity.HasOne(d => d.FiltredSubrecording).WithMany(p => p.DetectedDialects)
                .HasForeignKey(d => d.FiltredSubrecordingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("DetectedDialects_FiltredSubrecordingId_fkey");
        });

        modelBuilder.Entity<Dialect>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Dialects_pkey");

            entity.HasIndex(e => e.DialectCode, "Dialects_DialectCode_key").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DialectCode).HasMaxLength(10);
            entity.Property(e => e.PathSpectrogram).HasMaxLength(255);
            entity.Property(e => e.PathVoice).HasMaxLength(255);
        });

        modelBuilder.Entity<FiltredSubrecording>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("FiltredSubrecordings_pkey");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.End).HasColumnType("timestamp without time zone");
            entity.Property(e => e.PathFile).HasMaxLength(255);
            entity.Property(e => e.ProbabilityVector)
                .HasMaxLength(45)
                .HasDefaultValueSql("'(0,0,0,0,0,0,0)'::character varying");
            entity.Property(e => e.Start).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.Birds).WithMany(p => p.FiltredSubrecordings)
                .HasForeignKey(d => d.BirdsId)
                .HasConstraintName("FiltredSubrecordings_BirdsId_fkey");

            entity.HasOne(d => d.RecordingPart).WithMany(p => p.FiltredSubrecordings)
                .HasForeignKey(d => d.RecordingPartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FiltredSubrecordings_RecordingPartId_fkey");
        });

        modelBuilder.Entity<Photo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Photos_pkey");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.PhotoFilePath).HasMaxLength(255);

            entity.HasOne(d => d.RecordingPart).WithMany(p => p.Photos)
                .HasForeignKey(d => d.RecordingPartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_recording_part_id");
        });

        modelBuilder.Entity<Recording>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Recordings_pkey");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.Device).HasMaxLength(45);

            entity.HasOne(d => d.User).WithMany(p => p.Recordings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Recordings_UserId_fkey");
        });

        modelBuilder.Entity<RecordingPart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("RecordingParts_pkey");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.End)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.FilePath).HasMaxLength(255);
            entity.Property(e => e.Square)
                .HasMaxLength(5)
                .IsFixedLength();
            entity.Property(e => e.Start)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.Recording).WithMany(p => p.RecordingParts)
                .HasForeignKey(d => d.RecordingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_recording_id");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Users_pkey");

            entity.HasIndex(e => e.Email, "Users_Email_key").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Role).HasConversion<string>(
                role => ConvertUserRole(role),
                @string => ConvertUserRole(@string)
            );
                
            entity.Property(e => e.Consent).HasDefaultValue(false);
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(255);
            entity.Property(e => e.IsEmailVerified).HasDefaultValue(false);
            entity.Property(e => e.LastName).HasMaxLength(255);
            entity.Property(e => e.Nickname).HasMaxLength(20);
            entity.Property(e => e.Password).HasMaxLength(255);
        });

        modelBuilder.Entity<UserObject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("UserObjectsRepository_pkey");

            entity.ToTable("UserObjectsRepository");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.End).HasColumnType("timestamp without time zone");
            entity.Property(e => e.Name).HasMaxLength(45);
            entity.Property(e => e.Start)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.Value).HasMaxLength(255);
        });

        modelBuilder.Entity<UserPart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("UserParts_pkey");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.RecordingPart).WithMany(p => p.UserParts)
                .HasForeignKey(d => d.RecordingPartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_recording_part_id");
        });
    }

    private string ConvertUserRole(UserRole role)
    {
        return role switch
        {
            UserRole.Admin => "admin",
            UserRole.User => "user",
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
    }

    private UserRole ConvertUserRole(string role)
    {
        return role switch
        {
            "admin" => UserRole.Admin,
            "user" => UserRole.User,
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
    }
}
