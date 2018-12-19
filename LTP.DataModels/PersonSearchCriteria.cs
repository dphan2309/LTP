using System;

namespace LTP.DataModels
{
    public class PersonSearchCriteria
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int StateId { get; set; }
        public char Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        
        public int pageIndex { get; set; }

        public int pageSize { get; set; }
        public PersonSearchCriteria()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            // 'A' = All, 'F' = Female, 'M' = Male.
            StateId = 0;
            Gender = 'A';
            // Min Sql DateTime
            DateOfBirth = new DateTime(1753, 1, 1);
            pageIndex = 1;
            pageSize = 10;
        }
    }
}