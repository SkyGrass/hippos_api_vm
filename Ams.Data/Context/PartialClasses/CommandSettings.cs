﻿namespace Ams.Data
{
	public partial class DbContext
	{
		public IDbContext CommandTimeout(int timeout)
		{
			Data.CommandTimeout = timeout;
			return this;
		}
	}
}
