using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Threading;

using System.Data;
using System.Xml.Serialization;

using System.Text.RegularExpressions;

using System.Diagnostics;
using System.ComponentModel;

using System.ComponentModel.Design.Serialization;
using System.Collections;
using Microsoft.Win32;

// ReSharper disable All

namespace PWR_PL_Implementator
{
    public class WersjaOS
    {
        public string Nazwa { get; set; }
        public string WersjaGlowna { get; set; }
        public string Build { get; set; }
        public string Podbuild { get; set; }
        public string ServicePack { get; set; }

        public override string ToString()
        {
            string wyswietlany_string;
            if (Podbuild != "" && Podbuild != " ")
            {
                wyswietlany_string = Nazwa + " " + WersjaGlowna + " " + Build + "." + Podbuild + " " + ServicePack; ;
            }
            else
            {
                wyswietlany_string = Nazwa + " " + WersjaGlowna + " " + Build + " " + ServicePack; ;
            }

            return wyswietlany_string;
        }
    }

    class PWR_PL_Implementator
    {
        readonly static long _kompilacja = 202405211709;
        readonly static string _PWR_PL_naglowek = "Implementator polonizacji PWR_PL (2024), kompilacja " + _kompilacja + " by Revok";

        public static readonly char sc = System.IO.Path.DirectorySeparatorChar;
        public static readonly WersjaOS wersjauzywanegoOS = WersjaUzywanegoOS();
        public static string exe_sciezka = AppDomain.CurrentDomain.BaseDirectory + "Zaimplementuj_PWR_PL";
        readonly static string wersja_polonizacji = PobierzNumerWersjiPolonizacji();
        
        readonly static bool rejestruj_log = true; // jeśli zmienna jest ustawiona jako "true", wtedy rejestrowane są zdarzenia implementatora do pliku: %APPDATA$\PWR_PL\PWR_PL.log
        static FileStream? plikLOG_fs;
        static StreamWriter? plikLOG_sw;

        static List<string> listasciezek_wykrytekonflikty = new List<string>();

