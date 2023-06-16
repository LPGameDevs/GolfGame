using Godot;

namespace GolfGame
{
    public class LoadingSpinner : TextureRect
    {
        private float _rotationSpeed = 0.5f;
        private float _rotation = 0f;

        public override void _Process(float delta)
        {
            _rotation += _rotationSpeed;
            if (_rotation > 360)
            {
                _rotation = 0;
            }

            RectRotation = _rotation;
        }
    }
}
