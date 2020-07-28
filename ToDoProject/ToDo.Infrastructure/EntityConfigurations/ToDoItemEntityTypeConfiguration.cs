using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using ToDo.Domain.Entities;

namespace ToDo.Infrastructure.EntityConfigurations
{
    class ToDoItemEntityTypeConfiguration : IEntityTypeConfiguration<ToDoItem>
    {
        public void Configure(EntityTypeBuilder<ToDoItem> builder)
        {
            builder.HasKey(e => e.ToDoItemId);
            builder.Property(e => e.ToDoItemId)
                .ValueGeneratedOnAdd()
                .IsRequired();
            builder.Property(e => e.TaskName)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}
