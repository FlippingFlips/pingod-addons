using Godot;
using PinGod.Base;
using PinGod.Core;
using PinGod.Core.Service;
using System;

namespace PinGodAddOns.addons.pingod_machine
{
    public partial class RecordingNode : Node
    {
        protected EventRecordFile _recordFile;
        protected Label _recordingStatusLabel;
        private MachineNode _machineNode;
        protected ulong _machineLoadTime;
        protected RecordPlaybackOption _recordPlayback;

        [Export(PropertyHint.GlobalFile, "*.record")] string _playbackfile = null;

        [ExportCategory("Record / Playback")]
        [Export] RecordPlaybackOption recordPlayback = RecordPlaybackOption.Off;

        public override void _Ready()
        {
            if (!Engine.IsEditorHint())
            {
                _machineNode = GetNodeOrNull(Paths.ROOT_MACHINE) as MachineNode;

                //set start time
                _machineLoadTime = Time.GetTicksMsec();

                //set up recording / playback from [export] properties
                SetUpRecordingsOrPlayback(recordPlayback, _playbackfile);
            }
        }

        public override void _EnterTree()
        {
            if (!Engine.IsEditorHint())
            {
                //display status of recordings
                _recordFile = new EventRecordFile();
                _recordingStatusLabel = GetNodeOrNull<Label>("RecordingStatusLabel");
                if (_recordingStatusLabel != null) _recordingStatusLabel.Text = string.Empty;
            }
        }

        public override void _ExitTree()
        {
            if (_recordPlayback == RecordPlaybackOption.Record)
            {
                Logger.Info(nameof(RecordingNode), ":_ExitTree, saving recording");
                _recordFile.SaveRecording();
            }
        }

        public void RecordEventByName(PinGod.Core.Switch swName)
        {
            //record switch
            if (_recordPlayback == RecordPlaybackOption.Record)
            {
                _recordFile.RecordEventByName(swName, _machineLoadTime);
            }
        }

        public void RecordEventByAction(string action, byte state)
        {
            //record switch
            if (_recordPlayback == RecordPlaybackOption.Record)
            {
                _recordFile.RecordEventAction(action, state, _machineLoadTime);
            }
        }

        /// <summary>
        /// Process playback events. Turns off if playback is switched off.
        /// </summary>
        /// <param name="delta"></param>
        public override void _Process(double delta)
        {
            base._Process(delta);
            if (_recordPlayback != RecordPlaybackOption.Playback)
            {
                SetProcess(false);
                Logger.Info(nameof(MachineNode), ": Playback _Process loop stopped. No recordings are being played back.");                
                return;
            }
            else
            {
                var cnt = _recordFile.GetQueueCount();
                if (cnt <= 0)
                {
                    Logger.Info(nameof(MachineNode), ": playback events ended, RecordPlayback is off.");
                    _recordPlayback = RecordPlaybackOption.Off;
                    _recordFile.SaveRecording();
                    if (_recordingStatusLabel != null) _recordingStatusLabel.Text = "Machine:Playback ended";
                    return;
                }

                var evt = _recordFile.ProcessQueue(_machineLoadTime);
                if (evt != null)
                {
                    if (!evt.EvtName.StartsWith("action_"))
                    {
                        _machineNode?.SetSwitch(evt.EvtName, evt.State, false);
                    }                        
                    else Input.ParseInputEvent(
                        new InputEventAction
                        {
                            Action = evt.EvtName.Replace("action_", ""),
                            Pressed = evt.State > 0 ? true : false
                        });
                }
            }
        }

        /// <summary>
		/// Sets up recording and playback for capturing switch events.
		/// </summary>
		/// <param name="playbackOption"></param>
		/// <param name="playbackfile"></param>
		public virtual void SetUpRecordingsOrPlayback(RecordPlaybackOption playbackOption, string playbackfile)
        {
            _recordPlayback = playbackOption;

            Logger.Info(nameof(MachineNode), ":setup playback?: ", _recordPlayback.ToString());

            if (_recordPlayback == RecordPlaybackOption.Playback)
            {
                if (string.IsNullOrWhiteSpace(playbackfile))
                {
                    Logger.Warning(nameof(MachineNode), ":", nameof(SetUpRecordingsOrPlayback), ": playback enabled but no record file set.");
                    _recordPlayback = RecordPlaybackOption.Off;
                    if (_recordingStatusLabel != null) _recordingStatusLabel.Text = "Machine:No playback file found";
                }
                else
                {
                    try
                    {
                        if (_recordFile.PopulateQueueFromPlaybackFile(playbackfile) == Error.Ok)
                        {
                            Logger.Info(nameof(MachineNode), ":running playback file: ", playbackfile);
                            if (_recordingStatusLabel != null)
                            {
                                _recordingStatusLabel.Text = "Machine:Playback in progress";
                                _recordingStatusLabel.Visible = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"playback file failed: " + ex.Message);
                    }
                }
            }
            else if (_recordPlayback == RecordPlaybackOption.Record)
            {
                _recordFile.StartRecording(playbackfile);
                Logger.Debug(nameof(MachineNode), ":game recording on");
                if (_recordingStatusLabel != null)
                {
                    _recordingStatusLabel.Text = "Machine:Recording in progress";
                    _recordingStatusLabel.Visible = true;
                }
            }
        }
    }
}
