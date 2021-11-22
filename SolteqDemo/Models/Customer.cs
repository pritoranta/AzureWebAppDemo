using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SolteqDemo2
{
    // Class for handling data and data validation of a customer data entry.
    public class Customer
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }
        public String Phone { get; set; } // 10 digits
        public String Address { get; set; }
        public String PostalCode { get; set; } // 5 digits
        public String City { get; set; }

        public Customer() { }

        public Customer(String FirstName, String LastName, String Email, String Phone, String Address, String PostalCode, String City)
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Email = Email;
            this.Phone = Phone;
            this.Address = Address;
            this.PostalCode = PostalCode;
            this.City = City;
        }

        // Sets a new FirstName and then returns a bool telling if the value is valid.
        public bool SetFirstName(String name)
        {
            this.FirstName = name.Trim();
            if (FirstName.Length < 1)
                return false;
            return true;
        }

        // Sets a new LastName and then returns a bool that tells if the value is valid.
        public bool SetLastName(String name)
        {
            this.LastName = name.Trim();
            if (LastName.Length < 1)
                return false;
            return true;
        }

        // Attempts to set a new Email. If the value is invalid, new value of Email is "".
        // Return value bool is true when the given email was of valid form.
        public bool SetEmail(String email)
        {
            this.Email = email.Trim();
            if (Email.Length < 1)
                return false;
            // Thanks Stack Overflow user Brad Larson for this regex!
            Regex EmailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match Match = EmailRegex.Match(Email);
            if (!Match.Success)
            {
                Email = "";
                return false;
            }
            return true;
        }

        // Attempts to set a new Phone value. If the given value is invalid, the value will be set to "".
        // A 10 digit-number is required.
        // Returns a bool value of true if the number was valid, false otherwise.
        public bool SetPhone(String phone)
        {
            this.Phone = phone.Trim();
            if (Phone.Length < 1)
                return false;
            // Thanks Stack Overflow user Ravi K Thapliyal!
            Regex PhoneRegex = new Regex(@"^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$");
            Match Match = PhoneRegex.Match(Phone);
            if (!Match.Success)
            {
                Phone = "";
                return false;
            }
            return true;
        }

        // Sets a new Address.
        // Only requirement is that the String isn't empty.
        // Returns true if the given value was valid.
        public bool SetAddress(String address)
        {
            this.Address = address.Trim();
            if (Address.Length < 1)
                return false;
            return true;
        }

        // Sets a new postal code.
        // Only 5-digit number sequences are accepted.
        // If the given value is invalid, returns false and sets PostalCode to "".
        public bool SetPostalCode(String postalCode)
        {
            this.PostalCode = postalCode.Trim();
            if (PostalCode.Length < 1)
                return false;
            // There are so many postal codes in the world, that for this app this regex will do:
            Regex PostalCodeRegex = new Regex(@"\d{5}");
            Match Match = PostalCodeRegex.Match(PostalCode);
            if (!Match.Success)
            {
                PostalCode = "";
                return false;
            }
            return true;
        }

        // Sets a new City and then returns bool telling if the value was valid.
        // Returns true if valid.
        public bool SetCity(String city)
        {this.City = city.Trim();
            if (City.Length < 1)
                return false;
            return true;
        }

        // Returns a String representation of this Customer as a JSON object.
        public String ToJson()
        {
            LinkedList<(String, String)> list = new LinkedList<(String, String)>();
            list.AddLast(("FirstName", FirstName));
            list.AddLast(("LastName", LastName));
            list.AddLast(("Email", Email));
            list.AddLast(("Phone", Phone));
            list.AddLast(("Address", Address));
            list.AddLast(("PostalCode", PostalCode));
            list.AddLast(("City", City));
            return JsonHelper.JsonObject(list);
        }
    }
}
