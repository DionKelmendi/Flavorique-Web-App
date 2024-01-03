namespace Flavorique_Web_App
{
	public class HTMLToPDFConverter
	{
		public byte[] ConvertHTMLToPDF(string? body, string? title)
		{
			TXTextControl.ServerTextControl tx = new TXTextControl.ServerTextControl();
			byte[] bPDF;

			tx.Create();

			// Load the HTML String
			tx.Load(body, TXTextControl.StringStreamType.HTMLFormat);

			// Save as PDF
			tx.Save(out bPDF, TXTextControl.BinaryStreamType.AdobePDF);
			
			// Return byte array for user download
			return bPDF;
		}
	}
}
