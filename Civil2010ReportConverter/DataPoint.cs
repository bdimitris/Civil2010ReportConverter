using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Civil2010ReportConverter
{
    public class DataPoint
    {
        private int _number;
        private string _station;
        private string _code;
        private string _offset;
        private string _elevation;
        private string _slope;
        private string _easting;
        private string _northing;

        public int Number { get => _number; set => _number = value; }
        public string Station { get => _station; set => _station = value; }
        public string Offset { get => _offset; set => _offset = value; }
        public string Code { get => _code; set => _code = value; }
        public string Elevation { get => _elevation; set => _elevation = value; }
        public string Slope { get => _slope; set => _slope = value; }
        public string Easting { get => _easting; set => _easting = value; }
        public string Northing { get => _northing; set => _northing = value; }

    }
}