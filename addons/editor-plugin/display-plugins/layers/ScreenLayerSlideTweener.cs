using Godot;
using PinGod.Base;

namespace PinGod.EditorPlugins
{
    /// <summary>
    /// Simple screen slider as used in older DMD games. Slide in from a direction, pause then slide out. <para/>
    /// </summary>
    public partial class ScreenLayerSlideTweener : Control
    {
        [Export] float _inTime = 1f;
        [Export] double _pauseTime = 1;
        [Export] float _outTime = 1f;
        [Export] Direction _inDirection = Direction.West;
        [Export] Direction _outDirection = Direction.East;

        private Label _titleLabel;
        private Label _pointsLabel;

        public ScreenLayerSlideTweener()
        {

        }

        /// <summary>
        /// Create sliding background from script
        /// </summary>
        /// <param name="titleLabel"></param>
        /// <param name="pointsLabel"></param>
        /// <param name="inTime"></param>
        /// <param name="pauseTime"></param>
        /// <param name="outTime"></param>
        /// <param name="inDirection"></param>
        /// <param name="outDirection"></param>
        public ScreenLayerSlideTweener(Label titleLabel, Label pointsLabel, float inTime = 1f, double pauseTime = 1, float outTime = 1f,
            Direction inDirection = Direction.West, Direction outDirection = Direction.East)
        {
            _inTime = inTime;
            _pauseTime = pauseTime;
            _outTime = outTime;
            _inDirection = inDirection;
            _outDirection = outDirection;
            _titleLabel = titleLabel;
            _pointsLabel = pointsLabel;
        }

        public override void _EnterTree()
        {
            base._EnterTree();
            _titleLabel = GetNodeOrNull<Label>("ColorRect/Label2");
            _pointsLabel = GetNodeOrNull<Label>("ColorRect/Points");
        }

        /// <summary>
        /// Create a tween of whole control, like sliding classic pinball
        /// </summary>
        public override void _Ready()
        {
            float startX = Position.x;
            float startY = Position.y;
            switch (_inDirection)
            {
                case Direction.North:
                    startY = Position.y - Size.y;
                    break;
                case Direction.South:
                    startY = Position.y + Size.y;
                    break;
                case Direction.East:
                    startX = Position.x + Size.x; //start end of screen
                    break;
                case Direction.West:
                    startX = Position.x - Size.x; //start left of screen
                    break;
            }

            //move the layer to starting position off screen
            SetPosition(new Vector2(startX, startY));

            //we are moving to 0,0 full screen slides, then pause, then the out direction
            float endX = Position.x; float endY = Position.y;
            switch (_outDirection)
            {
                case Direction.North:
                    endY = (0 - Size.y);
                    break;
                case Direction.South:
                    endY = (0 + Size.y);
                    break;
                case Direction.East:
                    endX = (0 + Size.x);
                    break;
                case Direction.West:
                    endX = (0 - this.Size.x);
                    break;
            }

            var tween = GetTree().CreateTween();
            tween.TweenProperty(this, "position", new Vector2(0, 0), _inTime);
            tween.TweenInterval(_pauseTime);
            GD.Print("moving to " + endX);
            tween.TweenProperty(this, "position", new Vector2(endX, endY), _outTime);

            //tween.TweenCallback(new Callable(this, nameof(QueueFree)));
            tween.Finished += QueueFree;
        }

        public override void _Process(double delta)
        {
        }
    }
}
