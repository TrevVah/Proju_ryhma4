using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KilsatMassiks
{
    public class Trip
    {
        public DateTime? date_time {  get; set; }
        public int? km {  get; set; }
        public int? status { get; set; }
        public float? duration { get; set; }
        public string? info { get; set; }

        public Trip()
        {
            this.date_time = null;
            this.km = null;
            this.status = null;
            this.duration = null;
            this.info = null;
        }

        public Trip(DateTime _date_time, int _km, float _duration, string _info, int _status)
        {
            this.date_time = _date_time;
            this.km = _km;
            this.duration = _duration;
            this.info = _info;
            this.status = _status; // 0 = Unhandled
                                   // 1 = Paid
                                   // 2 = Locked
        }
    }
}
