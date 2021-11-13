using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;


namespace SaveDateTime
{
  class Program
  {
    static void Main(string[] args)
    {
       bool firstChoice = false;
       Console.WriteLine("Would you like to save the current time and date (Y / N) ?");
       while (!firstChoice)
       {
         var doSave = Console.ReadKey().Key;

         if (doSave == ConsoleKey.Y)
         {
           SaveDateTimeData();
           Console.Clear();
           Console.WriteLine("Date and Time data was saved...\nPress any key to display saved date and time...\n");
           firstChoice = true;
         }
         else if (doSave == ConsoleKey.N)
         {
           Console.Clear();
           Console.WriteLine("\nPress any key to display saved date and time...");
           firstChoice = true;
         }
         else
         {
           firstChoice = false;
           Console.Clear();
           Console.WriteLine("Would you like to save the current time and date (Y / N) ?");
           continue;
         }
       }



       Console.ReadKey();

       var savedDateTime = SaveSystem.GetDateTimeData();

       if (savedDateTime != null)
       {
         Console.Clear();

         // Current
         Console.WriteLine(CurrentTime + CurrentDate + "\n");

         // Saved time
         Console.WriteLine(SavedTime(savedDateTime) + SavedDate(savedDateTime));

         //Console.WriteLine();

       }
       else { Console.WriteLine("The save data was not located!"); }

      EndOfProgram();

    }
    static void EndOfProgram()
    {
      SetForegroundColor(ConsoleColor.Red);
      Console.WriteLine("Press any key to exit...");
      Console.ReadKey();
    }
    static string CurrentTime
    {
      get
      {
        var format = DateTime.Now.Hour > 9 ? "{0:00}" : "{0:0}";
        var meridian = DateTime.Now.Hour <= 11 ? "AM" : "PM";
        var standardConversion = DateTime.Now.Hour > 12 ? DateTime.Now.Hour - 12 : DateTime.Now.Hour;
        var fixedHour = string.Format(format, standardConversion);
        var minute = string.Format("{0:00}", DateTime.Now.Minute);
        var mTime = DateTime.Now.Hour + ":" + DateTime.Now.Minute;

        return "\nCurrent time (12H): \n" + fixedHour + ":" + minute + " " + meridian + "\n\n" + "Current time (24H): \n" +
          mTime + "\n";
      }
    }
    static string CurrentDate
    {
      get
      {
        return "\n" + "Current date: \n" + DateTime.Now.Date.ToShortDateString();
      }
    }
    static string SavedTime(SDateTime data)
    {
      var hour = data.hour;
      var minute = data.minute;
      var fixedTime = hour > 12 ? hour - 12 : hour;  // convert 24H time to 12H time
      var format = hour > 9 ? "{0:00}" : "{0:0}";    // determine hour format
      var meridian = hour <= 11 ? "AM" : "PM";      // determine meridian
      var time = "Saved time (12H):\n" + string.Format(format, fixedTime) + ":" + string.Format("{0:00}", minute) + " " + meridian + "\n";
      var mTime = "\nSaved time (24H): \n" + string.Format(format, hour) + ":" + string.Format("{0:00}", minute) + "\n";

      return time + mTime;

    }
    static string SavedDate(SDateTime data)
    {
      return "\nSaved date: \n" + data.date;
    }
    static void SetForegroundColor(ConsoleColor newColor)
    {
      Console.ForegroundColor = newColor;
    }
    static void SaveDateTimeData()
    {
      var currentHour = DateTime.Now.Hour;
      var currentMinute = DateTime.Now.Minute;
      var currentDate = DateTime.Now.ToShortDateString();

      SaveSystem.SaveDateTime(new SDateTime(currentHour, currentMinute, currentDate));

    }
  } // END OF CLASS
  public static class SaveSystem
  {
    public static string SavePath
    {
      get
      {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "savedDate.json");
      }
    }

    public static void SaveDateTime(SDateTime data)
    {
      var jsonWriter = new DataContractJsonSerializer(typeof(SDateTime));
      var stream = new FileStream(SavePath, FileMode.Create);

      try
      {
        jsonWriter.WriteObject(stream, data);
      }
      catch (SerializationException e)
      {
        Console.WriteLine("Serialization failed, Reason: " + e.Message);
        throw;
      }
      finally
      {
        stream.Close();
      }

    }
    public static SDateTime GetDateTimeData()
    {
      var stream = new FileStream(SavePath, FileMode.Open);

      try
      {
        var jsonReader = new DataContractJsonSerializer(typeof(SDateTime));
        var data = (SDateTime)jsonReader.ReadObject(stream);
        return data;
      }
      catch (SerializationException e)
      {
        Console.WriteLine("Deserialization failed, Reason: " + e.Message);
        throw;
      }
      finally
      {
        stream.Close();
      }
    }

    public static bool SaveExists { get { return File.Exists(SavePath); } }


  } // END OF CLASS

  public static class Extensions
  {
    // Extension of string to determine if a string is empty ("" or " ")
    // all strings are char arrays, if string.Length == 0, string is empty
    public static bool IsEmptyString(this string s)
    {
      return s.Length == 0 || s == " ";
    }
  }
  [Serializable]
  public class SDateTime
  {
    public int hour, minute;
    public string date;

    public SDateTime(int h, int m, string d)
    {
      hour = h;
      minute = m;

      if (!date.IsEmptyString())
        date = d;
      else { Console.WriteLine("Date was empty when saving data!"); }
    }
  }// END OF CLASS


}// END NAMESPACE
