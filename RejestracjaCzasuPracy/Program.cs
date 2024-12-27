using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace RejestracjaCzasuPracy
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Podaj ścieżkę do folderu z plikami CSV:");
            string folderPath = Console.ReadLine();

            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
            {
                Console.WriteLine("Nieprawidłowa ścieżka folderu!");
                return;
            }

            var csvFiles = Directory.GetFiles(folderPath, "*.csv").ToList();
            if (!csvFiles.Any())
            {
                Console.WriteLine("Nie znaleziono plików CSV w podanym folderze.");
                return;
            }

            Console.WriteLine("Znalezione pliki CSV:");
            for (int i = 0; i < csvFiles.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Path.GetFileName(csvFiles[i])}");
            }

            Console.WriteLine("Wprowadź numery plików i ich typy (rcp1 lub rcp2), oddzielone spacją (np. '1 rcp1 2 rcp2'):");
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Nie wprowadzono danych.");
                return;
            }

            var assignments = ParseUserInput(input, csvFiles.Count);
            if (assignments == null)
            {
                Console.WriteLine("Niepoprawny format danych wejściowych.");
                return;
            }

            var csvProcessor = new CsvProcessor();
            var dzienPracyList = csvProcessor.PrzetworzPliki(csvFiles, assignments);

            if (!dzienPracyList.Any())
            {
                Console.WriteLine("Brak przetworzonych danych.");
                return;
            }

            Console.WriteLine("Przetworzone dane:");
            foreach (var dzienPracy in dzienPracyList)
            {
                Console.WriteLine($"{dzienPracy.KodPracownika}; {dzienPracy.Data:yyyy-MM-dd}; {dzienPracy.GodzinaWejscia}; {dzienPracy.GodzinaWyjscia}");
            }
        }

        static Dictionary<int, string> ParseUserInput(string input, int fileCount)
        {
            var parts = input.Split(' ');
            if (parts.Length % 2 != 0) return null;

            var assignments = new Dictionary<int, string>();
            for (int i = 0; i < parts.Length; i += 2)
            {
                if (!int.TryParse(parts[i], out var fileIndex) || fileIndex < 1 || fileIndex > fileCount) return null;
                var fileType = parts[i + 1].ToLower();
                if (fileType != "rcp1" && fileType != "rcp2") return null;

                assignments[fileIndex - 1] = fileType;
            }

            return assignments;
        }
    }

    public class CsvProcessor
    {
        public List<DzienPracy> PrzetworzPliki(List<string> csvFiles, Dictionary<int, string> assignments)
        {
            var dzienPracyList = new List<DzienPracy>();

            foreach (var assignment in assignments)
            {
                var filePath = csvFiles[assignment.Key];
                Console.WriteLine($"Przetwarzanie pliku: {filePath} jako {assignment.Value}");
                if (assignment.Value == "rcp1")
                {
                    PrzetworzPlikRcp1(filePath, dzienPracyList);
                }
                else if (assignment.Value == "rcp2")
                {
                    PrzetworzPlikRcp2(filePath, dzienPracyList);
                }
            }

            return dzienPracyList;
        }

        private void PrzetworzPlikRcp1(string sciezka, List<DzienPracy> dzienPracyList)
        {
            foreach (var linia in File.ReadAllLines(sciezka).Skip(1))
            {
                var dane = linia.Split(';');
                if (dane.Length < 5) continue;

                try
                {
                    var dzienPracy = new DzienPracy
                    {
                        KodPracownika = dane[0],
                        Data = DateTime.ParseExact(dane[1], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        GodzinaWejscia = TimeSpan.Parse(dane[2]),
                        GodzinaWyjscia = TimeSpan.Parse(dane[3])
                    };

                    if (!dzienPracyList.Any(dp => dp.KodPracownika == dzienPracy.KodPracownika && dp.Data == dzienPracy.Data))
                    {
                        dzienPracyList.Add(dzienPracy);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd przetwarzania linii: {linia}. Szczegóły: {ex.Message}");
                }
            }
        }

        private void PrzetworzPlikRcp2(string sciezka, List<DzienPracy> dzienPracyList)
        {
            var tempData = new Dictionary<string, Dictionary<DateTime, (TimeSpan wejscie, TimeSpan wyjscie)>>();

            foreach (var linia in File.ReadAllLines(sciezka).Skip(1))
            {
                var dane = linia.Split(';');
                if (dane.Length < 4) continue;

                try
                {
                    var kodPracownika = dane[0];
                    var data = DateTime.ParseExact(dane[1], "yyyy-MM-dd", CultureInfo.InvariantCulture);

                    if (string.IsNullOrWhiteSpace(dane[2]))
                    {
                        Console.WriteLine($"Pomijam linię z pustą godziną: {linia}");
                        continue;
                    }

                    var godzina = TimeSpan.Parse(dane[2]);
                    var wejscieWyjscie = dane[3];

                    if (!tempData.ContainsKey(kodPracownika))
                    {
                        tempData[kodPracownika] = new Dictionary<DateTime, (TimeSpan, TimeSpan)>();
                    }

                    if (!tempData[kodPracownika].ContainsKey(data))
                    {
                        tempData[kodPracownika][data] = (TimeSpan.Zero, TimeSpan.Zero);
                    }

                    if (wejscieWyjscie == "WE")
                    {
                        tempData[kodPracownika][data] = (godzina, tempData[kodPracownika][data].wyjscie);
                    }
                    else if (wejscieWyjscie == "WY")
                    {
                        tempData[kodPracownika][data] = (tempData[kodPracownika][data].wejscie, godzina);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd przetwarzania linii: {linia}. Szczegóły: {ex.Message}");
                }
            }

            foreach (var pracownik in tempData)
            {
                foreach (var dzien in pracownik.Value)
                {
                    if (dzien.Value.wejscie != TimeSpan.Zero && dzien.Value.wyjscie != TimeSpan.Zero)
                    {
                        dzienPracyList.Add(new DzienPracy
                        {
                            KodPracownika = pracownik.Key,
                            Data = dzien.Key,
                            GodzinaWejscia = dzien.Value.wejscie,
                            GodzinaWyjscia = dzien.Value.wyjscie
                        });
                    }
                }
            }
        }
    }

    public class DzienPracy
    {
        public string KodPracownika { get; set; }
        public DateTime Data { get; set; }
        public TimeSpan GodzinaWejscia { get; set; }
        public TimeSpan GodzinaWyjscia { get; set; }
    }
}
