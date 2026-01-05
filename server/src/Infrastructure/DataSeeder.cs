using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure;

public static class DataSeeder
{


    public static void SeedUsers(this ModelBuilder modelBuilder)
    {
        PasswordHasher<User> hasher = new();
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                UserName = "admin",
                NormalizedUserName = "ADMIN@GMAIL.COM",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Admin123"),
                PhoneNumber = "555334455",
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnd = null,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                Name = "Admin",
                Surname = "Admininistrator"
            },
            new User
            {
                Id = 2,
                UserName = "User1",
                NormalizedUserName = "USER1",
                Email = "user1@gmail.com",
                NormalizedEmail = "USER1@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "!Ab123123"),
                PhoneNumber = "555334456",
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnd = null,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                Name = "Jon",
                Surname = "Doe"

            },
            new User
            {
                Id = 3,
                UserName = "User2",
                NormalizedUserName = "USER2",
                Email = "user2@gmail.com",
                NormalizedEmail = "USER2@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "!Ab123123"),
                PhoneNumber = "555334456",
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnd = null,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                Name = "Tyler",
                Surname = "Durden"
            }

        );
    }

    public static void SeedRoles(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = -3, Name = "Moderator", NormalizedName = "MODERATOR" },
            new Role { Id = -2, Name = "Admin", NormalizedName = "ADMIN" },
            new Role { Id = -1, Name = "User", NormalizedName = "USER" }
        );
    }

    public static void SeedUserRoles(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole { RoleId = -2, UserId = 1 },
            new UserRole { RoleId = -1, UserId = 2 },
            new UserRole { RoleId = -1, UserId = 3 }
        );
    }

}