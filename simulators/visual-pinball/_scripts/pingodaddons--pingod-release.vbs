'#########################################################################################
' PinGod AddOns Demo Template by HorsePin
' Demo game running pingod addons
'#########################################################################################
Option Explicit
On Error Resume Next
	ExecuteGlobal GetTextFile("controller.vbs") ' Std Controller for DOF etc.
	If Err Then MsgBox "You need the Controller.vbs file in order to run this table (installed with the VPX package in the scripts folder)"
On Error Goto 0
' ----------------------------------------------------------------------------------------
' SET DEBUG OR RELEASE
' ----------------------------------------------------------------------------------------
'PinGod Debug
'Const IsDebug = True
'Const GameDirectory = "..\..\" ' Loads the `project.godot` from the addons demo or your project

'PinGod Release - Use these for release versions
Const IsDebug = False ' set false to load an export
Const GameDirectory = "..\..\win.exe" 'exported game from Github. Providing controller installed this will run out of the box

' ----------------------------------------------------------------------------------------
' LoadPinGoVpController = Requires modded Core Vbs for different switches csharp. PinGod.vbs, core_csharp.vbs
' https://github.com/FlippingFlips/pingod-controller-com/releases
' ----------------------------------------------------------------------------------------
LoadPinGodVpController 
Sub LoadPinGodVpController
	'On Error Resume Next
		If ScriptEngineMajorVersion<5 Then MsgBox "VB Script Engine 5.0 or higher required"
		ExecuteGlobal GetTextFile("PinGod.vbs")
		If Err Then MsgBox "Unable to open " & VBSfile & ". Ensure that it is in the same folder as this table. " & vbNewLine & Err.Description
		Set Controller=CreateObject("PinGod.VP.Controller")		
		If Err Then MsgBox "Failed to initialize PinGod.VP controller, is it registered?" : Exit Sub		
	On Error Goto 0
End Sub
' ----------------------------------------------------------------------------------------
' CONTROLLER OPTIONS - This game is using leds, not lamps, that can be changed here
' ----------------------------------------------------------------------------------------
Const UseSolenoids = 1 ' Check for solenoid states?
Const UsePdbLeds = 1  ' use led (color)
Const PdbOffColor = 3815994 ' Led off state color 
Const UseLamps = 0  ' Check for lamp states?
' ----------------------------------------------------------------------------------------
' TABLE OBJECT SWITCH LINKING
' Some switches are automatic from their TimerInterval box with the switch number, see the lane switches in VP
' ----------------------------------------------------------------------------------------
Dim bsSaucer, plungerIM, swSaucer : swSaucer = 31
Dim slingL, slingR : slingL = 22 : slingR = 24
' Set trough switch numbers.
Dim TroughSwitches,bsTrough:TroughSwitches = Array(27,28,29,30)
' ----------------------------------------------------------------------------------------
' VP MAIN TABLE CONTROL - Exit, Pause
' ----------------------------------------------------------------------------------------
Sub Table1_Exit : Controller.Pause 1 :Controller.Stop : End Sub ' Closes the display window, sends the quit action
Sub Table1_Paused: Controller.Pause 1 : End Sub
Sub Table1_UnPaused: Controller.Pause 0 : End Sub
' ----------------------------------------------------------------------------------------
' VP TABLE INIT = Inits the controller then waits for the display to fully load into initial scene.
' * This wait is done by a timer on the LeftFlipper. 
' ----------------------------------------------------------------------------------------
Sub Table1_Init	
	With Controller
		.DisplayOverrideOn 	= True ' Enable this to allow custom display properties
		.DisplayFullScreen	= False
		.DisplayScreen		= 0
		.DisplayWidth 		= 512
		.DisplayHeight 		= 256	
		.DisplayX	 		= 0
		.DisplayY	 		= 0
		.DisplayAlwaysOnTop	= True
	    '.CoilCount			= 64 ' add more coils, * must match the memory map in the godot scene *

	On Error Resume Next
		if isDebug Then '
			.RunDebug GetPlayerHWnd, GameDirectory ' Load game from Godot folder with Godot exe
		else
			.Run GetPlayerHWnd, GameDirectory ' With game exported executable
		end if		
	If Err Then MsgBox Err.Description : Exit Sub
	End With
    On Error Goto 0	
	If Err Then MsgBox Err.Description : Exit Sub	

	'Controller.FocusSimulator

	LeftFlipper.TimerInterval = 500
	LeftFlipper.TimerEnabled = 1
