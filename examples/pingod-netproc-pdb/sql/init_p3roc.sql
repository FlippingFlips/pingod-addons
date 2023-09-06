BEGIN TRANSACTION;

-- Table: Machine PDB = 7
INSERT INTO Machine (Id, MachineType, NumBalls, DisplayMonitor) VALUES (1, 7, 4, 0);

-- Table: Coils
INSERT INTO Coils (Number, Name, PulseTime, Bus, Polarity, Search, Tags) VALUES ('A0-B0-0', 'flipperLwRMain', 30, '', 0, 0, '');
INSERT INTO Coils (Number, Name, PulseTime, Bus, Polarity, Search, Tags) VALUES ('A0-B0-1', 'flipperLwRHold', 30, '', 0, 0, '');
INSERT INTO Coils (Number, Name, PulseTime, Bus, Polarity, Search, Tags) VALUES ('A0-B0-2', 'flipperLwLMain', 30, '', 0, 0, '');
INSERT INTO Coils (Number, Name, PulseTime, Bus, Polarity, Search, Tags) VALUES ('A0-B0-3', 'flipperLwLHold', 30, '', 0, 0, '');
INSERT INTO Coils (Number, Name, PulseTime, Bus, Polarity, Search, Tags) VALUES ('A0-B0-4', 'trough', 30, '', 0, 0, 'trough');
INSERT INTO Coils (Number, Name, PulseTime, Bus, Polarity, Search, Tags) VALUES ('A0-B0-5', 'sling_l', 30, '', 0, 20, 'sling');
INSERT INTO Coils (Number, Name, PulseTime, Bus, Polarity, Search, Tags) VALUES ('A0-B0-6', 'sling_r', 30, '', 0, 20, 'sling');
INSERT INTO Coils (Number, Name, PulseTime, Bus, Polarity, Search, Tags) VALUES ('A0-B0-7', 'bumper_1', 30, '', 0, 20, 'bumper');
INSERT INTO Coils (Number, Name, PulseTime, Bus, Polarity, Search, Tags) VALUES ('A0-B0-8', 'bumper_2', 30, '', 0, 20, 'bumper');
INSERT INTO Coils (Number, Name, PulseTime, Bus, Polarity, Search, Tags) VALUES ('A0-B0-9', 'bumper_3', 30, '', 0, 20, 'bumper');
INSERT INTO Coils (Number, Name, PulseTime, Bus, Polarity, Search, Tags) VALUES ('A0-B0-10', 'flippersRelay', 30, '', 0, 0, '');
INSERT INTO Coils (Number, Name, PulseTime, Bus, Polarity, Search, Tags) VALUES ('A0-B0-11', 'saucerEject', 30, '', 0, 40, '');
INSERT INTO Coils (Number, Name, PulseTime, Bus, Polarity, Search, Tags) VALUES ('A0-B0-12', 'autoPlunger', 30, '', 0, 0, '');

-- Table: Leds
INSERT INTO Leds (Number, Name, Bus, Polarity, Tags) VALUES 
('A0-R0-G1-B2', 'start', '', 0,  'start'),
('A0-R3-G4-B5', 'shooter', '', 0,  ''),
('A0-R6-G7-B8', 'shootAgain', '', 0,  'shootAgain'),
('A0-R9-G10-B11', 'target0', '', 0,'targetBank'),
('A0-R12-G13-B14', 'target1', '', 0,'targetBank'),
('A0-R15-G16-B17', 'target2', '', 0,'targetBank'),
('A0-R18-G19-B20', 'target3', '', 0,'targetBank'),
('A0-R21-G22-B23', 'saucer', '', 0,'');

-- Table: Switches
INSERT INTO Switches (Number, Name, Type, Tags, SearchReset, SearchStop, IsOptoSwitch) VALUES 

