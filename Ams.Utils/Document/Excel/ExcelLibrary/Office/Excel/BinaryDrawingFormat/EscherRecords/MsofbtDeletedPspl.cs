﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Ams.Utils.ExcelLibrary.BinaryDrawingFormat
{
	public partial class MsofbtDeletedPspl : EscherRecord
	{
		public MsofbtDeletedPspl(EscherRecord record) : base(record) { }

		public MsofbtDeletedPspl()
		{
			this.Type = EscherRecordType.MsofbtDeletedPspl;
		}

	}
}
