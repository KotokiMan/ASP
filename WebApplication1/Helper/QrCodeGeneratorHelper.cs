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
using ZXing;
using ZXing.Common;
using ZXing.OneD;
using ZXing.Rendering;

namespace Pbs.Ctc.Web.Helpers
{
    public class QrCodeGeneratorHelper
    {
        private static float _headerSize = 70f;
        private static float _footerSize = 80f;
        private static int _countQrForPage = 7;

        public void AddQrCodeForPdf(Syncfusion.Pdf.PdfDocument doc, string url)
        {            
            Bitmap[] qr = QrCodeGen(url);
            int q = 0;
            foreach (Syncfusion.Pdf.PdfPage page in doc.Pages)
            {
                page.Graphics.DrawPdfTemplate(AddHeader(page, qr[0]), new Syncfusion.Drawing.PointF());
                
            }    
        }

        private static Syncfusion.Pdf.Graphics.PdfTemplate AddHeader(Syncfusion.Pdf.PdfPage page, Bitmap qr)
        {
            Syncfusion.Drawing.RectangleF bounds = new Syncfusion.Drawing.RectangleF(0f, 0f, page.GetClientSize().Width, _headerSize);
            Syncfusion.Pdf.Graphics.PdfTemplate header = new Syncfusion.Pdf.Graphics.PdfTemplate(bounds);
            MemoryStream memoryStream = new MemoryStream();
            qr.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            Syncfusion.Drawing.SizeF imageSize = new Syncfusion.Drawing.SizeF(_headerSize, _headerSize);
            Syncfusion.Drawing.PointF imageLocation = new Syncfusion.Drawing.PointF(page.GetClientSize().Width - _headerSize, 0f);
            header.Graphics.DrawImage(new Syncfusion.Pdf.Graphics.PdfBitmap(memoryStream), imageLocation, imageSize);
            return header;
        }


        private static Syncfusion.Pdf.Graphics.PdfTemplate AddFooter(Syncfusion.Pdf.PdfPage page, List<Bitmap> qr)
        {
            Syncfusion.Drawing.RectangleF bounds = new Syncfusion.Drawing.RectangleF(0f, 0f, page.GetClientSize().Width, _footerSize);
            Syncfusion.Pdf.Graphics.PdfTemplate footer = new Syncfusion.Pdf.Graphics.PdfTemplate(bounds);
            Syncfusion.Drawing.SizeF imageSize = new Syncfusion.Drawing.SizeF(_footerSize, _footerSize);
            for (int i = 0; i < qr.Count; ++i)
            {
                MemoryStream memoryStream = new MemoryStream();
                Syncfusion.Drawing.PointF imageLocation = new Syncfusion.Drawing.PointF(i * _footerSize + 2f, 0f);
                qr[i].Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                footer.Graphics.DrawImage(new Syncfusion.Pdf.Graphics.PdfBitmap(memoryStream), imageLocation, imageSize);
            }
            return footer;
        }


        public static Bitmap[] QrCodeGen(string url)
        {  
            var writer = new BarcodeWriter<Bitmap>
            {
                Format = BarcodeFormat.QR_CODE,
                Renderer = new BitmapRenderer()
            };
            Bitmap[] bitmaps = new Bitmap[1];
            bitmaps[0] = writer.Write(url);
            
            return bitmaps;
        }

        public static string[] Split(string value, int desiredLength, bool strict = false)
        {
            EnsureValid(value, desiredLength, strict);

            var stringInfo = new StringInfo(value);
            int currentLength = stringInfo.LengthInTextElements;
            if (currentLength == 0) { return new string[0]; }
            int numberOfItems = currentLength / desiredLength;
            int remaining = (currentLength > numberOfItems * desiredLength) ? 1 : 0;
            var chunks = new string[numberOfItems + remaining];
            for (var i = 0; i < numberOfItems; i++)
            {
                chunks[i] = stringInfo.SubstringByTextElements(i * desiredLength, desiredLength);
            }
            if (remaining != 0)
            {
                chunks[numberOfItems] = stringInfo.SubstringByTextElements(numberOfItems * desiredLength);
            }
            return chunks;
        }

