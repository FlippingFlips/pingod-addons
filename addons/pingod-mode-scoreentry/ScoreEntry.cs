using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static PinGodBase;

/// <summary>
/// Simple score entry: Sends <see cref="ScoreEntryEnded"/> TODO entries, placeholder
/// </summary>
public partial class ScoreEntry : Control
{
    /// <summary>
    /// 
    /// </summary>
    protected IPinGodGame pinGod;

    private PinGodPlayer _cPlayer;

    private char[] _entry;

    /// <summary>
    /// include chars 0-9
    /// </summary>
    [Export] bool _includeZeroToNine = false;

    /// <summary>
    /// player max chars
    /// </summary>
    [Export] int _nameMaxLength = 8;

    /// <summary>
    /// title message node selectable
    /// </summary>
    [Export] NodePath _playerMessage = null;

    /// <summary>
    /// space when changing between chars with flippers
    /// </summary>
    [Export] int _selectCharMargin = 25;

    /// <summary>
    /// char select node path
    /// </summary>
    [Export] NodePath _selectedChar = null;

    [Signal] public delegate void ScoreEntryEndedEventHandler();

    int[] allowedChars;
    int currentEntryIndex = 0;
    int CurrentPlayer = 0;
    string entry = "";
    bool IsPlayerEnteringScore = false;
    int PlayerCount;
    private Label playerMessageLabel;
    private Label selectedCharLabel;
    private Vector2 selectedCharLabelStartPos;
    int selectedIndex = 0;
    private Label selectedName;
    /// <summary>
    /// get ref to the labels needed for the scene
    /// </summary>
    public override void _EnterTree()
    {
        Logger.Info(nameof(ScoreEntry), ":",nameof(_EnterTree));
        selectedCharLabel = _selectedChar == null ? null : GetNode<Label>(_selectedChar);
        playerMessageLabel = _playerMessage == null ? null : GetNode<Label>(_playerMessage);       
        _entry = new char[_nameMaxLength];

        if (HasNode("/root/PinGodGame"))
        {
            pinGod = GetNode("/root/PinGodGame") as IPinGodGame;
            pinGod.PinGodMachine.Connect("SwitchCommand", new Callable(this, nameof(OnSwitchCommandHandler)));
        }
        else { Logger.Warning(nameof(ScoreEntry), ": no PinGodGame found"); }
    }

    /// <summary>
    /// Sets <see cref="IsPlayerEnteringScore"/> to true
    /// </summary>
    public override void _Ready()
    {        
        CharSelectionSetup();
        selectedName = GetNode("CenterContainer/VBoxContainer/Name") as Label;

        IsPlayerEnteringScore = true;
        selectedCharLabelStartPos = new Vector2(selectedCharLabel?.Position.x ?? 0, selectedCharLabel?.Position.y ?? 0);
    }

    /// <summary>
    /// Sets <see cref="IsPlayerEnteringScore"/> to true and shows this scene. Moves to each player who has a high score to let them enter their initials
    /// </summary>
	public virtual void DisplayHighScore()
    {
        Logger.Info(nameof(ScoreEntry), ":display high score");
        IsPlayerEnteringScore = true;
        this.Visible = true;
        PlayerCount = pinGod.Players?.Count ?? 0;
        if (PlayerCount <= 0)
        {
            Logger.Error("Need players for this mode to cycle through");
            QuitScoreEntry();
            return;
        }
        MoveNextPlayer();
    }

