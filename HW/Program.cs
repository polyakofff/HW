using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace HW {
    delegate void ErrorNotificationType(string message);

    class Program {

        private static string error;

        private static List<string> ReadLines(string file) {
            List<string> lines = new List<string>();
            try {
                using (StreamReader sr = new StreamReader(file, Encoding.UTF8)) {
                    string line = "";
                    while ((line = sr.ReadLine()) != null) {
                        line = Regex.Replace(line, @"\u00A0", " ");
                        lines.Add(line);
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }

            return lines;
        }

        private static void WriteLines(List<string> lines, string file) {
            try {
                using (StreamWriter sw = new StreamWriter(file, false)) {
                    lines.ForEach(line => sw.WriteLine(line));
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        private static void ConsoleErrorHandler(string message) {
            string time = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
            Console.WriteLine(message + "\n" + time + "\n");
        }

        private static void ResultErrorHandler(string message) {
            switch (message) {
                case "не число":
                    error = "не число";
                    break;
                case "неверный оператор":
                    error = "неверный оператор";
                    break;
                case "bruh":
                    error = "bruh";
                    break;
            }
        }

        static void Main(string[] args) {
            Console.OutputEncoding = Encoding.UTF8;

            Calculator calculator = new Calculator();
            calculator.ErrorNotification += ConsoleErrorHandler;
            calculator.ErrorNotification += ResultErrorHandler;

            List<string> exps = ReadLines("expressions.txt");
            List<string> myAnswers = new List<string>();
            for (int i = 0; i < exps.Count; i++) {
                double res = calculator.Calculate(exps[i]);
                if (double.IsNaN(res)) {
                    myAnswers.Add(error);
                } else {
                    myAnswers.Add(res.ToString("#0.000"));
                }
            }

            WriteLines(myAnswers, "myAnswers.txt");

            List<string> answers = ReadLines("expressions_checker.txt");

            List<string> differences = new List<string>();
            int numOfErrors = 0;
            for (int i = 0; i < myAnswers.Count; i++) {
                if (myAnswers[i].Equals(answers[i])) {
                    differences.Add("OK");
                } else {
                    differences.Add("Error");
                    numOfErrors++;
                }
            }
            differences.Add(numOfErrors.ToString());

            WriteLines(differences, "differences.txt");


            Console.WriteLine(myAnswers[135].Equals(answers[135]));


            Console.ReadKey();
        }
    }
}
