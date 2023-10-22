using Godot;
using PinGod.Base;
using PinGod.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PinGod.Core
{
    public class EventRecordFile
    {
        private Queue<PlaybackEvent> _playbackQueue;
        private string _fileName;

        /// <summary>
        /// recording actions to file using godot
        /// </summary>
        private FileAccess _recordFile;

        private RecordPlaybackOption _recordPlayback;

        /// <summary>
        /// Creates the recordings directory in the users folder
        /// </summary>
        /// <returns>The path to the recordings</returns>
        public virtual string CreateRecordingsDirectory()
        {
            var userDir = OS.GetUserDataDir();
            var dir = userDir + $"/recordings/";

            Logger.Info("recordings path: " + dir);

            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);

            return dir;
        }
        public int GetQueueCount() => _playbackQueue?.Count ?? 0;

        /// <summary>
        /// Opens a record file a parses the lines into a queue
        /// </summary>
        /// <param name="playbackfile"></param>
        /// <returns></returns>
        public Error PopulateQueueFromPlaybackFile(string playbackfile)
        {
            //,  FileAccess.ModeFlags.Read
            using var pBackFile = FileAccess.Open(playbackfile, FileAccess.ModeFlags.Read);
            if (FileAccess.GetOpenError() == Error.FileNotFound)
            {
                _recordPlayback = RecordPlaybackOption.Off;
                Logger.Error(nameof(EventRecordFile), ":ERROR: playback file not found, set playback false");
                return Error.FileNotFound;
            }

            string[] eventLine = null;
            _playbackQueue = new Queue<PlaybackEvent>();
            while ((eventLine = pBackFile.GetCsvLine("|"))?.Length == 3)
            {
                byte.TryParse(eventLine[1], out var state);
                uint.TryParse(eventLine[2], out var time);
                _playbackQueue.Enqueue(new PlaybackEvent(eventLine[0], state, time));
            }

            _playbackQueue.Reverse();
            Logger.Debug(nameof(EventRecordFile), $" {_playbackQueue.Count} playback events queued. first action: ", _playbackQueue.Peek().EvtName);

            return Error.Ok;
        }

        /// <summary>
        /// checks the next in line for playback, returns true if was playback time. Pushes the event into an action onto the godot inputeventaction queue <para/>
        /// TODO: don't use the Godot input, but this is probably ok
        /// </summary>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public PlaybackEvent ProcessQueue(ulong startTime)
        {
            var nextEvt = _playbackQueue.Peek().Time;
            var time = Time.GetTicksMsec() - startTime;
            if (nextEvt <= time)
            {
                var pEvent = _playbackQueue.Dequeue();
                //var ev = new InputEventAction() { Action = pEvent.EvtName, Pressed = pEvent.State };
                //Input.ParseInputEvent(ev);
                Logger.Debug(nameof(MachineNode), ":playback evt ", pEvent.EvtName);
                return pEvent;
            }

            return null;
        }

        /// <summary>
        /// flushes the record file
        /// </summary>
        public virtual void SaveRecording()
        {
            if (_recordFile !=null)
            {
                _recordFile?.Flush();
                _recordFile?.Free();
                Logger.Info(nameof(EventRecordFile), ":recording file flushed, saved. ", _fileName);
            }
        }

        /// <summary>
        /// Requires just the name of the file. MyRecord.recording
        /// </summary>
        /// <param name="fileName"></param>
        public virtual void StartRecording(string fileName)
        {
            var userDir = CreateRecordingsDirectory();
            _fileName = fileName;
            _recordFile = FileAccess.Open(fileName, FileAccess.ModeFlags.Write);
            Logger.Info(nameof(IPinGodGame), ":started recording to file: " + fileName);
        }

        internal void RecordEventPrefixed(Switch @switch, ulong machineLoadTime)
        {
            if (_recordFile != null)
            {
                var switchTime = Time.GetTicksMsec() - machineLoadTime;
                var recordLine = $"sw{@switch.Num}|{@switch.State}|{switchTime}";
                _recordFile?.StoreLine(recordLine);
                Logger.Debug($"switch recorded: ", recordLine);
            }                
        }

        internal void RecordEventByName(Switch @switch, ulong machineLoadTime)
        {
            if( _recordFile != null )
                RecordSwitchEvent(@switch.Name, @switch.State, machineLoadTime);
        }

        internal void RecordSwitchEvent(string name, byte state, ulong machineLoadTime)
        {
            var switchTime = Time.GetTicksMsec() - machineLoadTime;
            var recordLine = $"{name}|{state}|{switchTime}";
            
            _recordFile.CallDeferred("store_line", recordLine);
            Logger.Debug($"switch recorded: ", recordLine);
        }

        internal void RecordEventAction(string action, byte pressed, ulong machineLoadTime)
        {
            if(_recordFile != null)
            {
                var switchTime = Time.GetTicksMsec() - machineLoadTime;
                var recordLine = $"action_{action}|{pressed}|{switchTime}";
                _recordFile?.StoreLine(recordLine);
                Logger.Debug($"action recorded: ", recordLine);
            }
        }        
    }
}
