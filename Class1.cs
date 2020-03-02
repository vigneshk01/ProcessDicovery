using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace WinApplWatcher
{

    public struct ValuePair
    {
        public string Value1;
        public string Value2;
    }

    public class class1
    {
        private static int i;
        private static void Main2()
        {
            /*Dictionary<string, Tuple<double, double>> applhashdict = new Dictionary<string, Tuple<double, double>>();
            applhashdict.Add("1", new Tuple<double, double>(0,0));
            applhashdict.Add("2", new Tuple<double, double>(22,22));
            //applhashdict.Add("2", new Tuple<double, double>(22,22));
            /*foreach (var item in applhashdict)
            {
                Debug.WriteLine(item.Value.Item1);
            }
            int i = 100;
            //Debug.WriteLine(i);
             i = 200;
            //Debug.WriteLine(i);
            IDictionaryEnumerator eb = applhashdict.GetEnumerator();
            while (eb.MoveNext())
            {
                string user = eb.Value.ToString().Split(',')[0];
                //Debug.WriteLine(user);
                //Debug.WriteLine(applhashdict[eb.Key.ToString()].Item1);
                double prevseconds = Convert.ToDouble(applhashdict[eb.Key.ToString()].Item1.ToString());
               // Debug.WriteLine(prevseconds);
            }*/

            Dictionary<Tuple<int, string>, Tuple<double, string, string>> applhashdict2 = new Dictionary<Tuple<int, string>, Tuple<double, string, string>>();
            applhashdict2.Add(new Tuple<int, string>(1,"2"), new Tuple<double, string, string>(0,"0","0"));
            applhashdict2.Add(new Tuple<int, string>(2,"t"), new Tuple<double, string, string>(22,"22","22"));
            IDictionaryEnumerator eb = applhashdict2.GetEnumerator();
            while (eb.MoveNext())
            {
                string d = applhashdict2[new Tuple<int, string>(int.Parse(eb.Key.ToString().Remove(0, 1).Split(',')[0].Trim()),eb.Key.ToString().Remove(eb.Key.ToString().Length-1, 1).Split(',')[1].Trim())].Item1.ToString().Trim();
                //string f = eb.Key.ToString().Remove(eb.Key.ToString().Length-1, 1).Split(',')[1].Trim();
            }


                /*Dictionary<int, List<ValuePair>> dictionary = new Dictionary<int, List<ValuePair>>();
                List<ValuePair> list = new List<ValuePair>();
                list.Add(new ValuePair { Value1 = "1", Value2 = "2" });
                dictionary.Add(1, list);
                dictionary.Add(2, list);


                #pragma warning disable CS1001 // Identifier expected
                //Debug.WriteLine(string.Join(", ", dictionary.Select(pair => $"{pair.Key} => {pair.Value}")));
                #pragma warning restore CS1001 // Identifier expected
                */
            }
    }
}