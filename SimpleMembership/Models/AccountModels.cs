using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using SimpleMembership.Migrations;

namespace SimpleMembership.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("OAuthClientDb")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<OAuthToken> OAuthTokens { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
          //  Database.SetInitializer(new MigrateDatabaseToLatestVersion<UsersContext, Configuration>());
        }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public virtual ICollection<OAuthToken> Tokens { get; set; } 
    }

    public class OAuthToken
    {
        [Key]
        public string Token { get; set; }
        public string Secret { get; set; }
        public string Provider { get; set; }
        public string ProviderUserId { get; set; }

        public int UserId { get; set; }
        public virtual UserProfile User { get; set; }
    }
}
