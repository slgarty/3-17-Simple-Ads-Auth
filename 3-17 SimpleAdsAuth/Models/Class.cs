using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace _3_17_SimpleAdsAuth.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string HashPassword { get; set; }
    }

    public class SimpleAd
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime DateListed { get; set; }
        public string PhoneNumber { get; set; }
        public string Details { get; set; }
        public int UserId { get; set; }
    }
    public class AdDb
    {
        private readonly string _connectionString;
        public AdDb(string connectionString)
        {
            _connectionString = connectionString;
        }
        public List<SimpleAd> GetSimpleAds()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = ("select * from SimpleAds order by datelisted desc");
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            var simpleAds = new List<SimpleAd>();
            while (reader.Read())
            {
                simpleAds.Add(new SimpleAd
                {
                    Id = (int)reader["id"],
                    Details = (string)reader["Details"],
                    DateListed = (DateTime)reader["dateListed"],
                    PhoneNumber = (string)reader["phoneNumber"],
                    UserId = (int)reader["userId"],

                });
            }
            return simpleAds;
        }

        public void AddAd(SimpleAd simpleAd)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = ("insert into simpleAds( DateListed, PhoneNumber, Details, UserId) values(GETDATE(), @PhoneNumber, @Details, @userId) select scope_identity()");
                cmd.Parameters.AddWithValue("@PhoneNumber", simpleAd.PhoneNumber);
                cmd.Parameters.AddWithValue("@Details", simpleAd.Details);
                cmd.Parameters.AddWithValue("@UserId", simpleAd.UserId);

                connection.Open();
              simpleAd.Id = (int)(decimal)cmd.ExecuteScalar();
            }
        }
        public void DeleteAdd(int id)
        {

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = ("delete  from SimpleAds where Id= @id");
                cmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public void AddUser(User user, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Users (Name, Email, HashPassword) " +
                              "VALUES (@name, @email, @HashPassword)";
            user.HashPassword = BCrypt.Net.BCrypt.HashPassword(password);

            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@HashPassword", user.HashPassword);
            connection.Open();
            cmd.ExecuteNonQuery();

        }
        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }

            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.HashPassword);
            return isValid ? user : null;

        }
        public User GetByEmail(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Users WHERE Email = @email";
            cmd.Parameters.AddWithValue("@email", email);
            connection.Open();
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new User
            {
                Id = (int)reader["Id"],
                Name = (string)reader["Name"],
                Email = (string)reader["Email"],
                HashPassword = (string)reader["HashPassword"]
            };
        }
    }
  
    public class HomeViewModel
    {
        public List<SimpleAd> SimpleAds { get; set; }
        public User CurrentUser { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