End Sub
'Game ready checker from flipper timer
Sub LeftFlipper_Timer
	Debug.Print "checking game state..."
	LeftFlipper.TimerEnabled = 0		
	if not Controller.GameRunning Then LeftFlipper.TimerEnabled = 1 : Exit Sub
	InitGame
End Sub
' ----------------------------------------------------------------------------------------
' GAME / VP init
' ** When the display is ready initialize VPM controller scripts and table objects **
' ----------------------------------------------------------------------------------------
Dim initialized : initialized = 0
Sub InitGame
	
	if initialized then exit sub ' prevent any chance of init twice if author decides to use LFlipper Timers
	initialized=1

	LoadingText.Visible = false 

	' INIT CORE VBS - SETUP SWITCHES AND LAMPS
	vpmInit me
	vpmMapLights AllLamps		'Auto lamps collection, lamp id in timerinterval
	vpmCreateEvents AllSwitches 'Auto Switches collection from the swNum in timerInterval

	' INIT PINMAME TIMERS
	pulsetimer.Enabled=1
	PinMAMETimer.Enabled=1				

    ' AUTO PLUNGER
    Const IMPowerSetting = 50 ' Plunger Power
    Const IMTime = 0.6        ' Time in seconds for Full Plunge
    Set plungerIM = New cvpmImpulseP
    With plungerIM
        .InitImpulseP swplunger, IMPowerSetting, IMTime
        .Random 0.3        
        .CreateEvents "plungerIM"
		'.InitExitSnd "plunger2", "plunger"
    End With

	' TROUGH
	Set bsTrough = New cvpmTrough
	bsTrough.Size = UBound(TroughSwitches)+1 ' calc size and balls from switch count
	bsTrough.Balls = UBound(TroughSwitches)+1
	bsTrough.InitSwitches TroughSwitches ' trough switches
	bsTrough.InitExit BallRelease, 90, 8	
	bsTrough.CreateEvents "bsTrough",  Drain
	bsTrough.InitEntrySounds "Drain", "", ""
	bsTrough.InitExitSounds "BallRelease", ""	
	bsTrough.Reset

	' SAUCER
	Set bsSaucer = New cvpmSaucer
	bsSaucer.InitKicker Kicker001, swSaucer, 165, 10, 0
	bsSaucer.CreateEvents "bsSaucer", Kicker001
	'bsSaucer.InitSounds "sp76-kick-enter", "", "sp76-kick-exit"

	' NUDGING
    vpmNudge.TiltSwitch = swTilt
    vpmNudge.Sensitivity = 0.8
	vpmNudge.TiltObj = Array(LSling,RSling)

	' LOADING SCREEN
	If Err Then MsgBox Err.Description
	LoadingText.Visible = false ' Hide the overlay (loading screen)	
	On Error Goto 0		
End Sub
' ----------------------------------------------------------------------------------------
' KEY / INPUT HANDLING = PLAYER CAN'T USE UNTIL CONTROLLER.GAMERUNNING = TRUE
' ----------------------------------------------------------------------------------------
Sub Table1_KeyDown(ByVal keycode)
	' GAME RUNNING CHECK
	if Controller.GameRunning = 0 then Exit Sub 'exit because no display is available
	' MANUAL PLUNGER 
	If keycode = PlungerKey Then Plunger.PullBack : PlaySoundAt "plungerpull", Plunger : End If
	' FLIPPERS
	If keycode = LeftFlipperKey and FlippersOn Then : SolLFlipper(1) End If
	If keycode = RightFlipperKey and FlippersOn Then : SolRFlipper(1) End If
	' MACHINE SWITCHES = This will handle machine switches and flippers etc
	If vpmKeyDown(keycode) Then Exit Sub 

End Sub
Sub Table1_KeyUp(ByVal keycode)
	' GAME RUNNING CHECK
	if Controller.GameRunning = 0 then Exit Sub 'exit because no display is available
	' MANUAL PLUNGER 
	If keycode = PlungerKey Then Plunger.Fire : PlaySoundAt "plunger", Plunger :End If
	' FLIPPERS
	If keycode = LeftFlipperKey and FlippersOn Then : SolLFlipper(0) End If
	If keycode = RightFlipperKey and FlippersOn Then : SolRFlipper(0) End If
	' MACHINE SWITCHES = This will handle machine switches and flippers etc
	If vpmKeyUp(keycode) Then Exit Sub
