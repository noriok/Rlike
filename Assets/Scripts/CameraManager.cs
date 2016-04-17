public class CameraManager {
	private readonly float[] _sizes = {
		3.2f, // 9 マス
		2.8f, // 8 マス
		2.5f, // 7 マス
		2.1f, // 6 マス
		1.74f, // 5 マス
	};
	private int _index = 2;

	public float CurrentSize { get { return _sizes[_index]; } }

	public float NextSize() {
		_index++;
		if (_index == _sizes.Length) {
			_index = 0;
		}
		return _sizes[_index];
	}
}
