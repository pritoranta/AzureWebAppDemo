using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Azure.Storage.Queues;

namespace SolteqDemo2.Pages
{
    // PageModel for the app's main cshtml page.
    // Handles form data and sends it to MS Azure Storage when applicable.
    public class IndexModel : PageModel
    {
        // Keys for handling interactions with Azure and Google reCaptcha:
        private String Queue = Keys.AzureStorageQueueName;
        private String ConnectionString = Keys.AzureConnectionString;
        private String SiteKey = Keys.RecaptchaSiteKey;
        private String SecretKey = Keys.RecaptchaSecretKey;

        private Customer c = new Customer();
        private HttpClient client = new HttpClient();

        // Called when page is initially loaded.
        public void OnGet()
        {
            ViewData["RecaptchaSiteKey"] = SiteKey; // providing data for reCaptcha widget
        }

        // Invoked when form is posted.
        // Gathers data from the posted form and posts it into Azure storage, if it's valid.
        // If the data isn't accepted, error messages are written.
        public void OnPost()
        {
            ViewData["RecaptchaSiteKey"] = SiteKey; // providing data for reCaptcha widget
            bool isValid = true;
            // Next, we'll handle all the form entries and their data after form was posted.
            if (c.SetFirstName(Request.Form["FirstName"]))
                ViewData["FirstNameErr"] = "";
            else
            {
                isValid = false;
                ViewData["FirstNameErr"] = "Please enter a first name.";
            }
            if (c.SetLastName(Request.Form["LastName"]))
                ViewData["LastNameErr"] = "";
            else
            {
                isValid = false;
                ViewData["LastNameErr"] = "Please enter a last name.";
            }
            if (c.SetEmail(Request.Form["Email"]))
                ViewData["EmailErr"] = "";
            else
            {
                isValid = false;
                ViewData["EmailErr"] = "Please enter a valid email address.";
            }
            if (c.SetPhone(Request.Form["Phone"]))
                ViewData["PhoneErr"] = "";
            else
            {
                isValid = false;
                ViewData["PhoneErr"] = "Please enter a valid 10-digit phone number.";
            }
            if (c.SetAddress(Request.Form["Address"]))
                ViewData["AddressErr"] = "";
            else
            {
                isValid = false;
                ViewData["AddressErr"] = "Please enter an address.";
            }
            if (c.SetPostalCode(Request.Form["PostalCode"]))
                ViewData["PostalCodeErr"] = "";
            else
            {
                isValid = false;
                ViewData["PostalCodeErr"] = "Please enter a valid 5-digit postal code.";
            }
            if (c.SetCity(Request.Form["City"]))
                ViewData["CityErr"] = "";
            else
            {
                isValid = false;
                ViewData["CityErr"] = "Please enter a city.";
            }
            // Lastly, if all form data was valid, reCaptcha is checked.
            // If something wasn't valid, error messages are shown and applicable data is loaded back onto the form.
            if (isValid)
            {
                RecaptchaCheck(Request.Form["g-recaptcha-response"]);
            }
            else
                Retry();
        }

        // reCaptcha success check. If succesful, data is sent forward.
        // Otherwise Retry() is called.
        // Thank you Stack Overflow user Evan Mulawski for advice! :)
        private async Task RecaptchaCheck(String UserToken)
        {
            var Values = new Dictionary<String, String>
            {
                { "secret", SecretKey },
                { "response", UserToken }
            };
            var Content = new FormUrlEncodedContent(Values);
            var Response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", Content);
            var ResponseString = await Response.Content.ReadAsStringAsync();
            if (ResponseString.Contains("true"))
                InsertMessage(c.ToJson());
            else
            {
                Retry();
            }
        }

        // Refills each form entry with the given data after form was posted
        private void Retry()
        {
            ViewData["FirstName"] = c.FirstName;
            ViewData["LastName"] = c.LastName;
            ViewData["Email"] = c.Email;
            ViewData["Phone"] = c.Phone;
            ViewData["Address"] = c.Address;
            ViewData["PostalCode"] = c.PostalCode;
            ViewData["City"] = c.City;
        }

        // Sends the form data to MS Azure's storage queue.
        private void InsertMessage(string message)
        {
            // Instantiate a QueueClient which will be used to create and manipulate the queue
            QueueClient queueClient = new QueueClient(ConnectionString, Queue);

            // Create the queue if it doesn't already exist
            queueClient.CreateIfNotExists();

            if (queueClient.Exists())
            {
                // Send a message to the queue
                queueClient.SendMessage(message);
            }
        }

    }
}
