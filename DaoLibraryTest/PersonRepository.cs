using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Npgsql;
using System;

namespace DaoLibraryTest
{
	public class PersonRepository : DbContext
	{

		public DbSet<Person> Persons { set; get; }
        public DbSet<Phone> Phones { set; get; }
        public DbSet<Address> Addresses { set; get; }

		public PersonRepository()
			//: base(Program.ConnectionString)
		{
            //this.Configuration.ProxyCreationEnabled = false;

		}

		public PersonRepository(DbContextOptions<PersonRepository> options)
			: base(options)
		{

		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{

			optionsBuilder.UseNpgsql(Program.ConnectionString, options => options.EnableRetryOnFailure());

		}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Person>()
						.HasMany(prop => prop.Addresses)
						.WithOne(prop => prop.Person);
			modelBuilder.Entity<Person>()
						.HasMany(prop => prop.Phones)
						.WithOne(prop => prop.Person);
						

            modelBuilder.Entity<Phone>()
                        .HasOne(prop => prop.Person)
                        .WithMany(prop => prop.Phones)
                        .HasForeignKey(prop => prop.PersonId)
						.IsRequired()
						.OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Address>()
                        .HasOne(prop => prop.Person)
                        .WithMany(prop => prop.Addresses)
                        .HasForeignKey(prop => prop.PersonId)
						.IsRequired()						
						.OnDelete(DeleteBehavior.Cascade);


		}

	}
}
