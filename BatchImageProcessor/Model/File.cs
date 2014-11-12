namespace BatchImageProcessor.Model
{
	public class File : IoObject
	{
		#region Properties

		//WeakThumbnail _thumb = null;
		public override sealed WeakThumbnail Thumbnail { get; protected set; }
		//{
		//get { return _thumb != null ? _thumb.Source : (_thumb = new WeakThumbnail(Path)).Source; }
		//}

		#endregion

		public File(string path)
			: base(path)
		{
			Thumbnail = new WeakThumbnail(path);
		}
	}
}