using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrsLibrary {
    public class LineItem : PrsTable {
        public int Id { get; set; }
        public int PurchaseRequestId { get; set; }
        public PurchaseRequest PurchaseRequest { get; private set; }
        public int ProductId { get; set; }
        public Product Product { get; private set; }
        public int Quantity { get; set; }

        private static void AddSQLInsertUpdateParameters(SqlCommand Command0, LineItem LineItem) {
            // Adds parameters in the database from the LineItem LineItem object
            Command0.Parameters.Add(new SqlParameter("@PurchaseRequestId", LineItem.PurchaseRequestId));
            Command0.Parameters.Add(new SqlParameter("@ProductId", LineItem.ProductId));
            Command0.Parameters.Add(new SqlParameter("@Quantity", LineItem.Quantity));
        }

        public static bool Insert(LineItem lineItem) {
            string Sql = String.Format(@"INSERT [LineItem] (PurchaseRequestId, ProductId, Quantity)"
                                                        + "VALUES(@PurchaseRequestId, @ProductId, @Quantity)");

            SqlCommand Command0 = CreateSQLConnection(ConnStr, Sql, "Connection didn't open ... ");
            AddSQLInsertUpdateParameters(Command0, lineItem);

            // If there are no records affected by the method call, crash the program and throw exception
            int recsAffected = ExecuteSQLInsUpdDelCommand(Command0, "Insert failed ... ");
            
            lineItem.Id = GetLastIdGenerated(ConnStr, "lineItem");

            lineItem.PurchaseRequest = PurchaseRequest.Select(lineItem.PurchaseRequestId);
            lineItem.Product = Product.Select(lineItem.ProductId);

            Command0.Connection.Close();
            return (recsAffected == 1);
        }

        public static bool Update(LineItem lineItem) {
            string Sql = String.Format("UPDATE [LineItem] SET "
                                            + "PurchaseRequestId = @PurchaseRequestId,"
                                            + "ProductId = @ProductId,"
                                            + "Quantity = @Quantity "
                                            + "WHERE ID = @id");

            SqlCommand Command0 = CreateSQLConnection(ConnStr, Sql, "Connection didn't open ... ");
            Command0.Parameters.Add(new SqlParameter("@id", lineItem.Id));
            AddSQLInsertUpdateParameters(Command0, lineItem);

            int recsAffected = ExecuteSQLInsUpdDelCommand(Command0, "Update failed ... ");

            Command0.Connection.Close();
            return (recsAffected == 1);
        }

        public static bool Delete(LineItem lineItem) {
            string Sql = String.Format("DELETE FROM [LineItem] WHERE ID = @id");

            SqlCommand Command0 = CreateSQLConnection(ConnStr, Sql, "Connection didn't open ... ");
            Command0.Parameters.Add(new SqlParameter("@id", lineItem.Id));

            // If there are no records affected by the method call, crash the program and throw exception
            int recsAffected = ExecuteSQLInsUpdDelCommand(Command0, "Delete failed ... ");

            Command0.Connection.Close();
            return (recsAffected == 1);
        }

        public static LineItemCollection Select(string WhereClause, string OrderByClause) {
            string Sql = String.Format("SELECT * FROM [LineItem] WHERE {0} ORDER BY {1}", WhereClause, OrderByClause);

            SqlCommand Command0 = CreateSQLConnection(ConnStr, Sql, "Connection didn't open ... ");
            SqlDataReader Reader = Command0.ExecuteReader();
            if (!Reader.HasRows) {
                throw new ApplicationException("Result set has no rows ... ");
            }

            LineItemCollection lineItems = new LineItemCollection();
            while (Reader.Read()) {
                int id = Reader.GetInt32(Reader.GetOrdinal("Id"));
                int purchaseRequestId = Reader.GetInt32(Reader.GetOrdinal("PurchaseRequestId"));
                int productId = Reader.GetInt32(Reader.GetOrdinal("ProductId"));
                int quantity = Reader.GetInt32(Reader.GetOrdinal("Quantity"));

                LineItem lineItem = new LineItem();
                lineItem.Id = id;
                lineItem.PurchaseRequestId = purchaseRequestId;
                // lineItem.PurchaseRequest = PurchaseRequest.Select(purchaseRequestId);
                lineItem.ProductId = productId;
                lineItem.Product = Product.Select(productId);
                lineItem.Quantity = quantity;

                lineItems.Add(lineItem);
            }

            Command0.Connection.Close();
            return lineItems;
        }

        public static LineItem Select(int Id) {
            LineItemCollection lineItems = Select($"Id =  + {Id}", "Id");
            LineItem lineItem = (lineItems.Count == 1) ? lineItems[0] : null;
            return lineItem;
        }

        public static bool Delete(int Id) {
            LineItem lineItem = LineItem.Select(Id);
            if (lineItem == null) {
                return false;
            }
            return LineItem.Delete(lineItem);
        }

        public LineItem() {
            this.Quantity = 1;
        }
    }
}
