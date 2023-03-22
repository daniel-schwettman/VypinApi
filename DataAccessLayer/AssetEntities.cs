using System.Data.Entity;
using VypinApi.Models;

namespace VypinApi.DataAccessLayer
{
    public partial class AssetEntities : DbContext
    {
        public AssetEntities()
            /*: base("name=TagsConnection")*/
            : base("name=AssetEntities")
        {
        }

        public virtual DbSet<Asset> Assets { get; set; }
        public virtual DbSet<TagEntity> TagEntities { get; set; }
        public virtual DbSet<TagHistory> TagHistories { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<AccountGroup> AccountGroups { get; set; }
        public virtual DbSet<TagGroup> TagGroups { get; set; }
		public virtual DbSet<TagType> TagTypes { get; set; }
		public virtual DbSet<TagCategory> TagCategories { get; set; }
        public virtual DbSet<TagAudit> TagAudits { get; set; }
        public virtual DbSet<MicroZoneEntity> MicroZoneEntities { get; set; }
        public virtual DbSet<CartEntity> CartEntities { get; set; }
        public virtual DbSet<DepartmentEntity> DepartmentEntities { get; set; }

        // TODO: Create sets for AccountGroup and TagGroup then use that to filter the tags returned to the user
        //public virtual DbSet<AccountGroupTag> AccountGroupTags { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<AssetEntities>(null);
            base.OnModelCreating(modelBuilder);

        /*
        // Asset
        modelBuilder.Entity<Asset>()
            .Property(e => e.Name)
            .IsUnicode(false);

        // TagEntity
        modelBuilder.Entity<TagEntity>()
            .Property(e => e.RawId)
            .IsUnicode(false);

        modelBuilder.Entity<TagEntity>()
            .Property(e => e.Version)
            .IsUnicode(false);

        modelBuilder.Entity<TagEntity>()
            .Property(e => e.Rssi)
            .IsUnicode(false);

        modelBuilder.Entity<TagEntity>()
            .Property(e => e.mZone1Rssi)
            .IsUnicode(false);

        modelBuilder.Entity<TagEntity>()
            .Property(e => e.LastUpdatedOn)
            .IsUnicode(false);

        modelBuilder.Entity<TagEntity>()
            .Property(e => e.StatusCode)
            .IsUnicode(false);

        modelBuilder.Entity<TagEntity>()
            .Property(e => e.ReceivedOn)
            .IsUnicode(false);

        modelBuilder.Entity<TagEntity>()
            .Property(e => e.Battery)
            .IsUnicode(false);

        modelBuilder.Entity<TagEntity>()
            .Property(e => e.Sensor1)
            .IsUnicode(false);

        modelBuilder.Entity<TagEntity>()
            .Property(e => e.TagType)
            .IsUnicode(false);

        modelBuilder.Entity<TagEntity>()
            .Property(e => e.FirmwareVersion)
            .IsUnicode(false);

        modelBuilder.Entity<TagEntity>()
            .Property(e => e.Raw)
            .IsUnicode(false);

        modelBuilder.Entity<TagEntity>()
            .Property(e => e.MicroZoneCurrent)
            .IsUnicode(false);

        modelBuilder.Entity<TagEntity>()
            .Property(e => e.MicroZonePrevious)
            .IsFixedLength();

        modelBuilder.Entity<TagEntity>()
            .Property(e => e.Name)
            .IsUnicode(false);

        modelBuilder.Entity<TagEntity>()
            .Property(e => e.Latitude)
            .IsUnicode(false);

        modelBuilder.Entity<TagEntity>()
            .Property(e => e.Longitude)
            .IsUnicode(false);

        // Account
        modelBuilder.Entity<Account>()
            .Property(e => e.AccountId);
        modelBuilder.Entity<Account>()
            .Property(e => e.Email)
            .IsUnicode(false);
        modelBuilder.Entity<Account>()
            .Property(e => e.Salt)
            .IsUnicode(false);
        modelBuilder.Entity<Account>()
            .Property(e => e.Hash)
            .IsUnicode(false);
        modelBuilder.Entity<Account>()
            .Property(e => e.Iterations);
        modelBuilder.Entity<Account>()
            .Property(e => e.Session)
            .IsUnicode(false);
        modelBuilder.Entity<Account>()
            .Property(e => e.StatusId);
        */
        }
    }
}
