using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR_1
{
    internal class GradesList
    {
        static private string ConnectionString = @"Data Source=DESKTOP-GPKQNQK;User id=default;Password=!gbpk0908;Integrated Security=True";

        public void AddAGradeBook()
        {
            Console.WriteLine("Введите код студента: ");
            int StudentID = Console.ReadLine();
            Console.WriteLine("Введите код преподавателя: ");
            int EmployeeID = Console.ReadLine();
            Console.WriteLine("Введите код предмета: ");
            int SubjectID = Console.ReadLine();
            Console.WriteLine("Введите оценку: ");
            string Grade = Console.ReadLine();
            SqlConnection SConnection = new SqlConnection(ConnectionString);
            try
            {
                SConnection.Open();
                string query = $"Insert into GradeBookAndLogin.dbo.GradeLists ({StudentID}, {EmployeeID}, {SubjectID}, '{Grade}')";
                SqlCommand cmd = new SqlCommand(query, SConnection);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Оценка успешно выставлена!");
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void EditAGradeBook()
        {
            SqlConnection SConnection = new SqlConnection(ConnectionString);
            Console.WriteLine("Введите код выставленной оценки: ");
            int GradeListID = Console.ReadLine();
            Console.WriteLine("Выберите действие:\n1 - Удалить\n2 - Изменить");
            int choice = Console.ReadLine();
            switch (choice)
            {
                case 1:
                    try
                    {
                        SConnection.Open();
                        string query = $"Delete from GradeBookAndLogin.dbo.GradeLists where GradeListID = {GradeListID}";
                        SqlCommand cmd = new SqlCommand(query, SConnection);
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Оценка успешно удалена!");
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine(ex);
                    }
                    break;
                case 2:
                    try
                    {
                        Console.WriteLine("Введите код студента: ");
                        int StudentID = Console.ReadLine();
                        Console.WriteLine("Введите код преподавателя: ");
                        int EmployeeID = Console.ReadLine();
                        Console.WriteLine("Введите код предмета: ");
                        int SubjectID = Console.ReadLine();
                        Console.WriteLine("Введите оценку: ");
                        string Grade = Console.ReadLine();
                        SConnection.Open();
                        string query = $"Update GradeBookAndLogin.dbo.GradeLists set ({StudentID}, {EmployeeID}, {SubjectID}, '{Grade}') where GradeListID = {GradeListID}";
                        SqlCommand cmd = new SqlCommand(query, SConnection);
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Оценка успешно изменена!");
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine(ex);
                    }
                    break;
                default:
                    return;
            }
            
        }

        public void SeeGroupMembers()
        {
            SqlConnection SConnection = new SqlConnection(ConnectionString);
            Console.WriteLine("Введите код группы: ")
            int GroupID = Int32.Parse(Console.ReadLine());
            try
            {
                SConnection.Open();
                string query = $"Select * from GradeBookAndLogin.dbo.Students where GroupID = {GroupID}";
                SqlCommand cmd = new SqlCommand(query, SConnection);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"StudentID: {reader.GetValue(0)}, StudentFirstName: {reader.GetValue(1)}, StudentLastName: {reader.GetValue(2)}, StudentPatronymic: {reader.GetValue(3)}" +
                            $", StudentSex: {reader.GetValue(4)}, StudentBrithDate: {reader.GetValue(5)}, StudentHomeAddress: {reader.GetValue(6)}, StudentPhoneNumber: {reader.GetValue(7)}, GroupID: {reader.GetValue(8)}");
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void SeeStudentAverageGradeOverall()
        {
            SqlConnection SConnection = new SqlConnection(ConnectionString);
            Console.WriteLine("Введите код студента: ")
            int StudentID = Int32.Parse(Console.ReadLine());
            try
            {
                SConnection.Open();
                string query = $"Select Grade from GradeBookAndLogin.dbo.GradeLists where StudentID = {StudentID}";
                SqlCommand cmd = new SqlCommand(query, SConnection);
                int AvgGrade;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    int counter = 0;
                    int AllGrades = 0;
                    while (reader.Read())
                    {
                        AllGrades = Int32.Parse(reader.GetValue(0));
                        counter++;
                    }
                    AvgGrade = AllGrades / counter;
                }
                Console.WriteLine("Средний балл этого студента: " +  AvgGrade);
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void SeeStudentsGrades()
        {
            SqlConnection SConnection = new SqlConnection(ConnectionString);
            Console.WriteLine("Введите код студента: ")
            int StudentID = Int32.Parse(Console.ReadLine());
            try
            {
                SConnection.Open();
                string query = $"Select * from GradeBookAndLogin.dbo.GradeLists where StudentID = {StudentID}";
                SqlCommand cmd = new SqlCommand(query, SConnection);
                int AvgGrade;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"GradeListID: {reader.GetValue(0)}, StudentID: {reader.GetValue(1)}, EmployeeID: {reader.GetValue(2)}, SubjectID: {reader.GetValue(3)}, Grade: {reader.GetValue(4)}");
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
