using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FizzBuzz;

namespace FizzBuzz
{
    public class FizzBuzzApplication
    {
        /// <summary>
        /// The start of the program
        /// </summary>
        public static void Main()
        {
            // Helper lambda in order to make tuples
            Func<int,string,Tuple<int,string>> makeTuple = (a, b) => new Tuple<int, string>(a, b);

            // Our standard FizzBuzz mapping:
            //   if a number is divisible by 3, print "Fizz"
            //   if a number is divisible by 5, print "Buzz"
            var mapping = new List<Tuple<int, string>> {
				makeTuple(3, "Fizz"),
				makeTuple(5, "Buzz")
				//, makeTuple(7, "Baz")
			};

            // In C# 6 the following should work, but in VS2013 it does not
            // This uses the extension method Add<T1,T2> below
            //var mapping = new List<Tuple<int, string>> {
            //    {3, "Fizz"},
            //    {5, "Buzz"}
            //    //,{7, "Baz"}
            //};

            

            // Construct the enumerator
            var fizzBuzz = new FizzBuzzEnumerator(100, mapping);

            // Print the output to the console
            foreach (string fizzBuzzOutput in fizzBuzz)
                Console.WriteLine(fizzBuzzOutput);

            Console.Read();
        }
    }

    /// <summary>
    /// Class to hold extension methods for Lists of Tuples.  This allows us to easily
    /// construct lists of tuples.
    /// </summary>
    public static class TupleExtension
    {
        public static void Add<T1, T2>(this IList<Tuple<T1, T2>> list, T1 Item1, T2 Item2)
        {
            list.Add(new Tuple<T1, T2>(Item1, Item2));
        }

    }

    /// <summary>
    /// Given a length and a mapping this class will enumerate the FizzBuzz strings
    /// </summary>
    public class FizzBuzzEnumerator : IEnumerable<string>
    {
        /// <summary>
        /// Gets or sets the length of the enumeration
        /// </summary>
        /// <value>The length</value>
        public int Length { get; set; }

        /// <summary>
        /// Gets or sets the FizzBuzzMappings mappings
        /// </summary>
        /// <value>The mappings</value>

        // Note: this could be implemented with a custom class, but using a list
        // of tuples suffices in this case.
        public List<Tuple<int, string>> Mappings { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="FizzBuzz.FizzBuzzEnumerator"/> class
        /// </summary>
        /// <param name="length">Length of the enumeration to generate</param>
        /// <param name="fizzBuzzMappings">Fizz buzz mappings</param>
        public FizzBuzzEnumerator(int length, List<Tuple<int, string>> fizzBuzzMappings)
        {
            Length = length;
            Mappings = fizzBuzzMappings;
        }

        /// <summary>
        /// Return the enumeration of the output of FizzBuzz
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<string> GetEnumerator()
        {
            for (int currentNumber = 1; currentNumber <= Length; currentNumber++)
            {
                bool printNumber = true;
                // We use a string builder since the number of concatenations is not
                // known ahead of time, and string concatenations can be slow.
                var outputString = new StringBuilder();

                // Loop through all the mappings, in order, and see if the currentNumber
                // is divisible by any of them.  If so, append the associated string to 
                // the output.
                foreach (var map in Mappings)
                {
                    if (currentNumber % map.Item1 == 0)
                    {
                        outputString.Append(map.Item2);
                        printNumber = false;
                    }
                }

                // No string to output, so instead we output the number
                if (printNumber)
                    outputString.Append(currentNumber.ToString());
                yield return outputString.ToString();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
