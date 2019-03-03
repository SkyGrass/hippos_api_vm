using System;

namespace Ams.Data
{
	public interface IEntityFactory
	{
		object Create(Type type);
	}
}
