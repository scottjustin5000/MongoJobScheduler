using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Scheduling
{
    
   /// <summary>
   /// Provides methods for creating schedule objects.
   /// </summary>
   /// <example>
   /// To create an instance of the <see cref="ScheduleSection"/>
   /// defined in the application configuration file (though using 
   /// <see cref="ScheduleManager.Configuration"/> property is 
   /// probably a better idea):
   /// <code>
   /// ScheduleSection section = ScheduleFactory.CreateSection();
   /// </code>
   /// <br/>
   /// To create a schedule named "ExampleSchedule" that is 
   /// defined in the application configuration file, use the following
   /// template. Note that we are using the generic schedule 
   /// because we do not know what the specific type of the schedule.
   /// This example assumes there is a schedule named "ExampleSchedule"
   /// defined in the application configuration file.
   /// <code>
   /// ScheduleBase schedule = ScheduleFactory.CreateSchedule("ExampleSchedule");
   /// </code>
   /// <br/>
   /// When creating a schedule by specifying it's type name, it is allowable
   /// to cast the returned schedule to the expected type. This is because we
   /// already know the specific type of the schedule. This example assumes 
   /// that there is a Crc32 schedule (or a schedule derived
   /// from Crc32Schedule) named "Crc32" defined in the application 
   /// configuration file or that there is no schedule named "Crc32" defined
   /// in the application configuration file.
   /// <code>
   /// Crc32Schedule schedule = (Crc32Schedule)ScheduleFactory.CreateSchedule("Crc32");
   /// </code>
   /// </example>
   public static class ScheduleFactory
   {
      /// <summary>
      /// Creates an instance of the scheduling section defined in a provider
      /// specific configuration file or the application configuration file. 
      /// The provider specific configuration file must live in the application
      /// directory and be named "RcScheduling.config" or "RcScheduling.dll.config".
      /// </summary>
      /// <remarks>
      /// Each time this method is called, a new instance of the scheduling section
      /// is created from the application configuration file. Because this is a 
      /// rather expensive operation, a singleton of type <see cref="ScheduleSection"/>
      /// is provided in the static <see cref="ScheduleManager.Configuration"/>
      /// property. It is preferable for client code to use this singleton property
      /// instead of calling this method repeatedly.
      /// </remarks>
      /// 


      /// <summary>
      /// Creates an instance of the scheduling section defined in the application 
      /// configuration file.
      /// </summary>
      /// <remarks>
      /// Each time this method is called, a new instance of the scheduling section
      /// is created from the application configuration file. Because this is a 
      /// rather expensive operation, a singleton of type <see cref="ScheduleSection"/>
      /// is provided in the static <see cref="ScheduleManager.Configuration"/>
      /// property. It is preferable for client code to use this singleton property
      /// instead of calling this method repeatedly.
      /// </remarks>
      /// <exception cref="ArgumentNullException">
      /// Thrown if fileName is null.
      /// </exception>
      /// <exception cref="ArgumentException">
      /// Thrown if fileName is empty.
      /// </exception>
      /// <exception cref="System.IO.FileNotFoundException">
      /// Thrown if the file named fileName does not exist.
      /// </exception>

      /// <summary>
      /// Creates a schedule based on the specified settings. 
      /// </summary>
      /// <exception cref="ArgumentNullException">
      /// Thrown if scheduleSettings is null.
      /// </exception>
      /// 
 
      public static ScheduleBase CreateSchedule (ScheduleSettings scheduleSettings)
      {
         if (scheduleSettings == null)
         {
            throw new ArgumentNullException("scheduleSettings");
         }
         
          Type type = Type.GetType(scheduleSettings.Type, false, true);
          ScheduleBase schedule = (ScheduleBase)Activator.CreateInstance(type);
        
         if (schedule != null)
         {
            schedule.Configure(scheduleSettings, false);
         }
         else
         {
             throw new Exception("Unable to Create Schedule Settings");
         }

         return schedule;
      }

      /// <summary>
      /// Creates and returns an instance of a schedule.
      /// </summary>
      /// <remarks>
      /// The schedule name can be the name of a schedule defined in 
      /// the application configuration file, a well-known schedule 
      /// such as "Adler32", "Crc32", or "CrcX", a schedule that abides 
      /// by the standard naming convention of 
      /// "RcScheduling.{scheduleName}.{scheduleName}ScheduleBase"
      /// defined in an assembly named "RcScheduling.{scheduleName}"
      /// residing in the application directory or global assembly cache,
      /// or the type name of a schedule with the assembly of the schedule
      /// (e.g. "CustomSchedule.CustomSchedule, CustomSchedule").
      /// </remarks>
      /// <param name="scheduleName">
      /// The name of a schedule defined in the application configuration 
      /// file, a well-known schedule such as "Adler32", "Crc32", or "CrcX", 
      /// a schedule that abides by the standard naming convention of 
      /// "RcScheduling.{scheduleName}.{scheduleName}ScheduleBase"
      /// defined in an assembly named "RcScheduling.{scheduleName}"
      /// residing in the application directory or global assembly cache,
      /// or the type name of a schedule with the assembly of the schedule
      /// (e.g. "CustomSchedule.CustomSchedule, CustomSchedule").
      /// </param>
      /// <exception cref="ArgumentNullException">
      /// Thrown if scheduleName is null.
      /// </exception>
      /// <exception cref="ArgumentException">
      /// Thrown if scheduleName is empty.
      /// </exception>
      public static ScheduleBase CreateSchedule (string scheduleName)
      {
         if (scheduleName == null)
         {
            throw new ArgumentNullException("scheduleName");
         }

         ScheduleSettings scheduleSettings = null;

         if (ScheduleManager.Configuration != null &&
            ScheduleManager.Configuration.ScheduleSettings != null)
         {

             if (ScheduleManager.Configuration.Contains(scheduleName))
             {
                 scheduleSettings = ScheduleManager.Configuration.GetByName(scheduleName);
             }
             else
             {
                 string scheduleType = ScheduleManager.Configuration.TypeOf(scheduleName);

                 if (scheduleType == null)
                 {
                     // this was not a standard schedule, so if the name has any periods in it,
                     // then we assume the name is actually a type name. Otherwise we assume the
                     // name is in the standard format for schedules.
                     if (scheduleName.IndexOfAny(new char[] { ',', '.' }) != -1)
                     {
                         scheduleType = scheduleName;
                     }
                     else
                     {
                         scheduleType = string.Format("Infrastructure.Scheduling.{0}Schedule, Infrastructure.Scheduling.{0}", scheduleName);
                             //string.Format("Infrastructure.Scheduling.{0}.{0}Schedule, Infrastructure.Scheduling.{0}", scheduleName);
                     }
                 }

                 scheduleSettings = new ScheduleSettings();
                 scheduleSettings.Name = scheduleName;
                 scheduleSettings.Type = scheduleType;
             }
         }
         else
         {
            ScheduleSection section = new ScheduleSection();
            string scheduleType= section.TypeOf(scheduleName);

            if (scheduleType != null)
            {
               scheduleSettings = new ScheduleSettings();
               scheduleSettings.Name = scheduleName;
               scheduleSettings.Type = scheduleType;
            }
         }

         ScheduleBase schedule = null;

         if (scheduleSettings != null)
         {
            Type type = Type.GetType(scheduleSettings.Type, false, true);
            schedule = (ScheduleBase)Activator.CreateInstance(type);

            if (schedule != null)
            {
               schedule.Configure(scheduleSettings, false);
            }
         }

         if (schedule == null)
         {
             throw new Exception(string.Format("Unable to create {0}",scheduleName));
         }

         return schedule;
      }
   }
}

