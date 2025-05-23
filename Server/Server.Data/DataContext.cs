﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Server.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = Server.Core.Entities.File;

namespace Server.Data
{
    public class DataContext(IConfiguration configuration) : DbContext
    {
        private readonly IConfiguration _configuration = configuration;
        public DbSet<User> Users { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var mySqlServerVersionInfo = new MySqlServerVersion(new Version(8, 0, 41));

            optionsBuilder.UseMySql(_configuration["DBConnectionString"], mySqlServerVersionInfo);
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                var entity = (BaseModel)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.Now;
                    entity.CreatedBy = GetLoggedInUserId();
                }

                entity.UpdatedAt = DateTime.Now;
                entity.UpdatedBy = GetLoggedInUserId();
            }

            return base.SaveChanges();
        }

        private int GetLoggedInUserId()
        {
            //לממש החזרת מזהה משתמש מחובר
            return 1; 
        }
    }
}
