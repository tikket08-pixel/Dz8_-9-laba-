using System;

namespace Task9_4
{
    class Song
    {
        private string _name;
        private string _author;
        private Song _prev;

        public Song()
        {
            _name = "(без названия)";
            _author = "(без автора)";
            _prev = null;
        }

        public Song(string name, string author)
        {
            _name = name;
            _author = author;
            _prev = null;
        }

        public Song(string name, string author, Song prev)
        {
            _name = name;
            _author = author;
            _prev = prev;
        }

        public void SetName(string n) { _name = n; }
        public void SetAuthor(string a) { _author = a; }
        public void SetPrev(Song p) { _prev = p; }

        public string GetName() { return _name; }
        public string GetAuthor() { return _author; }
        public Song GetPrev() { return _prev; }

        public string Title()
        {
            return _name + " — " + _author;
        }

        public void Print()
        {
            Console.WriteLine(Title());
        }

        public override bool Equals(object d)
        {
            Song other = d as Song;
            if (other == null) return false;
            return _name == other._name && _author == other._author;
        }

        public override int GetHashCode()
        {
            return (_name == null ? 0 : _name.GetHashCode())
                 ^ (_author == null ? 0 : _author.GetHashCode());
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Задача ДЗ 9.1. Класс Song с конструкторами.");
            Console.WriteLine("Вопрос: будет ли ошибка при Song mySong = new Song(); ?");
            Console.WriteLine("Ответ: БЕЗ конструктора по умолчанию — да, ошибка компиляции,");
            Console.WriteLine("потому что после объявления своих конструкторов с параметрами");
            Console.WriteLine("компилятор больше не добавляет default-конструктор автоматически.");
            Console.WriteLine("Исправление: добавлен конструктор Song() без параметров.");
            Console.WriteLine("Действий не требуется.");
            Console.WriteLine();

            Song mySong = new Song();
            Console.WriteLine("Song mySong = new Song(); — работает после исправления:");
            mySong.Print();
            Console.WriteLine();

            Song s1 = new Song("Ветер", "Кино");
            Console.WriteLine("Song s1 = new Song(\"Ветер\", \"Кино\") — 1-й конструктор:");
            s1.Print();
            Console.WriteLine();

            Song s2 = new Song("Звезда", "Кино", s1);
            Console.WriteLine("Song s2 = new Song(\"Звезда\", \"Кино\", s1) — 2-й конструктор:");
            s2.Print();
            Console.WriteLine("  предыдущая: " + s2.GetPrev().Title());
            Console.WriteLine();

            Song s3 = new Song("Ветер", "Кино");
            Console.WriteLine("Сравнение s1 и s3 (одинаковые поля): " + s1.Equals(s3));
            Console.WriteLine("Сравнение s1 и s2 (разные): " + s1.Equals(s2));

            Console.WriteLine();
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
} 