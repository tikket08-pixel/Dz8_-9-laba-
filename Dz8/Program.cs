using System;
using System.Collections.Generic;

namespace Task9_5
{
    enum ProjectStatus { Design, InProgress, Closed }
    enum TaskStatus { Assigned, InWork, OnReview, Done }

    class Report
    {
        private string _text;
        private DateTime _date;
        private Developer _author;

        public string Text { get { return _text; } set { _text = value; } }
        public DateTime Date { get { return _date; } }
        public Developer Author { get { return _author; } }
        public bool IsApproved { get; set; }

        public Report(string text, Developer author)
        {
            _text = text;
            _date = DateTime.Now;
            _author = author;
            IsApproved = false;
        }

        public void UpdateText(string newText)
        {
            _text = newText;
            _date = DateTime.Now;
        }

        public string GetAuthorName() { return _author.GetName(); }
        public string GetFormattedDate() { return _date.ToString("dd.MM.yyyy HH:mm"); }

        public void Print()
        {
            Console.WriteLine("Отчет от " + GetAuthorName() + " (" + GetFormattedDate() + ")");
            Console.WriteLine("  " + _text);
        }

        public void PrintShort()
        {
            Console.WriteLine(GetAuthorName() + " | " + GetFormattedDate() + " | " + _text);
        }
    }

    class ProjectTask
    {
        private string _description;
        private DateTime _deadline;
        private TeamLead _initiator;
        private Developer _executor;
        private TaskStatus _status;
        private Report _report;

        public string Description { get { return _description; } }
        public DateTime Deadline { get { return _deadline; } }
        public TeamLead Initiator { get { return _initiator; } }
        public Developer Executor { get { return _executor; } set { _executor = value; } }
        public TaskStatus Status { get { return _status; } set { _status = value; } }
        public Report Report { get { return _report; } set { _report = value; } }

        public ProjectTask(string description, DateTime deadline, TeamLead initiator)
        {
            _description = description;
            _deadline = deadline;
            _initiator = initiator;
            _status = TaskStatus.Assigned;
        }

        public bool IsCompleted() { return _status == TaskStatus.Done; }

        public void Print()
        {
            string executorName = _executor != null ? _executor.GetName() : "(нет)";
            Console.WriteLine("Задача: " + _description + " | до " + _deadline.ToString("dd.MM.yyyy")
                + " | исполнитель: " + executorName + " | статус: " + _status);
        }

        public void PrintFull()
        {
            Print();
            if (_report != null)
            {
                Console.WriteLine("  --- Отчёт ---");
                _report.Print();
            }
        }
    }

    abstract class Person
    {
        private static int _lastId = 0;
        private int _id;
        private string _name;

        public int Id { get { return _id; } }
        public string Name { get { return _name; } set { _name = value; } }

        protected Person(string name)
        {
            _id = ++_lastId;
            _name = name;
        }

        public abstract string GetRole();

        public string GetName() { return _name; }
        public void SetName(string n) { _name = n; }

        public void Print()
        {
            Console.WriteLine("#" + _id + " " + _name + " — " + GetRole());
        }
    }

    class Developer : Person
    {
        private List<ProjectTask> _tasks = new List<ProjectTask>();

        public List<ProjectTask> Tasks { get { return _tasks; } }
        public int TaskCount { get { return _tasks.Count; } }

        public Developer(string name) : base(name) { }
        public Developer() : base("Безымянный разработчик") { }

        public override string GetRole() { return "Разработчик"; }

        public void ReceiveTask(ProjectTask t)
        {
            _tasks.Add(t);
            t.Executor = this;
            t.Status = TaskStatus.Assigned;
            Console.WriteLine("  " + GetName() + " получил задачу: «" + t.Description + "»");
        }

        public void AcceptTask(ProjectTask t)
        {
            if (t.Executor != this) { Console.WriteLine("  Не моя задача"); return; }
            t.Status = TaskStatus.InWork;
            Console.WriteLine("  " + GetName() + " взял в работу: «" + t.Description + "»");
        }

        public void RejectTask(ProjectTask t)
        {
            if (t.Executor != this) { Console.WriteLine("  Не моя задача"); return; }
            t.Executor = null;
            t.Status = TaskStatus.Assigned;
            _tasks.Remove(t);
            Console.WriteLine("  " + GetName() + " отклонил задачу: «" + t.Description + "»");
        }

