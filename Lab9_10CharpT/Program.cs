using System;

    // 1. Оголошуємо делегат - це "шаблон" для методу, який буде обробляти подію
    public delegate void FacultyEventHandler(string message);

    // ==========================================
    // ВИДАВЕЦЬ (Publisher): Клас Факультет
    // ==========================================
    public class Faculty
    {
        // 2. Створюємо події на основі делегата
        public event FacultyEventHandler OnNewsAnnounced;
        public event FacultyEventHandler OnExamStarted;

        // Метод, що імітує публікацію новин
        public void AnnounceNews(string news)
        {
            Console.WriteLine($"\n[📢 ДЕКАНАТ]: Увага всім! {news}");
            
            // Якщо на подію хтось підписаний, викликаємо її
            OnNewsAnnounced?.Invoke(news);
        }

        // Метод, що імітує початок іспиту
        public void StartExam(string subject)
        {
            Console.WriteLine($"\n[🎓 ДЕКАНАТ]: Розпочинається іспит з дисципліни '{subject}'.");
            
            // Запускаємо подію
            OnExamStarted?.Invoke(subject);
        }
    }

    // ==========================================
    // ПІДПИСНИК 1: Клас Студент
    // ==========================================
    public class Student
    {
        public string Name { get; set; }

        public Student(string name)
        {
            Name = name;
        }

        // Реакція студента на новину
        public void ReactToNews(string news)
        {
            Console.WriteLine($"  -> Студент {Name} радіє або сумує через новину: '{news}'");
        }

        // Реакція студента на іспит
        public void ReactToExam(string subject)
        {
            Console.WriteLine($"  -> Студент {Name} панікує, купує енергетик і йде здавати {subject}!");
        }
    }

    // ==========================================
    // ПІДПИСНИК 2: Клас Викладач
    // ==========================================
    public class Teacher
    {
        public string Name { get; set; }

        public Teacher(string name)
        {
            Name = name;
        }

        // Реакція викладача на новину
        public void ReactToNews(string news)
        {
            Console.WriteLine($"  -> Викладач {Name} взяв до уваги новину: '{news}'");
        }

        // Реакція викладача на іспит
        public void ReactToExam(string subject)
        {
            Console.WriteLine($"  -> Викладач {Name} суворо розкладає білети для іспиту з {subject}.");
        }
    }

    // ==========================================
    // ГОЛОВНА ПРОГРАМА
    // ==========================================
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; // Для коректного відображення української мови

            // Створюємо об'єкти
            Faculty fikt = new Faculty(); // Наш факультет
            
            Student student1 = new Student("Олександр");
            Student student2 = new Student("Марія");
            Teacher teacher1 = new Teacher("Василь Петрович");

            // 3. ПІДПИСКА НА ПОДІЇ (використовуємо оператор +=)
            // Студенти та викладач підписуються на новини
            fikt.OnNewsAnnounced += student1.ReactToNews;
            fikt.OnNewsAnnounced += student2.ReactToNews;
            fikt.OnNewsAnnounced += teacher1.ReactToNews;

            // На іспит підписуються всі
            fikt.OnExamStarted += student1.ReactToExam;
            fikt.OnExamStarted += student2.ReactToExam;
            fikt.OnExamStarted += teacher1.ReactToExam;

            // ==========================================
            // СИМУЛЯЦІЯ ЖИТТЯ ФАКУЛЬТЕТУ
            // ==========================================
            Console.WriteLine("=== СИМУЛЯЦІЯ ПОЧАЛАСЯ ===");

            // Деканат публікує новину
            fikt.AnnounceNews("Завтра вихідний день на честь Дня Університету!");

            // Деканат починає іспит
            fikt.StartExam("Програмування (C#)");

            // Відписка від події (наприклад, студент випустився або відрахований)
            Console.WriteLine("\n[Система]: Студент Олександр відрахований і більше не отримує сповіщень.");
            fikt.OnNewsAnnounced -= student1.ReactToNews;
            fikt.OnExamStarted -= student1.ReactToExam;

            // Нова подія (Олександр вже не відреагує)
            fikt.AnnounceNews("Зміна розкладу на наступний тиждень.");
            
            Console.ReadLine();
        }
    }