End Sub
' ----------------------------------------------------------------------------------------
' Solenoids / Coils / Callbacks
' ----------------------------------------------------------------------------------------
SolCallback(0)  = "bsTrough.solOut" ' Trough Eject Coil
SolCallback(10) = "bsSaucer.solOut" ' Saucer Eject Coil
SolCallback(11) = "AutoPlunger" '	' Auto plunger ball saves
SolCallback(12) = "FlippersEnabled" ' Flipper Relay (Hack) to enable flippers

Sub AutoPlunger(Enabled)
  If Enabled Then PlungerIM.AutoFire : End If
End Sub
 ' ----------------------------------------------------------------------------------------
' FLIPPERS - THESE ARE ENABLED THROUGH A RELAY IN GAME
' ----------------------------------------------------------------------------------------
Dim FlippersOn : FlippersOn = 0
Sub FlippersEnabled(Enabled) : FlippersOn = Enabled : End Sub

Sub SolLFlipper(Enabled)
    If Enabled Then
        PlaySoundAt SoundFX("flipperup",DOFFlippers),LeftFlipper:LeftFlipper.RotateToEnd
    Else
        PlaySoundAt SoundFX("flipperdown",DOFFlippers),LeftFlipper:LeftFlipper.RotateToStart
    End If
End Sub
 
Sub SolRFlipper(Enabled)
    If Enabled Then
        PlaySoundAt SoundFX("flipperup",DOFFlippers),RightFlipper:RightFlipper.RotateToEnd
    Else
        PlaySoundAt SoundFX("flipperdown",DOFFlippers),RightFlipper:RightFlipper.RotateToStart
    End If
End Sub
' ----------------------------------------------------------------------------------------
' SLINGSHOTS - SWITCH NUMBERS SET TOP OF SCRIPT
' ----------------------------------------------------------------------------------------
Dim RStep, Lstep
Sub RightSlingShot_Slingshot
	vpmTimer.PulseSw slingR ' slingR top script
    PlaySoundAt "right_slingshot", RightFlipper
    RSling.Visible = 0 : RSling1.Visible = 1
    sling1.rotx = 20 : RStep = 0
    RightSlingShot.TimerEnabled = 1
	gi1.State = 0:Gi2.State = 0
End Sub
Sub RightSlingShot_Timer
    Select Case RStep
        Case 3:RSLing1.Visible = 0:RSLing2.Visible = 1:sling1.rotx = 10
        Case 4:RSLing2.Visible = 0:RSLing.Visible = 1:sling1.rotx = 0:RightSlingShot.TimerEnabled = 0:gi1.State = 1:Gi2.State = 1
    End Select
    RStep = RStep + 1
End Sub
Sub LeftSlingShot_Slingshot
	vpmTimer.PulseSw slingL ' slingR top script
    PlaySoundAt "left_slingshot", LeftFlipper
    LSling.Visible = 0 : LSling1.Visible = 1
    sling2.rotx = 20 : LStep = 0
    LeftSlingShot.TimerEnabled = 1
	gi3.State = 0:Gi4.State = 0
End Sub
Sub LeftSlingShot_Timer
    Select Case LStep
        Case 3:LSLing1.Visible = 0:LSLing2.Visible = 1:sling2.rotx = 10
        Case 4:LSLing2.Visible = 0:LSLing.Visible = 1:sling2.rotx = 0:LeftSlingShot.TimerEnabled = 0:gi3.State = 1:Gi4.State = 1
    End Select
    LStep = LStep + 1
End Sub

' ----------------------------------------------------------------------------------------
' GENERAL ILLUMINATION
' ----------------------------------------------------------------------------------------
dim xx
For each xx in GI:xx.State = 1: Next
' ----------------------------------------------------------------------------------------
' PINGOD P-ROC END
' ----------------------------------------------------------------------------------------

'Table Example scripts
Dim EnableBallControl
EnableBallControl = false 'Change to true to enable manual ball control (or press C in-game) via the arrow keys and B (boost movement) keys

'*********************************************************************
'                 Positional Sound Playback Functions
'*********************************************************************

