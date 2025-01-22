using Microsoft.EntityFrameworkCore;

namespace DataBase.Models
{
    public class WordGameContext : DbContext
    {
        public DbSet<Modo> Modo { get; set; }
        public DbSet<Word> Word { get; set; }
        public DbSet<Definicion> Definicion { get; set; }
        public DbSet<Sinonimo> Sinonimo { get; set; }
        public DbSet<Antonimo> Antonimo { get; set; }
        public DbSet<Uso> Uso { get; set; }
        public DbSet<PalabraEn> PalabraEn { get; set; }
        public DbSet<Pista> Pista { get; set; }
        public DbSet<WordMode> WordMode { get; set; }

        public WordGameContext(DbContextOptions<WordGameContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Word>()
                .HasIndex(e => e.Descripcion)
                .IsUnique();

            modelBuilder.Entity<Word>()
                .HasOne<Word>()
                .WithMany()
                .HasForeignKey(wm => wm.ModoId);

            modelBuilder.Entity<WordMode>()
                .HasOne<Word>()
                .WithMany()
                .HasForeignKey(wm => wm.WordId);

            modelBuilder.Entity<WordMode>()
                .HasOne<Modo>()
                .WithMany()
                .HasForeignKey(wm => wm.ModoId);

            modelBuilder.Entity<WordMode>()
                .Property(e => e.Fecha)
                .HasColumnType("datetime2(0)");
        }

    }
}
