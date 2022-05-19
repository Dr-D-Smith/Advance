using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Xsl;

namespace AdvanceTest.Controllers
{
	public class HomeController : Controller
	{
		private List<ExplanatoryNote> getNotes()
		{
			List<ExplanatoryNote> result;
			using (ISession session = OpenNHibertnateSession.OpenSession())
			{
				result = session.Query<ExplanatoryNote>().ToList<ExplanatoryNote>();
			}
			return result;
		}

		private ExplanatoryNote getNote(int id)
		{
			ExplanatoryNote result;
			using (ISession session = OpenNHibertnateSession.OpenSession())
			{
				result = session.Get<ExplanatoryNote>(id);
			}
			return result;
		}

		public ActionResult Index()
		{
			return base.View(this.getNotes());
		}

		public ActionResult Details(int id = 0)
		{
			return base.View(this.getNote(id));
		}

		[HttpPost]
		public ActionResult Create(ExplanatoryNote note)
		{
			using (ISession session = OpenNHibertnateSession.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					note.CreatedDate = DateTime.Now.Date;
					session.Save(note);
					transaction.Commit();
				}
			}

            return base.RedirectToAction("Index", this.getNotes());
        }

        [HttpGet]
		public ActionResult Edit(int id = 0)
		{
			return this.PartialView("~/Views/Home/NoteFields.cshtml", this.getNote(id));
		}

		[HttpPost]
		public ActionResult Update(int? id, ExplanatoryNote n)
		{
			using (ISession session = OpenNHibertnateSession.OpenSession())
			{
				ExplanatoryNote note = session.Get<ExplanatoryNote>(id);
				note.Action = n.Action;
				note.Actor = n.Actor;
				note.Description = n.Description;
				note.FromName = n.FromName;
				note.FromPosition = n.FromPosition;
				note.ToName = n.ToName;
				note.ToPosition = n.ToPosition;
				
				using (ITransaction transaction = session.BeginTransaction())
				{
					session.Save(note);
					transaction.Commit();
				}
			}
			return base.RedirectToAction("Index", this.getNotes());
		}

		[ActionName("Delete"), HttpPost]
		public ActionResult DeleteConfirmed(int id)
		{
			using (ISession session = OpenNHibertnateSession.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					session.Delete(this.getNote(id));
					transaction.Commit();
				}
			}
			return base.RedirectToAction("Index", this.getNotes());
		}

		private XmlDocument getXML(string file)
		{
			XmlDocument xmlBody = new XmlDocument();
			string xmlPath = Path.Combine(base.Server.MapPath("/Content/"), file);
			xmlBody.LoadXml(xmlPath);
			return xmlBody;
		}

		private MemoryStream getTemplate(string path)
		{
			Stream templateDocumentReadStream = System.IO.File.OpenRead(path);
			BinaryReader templateDocumentBinaryReader = new BinaryReader(templateDocumentReadStream);
			byte[] templateDocumentByteArray = templateDocumentBinaryReader.ReadBytes(Convert.ToInt32(templateDocumentReadStream.Length));
			
			MemoryStream templateDocumentStream = new MemoryStream();
			templateDocumentStream.Write(templateDocumentByteArray, 0, templateDocumentByteArray.Length);
			return templateDocumentStream;
		}

		private string getXSLT(string file)
		{
			string xsltPath = Path.Combine(base.Server.MapPath("/Content/"), file);
			return System.IO.File.ReadAllText(xsltPath);
		}

		[HttpGet]
		public FileResult GetDoc(int id = 0)
		{
			base.Response.ContentType = "text/xml";
			string xsltBody = this.getXSLT("document.xslt");

			MemoryStream templateDocumentStream = this.getTemplate(Path.Combine(base.Server.MapPath("/Content/"), "template.docx"));
			
			string xmlData = this.getNote(id).ToXml();
            XmlDocument xmlDataBody = new XmlDocument();
			xmlDataBody.LoadXml(xmlData);

            GenerateWordDocument(xmlDataBody.OuterXml, xsltBody, ref templateDocumentStream);
			byte[] fileContent = templateDocumentStream.ToArray();
			templateDocumentStream.Close();
			
			base.Response.Buffer = true;
			base.Response.AddHeader("Content-Disposition", "filename=result.docx");
			return this.File(fileContent, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "result.docx");
		}

		private static void GenerateWordDocument(string xmlBody, string xsltBody, ref MemoryStream templateDocumentStream)
		{
			StringWriter sw = new StringWriter();
			XmlWriter xw = XmlWriter.Create(sw);
			
			XslCompiledTransform transform = new XslCompiledTransform();
			transform.Load(new XmlTextReader(new StringReader(xsltBody)));
			transform.Transform(XmlReader.Create(new StringReader(xmlBody)), xw);
			
			XmlDocument wordBody = new XmlDocument();
			wordBody.LoadXml(sw.ToString());
			
			using (WordprocessingDocument output = WordprocessingDocument.Open(templateDocumentStream, true))
			{
				Body updatedBodyContent = new Body(wordBody.DocumentElement.InnerXml);
				output.MainDocumentPart.Document.Body = updatedBodyContent;
				output.Close();
			}
		}
	}
}
