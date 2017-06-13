using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrsLibrary {
    public class User : PrsTable {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool isReviewer { get; set; }
        public bool isAdmin { get; set; }

        private static void AddSQLInsertUpdateParameters(SqlCommand Command0, User user) {
            // Adds parameters in the database from the Vendor vendor object
            Command0.Parameters.Add(new SqlParameter("@username", user.UserName));
            Command0.Parameters.Add(new SqlParameter("@password", user.Password));

            // Check FirstName for appostrophes and fixes them to be properly added to the SQL database
            if (user.FirstName.Contains("'")) {
                user.FirstName.Replace("'", "''");
            }
            Command0.Parameters.Add(new SqlParameter("@firstname", user.FirstName));

            // Check LastName for appostrophes and fixes them to be properly added to the SQL database
            if (user.LastName.Contains("'")) {
                user.LastName.Replace("'", "''");
            }
            Command0.Parameters.Add(new SqlParameter("@lastname", user.LastName));
            Command0.Parameters.Add(new SqlParameter("@phone", user.Phone));
            Command0.Parameters.Add(new SqlParameter("@email", user.Email));
            Command0.Parameters.Add(new SqlParameter("@isReviewer", user.isReviewer));
            Command0.Parameters.Add(new SqlParameter("@isAdmin", user.isAdmin));
        }
        
        public static bool Insert(User user) {
            string Sql = String.Format(@"INSERT [user] (UserName, Password, FirstName, LastName, Phone, Email, IsReviewer, IsAdmin)" 
                                        + "VALUES(@username, @password, @firstname, @lastname, @phone, @email, @isReviewer, @isAdmin)");

            SqlCommand Command0 = CreateSQLConnection(ConnStr, Sql, "Connection didn't open ... ");
            AddSQLInsertUpdateParameters(Command0, user);

            // If there are no records affected by the method call, crash the program and throw exception
            int recsAffected = ExecuteSQLInsUpdDelCommand(Command0, "Insert failed ... ");

            // Establish the ID for the Vendor vendor object
            user.Id = GetLastIdGenerated(ConnStr, "User");

            Command0.Connection.Close();
            return (recsAffected == 1);
        }

        public static bool Update(User user) {
            string Sql = String.Format("UPDATE [user] SET "
                                            + "UserName = @username,"
                                            + "Password = @password,"
                                            + "FirstName = @firstname,"
                                            + "LastName = @lastname,"
                                            + "Phone = @phone,"
                                            + "Email = @email,"
                                            + "isReviewer = @isReviewer,"
                                            + "isAdmin = @isAdmin "
                                            + "WHERE ID = @id");

            SqlCommand Command0 = CreateSQLConnection(ConnStr, Sql, "Connection didn't open ... ");
            Command0.Parameters.Add(new SqlParameter("@id", user.Id));
            AddSQLInsertUpdateParameters(Command0, user);

            // If there are no records affected by the method call, crash the program and throw exception
            int recsAffected = ExecuteSQLInsUpdDelCommand(Command0, "Update failed ... ");

            Command0.Connection.Close();
            return (recsAffected == 1);
        }

        public static bool Delete(User user) {
            string Sql = String.Format("DELETE FROM [user] WHERE ID = @id");
            
            SqlCommand Command0 = CreateSQLConnection(ConnStr, Sql, "Connection didn't open ... ");
            Command0.Parameters.Add(new SqlParameter("@id", user.Id));

            // If there are no records affected by the method call, crash the program and throw exception
            int recsAffected = ExecuteSQLInsUpdDelCommand(Command0, "Delete failed ... ");

            Command0.Connection.Close();
            return (recsAffected == 1);
        }

        public static UserCollection Select(string WhereClause, string OrderByClause) {
            string Sql = String.Format("SELECT * FROM [user] WHERE {0} ORDER BY {1}", WhereClause, OrderByClause);

            SqlCommand Command0 = CreateSQLConnection(ConnStr, Sql, "Connection didn't open ... ");
            SqlDataReader Reader = Command0.ExecuteReader();
            if (!Reader.HasRows) {
                throw new ApplicationException("Result set has no rows ... ");
            }

            UserCollection users = new UserCollection();
            while (Reader.Read()) {
                int id = Reader.GetInt32(Reader.GetOrdinal("Id"));
                string username = Reader.GetString(Reader.GetOrdinal("UserName"));
                string password = Reader.GetString(Reader.GetOrdinal("Password"));
                string firstname = Reader.GetString(Reader.GetOrdinal("FirstName"));
                string lastname = Reader.GetString(Reader.GetOrdinal("LastName"));
                string phone = Reader.GetString(Reader.GetOrdinal("Phone"));
                string email = Reader.GetString(Reader.GetOrdinal("Email"));
                bool isreviewer = Reader.GetBoolean(Reader.GetOrdinal("IsReviewer"));
                bool isadmin = Reader.GetBoolean(Reader.GetOrdinal("IsAdmin"));

                User user = new User();
                user.Id = id;
                user.UserName = username;
                user.Password = password;
                user.FirstName = firstname;
                user.LastName = lastname;
                user.Phone = phone;
                user.Email = email;
                user.isReviewer = isreviewer;
                user.isAdmin = isadmin;

                users.Add(user);
            }

            Command0.Connection.Close();
            return users;
        }

        public static User Select(int Id) {
            UserCollection users = Select($"Id =  + {Id}", "Id");
            User user = (users.Count == 1) ? users[0] : null;
            return user;
        }
    }
}
