using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KilsatMassiks
{
    internal class Trip
    {
        public DateTime? _date_time {  get; set; }
        public int? _km {  get; set; }
        public int? _status { get; set; }

        public Trip()
        {
            this._date_time = null;
            this._km = null;
            this._status = null;
        }

        public Trip(DateTime date_time, int km, int status)
        {
            this._date_time = date_time;
            this._km = km;
            this._status = status; // 0 = Unhandled
                                   // 1 = Paid
                                   // 2 = Locked
        }
    }
}
