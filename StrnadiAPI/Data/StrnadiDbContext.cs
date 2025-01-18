using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using StrnadiAPI.Data.Models;
using StrnadiAPI.Data.Models.Database;

namespace StrnadiAPI.Data;

public partial class StrnadiDbContext : DbContext
{
    // 
    
    public StrnadiDbContext()
    {
    }

    public StrnadiDbContext(DbContextOptions<StrnadiDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bird> Birds { get; set; }

    public virtual DbSet<Dialect> Dialects { get; set; }

    public virtual DbSet<FiltredSubrecording> FiltredSubrecordings { get; set; }

    public virtual DbSet<Photo> Photos { get; set; }

    public virtual DbSet<Recording> Recordings { get; set; }

    public virtual DbSet<RecordingPart> RecordingParts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserObject> UserObjectsRepository { get; set; }

    private string _connectionString => Environment.GetEnvironmentVariable("Default")!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(_connectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bird>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("birds_pkey");

            entity.ToTable("birds");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Mfcc)
                .HasMaxLength(45)
                .HasComment("mel-frekvenční spektrální koeficient")
                .HasColumnName("mfcc");
            entity.Property(e => e.Uid)
                .HasMaxLength(100)
                .HasComment("poprvé zaznamenán kdy a kde")
                .HasColumnName("uid");
        });

        modelBuilder.Entity<Dialect>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("dialects_pkey");

            entity.ToTable("dialects", tb => tb.HasComment("Číselník dialektů"));

            entity.HasIndex(e => e.DialectCode, "dialects_dialect_code_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DialectCode)
                .HasMaxLength(10)
                .HasColumnName("dialect_code");
            entity.Property(e => e.PathSpectrogram)
                .HasMaxLength(255)
                .HasColumnName("path_spectrogram");
            entity.Property(e => e.PathVoice)
                .HasMaxLength(255)
                .HasComment("cesta k souboru se vzorovým zvukovým záznamem dialektu")
                .HasColumnName("path_voice");
        });

        modelBuilder.Entity<FiltredSubrecording>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("filtred_subrecordings_pkey");

            entity.ToTable("filtred_subrecordings", tb => tb.HasComment("Vysekané signifikantní části z nahrávek (např. 5ti vteřinové úseky se zaznamenaným dialektem). Výstup prvního AI modelu pro další zpracování a určení dialektu druhým AI modelem."));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BirdsId).HasColumnName("birds_id");
            entity.Property(e => e.End)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("end");
            entity.Property(e => e.PathFile)
                .HasMaxLength(255)
                .HasColumnName("path_file");
            entity.Property(e => e.ProbabilityVector)
                .HasMaxLength(45)
                .HasDefaultValueSql("'(0,0,0,0,0,0,0)'::character varying")
                .HasComment("pravděpodobnostní vektor jednotlivých dialektů (s jakou mírou pravděpodobnosti se v nahrávce vyskytují: dialekt A, dialekt B, dialekt C,...)")
                .HasColumnName("probability_vector");
            entity.Property(e => e.RecordingPartsId).HasColumnName("recording_parts_id");
            entity.Property(e => e.RepresentantFlag)
                .HasComment("1 - vzorek je reprezentant nahrávky\\n0 - nereprezentuje nahrávku (podružný vzorek)")
                .HasColumnName("representant_flag");
            entity.Property(e => e.Start)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("start");
            entity.Property(e => e.State)
                .HasComment("0-vloženo\\n1-vytvořen pravděpodobnostní vektor\\n2-pravděpodobnost dialektu X přesáhla nastavenou hranici a AI sama přiřadila dialekt\\n3- přiřazení dialektu ověřeno uživatelem\\n4- manuální přiřazení dialektu\\n5- dialekt nelze určit ani manuálně")
                .HasColumnName("state");

            entity.HasOne(d => d.Birds).WithMany(p => p.FiltredSubrecordings)
                .HasForeignKey(d => d.BirdsId)
                .HasConstraintName("fk_filtred_subrecordings_birds");

            entity.HasOne(d => d.RecordingParts).WithMany(p => p.FiltredSubrecordings)
                .HasForeignKey(d => d.RecordingPartsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_filtred_subrecordings_recording_parts");
        });

        modelBuilder.Entity<Photo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("photos_pkey");

            entity.ToTable("photos", tb => tb.HasComment("K nahrávkám může být jedna a více fotek. (Asi bych doporučoval ukládat i meta-data. Nicmoc nás to nebude stát a třeba se to bude někdy hodit - např při dalším zpracování pomocí AI.)\n\npřidat GPS k fotce"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreationDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("creation_date");
            entity.Property(e => e.Device)
                .HasMaxLength(45)
                .HasColumnName("device");
            entity.Property(e => e.GpsLatitude).HasColumnName("gps_latitude");
            entity.Property(e => e.GpsLongitude).HasColumnName("gps_longitude");
            entity.Property(e => e.PhotoFilePath)
                .HasMaxLength(255)
                .HasColumnName("photo_file_path");
            entity.Property(e => e.RecordingPartsId).HasColumnName("recording_parts_id");

            entity.HasOne(d => d.RecordingParts).WithMany(p => p.Photos)
                .HasForeignKey(d => d.RecordingPartsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_photos_recording_parts");
        });

        modelBuilder.Entity<Recording>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("recordings_pkey");

            entity.ToTable("recordings", tb => tb.HasComment("Tabulka s originálními - dlouhými nahrávkami."));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ByApp)
                .HasComment("1 - nahráno s palikace,\n0 - nahráno jinak")
                .HasColumnName("by_app");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Device)
                .HasMaxLength(45)
                .HasComment("čím bylo nahrané (předá přímo mobilní telefon, případně uživatel zadá u externích zařízení)\\n\\nbylo by fajn nabízet uživateli poslední zařízení, co vkládal (+ našeptávač)\\n\\n???u uživatele přidat defaultní zařízení???\\n\\n- přidat políčko přes_ appku nebo jinak")
                .HasColumnName("device");
            entity.Property(e => e.EstimatedBirdsCount)
                .HasComment("1, 2, 3-více")
                .HasColumnName("estimated_birds_count");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.NotePost).HasColumnName("note_post");
            entity.Property(e => e.RecordingFilePath).HasColumnName("recording_file_path");
            entity.Property(e => e.State)
                .HasDefaultValue((short)0)
                .HasComment("0-založeno\\n1-další zpracování - je třeba rozmyslet stavy, jakými zpracování může probíhat a jakých výsledků může být zpracováním dosaženo\\n...")
                .HasColumnName("state");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Recordings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_recordings_users");
        });

        modelBuilder.Entity<RecordingPart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("recording_parts_pkey");

            entity.ToTable("recording_parts", tb => tb.HasComment("Originální nahrávka může být slepena z jednotlivých částí. Bylo by fajn ukládat i informace o těchto částech. Mohou se tam vyskytnout třeba nějaké zdroje chyb pro zpracování AI."));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.End)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("end");
            entity.Property(e => e.FilePath)
                .HasMaxLength(255)
                .HasColumnName("file_path");
            entity.Property(e => e.GpsLatitudeEnd).HasColumnName("gps_latitude_end");
            entity.Property(e => e.GpsLatitudeStart).HasColumnName("gps_latitude_start");
            entity.Property(e => e.GpsLongitudeEnd).HasColumnName("gps_longitude_end");
            entity.Property(e => e.GpsLongitudeStart).HasColumnName("gps_longitude_start");
            entity.Property(e => e.RecordingsId).HasColumnName("recordings_id");
            entity.Property(e => e.Square)
                .HasMaxLength(5)
                .IsFixedLength()
                .HasComment("určení KFME čtverce ve formátu 0000a (56°0\\N 5°40\\E)\\n\\nPokud by bylo GPS mimo rozsah KFME, byl by squre NULL")
                .HasColumnName("square");
            entity.Property(e => e.Start)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("start");

            entity.HasOne(d => d.Recordings).WithMany(p => p.RecordingParts)
                .HasForeignKey(d => d.RecordingsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_recording_parts_recordings");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users", tb => tb.HasComment("tabulka uživatelů - možná ještě budou přibývat sloupečky a měnit se datové typy"));

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Consent)
                .HasDefaultValue(false)
                .HasComment("přírodovědci dodají seznam \"souhlasů s\" - abychom byli schopni přiřadit hodnoty")
                .HasColumnName("consent");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("creation_date");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .HasColumnName("first_name");
            entity.Property(e => e.IsEmailVerified)
                .HasDefaultValue(false)
                .HasColumnName("is_email_verified");
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .HasColumnName("last_name");
            entity.Property(e => e.Nickname)
                .HasMaxLength(20)
                .HasColumnName("nickname");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
        });

        modelBuilder.Entity<UserObject>(entity =>
            entity.ToTable("user_objects_repository",
                tb => tb.HasComment(
                    "Tabulka pro uchovávání rádoby \"konstant\" (například délka GPS čtverce, délka GPS subčtverce, délka nahrávky pro určení dialektu")));
        
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    public PropertyInfo[] GetUniqueProperties<TEntity>()
    {
        IEntityType? entityType = Model.FindEntityType(typeof(TEntity));

        if (entityType is null)
            throw new Exception($"The type {typeof(TEntity).Name} is not a known dbcontext's entity type.");
        
        IEnumerable<string> propertyNames = entityType.GetIndexes()
            .Where(i => i.IsUnique)
            .SelectMany(index => entityType.GetProperties())
            .Select(property => property.Name);
        
        PropertyInfo[] properties = propertyNames
            .Select(propertyName => typeof(TEntity).GetProperty(propertyName)!)
            .ToArray();
        
        return properties;
    }
}