using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Test1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusiciansController : ControllerBase
    {

        [HttpGet("{IdMusician}")]
        public IActionResult GetMusician(int IdMusician)
        {
            List<string> tracks = new List<string>();
            var m = new Musician();

            using (var client = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True"))
            using (var command = new SqlCommand())
            {
                command.Connection = client;
                command.CommandText = "SELECT m.IdMusician, m.FirstName, m.LastName, m.Nickname, t.TrackName"+
                                       "FROM Musician m JOIN Musician_Track mt ON m.IdMusician = mt.IdMusician"+
                                       "JOIN Track t ON t.IdTrack = mt.IdTrack"+
                                       "WHERE m.IdMusician = @IdMusician";
                command.Parameters.AddWithValue("IdMusician", IdMusician);

                client.Open();
                var dr = command.ExecuteReader();
                
                while (dr.Read())
                {
                    m.IdMusician = IdMusician;
                    m.FirstName = dr["FirstName"].ToString();
                    m.LastName = dr["LastName"].ToString();
                    m.Nickname = dr["Nickname"].ToString();
                    tracks.Add(dr["TrackName"].ToString());
                    
                  
                }
                tracks = tracks.OrderBy(q => q).ToList();
                m.Tracks = tracks;

            }
            return Ok(m);
        }

        [HttpPost]
        public IActionResult AddMusician(Musician m)
        {
            string FirstName = m.FirstName;
            string LastName = m.LastName;
            string Nickname = m.Nickname;
            List<int> IdTracks = m.IdTracks;

            using (var client = new SqlConnection("Data Source = db - mssql; Initial Catalog = 2019SBD; Integrated Security = True"))
            using (var command = new SqlCommand())
            {
                command.Connection = client;
              
                
                command.CommandText = "INSERT INTO Musician(FirstName, LastName, Nickname)"+
                                        "VALUES('@FirstName', @LastName, @Nickname)";
                command.Parameters.AddWithValue("FirstName", FirstName);
                command.Parameters.AddWithValue("LastName", LastName);
                command.Parameters.AddWithValue("Nickname", Nickname);

                command.ExecuteNonQuery();

                client.Open();
                var dr = command.ExecuteReader();

                command.CommandText = "SELECT IdMusician FROM Musician WHERE FirstName = @FirstName AND LastName = @LastName";
                dr = command.ExecuteReader();

                int IdMusician = 0;
                while (dr.Read())
                {
                    IdMusician = Convert.ToInt32(dr[0]);
                }




                foreach (int i in IdTracks)
                {
                    command.CommandText = "SELECT COUNT(IdTrack)" +
                                        "FROM Musician_Track" +
                                        "WHERE IdTrack = @i";
                    command.Parameters.AddWithValue("i", i);

                   

                    while (dr.Read())
                    {
                        if (Convert.ToInt32(dr[0]) != 0)
                        {
                            return BadRequest("These tracks are already assigned to a different musician");
                        } else
                        {
                            command.CommandText = "INSERT INTO Musician_Track(IdTrack, IdMusician)" +
                                            "VALUES(@i, @IdMusician)";
                            command.Parameters.AddWithValue("IdMusician", IdMusician);
                            command.Parameters.AddWithValue("i", i);
                            command.ExecuteNonQuery();
                        }

                    }
                        
                }
            }
            return Ok();
        }
    }
}