        public void DelegateTask(ProjectTask t, Developer other)
        {
            if (t.Executor != this) { Console.WriteLine("  Не моя задача"); return; }
            t.Executor = other;
            t.Status = TaskStatus.Assigned;
            other.Tasks.Add(t);
            _tasks.Remove(t);
            Console.WriteLine("  " + GetName() + " делегировал задачу «" + t.Description + "» → " + other.GetName());
        }

        public Report WriteReport(ProjectTask t, string text)
        {
            if (t.Executor != this) { Console.WriteLine("  Не моя задача"); return null; }
            Report r = new Report(text, this);
            t.Report = r;
            t.Status = TaskStatus.OnReview;
            Console.WriteLine("  " + GetName() + " написал отчет по задаче «" + t.Description + "» → статус: На проверке");
            return r;
        }

        public void ShowMyTasks()
        {
            Console.WriteLine("Задачи " + GetName() + " (" + _tasks.Count + "):");
            foreach (ProjectTask t in _tasks)
                t.Print();
        }
    }

    class TeamLead : Person
    {
        private List<ProjectTask> _createdTasks = new List<ProjectTask>();

        public List<ProjectTask> CreatedTasks { get { return _createdTasks; } }
        public int CreatedTasksCount { get { return _createdTasks.Count; } }

        public TeamLead(string name) : base(name) { }
        public TeamLead() : base("Безымянный тимлид") { }

        public override string GetRole() { return "Тимлид"; }

        public ProjectTask CreateTask(Project p, string description, DateTime deadline)
        {
            if (p.Status != ProjectStatus.Design)
            {
                Console.WriteLine("  Нельзя создавать задачи: проект не в статусе «Проект»");
                return null;
            }
            ProjectTask t = new ProjectTask(description, deadline, this);
            p.AddTask(t);
            _createdTasks.Add(t);
            Console.WriteLine("  " + GetName() + " создал задачу: «" + description + "» в проекте " + p.Name);
            return t;
        }

        public void AssignTask(ProjectTask t, Developer d)
        {
            d.ReceiveTask(t);
        }

        public void ApproveReport(ProjectTask t)
        {
            if (t.Status != TaskStatus.OnReview) { Console.WriteLine("  Задача не на проверке"); return; }
            t.Status = TaskStatus.Done;
            if (t.Report != null) t.Report.IsApproved = true;
            Console.WriteLine("  " + GetName() + " утвердил отчет «" + t.Description + "» → статус: Выполнена");
        }

        public void ReturnReport(ProjectTask t)
        {
            if (t.Status != TaskStatus.OnReview) { Console.WriteLine("  Задача не на проверке"); return; }
            t.Status = TaskStatus.InWork;
            if (t.Report != null) t.Report.IsApproved = false;
            Console.WriteLine("  " + GetName() + " вернул отчет «" + t.Description + "» на доработку → статус: В работе");
        }

        public void StartProject(Project p)
        {
            if (p.Status != ProjectStatus.Design) { Console.WriteLine("  Проект не в статусе «Проект»"); return; }
            p.Status = ProjectStatus.InProgress;
            Console.WriteLine("  " + GetName() + " перевел проект " + p.Name + " в статус: Исполнение");
        }

        public void CloseProject(Project p)
        {
            if (p.Status != ProjectStatus.InProgress) { Console.WriteLine("  Проект не в статусе Исполнение"); return; }
            if (!p.IsAllTasksDone()) { Console.WriteLine("  Не все задачи выполнены — нельзя закрыть"); return; }
            p.Status = ProjectStatus.Closed;
            Console.WriteLine("  " + GetName() + " закрыл проект " + p.Name);
        }
    }

    class Customer : Person
    {
        private List<Project> _projects = new List<Project>();

        public List<Project> Projects { get { return _projects; } }
        public int ProjectsCount { get { return _projects.Count; } }

        public Customer(string name) : base(name) { }
        public Customer() : base("Безымянный заказчик") { }

        public override string GetRole() { return "Заказчик"; }

        public Project CreateProject(string name, string description, DateTime deadline, TeamLead teamLead)
        {
            Project p = new Project(name, description, deadline, this, teamLead);
            _projects.Add(p);
            Console.WriteLine("  " + GetName() + " создал проект «" + name + "» (тимлид: " + teamLead.GetName() + ")");
            return p;
        }

