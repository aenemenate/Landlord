using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{

    class TimeHandler
    {
        private Time currentTime;
        private int secondsPerFrame;


        // CONSTRUCTORS //

        public TimeHandler(int currentHour, int currentMinute, int currentSecond, int secondsPerFrame = 4)
        {
            currentTime = new Time(0, currentHour, currentMinute, currentSecond);
            this.secondsPerFrame = secondsPerFrame;
        }

        public TimeHandler()
        {
        }

        // FUNCS //

        public int GetOutsideSightDist(int creatureSightDist)
        {
            if (currentTime.Hour < 5 || currentTime.Hour > 22)
                return (int)((float)creatureSightDist * 0.7F);
            if (currentTime.Hour < 7 || currentTime.Hour > 20)
                return (int)((float)creatureSightDist * 0.8F);
            if (currentTime.Hour < 8 || currentTime.Hour > 19)
                return (int)((float)creatureSightDist * 0.9F);
            return creatureSightDist;
        }

        // PROPERTIES //

        public Time CurrentTime
        {
            get { return currentTime; }
            set { currentTime = value; }
        }

        public int SecondsPerFrame
        {
            get { return secondsPerFrame; }
            set { secondsPerFrame = value; }
        }
    }

    class Time
    {
        private int day, hour, minute, second;


        // CONSTRUCTORS //

        public Time(int day, int hour, int minute, int second)
        {
            this.day = day;
            this.hour = hour;
            this.minute = minute;
            this.second = second;
        }
        
        public Time (Time time)
        {
            day = time.Day;
            hour = time.Hour;
            minute = time.Minute;
            second = time.Second;
        }

        public Time()
        {
        }

        // FUNCTIONS //

        public void AddTime(int seconds)
        {
            second += seconds;
            while (second >= 60)
            {
                minute += 1;
                second = second - 60;
            }
            while (minute >= 60)
            {
                hour += 1;
                minute = minute - 60;
            }
            while (hour >= 24)
            {
                day += 1;
                hour = hour - 24;
            }
        }

        public void SubtractTime(int seconds)
        {
            second -= seconds;
            while (second < 0)
            {
                minute -= 1;
                second += 60;
            }
            while (minute < 0)
            {
                hour -= 1;
                minute += 60;
            }
            while (hour < 0)
            {
                day -= 1;
                hour += 24;
            }
        }

        public bool Equals(Time otherTime)
        {
            if (day == otherTime.Day && hour == otherTime.Hour && minute == otherTime.Minute && second == otherTime.Second)
                return true;
            return false;
        }

        public bool IsGreaterThan(Time otherTime)
        {
            if (day < otherTime.Day)
                return false;
            if (day > otherTime.Day)
                return true;
            if (hour < otherTime.Hour)
                return false;
            if (hour > otherTime.Hour)
                return true;
            if (minute < otherTime.Minute)
                return false;
            if (minute > otherTime.Minute)
                return true;
            if (second < otherTime.Second)
                return false;
            if (second > otherTime.Second)
                return true;
            return false;
        }

        public bool IsLessThan(Time otherTime)
        {
            if (day > otherTime.Day)
                return false;
            if (day < otherTime.Day)
                return true;
            if (hour > otherTime.Hour)
                return false;
            if (hour < otherTime.Hour)
                return true;
            if (minute > otherTime.Minute)
                return false;
            if (minute < otherTime.Minute)
                return true;
            if (second > otherTime.Second)
                return false;
            if (second < otherTime.Second)
                return true;

            return false;
        }
        
        public int Minus(Time otherTime)
        {
            int totalSeconds = day * 86400 + hour * 3600 + minute * 60 + second;
            int othersTotalSeconds = otherTime.Day * 86400 + otherTime.Hour * 3600 + otherTime.Minute * 60 + otherTime.Second;

            return totalSeconds - othersTotalSeconds;
        }

        // PROPERTIES //

        public int Day
        {
            get { return day; }
            set { day = value; }
        }

        public int Hour
        {
            get { return hour; }
            set { hour = value; }
        }

        public int Minute
        {
            get { return minute; }
            set { minute = value; }
        }

        public int Second
        {
            get { return second; }
            set { second = value; }
        }
    }
}
