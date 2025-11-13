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
    class UserRegistration
    {
        //;Integrated Security=True
        private string ConnectionString;
        public UserRegistration(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }
        //("lr1mailmessages@gmail.com", "llfp xaex ylgk ccgq")


        //these two should be sent to DB for security AND for it to save regardless of programm's state. 
        private string SenderMail = "lr1mailmessages@gmail.com";
        private string SenderAppPassword = "llfp xaex ylgk ccgq";

        private void AddUser(string UserLogin, string UserPassword, int RoleID)
        {
            SqlConnection SConnection = new SqlConnection(ConnectionString);
            try
            {
                SConnection.Open();
                string SqlQuery = $"INSERT INTO GradeBookAndLogin.dbo.Users (UserLogin, UserPassword, RoleID) values ('{UserLogin}', HashBytes('SHA1', '{UserPassword}'), {RoleID})";
                SqlCommand cmd = new SqlCommand(SqlQuery, SConnection);
                int rowsAffected = cmd.ExecuteNonQuery();
                Console.WriteLine("Любое поле можно оставить пустым.");
                Console.Write("Имя: ");
                string FirstName = Console.ReadLine();
                Console.Write("Фамилия: ");
                string LastName = Console.ReadLine();
                Console.Write("Отчество: ");
                string Patronymic = Console.ReadLine();
                Console.Write("Пол: ");
                string Sex = Console.ReadLine();
                Console.Write("Дата рождения (гггг-мм-дд): ");
                string DateOfBirth = Console.ReadLine();
                Console.Write("Адрес проживания: ");
                string HomeAddress = Console.ReadLine();
                Console.Write("Номер телефона: ");
                string PhoneNumber = Console.ReadLine();
                switch (RoleID)
                {
                    case 1:
                        cmd = new SqlCommand($"INSERT INTO GradeBookAndLogin.dbo.Employees values ('{FirstName}', '{LastName}', '{Patronymic}', '{Sex}', '{DateOfBirth}', '{HomeAddress}', '{PhoneNumber}', (select UserID from GradeBookAndLogin.dbo.Users where UserLogin = '{UserLogin}'))", SConnection);
                        cmd.ExecuteNonQuery();
                        break;
                    case 2:
                        cmd = new SqlCommand($"INSERT INTO GradeBookAndLogin.dbo.Employees values ('{FirstName}', '{LastName}', '{Patronymic}', '{Sex}', '{DateOfBirth}', '{HomeAddress}', '{PhoneNumber}', (select UserID from GradeBookAndLogin.dbo.Users where UserLogin = '{UserLogin}'))", SConnection);
                        cmd.ExecuteNonQuery();
                        break;
                    case 3:
                        cmd = new SqlCommand($"INSERT INTO GradeBookAndLogin.dbo.Students values ('{FirstName}', '{LastName}', '{Patronymic}', '{Sex}', '{DateOfBirth}', '{HomeAddress}', '{PhoneNumber}', 1, (select UserID from GradeBookAndLogin.dbo.Users where UserLogin = '{UserLogin}'))", SConnection);
                        cmd.ExecuteNonQuery();
                        break;
                }
                //Console.WriteLine($"{rowsAffected} row(s) inserted successfully.");
                SConnection.Close();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void SetSenderDetails(string Email, string Password)
        {
            this.SenderMail = Email;
            this.SenderAppPassword = Password;
        }

        public int RegisterUser()
        {
            Console.WriteLine("Введите адрес электронной почты:");
            string UserEmail = Console.ReadLine();

            if (IsUser(UserEmail))
            {
                Console.WriteLine("Данный пользователь уже существует.");
                return 0;
            }
            Console.WriteLine("Введите пароль:");
            string UserPassword = Console.ReadLine();
            // Проверка корректности zэлектронной почты и пароля
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
                Console.WriteLine("Укажите роль:\n1 - Педагог\n2 - Студент");
                int UserRole = Int32.Parse(Console.ReadLine()) + 1;
                AddUser(UserEmail, UserPassword, UserRole);
                Console.WriteLine("Пользователь успешно зарегистрирован");
                return UserRole;
            }
            else
            {
                Console.WriteLine("Неверный адрес электронной почты или пароль.");
                return 0;
            }

        }
        private bool IsValidEmail(string Email)
        {
            // Простейшая проверка формата email
            try
            {
                MailAddress m = new MailAddress(Email);
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
                int ScalarCount = (int)cmd.ExecuteScalar();
                if (ScalarCount == 0)
                {
                    return false;
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
                Console.WriteLine("Неверный адрес электронной почты или пароль.");
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
            RestorationMessage.Subject = "Восстановление.";
            RestorationMessage.Body = $"<h3>Здравствуйте! Ваш код </h3>\n<h1> {RestorationCode} </h1>";
            RestorationMessage.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(SenderMail, SenderAppPassword);
            smtp.EnableSsl = true;
            smtp.Send(RestorationMessage);
            return RestorationCode;
        }

        public void PersonalInfoEdit(int RoleID)
        {

            SqlConnection SConnection = new SqlConnection(ConnectionString);
            try
            {
                int UserID = 0;
                SConnection.Open();
                Console.WriteLine("Для дополнительного подтвеждения введите логин: ");
                string UserLogin = Console.ReadLine();
                string SqlQuery = $"select UserID from GradeBookAndLogin.dbo.Users where UserLogin = '{UserLogin}'";
                SqlCommand cmd = new SqlCommand(SqlQuery, SConnection);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        UserID = reader.GetInt32(0);
                    }
                }
                string RoleName = "Employee";
                SqlQuery = $"select RoleName from GradeBookAndLogin.dbo.Roles where RoleID = {RoleID}";
                cmd = new SqlCommand(SqlQuery, SConnection);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        RoleName = reader.GetString(0);
                    }
                }
                while (true)
                {
                    SqlQuery = $"select {RoleName}FirstName, {RoleName}LastName, {RoleName}Patronymic, {RoleName}Sex, {RoleName}BirthDate, {RoleName}HomeAddress from GradeBookAndLogin.dbo.{RoleName}s where UserID = {UserID}";
                    cmd = new SqlCommand(SqlQuery, SConnection);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Console.WriteLine($"Имя: {reader.GetValue(0)}\nФамилия: {reader.GetValue(1)}\nОтчество: {reader.GetValue(2)}\nПол: {reader.GetValue(3)}\nДата рождения (гггг-мм-дд): {reader.GetValue(4)}\nАдрес проживания: {reader.GetValue(5)}");
                        }
                    }
                    Console.WriteLine("Выберите пункт для изменения:\n1 - Имя\n2 - Фамилия\n3 - Отчество\n4 - Пол\n5 - Дата рождения\n6 - Домашний адрес\n0 - Назад\n\n");
                    string choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "0":
                            return;
                        case "1":
                            Console.WriteLine("Введите новое имя: ");
                            string NewName = Console.ReadLine();
                            SqlQuery = $"update GradeBookAndLogin.dbo.{RoleName}s set {RoleName}FirstName = '{NewName}' where UserID = '{UserID}'";
                            cmd = new SqlCommand(SqlQuery, SConnection);
                            cmd.ExecuteNonQuery();
                            break;
                        case "2":
                            Console.WriteLine("Введите новую фамиилию: ");
                            string NewLastName = Console.ReadLine();
                            SqlQuery = $"update GradeBookAndLogin.dbo.{RoleName}s set {RoleName}LastName = '{NewLastName}' where UserID = '{UserID}'";
                            cmd = new SqlCommand(SqlQuery, SConnection);
                            cmd.ExecuteNonQuery();
                            break;
                        case "3":
                            Console.WriteLine("Введите новое отчество: ");
                            string NewPatronymic = Console.ReadLine();
                            SqlQuery = $"update GradeBookAndLogin.dbo.{RoleName}s set {RoleName}Patronymic = '{NewPatronymic}' where UserID = '{UserID}'";
                            cmd = new SqlCommand(SqlQuery, SConnection);
                            cmd.ExecuteNonQuery();
                            break;
                        case "4":
                            Console.WriteLine("Введите новыый пол: ");
                            string NewSex = Console.ReadLine();
                            SqlQuery = $"update GradeBookAndLogin.dbo.{RoleName}s set {RoleName}Sex = '{NewSex}' where UserID = '{UserID}'";
                            cmd = new SqlCommand(SqlQuery, SConnection);
                            cmd.ExecuteNonQuery();
                            break;
                        case "5":
                            Console.WriteLine("Введите новую дату рождения: ");
                            string NewBirthDate = Console.ReadLine();
                            SqlQuery = $"update GradeBookAndLogin.dbo.{RoleName}s set {RoleName}DateOfBirth = '{NewBirthDate}' where UserID = '{UserID}'";
                            cmd = new SqlCommand(SqlQuery, SConnection);
                            cmd.ExecuteNonQuery();
                            break;
                        case "6":
                            Console.WriteLine("Введите новый домашний адрес: ");
                            string NewHomeAddress = Console.ReadLine();
                            SqlQuery = $"update GradeBookAndLogin.dbo.{RoleName}s set {RoleName}HomeAddress = '{NewHomeAddress}' where UserID = '{UserID}'";
                            cmd = new SqlCommand(SqlQuery, SConnection);
                            cmd.ExecuteNonQuery();
                            break;
                        default:
                            Console.WriteLine("Некорректный ввод.");
                            break;
                    }
                }

            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void EditBackupEmail()
        {
            SqlConnection SConnection = new SqlConnection(ConnectionString);
            try
            {
                int UserID = 0;
                SConnection.Open();
                Console.WriteLine("Для дополнительного подтвеждения введите логин: ");
                string UserLogin = Console.ReadLine();
                string SqlQuery = $"select UserID from GradeBookAndLogin.dbo.Users where UserLogin = '{UserLogin}'";
                SqlCommand cmd = new SqlCommand(SqlQuery, SConnection);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        UserID = reader.GetInt32(0);
                    }
                }               
                SqlQuery = $"Select UserBackupEmail from GradeBookAndLogin.dbo.Users where UserID = {UserID}";
                cmd = new SqlCommand(SqlQuery, SConnection);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Console.WriteLine("Резервный почтовый адрес: " + reader.GetValue(0));
                    }
                }
                Console.WriteLine("Введите новый резервный почтовый адрес: ");
                string BackupEmail = Console.ReadLine();
                SqlQuery = $"Update GradeBookAndLogin.dbo.Users set UserBackupEmail = '{BackupEmail}' where UserID = {UserID}";
                string RealCode = CodeMailSender(BackupEmail);
                Console.WriteLine("Введите код, отправленный Вам на почту: ");
                string UserInputCode = Console.ReadLine();
                if (RealCode == UserInputCode)
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Успешно!");

                }
                else
                {
                    Console.WriteLine("Неверный код.");
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void AccountSettings(int RoleID)
        {
            while (true)
            {
                Console.WriteLine("Выберите действие:\n1 - Изменить личную информацию.\n2 - Добавить/Изменить резервную почту.\n3 - Сменить пароль.\n0 - Выход.\n\n");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        PersonalInfoEdit(RoleID);
                        break;
                    case "2":
                        EditBackupEmail();
                        break;
                    case "3":
                        PasswordRestoration();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Некорректный ввод. Попробуйте снова.");
                        break;

                }
            }

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
            Console.WriteLine("Enter connectionstring: ");
            string ConnectionString = Console.ReadLine(); //"Data Source=LAB-405-ST-3\\MSLR1REG;User id=default;Password=12345678" 
            UserRegistration userRegistration = new UserRegistration(ConnectionString);

            GradesList gradesList = new GradesList(ConnectionString);

            int RoleID = 0; //pre registration/login user
            string MenuText = "";
            while (true)
            {
                switch (RoleID)
                {
                    case 0:
                        MenuText = "\nВыберите действие:\n1 - Регистрация.\n2 - Вход.\n3 - Восстановить.\n0 - Выход.\n\n";
                        break;
                    case 1:
                        MenuText = "\n\nРоль - Администратор.\nВыберите действие:\n4 - Установка почты рассылки писем.\n5 - Управление данными учетной записи.\n13 - Управление группами\n6 - Выход из учетной записи.\n0 - Выход из программы.\n\n";
                        break;
                    case 2:
                        MenuText = "\n\nРоль - Работник учебного заведения.\nВыберите действие:\n9 - Выставить оценку.\n10 - Изменить существующую оценку.\n11 - Просмотреть списки студентов групп.\n8 - Просмотр списка средних баллов по предметам.\n12 - Просмотреть общий средний балл студента.\n" +
                            "5 - Управление данными учетной записи.\n6 - Выход из учетной записи.\n0 - Выход из программы.\n\n";
                        break;
                    case 3:
                        MenuText = "\n\nРоль - Студент учебного заведения.\nВыберите действие:\n7 - Просмотр списка всех оценок.\n8 - Просмотр списка средних баллов по предметам.\n12 - Просмотреть общий средний балл.\n5 - Управление данными учетной записи.\n6 - Выход из учетной записи.\n0 - Выход из программы.\n\n"; //7 и 8 пункты придут в более поздних версиях.
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
                        userRegistration.AccountSettings(RoleID);
                        if (RoleID == 0)
                        {
                            Console.WriteLine("Некорректный ввод. Попробуйте снова.");
                        }
                        break;
                    case "6":
                        RoleID = 0;
                        break;
                    case "7":
                        if (RoleID != 3 && RoleID != 1)
                        {
                            Console.WriteLine("Некорректный ввод. Попробуйте снова.");
                        }
                        else
                        {
                            gradesList.SeeStudentsGrades();
                        }
                        break;
                    case "8":
                        if (RoleID != 3 && RoleID != 1)
                        {
                            Console.WriteLine("Некорректный ввод. Попробуйте снова.");
                        }
                        else
                        {
                            gradesList.SeeStudentAverageGrade();
                        }
                        break;
                    case "9":
                        if (RoleID != 2 && RoleID != 1)
                        {
                            Console.WriteLine("Некорректный ввод. Попробуйте снова.");
                        }
                        else
                        {
                            gradesList.AddGradeBook();
                        }
                        break;
                    case "10":
                        if (RoleID != 2 && RoleID != 1)
                        {
                            Console.WriteLine("Некорректный ввод. Попробуйте снова.");
                        }
                        else
                        {
                            gradesList.EditGradeBook();
                        }
                        break;
                    case "11":
                        if (RoleID != 2 && RoleID != 1)
                        {
                            Console.WriteLine("Некорректный ввод. Попробуйте снова.");
                        }
                        else
                        {
                            gradesList.SeeGroupMembers();
                        }
                        break;
                    case "12":
                        if (RoleID != 2 && RoleID != 1)
                        {
                            Console.WriteLine("Некорректный ввод. Попробуйте снова.");
                        }
                        else
                        {
                            gradesList.SeeStudentAverageGradeOverall();
                        }
                        break;
                    case "13":
                        if (RoleID != 1)
                        {
                            Console.WriteLine("Некорректный ввод. Попробуйте снова.");
                        }
                        else
                        {
                            gradesList.EditGroups();
                        }
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
