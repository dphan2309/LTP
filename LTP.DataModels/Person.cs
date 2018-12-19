using System;

namespace LTP.DataModels
{
    public class Person
    {
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int StateId { get; set; }
        public char Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}