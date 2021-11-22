using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Text;

namespace SolteqDemo2.Pages
{
    // PageModel for ManageForms.cshtml.
    // The page allows for viewing and handling of sent forms.
    public class ManageFormsModel : PageModel
    {
        private String queue = Keys.AzureStorageQueueName;
        private String connectionString = Keys.AzureConnectionString;

        // Called when page initially loads
        public void OnGet()
        {
            UpdateEntry();
        }

        // Reads the next message from the Azure storage queue and renders it for the user.
        private void UpdateEntry()
        {
            String entry = PeekMessage();
            if (entry.Length < 1)
            {
                ViewData["Message"] = "There are no awaiting forms at the moment."; // UI Message for the user
                ViewData["Entry"] = "";
                return;
            }
            ViewData["Message"] = "There is a form to be accepted:";
            LinkedList<(String, String)> objects = JsonHelper.ReadJsonObject(entry);
            StringBuilder sb = new StringBuilder();
            foreach((String,String) st in objects)
            {
                sb.Append(st.Item1 + ": " + st.Item2 + ",\n ");
            }
            ViewData["Entry"] = sb.ToString();
        }

        // Returns the value of the next message in the Azure storage's queue.
        // If there are no messages, returns an empty string.
        // Made with the assistance of official MS Docs.
        private String PeekMessage()
        {
            // Instantiate a QueueClient which will be used to manipulate the queue
            QueueClient queueClient = new QueueClient(connectionString, queue);

            if (queueClient.Exists())
            {
                // Peek at the next message
                PeekedMessage[] peekedMessage = queueClient.PeekMessages();
                if (peekedMessage.Length < 1) // No messages in the queue. Return with an empty message.
                    return "";
                String message = peekedMessage[0].Body.ToString();
                return message;
            }
            return "";
        }

        // Deletes the next entry from the queue.
        // Made with the assistance of official MS Docs.
        public Object Dequeue()
        {
            // Instantiate a QueueClient which will be used to manipulate the queue
            QueueClient queueClient = new QueueClient(connectionString, queue);

            if (queueClient.Exists())
            {
                // Get the next message
                QueueMessage[] retrievedMessage = queueClient.ReceiveMessages();
                if (retrievedMessage.Length < 1) // If there are no messages in the queue, return
                    return new object(); // This is to avoid an error in cshtml. No point otherwise. Help??

                // Delete the message
                queueClient.DeleteMessage(retrievedMessage[0].MessageId, retrievedMessage[0].PopReceipt);
                UpdateEntry();
            }
            return new Object(); // This is to avoid an error in cshtml. No point otherwise. Help??
        }
    }
}
