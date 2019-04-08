using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace NGU_Idle_Master
{
    public class Timer
    {
        Stopwatch sw = new Stopwatch();
        int milliseconds = 0;
        TimeSpan timeSpan = new TimeSpan();
        bool elapsed = false;

        public bool Elapsed {
            get
            {
                if (elapsed)
                {
                    return true;
                }

                if (milliseconds == 0)
                {
                    if (timeSpan.Subtract(sw.Elapsed).TotalMilliseconds < 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (sw.ElapsedMilliseconds > milliseconds)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            set
            {
                elapsed = value;
            }
        }

        public int RemainingMilliseconds
        {
            get
            {
                int millisecondsRemaining = 0;

                if (milliseconds == 0)
                {
                    millisecondsRemaining = (int)timeSpan.Subtract(sw.Elapsed).TotalMilliseconds;
                }
                else
                {
                    millisecondsRemaining = milliseconds - (int)sw.ElapsedMilliseconds;
                }

                if (millisecondsRemaining < 0)
                {
                    millisecondsRemaining = 0;
                }

                return millisecondsRemaining;
            }
        }

        public TimeSpan RemainingTimeSpan
        {
            get
            {
                TimeSpan timeSpanRemaining = new TimeSpan();

                if (milliseconds == 0)
                {
                    timeSpanRemaining = timeSpan.Subtract(sw.Elapsed);
                }
                else
                {
                    timeSpanRemaining = new TimeSpan(0, 0, 0, 0, (int)(milliseconds - sw.ElapsedMilliseconds));
                }

                if (timeSpanRemaining.TotalMilliseconds < 0)
                {
                    timeSpanRemaining = new TimeSpan();
                }

                return timeSpanRemaining;
            }
        }

        public Timer(int millisenods)
        {
            this.milliseconds = millisenods;
            sw.Start();
        }

        public Timer(TimeSpan timeSpan)
        {
            this.timeSpan = timeSpan;
            sw.Start();
        }

        public void Start()
        {
            sw.Start();
            elapsed = false;
        }

        public void Restart()
        {
            sw.Restart();
            elapsed = false;
        }

        public void Stop()
        {
            sw.Stop();
        }
    }
}
