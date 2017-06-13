using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrsLibrary {
    public class Product : PrsTable {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public Vendor Vendor { get; set; }
        public string Name { get; set; }
        public string VendorPartNumber { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; }
        public string PhotoPath { get; set; }

        private static void AddSQLInsertUpdateParameters(SqlCommand Command0, Product product) {
            // Adds parameters in the database from the Product product object
            Command0.Parameters.Add(new SqlParameter("@vendorId", product.VendorId));
            Command0.Parameters.Add(new SqlParameter("@name", product.Name));
            Command0.Parameters.Add(new SqlParameter("@vendorpartnumber", product.VendorPartNumber));
            Command0.Parameters.Add(new SqlParameter("@price", product.Price));
            Command0.Parameters.Add(new SqlParameter("@unit", product.Unit));
            Command0.Parameters.Add(new SqlParameter("@photopath", product.PhotoPath));
        }

        public static bool Insert(Product product) {
            string Sql = String.Format(@"INSERT [product] (VendorId, Name, VendorPartNumber, Price, Unit, PhotoPath)"
                                        + "VALUES(@vendorId, @name, @vendorpartnumber, @price, @unit, @photopath)");

            SqlCommand Command0 = CreateSQLConnection(ConnStr, Sql, "Connection didn't open ... ");
            AddSQLInsertUpdateParameters(Command0, product);

            // If there are no records affected by the method call, crash the program and throw exception
            int recsAffected = ExecuteSQLInsUpdDelCommand(Command0, "Insert failed ... ");

            // Establish the ID for the Product product object
            product.Id = GetLastIdGenerated(ConnStr, "Product");

            // Attach the Vendor object
            VendorCollection vendors = Vendor.Select("Id = " + product.VendorId, "id");
            if (vendors.Count > 0) {
                product.Vendor = vendors[0];
            }

            Command0.Connection.Close();
            return (recsAffected == 1);
        }

        public static bool Update(Product product) {
            string Sql = String.Format("UPDATE [product] SET "
                                            + "VendorId = @vendorId,"
                                            + "Name = @name,"
                                            + "VendorPartNumber = @vendorpartnumber,"
                                            + "Price = @price,"
                                            + "Unit = @unit,"
                                            + "PhotoPath = @photopath "
                                            + "WHERE ID = @id");

            SqlCommand Command0 = CreateSQLConnection(ConnStr, Sql, "Connection didn't open ... ");
            Command0.Parameters.Add(new SqlParameter("@id", product.Id));
            AddSQLInsertUpdateParameters(Command0, product);

            //VendorCollection vendors = Vendor.Select("Name = " + product.Vendor.Name, "id");
            //if (vendors.Count > 0) {
            //    product.VendorId = vendors[0].Id;
            //}

            // If there are no records affected by the method call, crash the program and throw exception
            int recsAffected = ExecuteSQLInsUpdDelCommand(Command0, "Update failed ... ");

            product.Vendor = Vendor.Select(product.VendorId);

            Command0.Connection.Close();
            return (recsAffected == 1);
        }

        public static bool Delete(Product product) {
            string Sql = String.Format("DELETE FROM [product] WHERE ID = @id");

            SqlCommand Command0 = CreateSQLConnection(ConnStr, Sql, "Connection didn't open ... ");
            Command0.Parameters.Add(new SqlParameter("@id", product.Id));

            // If there are no records affected by the method call, crash the program and throw exception
            int recsAffected = ExecuteSQLInsUpdDelCommand(Command0, "Delete failed ... ");

            Command0.Connection.Close();
            return (recsAffected == 1);
        }

        public static ProductCollection Select(string WhereClause, string OrderByClause) {
            string Sql = String.Format("SELECT * FROM [product] WHERE {0} ORDER BY {1}", WhereClause, OrderByClause);

            SqlCommand Command0 = CreateSQLConnection(ConnStr, Sql, "Connection didn't open ... ");
            SqlDataReader Reader = Command0.ExecuteReader();
            if (!Reader.HasRows) {
                throw new ApplicationException("Result set has no rows ... ");
            }

            ProductCollection products = new ProductCollection();
            while (Reader.Read()) {
                int id = Reader.GetInt32(Reader.GetOrdinal("Id"));
                int vendorId = Reader.GetInt32(Reader.GetOrdinal("VendorId"));
                string name = Reader.GetString(Reader.GetOrdinal("Name"));
                string vendorpartnumber = Reader.GetString(Reader.GetOrdinal("VendorPartNumber"));
                decimal price = Reader.GetDecimal(Reader.GetOrdinal("Price"));
                string unit = Reader.GetString(Reader.GetOrdinal("Unit"));
                string photopath = String.Empty;
                if (!Reader.IsDBNull(Reader.GetOrdinal("PhotoPath"))) {
                    photopath = Reader.GetString(Reader.GetOrdinal("PhotoPath"));
                }

                string whereClause = String.Format("Id = {0}", vendorId);
                VendorCollection vendors = Vendor.Select(whereClause, "Id");
                Vendor vendor = (vendors.Count > 0) ? vendors[0] : null;

                Product product = new Product();
                product.Id = id;
                product.Vendor = vendor;
                product.VendorId = vendorId;
                product.Name = name;
                product.VendorPartNumber = vendorpartnumber;
                product.Price = price;
                product.Unit = unit;
                product.PhotoPath = photopath;

                products.Add(product);
            }

            Command0.Connection.Close();
            return products;
        }

        public static Product Select(int Id) {
            ProductCollection products = Select($"Id =  + {Id}", "Id");
            Product product = (products.Count == 1) ? products[0] : null;
            return product;
        }
    }
}
