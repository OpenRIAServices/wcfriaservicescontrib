
namespace EntityToolsTests.Web
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;


    // TODO: Create methods containing your application logic.
    [EnableClientAccess()]
    public class EntityToolsDomainService : DomainService
    {
        public List<Person> GetPersonSet()
        {
            return new List<Person>
            {
                new Person{Name="John", Id = Guid.NewGuid()},
                new Person{Name = "Andy", Id = Guid.NewGuid()}
            };
        }
        public void UpdatePerson(Person person) { }
        public void InsertPerson(Person person) { }
    }
}


