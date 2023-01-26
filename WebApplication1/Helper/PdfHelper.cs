using Microsoft.AspNetCore.Mvc;
using Syncfusion.Pdf;
using WebApplication1.Core;
using WebApplication1.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using Pbs.Ctc.Web.Helpers;

namespace WebApplication1.Helper
{
    public class PdfHelper
    {
        private readonly MyDbContext _context;
        public PdfHelper(MyDbContext context) { _context = context; }
        public void CreatePdf(int accountid)
        {



        }
        public FileStreamResult CreateTablePdf(int accountid)
        {
            var dataTab = from t1 in _context.Clients.Where(r => r.UserAuthBankId == accountid).ToList()
                            join t2 in _context.Accounts.ToList() on t1.ID equals t2.UserId
                            join t3 in _context.BankOperation.ToList() on t2.AccountNumber equals t3.NumberAccountRecepient
                            //join t4 in _context.BankOperation.ToList() on t2.AccountNumber equals t4.NumberAccountSending
                            select new DtoPdf
                            {
                                AccountNumber = t3.NumberAccountRecepient,
                                AccountNumberSend = t3.NumberAccountSending,
                                Summa = t3.TransferSum,
                                OperationType = t3.operationType,
                                Name = t1.Name,
                                SecondName = t1.SecondName,
                                LastName = t1.LastName,
                            };
            //Add values to the list.
            string ui=string.Empty;
            List<object> data = new List<object>();
            foreach(var item in dataTab)
            {
                ui = item.FullName;
                data.Add(new {
                    Account = item.AccountNumber,
                    SendAccount = item.AccountNumberSend, 
                    Summa = item.Summa,
                    OperationType = item.OperationType,
                    FullName = item.FullName
                });
            }
            PdfDocument doc = new PdfDocument();

            //Add a page.
            PdfPage page = doc.Pages.Add();

            //Create a PdfGrid.
            Syncfusion.Pdf.Grid.PdfGrid pdfGrid = new Syncfusion.Pdf.Grid.PdfGrid();


            //Object row1 = new { ID = "E01", Name = "Clay" };
            //Object row2 = new { ID = "E02", Name = "Thomas" };

            

            //Add list to IEnumerable.
            IEnumerable<object> dataTable = data;

            //Assign data source.
            pdfGrid.DataSource = dataTable;

            //Draw the grid to the page of PDF document.
            pdfGrid.Draw(page, new Syncfusion.Drawing.PointF(10, 70));
            string host = System.Net.Dns.GetHostName();
            var ip = "192.168.188.215";
            string url = String.Format(@"http://{0}:80/User/PdfGenerationn?{1}", ip, accountid.ToString());
            new QrCodeGeneratorHelper().AddQrCodeForPdf(doc, url);
            string name= "C:\\Project\\ASP\\WebApplication1\\PDF\\" + accountid.ToString() + ".pdf";
            //Creating the stream object.
            if (File.Exists(name))
                File.Delete(name);
            var st = new FileStream(name, FileMode.CreateNew,FileAccess.ReadWrite);

            //Save the PDF document to stream.
            doc.Save(st);

            //If the position is not set to '0,' a PDF will be empty.
            
            st.Close();
            st.Dispose();
            //Close the document.
            doc.Close();
            var fileStream = new FileStream(name, FileMode.Open);
            return new FileStreamResult(fileStream, "application/pdf") { FileDownloadName = ui + ".pdf" };
        }
        public FileStreamResult CreateTablePdff(string path)
        {



            string name = "C:\\Project\\ASP\\WebApplication1\\PDF\\" + path+".pdf";
            

            var fileStream = new FileStream(name, FileMode.Open);
            return new FileStreamResult(fileStream, "application/pdf") { FileDownloadName = "otchet" + ".pdf" };




        }
    }
}