        public void ShowProject(Project p) { p.PrintFull(); }

        public void ConfirmProject(Project p)
        {
            if (p.IsClosed)
                Console.WriteLine("  " + GetName() + " подтвердил закрытие проекта " + p.Name);
            else
                Console.WriteLine("  Проект ещё не закрыт");
        }
    }

    class Project
    {
        private string _name;
        private string _description;
        private DateTime _startDate;
        private DateTime _deadline;
        private Customer _customer;
        private TeamLead _teamLead;
        private List<ProjectTask> _tasks = new List<ProjectTask>();
        private ProjectStatus _status;

        public string Name { get { return _name; } }
        public string Description { get { return _description; } set { _description = value; } }
        public DateTime StartDate { get { return _startDate; } }
        public DateTime Deadline { get { return _deadline; } set { _deadline = value; } }
        public Customer Customer { get { return _customer; } }
        public TeamLead TeamLead { get { return _teamLead; } }
        public List<ProjectTask> Tasks { get { return _tasks; } }
        public ProjectStatus Status { get { return _status; } set { _status = value; } }
        public int TaskCount { get { return _tasks.Count; } }
        public bool IsClosed { get { return _status == ProjectStatus.Closed; } }

        public Project(string name, string description, DateTime deadline, Customer customer, TeamLead teamLead)
        {
            _name = name;
            _description = description;
            _startDate = DateTime.Now;
            _deadline = deadline;
            _customer = customer;
            _teamLead = teamLead;
            _status = ProjectStatus.Design;
        }

        public void AddTask(ProjectTask t) { _tasks.Add(t); }

        public bool IsAllTasksDone()
        {
            if (_tasks.Count == 0) return false;
            foreach (ProjectTask t in _tasks)
                if (!t.IsCompleted()) return false;
            return true;
        }

        public void Print()
        {
            Console.WriteLine("Проект: " + _name);
            Console.WriteLine("  описание: " + _description);
            Console.WriteLine("  сроки: " + _startDate.ToString("dd.MM.yyyy") + " — " + _deadline.ToString("dd.MM.yyyy"));
            Console.WriteLine("  заказчик: " + _customer.GetName());
            Console.WriteLine("  тимлид: " + _teamLead.GetName());
            Console.WriteLine("  статус: " + _status);
            Console.WriteLine("  задач: " + _tasks.Count);
        }

