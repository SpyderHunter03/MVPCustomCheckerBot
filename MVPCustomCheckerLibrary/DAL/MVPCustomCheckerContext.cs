﻿using Microsoft.EntityFrameworkCore;
using MVPCustomCheckerLibrary.DAL.Entities;
using System;

namespace MVPCustomCheckerLibrary.DAL
{
    public class MVPCustomCheckerContext(DbContextOptions<MVPCustomCheckerContext> options) : DbContext(options)
    {
        public DbSet<Settings> Settings { get; set; }
        public DbSet<AvailableMolds> AvailableMolds { get; set; }
    }
}
