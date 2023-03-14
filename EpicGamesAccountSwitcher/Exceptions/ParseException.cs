using System;

namespace EpicGamesAccountSwitcher.Exceptions
{
    class ParseException : Exception
    {
        public ParseException(string message) : base(message) { }
    }
}