        private static void EnsureValid(string value, int desiredLength, bool strict)
        {
            if (value == null) { throw new ArgumentNullException(nameof(value)); }
            if (value.Length == 0 && desiredLength != 0)
            {
                throw new ArgumentException($"The passed {nameof(value)} may not be empty if the {nameof(desiredLength)} <> 0");
            }
            var info = new StringInfo(value);
            int valueLength = info.LengthInTextElements;
            if (valueLength != 0 && desiredLength < 1) { throw new ArgumentException($"The value of {nameof(desiredLength)} needs to be > 0"); }
            if (strict && (valueLength % desiredLength != 0))
            {
                throw new ArgumentException($"The passed {nameof(value)}'s length can't be split by the {nameof(desiredLength)}");
            }
        }
    }
    public class BitmapRenderer : IBarcodeRenderer<Bitmap>
    {
        /// <summary>
        /// Gets or sets the foreground color.
        /// </summary>
        /// <value>The foreground color.</value>
        public System.Drawing.Color Foreground { get; set; }

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        /// <value>The background color.</value>
        public System.Drawing.Color Background { get; set; }

#if !WindowsCE
        /// <summary>
        /// Gets or sets the resolution which should be used to create the bitmap
        /// If nothing is set the current system settings are used
        /// </summary>
        public float? DpiX { get; set; }

        /// <summary>
        /// Gets or sets the resolution which should be used to create the bitmap
        /// If nothing is set the current system settings are used
        /// </summary>
        public float? DpiY { get; set; }
#endif

        /// <summary>
        /// Gets or sets the text font.
        /// </summary>
        /// <value>
        /// The text font.
        /// </value>
        public Font TextFont { get; set; }

        private static readonly Font DefaultTextFont;

