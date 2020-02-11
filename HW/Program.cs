using System;
using System.Collections.Generic;
using System.IO;

namespace HW {
    class Program {

        delegate double MathOperation(double a, double b);
        static Dictionary<string, MathOperation> operations = new Dictionary<string, MathOperation>{
            {"+", (a, b) => a + b },
            {"-", (a, b) => a - b },
            {"*", (a, b) => a * b },
            {"/", (a, b) => a / b },
            {"^", Math.Pow }
        };

        static void Main(string[] args) {
            Console.WriteLine(Calculate("16,3625 ^ 12,95208"));

            List<string> answers = new List<string>();
            try {
                using (StreamReader sr = new StreamReader("expressions.txt")) {
                    string line = "";
                    while ((line = sr.ReadLine()) != null) {
                        double res = Calculate(line);
                        answers.Add(res.ToString("#0.000"));
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
            try {
                using (StreamWriter sw = new StreamWriter("answers.txt", false)) {
                    foreach (string s in answers) {
                        sw.WriteLine(s);
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }


            List<string> results = new List<string>();
            int cntErr = 0;
            try {
                using (StreamReader sr = new StreamReader("expressions_checker.txt")) {
                    int i = 0, errCnt = 0;
                    string line = "";
                    while ((line = sr.ReadLine()) != null) {
                        if (answers[i] == line) {
                            results.Add("OK");
                        } else {
                            results.Add("Error");
                            cntErr++;
                        }
                        i++;
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
            try {
                using (StreamWriter sw = new StreamWriter("results.txt", false)) {
                    foreach (string s in results) {
                        sw.WriteLine(s);
                    }
                    sw.WriteLine(cntErr);
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }


            Console.ReadKey();
        }

        private static double Calculate(string exp) {
            string[] ss = exp.Split();

            // Перевод в обратную польскую запись
            List<string> rpn = new List<string>();
            Stack<string> st1 = new Stack<string>();
            bool IsOp(string s) => (s == "+" || s == "-" || s == "*" || s == "/" || s == "^") ? true : false;
            int Prior(string op) => (op == "+" || op == "-") ? 1 : (op == "*" || op == "/") ? 2 : (op == "^") ? 3 : 0;
            foreach (string s in ss) {
                if (s == "(") {
                    st1.Push(s);
                } else if (s == ")") {
                    while (st1.Peek() != "(") {
                        rpn.Add(st1.Pop());
                    }
                    st1.Pop();
                } else if (IsOp(s)) {
                    while (st1.Count > 0 && Prior(st1.Peek()) >= Prior(s)) {
                        rpn.Add(st1.Pop());
                    }
                    st1.Push(s);
                } else {
                    rpn.Add(s);
                }
            }
            while (st1.Count > 0) {
                rpn.Add(st1.Pop());
            }

            // Вычисление
            Stack<double> st2 = new Stack<double>();
            foreach (string s in rpn) {
                if (IsOp(s)) {
                    double b = st2.Pop();
                    double a = st2.Pop();
                    st2.Push(operations[s](a, b));
                } else {
                    st2.Push(double.Parse(s));
                }
            }
            double res = st2.Pop();

            return res;
        }
    }
}
