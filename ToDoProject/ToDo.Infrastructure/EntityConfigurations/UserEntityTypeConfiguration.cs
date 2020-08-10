using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using ToDo.Domain.Entities;

namespace ToDo.Infrastructure.EntityConfigurations
{
    class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(e => e.UserId);
            builder.Property(e => e.UserId)
                .ValueGeneratedOnAdd()
                .IsRequired();
            builder.Property(e => e.UserName)
                .HasMaxLength(20)
                .IsRequired();
            builder.Property(e => e.Name)
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(e => e.Role)
                .IsRequired();
            builder.Property(e => e.Email)
                .IsRequired();

            builder.HasMany(t => t.TodoItems)
                .WithOne(u => u.User)
                .HasForeignKey(e => e.UserId);
        }
    }
}