        public void PrintFull()
        {
            Print();
            if (_tasks.Count > 0)
            {
                Console.WriteLine("  --- Задачи ---");
                foreach (ProjectTask t in _tasks)
                    t.Print();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Задача. Task Manager.");
            Console.WriteLine("Один проект, 10 человек команды: 1 заказчик, 1 тимлид, 8 разработчиков.");
            Console.WriteLine("Показан полный цикл: создание проекта, создание задач, назначение,");
            Console.WriteLine("принятие/делегирование/отклонение, отчеты, проверка, закрытие.");
            Console.WriteLine("Действий не требуется.");
            Console.WriteLine();

            Customer customer = new Customer("Александр Заказчиков");
            TeamLead teamLead = new TeamLead("Олег Тимлидов");
            Developer[] devs = new Developer[8];
            string[] names = { "Иван", "Пётр", "Сергей", "Анна", "Мария", "Дмитрий", "Елена", "Никита" };
            for (int i = 0; i < 8; i++) devs[i] = new Developer(names[i]);

            Console.WriteLine("=== Команда (10 человек) ===");
            customer.Print();
            teamLead.Print();
            for (int i = 0; i < 8; i++) devs[i].Print();
            Console.WriteLine();

            Console.WriteLine("=== Создание проекта ===");
            Project project = customer.CreateProject(
                "Внутренний портал",
                "Разработка корпоративного портала для учёта задач",
                new DateTime(2026, 3, 1),
                teamLead);
            project.Print();
            Console.WriteLine();

            Console.WriteLine("=== Тимлид создаёт задачи (проект в статусе «Проект») ===");
            string[] taskDescr =
            {
                "Спроектировать БД",
                "Сверстать главную страницу",
                "Написать API для задач",
                "Сделать авторизацию",
                "Настроить уведомления",
                "Сделать страницу отчетов",
                "Написать тесты",
                "Подготовить документацию"
            };
            ProjectTask[] tasks = new ProjectTask[8];
            for (int i = 0; i < 8; i++)
                tasks[i] = teamLead.CreateTask(project, taskDescr[i], new DateTime(2026, 2, 1 + i));
            Console.WriteLine();

            Console.WriteLine("=== Назначение задач исполнителям ===");
            for (int i = 0; i < 8; i++) teamLead.AssignTask(tasks[i], devs[i]);
            Console.WriteLine();

            Console.WriteLine("=== Переводим проект в «Исполнение» ===");
            teamLead.StartProject(project);
            Console.WriteLine();

            Console.WriteLine("=== Нельзя создавать задачи после старта ===");
            teamLead.CreateTask(project, "Новая задача", DateTime.Now.AddDays(30));
            Console.WriteLine();

            Console.WriteLine("=== Задача 1 (БД): принять и сдать ===");
            devs[0].AcceptTask(tasks[0]);
            devs[0].WriteReport(tasks[0], "Схема БД готова, таблицы созданы.");
            teamLead.ApproveReport(tasks[0]);
            Console.WriteLine();

            Console.WriteLine("=== Задача 2 (Вёрстка): возврат на доработку ===");
            devs[1].AcceptTask(tasks[1]);
            devs[1].WriteReport(tasks[1], "Сверстал без адаптива.");
            teamLead.ReturnReport(tasks[1]);
            devs[1].WriteReport(tasks[1], "Доработал, добавил адаптив.");
            teamLead.ApproveReport(tasks[1]);
            Console.WriteLine();

            Console.WriteLine("=== Задача 3 (API): принять и сдать ===");
            devs[2].AcceptTask(tasks[2]);
            devs[2].WriteReport(tasks[2], "API готов, документация в Swagger.");
            teamLead.ApproveReport(tasks[2]);
            Console.WriteLine();

            Console.WriteLine("=== Задача 4 (Авторизация): делегирование ===");
            devs[3].DelegateTask(tasks[3], devs[7]);
            devs[7].AcceptTask(tasks[3]);
            devs[7].WriteReport(tasks[3], "Авторизация по JWT сделана.");
            teamLead.ApproveReport(tasks[3]);
            Console.WriteLine();

            Console.WriteLine("=== Задача 5 (Уведомления): отклонение и переназначение ===");
            devs[4].RejectTask(tasks[4]);
            teamLead.AssignTask(tasks[4], devs[0]);
            devs[0].AcceptTask(tasks[4]);
            devs[0].WriteReport(tasks[4], "Уведомления через email и push.");
            teamLead.ApproveReport(tasks[4]);
            Console.WriteLine();

            Console.WriteLine("=== Задача 6 (Отчеты): принять и сдать ===");
            devs[5].AcceptTask(tasks[5]);
            devs[5].WriteReport(tasks[5], "Страница отчетов готова.");
            teamLead.ApproveReport(tasks[5]);
            Console.WriteLine();

            Console.WriteLine("=== Задача 7 (Тесты): принять и сдать ===");
            devs[6].AcceptTask(tasks[6]);
            devs[6].WriteReport(tasks[6], "Покрытие 80%.");
            teamLead.ApproveReport(tasks[6]);
            Console.WriteLine();

            Console.WriteLine("=== Задача 8 (Документация): принять и сдать ===");
            devs[7].AcceptTask(tasks[7]);
            devs[7].WriteReport(tasks[7], "Документация в Wiki.");
            teamLead.ApproveReport(tasks[7]);
            Console.WriteLine();

            Console.WriteLine("=== Итоговое состояние проекта ===");
            project.PrintFull();
            Console.WriteLine();

            Console.WriteLine("=== Закрытие проекта ===");
            teamLead.CloseProject(project);
            customer.ConfirmProject(project);
            Console.WriteLine();

            Console.WriteLine("=== Кто чем занимался ===");
            for (int i = 0; i < 8; i++)
                Console.WriteLine("  " + devs[i].GetName() + " — текущих задач: " + devs[i].TaskCount);
            Console.WriteLine("Заказчик: создал проект, подтвердил закрытие.");
            Console.WriteLine("Тимлид: создал 8 задач, проверил 8 отчетов, закрыл проект.");
            Console.WriteLine("Все 8 разработчиков получали задачи (Иван и Никита — по две,");
            Console.WriteLine("Анна делегировала, Мария отклонила — это тоже участие).");
            Console.WriteLine("Все 10 членов команды задействованы.");

            Console.WriteLine();
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
} 