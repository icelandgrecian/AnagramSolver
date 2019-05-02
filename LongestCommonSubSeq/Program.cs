using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace LongestCommonSubSeq
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Search for a word?");

            string value = Console.ReadLine();


            string connetionString = "Data Source=uranus;Initial Catalog=InfomentorOpenHackSQL;User ID=open_hack;Password=open_hack";
            using (SqlConnection cnn = new SqlConnection(connetionString))
            {
                cnn.Open();

                while (!string.IsNullOrWhiteSpace(value))
                {
                    var allWordPossibilities = GetSubSequences(value, value.Length);

                    StringBuilder sql = new StringBuilder("select word from words_alpha where SortedChars in (");

                    foreach (string possibility in allWordPossibilities.Values.Distinct())
                    {
                        sql.Append("'");
                        sql.Append(possibility);
                        sql.Append("',");
                    }

                    sql.Remove(sql.Length - 1, 1);
                    sql.Append(")");

                    using (SqlCommand sqlCommand = new SqlCommand(sql.ToString(), cnn))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand))
                        {
                            DataSet ds = new DataSet();

                            adapter.Fill(ds);

                            List<string> matches = new List<string>();

                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                matches.Add(dr[0].ToString());
                            }

                            Console.WriteLine("Matches");
                            foreach (string match in matches.Distinct().OrderByDescending(f => f.Length).Take(10))
                            {

                                Console.WriteLine(match + " (" + match.Length.ToString() + ")");
                            }

                            Console.WriteLine("Search for another word?");
                            value = Console.ReadLine();
                        }
            
                    }

                }

            }

            Console.WriteLine("Press any key");
            Console.ReadKey();
        }

        public static double GetTotalPowerOf(int power, int yValue)
        {
            return yValue == 0 ? 1 : Math.Pow(2, yValue) + GetTotalPowerOf(2, yValue - 1);
        }

        public static string GetLongestSubSequence(string str1, string str2)
        {
            string result = "";

            // "ABAZDC", "BACBAD" > "ABAD"
            // for each letter

            int maxLength = str1.Length;
            if (str2.Length < maxLength)
            {
                maxLength = str2.Length;
            }
            IDictionary<string, string> str1Ht = GetSubSequences(str1, maxLength);
            IDictionary<string, string> str2Ht = GetSubSequences(str2, maxLength);

            IEnumerable<string> matchingKeys = str1Ht.Keys.Intersect(str2Ht.Keys);

            return matchingKeys.OrderByDescending(f => f.Length).FirstOrDefault();
        }

        public static IDictionary<string, string> GetSubSequences(string str, int maxLength)
        {
            char[] str1Array = str.ToCharArray();

            // use binary logic to figure out the letters to include

            // work out the maximum power so if there are 5 letters there 1+2+4+8+16=31
            // 
            double maxLengthBinary = GetTotalPowerOf(2, str1Array.Length);
         
            IDictionary<string, string> hashTable = new Dictionary<string, string>();

            // loop through all the possible numbers from 1 to the maximum power
            // to figuer out all the letter combinations possible
            for (int position = 1; position <= maxLengthBinary; position++) {
                StringBuilder seq = new StringBuilder();

                for (int charPosition = 0; charPosition < str1Array.Length; charPosition++) {

                    // use binary logic to determine if we use the
                    // current letter on this round
                  
                    if ((position & (int)Math.Pow(2, charPosition)) != 0) {
                        seq.Append(str1Array[charPosition]);
                    }
                }

                if (seq.Length > 1 && !hashTable.ContainsKey(seq.ToString()) && seq.Length <= maxLength)
                {
                    // add the letter combinations and the letters in order
                    hashTable.Add(seq.ToString(), new string(seq.ToString().ToCharArray().OrderBy(f => f).ToArray()));
                }
            }

            return hashTable;
        }
    }
}
