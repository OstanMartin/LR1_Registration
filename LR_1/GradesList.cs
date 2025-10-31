using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace LR_1
{
    internal class GradesList
    {
        private string ConnectionString;

        public GradesList(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }

        public void AddGradeBook()
        {
            Console.WriteLine("Введите код студента: ");
            int StudentID = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Введите код преподавателя: ");
            int EmployeeID = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Введите код предмета: ");
            int SubjectID = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Введите оценку: ");
            string Grade = Console.ReadLine();
            SqlConnection SConnection = new SqlConnection(ConnectionString);
            try
            {
                SConnection.Open();
                string query = $"Insert into GradeBookAndLogin.dbo.GradeLists values ({StudentID}, {EmployeeID}, {SubjectID}, '{Grade}')";
                SqlCommand cmd = new SqlCommand(query, SConnection);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Оценка успешно выставлена!");
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void EditGradeBook()
        {
            SqlConnection SConnection = new SqlConnection(ConnectionString);
            Console.WriteLine("Введите код выставленной оценки: ");
            int GradeListID = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Выберите действие:\n1 - Удалить\n2 - Изменить\n3 - Вернуться");
            int choice = Int32.Parse(Console.ReadLine());
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
                        /*Console.WriteLine("Введите код студента: ");
                        int StudentID = Int32.Parse(Console.ReadLine());
                        Console.WriteLine("Введите код преподавателя: ");
                        int EmployeeID = Int32.Parse(Console.ReadLine());
                        Console.WriteLine("Введите код предмета: ");
                        int SubjectID = Int32.Parse(Console.ReadLine());*/
                        Console.WriteLine("Введите оценку: ");
                        string Grade = Console.ReadLine();
                        SConnection.Open();
                        string query = $"Update GradeBookAndLogin.dbo.GradeLists set Grade = '{Grade}' where GradeListID = {GradeListID}";
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
            Console.WriteLine("Введите код группы: ");
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

        public void SeeStudentAverageGrade()
        {
            SqlConnection SConnection = new SqlConnection(ConnectionString);
            Console.WriteLine("Введите код студента: ");
            int StudentID = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Введите код предмета: ");
            int SubjectID = Int32.Parse(Console.ReadLine());
            string SubjectName;
            try
            {
                SConnection.Open();
                string query = $"Select SubjectName, Grade from GradeBookAndLogin.dbo.GradeLists JOIN Subjects on GradeLists.SubjectID = Subjects.SubjectID where StudentID = {StudentID} and SubjectID = {SubjectID}";
                SqlCommand cmd = new SqlCommand(query, SConnection);
                int AvgGrade = 0;                
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    SubjectName = (string)reader.GetValue(0);
                    int counter = 0;
                    int AllGrades = 0;
                    while (reader.Read())
                    {
                        AllGrades = Convert.ToInt32(reader.GetValue(1));
                        counter++;
                    }
                    if (counter != 0)
                    {
                        AvgGrade = AllGrades / counter;
                    }
                    else
                    {
                        Console.WriteLine("У этого студента нет оценок.");
                        return;
                    }
                }
                Console.WriteLine($"Средний балл этого студента по предмету {SubjectName}: {AvgGrade}");
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex);
            }
        }
        public void SeeStudentAverageGradeOverall()
        {
            SqlConnection SConnection = new SqlConnection(ConnectionString);
            Console.WriteLine("Введите код студента: ");
            int StudentID = Int32.Parse(Console.ReadLine());
            try
            {
                SConnection.Open();
                string query = $"Select Grade from GradeBookAndLogin.dbo.GradeLists where StudentID = {StudentID}";
                SqlCommand cmd = new SqlCommand(query, SConnection);
                int AvgGrade = 0;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    int counter = 0;
                    int AllGrades = 0;
                    while (reader.Read())
                    {
                        AllGrades = Convert.ToInt32(reader.GetValue(0));
                        counter++;
                    }
                    if (counter != 0)
                    {
                        AvgGrade = AllGrades / counter;
                    }
                    else
                    {
                        Console.WriteLine("У этого студента нет оценок.");
                        return;
                    }
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
            Console.WriteLine("Введите код студента: ");
            int StudentID = Int32.Parse(Console.ReadLine());
            try
            {
                SConnection.Open();
                string query = $"Select * from GradeBookAndLogin.dbo.GradeLists where StudentID = {StudentID}";
                SqlCommand cmd = new SqlCommand(query, SConnection);
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
