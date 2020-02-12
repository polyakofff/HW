using System;
using System.Collections.Generic;
using System.Text;

namespace HW {
    class Calculator {

        public event ErrorNotificationType ErrorNotification;

        delegate double MathOperation(double a, double b);
        private static Dictionary<string, MathOperation> operations = new Dictionary<string, MathOperation>{
            {"+", (a, b) => a + b },
            {"-", (a, b) => a - b },
            {"*", (a, b) => a * b },
            {"/", (a, b) => a / b },
            {"^", Math.Pow }
        };

        public double Calculate(string exp) {
            string[] ss = exp.Split();

            // Перевод в обратную польскую запись
            List<string> rpn = new List<string>();
            Stack<string> st1 = new Stack<string>();
            bool IsOp(string s) => (s.Length == 1 && !(48 <= s[0] && s[0] <= 57)) ? true : false;
            int Prior(string op) => (op == "+" || op == "-") ? 1 : (op == "*" || op == "/") ? 2 : (op == "^") ? 3 : 4;
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

            try {
                // Вычисление
                Stack<double> st2 = new Stack<double>();
                foreach (string s in rpn) {
                    if (IsOp(s)) {
                        double b = st2.Pop();
                        double a = st2.Pop();
                        if (s == "/" && b == 0) {
                            throw new DivideByZeroException();
                        }
                        st2.Push(operations[s](a, b));
                    } else {
                        st2.Push(double.Parse(s));
                    }
                }
                double res = st2.Pop();

                if (double.IsNaN(res)) {
                    ErrorNotification("не число");
                }
                return res;

            } catch (KeyNotFoundException) {
                ErrorNotification("неверный оператор");
            } catch (DivideByZeroException) {
                ErrorNotification("bruh");
            } catch (Exception) {
                ErrorNotification("ошибка");
            }

            return double.NaN;
        }
    }
}
