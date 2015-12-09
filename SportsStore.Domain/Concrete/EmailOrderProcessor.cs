using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Text;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System.Net;

namespace SportsStore.Domain.Concrete
{
    public class EmailSettings
    {
        public string MailToAddress = ""; //populate this property with the current user's email address.
        public string MailFromAddress = "steve.collins527test@gmail.com";
        public bool UseSsl = true;
        public string Username = "steve.collins527test";
        public string Password = "Password1.";
        public string ServerName = "smtp.gmail.com";
        public int ServerPort = 587; 
        public bool WriteAsFile = false; 
        public string FileLocation = @"C:\Users\Steve\Google Drive\Grad School\UIC\MS-MIS\IDS413\Sports_store_emails"; 
    }

    public class EmailOrderProcessor : IOrderProcessor
    {
        private EFDbContext context = new EFDbContext();
        private EmailSettings emailSettings;
        public EmailOrderProcessor(EmailSettings settings)
        {
            emailSettings = settings;
        }

        public void ProcessOrder(Cart cart, OrderHeader orderHeader, string userId, string userEmail)
        {
            //populate MailToAddress property of the emailSettings object with current user's email address.
            emailSettings.MailToAddress = userEmail;

            //get the credit card number of the current user.
            string cc_no = context.CreditCards
                                  .First(c => c.AspnetUser_Id == userId)
                                  .Id;

            decimal totalAmount = cart.ComputeTotalValue();  //total amount for this purchase

            //populate some of the order header attributes (properties)
            orderHeader.OrderDate = System.DateTime.Now;
            orderHeader.AspnetUser_Id = userId;
            orderHeader.CreditCard_Id = cc_no;
            orderHeader.TotalAmount = totalAmount;

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);

                if (emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }

                StringBuilder body = new StringBuilder()
                        .AppendLine("A new order has been submitted")
                        .AppendLine("---")
                        .AppendLine("Items:");

                foreach (var line in cart.Lines)
                {
                    OrderLine orderLine = new OrderLine(); //create a new oderLine object
                    orderLine.Product = line.Product;
                    orderLine.Quantity = line.Quantity;
                    orderLine.SalePrice = line.Product.Price;
                    orderLine.OrderHeader = orderHeader; //attach orderLine to orderHeader
                    context.OrderLines.Add(orderLine);

                    //add the following line to prevent adding a duplicate product row in the
                    // Products table everytime a user buys that product.

                    context.Entry(orderLine.Product).State = System.Data.Entity.EntityState.Modified;

                    var subtotal = line.Product.Price * line.Quantity;
                    body.AppendFormat("{0} x {1} (subtotal: {2:c}); ", line.Quantity, line.Product.Name, subtotal);
                }

                context.OrderHeaders.Add(orderHeader);
                context.SaveChanges();
                context.Dispose();  //dispose context object - it is no longer needed.

                body.AppendFormat("Total order value: {0:c}", cart.ComputeTotalValue())
                    .AppendLine("---")
                    .AppendLine("Ship to:")
                    .AppendLine(orderHeader.Name)
                    .AppendLine(orderHeader.Line1)
                    .AppendLine(orderHeader.Line2 ?? "")
                    .AppendLine(orderHeader.Line3 ?? "")
                    .AppendLine(orderHeader.City)
                    .AppendLine(orderHeader.State ?? "")
                    .AppendLine(orderHeader.Country)
                    .AppendLine(orderHeader.Zip)
                    .AppendLine("---")
                    .AppendFormat("Gift wrap: {0}", orderHeader.IsGiftwrap ? "Yes" : "No");

                MailMessage mailMessage = new MailMessage(
                    emailSettings.MailFromAddress, // From
                    emailSettings.MailToAddress, // To
                    "New order submitted!", // Subject
                    body.ToString()); // Body
                if (emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }
                smtpClient.Send(mailMessage);
            }
        }
    }
}