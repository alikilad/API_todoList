using Microsoft.EntityFrameworkCore;
using todolist_API.Models;
namespace todolist_API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<todolist_API.Models.Task> Tasks { get; set; }  // Using fully qualified name for Task to avoid conflict with System.Threading.Tasks.Task
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<todolist_API.Models.Task>() 
                .HasMany(t => t.Tags)
                .WithMany(t => t.Tasks)
                .UsingEntity(j => j.ToTable("TaskTags"));
        }
    } 
}