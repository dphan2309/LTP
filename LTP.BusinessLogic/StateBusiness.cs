using System.Collections.Generic;
using LTP.DataAccess;
using LTP.DataModels;

namespace LTP.BusinessLogic
{
    /// <summary>
    /// Handles business logic for State data model.
    /// </summary>
    /// <remarks>
    /// For demo purpose, this class acts as a transparent layer on top of DAO class.
    /// However, we can add extra business logic here such as logging if exception occurs.
    /// </remarks>
    public class StateBusiness
    {
        private readonly StateDao _stateDao = new StateDao();

        public ExecuteResult<List<State>> GetAll()
        {
            // Add additional logic here.
            return _stateDao.GetAll();
        }
    }
}
