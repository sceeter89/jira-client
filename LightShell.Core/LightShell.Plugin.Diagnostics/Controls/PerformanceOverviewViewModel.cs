using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using LightShell.Api;
using LightShell.Messaging.Api;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;

namespace LightShell.Plugin.Diagnostics.Controls
{
   public class PerformanceOverviewViewModel : IMicroservice,
      IHandleAllMessages
   {
      private const int TotalEventsStatisticsWindowSize = 15;

      private long _totalEvents;
      private IMessageBus _messageBus;

      public ObservableCollection<EventStats> Statistics { get; private set; }

      public ObservableCollection<EventsCount> TotalEventsStatistics { get; private set; }

      public void Handle(IMessage message)
      {
         _totalEvents++;

         var eventName = message.GetType().Name;

         var stat = Statistics.FirstOrDefault(s => s.EventName == eventName);
         DispatcherHelper.CheckBeginInvokeOnUI(() =>
         {
            if (stat != null)
               stat.Hits++;
            else
            {
               stat = new EventStats(eventName, message.GetType().Assembly.FullName)
               {
                  Hits = 1
               };
               Statistics.Add(stat);
            }
         });
      }

      public void Initialize(IMessageBus messageBus)
      {
         DispatcherHelper.CheckBeginInvokeOnUI(() =>
         {
            Statistics = new ObservableCollection<EventStats>();
            TotalEventsStatistics = new ObservableCollection<EventsCount>();

            var timer = new DispatcherTimer();
            timer.Tick += (s, a) =>
            {
               TotalEventsStatistics.Add(new EventsCount { Time = DateTime.Now, Count = _totalEvents });

               if (TotalEventsStatistics.Count > TotalEventsStatisticsWindowSize)
                  TotalEventsStatistics.RemoveAt(0);
            };
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Start();

            _messageBus = messageBus;
            _messageBus.SubscribeAllMessages(this);
         });
      }

      public class EventStats : ViewModelBase
      {
         public EventStats(string eventName, string assemblyName)
         {
            EventName = eventName;
            AssemblyName = assemblyName;
         }

         private long _hits;

         public long Hits
         {
            get
            {
               return _hits;
            }
            set
            {
               _hits = value;
               RaisePropertyChanged();
            }
         }

         public string EventName { get; private set; }
         public string AssemblyName { get; private set; }
      }

      public class EventsCount
      {
         public DateTime Time { get; set; }
         public long Count { get; set; }
      }
   }
}
