using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;
using System.IO;
using System.Runtime.InteropServices;

namespace EscapeRoomCP
{
    class Utils
    {
        public static string filename;
        public static string GetVideoFromPpt(string _filename)
        {
            filename = _filename;

            string rv = Path.GetTempPath() + "videoFromppt.wmv";

            Microsoft.Office.Interop.PowerPoint.Application objApp;
            Microsoft.Office.Interop.PowerPoint.Presentation objPres;
            objApp = new Microsoft.Office.Interop.PowerPoint.Application();
            //objApp.Visible = Microsoft.Office.Core.MsoTriState.msoFalse;
            objApp.WindowState = Microsoft.Office.Interop.PowerPoint.PpWindowState.ppWindowMinimized;
            
            objPres = objApp.Presentations.Open(filename, MsoTriState.msoTrue, MsoTriState.msoTrue, MsoTriState.msoTrue);
            Console.WriteLine("File name: " + filename);
            try
            {
                objPres.SaveAs(rv, Microsoft.Office.Interop.PowerPoint.PpSaveAsFileType.ppSaveAsWMV, MsoTriState.msoTrue);
                // Wait for creation of video file
                while (objApp.ActivePresentation.CreateVideoStatus == Microsoft.Office.Interop.PowerPoint.PpMediaTaskStatus.ppMediaTaskStatusInProgress || objApp.ActivePresentation.CreateVideoStatus == Microsoft.Office.Interop.PowerPoint.PpMediaTaskStatus.ppMediaTaskStatusQueued)
                {
                    //Application.DoEvents();
                    System.Threading.Thread.Sleep(500);
                }

                //txtStat.Text = "Done";
                objPres.Close();
                objApp.Quit();
                // Release COM Objects
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(objPres);
                objPres = null;
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(objApp);
                objApp = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return rv;

            /*
            var app = new Microsoft.Office.Interop.PowerPoint.Application();
            var presentation = app.Presentations.Open(filename, MsoTriState.msoTrue, MsoTriState.msoTrue, MsoTriState.msoFalse);

            var wmvfile = Guid.NewGuid().ToString() + ".wmv";
            var fullpath = filename;

            try
            {
                presentation.CreateVideo(wmvfile);
                presentation.SaveCopyAs(fullpath, Microsoft.Office.Interop.PowerPoint.PpSaveAsFileType.ppSaveAsWMV, MsoTriState.msoCTrue);
            }
            catch (COMException)
            {
                wmvfile = null;
            }
            finally
            {
                app.Quit();
            }

            return wmvfile;
            */
        }

        public static void deleteVideoFromPpt()
        {
            File.Delete(filename);
        }
    }
}
