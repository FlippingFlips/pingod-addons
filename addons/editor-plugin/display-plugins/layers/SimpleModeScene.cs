using Godot;
using PinGod.Core;
using PinGod.Core.Service;
using System;

namespace PinGod.EditorPlugins
{
    //[Tool]
    /// <summary>
    /// Simple scene to change Background, animations, Foreground, text labels <para/>
    /// TODO: when finishing this, need to remove the default textures
    /// </summary>
    public partial class SimpleModeScene : Control
    {
        /// <summary>
        /// The animations scene res:// path
        /// </summary>
        [Export(PropertyHint.File, "*.tscn")] string _animations;

        [Signal] public delegate void ClearDisplayEventHandler();

        /// <summary>
        /// Animated sprites. Can play animation by give name, play("myanim")
        /// </summary>
        AnimatedSprite2D _animsNode;
        private Label _labelLg;
        private Label _labelSm;
        private TextureRect _textureBg;
        private TextureRect _textureFg;
        private Timer _clearDisplayTimer;

        /// <summary>
        /// Replace the AnimatedSprite2D node with a node in the pre loaded resources node and name it AnimatedSprite2D if the <see cref="_animations"/> is set to a scene <para/>
        /// In this case the scene is a set of animations in an animated sprite. The modes can then set the animation <para/>
        /// The packed scene paths are set in autoload/Resources.tscn
        /// </summary>
        public override void _EnterTree()
        {
            base._EnterTree();

            //get nodes
            _animsNode = GetNodeOrNull<AnimatedSprite2D>(nameof(AnimatedSprite2D));
            _clearDisplayTimer = GetNode<Timer>("ClearDisplayTimer");
            _labelLg = GetNode<Label>("LabelLg");
            _labelSm = GetNode<Label>("LabelSm");
            _textureBg = GetNode<TextureRect>("BgTextureRect");
            _textureFg = GetNode<TextureRect>("FgTextureRect");

            //get animations from resources if file set
            if (!string.IsNullOrWhiteSpace(_animations))
            {
                var resources = GetNodeOrNull<Resources>("/root/Resources");
                if (resources != null)
                {
                    if (resources.HasResource(_animations.GetBaseName()))
                    {
                        var res = resources.GetResource(_animations.GetBaseName()) as PackedScene;
                        if (res != null)
                        {
                            AddAnimatedSpritesChild(res);
                            return;
                        }
                    }
                    else { Logger.Warning(nameof(SimpleModeScene), $": no resource loaded from at {_animations}. Is this resource set in the autoload/Resources.tscn ?"); }
                }
                else { Logger.Warning(nameof(SimpleModeScene), ": no resources module found"); }

                var resw = ResourceLoader.Load(_animations) as PackedScene;
                AddAnimatedSpritesChild(resw);
            }
        }

        private void AddAnimatedSpritesChild(PackedScene res)
        {
            this.RemoveChild(_animsNode);
            var child = res.Instantiate();
            child.Name = nameof(AnimatedSprite2D);
            AddChild(child);
            MoveChild(child, 0);
            _animsNode = child as AnimatedSprite2D;
        }

        public override void _Ready()
        {
            base._Ready();
            if (!Engine.IsEditorHint())
                _clearDisplayTimer.Timeout += ClearDisplayTimer_timeout;
        }

        /// <summary>
        /// Stops the animations and hides.
        /// </summary>
        public virtual void ClearDisplayTimer_timeout()
        {
            Visible = false;
            _animsNode?.Stop();
            EmitSignal(nameof(ClearDisplay));

            //todo: options to do lamps?
            //if (!pinGod.IsMultiballRunning)
            //{
            //    pinGod.DisableAllLamps();
            //    UpdateLamps();
            //    //turn off lamp sequencing
            //    pinGod.SolenoidOn("vpcoil", 0);
            //}
        }

        /// <summary>
        /// Changes the animation and sets labels.
        /// </summary>
        /// <param name="anim">name inside the animated sprite</param>
        /// <param name="lbLg"></param>
        /// <param name="lbSm"></param>
        /// <param name="delay">if greater than 1 it will play for given time then set visible to false on this control</param>
        public virtual void PlaySequence(string anim = "default", string lbLg = "", string lbSm = "", float delay = 2f)
        {
            //any running timer stop it
            _clearDisplayTimer.Stop();
            //set animation
            if (_animsNode != null)
            {
                _animsNode.Animation = anim;
                _animsNode.Frame = 0;
                _animsNode.Play();
            }
            //set text
            SetLabelText(0, lbLg);
            SetLabelText(1, lbSm);
            //show then hide after delay
            Visible = true;
            if (delay > 0)
            {
                _clearDisplayTimer.Start(delay);
            }
        }

        /// <summary>
        /// Two labels 0 = large
        /// </summary>
        /// <param name="index"></param>
        /// <param name="text"></param>
        public virtual void SetLabelText(int index, string text)
        {
            switch (index)
            {
                case 0: _labelLg.Text = text; break;
                case 1: _labelSm.Text = text; break;
            }
        }

        public virtual bool IsStopped() => _clearDisplayTimer.IsStopped();

        internal void StopAndHide()
        {
            _clearDisplayTimer.Stop();
            _animsNode.Stop();
            Visible = false;
        }
    }

}
