
using u22519433_HW03.Models;
using System.Data.Entity;
using Type = u22519433_HW03.Models.Type;
namespace u22519433_HW03.DAL
{
    public class LibraryContext: DbContext
    {
        public LibraryContext() : base("name=LibraryConnection")
        {
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Borrow> Borrows { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Type> Types { get; set; }
        public DbSet<SavedReport> SavedReports { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Book>()
                .HasRequired(b => b.Author)
                .WithMany(a => a.Books)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Book>()
                .HasRequired(b => b.Type)
                .WithMany(t => t.Books)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Borrow>()
                .HasRequired(b => b.Student)
                .WithMany(s => s.Borrows)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Borrow>()
                .HasRequired(b => b.Book)
                .WithMany(book => book.Borrows)
                .WillCascadeOnDelete(false);
        }
    }

}
