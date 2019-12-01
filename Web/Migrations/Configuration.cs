
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Web.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<Web.Models.Domain.EFDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }
    } 
}