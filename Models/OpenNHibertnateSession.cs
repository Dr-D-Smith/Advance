using NHibernate;
using NHibernate.Cfg;
using System;
using System.Web;

namespace AdvanceTest
{
	public class OpenNHibertnateSession
	{
		public static ISession OpenSession()
		{
			Configuration configuration = new Configuration();
			string configurationPath = HttpContext.Current.Server.MapPath("~\\Models\\NHibernate\\Configuration.xml");
			configuration.Configure(configurationPath);
			string configurationFile = HttpContext.Current.Server.MapPath("~\\Models\\Nhibernate\\ExplanatoryNote.hbm.xml");
			configuration.AddFile(configurationFile);
			ISessionFactory sessionFactory = configuration.BuildSessionFactory();
			return sessionFactory.OpenSession();
		}
	}
}
