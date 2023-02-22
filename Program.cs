using ADO.NET_Testing;
using Bogus;
using Bogus.DataSets;
using System.Diagnostics.Metrics;

StudentsService ss = new StudentsService();
StudentsVisitationService svs = new StudentsVisitationService();
string cmd;

do
{
    Console.Clear();
    Console.Write("---ADO.NET Students/Visitations---\n" +
        "Welcome Here's A List Of Commands:\n" +
        "1)create_tables\n" +
        "2)drop_tables\n" +
        "3)students_stats\n" +
        "4)visits_stats\n" +
        "5)add_student\n" +
        "6)add_visitation\n" +
        "7)generate_students\n" +
        "8)generate_visitations\n" +
        "0)exit\nYour Input: ");

    cmd = Console.ReadLine();
    cmd.ToLower();

    switch (cmd)
    {
        case "create_tables":
        case "1":
            create_tables();
            break;

        case "drop_tables":
        case "2":
            drop_tables();
            break;

        case "students_stats":
        case "3":
            Int64 count = 0;
            try { count = ss.GetCount(); }
            catch {Console.WriteLine("Table Doesn't Exist!"); Prompt(); break; }
            students_stats(count);
            break;

        case "visits_stats":
        case "4":
            Int64 count2 = 0;
            try { count2 = svs.GetCount(); }
            catch { Console.WriteLine("Table Doesn't Exist!"); Prompt(); break; }
            visitations_stats(count2);
            break;

        case "add_student":
        case "5":
            var student = make_student();
            if(ss.TableExists() == true && student.DOB.ToString() != "01/01/0001")
            {
                try { ss.AddStudent(student); Prompt(); }
                catch{ Console.WriteLine($"Add Failed! Please try again later!"); Prompt(); }
            }
            break;

        case "add_visitation":
        case "6":
            if(ss.TableExists()==false || svs.TableExists() == false) 
            { Console.WriteLine("A Table Is Missing!"); Prompt(); break; }
            var visit = make_visitation();
            var tempstudents = ss.GetStudents();
            if (svs.TableExists() == true && visit.STUDENTID != 0 && visit.DATE.ToString() != "01/01/0001")
            {
                if (tempstudents[visit.STUDENTID-1].DOB > visit.DATE)
                {
                    Console.WriteLine("Visit Date Cannot Be Before Date Of Birth!");
                    Prompt();
                }
                else
                {
                    try { svs.AddVisitation(visit); Prompt(); }
                    catch { Console.WriteLine($"Add Failed! Please try again later!"); Prompt(); }
                }
            }
            break;

        case "generate_students":
        case "7":
            if (ss.TableExists() == false || svs.TableExists() == false)
            { Console.WriteLine("A Table Is Missing!"); Prompt(); break; }
            generate_students();
            break;

        case "generate_visitations":
        case "8":
            if (ss.TableExists() == false || svs.TableExists() == false)
            { Console.WriteLine("A Table Is Missing!"); Prompt(); break; }
            generate_visitations();
            break;

        case "exit":
        case "0":
            Console.WriteLine("Exiting Programm...");
            break;

        default: 
            Console.WriteLine("Wrong Input! Please try again!");
            Prompt();
            break;



    }
   

} while (cmd != "Exit" && cmd != "exit" && cmd != "0");


void Prompt()
{
    Console.Write("Press Enter To Continue... ");
    Console.ReadLine();
}

void create_tables()
{
    try
    {
        ss.CreateTable();
        svs.CreateTable();
        Console.WriteLine("Tables Created!");
        Prompt();
    }
    catch
    {
        Console.WriteLine("Tables Already Exist!");
        Prompt();
    }
}

void drop_tables()
{
    try
    {
        svs.DropTable();
        ss.DropTable();
        Console.WriteLine("Tables Dropped!");
        Prompt();
    }
    catch
    {
        Console.WriteLine("Tables Don't Exist!");
        Prompt();
    }
}

void students_stats(Int64 count)
{
    int index = 1;
    if (count > 0)
    {
        var students = ss.GetStudents();
        foreach (var student in students)
        {
            Console.WriteLine($"ID: {index}, {student}");
            index++;
        }
        Prompt();
    }
    else
    {
        Console.WriteLine("Table Is Empty!");
        Prompt();
    }
}

void visitations_stats(Int64 count)
{
    if (count > 0)
    {
        var visits = svs.GetVisitations();
        var students = ss.GetStudents();

        foreach (var visit in visits)
        {
            Console.WriteLine($"STUDENT ID: {visit.STUDENTID}, STUDENT NAME: {students[visit.STUDENTID-1].FIO}, VISIT DATE: {visit.DATE}");
        }
        Prompt();
    }
    else
    {
        Console.WriteLine("Table Is Empty!");
        Prompt();
    }
}