        private static WersjaOS WersjaUzywanegoOS()
        {
            /*
            +-----------------------------------------------------------+
|           | OS              | Platform     | Major | Minor | Build    |
            +-----------------------------------------------------------+
            | Windows 95      | Win32Windows |   4   |   0   |          |
            | Windows 98      | Win32Windows |   4   |  10   |          |
            | Windows Me      | Win32Windows |   4   |  90   |          |
            | Windows NT 4.0  | Win32NT      |   4   |   0   |          |
            | Windows 2000    | Win32NT      |   5   |   0   |          |
            | Windows XP      | Win32NT      |   5   |   1   |          |
            | Windows 2003    | Win32NT      |   5   |   2   |          |
            | Windows Vista   | Win32NT      |   6   |   0   |          |
            | Windows 2008    | Win32NT      |   6   |   0   |          |
            | Windows 7       | Win32NT      |   6   |   1   |          |
            | Windows 2008 R2 | Win32NT      |   6   |   1   |          |
            | Windows 8       | Win32NT      |   6   |   2   |          |
            | Windows 8.1     | Win32NT      |   6   |   3   |          |
            | Windows 10      | Win32NT      |  10   |   0   | <22000   |
            | Windows 11      | Win32NT      |  10   |   0   |  22000<= |
            +-----------------------------------------------------------+
            */

            /*
            Console.WriteLine("[DEBUG] Environment.OSVersion==" + Environment.OSVersion);
            Console.WriteLine("[DEBUG] Environment.OSVersion.Platform==" + Environment.OSVersion.Platform);
            Console.WriteLine("[DEBUG] Environment.OSVersion.Version==" + Environment.OSVersion.Version);
            Console.WriteLine("[DEBUG] Environment.OSVersion.Version.Major==" + Environment.OSVersion.Version.Major);
            Console.WriteLine("[DEBUG] Environment.OSVersion.Version.MajorRevision==" + Environment.OSVersion.Version.MajorRevision);
            Console.WriteLine("[DEBUG] Environment.OSVersion.Version.Minor==" + Environment.OSVersion.Version.Minor);
            Console.WriteLine("[DEBUG] Environment.OSVersion.Version.MinorRevision==" + Environment.OSVersion.Version.MinorRevision);
            Console.WriteLine("[DEBUG] Environment.OSVersion.Version.Revision==" + Environment.OSVersion.Version.Revision);
            Console.WriteLine("[DEBUG] Environment.OSVersion.Version.Build==" + Environment.OSVersion.Version.Build);
            Console.WriteLine("[DEBUG] Environment.OSVersion.ServicePack==" + Environment.OSVersion.ServicePack);
            Console.WriteLine("[DEBUG] Environment.OSVersion.VersionString==" + Environment.OSVersion.VersionString);
            */

            string? glownanazwa_OS;
            string? glowneoznaczeniewersji_OS = "NULL";
            string? numerbuildu_OS = Environment.OSVersion.Version.Build.ToString();
            string? podnumerbuildu_OS = "-1";
            string? servicepack_OS = "";

            string[] pelnynumerwersji_OS = Environment.OSVersion.Version.ToString().Split(new char[] { '.' });


            if (Environment.OSVersion.Platform.ToString() == "Win32NT")
            {
                if (Environment.OSVersion.Version.Major == 4 && Environment.OSVersion.Version.Minor == 0)
                {
                    glowneoznaczeniewersji_OS = "NT 4.0";
                }
                else if (Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor == 0)
                {
                    glowneoznaczeniewersji_OS = "2000";
                }
                else if (Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor == 1)
                {
                    glowneoznaczeniewersji_OS = "XP";
                }
                else if (Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor == 2)
                {
                    glowneoznaczeniewersji_OS = "2003";
                }
                else if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 0)
                {
                    glowneoznaczeniewersji_OS = "Vista/2008";
                }
                else if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1)
                {
                    glowneoznaczeniewersji_OS = "7/2008 R2";
                }
                else if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 2)
                {
                    glowneoznaczeniewersji_OS = "8";
                }
                else if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 3)
                {
                    glowneoznaczeniewersji_OS = "8.1";
                }
                else if (Environment.OSVersion.Version.Major == 10 && Environment.OSVersion.Version.Minor == 0)
                {
                    if (Environment.OSVersion.Version.Build < 22000)
                    {
                        glowneoznaczeniewersji_OS = "10";
                    }
                    else if (Environment.OSVersion.Version.Build >= 22000)
                    {
                        glowneoznaczeniewersji_OS = "11";
                    }
                }

                servicepack_OS = Environment.OSVersion.ServicePack;
            }
            else if (Environment.OSVersion.Platform.ToString() == "Win32Windows")
            {
                if (Environment.OSVersion.Version.Major == 4 && Environment.OSVersion.Version.Minor == 0)
                {
                    glowneoznaczeniewersji_OS = "95";
                }
                else if (Environment.OSVersion.Version.Major == 4 && Environment.OSVersion.Version.Minor == 10)
                {
                    glowneoznaczeniewersji_OS = "98";
                }
                else if (Environment.OSVersion.Version.Major == 4 && Environment.OSVersion.Version.Minor == 90)
                {
                    glowneoznaczeniewersji_OS = "Me";
                }

                servicepack_OS = Environment.OSVersion.ServicePack;
            }
            else if (Environment.OSVersion.Platform.ToString() == "Unix" || Environment.OSVersion.Platform.ToString() == "Linux")
            {
                if (File.Exists("/etc/os-release") == true)
                {
                    
                    FileStream etcosrelease_fs = new FileStream("/etc/os-release", FileMode.Open, FileAccess.Read);

                    try
                    {
                        StreamReader plik_etcosrelease_sr = new StreamReader(etcosrelease_fs);

                        while (plik_etcosrelease_sr.Peek() != -1)
                        {
                            string tresc_linii = plik_etcosrelease_sr.ReadLine();

                            if (tresc_linii.Contains("NAME") == true && tresc_linii.Contains("PRETTY_NAME") == false && tresc_linii.Contains("CODENAME") == false)
                            {
                                glowneoznaczeniewersji_OS = tresc_linii.Replace("NAME=", "").Replace("\"", "");
                                //Console.WriteLine("[DEBUG] glowneoznaczeniewersji_OS == " + glowneoznaczeniewersji_OS);
                            }
                            else if (tresc_linii.Contains("VERSION_ID") == true)
                            {
                                numerbuildu_OS = tresc_linii.Replace("VERSION_ID=", "").Replace("\"", "");;
                                //Console.WriteLine("[DEBUG] numerbuildu_OS == " + numerbuildu_OS);
                            }
                            else if (tresc_linii.Contains("VERSION_CODENAME") == true)
                            {
                                podnumerbuildu_OS = tresc_linii.Replace("VERSION_CODENAME=", "").Replace("\"", "");;
                                //Console.WriteLine("[DEBUG] podnumerbuildu_OS == " + podnumerbuildu_OS);
                            }
                            else if (tresc_linii.Contains("ID") && (tresc_linii.Contains("VERSION_ID") == false))
                            {
                                servicepack_OS = tresc_linii.Replace("ID=", "").Replace("\"", "");;
                                //Console.WriteLine("[DEBUG] servicepack_OS == " + servicepack_OS);
                            }
                        }
                    }
                    catch
                    {
                        //pusta
                    }
                }
                else
                {
                    glowneoznaczeniewersji_OS = Environment.OSVersion.Platform.ToString();
                    
                    string[] osversion_split = Environment.OSVersion.ToString().Split(" ");
                    if (osversion_split.Length > 1)
                    {
                        numerbuildu_OS = osversion_split[1];
                    }
                    else
                    {
                        numerbuildu_OS = osversion_split[0];
                    }

                    podnumerbuildu_OS = "";
                    servicepack_OS = "";


                }
            }

            if (Environment.OSVersion.ToString().Contains("Microsoft Windows") == true)
            {
                glownanazwa_OS = "Microsoft Windows";

                podnumerbuildu_OS = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion").GetValue("UBR").ToString();

            }
            else if (Environment.OSVersion.ToString().Contains("Unix") == true || Environment.OSVersion.ToString().Contains("Linux"))
            {
                glownanazwa_OS = "Unix";
            }
            else
            {
                glownanazwa_OS = "NULL";
            }

            WersjaOS wersja_OS = new WersjaOS { Nazwa = glownanazwa_OS, WersjaGlowna = glowneoznaczeniewersji_OS, Build = "build " + numerbuildu_OS.ToString(), Podbuild = podnumerbuildu_OS.ToString(), ServicePack = servicepack_OS };

            return wersja_OS;

        }

        private static string PobierzAktualnaDateICzas()
        {
            return DateTime.Now.ToString("[yyyy.MM.dd HH:mm:ss]");
        }

        private static void InicjalizujRejestratorLOG()
        {
            if (rejestruj_log == true)
            {
                if (Directory.Exists(APPDATA("PWR_PL" + sc)) == false)
                {
                    Directory.CreateDirectory(APPDATA("PWR_PL" + sc));
                }

                plikLOG_fs = new FileStream(APPDATA($"PWR_PL{sc}PWR_PL.log"), FileMode.Append, FileAccess.Write);

                try
                {
                    plikLOG_sw = new StreamWriter(plikLOG_fs);
                }
                catch
                {
                    Blad("BŁĄD (#LOG): Wystąpił nieoczekiwany problem z zapisywaniem zdarzeń implementatora. Spróbuj uruchomić instalator spolszczenia z uprawnieniami Administratora.");
                }

            }
        }

        private static void ZapiszLOG(string tresc)
        {
            if (rejestruj_log == true)
            {
                plikLOG_sw.WriteLine(PobierzAktualnaDateICzas() + " " + tresc);
            }
        }

        private static void ZamknijRejestratorLOG()
        {
            if (rejestruj_log == true)
            {
                plikLOG_sw.Close();
                plikLOG_fs.Close();
            }
        }

        private static void Koniec()
        {
            ZapiszLOG("Zakończono działanie implementatora (kompilacja " + _kompilacja + ") w OS: " + wersjauzywanegoOS + " w: " + exe_sciezka);
            ZamknijRejestratorLOG();

            Console.WriteLine("Kliknij dowolny klawisz, aby zamknąć to okno.");
            Console.ReadKey();
        }
        private static void Blad(string tresc)
        {
            ZapiszLOG("Komunikat BŁĘDU: \"" + tresc + "\"");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(tresc);
            Console.ResetColor();

        }
        private static void Sukces(string tresc)
        {
            ZapiszLOG("Komunikat SUKCESU: \"" + tresc + "\"");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(tresc);
            Console.ResetColor();

        }
        private static void Sukces2(string tresc)
        {
            ZapiszLOG("Komunikat SUKCESU: \"" + tresc + "\"");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(tresc);
            Console.ResetColor();
        }
        private static void Informacja(string tresc)
        {
            ZapiszLOG("Komunikat INFORMACYJNY: \"" + tresc + "\"");

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(tresc);
            Console.ResetColor();
        }

        private static string PobierzNumerWersjiPolonizacji()
        {
            string numerwersjipolonizacji = "NULL";

            string sciezka_do_pliku_IntroductoryText = "Implementacja" + sc + "Wrath_Data" + sc + "StreamingAssets" + sc + "IntroductoryText.json";

            string plikIntroductoryText_tresc = "";

            if (File.Exists(sciezka_do_pliku_IntroductoryText) == true)
            {
                FileStream plikIntroductoryText_fs = new FileStream(sciezka_do_pliku_IntroductoryText, FileMode.Open, FileAccess.Read);

                try
                {
                    StreamReader plikIntroductoryText_sr = new StreamReader(plikIntroductoryText_fs);

                    plikIntroductoryText_tresc = plikIntroductoryText_sr.ReadToEnd();

                    plikIntroductoryText_sr.Close();

                }
                catch
                {
                    Blad("BŁĄD (#IntroductoryText): Wystąpił nieoczekiwany problem z odczytem przynajmniej jednego pliku gry. Spróbuj uruchomić instalator spolszczenia z uprawnieniami Administratora.");

                }

                plikIntroductoryText_fs.Close();

                if (plikIntroductoryText_tresc.Contains("Zainstalowana wersja: ") == true && plikIntroductoryText_tresc.Contains("</size>"))
                {
                    string[] filtr1 = plikIntroductoryText_tresc.Split("Zainstalowana wersja: ", StringSplitOptions.None);

                    if (filtr1.Length > 1)
                    {
                        string[] filtr2 = filtr1[1].Split("</size>", StringSplitOptions.None);

                        if (filtr2.Length > 1)
                        {
                            numerwersjipolonizacji = filtr2[0];
                        }
                    }
                    else
                    {
                        numerwersjipolonizacji = "NULL";
                    }
                }
                else
                {
                    numerwersjipolonizacji = "NULL";
                }

            }
            else
            {
                numerwersjipolonizacji = "NULL";
            }

            //Console.WriteLine("[DEBUG] numerwersjipolonizacji==" + numerwersjipolonizacji);

            return numerwersjipolonizacji;

        }

        private static string PobierzDaneZVersionInfo(string sciezka_do_pliku_VersionInfo)
        {
            string plikVersionInfo_tresc = "";

            if (File.Exists(sciezka_do_pliku_VersionInfo))
            {
                FileStream plikVersionInfo_fs = new FileStream(sciezka_do_pliku_VersionInfo, FileMode.Open, FileAccess.Read);

                try
                {
                    StreamReader plikVersionInfo_sr = new StreamReader(plikVersionInfo_fs);

                    plikVersionInfo_tresc = plikVersionInfo_sr.ReadToEnd();

                    plikVersionInfo_sr.Close();

                }
                catch
                {
                    Blad("BŁĄD (#Version): Wystąpił nieoczekiwany problem z odczytem przynajmniej jednego pliku gry. Spróbuj uruchomić instalator spolszczenia z uprawnieniami Administratora.");
                }

                plikVersionInfo_fs.Close();

                return plikVersionInfo_tresc;
            }
            else
            {
                return "NULL NULL NULL NULL NULL";
            }
        }

        private static void SkopiujFolderWrazZZawartoscia(string sciezka_folderu_do_skopiowania, string sciezka_docelowa)
        {
            DirectoryInfo folderzrodlowy_di = new DirectoryInfo(sciezka_folderu_do_skopiowania);
            DirectoryInfo folderdocelowy_di = new DirectoryInfo(sciezka_docelowa);

            if (folderzrodlowy_di.FullName.ToLower() == folderdocelowy_di.FullName.ToLower())
            {
                return;
            }

            if (Directory.Exists(folderdocelowy_di.FullName) == false)
            {
                Directory.CreateDirectory(folderdocelowy_di.FullName);
            }

            foreach (FileInfo fi in folderzrodlowy_di.GetFiles())
            {
                //Console.WriteLine(@"[DEBUG] Kopiowanie {0}\{1}", folderdocelowy_di.FullName, fi.Name);

                fi.CopyTo(Path.Combine(folderdocelowy_di.ToString(), fi.Name), true);
            }

            foreach (DirectoryInfo diSourceSubDir in folderzrodlowy_di.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    folderdocelowy_di.CreateSubdirectory(diSourceSubDir.Name);
                //SkopiujFolderWrazZZawartoscia(diSourceSubDir, nextTargetSubDir);
            }
        }

        private static string APPDATA(string sciezka_wewnatrz_APPDATA)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), sciezka_wewnatrz_APPDATA);
        }

        private static void ZmienJezykWPlikuKonfiguracyjnymGry(string oznaczeniejezyka_przedzmiana, string oznaczeniejezyka_pozmianie) // "deDE": niemiecki/polski, "enGB": angielski
        {
            string path_pwrconfig = "";
            if (wersjauzywanegoOS.Nazwa.Contains("Windows") == true)
            {
                path_pwrconfig = APPDATA($"..{sc}LocalLow{sc}Owlcat Games{sc}Pathfinder Wrath Of The Righteous{sc}");
            }
            else if (wersjauzywanegoOS.Nazwa.Contains("Unix") == true || wersjauzywanegoOS.Nazwa.Contains("Linux"))
            {
                path_pwrconfig = $"..{sc}..{sc}..{sc}compatdata{sc}1184370{sc}pfx{sc}drive_c{sc}users{sc}steamuser{sc}AppData{sc}LocalLow{sc}Owlcat Games{sc}Pathfinder Wrath Of The Righteous{sc}";
            }
            else
            {
                path_pwrconfig = "NULL";
            }

            if (path_pwrconfig != "NULL")
            {
                if (File.Exists(path_pwrconfig + "general_settings.json") ==
                    true)
                {
                    File.Move
                    (
                        path_pwrconfig + "general_settings.json",
                        path_pwrconfig + "general_settings.json.TMP"
                    );

                    FileStream plikkonfiguracjigry_przedzmiana_fs = new FileStream(
                        path_pwrconfig + "general_settings.json.TMP",
                        FileMode.Open, FileAccess.Read);

                    string plikkonfiguracjigry_przedzmiana_tresc = "";

                    try
                    {
                        StreamReader plikkonfiguracjigry_przedzmiana_sr =
                            new StreamReader(plikkonfiguracjigry_przedzmiana_fs);

                        plikkonfiguracjigry_przedzmiana_tresc = plikkonfiguracjigry_przedzmiana_sr.ReadToEnd();

                        plikkonfiguracjigry_przedzmiana_sr.Close();

                    }
                    catch
                    {
                        Blad(
                            "BŁĄD (#GeneralSettingsTMP(Read)): Wystąpił nieoczekiwany problem z odczytem przynajmniej jednego pliku gry. Spróbuj uruchomić instalator spolszczenia z uprawnieniami Administratora.");
                    }

                    plikkonfiguracjigry_przedzmiana_fs.Close();



                    FileStream plikkonfiguracjigry_pozmianie_fs = new FileStream(
                        path_pwrconfig + "general_settings.json",
                        FileMode.CreateNew, FileAccess.Write);

                    try
                    {
                        StreamWriter plikkonfiguracjigry_pozmianie_sw =
                            new StreamWriter(plikkonfiguracjigry_pozmianie_fs);

                        plikkonfiguracjigry_pozmianie_sw.Write(plikkonfiguracjigry_przedzmiana_tresc.Replace(
                            "\"settings.game.main.locale\": \"" + oznaczeniejezyka_przedzmiana + "\"",
                            "\"settings.game.main.locale\": \"" + oznaczeniejezyka_pozmianie + "\""));

                        plikkonfiguracjigry_pozmianie_sw.Close();

                    }
                    catch
                    {
                        Blad(
                            "BŁĄD (#GeneralSettings(Write)): Wystąpił nieoczekiwany problem z odczytem przynajmniej jednego pliku gry. Spróbuj uruchomić instalator spolszczenia z uprawnieniami Administratora.");
                    }

                    plikkonfiguracjigry_pozmianie_fs.Close();

                    if (File.Exists(path_pwrconfig + "general_settings.json.TMP") == true)
                    {
                        File.Delete(path_pwrconfig + "general_settings.json.TMP");
                    }

                    ZapiszLOG("Zmodyfikowano oznaczenie języka w pliku konfiguracyjnym gry z \"" +
                    oznaczeniejezyka_przedzmiana + "\" na \"" + oznaczeniejezyka_pozmianie + "\".");
                    
                }
                
            }

        }

        private static IEnumerable<string> WyszukajPlikiKopiiZapasowych(string sciezka_do_folderu, bool zapisz_na_liscie_jako_element_potencjalnie_stwarzajacy_konflikt = true)
        {
            IEnumerable<string> rezultat = null;

            if (Directory.Exists(sciezka_do_folderu))
            {
                Regex kopiezapasowe_regex = new Regex(@"ORIG.BAK");

                rezultat = Directory.GetFiles(sciezka_do_folderu, "*.*").Where(sciezka => kopiezapasowe_regex.IsMatch(sciezka));


                foreach (string plik in rezultat)
                {
                    if (zapisz_na_liscie_jako_element_potencjalnie_stwarzajacy_konflikt == true)
                    {
                        listasciezek_wykrytekonflikty.Add(plik);
                    }

                    //Console.WriteLine("[DEBUG] plik==" + plik);
                }

            }

            return rezultat;
        }
        private static IEnumerable<string> WyszukajFolderyKopiiZapasowych(string sciezka_do_folderu, bool zapisz_na_liscie_jako_element_potencjalnie_stwarzajacy_konflikt = true)
        {
            IEnumerable<string> rezultat = null;

            if (Directory.Exists(sciezka_do_folderu))
            {
                Regex kopiezapasowe_regex = new Regex(@"ORIG.BAK");

                rezultat = Directory.GetDirectories(sciezka_do_folderu, "*.*").Where(sciezka => kopiezapasowe_regex.IsMatch(sciezka));


                foreach (string folder in rezultat)
                {
                    if (zapisz_na_liscie_jako_element_potencjalnie_stwarzajacy_konflikt == true)
                    {
                        listasciezek_wykrytekonflikty.Add(folder);
                    }

                    //Console.WriteLine("[DEBUG] folder==" + folder);
                }


            }

            return rezultat;
        }

        private static void Main(string[] args)
        {
            if (wersjauzywanegoOS.Nazwa.Contains("Windows") == true)
            { exe_sciezka = exe_sciezka + ".exe"; }
            
            try
            {

                Console.Title = _PWR_PL_naglowek;

                Process[] proces_implementatora = Process.GetProcessesByName("Zaimplementuj_PWR_PL");

                InicjalizujRejestratorLOG();

                ZapiszLOG("Uruchomiono implementator (kompilacja " + _kompilacja + ") w OS: " + wersjauzywanegoOS + " w: " + exe_sciezka);


                if (proces_implementatora.Length <= 1)
                {
                    string separator_naglowka = "+";
                    for (int s1 = 0; s1 < _PWR_PL_naglowek.Length + 2; s1++)
                    {
                        separator_naglowka += "-";
                    }
                    separator_naglowka = separator_naglowka += "+";

                    Console.WriteLine(separator_naglowka);
                    Console.WriteLine("| " + _PWR_PL_naglowek + " |");
                    Console.WriteLine(separator_naglowka);

                    Console.WriteLine("Wersja polonizacji PWR_PL: " + wersja_polonizacji);

                    if (args.Length > 0)
                    {
                        if (args[0] == "-test")
                        {
                            Test();
                        }
                        else if (args[0] == "-deimplementuj")
                        {
                            Deimplementuj_PWR_PL();
                        }
                    }
                    else
                    {
                        Zaimplementuj_PWR_PL();
                    }

                }

            }
            catch (Exception ex)
            {
                ZapiszLOG("Implementator napotkał krytyczny wyjątek i został niespodziewanie zamknięty: " + ex.Message);

                Blad("NIEOCZEKIWANY BŁĄD: Implementator spolszczenia napotkał krytyczny wyjątek i musi zostać zamknięty.");

                Koniec();
                
            }

        }

        private static void Test()
        {

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Test.");


            Console.WriteLine("Environment.OSVersion: " + Environment.OSVersion.ToString());
            Console.WriteLine("Environment.OSVersion.Platform: " + Environment.OSVersion.Platform.ToString());
            Console.WriteLine("Environment.OSVersion.Version.Major: " + Environment.OSVersion.Version.Major);
            Console.WriteLine("Environment.OSVersion.Version.Minor: " + Environment.OSVersion.Version.Minor);
            Console.WriteLine("Environment.OSVersion.Version.Build: " + Environment.OSVersion.Version.Build);
            
            

            Console.WriteLine("WersjaUzywanegoOS(): " + WersjaUzywanegoOS());
            Console.WriteLine("APPDATA: " + APPDATA(""));



            Console.ResetColor();
            Koniec();
        }

        private static void Zaimplementuj_PWR_PL()
        {
            ZapiszLOG("Zainicjalizowano implementację polonizacji: " + wersja_polonizacji + " w OS: " + wersjauzywanegoOS + " w: " + exe_sciezka);

            if
            (
            File.Exists($"..{sc}Wrath_Data{sc}StreamingAssets{sc}Version.info")
            &&
            File.Exists($"..{sc}Wrath.exe")
            &&
            File.Exists($"..{sc}Wrath_Data{sc}sharedassets0.assets")
            &&
            File.Exists($"..{sc}Bundles{sc}ui")
            &&
            Directory.Exists($"..{sc}Wrath_Data{sc}StreamingAssets{sc}Localization{sc}")
            &&
            File.Exists($"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText.json")
            &&
            Directory.Exists($"Implementacja{sc}Wrath_Data{sc}StreamingAssets{sc}Localization{sc}")
            &&
            File.Exists($"Implementacja{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText.json")
            &&
            File.Exists($"Konfiguracja{sc}deDE-default-general_settings.json")
            &&
            File.Exists($"Kompatybilnosc{sc}Version.info")
            )
            {
                string kompatybilnoscspolszczenia_dane = PobierzDaneZVersionInfo($"Kompatybilnosc{sc}Version.info");
                string aktualniezainstalowanawersjagry_dane = PobierzDaneZVersionInfo($"..{sc}Wrath_Data{sc}StreamingAssets{sc}Version.info");

                string kompatybilny_numerwersjigry = kompatybilnoscspolszczenia_dane.Split(new char[] { ' ' })[3];
                string numerzainstalowanejwersjigry = aktualniezainstalowanawersjagry_dane.Split(new char[] { ' ' })[3];

                if (kompatybilnoscspolszczenia_dane == aktualniezainstalowanawersjagry_dane)
                {
                    ZapiszLOG("Kompatybilność wersji polonizacji z wersją gry została potwierdzona (" + wersja_polonizacji + " ---> " + numerzainstalowanejwersjigry + ").");

                    var kopiezapasowe_sharedassets0assets = WyszukajPlikiKopiiZapasowych($"..{sc}Wrath_Data{sc}");
                    var kopiezapasowe_Bundlesui = WyszukajPlikiKopiiZapasowych($"..{sc}Bundles{sc}");
                    var kopiezapasowe_IntroductoryText = WyszukajPlikiKopiiZapasowych($"..{sc}Wrath_Data{sc}StreamingAssets{sc}");

                    var kopiezapasowe_Localization = WyszukajFolderyKopiiZapasowych($"..{sc}Wrath_Data{sc}StreamingAssets{sc}");

                    if (listasciezek_wykrytekonflikty.Count > 0)
                    {
                        string listasciezek_string = "";

                        for (int liwr = 0; liwr < listasciezek_wykrytekonflikty.Count; liwr++)
                        {
                            int np = liwr + 1;

                            listasciezek_string = listasciezek_string + "\n                      " + np.ToString() + ") " + exe_sciezka.Replace($"{sc}Zaimplementuj_PWR_PL", "").Replace(".dll", "").Replace(".exe", "") + sc + listasciezek_wykrytekonflikty[liwr];
                        }

                        ZapiszLOG("Znaleziono następujące elementy kopii zapasowych gry:" + listasciezek_string);
                    }

                    /*
                    for (int il1 = 0; il1 < listasciezek_wykrytekonflikty.Count; il1++)
                    {
                        Console.WriteLine("[DEBUG] listasciezek_wykrytekonflikty[" + il1 + "]==" + listasciezek_wykrytekonflikty[il1]);
                    }
                    */


                    if (listasciezek_wykrytekonflikty.Count == 0)
                    {
                        Console.WriteLine("Trwa implementacja spolszczenia...");
                        Console.WriteLine("Nie zamykaj tego okna i poczekaj, aż wyświetlą się kolejne informacje. Może to trochę potrwać...");

                        if (File.Exists($"Implementacja{sc}Wrath_data{sc}sharedassets0.assets") == true)
                        {
                            if (File.Exists($"..{sc}Wrath_Data{sc}sharedassets0.assets.ORIG.BAK-" + kompatybilny_numerwersjigry) == false)
                            {
                                File.Move($"..{sc}Wrath_Data{sc}sharedassets0.assets", $"..{sc}Wrath_Data{sc}sharedassets0.assets.ORIG.BAK-" + kompatybilny_numerwersjigry);
                            }
                        }

                        string nazwaplikupatchera = "xdelta3";

                        if (wersjauzywanegoOS.Nazwa.Contains("Windows") == true)
                        {
                            nazwaplikupatchera = nazwaplikupatchera + ".exe"; }
                        
                        
                        if
                        (
                            Directory.Exists($"Implementacja{sc}bundle-ui{sc}") == true
                            &&
                            File.Exists($"Implementacja{sc}bundle-ui{sc}{nazwaplikupatchera}") == true
                            &&
                            File.Exists($"Implementacja{sc}bundle-ui{sc}pwr_pl-ui.patch") == true
                        )
                        {
                            if (File.Exists($"..{sc}Bundles{sc}ui.PWR_PL") == true)
                            {
                                File.Delete($"..{sc}Bundles{sc}ui.PWR_PL");
                            }

                            ProcessStartInfo xdelta3_startInfo = new ProcessStartInfo();
                            xdelta3_startInfo.CreateNoWindow = false;
                            xdelta3_startInfo.UseShellExecute = false;
                            xdelta3_startInfo.FileName = $"Implementacja{sc}bundle-ui{sc}{nazwaplikupatchera}";
                            xdelta3_startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            xdelta3_startInfo.Arguments = $"-d -s ..{sc}Bundles{sc}ui Implementacja{sc}bundle-ui{sc}pwr_pl-ui.patch ..{sc}Bundles{sc}ui.PWR_PL";



                            try
                            {
                                using (Process xdelta3_proces = Process.Start(xdelta3_startInfo))
                                {
                                    xdelta3_proces.WaitForExit();
                                }
                            }
                            catch
                            {
                                Blad("BŁĄD: Wystąpił nieoczekiwany problem z dostępem do patchera. Spróbuj uruchomić instalator spolszczenia z uprawnieniami Administratora.");
                            }

                            if (File.Exists($"..{sc}Bundles{sc}ui.PWR_PL") == true)
                            {
                                if (File.Exists($"..{sc}Bundles{sc}ui.ORIG.BAK-" + kompatybilny_numerwersjigry) == false)
                                {
                                    File.Move($"..{sc}Bundles{sc}ui", $"..{sc}Bundles{sc}ui.ORIG.BAK-" + kompatybilny_numerwersjigry);
                                }

                                File.Move($"..{sc}Bundles{sc}ui.PWR_PL", $"..{sc}Bundles{sc}ui");
                            }

                        }


                        if (Directory.Exists($"..{sc}Wrath_Data{sc}StreamingAssets{sc}Localization.ORIG.BAK-" + kompatybilny_numerwersjigry + sc) == false)
                        {
                            SkopiujFolderWrazZZawartoscia($"..{sc}Wrath_Data{sc}StreamingAssets{sc}Localization{sc}", $"..{sc}Wrath_Data{sc}StreamingAssets{sc}Localization.ORIG.BAK-" + kompatybilny_numerwersjigry + sc);
                        }

                        if (File.Exists($"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText.json.ORIG.BAK-" + kompatybilny_numerwersjigry) == false)
                        {
                            File.Move($"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText.json", $"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText.json.ORIG.BAK-" + kompatybilny_numerwersjigry);
                        }

                        if (File.Exists($"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText_Steam.json") == true)
                        {
                            if (File.Exists($"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText_Steam.json.ORIG.BAK-" + kompatybilny_numerwersjigry) == false)
                            {
                                File.Move($"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText_Steam.json", $"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText_Steam.json.ORIG.BAK-" + kompatybilny_numerwersjigry);
                            }
                        }


                        SkopiujFolderWrazZZawartoscia($"Implementacja{sc}Wrath_Data{sc}StreamingAssets{sc}Localization{sc}", $"..{sc}Wrath_Data{sc}StreamingAssets{sc}Localization{sc}");

                        File.Copy($"Implementacja{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText.json", $"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText.json");

                        if (File.Exists($"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText_Steam.json.ORIG.BAK-" + kompatybilny_numerwersjigry) == true)
                        {
                            File.Copy($"Implementacja{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText.json", $"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText_Steam.json");
                        }

                        if (File.Exists(APPDATA($"..{sc}LocalLow{sc}Owlcat Games{sc}Pathfinder Wrath Of The Righteous{sc}general_settings.json")) == true)
                        {
                            ZmienJezykWPlikuKonfiguracyjnymGry("enGB", "deDE");
                        }
                        else
                        {
                            if (Directory.Exists(APPDATA($"..{sc}LocalLow{sc}Owlcat Games{sc}")) == false) { Directory.CreateDirectory(APPDATA($"..{sc}LocalLow{sc}Owlcat Games{sc}")); }
                            if (Directory.Exists(APPDATA($"..{sc}LocalLow{sc}Owlcat Games{sc}Pathfinder Wrath Of The Righteous{sc}")) == false) { Directory.CreateDirectory(APPDATA($"..{sc}LocalLow{sc}Owlcat Games{sc}Pathfinder Wrath Of The Righteous{sc}")); }
                            File.Copy($"Konfiguracja{sc}deDE-default-general_settings.json", APPDATA($"..{sc}LocalLow{sc}Owlcat Games{sc}Pathfinder Wrath Of The Righteous{sc}general_settings.json"));
                        }


                        Sukces("Zaimplementowano polonizację " + wersja_polonizacji + " do gry Pathfinder Wrath of the Righteous v." + kompatybilny_numerwersjigry + ".");
                        Informacja("Jeśli po instalacji spolszczenia gra uruchamia się w języku angielskim to aby aktywować język polski, w opcjach gry przełącz na niemiecki wchodząc w Options/Language/\"Deutsch\"/Accept.");


                    }
                    else
                    {

                        if (File.Exists($"..{sc}Wrath_Data{sc}sharedassets0.assets")) { File.Delete($"..{sc}Wrath_Data{sc}sharedassets0.assets"); }
                        if (File.Exists($"..{sc}Bundles{sc}ui")) { File.Delete($"..{sc}Bundles{sc}ui"); }
                        if (File.Exists($"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText.json"))
                        {
                            File.Delete($"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText.json");

                            if (File.Exists($"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText_Steam.json") == true)
                            {
                                File.Delete($"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText_Steam.json");
                            }
                        }

                        if (Directory.Exists($"..{sc}Wrath_Data{sc}StreamingAssets{sc}Localization{sc}")) { Directory.Delete($"..{sc}Wrath_Data{sc}StreamingAssets{sc}Localization{sc}", true); }


                        for (int il2 = 0; il2 < listasciezek_wykrytekonflikty.Count; il2++)
                        {
                            if (File.Exists(listasciezek_wykrytekonflikty[il2])) { File.Delete(listasciezek_wykrytekonflikty[il2]); }
                            if (Directory.Exists(listasciezek_wykrytekonflikty[il2])) { Directory.Delete(listasciezek_wykrytekonflikty[il2], true); }
                        }


                        Blad("Nie można zainstalować spolszczenia, ponieważ wykryto błędy w integralności plików gry. Istnieją pliki i/lub foldery potencjalnie stwarzające konflikty.");
                        Informacja("Powyższy błąd może wynikać z faktu wielokrotnych prób instalacji spolszczenia niewłaściwą metodą nałożenia jednej instalacji na drugą (zawsze przed instalacją innej wersji polonizacji należy najpierw odinstalować poprzednią).");
                        Informacja("Pliki/foldery stwarzające konflikty zostały teraz automatycznie usunięte przez implementator spolszczenia, natomiast koniecznie sprawdź spójność plików gry w Steam/GoG/Epic przed kolejną próbą uruchomienia gry lub ponowną instalacją polonizacji.");
                    }

                }
                else
                {
                    ZapiszLOG("Instalowana wersja polonizacji (" + wersja_polonizacji + ") jest przeznaczona dla wersji gry " + kompatybilny_numerwersjigry + " i NIE JEST kompatybilna z wersją zainstalowanej gry (" + numerzainstalowanejwersjigry + ").");

                    Blad("BŁĄD: Nie można zainstalować spolszczenia, ponieważ wystąpiła niezgodność wersji polonizacji z zainstalowaną wersją gry.");
                    Informacja("Upewnij się, że instalujesz wersję polonizacji zgodną z aktualnie zainstalowaną wersją gry.");
                    Blad("Wersja spolszczenia, którą próbujesz zainstalować jest przeznaczona dla wersji gry: " + kompatybilny_numerwersjigry);
                    Blad("Posiadasz zainstalowaną wersję gry: " + numerzainstalowanejwersjigry);

                }

            }
            else
            {
                ZapiszLOG("Folder PWR_PL został umieszczony przed próbą implementacji w niewłaściwym miejscu lub brakuje istotnych plików gry (ścieżka zainicjowanego implementatora: " + exe_sciezka + ").");
                Blad("BŁĄD: Weryfikacja plików gry nie powiodła się. Upewnij się, że folder \"PWR_PL\" wraz całą zawartością znajduje się w głównym folderze z zainstalowaną grą Pathfinder Wrath of the Righteous. Jeśli tak jest, a mimo tego wyświetla się ten błąd, wtedy sprawdź spójność plików gry w Steam/GoG/Epic, a nastepnie spróbuj ponownie zainstalować spolszczenie.");
            }

            Koniec();
        }

        private static void Deimplementuj_PWR_PL()
        {
            ZapiszLOG("Zainicjalizowano deimplementację polonizacji: " + wersja_polonizacji + " w OS: " + wersjauzywanegoOS + "w: " + exe_sciezka);

            int ilosc_wykrytychbrakujacychelementowORIGBAKdlaTEJWERSJIGRY = 0;

            if
            (
            File.Exists($"..{sc}Wrath_Data{sc}StreamingAssets{sc}Version.info")
            &&
            File.Exists($"..{sc}Wrath.exe")
            &&
            File.Exists($"..{sc}Wrath_Data{sc}sharedassets0.assets")
            &&
            File.Exists($"..{sc}Bundles{sc}ui")
            &&
            Directory.Exists($"..{sc}Wrath_Data{sc}StreamingAssets{sc}Localization{sc}")
            &&
            File.Exists($"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText.json")
            &&
            Directory.Exists($"Implementacja{sc}Wrath_Data{sc}StreamingAssets{sc}Localization{sc}")
            &&
            File.Exists($"Implementacja{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText.json")
            &&
            File.Exists($"Konfiguracja{sc}deDE-default-general_settings.json")
            &&
            File.Exists($"Kompatybilnosc{sc}Version.info")
            )
            {

                string kompatybilnoscspolszczenia_dane = PobierzDaneZVersionInfo($"Kompatybilnosc{sc}Version.info");
                string aktualniezainstalowanawersjagry_dane = PobierzDaneZVersionInfo($"..{sc}Wrath_Data{sc}StreamingAssets{sc}Version.info");

                string kompatybilny_numerwersjigry = kompatybilnoscspolszczenia_dane.Split(new char[] { ' ' })[3];
                string numerzainstalowanejwersjigry = aktualniezainstalowanawersjagry_dane.Split(new char[] { ' ' })[3];

                var kopiezapasowe_sharedassets0assets = WyszukajPlikiKopiiZapasowych($"..{sc}Wrath_Data{sc}");
                var kopiezapasowe_Bundlesui = WyszukajPlikiKopiiZapasowych($"..{sc}Bundles{sc}");
                var kopiezapasowe_IntroductoryText = WyszukajPlikiKopiiZapasowych($"..{sc}Wrath_Data{sc}StreamingAssets{sc}");

                var kopiezapasowe_Localization = WyszukajFolderyKopiiZapasowych($"..{sc}Wrath_Data{sc}StreamingAssets{sc}");

                if (listasciezek_wykrytekonflikty.Count > 0)
                {
                    string listasciezek_string = "";

                    for (int liwr = 0; liwr < listasciezek_wykrytekonflikty.Count; liwr++)
                    {
                        int np = liwr + 1;

                        listasciezek_string = listasciezek_string + "\n                      " + np.ToString() + ") " + exe_sciezka.Replace($"{sc}Zaimplementuj_PWR_PL", "").Replace(".dll", "").Replace(".exe", "") + sc + listasciezek_wykrytekonflikty[liwr];
                    }

                    ZapiszLOG("Znaleziono następujące elementy kopii zapasowych gry:" + listasciezek_string);
                }

                /*
                for (int il1 = 0; il1 < listasciezek_wykrytekonflikty.Count; il1++)
                {
                    Console.WriteLine("[DEBUG] listasciezek_wykrytekonflikty[" + il1 + "]==" + listasciezek_wykrytekonflikty[il1]);
                }
                */


                if (kompatybilnoscspolszczenia_dane == aktualniezainstalowanawersjagry_dane)
                {
                    ZapiszLOG("Kompatybilność wersji polonizacji z wersją gry została potwierdzona (" + wersja_polonizacji + " ---> " + numerzainstalowanejwersjigry + ").");
                    ZapiszLOG("Wykryto użycie zgodnego/prawidłowego implementatora w celu deimplementacji spolszczenia z zainstalowanej gry.");

                    Console.WriteLine("Trwa usuwanie Polskiej Lokalizacji PWR_PL: " + PobierzNumerWersjiPolonizacji() + "");
                    Console.WriteLine("Nie zamykaj tego okna i poczekaj, aż wyświetlą się kolejne informacje. Może to trochę potrwać...");

                    if (File.Exists($"Implementacja{sc}Wrath_Data{sc}sharedassets0.assets") == true)
                    {
                        if (File.Exists($"..{sc}Wrath_Data{sc}sharedassets0.assets") == true)
                        {
                            File.Delete($"..{sc}Wrath_Data{sc}sharedassets0.assets");
                        }

                        if (File.Exists($"..{sc}Wrath_Data{sc}sharedassets0.assets.ORIG.BAK-" + kompatybilny_numerwersjigry) == true)
                        {
                            File.Move($"..{sc}Wrath_Data{sc}sharedassets0.assets.ORIG.BAK-" + kompatybilny_numerwersjigry, $"..{sc}Wrath_Data{sc}sharedassets0.assets");
                        }
                        else
                        {
                            ZapiszLOG("Wykryto brak kopii zapasowej oryginalnego pliku gry, który deimplementator chciał automatycznie przywrócić z: " + exe_sciezka.Replace($"{sc}Zaimplementuj_PWR_PL", "").Replace(".dll", "").Replace(".exe", "") + $"{sc}..{sc}Wrath_Data{sc}sharedassets0.assets.ORIG.BAK-" + kompatybilny_numerwersjigry + ".");

                            ilosc_wykrytychbrakujacychelementowORIGBAKdlaTEJWERSJIGRY++;
                        }
                    }


                    if (File.Exists($"Implementacja{sc}bundle-ui{sc}pwr_pl-ui.patch") == true)
                    {
                        if (File.Exists($"..{sc}Bundles{sc}ui") == true)
                        {
                            File.Delete($"..{sc}Bundles{sc}ui");
                        }


                        if (File.Exists($"..{sc}Bundles{sc}ui.ORIG.BAK-" + kompatybilny_numerwersjigry) == true)
                        {
                            File.Move($"..{sc}Bundles{sc}ui.ORIG.BAK-" + kompatybilny_numerwersjigry, $"..{sc}Bundles{sc}ui");
                        }
                        else
                        {
                            ZapiszLOG("Wykryto brak kopii zapasowej oryginalnego pliku gry, który deimplementator chciał automatycznie przywrócić z: " + exe_sciezka.Replace($"{sc}Zaimplementuj_PWR_PL", "").Replace(".dll", "").Replace(".exe", "") + $"{sc}..{sc}Bundles{sc}ui.ORIG.BAK-" + kompatybilny_numerwersjigry + ".");

                            ilosc_wykrytychbrakujacychelementowORIGBAKdlaTEJWERSJIGRY++;
                        }
                    }


                    //if (File.Exists($"Implementacja{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText.json") == true) /* WARUNEK IF W TYM PRZYPADKU NIE OBOWIĄZUJE, ponieważ plik "IntroductoryText.json" zawsze musi znajdować się w builderze instalatora */
                    {
                        if (File.Exists($"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText.json") == true)
                        {
                            File.Delete($"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText.json");
                        }

                        if (File.Exists($"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText_Steam.json") == true)
                        {
                            File.Delete($"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText_Steam.json");
                        }


                        if (File.Exists($"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText.json.ORIG.BAK-" + kompatybilny_numerwersjigry) == true)
                        {
                            File.Move($"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText.json.ORIG.BAK-" + kompatybilny_numerwersjigry, $"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText.json");
                        }
                        else
                        {
                            ZapiszLOG("Wykryto brak kopii zapasowej oryginalnego pliku gry, który deimplementator chciał automatycznie przywrócić z: " + exe_sciezka.Replace($"{sc}Zaimplementuj_PWR_PL", "").Replace(".dll", "").Replace(".exe", "") + $"{sc}..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText.json.ORIG.BAK-" + kompatybilny_numerwersjigry + ".");

                            ilosc_wykrytychbrakujacychelementowORIGBAKdlaTEJWERSJIGRY++;
                        }

                        if (File.Exists($"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText_Steam.json.ORIG.BAK-" + kompatybilny_numerwersjigry) == true)
                        {
                            File.Move($"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText_Steam.json.ORIG.BAK-" + kompatybilny_numerwersjigry, $"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText_Steam.json");
                        }
                        else
                        {
                            ZapiszLOG("Wykryto brak kopii zapasowej oryginalnego pliku gry, który deimplementator chciał automatycznie przywrócić z: " + exe_sciezka.Replace($"{sc}Zaimplementuj_PWR_PL", "").Replace(".dll", "").Replace(".exe", "") + $"{sc}..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText_Steam.json.ORIG.BAK-" + kompatybilny_numerwersjigry + ".");

                            ilosc_wykrytychbrakujacychelementowORIGBAKdlaTEJWERSJIGRY++;
                        }
                    }


                    //if (Directory.Exists($"Implementacja{sc}Wrath_Data{sc}StreamingAssets{sc}Localization{sc}") == true) /* WARUNEK IF W TYM PRZYPADKU NIE OBOWIĄZUJE, ponieważ folder "Localization" zawsze musi znajdować się w builderze instalatora */
                    {
                        if (Directory.Exists($"..{sc}Wrath_Data{sc}StreamingAssets{sc}Localization{sc}") == true)
                        {
                            Directory.Delete($"..{sc}Wrath_Data{sc}StreamingAssets{sc}Localization{sc}", true);
                        }


                        if (Directory.Exists($"..{sc}Wrath_Data{sc}StreamingAssets{sc}Localization.ORIG.BAK-" + kompatybilny_numerwersjigry) == true)
                        {
                            Directory.Move($"..{sc}Wrath_Data{sc}StreamingAssets{sc}Localization.ORIG.BAK-" + kompatybilny_numerwersjigry, $"..{sc}Wrath_Data{sc}StreamingAssets{sc}Localization");
                        }
                        else
                        {
                            ZapiszLOG("Wykryto brak kopii zapasowej oryginalnego folderu gry z zawartością, który deimplementator chciał automatycznie przywrócić z: " + exe_sciezka.Replace($"{sc}Zaimplementuj_PWR_PL", "").Replace(".dll", "").Replace(".exe", "") + $"{sc}..{sc}Wrath_Data{sc}StreamingAssets{sc}Localization.ORIG.BAK-" + kompatybilny_numerwersjigry + ".");

                            ilosc_wykrytychbrakujacychelementowORIGBAKdlaTEJWERSJIGRY++;
                        }
                    }

                }
                else
                { /*---*/
                    ZapiszLOG("Użyta wersja polonizacji (" + wersja_polonizacji + ") w celu deimplementacji NIE JEST kompatybilna z aktualnie zainstalowaną wersją gry (" + numerzainstalowanejwersjigry + ").");
                    ZapiszLOG("Wykryto użycie NIEzgodnego/NIE tego samego implementatora w celu deimplementacji spolszczenia z zainstalowanej gry.");


                    if (File.Exists($"..{sc}Wrath_Data{sc}sharedassets0.assets")) { File.Delete($"..{sc}Wrath_Data{sc}sharedassets0.assets"); }
                    if (File.Exists($"..{sc}Bundles{sc}ui")) { File.Delete($"..{sc}Bundles{sc}ui"); }
                    if (File.Exists($"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText.json")) { File.Delete($"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText.json"); }
                    if (File.Exists($"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText_Steam.json")) { File.Delete($"..{sc}Wrath_Data{sc}StreamingAssets{sc}IntroductoryText_Steam.json"); }

                    if (Directory.Exists($"..{sc}Wrath_Data{sc}StreamingAssets{sc}Localization{sc}")) { Directory.Delete($"..{sc}Wrath_Data{sc}StreamingAssets{sc}Localization{sc}", true); }


                    for (int il2 = 0; il2 < listasciezek_wykrytekonflikty.Count; il2++)
                    {
                        if (File.Exists(listasciezek_wykrytekonflikty[il2])) { File.Delete(listasciezek_wykrytekonflikty[il2]); }
                        if (Directory.Exists(listasciezek_wykrytekonflikty[il2])) { Directory.Delete(listasciezek_wykrytekonflikty[il2], true); }
                    }


                    Blad("Wykryto błędy w integralności plików gry. Istnieją pliki i/lub foldery potencjalnie stwarzające konflikty.");
                    Informacja("Powyższy błąd może wynikać z faktu próby użycia deinstalatora nie z tej wersji spolszczenia, która aktualnie była zainstalowana.");
                    Informacja("Pliki/foldery stwarzające konflikty zostały teraz automatycznie usunięte przez deimplementator spolszczenia, natomiast koniecznie sprawdź spójność plików gry w Steam/GoG/Epic przed kolejną próbą uruchomienia gry lub ponowną instalacją polonizacji.");

                }

                ZmienJezykWPlikuKonfiguracyjnymGry("deDE", "enGB");

                if (ilosc_wykrytychbrakujacychelementowORIGBAKdlaTEJWERSJIGRY == 0)
                {
                    Sukces("Polska Lokalizacja PWR " + PobierzNumerWersjiPolonizacji() + " została pomyślnie odinstalowana z gry Pathfinder Wrath of the Righteous " + kompatybilny_numerwersjigry + ".");
                }
                else
                {
                    Informacja("Polska Lokalizacja PWR " + PobierzNumerWersjiPolonizacji() + " została usunięta z gry Pathfinder Wrath of the Righteous " + kompatybilny_numerwersjigry + ", natomiast deimplementator spolszczenia napotkał przynajmniej jeden krytyczny wyjątek i wyniku tego nie zdołał przywrócić wszystkich plików gry do oryginalnego stanu.");
                    Blad("Przed próbą uruchomienia gry koniecznie sprawdź spójność plików gry w Steam/GoG/Epic.");
                }


                if (File.Exists("unins000.exe") && File.Exists("unins000.dat"))
                {
                    Process[] procesy_nazwa = Process.GetProcessesByName("unins000");

                    if (procesy_nazwa.Length <= 1)
                    {
                        ProcessStartInfo unins000_startInfo = new ProcessStartInfo();
                        unins000_startInfo.CreateNoWindow = false;
                        unins000_startInfo.UseShellExecute = false;
                        unins000_startInfo.FileName = "unins000.exe";
                        unins000_startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        //unins000_startInfo.Arguments = "";



                        try
                        {
                            using (Process unins000_proces = Process.Start(unins000_startInfo))
                            {
                                ZapiszLOG("Automatycznie uruchomiono proces aplikacji odinstalowującej polonizację (unins000.exe).");

                                //unins000_proces.WaitForExit();
                            }
                        }
                        catch
                        {
                            Blad("BŁĄD: Wystąpił nieoczekiwany problem z dostępem do aplikacji odinstalowującej (unins000.exe). Spróbuj uruchomić plik \"unins000.exe\" znajdujący się w folderze \"PWR_PL\" z uprawnieniami Administratora.");
                        }

                    }
                }


            }
            else
            {
                ZapiszLOG("Folder PWR_PL został umieszczony przed próbą implementacji w niewłaściwym miejscu lub brakuje istotnych plików gry (ścieżka zainicjowanego implementatora: " + exe_sciezka + ").");
                Blad("BŁĄD: Weryfikacja plików gry nie powiodła się. Upewnij się, że folder \"PWR_PL\" wraz całą zawartością znajduje się w głównym folderze z zainstalowaną grą Pathfinder Wrath of the Righteous. Jeśli tak jest, a mimo tego wyświetla się ten błąd, wtedy sprawdź spójność plików gry w Steam/GoG/Epic.");
            }


            Koniec();

        }

    }

}