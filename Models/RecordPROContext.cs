using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RecordPRO.Models;

namespace RecordPRO.Models
{
    public class RecordPROContext:DbContext
    {
        public RecordPROContext(DbContextOptions<RecordPROContext> options)
    : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<UserBill>().ToTable("UserBill");
            modelBuilder.Entity<UserNote>().ToTable("UserNote");
            modelBuilder.Entity<UserFace>().ToTable("UserFace");
            modelBuilder.Entity<UserMemorial>().ToTable("UserMemorial");
            modelBuilder.Entity<UserBody>().ToTable("UserBody");
        }
        public DbSet<User> Users { get; set; }

        public DbSet<RecordPRO.Models.UserBill> UserBill { get; set; }

        public DbSet<RecordPRO.Models.UserNote> UserNote { get; set; }

        public DbSet<RecordPRO.Models.UserFace> UserFace { get; set; }
        public DbSet<RecordPRO.Models.UserMemorial> UserMemorial { get; set; }
        public DbSet<UserBody> UserBody { get; set; }
    }
}