Student make_student()
{
    Student student = new Student(); //ID,FIO,DOB,EMAIL
    if (ss.TableExists() == true)
    {
        do
        {
            Console.Write("Enter Student FIO: ");
            string fio = Console.ReadLine();
            fio = fio.Trim();
            student.FIO = fio;
        } while (student.FIO.Length <= 0);

        int day = 0;
        do
        {
            Console.Write("Enter Day Of Birth: ");
            try { day = int.Parse(Console.ReadLine()); }
            catch { Console.WriteLine("Invalid Term Detected!"); day = 0; }
        } while (day == 0);

        int month = 0;
        do
        {
            Console.Write("Enter Month Of Birth: ");
            try { month = int.Parse(Console.ReadLine()); }
            catch { Console.WriteLine("Invalid Term Detected!"); month = 0; }
        } while (month == 0);

        int year = 0;
        do
        {
            Console.Write("Enter Year Of Birth: ");
            try { year = int.Parse(Console.ReadLine()); }
            catch { Console.WriteLine("Invalid Term Detected!"); year = 0; }
        } while (year == 0);

        try { student.DOB = new DateOnly(year, month, day); }
        catch { Console.WriteLine("Invalid Date!"); Prompt(); return student; }


        do
        {
            Console.Write("Enter Student EMAIL: ");
            string email = Console.ReadLine();
            email = email.Trim();
            student.EMAIL = email;
        } while (student.FIO.Length <= 0);

        return student;
    }
    else
    {
        Console.WriteLine("Table Doesn't Exist!");
        Prompt();
        return student;
    }
    
}

Visitation make_visitation()
{
    Visitation visitation = new Visitation(); //ID,STUDENTID,DATE
    if (svs.TableExists() == true)
    {
        int id = 0;
        do
        {
            Console.Write("Enter StudentID: ");
            try { id = int.Parse(Console.ReadLine()); }
            catch { Console.WriteLine("Invalid Term Detected!"); id = 0; }
        } while (id == 0);

        if(ss.StudentExists(id))
        {
            visitation.STUDENTID = id;
        }
        else
        {
            Console.WriteLine("Student with that ID doesn't exist!");
            Prompt();
            visitation.STUDENTID = 0;
            return visitation;
        }
        

        int day = 0;
        do
        {
            Console.Write("Enter Day Of Visit: ");
            try { day = int.Parse(Console.ReadLine()); }
            catch { Console.WriteLine("Invalid Term Detected!"); day = 0; }
        } while (day == 0);

        int month = 0;
        do
        {
            Console.Write("Enter Month Of Visit: ");
            try { month = int.Parse(Console.ReadLine()); }
            catch { Console.WriteLine("Invalid Term Detected!"); month = 0; }
        } while (month == 0);

        int year = 0;
        do
        {
            Console.Write("Enter Year Of Visit: ");
            try { year = int.Parse(Console.ReadLine()); }
            catch { Console.WriteLine("Invalid Term Detected!"); year = 0; }
        } while (year == 0);

        try { visitation.DATE = new DateOnly(year, month, day); }
        catch { Console.WriteLine("Invalid Date!"); Prompt(); return visitation; }

        return visitation;
    }

    else
    {
        Console.WriteLine("Table Doesn't Exist!");
        Prompt();
        return visitation;
    }
}

void generate_students()
{
    int ammount = 0;
    do
    {
        Console.Write("Enter Ammount To Generate: ");
        try { ammount = int.Parse(Console.ReadLine()); }
        catch { Console.WriteLine("Invalid Term Detected!"); ammount = 0; }
    } while (ammount == 0);

    //Name & email Generator 
    var studentFaker = new Faker<Student>("en")
        .RuleFor(it => it.FIO, f => f.Name.FullName())
        .RuleFor(it => it.EMAIL, (f, it) => f.Internet.Email(it.FIO));

    //Randomiser
    var randomiser = new Bogus.Randomizer();

    //Generation
    for (int i = 0; i < ammount; i++)
    {
        var randominfo = studentFaker.Generate();
        Student student = new Student
        {
            FIO = randominfo.FIO,
            DOB = new DateOnly(randomiser.Int(1970, 2022), randomiser.Int(1, 12), randomiser.Int(1, 29)),
            EMAIL = randominfo.EMAIL
        };
        ss.AddStudent(student);
    }

    Console.WriteLine("Students Generated!");
    Prompt();
}

void generate_visitations()
{
    if(ss.GetCount() > 0)
    {
        int ammount = 0;
        do
        {
            Console.Write("Enter Ammount To Generate: ");
            try { ammount = int.Parse(Console.ReadLine()); }
            catch { Console.WriteLine("Invalid Term Detected!"); ammount = 0; }
        } while (ammount == 0);


        //Randomiser
        var randomiser = new Bogus.Randomizer();
        //Students
        var stu = ss.GetStudents();

        //Generation
        for (int i = 0; i < ammount; i++)
        {
            Visitation visitation = new Visitation
            {
                STUDENTID = randomiser.Int(1, (int)ss.GetCount()),
            };
            DateOnly bd = stu[visitation.STUDENTID - 1].DOB;
            visitation.DATE = new DateOnly(randomiser.Int(bd.Year+1, 2022), randomiser.Int(bd.Month+1, 12), randomiser.Int(bd.Day+1, 29));
            svs.AddVisitation(visitation);
        }

        Console.WriteLine("Students Generated!");
        Prompt();
    }

    else
    {
        Console.WriteLine("Cannot Generate Visitation With An Empty Student Table!");
        Prompt();
    }
    
}













