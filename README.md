# Dokumentacja Programu Rejestracji Czasu Pracy

## Opis programu
Program służy do przetwarzania plików CSV zawierających dane o czasie pracy pracowników. Użytkownik może wybrać pliki CSV do przetworzenia oraz określić ich typ (rcp1 lub rcp2). Program wyświetla dane wejścia i wyjścia pracowników, zorganizowane według dat.

## Funkcjonalności
1. Pobieranie ścieżki do folderu z plikami CSV od użytkownika.
2. Przetwarzanie plików w różnych formatach (rcp1 i rcp2).
3. Wyświetlanie przetworzonych danych w konsoli.
4. Obsługa błędów w plikach CSV, takich jak brakujące dane lub nieprawidłowe formaty.

## Struktura kodu
### Klasa `Program`
Główna klasa programu, która:
- Pobiera ścieżkę folderu z plikami CSV.
- Wyświetla listę plików CSV w folderze.
- Odbiera od użytkownika dane o przypisaniu typów do plików.
- Wywołuje przetwarzanie plików.
- Wyświetla wyniki w konsoli.

#### Funkcje:
##### `Main`
**Opis:** Punkt wejścia programu.
- Pobiera ścieżkę folderu z plikami CSV.
- Obsługuje interakcję użytkownika.
- Przekazuje pliki do klasy `CsvProcessor` do przetwarzania.

##### `ParseUserInput`
**Opis:** Parsuje dane wejściowe użytkownika, mapując pliki CSV do ich typów.
- **Parametry:**
  - `input`: Ciąg znaków wprowadzony przez użytkownika.
  - `fileCount`: Liczba plików w folderze.
- **Zwracany typ:** Słownik, który mapuje indeksy plików na ich typy ("rcp1" lub "rcp2").

### Klasa `CsvProcessor`
Odpowiada za przetwarzanie plików CSV i wyodrębnianie danych o czasie pracy.

#### Funkcje:
##### `PrzetworzPliki`
**Opis:** Przetwarza listę plików CSV na podstawie ich typów.
- **Parametry:**
  - `csvFiles`: Lista ścieżek do plików CSV.
  - `assignments`: Słownik mapujący pliki na ich typy ("rcp1" lub "rcp2").
- **Zwracany typ:** Lista obiektów `DzienPracy` zawierających dane o czasie pracy.

##### `PrzetworzPlikRcp1`
**Opis:** Przetwarza plik w formacie rcp1.
- **Parametry:**
  - `sciezka`: Ścieżka do pliku CSV.
  - `dzienPracyList`: Lista do przechowywania przetworzonych danych.
- **Obsługa wyjątków:** Wyświetla komunikat o błędach w liniach z nieprawidłowym formatem danych.

##### `PrzetworzPlikRcp2`
**Opis:** Przetwarza plik w formacie rcp2.
- **Parametry:**
  - `sciezka`: Ścieżka do pliku CSV.
  - `dzienPracyList`: Lista do przechowywania przetworzonych danych.
- **Obsługa wyjątków:** Ignoruje linie z brakującymi danymi i wyświetla komunikaty o błędach.

### Klasa `DzienPracy`
Reprezentuje pojedynczy dzień pracy pracownika.
- **Atrybuty:**
  - `KodPracownika` (string): Kod identyfikacyjny pracownika.
  - `Data` (DateTime): Data pracy.
  - `GodzinaWejscia` (TimeSpan): Czas wejścia pracownika.
  - `GodzinaWyjscia` (TimeSpan): Czas wyjścia pracownika.

## Przykładowe dane wejściowe
### Format rcp1
```
KodPracownika;Data;GodzinaWejscia;GodzinaWyjscia
12345;2023-01-01;08:00:00;16:00:00
12345;2023-01-02;08:15:00;16:30:00
```

### Format rcp2
```
KodPracownika;Data;Godzina;WejscieWyjscie
12345;2023-01-01;08:00:00;WE
12345;2023-01-01;16:00:00;WY
12345;2023-01-02;08:15:00;WE
12345;2023-01-02;16:30:00;WY
```

## Przykładowe dane wyjściowe
```
12345; 2023-01-01; 08:00:00; 16:00:00
12345; 2023-01-02; 08:15:00; 16:30:00
```

## Wymagania systemowe
- .NET Framework 5.0 lub wyższy.
- System operacyjny z dostępem do konsoli (Windows, Linux, macOS).

## Instalacja i uruchamianie
1. Sklonuj repozytorium z GitHub:
   ```
   git clone https://github.com/Marssepan/zadanie-rekrutacyjne-csharp.git
   ```
2. Otwórz projekt w Visual Studio.
3. Skompiluj projekt i uruchom aplikację.

## Licencja
Projekt jest udostępniany na licencji MIT. Szczegóły znajdują się w pliku `LICENSE` w repozytorium.