    /// <summary>
    /// just logs by default, override this method to act on new high scores.
    /// </summary>
	public virtual void OnNewHighScore()
    {
        Logger.Debug(nameof(ScoreEntry), ": NEW HIGH SCORE MADE:");
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void OnPlayerFinishedEntry()
    {
        Logger.Debug(nameof(ScoreEntry), nameof(OnPlayerFinishedEntry));
        if (pinGod.GameData?.HighScores != null)
        {
            try
            {
                pinGod.GameData?.HighScores?.Add(new HighScore()
                {
                    Name = new string(_entry),
                    Created = DateTime.Now,
                    Scores = _cPlayer.Points
                });
                pinGod.GameData.HighScores = pinGod.GameData.HighScores.OrderByDescending(x => x.Scores)
                    .Take(pinGod.Adjustments.MaxHiScoresCount)
                    .ToList();
                Logger.Info(nameof(ScoreEntry), ":hi score added ", entry, " ", _cPlayer.Points);
            }
            catch (Exception ex) { Logger.Warning("player finish score adding to hi scores. " + ex.ToString()); }
        }

        if (!MoveNextPlayer())
        {
            QuitScoreEntry();
            return;
        }
    }

    /// <summary>
    /// Checks if player made top score
    /// </summary>
    /// <returns></returns>
    protected virtual bool MoveNextPlayer()
    {
        if (CurrentPlayer > PlayerCount - 1)
        {
            Logger.Debug(nameof(ScoreEntry), nameof(MoveNextPlayer), ": current player higher than players count, finished");
            return false;
        }

        //reset the entry player initials
        entry = string.Empty;
        //get the player to check hi scores
        _cPlayer = pinGod.Players[CurrentPlayer] ?? new PinGodPlayer() { Points = 1000000 };

        if (playerMessageLabel != null)
        {
            playerMessageLabel.Text = Tr("HI_SCORE_ENTRY").Replace("%d", (CurrentPlayer + 1).ToString());
            Logger.Debug(nameof(ScoreEntry), ":", playerMessageLabel.Text);
        }            

        //hi scores has enough room to add new at any points, by default they go on board
        if (pinGod.GameData.HighScores.Count < pinGod.Adjustments.MaxHiScoresCount)
        {
            Logger.Debug(nameof(ScoreEntry), ":hi-score has space, adding this player");
            CurrentPlayer++;
        }
        //this hi score isn't as big as the others
        else if (!pinGod.GameData.HighScores.Any(x => x.Scores < _cPlayer.Points))
        {
            CurrentPlayer++;
            if (!MoveNextPlayer())
            {
                Logger.Debug(nameof(ScoreEntry), nameof(MoveNextPlayer), ": player score not large enough for entering, moving to player=", CurrentPlayer+1);
                QuitScoreEntry();
                return false;
            }
        }
        else
        {
            CurrentPlayer++;
        }

        OnNewHighScore();
        return true;
    }

    private void CharSelectionSetup()
    {
        var chars = new List<int>();
        chars.AddRange(Enumerable.Range(65, 26)); //A-Z

        if (_includeZeroToNine)
            chars.AddRange(Enumerable.Range(48, 10)); //0-9

        chars.Add(60); //delete
        chars.Add(61); //enter
        allowedChars = chars.ToArray();
    }

    private void OnLeftFlipper()
    {
        //scroll back, move the label to the left
        selectedIndex--;
        if (selectedIndex < 0)
        {
            selectedIndex = allowedChars.Length - 1;

            selectedCharLabel.SetPosition(new Vector2(selectedCharLabel.Position.x - (_selectCharMargin * allowedChars.Length - _selectCharMargin), selectedCharLabel.Position.y));
        }
        else
        {
            selectedCharLabel.SetPosition(new Vector2(selectedCharLabel.Position.x + _selectCharMargin, selectedCharLabel.Position.y));
        }

        _entry[currentEntryIndex] = (char)allowedChars[selectedIndex];
        selectedName.Text = new string(_entry);
    }

    private void OnRightFlipper()
    {
        //scroll forward, move the label to the right
        selectedIndex++;
        if (selectedIndex > allowedChars.Length - 1)
        {
            selectedIndex = 0;
            selectedCharLabel.SetPosition(new Vector2(selectedCharLabel.Position.x + (_selectCharMargin * allowedChars.Length - _selectCharMargin), selectedCharLabel.Position.y));
            Logger.Debug(nameof(ScoreEntry), ":set flip r start");
        }
        else if (selectedIndex == 0)
        {
            selectedCharLabel.SetPosition(selectedCharLabelStartPos);
        }
        else { selectedCharLabel.SetPosition(new Vector2(selectedCharLabel.Position.x - _selectCharMargin, selectedCharLabel.Position.y)); }

        _entry[currentEntryIndex] = (char)allowedChars[selectedIndex];
        selectedName.Text = new string(_entry);
    }

    private void OnStartButton()
    {
        //delete char
        if (allowedChars[selectedIndex] == 60)
        {
            if (_entry.Length > 0)
            {
                _entry[_entry.Length - 1] = ' ';
                currentEntryIndex--;
                if (currentEntryIndex < 0)
                    currentEntryIndex = 0;
            }
        }
        //accept
        else if (allowedChars[selectedIndex] == 61)
        {
            //add the hi score and order it
            OnPlayerFinishedEntry();
        }
        else
        {
            if (currentEntryIndex < _nameMaxLength - 1)
            {
                currentEntryIndex++;
                selectedName.Text = new string(_entry);
            }
            else
            {
                //OnPlayerFinishedEntry();
                //todo: move to last char
                selectedIndex = allowedChars.Length - 1;

                selectedCharLabel.SetPosition(new Vector2(selectedCharLabelStartPos.x + (_selectCharMargin * allowedChars.Length - 1), selectedCharLabelStartPos.y));
                selectedName.Text = new string(_entry);
            }
        }
    }

    /// <summary>
    /// Switch handlers for lanes and slingshots
    /// </summary>
    /// <param name="name"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    private void OnSwitchCommandHandler(string name, byte index, byte value)
    {
        if (value <= 0) return;
        if (this.Visible && IsPlayerEnteringScore)
        {
            switch (name)
            {
                case "flipper_l":
                    OnLeftFlipper();
                    break;
                case "flipper_r":
                    OnRightFlipper();
                    break;
                case "start":
                    OnStartButton();
                    break;
                default:
                    break;
            }
        }            
    }
    private void QuitScoreEntry()
    {
        Logger.Debug(nameof(ScoreEntry), nameof(QuitScoreEntry), ": score entry mode ending");
        IsPlayerEnteringScore = false;
		this.Visible = false;
        //emit signal to let know players have finished
        EmitSignal(nameof(ScoreEntryEnded));
	}
}
