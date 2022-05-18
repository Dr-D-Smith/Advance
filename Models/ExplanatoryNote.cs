using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace AdvanceTest
{
	public class ExplanatoryNote
	{
		public virtual int Id
		{
			get;
			set;
		}

		public virtual string ToPosition
		{
			get;
			set;
		}

		public virtual string ToName
		{
			get;
			set;
		}

		public virtual string FromPosition
		{
			get;
			set;
		}

		public virtual string FromName
		{
			get;
			set;
		}

		public virtual string Actor
		{
			get;
			set;
		}

		public virtual string Action
		{
			get;
			set;
		}

		public virtual string Description
		{
			get;
			set;
		}

		public virtual DateTime CreatedDate
		{
			get;
			set;
		}

		public virtual string ToXml()
		{
			string xml = string.Empty;
			StreamWriter writer = null;
			try
			{
				MemoryStream stream = new MemoryStream();
				writer = new StreamWriter(stream);
				XmlSerializer serializer = new XmlSerializer(base.GetType());
				serializer.Serialize(writer, this);
				xml = Encoding.UTF8.GetString(stream.GetBuffer());
			}
			catch
			{
			}
			finally
			{
				bool flag = writer != null;
				if (flag)
				{
					writer.Close();
				}
			}
			return xml;
		}
	}
}
