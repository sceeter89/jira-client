using SharpRaven;
using SharpRaven.Data;
using System;

namespace JiraAssistant.Logic.Extensions
{
    public class Sentry
    {
        private static readonly RavenClient _client = new RavenClient("https://bf8c809bf8694b46a3584a677abef67b:596daf01a6464c9c82e5c930c99d6e6a@sentry.io/107140");

        public static RavenClient Client
        {
            get { return _client; }
        }

        public static void CaptureException(Exception e)
        {
            CaptureEvent(new SentryEvent(e));
        }

        public static void CaptureMessage(string message)
        {
            CaptureEvent(new SentryEvent(message));
        }

        public static void CaptureEvent(SentryEvent e)
        {
            Client.Capture(e);
        }
    }
}
