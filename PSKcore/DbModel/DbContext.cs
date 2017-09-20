using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSKcore.DbModel
{
    public class APPDbContext : DbContext
    {
        public DbSet<StringSequenceObjA> SA { get; set; }

        public DbSet<StringSequenceObjB> SB { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Recording> Recordings { get; set; }

        public DbSet<LocalSetting> LocalSettings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite("FileName=localdb.db");
    }
}
