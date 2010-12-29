// DGrok Delphi parser
// Copyright (C) 2007 Joe White
// http://www.excastle.com/dgrok
//
// Licensed under the Open Software License version 3.0
// http://www.opensource.org/licenses/osl-3.0.php
using System;
using System.Collections.Generic;
using System.Text;

namespace DGrok.Framework
{
    public class DGrokException : Exception
    {
        private Location _location;

        public DGrokException(string message, Location location)
            : base(message)
        {
            _location = location;
        }

        public Location Location
        {
            get { return _location; }
        }
    }
}
