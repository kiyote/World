using System.Text;

namespace Common.Core {

	public static class Hash {
		public static long GetLong( string strText ) {
			long hashCode = 0;
			if( !string.IsNullOrEmpty( strText ) ) {
				//Unicode Encode Covering all characterset
				byte[] byteContents = Encoding.Unicode.GetBytes( strText );
				byte[] hashText = System.Security.Cryptography.SHA256.HashData( byteContents );
				//32Byte hashText separate
				//hashCodeStart = 0~7  8Byte
				//hashCodeMedium = 8~23  8Byte
				//hashCodeEnd = 24~31  8Byte
				//and Fold
				long hashCodeStart = BitConverter.ToInt64( hashText, 0 );
				long hashCodeMedium = BitConverter.ToInt64( hashText, 8 );
				long hashCodeEnd = BitConverter.ToInt64( hashText, 24 );
				hashCode = hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;
			}
			return ( hashCode );
		}
	}
}

