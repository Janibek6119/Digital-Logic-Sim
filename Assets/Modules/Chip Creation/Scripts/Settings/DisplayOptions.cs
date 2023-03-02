namespace DLS.ChipCreation
{
	[System.Serializable]
	public struct DisplayOptions
	{
		public enum PinNameDisplayMode { Always, Hover, Toggle, Never }
		public enum ToggleState { Off, On }
		public enum BackgroundGridDisplayMode { Always, Sync, Toggle, Never }

		public PinNameDisplayMode MainChipPinNameDisplayMode;
		public PinNameDisplayMode SubChipPinNameDisplayMode;
		public ToggleState ShowCursorGuide;
		public BackgroundGridDisplayMode GridDisplayMode;
	}
}