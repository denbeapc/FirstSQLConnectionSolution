using PrsLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstSQLConnectionProject {
    public class Program {
        private void Run() {
            // Establishes a connection to the SQL database
            string ConnStr = @"Server=STUDENT05;Database=prs;Trusted_Connection=true;";
            SqlConnection Conn = new SqlConnection(ConnStr);

            // Opens the SQL database connection
            Conn.Open();

            // Throws an error if the connection is closed (for whatever reason)
            if (Conn.State != ConnectionState.Open) {
                throw new ApplicationException("Connection failed ... ");
            }

            // ============================ TEST COMMANDS ============================ //

            PurchaseRequest pr = PurchaseRequest.Select(4);
            pr.UpdateLineItem(12, 2);

            // use linq syntax to total up all of purchase requests

            // ============================ BREAK STATEMENT IF NOT DISPLAYING ============================ //

            int i = 0;

            // ============================ DISPLAY OUTPUT ============================ //
            UserCollection users = User.Select("1 = 1", "id");
            VendorCollection vendors = Vendor.Select("1 = 1", "id");
            ProductCollection products = Product.Select("1 = 1", "id");
            PurchaseRequestCollection purchaseRequests = PurchaseRequest.Select("1 = 1", "id");

            // Simply loops through the lists and displays data accordingly
            string line;
            foreach (PurchaseRequest x in purchaseRequests) {
                line = String.Format("{0} - {2}\nUser: {1}\nDate Needed: {3}\nStatus: {4}\nTotal: {5}",
                                        x.Id, (x.User.FirstName + " " + x.User.LastName), x.Description,
                                        x.DateNeeded, x.Status, x.Total);
                Console.WriteLine(line);
                Console.WriteLine();
            }

            foreach (Product x in products) {
                line = String.Format("{0} - {1}: {2}\nPrice: ${3}\nUnit: {4}\nVendor Part Number: {5}",
                                        x.Id, x.Vendor.Name, x.Name,
                                        x.Price, x.Unit, x.VendorPartNumber);
                Console.WriteLine(line);
                Console.WriteLine();
            }

            foreach (Vendor x in vendors) {
                line = String.Format("{0} - {1} {2}\nEmail: {3}\nPhone: {4}",
                                        x.Id, x.Code, x.Name,
                                        x.Email, x.Phone);
                Console.WriteLine(line);
                Console.WriteLine();
            }

            foreach (User x in users) {
                line = String.Format("{0} - {1} {2}\nEmail: {3}\nPhone: {4}",
                                        x.Id, x.FirstName, x.LastName,
                                        x.Email, x.Phone);
                Console.WriteLine(line);
                Console.WriteLine();
            }

            // Finally, closes the SQL database connection
            Console.ReadKey();
            Conn.Close();
        }

        public static void Main(string[] args) {
            (new Program()).Run();
        }
    }
}