-- board 0/A
('00', 'coin1', 0, 'door', '', '', 0),
('01', 'coin2', 0, 'door', '', '', 0),
('02', 'coin3', 0, 'door', '', '', 0),
('03', 'coin4', 0, 'door', '', '', 0),
('04', 'exit', 0, 'door', '', '', 0),
('05', 'down', 0, 'door', '', '', 0),
('06', 'up', 0, 'door', '', '', 0),
('07', 'enter', 0, 'door', '', '', 0),
-- board 0/B
('08', 'coinDoor', 1, 'door', '', 'open', 0),
('09', 'flipperLwL', 0, '', '', 'closed', 0),
('11', 'flipperLwR', 0, '', '', 'closed', 0),
('13', 'flipperUpL', 0, '', '', 'closed', 0),
('15', 'flipperUpR', 0, '', '', 'closed', 0),
('16', 'slamTilt', 0, '', '', 'closed', 0),
-- board 1/A
('17', 'tilt', 0, '', '', 'closed', 0),
('18', 'ballShooter', 0, '', '', 'closed', 0),
('19', 'start', 0, '', '', '', 0),
('20', 'not_used_20', 0, '', '', '', 0),
('21', 'trough0', 0, 'trough', '', 'closed', 0),
('22', 'trough1', 0, 'trough', '', 'closed', 0),
('23', 'trough2', 0, 'trough', '', 'closed', 0),
('24', 'trough3', 0, 'trough,troughEject', '', 'closed', 0),
-- board 1/B
('25', 'plungerLane', 0, 'shooterLane', '', 'closed', 0),
('26', 'inlaneL', 0, '', 'open', '', 0),
('27', 'inlaneR', 0, '', 'open', '', 0),
('28', 'outlaneL', 0, 'early', 'open', '', 0),
('29', 'outlaneR', 0, 'early', 'open', '', 0),
('30', 'slingL', 0, 'sling', 'open', '', 0),
('31', 'slingR', 0, 'sling', 'open', '', 0),
('32', 'bumper0', 0, 'bumper', 'open', '', 0),
-- board 2/A
('33', 'bumper1',0, 'bumper', 'open', '', 0),
('34', 'bumper2', 0, 'bumper', 'open', '', 0),
('35', 'saucer', 0, '', 'open', 'closed', 0),
('36', 'target0', 0, 'targetBank', 'open', '', 0),
('37', 'target1', 0, 'targetBank', 'open', '', 0),
('38', 'target2', 0, 'targetBank', 'open', '', 0),
('39', 'target3', 0, 'targetBank', 'open', '', 0),
('40', 'not_used_40', 0, '', '', '', 0),
-- board 2/B
('41', 'not_used_41', 0, '', '', '', 0),
('42', 'not_used_42', 0, '', '', '', 0),
('43', 'not_used_43', 0, '', '', '', 0),
('44', 'not_used_44', 0, '', '', '', 0),
('45', 'not_used_45', 0, '', '', '', 0),
('46', 'not_used_46', 0, '', '', '', 0),
('47', 'not_used_47', 0, '', '', '', 0),
('48', 'not_used_48', 0, '', '', '', 0),
-- board 3/A
('49', 'not_used_49', 0, '', '', '', 0),
('50', 'not_used_50', 0, '', '', '', 0),
('51', 'not_used_51', 0, '', '', '', 0),
('52', 'not_used_52', 0, '', '', '', 0),
('53', 'not_used_53', 0, '', '', '', 0),
('54', 'not_used_54', 0, '', '', '', 0),
('55', 'not_used_55', 0, '', '', '', 0),
('56', 'not_used_56', 0, '', '', '', 0),
-- board 3/B
('57', 'not_used_57', 0, '', '', '', 0),
('58', 'not_used_58', 0, '', '', '', 0),
('59', 'not_used_59', 0, '', '', '', 0),
('60', 'not_used_60', 0, '', '', '', 0),
('61', 'not_used_61', 0, '', '', '', 0),
('62', 'not_used_62', 0, '', '', '', 0),
('63', 'not_used_63', 0, '', '', '', 0);

-- PLAYERS
INSERT INTO Players (Id, Initials, Name, [Default]) VALUES (1, 'NETPROC', 'Default', '1');

