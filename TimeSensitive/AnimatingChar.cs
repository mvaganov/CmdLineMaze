namespace TimeSensitive {
	public class AnimatingChar {
		public string animation;
		public int index;
		public int msDelayPerFrame;
		public int timer = 0;
		public AnimatingChar(string animation, int delay = 100) {
			this.animation = animation;
			msDelayPerFrame = delay;
		}
		public int MaxIndex => animation.Length;
		public char GetCurrentChar() => animation[index];
		public bool Update(int msPassed) {
			bool change = false;
			timer += msPassed;
			while (timer >= msDelayPerFrame) {
				timer -= msDelayPerFrame;
				IncreaseIndex(1);
				change = true;
			}
			return change;
		}
		public void IncreaseIndex(int count) {
			index += count;
			while (index < 0) { index += MaxIndex; }
			while (index >= MaxIndex) { index -= MaxIndex; }
		}
		public static explicit operator char(AnimatingChar self) => self.GetCurrentChar();
		public static AnimatingChar operator ++(AnimatingChar self) {
			self.IncreaseIndex(1);
			return self;
		}
	}
}