        static BitmapRenderer()
        {
            try
            {
                DefaultTextFont = new Font("Arial", 10, FontStyle.Regular);
            }
            catch (Exception exc)
            {
                // have to ignore, no better idea
#if !WindowsCE
                System.Diagnostics.Trace.TraceError("default text font (Arial, 10, regular) couldn't be loaded: {0}", exc.Message);
#endif
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapRenderer"/> class.
        /// </summary>
        public BitmapRenderer()
        {
            Foreground = System.Drawing.Color.Black;
            Background = System.Drawing.Color.White;
            TextFont = DefaultTextFont;
        }

        /// <summary>
        /// Renders the specified matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="format">The format.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public Bitmap Render(BitMatrix matrix, BarcodeFormat format, string content)
        {
            return Render(matrix, format, content, null);
        }

        /// <summary>
        /// Renders the specified matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="format">The format.</param>
        /// <param name="content">The content.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        virtual public Bitmap Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
        {
            var width = matrix.Width;
            var height = matrix.Height;
            var font = TextFont ?? DefaultTextFont;
            var emptyArea = 0;
            var outputContent = font != null &&
                                (options == null || !options.PureBarcode) &&
                                !String.IsNullOrEmpty(content) &&
                                (format == BarcodeFormat.CODE_39 ||
                                 format == BarcodeFormat.CODE_93 ||
                                 format == BarcodeFormat.CODE_128 ||
                                 format == BarcodeFormat.EAN_13 ||
                                 format == BarcodeFormat.EAN_8 ||
                                 format == BarcodeFormat.CODABAR ||
                                 format == BarcodeFormat.ITF ||
                                 format == BarcodeFormat.UPC_A ||
                                 format == BarcodeFormat.UPC_E ||
                                 format == BarcodeFormat.MSI ||
                                 format == BarcodeFormat.PLESSEY);

            if (options != null)
            {
                if (options.Width > width)
                {
                    width = options.Width;
                }
                if (options.Height > height)
                {
                    height = options.Height;
                }
            }

            // calculating the scaling factor
            var pixelsizeWidth = width / matrix.Width;
            var pixelsizeHeight = height / matrix.Height;

            if (pixelsizeWidth != pixelsizeHeight)
            {
                if (format == BarcodeFormat.QR_CODE ||
                    format == BarcodeFormat.AZTEC ||
                    format == BarcodeFormat.DATA_MATRIX ||
                    format == BarcodeFormat.MAXICODE ||
                    format == BarcodeFormat.PDF_417)
                {
                    // symetric scaling
                    pixelsizeHeight = pixelsizeWidth = pixelsizeHeight < pixelsizeWidth ? pixelsizeHeight : pixelsizeWidth;
                }
            }

            // create the bitmap and lock the bits because we need the stride
            // which is the width of the image and possible padding bytes
            var bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            using (var g = Graphics.FromImage(bmp))
            {
                var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                try
                {
                    var pixels = new byte[bmpData.Stride * height];
                    var padding = bmpData.Stride - (3 * width);
                    var index = 0;
                    var color = Background;

                    // going through the lines of the matrix
                    for (int y = 0; y < matrix.Height; y++)
                    {
                        // stretching the line by the scaling factor
                        for (var pixelsizeHeightProcessed = 0; pixelsizeHeightProcessed < pixelsizeHeight; pixelsizeHeightProcessed++)
                        {
                            // going through the columns of the current line
                            for (var x = 0; x < matrix.Width; x++)
                            {
                                color = matrix[x, y] ? Foreground : Background;
                                // stretching the columns by the scaling factor
                                for (var pixelsizeWidthProcessed = 0; pixelsizeWidthProcessed < pixelsizeWidth; pixelsizeWidthProcessed++)
                                {
                                    pixels[index++] = color.B;
                                    pixels[index++] = color.G;
                                    pixels[index++] = color.R;
                                }
                            }
                            // fill up to the right if the barcode doesn't fully fit in 
                            for (var x = pixelsizeWidth * matrix.Width; x < width; x++)
                            {
                                pixels[index++] = Background.B;
                                pixels[index++] = Background.G;
                                pixels[index++] = Background.R;
                            }
                            index += padding;
                        }
                    }
                    // fill up to the bottom if the barcode doesn't fully fit in 
                    for (var y = pixelsizeHeight * matrix.Height; y < height; y++)
                    {
                        for (var x = 0; x < width; x++)
                        {
                            pixels[index++] = Background.B;
                            pixels[index++] = Background.G;
                            pixels[index++] = Background.R;
                        }
                        index += padding;
                    }
                    // fill the bottom area with the background color if the content should be written below the barcode
                    if (outputContent)
                    {
                        var textAreaHeight = font.Height;
                        emptyArea = height + 10 > textAreaHeight ? textAreaHeight : 0;

                        if (emptyArea > 0)
                        {
                            index = (width * 3 + padding) * (height - emptyArea);
                            for (int y = height - emptyArea; y < height; y++)
                            {
                                for (var x = 0; x < width; x++)
                                {
                                    pixels[index++] = Background.B;
                                    pixels[index++] = Background.G;
                                    pixels[index++] = Background.R;
                                }
                                index += padding;
                            }
                        }
                    }

                    //Copy the data from the byte array into BitmapData.Scan0
                    Marshal.Copy(pixels, 0, bmpData.Scan0, pixels.Length);
                }
                finally
                {
                    //Unlock the pixels
                    bmp.UnlockBits(bmpData);
                }

                // output content text below the barcode
                if (emptyArea > 0)
                {
                    switch (format)
                    {
                        case BarcodeFormat.EAN_8:
                            if (content.Length < 8)
                                content = OneDimensionalCodeWriter.CalculateChecksumDigitModulo10(content);
                            content = content.Insert(4, "   ");
                            break;
                        case BarcodeFormat.EAN_13:
                            if (content.Length < 13)
                                content = OneDimensionalCodeWriter.CalculateChecksumDigitModulo10(content);
                            content = content.Insert(7, "   ");
                            content = content.Insert(1, "   ");
                            break;
                    }
                    var brush = new SolidBrush(Foreground);
                    var drawFormat = new StringFormat { Alignment = StringAlignment.Center };
                    g.DrawString(content, font, brush, pixelsizeWidth * matrix.Width / 2, height - emptyArea, drawFormat);
                }
            }

            return bmp;
        }
    }
}