-- Option Types 0=Range, 1=Array, 2= Enum
INSERT INTO Adjustments (Id, Name, Description, Options, OptionType, ValueDefault, Value, MenuName, SubMenuName) VALUES
('ALLOW_RESTART', 'Allow Restart','Allow game restart from holding start.', 'NO,YES', 2, 1, 1, 'STANDARD_ADJ', 'GENERAL'),
('ATTRACT_MUSIC', 'Attract Music','Allow music to play in attract', 'NO,YES', 2, 1, 1, 'STANDARD_ADJ', 'GENERAL'),
('BALLS_PER_GAME', 'Balls Per Game','Number of balls per game 1-10', '1,10', 0, 3, 3, 'STANDARD_ADJ', 'GENERAL'),
('BALL_SAVE_TIME', 'Ball Save Time','Ball saver time', '0,25', 0, 8, 8, 'STANDARD_ADJ', 'GENERAL'),
('BALL_SEARCH_TIME', 'Ball Search Time','Timeout to search for balls and pulse coils', '8,30', 0, 10, 10, 'STANDARD_ADJ', 'GENERAL'),
('IDLE_SHOOTER_TIMEOUT', 'Idle Shooter Timeout','Auto launch ball if idle in plunger lane, 0 disabled', '0,30,60,90,120,150', 1, 60, 60, 'STANDARD_ADJ', 'GENERAL'),
('MASTER_VOL', 'Master Volume','','-30,0', 0, -6, -6, 'STANDARD_ADJ', 'AUDIO'),
('MUSIC_VOL', 'Music Volume','','-30,0', 0, -6, -6, 'STANDARD_ADJ', 'AUDIO'),
('VOICE_VOL', 'Voice Volume','','-30,0', 0, -6, -6, 'STANDARD_ADJ', 'AUDIO'),
('FX_VOL', 'Sound FX Volume','','-30,0', 0, -6, -6, 'STANDARD_ADJ', 'AUDIO'),
('MATCH_PERCENT', 'Match Percent','Match percent, 0 off', '0,20', 0, 5, 5, 'STANDARD_ADJ', 'GENERAL'),
('DISP_W', 'Display Width','','100,1920', 0, 1920, 480, 'STANDARD_ADJ', 'DISPLAY'),
('DISP_H', 'Display Height','','100,1080', 0, 1080, 270, 'STANDARD_ADJ', 'DISPLAY'),
('DISP_X', 'Display X','','0,1920', 0, 0, 0, 'STANDARD_ADJ', 'DISPLAY'),
('DISP_Y', 'Display Y','','0,1080', 0, 0, 0, 'STANDARD_ADJ', 'DISPLAY'),
('DISP_MODE', 'Display Mode','Defaults to 0 = window','WIN,MIN,MAX,FS,FS_EXCLUSIVE', 2, 0, 0, 'STANDARD_ADJ', 'DISPLAY'),
('DISP_CONT_SCALE_MODE', 'Content Scale Mode','','Disabled,CanvasItems,Viewport', 2, 1, 1, 'STANDARD_ADJ', 'DISPLAY'),
('DISP_CONT_SCALE_ASPECT', 'Content Scale Aspect','','Ignore, Keep, KeepWidth, KeepHeight, Expand', 2, 0, 4, 'STANDARD_ADJ', 'DISPLAY'),
('DISP_TOP', 'Display On Top','','OFF,ON', 2, 1, 1, 'STANDARD_ADJ', 'DISPLAY'),
('TILT_WARNINGS', 'Tilt Warnings','Number of tilt warnings before tilt', '0,20', 0, 3, 3, 'STANDARD_ADJ', 'GENERAL');
--('XB_RESERVE', '0', 4, 'GENERAL', '[0, 1]', 'If enabled and in multiplayer the rounds continue with xb given at the end'),
-- todo: chase ball for ball search
--('XB_GAME_MAX', '-1', 4, 'EXTRA BALLS', '[-1,0,1,2,3,4,5,6]', 'Max extra balls, -1 unlimited'),
--('XB_PLAYER_MAX', '3', 4, 'EXTRA BALLS', '[-1,0,1,2,3,4,5,6]', 'Max extra balls PLAYER, -1 unlimited'),

--REPLAY
--SCORES
-- PRICING SETTINGS
-- GAME SETTINGS
-- COIL SETTINGS - flipper strength etc - use the actual pulse time rather than create new tables, they will be 1-255 anyway
-- LOG SETTINGS
--INSERT INTO Settings (Id, Value, Type) VALUES ('LOG_LEVEL_GAME', NULL, 'GAME');
--INSERT INTO Settings (Id, Value, Type) VALUES ('LOG_LEVEL_DISPLAY', '0', 'DISPLAY'); -- TraceLogType 0 == ALL

---- DEVELOPER SETTINGS
--INSERT INTO Settings (Id, Value, Type) VALUES ('PLAYBACK_ENABLED', '0', 'PLAYBACK');
--INSERT INTO Settings (Id, Value, Type) VALUES ('PLAYBACK_RECORDING_ID', NULL, 'PLAYBACK');
--INSERT INTO Settings (Id, Value, Type) VALUES ('RECORDING_ENABLED', '0', 'RECORDING');
--INSERT INTO Settings (Id, Value, Type) VALUES ('RECORDING_SET_PLAYBACK_ON_END', NULL, 'RECORDING');

-- AUDITS (STANDARD)
INSERT INTO Audits (Id, Value, Type, Description) VALUES
('CREDITS', 0, 0, 'Credits in machine'), 
('CREDITS_TOTAL', 0, 0, 'Total credits used'), 
('GAMES_STARTED', 0, 0, 'Games started log'), 
('GAMES_PLAYED', 0, 0, 'Games completed log'), 
('XB_AWARDED', 0, 0, 'Total extra balls awarded'),
('REPLAYS', 0, 0, 'Total replays awarded'), 
('MATCHES', 0, 0, 'Total Matches Awarded'),
('POWERED_ON_TIMES', 0, 0, 'Times machine powered on'),
('TOTAL_BALLS_PLAYED', 0, 0, 'Total balls played');

-- AUDITS (GAME) TODO: LOG SWITCHES, MODES, TIMES


COMMIT TRANSACTION;