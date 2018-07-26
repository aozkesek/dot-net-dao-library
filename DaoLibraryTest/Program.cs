
using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DaoLibraryTest
{
	class Program
	{
		public static IConfiguration Configuration;
		public static string ConnectionString { get { return Configuration["connectionstrings:postgres"]; } }      
		public static Counter ResultCounters = new Counter();

        static void Main(string[] args)
        {

			Configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();
                
			List<Task<Counter>> tasks = new List<Task<Counter>>();

			PrintCount();

			PersonDao pd = new PersonDao();

			int k = pd.Count();
            for (int i = 0; i < 32; i++)
            {
                Person p = new Person() { Name = "person." + k + i, Surname = "surname." };
                p.Phones = new List<Phone>() { 
					new Phone() { CountryCode = "90", AreaCode = "555", PhoneNumber = "1234567", Person = p }
					, new Phone() { CountryCode = "90", AreaCode = "222", PhoneNumber = "7654321", Person = p }
					};
                p.Addresses = new List<Address>() { 
					new Address() { Country = "TR", City = "IST", County = "CEKMEKOY", Person = p } 
					, new Address() { Country = "TR", City = "IST", County = "KADIKOY", Person = p }};
                pd.Create(p);
            }

            PrintCount();

			int p1 = PersonDao.PageNumber(pd.Count(), pd.PageSize);
			int p2 = PersonDao.PageNumber(pd.Count(new Person(){Surname="surname."}), pd.PageSize);
			List<Person> pl = pd.Read(new Person() { Surname = "surname." }, p2);
            pl.ForEach((Person p) => {
                Console.WriteLine(p);
				((List<Address>)p.Addresses).ForEach ((Address a) => Console.WriteLine(a));
                ((List<Phone>)p.Phones).ForEach ((Phone ph) => Console.WriteLine(ph));
            });
 
			for (int i = 0; i < 64; i++)
			{
				Task<Counter> t = new Task<Counter>((pi) => { return CRUDTest(pi.ToString()); }, "people-" + i);
				t.Start();
				tasks.Add(t);

			}

            try { Task.WaitAll(tasks.ToArray()); } catch (Exception e) { Console.WriteLine(e); };

            tasks.ForEach((Task<Counter> task) => { try { Console.WriteLine(task.Result); } catch (Exception e) { Console.WriteLine(e); } });

			PrintCount();

		}

		public static void PrintCount()
		{
			Console.WriteLine(
				string.Format("[{0} : {1} : {2}]",
							  DateTime.Now, new PersonDao().Count(), DateTime.Now)
			);
		}

		public static Counter CRUDTest(string person)
		{
            string name = new string(person.ToCharArray());
            string surname = name + "-surname";
			string surnameupdated = surname + "-updated";

			Counter counter = new Counter();
			counter.Start = DateTime.Now;
			counter.Name = "crudtest for " + person;

			PersonDao personDao = new PersonDao();

			personDao.PageSize = 10;

			for (int k = 0; k < 64; k++)
            {
				try {
                    Person p = new Person();
                    p.Name = name + "-" + k;
                    p.Surname = surname;
                    p.Phones = new List<Phone>() { new Phone() { AreaCode="532", PhoneNumber="1234567", Person=p } };
                    p.Addresses = new List<Address>() { new Address() { City="istanbul", County="cekmekoy", Person=p } };
                    personDao.Create(p); 
                }
				catch (Exception e) { Console.WriteLine(counter.Name + "<>" + e.Message); }
			}
                
			counter.Create = personDao.Count(new Person() { Surname = surname });
			int totalPage = PersonDao.PageNumber(counter.Create, personDao.PageSize);

			counter.Read = 0;
			for (int j = 1; j <= totalPage; j++)
            {
                try { counter.Read += personDao.Read(new Person() { Surname = surname }, j).Count; }
                catch(Exception e) { Console.WriteLine(counter.Name + "<>" + e.Message); }
            }
				
			List<Person> pList = personDao.Read(new Person() { Surname = surname });
			pList.ForEach((Person p) =>
			{
				p.Surname = surnameupdated;
                //if (p.Addresses != null)
                    p.Addresses[0].County = "UMRANIYE";
                //if (p.Phones != null)
                    p.Phones[0].PhoneNumber = "1098765";
				try { personDao.Update(p); } catch (Exception e) { Console.WriteLine(counter.Name + "<>" + e.Message); }
			});

			counter.Update = personDao.Read(new Person() { Surname = surnameupdated }).Count;

            try 
            { counter.Delete = personDao.Delete(new Person() { Surname = surname }).Count; }
            catch (Exception e) { Console.WriteLine(e); }

			try
			{ counter.Delete += personDao.Delete(new Person() { Surname = surnameupdated }).Count; }
			catch (Exception e) { Console.WriteLine(e); }

			counter.Finish = DateTime.Now;

			return counter;
		}

	}

	class Counter
	{
		public string Name { get; set; }
		public int Create { get; set; }
		public int Read { get; set; }
		public int Update { get; set; }
		public int Delete { get; set; }
		public DateTime Start { get; set; }
		public DateTime Finish { get; set; }

		public override string ToString()
		{
			return string.Format("[Start: {0}, Finish: {1}, Name: {2}, Create: {3}, Read: {4}, Update: {5}, Delete: {6}]"
								 , Start, Finish, Name, Create, Read, Update, Delete);
		}	
	}
}
