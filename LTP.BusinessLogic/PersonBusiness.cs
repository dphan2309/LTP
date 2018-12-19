using System.Collections.Generic;
using LTP.DataAccess;
using LTP.DataModels;

namespace LTP.BusinessLogic
{
    /// <summary>
    /// Handles business logic for Person data model.
    /// </summary>
    /// <remarks>
    /// For demo purpose, this class acts as a transparent layer on top of DAO class.
    /// However, we can add extra business logic here such as logging if exception occurs.
    /// </remarks>
    public class PersonBusiness
    {
        private readonly PersonDao _personDao = new PersonDao();

        public ExecuteResult<List<PersonSearchResult>> Search(PersonSearchCriteria criteria, ref int recordCount)
        {
            // Add additional logic here.
            return _personDao.Search(criteria, ref recordCount);
        }

        public ExecuteResult<Person> Save(Person person)
        {
            // Add additional logic here.
            return _personDao.Save(person);
        }
    }
}