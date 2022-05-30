using System;

namespace AvailabilityChecker.Email.Exceptions
{
    class IncorrectSenderEmailAddressFormatException : Exception
    {
        public IncorrectSenderEmailAddressFormatException(string emailAddress) : 
            base ($"Address {emailAddress} is incorrect")
        {

        }
    }
}
