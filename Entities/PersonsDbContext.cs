using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.Text.Json;

namespace Entities
{
    public class PersonsDbContext : DbContext
    {
        public PersonsDbContext(DbContextOptions options) : base (options)
        {
        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            // Seed to Countries
            string countriesJson = File.ReadAllText("countries.json");
            List<Country>? countries = JsonSerializer.Deserialize<List<Country>>(countriesJson);

            foreach (Country country in countries!)
            {
                modelBuilder.Entity<Country>().HasData(country);
            }

            string personsJson = File.ReadAllText("persons.json");
            List<Person>? persons = JsonSerializer.Deserialize<List<Person>>(personsJson);

            foreach (Person person in persons!)
            {
                modelBuilder.Entity<Person>().HasData(person);
            }
        }
    }
}
