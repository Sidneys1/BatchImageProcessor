namespace BatchImageProcessor.Types
{
	public struct ModelProgressUpdate
	{
		public int Total;
		public int Done;

		public ModelProgressUpdate(int total, int done)
		{
			Total = total;
			Done = done;
		}

	}
}