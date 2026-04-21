using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FacultyLife
{
    // Типи подій для статистики
    public enum EventType { News, Exam }

    // Клас для передачі даних події з пріоритетом
    public class FacultyEvent
    {
        public string Content { get; set; }
        public EventType Type { get; set; }
        public int Priority { get; set; } // 1 - Високий, 2 - Середній, 3 - Низький
        public int WorkTimeMs { get; set; } // Скільки часу триває подія

        public FacultyEvent(string content, EventType type, int priority, int workTimeMs)
        {
            Content = content;
            Type = type;
            Priority = priority;
            WorkTimeMs = workTimeMs;
        }
    }

    public delegate void FacultyEventHandler(string message);

    public class Faculty
    {
        public event FacultyEventHandler OnNewsAnnounced;
        public event FacultyEventHandler OnExamStarted;

        // Черга з пріоритетом (Елемент, Пріоритет)
        private PriorityQueue<FacultyEvent, int> _eventQueue = new PriorityQueue<FacultyEvent, int>();
        
        // Статистика
        public int TotalProcessed { get; private set; } = 0;

        public void AddEventToQueue(FacultyEvent ev)
        {
            _eventQueue.Enqueue(ev, ev.Priority);
            Console.WriteLine($"[📥 ЧЕРГА]: Додано {ev.Type} (Пріоритет: {ev.Priority}): {ev.Content}");
        }

        // Асинхронний метод обробки всієї черги
        public async Task ProcessAllEventsAsync()
        {
            Stopwatch sw = Stopwatch.StartNew();
            Console.WriteLine("\n=== ПОЧАТОК ОБРОБКИ ЗАВДАНЬ ФАКУЛЬТЕТОМ ===\n");

            while (_eventQueue.Count > 0)
            {
                var ev = _eventQueue.Dequeue();
                
                // Імітація підготовки (асинхронна затримка)
                await Task.Delay(ev.WorkTimeMs);

                if (ev.Type == EventType.News)
                    OnNewsAnnounced?.Invoke(ev.Content);
                else
                    OnExamStarted?.Invoke(ev.Content);

                TotalProcessed++;
            }

            sw.Stop();
            Console.WriteLine($"\n=== СТАТИСТИКА ПЕРІОДУ ===");
            Console.WriteLine($"Оброблено подій: {TotalProcessed}");
            Console.WriteLine($"Загальний час роботи: {sw.ElapsedMilliseconds / 1000.0:F2} сек.");
        }
    }

    // Класи Студент та Викладач залишаються майже такими самими
    public class Student
    {
        public string Name { get; set; }
        public Student(string name) => Name = name;

        public void ReactToNews(string news) => 
            Console.WriteLine($"  -> Студент {Name} отримав новину: '{news}'");

        public void ReactToExam(string subject) => 
            Console.WriteLine($"  -> Студент {Name} терміново вчить {subject}!");
    }

    public class Teacher
    {
        public string Name { get; set; }
        public Teacher(string name) => Name = name;

        public void ReactToNews(string news) => 
            Console.WriteLine($"  -> Викладач {Name} прочитав: '{news}'");

        public void ReactToExam(string subject) => 
            Console.WriteLine($"  -> Викладач {Name} готує білети для {subject}.");
    }

    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Faculty faculty = new Faculty();
            Student s1 = new Student("Олександр");
            Teacher t1 = new Teacher("Дмитро Іванович");

            // Підписка
            faculty.OnNewsAnnounced += s1.ReactToNews;
            faculty.OnNewsAnnounced += t1.ReactToNews;
            faculty.OnExamStarted += s1.ReactToExam;
            faculty.OnExamStarted += t1.ReactToExam;

            // Наповнюємо чергу різними подіями з різними пріоритетами
            // Зверніть увагу: ми додаємо "Вихідний" першим, але з низьким пріоритетом (3)
            faculty.AddEventToQueue(new FacultyEvent("Завтра вихідний", EventType.News, 3, 500));
            faculty.AddEventToQueue(new FacultyEvent("Іспит з С#", EventType.Exam, 1, 1500));
            faculty.AddEventToQueue(new FacultyEvent("Збори кафедри", EventType.News, 2, 800));

            // Запускаємо асинхронну обробку
            await faculty.ProcessAllEventsAsync();

            Console.WriteLine("\nРобочий день завершено. Натисніть будь-яку клавішу...");
            Console.ReadKey();
        }
    }
}