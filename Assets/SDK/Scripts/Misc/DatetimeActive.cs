using System;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;


namespace ThunderRoad
{
    public class DatetimeActive : MonoBehaviour
    {
        [Serializable]
        [DeclareHorizontalGroup("SimpleTime")]
        public class SimpleDateTime
        {
            [Group("SimpleTime")]
            [LabelWidth(40)]
            public int month;
            [Group("SimpleTime")]
            [LabelWidth(25)]
            public int day;
            private string hourMinuteString
            {
                get
                {
                    var time = GetHourMinute();
                    return $"Time: {time.hour}:{(time.minute == 0 ? "00" : time.minute)}";
                }
            }

            [LabelText("$hourMinuteString")]
            [LabelWidth(75)]
            [Group("SimpleTime")]
            [Range(0, 48)]
            public int time;

            public (int hour, int minute) GetHourMinute()
            {
                return (Mathf.FloorToInt(time / 2), 30 * (time % 2));
            }

            public DateTime ToDateTime(int year)
            {
                var time = GetHourMinute();
                DateTime dt = new DateTime(year, month, day, time.hour, time.minute, 0);
                return dt;
            }

            public SimpleDateTime(int month, int day, int time)
            {
                this.month = month;
                this.day = day;
                this.time = time;
            }
        }

        [InlineProperty]
        [HideLabel]
        [Header("From")]
        public SimpleDateTime activeStart = new(6, 17, 36);
        [InlineProperty]
        [HideLabel]
        [Header("Until")]
        public SimpleDateTime activeEnd = new(10, 28, 36);

        [Button]
        public bool CheckWithinRange()
        {
            DateTime now = DateTime.Now;
            int currentYear = now.Year;
            DateTime startDate = activeStart.ToDateTime(currentYear);
            DateTime endDate = activeEnd.ToDateTime(currentYear);
            // if the end date is earlier/less than the start date, we know the new year happens between the two dates, so we handle it separately
            if (endDate < startDate)
            {
                return now >= startDate || now <= endDate;
            }
            else
            {
                return now >= startDate && now <= endDate;
            }
        }

        private void Awake()
        {
            if (!CheckWithinRange()) gameObject.SetActive(false);
        }
    }
}
