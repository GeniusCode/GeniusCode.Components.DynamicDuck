using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gcDynamicTest
{
    
    public class Man
    {
        public Man()
        {
            SecretWord = "Brodie";
        }
        public string Name { get; set; }
        public int Age { get; set; }
        private string SecretWord { get; set; }


       public string  SayHello(string message)
        {
            return "Hi";
        }

        public int SayHello(int message)
       {
           return 5;
       }

        public int SayHello2(int message)
        {
            return 52;
        }

    }


    public interface IPerson
    {
        string SecretWord { get; set; }
        string Name { get; set; }
        int Age { get; set; }
        //void IncrementAge();
        //event EventHandler HadBirthday;

        int SayHello(int message);
        int SayHello2(int message);
        string SayHello(string message);
    }

    public interface IEmployee : IPerson
    {
        string Title { get; set; }
        int Salaray { get; set; }
    }
}
