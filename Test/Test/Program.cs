using System;
using System.Collections.Generic;

public class Student
{
    //public Guid Id { get; private set; }

    public string Name { get; private set; }

    public int Age { get; private set; }

    private List<int> Grades { get;  set; } = new List<int>();

    public IReadOnlyList<int> StudentGrades => Grades;

    public static HashSet<string> UserNames = new HashSet<string>();

    public Student(string name, int age)
    {
        //Id  = Guid.NewGuid();

        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("text");
        }

        if (UserNames.Count > 0)
        {
            if (UserNames.Contains(name))
            {
                throw new InvalidOperationException($"user with name: {name} already exists");
            }

            UserNames.Add(name);
        }
        else
        {
            UserNames.Add(name);
        }

        Name = name;

        if (age < 17 && age > 30)
        {
            throw new InvalidOperationException("invalid age");
        }

        Age = age;
    }

    public void SetGrade(int grade)
    {
        if (grade < 1 && grade > 100)
        {
            throw new ArgumentException("text");
        }

        Grades.Add(grade);
    }

    public override string ToString()
    {
        return $"Student: {Name}, Age: {Age}";
    }
}

public class School
{
    public void AddStudent(Student student)
    {
        Students.Add(student);
    }

    private List<Student> Students { get; set; } = new List<Student>();

    public IReadOnlyList<Student> SchoolStudents => Students;

    public double GetAverageGrade(string name)
    {
        Student? student = Students.FirstOrDefault(s => s.Name == name);

        if (student == null)
        {
            throw new InvalidOperationException("user not found");
        }

        if (student.StudentGrades.Any())
        {
            return student.StudentGrades.Average();
        }
        else
        {
            throw new InvalidOperationException("user has no grades");
        }
    }

    public void PrintAllStudents()
    {
        foreach (var student in Students)
        {
            Console.WriteLine(student.ToString());
        }
    }

    public void AddGrade(string name, int grade)
    {
        Student? student = Students.FirstOrDefault(s => s.Name == name);

        if (student == null)
        {
            throw new InvalidOperationException("user not found");
        }

        try
        {
            student.SetGrade(grade);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}

class Program
{
    static void Main()
    {
        School school = new School();

        Student student1 = new Student("Alice", 20);
        Student student2 = new Student("Bob", 22);

        school.AddStudent(student1);
        school.AddStudent(student2);

        school.AddGrade("Alice", 89);
        school.AddGrade("Alice", 95);
        school.AddGrade("Bob", 76);

        Console.WriteLine("Average grade for Alice: " + school.GetAverageGrade("Alice"));
        Console.WriteLine("Average grade for Bob: " + school.GetAverageGrade("Bob"));

        school.PrintAllStudents();

        Console.ReadLine();
    }
}
