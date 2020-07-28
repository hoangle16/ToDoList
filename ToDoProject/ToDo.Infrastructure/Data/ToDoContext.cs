using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ToDo.Domain.Entities;
using ToDo.Infrastructure.EntityConfigurations;

namespace ToDo.Infrastructure.Data
{
    public class ToDoContext : DbContext
    {
        public ToDoContext() { }
        public ToDoContext(DbContextOptions<ToDoContext> options) : base(options)
        {

        }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<ToDoItem> ToDoItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ToDoItemEntityTypeConfiguration());
        }
    }
}
