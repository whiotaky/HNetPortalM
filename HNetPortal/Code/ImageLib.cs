using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;
using System.Reflection;
using MySql.Data.MySqlClient;
using System.Configuration;
using WSHLib;

namespace HNetPortal {
    public static class ImageLib {

        public static void ResizeImage(string FileNameInput, string FileNameOutput, double ResizeHeight, double ResizeWidth , ImageFormat OutputFormat) {

            Logger.Log("called for file " + FileNameInput);
         
            try {

                using (System.Drawing.Image photo = new Bitmap(FileNameInput)) {
                    double aspectRatio = (double)photo.Width / photo.Height;
                    double boxRatio = ResizeWidth / ResizeHeight;
                    double scaleFactor = 0;

                    if (photo.Width < ResizeWidth && photo.Height < ResizeHeight) {
                        // keep the image the same size since it is already smaller than our max width/height
                        scaleFactor = 1.0;
                    } else {
                        if (boxRatio > aspectRatio)
                            scaleFactor = ResizeHeight / photo.Height;
                        else
                            scaleFactor = ResizeWidth / photo.Width;
                    }

                    int newWidth = (int)(photo.Width * scaleFactor);
                    int newHeight = (int)(photo.Height * scaleFactor);

                    using (Bitmap bmp = new Bitmap(newWidth, newHeight)) {
                        using (Graphics g = Graphics.FromImage(bmp)) {
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = SmoothingMode.HighQuality;
                            g.CompositingQuality = CompositingQuality.HighQuality;
                            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                            g.DrawImage(photo, 0, 0, newWidth, newHeight);

                            if (ImageFormat.Png.Equals(OutputFormat)) {
                                bmp.Save(FileNameOutput, OutputFormat);
                            } else if (ImageFormat.Jpeg.Equals(OutputFormat)) {
                                ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();
                                EncoderParameters encoderParameters;
                                using (encoderParameters = new System.Drawing.Imaging.EncoderParameters(1)) {
                                    // use jpeg info[1] and set quality to 90
                                    encoderParameters.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
                                    bmp.Save(FileNameOutput, info[1], encoderParameters);
                                }
                            }
                        }
                    }


                }

            } catch (Exception ex) {
                Logger.LogException("ImageLib.ResizeImage Exception: ", ex);

            }

        }


        public static List<string> loadPublicDirList() {
            List<string>  publicDirs = new List<string>();
           // publicDirs.Add("/2015");
           // publicDirs.Add("/0001a");
           // publicDirs.Add("/2016/Shower");
           // return publicDirs;


            MySqlConnection conn = new MySqlConnection();         
            try {

                conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select dirname from imbasedirperm where publicaccess='Y'", conn);
                cmd.Prepare();
                   
                MySqlDataReader reader;
                reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    string dirName = (string)reader[0];
                    publicDirs.Add(dirName);     
                }

            } catch (Exception ex) {
                Logger.LogException("imbasedirperm: ", ex);
               
            } finally {
                conn.Close();
            }

            return publicDirs;


        }


        //Not currently used!
        public static ImageFormat GetImageFormat(string extension) {

            if (extension.StartsWith(".")) {
                extension = extension.Substring(1);
            }

            ImageFormat result = null;
            PropertyInfo prop = typeof(ImageFormat).GetProperties().Where(p => p.Name.Equals(extension, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (prop != null) {
                result = prop.GetValue(prop, null) as ImageFormat;
            }
            return result;
        }



    }
}