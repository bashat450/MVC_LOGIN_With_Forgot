using CrudMap.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CrudMap
{
    public class Logic
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["image"].ToString());
        #region === Display All Details ===
        public List<AllModel> GetAllDetails()
        {
            List<AllModel> allDetails = new List<AllModel>();
            try
            {
                SqlCommand cmd = new SqlCommand("[dbo].[AllTeachersDetail_SP]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                dataAdapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    AllModel model = new AllModel
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Name = Convert.ToString(row["Name"]),
                        ImagePath = Convert.ToString(row["ImagePath"]),
                        PdfPath = Convert.ToString(row["PdfPath"])


                    };
                    allDetails.Add(model);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errors in Rows..." + ex.Message);
            }
            return allDetails;
        }
        #endregion

        #region === Create New Records ===
        public void CreateDetails(AllModel createModel)
        {
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("[dbo].[InsertRecords_SP]", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", createModel.Id);
                cmd.Parameters.AddWithValue("@Name", createModel.Name);
                cmd.Parameters.AddWithValue("@ImagePath", createModel.ImagePath);
                cmd.Parameters.AddWithValue("@PdfPath", createModel.PdfPath);


                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errors in rows..." + ex.Message);
            }
        }
        #endregion

        #region === Edit Some Records ===
        public void EditDetails(AllModel editModel)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[UpdateRecords_SP]", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", editModel.Id);
                cmd.Parameters.AddWithValue("@Name", editModel.Name);
                cmd.Parameters.AddWithValue("@ImagePath", editModel.ImagePath);
                cmd.Parameters.AddWithValue("@PdfPath", editModel.PdfPath);


                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region === Delete Some Details ===
        public void DeleteDetails(int id)
        {
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("[dbo].[DeleteRecord_SP]", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", id);

                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errors in rows..." + ex.Message);
            }
        }
        #endregion
    }
}