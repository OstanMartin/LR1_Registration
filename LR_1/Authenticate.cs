using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Collections;
using System.Security.Cryptography;


namespace LR_1
{
    class User
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }



    class UserRegistration
    {
        static private string ConnectionString = @"Data Source=DESKTOP-GPKQNQK;User id=default;Password=!gbpk0908;Integrated Security=True";
        //("lr1mailmessages@gmail.com", "llfp xaex ylgk ccgq")
       

        //next two should be sent to DB for security AND for it to save regardless of programm's state. 
        private string SenderMail = "lr1mailmessages@gmail.com";
        private string SenderAppPassword = "llfp xaex ylgk ccgq";
        
        public void SqlConnect()
        {
            SqlConnection SConnection = new SqlConnection(ConnectionString);
            try {
                SConnection.Open();
                Console.WriteLine("Connection successful.");
                /*string SqlQuery = "select * from GradeBookAndLogin.dbo.Users where UserPassword = (HASHBYTES('SHA1', '{UserPasswords}'))";
                SqlCommand cmd = new SqlCommand(SqlQuery, SConnection);
                using (SConnection)
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"UserID: {reader.GetValue(0)}, UserLogin: {reader.GetValue(1)}, UserPassword: {reader.GetValue(2)}, RoleID: {reader.GetValue(3)}" +
                                $", UserBackupEmail: {reader.GetValue(4)}");
                        }
                    }
                }*/
            }
            catch(SqlException ex)
            {
                Console.WriteLine(ex);
            }
        }
        
        private void AddUser(string UserLogin, string UserPassword, int RoleID)
        {
            SqlConnection SConnection = new SqlConnection(ConnectionString);
            try
            {
                SConnection.Open();
                string SqlQuery = $"INSERT INTO GradeBookAndLogin.dbo.Users (UserLogin, UserPassword, RoleID) values ('{UserLogin}', HashBytes('SHA1', '{UserPassword}'), {RoleID})";
                SqlCommand cmd = new SqlCommand(SqlQuery, SConnection);
                int rowsAffected = cmd.ExecuteNonQuery();
                //Console.WriteLine($"{rowsAffected} row(s) inserted successfully.");
                SConnection.Close();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void SetSenderDetails(string  Email, string Password)
        {
            this.SenderMail = Email;
            this.SenderAppPassword = Password;
        }

        private List<User> users = new List<User>();
        public int RegisterUser()
        {
            Console.WriteLine("Введите адрес электронной почты:");
            string UserEmail = Console.ReadLine();

            Console.WriteLine("Введите пароль:");
            string UserPassword = Console.ReadLine();
            // Проверка корректности электронной почты и пароля
            if (IsValidEmail(UserEmail) && IsStrongPassword(UserPassword))
            {
                string RestorationCode = CodeMailSender(UserEmail);
                Console.Write("Введите шестизначный код из письма: ");
                string CompareCodes = Console.ReadLine();
                if (CompareCodes != RestorationCode)
                {
                    Console.WriteLine("Неверный код!");
                    return 0;
                }
                Console.WriteLine("Укажите роль\n 1 - Педагог.\n2 - Студент: ");
                int UserRole = Int32.Parse(Console.ReadLine()) + 1;

                AddUser(UserEmail, UserPassword, UserRole);  
                Console.WriteLine("Пользователь успешно зарегистрирован");
                return UserRole;
            }
            else
            {
                Console.WriteLine("Ошибка регистрации пользователя");
                return 0;
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
            // Проверка надежности пароля (простая)
            string Pattern = @"(?=.*[A-Za-z_!])(?=.*\d)";
            bool isValid = (Password.Length >= 8) && Regex.IsMatch(Password, Pattern);
            return isValid;
        }

        private bool IsUser(string UserLogin)
        {
            SqlConnection SConnection = new SqlConnection(ConnectionString);
            try
            {
                SConnection.Open();
                string query = $"SELECT Count(*) from GradeBookAndLogin.dbo.Users where UserLogin = '{UserLogin}'";
                SqlCommand cmd = new SqlCommand(query, SConnection);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.FieldCount == 0)
                    {
                        return false;
                    }

                }
                return true;
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            
        }

        public int LoginUser()
        {
            string UserLogin, UserPassword;
            Console.WriteLine("Введите адрес электронной почты:");
            UserLogin = Console.ReadLine();
            Console.WriteLine("Введите пароль:");
            UserPassword = Console.ReadLine();

            if (!IsUser(UserLogin))
            {
                Console.WriteLine("Неверный адрес электронной почты или пароль");
                return 0;
            }
            else
            {
                SqlConnection SConnection = new SqlConnection(ConnectionString);
                try
               {
                    SConnection.Open();
                    string SqlQuery = $"select * from GradeBookAndLogin.dbo.Users where UserLogin = '{UserLogin}' and UserPassword = HashBytes('SHA1', '{UserPassword}')";
                    SqlCommand cmd = new SqlCommand(SqlQuery, SConnection);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Console.WriteLine("Вход успешно произведен.");
                            return reader.GetInt32(3);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex);
                }
                Console.WriteLine("Неверный адрес электронной почты или пароль");
                return 0;
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
                SqlConnection SConnection = new SqlConnection(ConnectionString);
                try
                {
                    SConnection.Open();
                    string query = $"Update GradeBookAndLogin.dbo.Users set UserPassword = HASHBYTES('SHA1', '{NewPassword}') where UserLogin = '{UserEmail}' or UserBackupEmail = '{UserEmail}'";
                    SqlCommand cmd = new SqlCommand(query, SConnection);
                    cmd.ExecuteNonQuery();
                    return;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex);
                    return;
                }
            }
        }
    }



    class Authenticate
    {
        static void Main()
        {
            UserRegistration userRegistration = new UserRegistration();

            userRegistration.SqlConnect();

            int RoleID = 0; //pre registration/login user
            string MenuText = "";
            while (true)
            {
                switch (RoleID)
                {
                    case 0:
                        MenuText = "Выберите действие:\n1 - Регистрация.\n2 - Вход.\n3 - Восстановить.\n0 - Выход.";
                        break;
                    case 1:
                        MenuText = "Роль - Администратор.\nВыберите действие:\n4 - Установка почты рассылки писем.\n5 - Управление данными учетной записи.\n6 - Выход из учетной записи.\n0 - Выход из программы."; //5 и 6 пункты придут в более поздних версиях.
                        break;
                    case 2:
                        MenuText = "Роль - Работник учебного заведения.\nВыберите действие:\n9 - Выставить оценку.\n10 - Изменить существующую оценку.\n11 - Просмотреть списки студентов групп.\n12 - Просмотреть средний балл студента.\n" +
                            "5 - Управление данными учетной записи.\n6 - Выход из учетной записи.\n0 - Выход из программы."; //5 и 6 пункты придут в более поздних версиях.
                        break;
                    case 3:
                        MenuText = "Роль - Студент учебного заведения.\nВыберите действие:\n7 - Просмотр списка всех оценок.\n8 - Просмотр списка средних баллов по предметам.\n5 - Управление данными учетной записи.\n6 - Выход из учетной записи.\n0 - Выход из программы."; //7 и 8 пункты придут в более поздних версиях.
                        break;
                }

                    
                Console.WriteLine(MenuText);
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RoleID = userRegistration.RegisterUser();
                        break;

                    case "2":
                        RoleID = userRegistration.LoginUser();
                        break;

                    case "3":
                        userRegistration.PasswordRestoration();
                        break;
                    case "4":
                        //userRegistration.AddUser("loginEmai", "loginy");
                        //userRegistration.IsUser("{UserLogin}");
                        Console.WriteLine("Enter new sender email: ");
                        string NewSenderEmail = Console.ReadLine();
                        Console.WriteLine("Enter new sender password: ");
                        string NewSenderPassword = Console.ReadLine();
                        userRegistration.SetSenderDetails(NewSenderEmail, NewSenderPassword);
                        break;
                    case "5":

                        break;
                    case "6":
                        RoleID = 0;
                        break;
                    case "7":

                        break;
                    case "8":

                        break;
                    case "9":

                        break;
                    case "10":

                        break;
                    case "11":

                        break;
                    case "12":

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
