using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using org.mao.Dao;

namespace DaoLibraryTest
{
	public class PersonDao : AbstractMixedBaseDao<Person, PersonRepository>
	{

		public override void CopyModel(Person Source, ref Person Target)
		{
			Target.Name = Source.Name;
			Target.Surname = Source.Surname;
		}

        protected override IQueryable<Person> Include(DbSet<Person> dbset)
        {
            return dbset
                .Include(prop => prop.Addresses)
                .Include(prop => prop.Phones);
            
        }

		public override IQueryable<Person> Filter(IQueryable<Person> query, Person model)
		{
			if (string.IsNullOrEmpty(model.Name) && string.IsNullOrEmpty(model.Surname))
				throw new Exception("Name and Surname are NULL.");

            IQueryable<Person> filteredQuery = query;

			if (string.IsNullOrEmpty(model.Name))
                filteredQuery = filteredQuery.Where((Person person) =>
                            person.Surname.Equals(model.Surname));
			else if (string.IsNullOrEmpty(model.Surname))
				filteredQuery = filteredQuery.Where((Person person) =>
                            person.Name.Equals(model.Name));
            else 
                filteredQuery = filteredQuery.Where((Person person) =>
                            person.Name.Equals(model.Name)
                            && person.Surname.Equals(model.Surname));
            return filteredQuery;
		}
	}
}