' Play a sound, depending on the X,Y position of the table element (especially cool for surround speaker setups, otherwise stereo panning only)
' parameters (defaults): loopcount (1), volume (1), randompitch (0), pitch (0), useexisting (0), restart (1))
' Note that this will not work (currently) for walls/slingshots as these do not feature a simple, single X,Y position
Sub PlayXYSound(soundname, tableobj, loopcount, volume, randompitch, pitch, useexisting, restart)
	PlaySound soundname, loopcount, volume, AudioPan(tableobj), randompitch, pitch, useexisting, restart, AudioFade(tableobj)
End Sub

' Similar subroutines that are less complicated to use (e.g. simply use standard parameters for the PlaySound call)
Sub PlaySoundAt(soundname, tableobj)
    PlaySound soundname, 1, 1, AudioPan(tableobj), 0,0,0, 1, AudioFade(tableobj)
End Sub

Sub PlaySoundAtBall(soundname)
    PlaySoundAt soundname, ActiveBall
End Sub


'*********************************************************************
'                     Supporting Ball & Sound Functions
'*********************************************************************

Function AudioFade(tableobj) ' Fades between front and back of the table (for surround systems or 2x2 speakers, etc), depending on the Y position on the table. "table1" is the name of the table
	Dim tmp
    tmp = tableobj.y * 2 / table1.height-1
    If tmp > 0 Then
		AudioFade = Csng(tmp ^10)
    Else
        AudioFade = Csng(-((- tmp) ^10) )
    End If
End Function

Function AudioPan(tableobj) ' Calculates the pan for a tableobj based on the X position on the table. "table1" is the name of the table
    Dim tmp
    tmp = tableobj.x * 2 / table1.width-1
    If tmp > 0 Then
        AudioPan = Csng(tmp ^10)
    Else
        AudioPan = Csng(-((- tmp) ^10) )
    End If
End Function

Function Vol(ball) ' Calculates the Volume of the sound based on the ball speed
    Vol = Csng(BallVel(ball) ^2 / 2000)
End Function

Function Pitch(ball) ' Calculates the pitch of the sound based on the ball speed
    Pitch = BallVel(ball) * 20
End Function

Function BallVel(ball) 'Calculates the ball speed
    BallVel = INT(SQR((ball.VelX ^2) + (ball.VelY ^2) ) )
End Function


