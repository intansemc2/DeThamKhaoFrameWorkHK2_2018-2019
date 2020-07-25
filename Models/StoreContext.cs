using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeThamKhaoHK2_2018_2019.Models
{
    public class StoreContext
    {
        private static String connectionString = "server=127.0.0.1;user id=root;password=;port=3306;database=playlist;";

        //Model Cau01: Trang thêm bài hát
        public int ThemBaiHat(BaiHat bh)
        {
            using (var conn = new MySqlConnection(StoreContext.connectionString)) 
            {
                conn.Open();

                String query = "INSERT INTO BAIHAT (MaBaiHat, TenBaiHat, TheLoai) VALUES (@MaBaiHat,@TenBaiHat,@TheLoai)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("MaBaiHat", bh.MaBaiHat);
                cmd.Parameters.AddWithValue("TenBaiHat", bh.TenBaiHat);
                cmd.Parameters.AddWithValue("TheLoai", bh.TheLoai);
                return (cmd.ExecuteNonQuery());
            }
        }

        //Model Cau02: Dùng kỹthuật lập trình Ajax đểliệt kê danh sách ca sĩ theo ngày tháng năm sinh
        public List<CaSi> GetCaSiNgayThang(String NgayThang) 
        {
            List<CaSi> casis =new List<CaSi>();
            using (var conn = new MySqlConnection(StoreContext.connectionString)) 
            {
                conn.Open();

                String query = "SELECT TenCaSi FROM CASI WHERE NamSinh = @NamSinh";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("NamSinh", NgayThang);
                using(MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {          
                        casis.Add(new CaSi() { TenCaSi = reader["TenCaSi"] as String });
                    }
                }
            }

            return casis;
        }

         //Model Cau03: Liệt kê danh sách người dùng vào một combobox.
        public List<NguoiNghe> GetNguoiNghes() 
        {
            List<NguoiNghe> nns =new List<NguoiNghe>();
            using (var conn = new MySqlConnection(StoreContext.connectionString)) 
            {
                conn.Open();

                String query = "SELECT MaNN, TenNN FROM NGUOINGHE";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                using(MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {          
                        nns.Add(new NguoiNghe() { MaNN = reader["MaNN"] as String, TenNN = reader["TenNN"] as String });
                    }
                }
            }

            return nns;
        }

        //Model Cau03: Dùng kỹthuật lập trình Ajax đểliệt kê các Playlist vào một bảng bên dưới.
        public List<PlayList> GetPlaylistByMaNN(String MaNN) 
        {
            List<PlayList> playlists = new List<PlayList>();
            using (var conn = new MySqlConnection(StoreContext.connectionString)) 
            {
                conn.Open();

                String query = "SELECT MaPlayList, TenPlayList FROM PLAYLIST WHERE MaNN = @MaNN";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("MaNN", MaNN);
                using(MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {          
                        playlists.Add(new PlayList() { MaPlayList = reader["MaPlayList"] as String, TenPlayList = reader["TenPlayList"] as String });
                    }
                }
            }

            return playlists;
        }

        public int DeletePlaylistAndSongByMaPlayList(String MaPlayList)
        {
            using (var conn = new MySqlConnection(StoreContext.connectionString)) 
            {
                conn.Open();

                //Khai báo biến 
                String query = null;
                MySqlCommand cmd = null;

                List<String> MaBaiHats = new List<String>();

                //Lấy danh sách bài hát của playlist                
                query = "SELECT BAIHAT.MaBaiHat FROM BAIHAT JOIN PLAYLIST_BAIHAT ON BAIHAT.MaBaiHat = PLAYLIST_BAIHAT.MaBaiHat WHERE PLAYLIST_BAIHAT.MaPlayList = @MaPlayList";
                cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("MaPlayList", MaPlayList);
                using(MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {          
                        MaBaiHats.Add(reader["MaBaiHat"] as String );
                    }
                }

                if (MaBaiHats.Count > 0)
                {
                    //Xóa Playlist_Baihat 
                    query = "DELETE FROM PLAYLIST_BAIHAT WHERE PLAYLIST_BAIHAT.MaPlayList = @MaPlayList";
                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("MaPlayList", MaPlayList);
                    cmd.ExecuteNonQuery();

                    //Xoá Ca sĩ_bài hát 
                    query = "DELETE FROM CASI_BAIHAT WHERE CASI_BAIHAT.MaBaiHat IN (";
                    query += " \'" + MaBaiHats[0] + "\'";
                    for(int i=1; i<MaBaiHats.Count; i+=1)
                    {
                        query += ", \'" + MaBaiHats[i] + "\'";
                    }
                    query += ")";
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    //Xóa Bài hát                     
                    query = "DELETE FROM BAIHAT WHERE BAIHAT.MaBaiHat IN (";
                    query += " \'" + MaBaiHats[0] + "\'";
                    for(int i=1; i<MaBaiHats.Count; i+=1)
                    {
                        query += ", \'" + MaBaiHats[i] + "\'";
                    }
                    query += ")";
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                }

                //Xóa Playlist 
                query = "DELETE FROM PLAYLIST WHERE PLAYLIST.MaPlayList = @MaPlayList";
                cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("MaPlayList", MaPlayList);
                return (cmd.ExecuteNonQuery());
            }
             
        }

        public List<CaSi> GetCaSiHatTatCaBaiHat() 
        {
            List<CaSi> casis = new List<CaSi>();
            using (var conn = new MySqlConnection(StoreContext.connectionString)) 
            {
                conn.Open();

                String query = "SELECT CASI.TenCaSi FROM CASI WHERE NOT EXISTS ( SELECT * FROM BAIHAT WHERE NOT EXISTS ( SELECT * FROM CASI_BAIHAT WHERE BAIHAT.MaBaiHat = CASI_BAIHAT.MaBaiHat AND CASI_BAIHAT.MaCaSi = CASI.MaCaSi ) ) ORDER BY CASI.MaCaSi ASC ";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                using(MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {          
                        casis.Add(new CaSi() { TenCaSi = reader["TenCaSi"] as String });
                    }
                }
            }

            return casis;
        }

        public List<BaiHat_SoLanXuatHien> GetBaiHatSoLanXuatHien() 
        {
            List<BaiHat_SoLanXuatHien> bhslxhs = new List<BaiHat_SoLanXuatHien>();
            using (var conn = new MySqlConnection(StoreContext.connectionString)) 
            {
                conn.Open();

                String query = "SELECT BAIHAT.TenBaiHat, COUNT(PLAYLIST_BAIHAT.MaPlayList) AS SoLanXuatHien FROM BAIHAT JOIN PLAYLIST_BAIHAT ON PLAYLIST_BAIHAT.MaBaiHat = BAIHAT.MaBaiHat GROUP BY BAIHAT.MaBaiHat ORDER BY COUNT(PLAYLIST_BAIHAT.MaPlayList) DESC LIMIT 10 ";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                using(MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {          
                        bhslxhs.Add(new BaiHat_SoLanXuatHien() { TenBaiHat = reader["TenBaiHat"] as String, SoLanXuatHien = Convert.ToInt32(reader["SoLanXuatHien"]) });
                    }
                }
            }
            return bhslxhs;
        }
    }
}
