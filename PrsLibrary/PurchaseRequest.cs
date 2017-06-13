using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrsLibrary {
    public class PurchaseRequest : PrsTable {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Description { get; set; }
        public string Justification { get; set; }
        public DateTime DateNeeded { get; set; }
        public string DeliveryMode { get; set; }
        public bool DocsAttached { get; set; }
        public string Status { get; set; }
        public decimal Total { get; set; }
        public DateTime SubmittedDate { get; set; }
        public LineItemCollection LineItems { get; set; }

        private static void AddSQLInsertUpdateParameters(SqlCommand Command0, PurchaseRequest purchaseRequest) {
            // Adds parameters in the database from the PurchaseRequest purchaseRequest object
            Command0.Parameters.Add(new SqlParameter("@UserId", purchaseRequest.UserId));
            Command0.Parameters.Add(new SqlParameter("@Description", purchaseRequest.Description));
            Command0.Parameters.Add(new SqlParameter("@Justification", purchaseRequest.Justification));
            Command0.Parameters.Add(new SqlParameter("@DateNeeded", purchaseRequest.DateNeeded));
            Command0.Parameters.Add(new SqlParameter("@DeliveryMode", purchaseRequest.DeliveryMode));
            Command0.Parameters.Add(new SqlParameter("@DocsAttached", purchaseRequest.DocsAttached));
            Command0.Parameters.Add(new SqlParameter("@Status", purchaseRequest.Status));
            Command0.Parameters.Add(new SqlParameter("@Total", purchaseRequest.Total));
            Command0.Parameters.Add(new SqlParameter("@SubmittedDate", purchaseRequest.SubmittedDate));
        }

        public static bool Insert(PurchaseRequest purchaseRequest) {
            string Sql = String.Format(@"INSERT [purchaseRequest] (UserId, Description, Justification, DateNeeded, DeliveryMode, DocsAttached, Status, Total, SubmittedDate)"
                                                        + "VALUES(@UserId, @Description, @Justification, @DateNeeded, @DeliveryMode, @DocsAttached, @Status, @Total, @SubmittedDate)");

            SqlCommand Command0 = CreateSQLConnection(ConnStr, Sql, "Connection didn't open ... ");
            AddSQLInsertUpdateParameters(Command0, purchaseRequest);

            // If there are no records affected by the method call, crash the program and throw exception
            int recsAffected = ExecuteSQLInsUpdDelCommand(Command0, "Insert failed ... ");

            // Establish the ID for the PurchaseRequest purchaseRequest object
            purchaseRequest.Id = GetLastIdGenerated(ConnStr, "PurchaseRequest");

            // Attach the User object
            UserCollection users = User.Select("Id = " + purchaseRequest.UserId, "id");
            if (users.Count > 0) {
                purchaseRequest.User = users[0];
            }

            Command0.Connection.Close();
            return (recsAffected == 1);
        }

        public static bool Update(PurchaseRequest purchaseRequest) {
            string Sql = String.Format("UPDATE [purchaseRequest] SET "
                                            + "UserId = @UserId,"
                                            + "Description = @Description,"
                                            + "Justification = @Justification,"
                                            + "DateNeeded = @DateNeeded,"
                                            + "DeliveryMode = @DeliveryMode,"
                                            + "DocsAttached = @DocsAttached, "
                                            + "Status = @Status,"
                                            + "Total = @Total, "
                                            + "SubmittedDate = @SubmittedDate "
                                            + "WHERE ID = @id");

            SqlCommand Command0 = CreateSQLConnection(ConnStr, Sql, "Connection didn't open ... ");
            Command0.Parameters.Add(new SqlParameter("@id", purchaseRequest.Id));
            AddSQLInsertUpdateParameters(Command0, purchaseRequest);

            int recsAffected = ExecuteSQLInsUpdDelCommand(Command0, "Update failed ... ");

            purchaseRequest.User = User.Select(purchaseRequest.UserId);

            Command0.Connection.Close();
            return (recsAffected == 1);
        }

        public static bool Delete(PurchaseRequest purchaseRequest) {
            string Sql = String.Format("DELETE FROM [purchaseRequest] WHERE ID = @id");

            SqlCommand Command0 = CreateSQLConnection(ConnStr, Sql, "Connection didn't open ... ");
            Command0.Parameters.Add(new SqlParameter("@id", purchaseRequest.Id));

            // If there are no records affected by the method call, crash the program and throw exception
            int recsAffected = ExecuteSQLInsUpdDelCommand(Command0, "Delete failed ... ");

            Command0.Connection.Close();
            return (recsAffected == 1);
        }

        public static PurchaseRequestCollection Select(string WhereClause, string OrderByClause) {
            string Sql = String.Format("SELECT * FROM [purchaseRequest] WHERE {0} ORDER BY {1}", WhereClause, OrderByClause);

            SqlCommand Command0 = CreateSQLConnection(ConnStr, Sql, "Connection didn't open ... ");
            SqlDataReader Reader = Command0.ExecuteReader();
            if (!Reader.HasRows) {
                throw new ApplicationException("Result set has no rows ... ");
            }

            PurchaseRequestCollection purchaseRequests = new PurchaseRequestCollection();
            while (Reader.Read()) {
                int id = Reader.GetInt32(Reader.GetOrdinal("Id"));
                int userId = Reader.GetInt32(Reader.GetOrdinal("UserId"));
                string description = Reader.GetString(Reader.GetOrdinal("Description"));
                string justification = Reader.GetString(Reader.GetOrdinal("Justification"));
                DateTime dateNeeded = Reader.GetDateTime(Reader.GetOrdinal("DateNeeded"));
                string deliveryMode = Reader.GetString(Reader.GetOrdinal("DeliveryMode"));
                bool docsAttached = Reader.GetBoolean(Reader.GetOrdinal("DocsAttached"));
                string status = Reader.GetString(Reader.GetOrdinal("Status"));
                decimal total = Reader.GetDecimal(Reader.GetOrdinal("Total"));
                DateTime submittedDate = Reader.GetDateTime(Reader.GetOrdinal("SubmittedDate"));
                
                PurchaseRequest purchaseRequest = new PurchaseRequest();
                purchaseRequest.Id = id;
                purchaseRequest.UserId = userId;
                purchaseRequest.User = User.Select(userId);
                purchaseRequest.Description = description;
                purchaseRequest.Justification = justification;
                purchaseRequest.DateNeeded = dateNeeded;
                purchaseRequest.DeliveryMode = deliveryMode;
                purchaseRequest.DocsAttached = docsAttached;
                purchaseRequest.Status = status;
                purchaseRequest.Total = total;
                purchaseRequest.SubmittedDate = submittedDate;

                // Get LineItemCollection
                purchaseRequest.LineItems = GetLineItems(purchaseRequest.Id);

                purchaseRequests.Add(purchaseRequest);
            }

            Command0.Connection.Close();
            return purchaseRequests;
        }

        private static LineItemCollection GetLineItems(int PurchaseRequestId) {
            LineItemCollection lineItems 
                = LineItem.Select($"PurchaseRequestId = {PurchaseRequestId}", "Id");
            return lineItems;
        }

        private void UpdateLineItemsProperty() {
            this.LineItems = GetLineItems(this.Id);
        }

        public static PurchaseRequest Select(int Id) {
            PurchaseRequestCollection purchaseRequests = Select($"Id =  + {Id}", "Id");
            PurchaseRequest purchaseRequest = (purchaseRequests.Count == 1) ? purchaseRequests[0] : null;
            return purchaseRequest;
        }

        public static bool Delete(int Id) {
            PurchaseRequest purchaseRequest = PurchaseRequest.Select(Id);
            if(purchaseRequest == null) {
                return false;
            }
            return PurchaseRequest.Delete(purchaseRequest);
        }

        public PurchaseRequest() {
            this.DateNeeded = DateTime.Now.AddDays(7);
            this.DeliveryMode = "USPS";
            this.DocsAttached = false;
            this.Status = "New";
            this.Total = 0.0M;
            this.SubmittedDate = DateTime.Now;
            LineItems = new LineItemCollection();
        }

        public bool AddLineItem(int ProductId, int Quantity) {
            Product product = Product.Select(ProductId);
            LineItem lineItem = new LineItem {
                PurchaseRequestId = this.Id,
                ProductId = ProductId,
                Quantity = Quantity
            };

            bool rc = LineItem.Insert(lineItem);
            if (!rc)
                throw new ApplicationException("Insert of line item failed ... ");

            this.Total += Quantity * product.Price;
            rc = PurchaseRequest.Update(this);
            UpdateLineItemsProperty();
            return rc;
        }

        public bool DeleteLineItem(int LineItemId) {
            LineItem lineItem = LineItem.Select(LineItemId);
            if (lineItem == null) {
                throw new ApplicationException("Line item to delete is not found ... ");
            }

            decimal amount = lineItem.Product.Price * lineItem.Quantity;
            bool rc = LineItem.Delete(lineItem);
            if (!rc) {
                throw new ApplicationException("Line item delete failed ... ");
            }

            this.Total -= amount;
            rc = PurchaseRequest.Update(this);
            if (!rc) {
                throw new ApplicationException("Purchase Request update failed ... ");
            }

            UpdateLineItemsProperty();
            return rc;
        }

        public bool UpdateLineItem(int LineItemId, int NewQuantity) {
            LineItem lineItem = LineItem.Select(LineItemId);
            if (lineItem == null) {
                throw new ApplicationException("Line item to update is not found ... ");
            }

            if (NewQuantity < 0) {
                throw new ApplicationException("New Quantity cannot be less then zero ... ");
            }

            decimal oldAmount = lineItem.Product.Price * lineItem.Quantity;
            lineItem.Quantity = NewQuantity;
            decimal newAmount = lineItem.Product.Price * lineItem.Quantity;
            decimal changeTotal = newAmount - oldAmount;

            bool rc = LineItem.Update(lineItem);
            if (!rc) {
                throw new ApplicationException("Line item update failed ... ");
            }

            this.Total += changeTotal;
            rc = PurchaseRequest.Update(this);
            if (!rc) {
                throw new ApplicationException("Purchase Request update failed ... ");
            }

            UpdateLineItemsProperty();
            return rc;
        }

        private void CalculateTotal() {
            this.Total = 0.0M;
            foreach (LineItem x in LineItems) {
                this.Total += (x.Quantity * x.Product.Price);
            }
        }
    }
}
