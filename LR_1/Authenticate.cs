using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace LR_1
{
    class User
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }



    class UserRegistration
    {
        //("lr1mailmessages@gmail.com", "llfp xaex ylgk ccgq")
        private string SenderMail, SenderAppPassword;

        public void SetSenderDetails(string  Email, string Password)
        {
            this.SenderMail = Email;
            this.SenderAppPassword = Password;
        }

        private List<User> users = new List<User>();
        public bool RegisterUser(string email, string password)
        {
            // Проверка корректности электронной почты и пароля
            if (IsValidEmail(email) && IsStrongPassword(password))
            {
                string RestorationCode = CodeMailSender(email);
                Console.Write("Введите шестизначный код из письма: ");
                string CompareCodes = Console.ReadLine();
                if (CompareCodes != RestorationCode)
                {
                    Console.WriteLine("Неверный код!");
                    return false;
                }
                users.Add(new User { Email = email, Password = password });
                Console.WriteLine("Пользователь успешно зарегистрирован");
                return true;
            }
            else
            {
                Console.WriteLine("Ошибка регистрации пользователя");
                return false;
            }
            
        }
        private bool IsValidEmail(string email)
        {
            // Простейшая проверка формата email
            try
            {
                MailAddress m = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private bool IsStrongPassword(string Password)
        {
            // Проверка надежности пароля (просто для примера - обычно используются более сложные методы)
            string Pattern = @"(?=.*[A-Za-z_!])(?=.*\d)";
            bool isValid = (Password.Length >= 8) && Regex.IsMatch(Password, Pattern);
            return isValid;
        }

        private bool IsUser(string email)
        {
            foreach (var user in users)
            {
                if (user.Email == email)
                {
                    return true;
                }
            }
            return false;
        }

        public bool LoginUser(string email, string password)
        {
            if (!IsUser(email))
            {
                Console.WriteLine("Неверный адрес электронной почты или пароль");
                return false;
            }
            else
            {
                foreach (var user in users)
                {
                    if (user.Email == email && user.Password == password)
                    {
                        Console.WriteLine("Вход выполнен успешно");
                        return true;
                    }
                }
                Console.WriteLine("Неверный адрес электронной почты или пароль");
                return false;
            }
                
        }

        private string CodeMailSender(string UserEmail)
        {
            Random RandGen = new Random();
            string RestorationCode = RandGen.Next(99999, 1000000).ToString();

            MailAddress from = new MailAddress("lr1mailmessages@gmail.com", "LR1_Registration");
            MailAddress to = new MailAddress(UserEmail);
            MailMessage RestorationMessage = new MailMessage(from, to);
            RestorationMessage.Subject = "Восстановление пароля.";
            RestorationMessage.Body = $"<h3>Здравствуйте! Ваш код </h3>\n<h1> {RestorationCode} </h1>";
            RestorationMessage.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(SenderMail, SenderAppPassword);
            smtp.EnableSsl = true;
            smtp.Send(RestorationMessage);
            return RestorationCode;
        }

        public void PasswordRestoration()
        {
            Console.Write("Введите почту, использованную при регистрации: ");
            string UserEmail = Console.ReadLine();
            if (!IsValidEmail(UserEmail) || !IsUser(UserEmail))
            {
                Console.Write("Адрес электронной почты некорректен.");
            }

            string RestorationCode = CodeMailSender(UserEmail);

            Console.Write("Введите шестизначный код, отправленный Вам на почту: ");
            string CompareCodes = Console.ReadLine();
            if (CompareCodes == RestorationCode)
            {
                Console.Write("Введите новый пароль: ");
                string NewPassword = Console.ReadLine();
                foreach (var user in users)
                {
                    if (user.Email == UserEmail)
                    {
                        user.Password = NewPassword;
                    }
                }
            }
        }
    }



    class Authenticate
    {
        static void Main()
        {
            UserRegistration userRegistration = new UserRegistration();

            while (true)
            {
                Console.WriteLine("Выберите действие:\n1 - Регистрация\n2 - Вход\n3 - Восстановить \n0 - Выход");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Введите адрес электронной почты:");
                        string regEmail = Console.ReadLine();

                        Console.WriteLine("Введите пароль:");
                        string regPassword = Console.ReadLine();

                        userRegistration.RegisterUser(regEmail, regPassword);
                        break;

                    case "2":
                        Console.WriteLine("Введите адрес электронной почты:");
                        string loginEmail = Console.ReadLine();

                        Console.WriteLine("Введите пароль:");
                        string loginPassword = Console.ReadLine();

                        userRegistration.LoginUser(loginEmail, loginPassword);
                        break;

                    case "3":
                        userRegistration.PasswordRestoration();
                        break;
                    case "0":
                        return;

                    default:
                        Console.WriteLine("Некорректный ввод. Попробуйте снова.");
                        break;
                }
            }
        }
    }

}
