using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrsLibrary {
    public class Vendor : PrsTable {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool isRecommended { get; set; }

        private static void AddSQLInsertUpdateParameters(SqlCommand Command0, Vendor vendor) {
            // Adds parameters in the database from the Vendor vendor object
            Command0.Parameters.Add(new SqlParameter("@code", vendor.Code));
            Command0.Parameters.Add(new SqlParameter("@name", vendor.Name));
            Command0.Parameters.Add(new SqlParameter("@address", vendor.Address));
            Command0.Parameters.Add(new SqlParameter("@city", vendor.City));
            Command0.Parameters.Add(new SqlParameter("@state", vendor.State));
            Command0.Parameters.Add(new SqlParameter("@zip", vendor.Zip));
            Command0.Parameters.Add(new SqlParameter("@phone", vendor.Phone));
            Command0.Parameters.Add(new SqlParameter("@email", vendor.Email));
            Command0.Parameters.Add(new SqlParameter("@isRecommended", vendor.isRecommended));
        }

        public static bool Insert(Vendor vendor) {
            string Sql = String.Format(@"INSERT [vendor] (Code, Name, Address, City, State, Zip, Phone, Email, IsRecommended)"
                                        + "VALUES(@code, @name, @address, @city, @state, @zip, @phone, @email, @isRecommended)");

            SqlCommand Command0 = CreateSQLConnection(ConnStr, Sql, "Connection didn't open ... ");
            AddSQLInsertUpdateParameters(Command0, vendor);

            // If there are no records affected by the method call, crash the program and throw exception
            int recsAffected = ExecuteSQLInsUpdDelCommand(Command0, "Insert failed ... ");

            // Establish the ID for the Vendor vendor object
            vendor.Id = GetLastIdGenerated(ConnStr, "Vendor");

            Command0.Connection.Close();
            return (recsAffected == 1);
        }

        public static bool Update(Vendor vendor) {
            string Sql = String.Format("UPDATE [vendor] SET "
                                            + "Code = @code,"
                                            + "Name = @name,"
                                            + "Address = @address,"
                                            + "City = @city,"
                                            + "State = @state,"
                                            + "Zip = @zip,"
                                            + "Phone = @phone,"
                                            + "Email = @email,"
                                            + "IsRecommended = @isRecommended "
                                            + "WHERE ID = @id");

            SqlCommand Command0 = CreateSQLConnection(ConnStr, Sql, "Connection didn't open ... ");
            Command0.Parameters.Add(new SqlParameter("@id", vendor.Id));
            AddSQLInsertUpdateParameters(Command0, vendor);

            // If there are no records affected by the method call, crash the program and throw exception
            int recsAffected = ExecuteSQLInsUpdDelCommand(Command0, "Update failed ... ");

            Command0.Connection.Close();
            return (recsAffected == 1);
        }

        public static bool Delete(Vendor vendor) {
            string Sql = String.Format("DELETE FROM [vendor] WHERE ID = @id");
            string ConnStr = @"Server=STUDENT05;Database=prs;Trusted_Connection=true;";

            SqlCommand Command0 = CreateSQLConnection(ConnStr, Sql, "Connection didn't open ... ");
            Command0.Parameters.Add(new SqlParameter("@id", vendor.Id));

            // If there are no records affected by the method call, crash the program and throw exception
            int recsAffected = ExecuteSQLInsUpdDelCommand(Command0, "Delete failed ... ");

            Command0.Connection.Close();
            return (recsAffected == 1);
        }

        public static VendorCollection Select(string WhereClause, string OrderByClause) {
            string Sql = String.Format("SELECT * FROM [vendor] WHERE {0} ORDER BY {1}", WhereClause, OrderByClause);

            SqlCommand Command0 = CreateSQLConnection(ConnStr, Sql, "Connection didn't open ... ");
            SqlDataReader Reader = Command0.ExecuteReader();
            if (!Reader.HasRows) {
                throw new ApplicationException("Result set has no rows ... ");
            }

            VendorCollection vendors = new VendorCollection();
            while (Reader.Read()) {
                int id = Reader.GetInt32(Reader.GetOrdinal("Id"));
                string code = Reader.GetString(Reader.GetOrdinal("Code"));
                string name = Reader.GetString(Reader.GetOrdinal("Name"));
                string address = Reader.GetString(Reader.GetOrdinal("Address"));
                string city = Reader.GetString(Reader.GetOrdinal("City"));
                string state = Reader.GetString(Reader.GetOrdinal("State"));
                string zip = Reader.GetString(Reader.GetOrdinal("Zip"));
                string phone = Reader.GetString(Reader.GetOrdinal("Phone"));
                string email = Reader.GetString(Reader.GetOrdinal("Email"));
                bool isrecommended = Reader.GetBoolean(Reader.GetOrdinal("IsRecommended"));

                Vendor vendor = new Vendor();
                vendor.Id = id;
                vendor.Code = code;
                vendor.Name = name;
                vendor.Address = address;
                vendor.City = city;
                vendor.State = state;
                vendor.Zip = zip;
                vendor.Phone = phone;
                vendor.Email = email;
                vendor.isRecommended = isrecommended;

                vendors.Add(vendor);
            }

            Command0.Connection.Close();
            return vendors;
        }

        public static Vendor Select(int Id) {
            VendorCollection vendors = Select($"Id =  + {Id}", "Id");
            Vendor vendor = (vendors.Count == 1) ? vendors[0] : null;
            return vendor;
        }
    }
}
