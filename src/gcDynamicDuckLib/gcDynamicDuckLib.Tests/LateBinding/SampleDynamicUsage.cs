using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gcDynamicTest;

namespace GcDynamic.Tests
{
    public class SampleDynamicUsage
    {
        public static void Sample()
        {
            dynamic p = new Man();
            p.Age = 5;


            var q = p.Age;
            p.Age = q;
        }


        public static void SayHello(object input)
        {
            dynamic p = new Man();

            p.SayHello("Hello World");


            var q = p.SayHello(input);

        }

    }
}
