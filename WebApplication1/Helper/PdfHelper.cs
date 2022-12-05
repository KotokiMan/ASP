using Org.BouncyCastle.Tls.Crypto.Impl.BC;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Tables;
using System.Drawing;
using WebApplication1.Core;
using WebApplication1.Models;

namespace WebApplication1.Helper
{
    public class PdfHelper
    {
        private readonly MyDbContext _context;
        public PdfHelper(MyDbContext context) { _context = context; }
        public void CreatePdf(int accountid)
        {
            var histiries = from t1 in _context.Clients.Where(r => r.UserAuthBankId == accountid).ToList()
                            join t2 in _context.Accounts.ToList() on t1.ID equals t2.UserId
                            join t3 in _context.BankOperation.ToList() on t2.AccountNumber equals t3.NumberAccountRecepient
                            join t4 in _context.BankOperation.ToList() on t2.AccountNumber equals t4.NumberAccountSending
                            select new DtoPdf
                            {
                                AccountNumber = t3.NumberAccountRecepient,
                                Summa = t3.TransferSum,
                                OperationType = t3.operationType,
                                Name = t1.Name,
                                SecondName =t1.SecondName,
                                LastName = t1.LastName,
                                balance = t2.Balance
                            };



            var file = File.Create("./re.pdf");
            PdfDocument document = new PdfDocument();
            
            //Add a page
            PdfPage page = document.Pages.Add();

            //Acquire pages graphics
            PdfGraphics graphics = page.Graphics;

            //Declare a PdfLightTable
            PdfLightTable pdfLightTable = new PdfLightTable();

            //Set the Data source as direct
            pdfLightTable.DataSourceType = PdfLightTableDataSourceType.TableDirect;

            //Create columns
            pdfLightTable.Columns.Add(new PdfColumn("ФИО"));
            pdfLightTable.Columns.Add(new PdfColumn("Номер счёта"));
            pdfLightTable.Columns.Add(new PdfColumn("Сумма"));
            pdfLightTable.Columns.Add(new PdfColumn("Тип транзакции"));

            foreach(var item in histiries)
            {
                pdfLightTable.Rows.Add(new object[] { item.FullName, item.AccountNumber, item.Summa,item.OperationType });

            }


            pdfLightTable.Rows.Add(new object[] { "111", "Maxim", "III","56" });

            //Draw the PdfLightTable
            pdfLightTable.Draw(page, PointF.Empty);

            //Save the document
            document.Save("Output.pdf");
            
        }
    }
}