'********************************************************************
'      JP's VP10 Rolling Sounds (+rothbauerw's Dropping Sounds)
'********************************************************************

Const tnob = 5 ' total number of balls
ReDim rolling(tnob)
InitRolling

Sub InitRolling
    Dim i
    For i = 0 to tnob
        rolling(i) = False
    Next
End Sub

Sub RollingTimer_Timer()
    Dim BOT, b
    BOT = GetBalls

    ' stop the sound of deleted balls
    For b = UBound(BOT) + 1 to tnob
        rolling(b) = False
        StopSound("fx_ballrolling" & b)
    Next

    ' exit the sub if no balls on the table
    If UBound(BOT) = -1 Then Exit Sub

    For b = 0 to UBound(BOT)
        ' play the rolling sound for each ball
        If BallVel(BOT(b) ) > 1 AND BOT(b).z < 30 Then
            rolling(b) = True
            PlaySound("fx_ballrolling" & b), -1, Vol(BOT(b)), AudioPan(BOT(b)), 0, Pitch(BOT(b)), 1, 0, AudioFade(BOT(b))
        Else
            If rolling(b) = True Then
                StopSound("fx_ballrolling" & b)
                rolling(b) = False
            End If
        End If

        ' play ball drop sounds
        If BOT(b).VelZ < -1 and BOT(b).z < 55 and BOT(b).z > 27 Then 'height adjust for ball drop sounds
            PlaySound "fx_ball_drop" & b, 0, ABS(BOT(b).velz)/17, AudioPan(BOT(b)), 0, Pitch(BOT(b)), 1, 0, AudioFade(BOT(b))
        End If
    Next
End Sub

'**********************
' Ball Collision Sound
'**********************

Sub OnBallBallCollision(ball1, ball2, velocity)
	PlaySound("fx_collide"), 0, Csng(velocity) ^2 / 2000, AudioPan(ball1), 0, Pitch(ball1), 0, 0, AudioFade(ball1)
End Sub

Sub Pins_Hit (idx)
	PlaySound "pinhit_low", 0, Vol(ActiveBall), AudioPan(ActiveBall), 0, Pitch(ActiveBall), 0, 0, AudioFade(ActiveBall)
End Sub

Sub Targets_Hit (idx)
	PlaySound "target", 0, Vol(ActiveBall), AudioPan(ActiveBall), 0, Pitch(ActiveBall), 0, 0, AudioFade(ActiveBall)
End Sub

Sub Metals_Thin_Hit (idx)
	PlaySound "metalhit_thin", 0, Vol(ActiveBall), AudioPan(ActiveBall), 0, Pitch(ActiveBall), 1, 0, AudioFade(ActiveBall)
End Sub

Sub Metals_Medium_Hit (idx)
	PlaySound "metalhit_medium", 0, Vol(ActiveBall), AudioPan(ActiveBall), 0, Pitch(ActiveBall), 1, 0, AudioFade(ActiveBall)
End Sub

Sub Metals2_Hit (idx)
	PlaySound "metalhit2", 0, Vol(ActiveBall), AudioPan(ActiveBall), 0, Pitch(ActiveBall), 1, 0, AudioFade(ActiveBall)
End Sub

Sub Gates_Hit (idx)
	PlaySound "gate4", 0, Vol(ActiveBall), AudioPan(ActiveBall), 0, Pitch(ActiveBall), 1, 0, AudioFade(ActiveBall)
End Sub

Sub Spinner_Spin
	PlaySound "fx_spinner", 0, .25, AudioPan(Spinner), 0.25, 0, 0, 1, AudioFade(Spinner)
End Sub

Sub Rubbers_Hit(idx)
 	dim finalspeed
  	finalspeed=SQR(activeball.velx * activeball.velx + activeball.vely * activeball.vely)
 	If finalspeed > 20 then 
		PlaySound "fx_rubber2", 0, Vol(ActiveBall), AudioPan(ActiveBall), 0, Pitch(ActiveBall), 1, 0, AudioFade(ActiveBall)
	End if
	If finalspeed >= 6 AND finalspeed <= 20 then
 		RandomSoundRubber()
 	End If
End Sub

Sub Posts_Hit(idx)
 	dim finalspeed
  	finalspeed=SQR(activeball.velx * activeball.velx + activeball.vely * activeball.vely)
 	If finalspeed > 16 then 
		PlaySound "fx_rubber2", 0, Vol(ActiveBall), AudioPan(ActiveBall), 0, Pitch(ActiveBall), 1, 0, AudioFade(ActiveBall)
	End if
	If finalspeed >= 6 AND finalspeed <= 16 then
 		RandomSoundRubber()
 	End If
End Sub

Sub RandomSoundRubber()
	Select Case Int(Rnd*3)+1
		Case 1 : PlaySound "rubber_hit_1", 0, Vol(ActiveBall), AudioPan(ActiveBall), 0, Pitch(ActiveBall), 1, 0, AudioFade(ActiveBall)
		Case 2 : PlaySound "rubber_hit_2", 0, Vol(ActiveBall), AudioPan(ActiveBall), 0, Pitch(ActiveBall), 1, 0, AudioFade(ActiveBall)
		Case 3 : PlaySound "rubber_hit_3", 0, Vol(ActiveBall), AudioPan(ActiveBall), 0, Pitch(ActiveBall), 1, 0, AudioFade(ActiveBall)
	End Select
End Sub

Sub LeftFlipper_Collide(parm)
 	RandomSoundFlipper()
End Sub

Sub RightFlipper_Collide(parm)
 	RandomSoundFlipper()
End Sub

Sub RandomSoundFlipper()
	Select Case Int(Rnd*3)+1
		Case 1 : PlaySound "flip_hit_1", 0, Vol(ActiveBall), AudioPan(ActiveBall), 0, Pitch(ActiveBall), 1, 0, AudioFade(ActiveBall)
		Case 2 : PlaySound "flip_hit_2", 0, Vol(ActiveBall), AudioPan(ActiveBall), 0, Pitch(ActiveBall), 1, 0, AudioFade(ActiveBall)
		Case 3 : PlaySound "flip_hit_3", 0, Vol(ActiveBall), AudioPan(ActiveBall), 0, Pitch(ActiveBall), 1, 0, AudioFade(ActiveBall)
	End Select
End Sub
